Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Text
Imports POSDeployTool.Models

Namespace Services

    Public Class RemoteToolsService

        Private Const BjcTaskName As String = "POSDeploy_BJCBCPOS"
        Private ReadOnly _remoteCommandService As New RemoteCommandService()

        Public Function CheckProcessAsync(store As StoreInfo,
                                          processName As String,
                                          timeoutMilliseconds As Integer,
                                          cancellationToken As CancellationToken) As Task(Of CheckResult)
            Dim imageName As String = NormalizeProcessImageName(processName)
            Dim command As String = String.Format(
                "cmd /c tasklist /FI ""IMAGENAME eq {0}"" /NH",
                imageName
            )
            Return _remoteCommandService.ExecuteFullAsync(store, command, timeoutMilliseconds, cancellationToken)
        End Function

        Public Function StopProcessAsync(store As StoreInfo,
                                         processName As String,
                                         timeoutMilliseconds As Integer,
                                         cancellationToken As CancellationToken) As Task(Of CheckResult)
            Dim imageName As String = NormalizeProcessImageName(processName)
            Dim command As String = String.Format(
                "cmd /c taskkill /F /IM {0}",
                QuoteArgument(imageName)
            )
            Return _remoteCommandService.ExecuteFullAsync(store, command, timeoutMilliseconds, cancellationToken)
        End Function

        Public Function StartProcessAsync(store As StoreInfo,
                                          processName As String,
                                          timeoutMilliseconds As Integer,
                                          cancellationToken As CancellationToken) As Task(Of CheckResult)
            Dim imageName As String = NormalizeProcessImageName(processName)

            If imageName.Equals("BJCBCPOS.exe", StringComparison.OrdinalIgnoreCase) Then
                Return RunBjcTaskAsync(store, timeoutMilliseconds, cancellationToken)
            End If

            Dim executablePath As String = BuildExecutablePath(store, processName)
            Dim command As String = String.Format(
                "cmd /c if exist {0} (start """" {0}) else (echo File not found: {1} & exit /b 2)",
                QuoteArgument(executablePath),
                executablePath
            )
            Return _remoteCommandService.ExecuteFullAsync(store, command, timeoutMilliseconds, cancellationToken)
        End Function

        Public Async Function RestartProcessAsync(store As StoreInfo,
                                                  processName As String,
                                                  timeoutMilliseconds As Integer,
                                                  cancellationToken As CancellationToken) As Task(Of CheckResult)
            Await StopProcessAsync(store, processName, timeoutMilliseconds, cancellationToken)
            Await Task.Delay(1500, cancellationToken)

            Dim startResult As CheckResult = Await StartProcessAsync(
                store, processName, timeoutMilliseconds, cancellationToken)

            If startResult.IsSuccess Then
                Return CheckResult.Success("Process restarted: " & NormalizeProcessImageName(processName) &
                                           " via Scheduled Task")
            End If

            Return CheckResult.Fail("Start failed: " & startResult.Message)
        End Function

        Public Function InstallBjcTaskAsync(store As StoreInfo,
                                            timeoutMilliseconds As Integer,
                                            cancellationToken As CancellationToken) As Task(Of CheckResult)
            Dim targetPath As String = GetTargetPath(store)
            Dim executablePath As String = Path.Combine(targetPath, "BJCBCPOS.exe")
            Dim script As String = BuildInstallTaskPowerShell(executablePath, targetPath)
            Dim encoded As String = Convert.ToBase64String(Encoding.Unicode.GetBytes(script))
            Dim command As String = "powershell.exe -NoProfile -ExecutionPolicy Bypass -EncodedCommand " & encoded

            Return _remoteCommandService.ExecuteFullAsync(
                store, command, timeoutMilliseconds, cancellationToken)
        End Function

        Public Function RemoveBjcTaskAsync(store As StoreInfo,
                                           timeoutMilliseconds As Integer,
                                           cancellationToken As CancellationToken) As Task(Of CheckResult)
            Dim command As String = String.Format(
                "cmd /c """"%WINDIR%\System32\schtasks.exe"" /Delete /TN ""{0}"" /F""",
                BjcTaskName)

            Return _remoteCommandService.ExecuteFullAsync(
                store, command, timeoutMilliseconds, cancellationToken)
        End Function

        Private Function RunBjcTaskAsync(store As StoreInfo,
                                         timeoutMilliseconds As Integer,
                                         cancellationToken As CancellationToken) As Task(Of CheckResult)
            Dim command As String = String.Format(
                "cmd /c """"%WINDIR%\System32\schtasks.exe"" /Run /TN ""{0}"" && timeout /t 5 /nobreak >nul && tasklist /FI ""IMAGENAME eq BJCBCPOS.exe"" /NH | find /I ""BJCBCPOS.exe"" >nul""",
                BjcTaskName)

            Return _remoteCommandService.ExecuteFullAsync(
                store, command, timeoutMilliseconds, cancellationToken)
        End Function

        Public Function RestartWindowsAsync(store As StoreInfo,
                                            timeoutMilliseconds As Integer,
                                            cancellationToken As CancellationToken) As Task(Of CheckResult)
            Dim command As String = "cmd /c shutdown /r /f /t 5 /c ""POSDeployTool remote restart"""
            Return _remoteCommandService.ExecuteFullAsync(store, command, timeoutMilliseconds, cancellationToken)
        End Function

        Public Function CancelShutdownAsync(store As StoreInfo,
                                            timeoutMilliseconds As Integer,
                                            cancellationToken As CancellationToken) As Task(Of CheckResult)
            Return _remoteCommandService.ExecuteFullAsync(
                store, "cmd /c shutdown /a", timeoutMilliseconds, cancellationToken)
        End Function

        Public Function StartServiceAsync(store As StoreInfo,
                                          serviceName As String,
                                          timeoutMilliseconds As Integer,
                                          cancellationToken As CancellationToken) As Task(Of CheckResult)
            Dim safeServiceName As String = SanitizeName(serviceName)
            If String.IsNullOrWhiteSpace(safeServiceName) Then
                Return Task.FromResult(CheckResult.Fail("Service name is empty"))
            End If

            Dim command As String = String.Format("cmd /c net start ""{0}""", safeServiceName)
            Return _remoteCommandService.ExecuteFullAsync(store, command, timeoutMilliseconds, cancellationToken)
        End Function

        Public Function StopServiceAsync(store As StoreInfo,
                                         serviceName As String,
                                         timeoutMilliseconds As Integer,
                                         cancellationToken As CancellationToken) As Task(Of CheckResult)
            Dim safeServiceName As String = SanitizeName(serviceName)
            If String.IsNullOrWhiteSpace(safeServiceName) Then
                Return Task.FromResult(CheckResult.Fail("Service name is empty"))
            End If

            Dim command As String = String.Format("cmd /c net stop ""{0}"" /y", safeServiceName)
            Return _remoteCommandService.ExecuteFullAsync(store, command, timeoutMilliseconds, cancellationToken)
        End Function

        Public Function RestartServiceAsync(store As StoreInfo,
                                            serviceName As String,
                                            timeoutMilliseconds As Integer,
                                            cancellationToken As CancellationToken) As Task(Of CheckResult)
            Dim safeServiceName As String = SanitizeName(serviceName)
            If String.IsNullOrWhiteSpace(safeServiceName) Then
                Return Task.FromResult(CheckResult.Fail("Service name is empty"))
            End If

            Dim command As String = String.Format(
                "cmd /c net stop ""{0}"" /y & net start ""{0}""",
                safeServiceName
            )
            Return _remoteCommandService.ExecuteFullAsync(store, command, timeoutMilliseconds, cancellationToken)
        End Function

        Public Function GetSystemHealthAsync(store As StoreInfo,
                                             timeoutMilliseconds As Integer,
                                             cancellationToken As CancellationToken) As Task(Of CheckResult)
            ' WMIC works on the Windows 7 POS estate and remains available on the targeted Windows 10 builds.
            Dim command As String =
                "cmd /c echo ===== COMPUTER ===== & hostname" &
                " & echo ===== OPERATING SYSTEM =====" &
                " & wmic os get Caption,Version,BuildNumber,OSArchitecture,LastBootUpTime /value" &
                " & echo ===== MEMORY KB =====" &
                " & wmic os get FreePhysicalMemory,TotalVisibleMemorySize /value" &
                " & echo ===== CPU =====" &
                " & wmic cpu get Name,LoadPercentage /value" &
                " & echo ===== DISK C =====" &
                " & wmic logicaldisk where ""DeviceID='C:'"" get FreeSpace,Size /value"

            Return _remoteCommandService.ExecuteFullAsync(store, command, timeoutMilliseconds, cancellationToken)
        End Function

        Public Function GetNetworkInfoAsync(store As StoreInfo,
                                            timeoutMilliseconds As Integer,
                                            cancellationToken As CancellationToken) As Task(Of CheckResult)
            Return _remoteCommandService.ExecuteFullAsync(
                store, "cmd /c ipconfig /all", timeoutMilliseconds, cancellationToken)
        End Function

        Public Function ListProcessesAsync(store As StoreInfo,
                                           timeoutMilliseconds As Integer,
                                           cancellationToken As CancellationToken) As Task(Of CheckResult)
            Return _remoteCommandService.ExecuteFullAsync(
                store, "cmd /c tasklist /FO TABLE", timeoutMilliseconds, cancellationToken)
        End Function

        Public Function ListServicesAsync(store As StoreInfo,
                                          timeoutMilliseconds As Integer,
                                          cancellationToken As CancellationToken) As Task(Of CheckResult)
            Return _remoteCommandService.ExecuteFullAsync(
                store, "cmd /c sc query state= all", timeoutMilliseconds, cancellationToken)
        End Function

        Public Function KillProcessByNameAsync(store As StoreInfo,
                                               processName As String,
                                               timeoutMilliseconds As Integer,
                                               cancellationToken As CancellationToken) As Task(Of CheckResult)
            Dim imageName As String = NormalizeProcessImageName(processName)
            Dim command As String = String.Format("cmd /c taskkill /F /IM {0}", QuoteArgument(imageName))
            Return _remoteCommandService.ExecuteFullAsync(store, command, timeoutMilliseconds, cancellationToken)
        End Function

        Public Function ExecuteCustomCommandAsync(store As StoreInfo,
                                                  commandText As String,
                                                  timeoutMilliseconds As Integer,
                                                  cancellationToken As CancellationToken) As Task(Of CheckResult)
            If String.IsNullOrWhiteSpace(commandText) Then
                Return Task.FromResult(CheckResult.Fail("Command is empty"))
            End If

            Dim command As String = commandText.Trim()
            If Not command.StartsWith("cmd ", StringComparison.OrdinalIgnoreCase) AndAlso
               Not command.StartsWith("powershell ", StringComparison.OrdinalIgnoreCase) Then
                command = "cmd /c " & command
            End If

            Return _remoteCommandService.ExecuteFullAsync(store, command, timeoutMilliseconds, cancellationToken)
        End Function

        Private Shared Function GetTargetPath(store As StoreInfo) As String
            Dim targetPath As String = String.Empty

            If store IsNot Nothing AndAlso store.TargetPath IsNot Nothing Then
                targetPath = store.TargetPath.Trim()
            End If

            If String.IsNullOrWhiteSpace(targetPath) Then
                targetPath = "C:\BJCBCPOS"
            End If

            Return targetPath.TrimEnd("\"c)
        End Function

        Private Shared Function BuildInstallTaskPowerShell(executablePath As String,
                                                             workingDirectory As String) As String
            Dim safeExe As String = EscapePowerShellSingleQuoted(executablePath)
            Dim safeWorkingDirectory As String = EscapePowerShellSingleQuoted(workingDirectory)
            Dim safeTaskName As String = EscapePowerShellSingleQuoted(BjcTaskName)

            Return _
                "$ErrorActionPreference='Stop';" & _
                "$task='" & safeTaskName & "';" & _
                "$exe='" & safeExe & "';" & _
                "$work='" & safeWorkingDirectory & "';" & _
                "if(-not (Test-Path -LiteralPath $exe -PathType Leaf)){throw ('File not found: ' + $exe)};" & _
                "$user=(Get-WmiObject Win32_ComputerSystem).UserName;" & _
                "if([string]::IsNullOrWhiteSpace($user)){throw 'No interactive Windows user is logged on'};" & _
                "$escUser=[System.Security.SecurityElement]::Escape($user);" & _
                "$escExe=[System.Security.SecurityElement]::Escape($exe);" & _
                "$escWork=[System.Security.SecurityElement]::Escape($work);" & _
                "$xml='<?xml version=""1.0"" encoding=""UTF-16""?>' +" & _
                "'<Task version=""1.2"" xmlns=""http://schemas.microsoft.com/windows/2004/02/mit/task"">' +" & _
                "'<RegistrationInfo><Description>BJCBCPOS On-Demand Task</Description></RegistrationInfo>' +" & _
                "'<Triggers />' +" & _
                "'<Principals><Principal id=""Author""><UserId>'+$escUser+'</UserId><LogonType>InteractiveToken</LogonType><RunLevel>HighestAvailable</RunLevel></Principal></Principals>' +" & _
                "'<Settings><MultipleInstancesPolicy>IgnoreNew</MultipleInstancesPolicy><DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries><StopIfGoingOnBatteries>false</StopIfGoingOnBatteries><AllowHardTerminate>true</AllowHardTerminate><StartWhenAvailable>true</StartWhenAvailable><AllowStartOnDemand>true</AllowStartOnDemand><Enabled>true</Enabled><Hidden>false</Hidden><ExecutionTimeLimit>PT0S</ExecutionTimeLimit></Settings>' +" & _
                "'<Actions Context=""Author""><Exec><Command>'+$escExe+'</Command><WorkingDirectory>'+$escWork+'</WorkingDirectory></Exec></Actions></Task>';" & _
                "$tmp=[IO.Path]::Combine($env:TEMP,($task+'_'+[Guid]::NewGuid().ToString('N')+'.xml'));" & _
                "try{$xml | Out-File -LiteralPath $tmp -Encoding Unicode -Force;" & _
                "$out=& ($env:WINDIR+'\System32\schtasks.exe') /Create /TN $task /XML $tmp /F 2>&1;" & _
                "if($LASTEXITCODE -ne 0){throw ('Cannot create task. ' + ($out -join ' '))};" & _
                "Write-Output ('Task installed: '+$task+' | RunAs='+$user+' | Program='+$exe+' | Trigger=None')}" & _
                "finally{Remove-Item -LiteralPath $tmp -Force -ErrorAction SilentlyContinue}"
        End Function

        Private Shared Function EscapePowerShellSingleQuoted(value As String) As String
            Return If(value, String.Empty).Replace("'", "''")
        End Function

        Private Shared Function BuildExecutablePath(store As StoreInfo, processName As String) As String
            Dim targetPath As String = If(store.TargetPath, String.Empty).Trim()
            Dim imageName As String = NormalizeProcessImageName(processName)
            Return Path.Combine(targetPath, imageName)
        End Function

        Private Shared Function NormalizeProcessImageName(processName As String) As String
            Dim value As String = If(processName, String.Empty).Trim()
            If String.IsNullOrWhiteSpace(value) Then value = "BJCBCPOS"
            value = Path.GetFileName(value)
            If Not value.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) Then
                value &= ".exe"
            End If
            Return value
        End Function

        Private Shared Function SanitizeName(value As String) As String
            Return If(value, String.Empty).Trim().Replace("""", String.Empty)
        End Function

        Private Shared Function QuoteArgument(value As String) As String
            Return """" & If(value, String.Empty).Replace("""", String.Empty) & """"
        End Function

    End Class

End Namespace
