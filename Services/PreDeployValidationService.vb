Imports System.Globalization
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Threading.Tasks
Imports POSDeployTool.Models

Namespace Services
    Public Class PreDeployValidationService
        Private ReadOnly _connection As New ConnectionCheckService()
        Private ReadOnly _remote As New RemoteCommandService()
        Private ReadOnly _remoteInfo As New RemoteSystemInfoService()
        Private ReadOnly _retry As New RetryService()
        Private ReadOnly _version As New VersionService()

        Public Async Function ValidateAsync(store As StoreInfo,
                                            executableName As String,
                                            releasePath As String,
                                            minimumFreeGb As Decimal,
                                            retryCount As Integer,
                                            token As CancellationToken,
                                            Optional onRetry As Action(Of Integer, String) = Nothing) As Task(Of PreDeployValidationResult)
            Dim result As New PreDeployValidationResult With {
                .ReleaseVersion = _version.GetReleaseVersion(releasePath, executableName),
                .ComputerName = store.ComputerName,
                .WindowsOS = store.WindowsOS,
                .OSVersion = store.OSVersion,
                .OSBuild = store.OSBuild,
                .OSArchitecture = store.OSArchitecture
            }

            Dim ping = Await _retry.ExecuteAsync("Ping", Function() _connection.CheckPingAsync(store.IpAddress, 2000, token), retryCount, 1000, token, onRetry)
            If Not ping.IsSuccess Then Return Fail(result, "Ping failed: " & ping.Message)

            Dim winrm = Await _retry.ExecuteAsync("WinRM", Function() _connection.CheckWinRmAsync(store, 10000, token), retryCount, 1500, token, onRetry)
            If Not winrm.IsSuccess Then Return Fail(result, "WinRM failed: " & winrm.Message)
            If String.IsNullOrWhiteSpace(result.ComputerName) Then result.ComputerName = winrm.Message

            ' Version selection is OS-aware:
            ' Windows 10 = PowerShell FileVersionInfo; Windows 7/6.x = WMI CIM_DataFile.
            Dim appVersion = Await _retry.ExecuteAsync(
                "BJCBCPOS version",
                Function() _remoteInfo.GetApplicationVersionAsync(store, executableName, store.OSVersion, 30000, token),
                retryCount, 1500, token, onRetry)
            If Not appVersion.IsSuccess Then Return Fail(result, "Version check failed: " & appVersion.Message)
            result.CurrentVersion = appVersion.Message

            Dim disk = Await _retry.ExecuteAsync(
                "Free disk",
                Function() _remote.ExecuteFullAsync(store, "cmd /c wmic logicaldisk where ""DeviceID='C:'"" get FreeSpace /value", 20000, token),
                retryCount, 1000, token, onRetry)
            If Not disk.IsSuccess Then Return Fail(result, "Disk check failed: " & disk.Message)

            Dim match = Regex.Match(disk.Message, "(?im)^\s*FreeSpace\s*=\s*(\d+)\s*$")
            If Not match.Success Then Return Fail(result, "Invalid disk information: " & disk.Message)
            Dim bytes As Decimal
            If Not Decimal.TryParse(match.Groups(1).Value, NumberStyles.Any, CultureInfo.InvariantCulture, bytes) Then Return Fail(result, "Invalid FreeSpace value")
            Dim free = Math.Round(bytes / 1073741824D, 2)
            result.FreeDiskGb = free.ToString("0.00", CultureInfo.InvariantCulture)
            If free < minimumFreeGb Then Return Fail(result, String.Format("Disk space insufficient: {0} GB", result.FreeDiskGb))

            If String.Equals(result.ReleaseVersion, "Not found", StringComparison.OrdinalIgnoreCase) Then
                Return Fail(result, "Release executable not found: " & executableName)
            End If

            result.IsSuccess = True
            result.Message = If(String.Equals(result.CurrentVersion, result.ReleaseVersion, StringComparison.OrdinalIgnoreCase),
                                "Ready to deploy (same version)", "Ready to deploy")
            Return result
        End Function

        Private Shared Function Fail(result As PreDeployValidationResult, message As String) As PreDeployValidationResult
            result.IsSuccess = False
            result.Message = message
            Return result
        End Function
    End Class
End Namespace
