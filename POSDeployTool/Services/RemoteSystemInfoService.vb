Imports System.Globalization
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Threading.Tasks
Imports POSDeployTool.Models

Namespace Services
    Public Class RemoteSystemInfo
        Public Property ComputerName As String = String.Empty
        Public Property WindowsOS As String = String.Empty
        Public Property OSVersion As String = String.Empty
        Public Property OSBuild As String = String.Empty
        Public Property OSArchitecture As String = String.Empty
        Public Property AppVersion As String = String.Empty
    End Class

    Public Class RemoteSystemInfoService
        Private ReadOnly _remote As New RemoteCommandService()

        Public Async Function GetSystemInfoAsync(store As StoreInfo,
                                                 executableName As String,
                                                 timeoutMilliseconds As Integer,
                                                 token As CancellationToken) As Task(Of CheckResult)
            Dim command = "cmd /c wmic os get Caption,Version,BuildNumber,OSArchitecture /value"
            Dim osResult = Await _remote.ExecuteFullAsync(store, command, timeoutMilliseconds, token)
            If Not osResult.IsSuccess Then Return CheckResult.Fail("Windows OS check failed: " & osResult.Message)

            Dim info As New RemoteSystemInfo With {
                .ComputerName = Await GetComputerNameAsync(store, timeoutMilliseconds, token),
                .WindowsOS = GetWmiValue(osResult.Message, "Caption"),
                .OSVersion = GetWmiValue(osResult.Message, "Version"),
                .OSBuild = GetWmiValue(osResult.Message, "BuildNumber"),
                .OSArchitecture = GetWmiValue(osResult.Message, "OSArchitecture")
            }

            If String.IsNullOrWhiteSpace(info.OSVersion) Then
                Return CheckResult.Fail("Cannot parse Windows OS information: " & OneLine(osResult.Message))
            End If

            Dim versionResult = Await GetApplicationVersionAsync(store, executableName, info.OSVersion, timeoutMilliseconds, token)
            If Not versionResult.IsSuccess Then Return CheckResult.Fail(versionResult.Message)
            info.AppVersion = versionResult.Message

            Dim payload = String.Join("|", {
                SafePipe(info.ComputerName), SafePipe(info.WindowsOS), SafePipe(info.OSVersion),
                SafePipe(info.OSBuild), SafePipe(info.OSArchitecture), SafePipe(info.AppVersion)
            })
            Return CheckResult.Success(payload)
        End Function

        Public Async Function GetApplicationVersionAsync(store As StoreInfo,
                                                          executableName As String,
                                                          knownOSVersion As String,
                                                          timeoutMilliseconds As Integer,
                                                          token As CancellationToken) As Task(Of CheckResult)
            Dim target = If(String.IsNullOrWhiteSpace(store.TargetPath), "C:\BJCBCPOS", store.TargetPath.Trim())
            Dim exe = If(String.IsNullOrWhiteSpace(executableName), "BJCBCPOS.exe", executableName.Trim())
            Dim fullPath = target.TrimEnd("\"c) & "\" & exe
            Dim osVersion = If(knownOSVersion, String.Empty).Trim()

            If String.IsNullOrWhiteSpace(osVersion) Then
                Dim os = Await _remote.ExecuteFullAsync(store, "cmd /c wmic os get Version /value", timeoutMilliseconds, token)
                If Not os.IsSuccess Then Return CheckResult.Fail("Cannot detect Windows version: " & os.Message)
                osVersion = GetWmiValue(os.Message, "Version")
            End If

            If osVersion.StartsWith("10.", StringComparison.OrdinalIgnoreCase) Then
                Dim escaped = fullPath.Replace("'", "''")
                Dim ps = "powershell -NoProfile -ExecutionPolicy Bypass -Command ""$p='" & escaped & "';if(Test-Path -LiteralPath $p){$v=(Get-Item -LiteralPath $p).VersionInfo.ProductVersion;if([string]::IsNullOrWhiteSpace($v)){$v=(Get-Item -LiteralPath $p).VersionInfo.FileVersion};if([string]::IsNullOrWhiteSpace($v)){'Unknown'}else{$v}}else{'Not found'}"""
                Dim result = Await _remote.ExecuteFullAsync(store, ps, timeoutMilliseconds, token)
                If Not result.IsSuccess Then Return CheckResult.Fail("PowerShell version check failed: " & result.Message)
                Return CheckResult.Success(FirstUsefulLine(result.Message))
            End If

            ' Windows 7 / Windows 6.x: use WMI CIM_DataFile, compatible with PowerShell 2.0 machines.
            Dim wmiPath = fullPath.Replace("\", "\\").Replace("'", "''")
            Dim wmiCommand = "cmd /c wmic datafile where ""name='" & wmiPath & "'"" get Version /value"
            Dim wmiResult = Await _remote.ExecuteFullAsync(store, wmiCommand, timeoutMilliseconds, token)
            If Not wmiResult.IsSuccess Then Return CheckResult.Fail("WMI version check failed: " & wmiResult.Message)
            Dim version = GetWmiValue(wmiResult.Message, "Version")
            If String.IsNullOrWhiteSpace(version) Then version = "Not found"
            Return CheckResult.Success(version)
        End Function

        Private Async Function GetComputerNameAsync(store As StoreInfo, timeoutMilliseconds As Integer, token As CancellationToken) As Task(Of String)
            Dim result = Await _remote.ExecuteFullAsync(store, "cmd /c hostname", timeoutMilliseconds, token)
            If result.IsSuccess Then Return FirstUsefulLine(result.Message)
            Return String.Empty
        End Function

        Public Shared Function ParsePayload(payload As String) As RemoteSystemInfo
            Dim parts = If(payload, String.Empty).Split("|"c)
            Dim info As New RemoteSystemInfo()
            If parts.Length > 0 Then info.ComputerName = parts(0)
            If parts.Length > 1 Then info.WindowsOS = parts(1)
            If parts.Length > 2 Then info.OSVersion = parts(2)
            If parts.Length > 3 Then info.OSBuild = parts(3)
            If parts.Length > 4 Then info.OSArchitecture = parts(4)
            If parts.Length > 5 Then info.AppVersion = parts(5)
            Return info
        End Function

        Private Shared Function GetWmiValue(text As String, key As String) As String
            If String.IsNullOrWhiteSpace(text) Then Return String.Empty
            Dim match = Regex.Match(text, "(?im)^\s*" & Regex.Escape(key) & "\s*=\s*(.*?)\s*$")
            If match.Success Then Return match.Groups(1).Value.Trim()
            Return String.Empty
        End Function

        Private Shared Function FirstUsefulLine(text As String) As String
            If String.IsNullOrWhiteSpace(text) Then Return "Unknown"
            For Each line In text.Replace(ControlChars.Cr, String.Empty).Split(ControlChars.Lf)
                Dim value = line.Trim()
                If value.Length > 0 AndAlso Not value.StartsWith("#< CLIXML", StringComparison.OrdinalIgnoreCase) Then Return value
            Next
            Return "Unknown"
        End Function

        Private Shared Function OneLine(text As String) As String
            Return Regex.Replace(If(text, String.Empty), "\s+", " ").Trim()
        End Function

        Private Shared Function SafePipe(value As String) As String
            Return If(value, String.Empty).Replace("|", "/").Replace(ControlChars.Cr, " ").Replace(ControlChars.Lf, " ").Trim()
        End Function
    End Class
End Namespace
