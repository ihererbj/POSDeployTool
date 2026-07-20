Imports System.Globalization
Imports System.Threading
Imports System.Threading.Tasks
Imports POSDeployTool.Models

Namespace Services
    Public Class HealthCheckService
        Private ReadOnly _remote As New RemoteCommandService()

        Public Async Function CheckAsync(store As StoreInfo,
                                         processName As String,
                                         timeoutMilliseconds As Integer,
                                         cancellationToken As CancellationToken) As Task(Of CheckResult)
            Dim imageName As String = If(processName, "BJCBCPOS").Trim()
            If Not imageName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) Then imageName &= ".exe"

            Dim command As String =
                "cmd /q /d /c " &
                "echo [OS] & wmic os get Caption,Version,BuildNumber,OSArchitecture,FreePhysicalMemory,TotalVisibleMemorySize,LastBootUpTime /value " &
                "& echo [CPU] & wmic cpu get Name,LoadPercentage /value " &
                "& echo [DISK] & wmic logicaldisk where ""DeviceID='C:'"" get Size,FreeSpace /value " &
                "& echo [PROCESS] & tasklist /FI ""IMAGENAME eq " & imageName.Replace("""", "") & """ /NH " &
                "& echo [FIREWALL] & netsh advfirewall show allprofiles state"

            Dim result As CheckResult = Await _remote.ExecuteFullAsync(store, command, timeoutMilliseconds, cancellationToken)
            If Not result.IsSuccess Then Return result

            Try
                Dim info As HealthInfo = Parse(result.Message, imageName)
                Return CheckResult.Success(Serialize(info))
            Catch ex As Exception
                Return CheckResult.Fail("Health parse failed: " & ex.Message)
            End Try
        End Function

        Public Shared Function Parse(payload As String) As HealthInfo
            Dim parts() As String = If(payload, String.Empty).Split("|"c)
            If parts.Length < 12 OrElse parts(0) <> "HEALTH" Then Throw New FormatException("Invalid health payload")
            Return New HealthInfo With {
                .CpuName = parts(1),
                .CpuLoadPercent = ToInt(parts(2), -1),
                .TotalRamGb = ToDouble(parts(3)),
                .FreeRamGb = ToDouble(parts(4)),
                .TotalDiskGb = ToDouble(parts(5)),
                .FreeDiskGb = ToDouble(parts(6)),
                .LastBoot = parts(7),
                .Uptime = parts(8),
                .FirewallStatus = parts(9),
                .PosProcessStatus = parts(10),
                .HealthScore = ToInt(parts(11), 0),
                .HealthStatus = If(parts.Length > 12, parts(12), String.Empty)
            }
        End Function

        Private Shared Function Parse(text As String, imageName As String) As HealthInfo
            Dim section As String = String.Empty
            Dim values As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            Dim cpuNames As New List(Of String)()
            Dim cpuLoads As New List(Of Integer)()
            Dim processRunning As Boolean = False
            Dim firewallOn As Boolean = False
            Dim firewallOff As Boolean = False

            For Each raw As String In If(text, String.Empty).Split({ControlChars.Cr, ControlChars.Lf}, StringSplitOptions.RemoveEmptyEntries)
                Dim line As String = raw.Trim()
                If line.StartsWith("[") AndAlso line.EndsWith("]") Then section = line : Continue For
                If section = "[PROCESS]" AndAlso line.IndexOf(imageName, StringComparison.OrdinalIgnoreCase) >= 0 Then processRunning = True
                If section = "[FIREWALL]" Then
                    If line.IndexOf("ON", StringComparison.OrdinalIgnoreCase) >= 0 Then firewallOn = True
                    If line.IndexOf("OFF", StringComparison.OrdinalIgnoreCase) >= 0 Then firewallOff = True
                End If
                Dim p As Integer = line.IndexOf("="c)
                If p > 0 Then
                    Dim key As String = line.Substring(0, p).Trim()
                    Dim val As String = line.Substring(p + 1).Trim()
                    If section = "[CPU]" AndAlso key.Equals("Name", StringComparison.OrdinalIgnoreCase) Then cpuNames.Add(val)
                    If section = "[CPU]" AndAlso key.Equals("LoadPercentage", StringComparison.OrdinalIgnoreCase) Then cpuLoads.Add(ToInt(val, -1))
                    values(section & key) = val
                End If
            Next

            Dim totalRam As Double = ToDouble(GetValue(values, "[OS]TotalVisibleMemorySize")) / 1024.0 / 1024.0
            Dim freeRam As Double = ToDouble(GetValue(values, "[OS]FreePhysicalMemory")) / 1024.0 / 1024.0
            Dim totalDisk As Double = ToDouble(GetValue(values, "[DISK]Size")) / 1024.0 / 1024.0 / 1024.0
            Dim freeDisk As Double = ToDouble(GetValue(values, "[DISK]FreeSpace")) / 1024.0 / 1024.0 / 1024.0
            Dim cpuLoad As Integer = If(cpuLoads.Count = 0, -1, CInt(cpuLoads.Where(Function(x) x >= 0).DefaultIfEmpty(-1).Average()))
            Dim bootRaw As String = GetValue(values, "[OS]LastBootUpTime")
            Dim bootDate As DateTime = ParseWmiDate(bootRaw)
            Dim uptimeText As String = If(bootDate = DateTime.MinValue, String.Empty, FormatUptime(DateTime.Now - bootDate))
            Dim fw As String = If(firewallOn AndAlso firewallOff, "Mixed", If(firewallOn, "ON", If(firewallOff, "OFF", "Unknown")))
            Dim processStatus As String = If(processRunning, "Running", "Not running")

            Dim score As Integer = 100
            If cpuLoad >= 80 Then
                score -= 25
            ElseIf cpuLoad >= 60 Then
                score -= 10
            End If
            Dim freeRamPct As Double = If(totalRam <= 0, 0, freeRam / totalRam * 100)
            If totalRam > 0 AndAlso freeRamPct < 10 Then
                score -= 25
            ElseIf totalRam > 0 AndAlso freeRamPct < 20 Then
                score -= 10
            End If
            If freeDisk < 5 Then
                score -= 30
            ElseIf freeDisk < 10 Then
                score -= 15
            ElseIf freeDisk < 20 Then
                score -= 5
            End If
            If Not processRunning Then score -= 20
            If score < 0 Then score = 0
            Dim status As String = If(score >= 80, "Healthy", If(score >= 50, "Warning", "Critical"))

            Return New HealthInfo With {
                .CpuName = String.Join(" / ", cpuNames.Distinct()), .CpuLoadPercent = cpuLoad,
                .TotalRamGb = Math.Round(totalRam, 2), .FreeRamGb = Math.Round(freeRam, 2),
                .TotalDiskGb = Math.Round(totalDisk, 2), .FreeDiskGb = Math.Round(freeDisk, 2),
                .LastBoot = If(bootDate = DateTime.MinValue, bootRaw, bootDate.ToString("yyyy-MM-dd HH:mm:ss")),
                .Uptime = uptimeText, .FirewallStatus = fw, .PosProcessStatus = processStatus,
                .HealthScore = score, .HealthStatus = status
            }
        End Function

        Private Shared Function Serialize(i As HealthInfo) As String
            Return String.Join("|", New String() {"HEALTH", Safe(i.CpuName), i.CpuLoadPercent.ToString(), i.TotalRamGb.ToString(CultureInfo.InvariantCulture), i.FreeRamGb.ToString(CultureInfo.InvariantCulture), i.TotalDiskGb.ToString(CultureInfo.InvariantCulture), i.FreeDiskGb.ToString(CultureInfo.InvariantCulture), Safe(i.LastBoot), Safe(i.Uptime), Safe(i.FirewallStatus), Safe(i.PosProcessStatus), i.HealthScore.ToString(), Safe(i.HealthStatus)})
        End Function
        Private Shared Function Safe(v As String) As String
            Return If(v, String.Empty) _
                .Replace("|", "/") _
                .Replace(ControlChars.Cr, " ") _
                .Replace(ControlChars.Lf, " ")
        End Function

        Private Shared Function GetValue(d As Dictionary(Of String, String), k As String) As String
            Dim v As String = Nothing

            If d IsNot Nothing AndAlso d.TryGetValue(k, v) Then
                Return v
            End If

            Return String.Empty
        End Function

        Private Shared Function ToInt(v As String, fallback As Integer) As Integer
            Dim n As Integer

            If Integer.TryParse(v, n) Then
                Return n
            End If

            Return fallback
        End Function

        Private Shared Function ToDouble(v As String) As Double
            Dim n As Double

            If Double.TryParse(v, NumberStyles.Any, CultureInfo.InvariantCulture, n) Then
                Return n
            End If

            If Double.TryParse(v, n) Then
                Return n
            End If

            Return 0
        End Function
        Private Shared Function ParseWmiDate(v As String) As DateTime
            If String.IsNullOrWhiteSpace(v) OrElse v.Length < 14 Then Return DateTime.MinValue
            Dim d As DateTime
            If DateTime.TryParseExact(v.Substring(0, 14), "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, d) Then Return d
            Return DateTime.MinValue
        End Function
        Private Shared Function FormatUptime(span As TimeSpan) As String
            If span.TotalDays >= 1 Then Return String.Format("{0}d {1}h", CInt(Math.Floor(span.TotalDays)), span.Hours)
            Return String.Format("{0}h {1}m", CInt(Math.Floor(span.TotalHours)), span.Minutes)
        End Function
    End Class
End Namespace
