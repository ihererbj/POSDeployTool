Imports System.ComponentModel
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports POSDeployTool.Models

Namespace Services

    Public Class DeployService

        Private ReadOnly _remoteCommandService As New RemoteCommandService()

        Public Async Function StopProcessAsync(
            store As StoreInfo,
            processName As String,
            timeoutMilliseconds As Integer,
            cancellationToken As CancellationToken
        ) As Task(Of CheckResult)

            Dim name As String = NormalizeProcessName(processName)

            If String.IsNullOrWhiteSpace(name) Then
                Return CheckResult.Success("Stop skipped")
            End If

            Dim command As String =
                String.Format(
                    "cmd.exe /c taskkill /IM {0} /F >nul 2>&1 & exit /b 0",
                    QuoteCmd(name)
                )

            Dim result As CheckResult =
                Await _remoteCommandService.ExecuteAsync(
                    store,
                    command,
                    timeoutMilliseconds,
                    cancellationToken
                )

            If result.IsSuccess Then
                Return CheckResult.Success(
                    "Process stopped or was not running: " & name
                )
            End If

            Return result

        End Function

        Public Async Function CopyPackageAsync(
            store As StoreInfo,
            packagePath As String,
            timeoutMilliseconds As Integer,
            cancellationToken As CancellationToken
        ) As Task(Of CheckResult)

            If store Is Nothing Then
                Return CheckResult.Fail("Store is null")
            End If

            If String.IsNullOrWhiteSpace(packagePath) OrElse
               Not Directory.Exists(packagePath) Then

                Return CheckResult.Fail(
                    "Release package folder not found"
                )
            End If

            Dim targetPath As String =
                If(store.TargetPath, "C:\BJCBCPOS").Trim()

            Dim remotePath As String =
                ToAdminShare(store.IpAddress, targetPath)

            If remotePath Is Nothing Then
                Return CheckResult.Fail(
                    "TargetPath must use a local drive, for example C:\BJCBCPOS"
                )
            End If

            Dim shareRoot As String =
                RemotePathRoot(remotePath)

            ' ตัดการเชื่อมต่อเดิมก่อน เพื่อป้องกัน error 1219
            Await RunProcessAsync(
                "net.exe",
                String.Format(
                    "use {0} /delete /y",
                    Quote(shareRoot)
                ),
                15000,
                CancellationToken.None,
                Function(code) True
            )

            Dim connectResult As CheckResult =
                Await RunProcessAsync(
                    "net.exe",
                    String.Format(
                        "use {0} /user:{1} {2} /persistent:no",
                        Quote(shareRoot),
                        Quote(If(store.Username, String.Empty)),
                        Quote(If(store.Password, String.Empty))
                    ),
                    30000,
                    cancellationToken,
                    Function(code) code = 0
                )

            If Not connectResult.IsSuccess Then
                Return CheckResult.Fail(
                    "SMB authentication failed: " &
                    connectResult.Message
                )
            End If

            Dim operationResult As CheckResult = Nothing

            Try
                Dim arguments As String =
                    String.Format(
                        "{0} {1} /E /COPY:DAT /DCOPY:DAT /R:2 /W:2 /XJ /FFT /NP /NFL /NDL /NJH /NJS",
                        Quote(
                            Path.GetFullPath(packagePath).
                                TrimEnd("\"c)
                        ),
                        Quote(remotePath.TrimEnd("\"c))
                    )

                Dim copyResult As CheckResult =
                    Await RunProcessAsync(
                        "robocopy.exe",
                        arguments,
                        timeoutMilliseconds,
                        cancellationToken,
                        Function(code) code >= 0 AndAlso code <= 7
                    )

                If Not copyResult.IsSuccess Then
                    operationResult =
                        CheckResult.Fail(
                            "Robocopy failed: " &
                            copyResult.Message
                        )
                Else
                    Dim sourceCount As Integer =
                        Directory.GetFiles(
                            packagePath,
                            "*",
                            SearchOption.AllDirectories
                        ).Length

                    If sourceCount = 0 Then
                        operationResult =
                            CheckResult.Fail(
                                "Release package contains no files"
                            )
                    Else
                        Dim destinationCount As Integer =
                            Directory.GetFiles(
                                remotePath,
                                "*",
                                SearchOption.AllDirectories
                            ).Length

                        If destinationCount < sourceCount Then
                            operationResult =
                                CheckResult.Fail(
                                    String.Format(
                                        "Verify failed. Source={0}, Destination={1}",
                                        sourceCount,
                                        destinationCount
                                    )
                                )
                        Else
                            operationResult =
                                CheckResult.Success(
                                    String.Format(
                                        "Copied and verified {0} files",
                                        sourceCount
                                    )
                                )
                        End If
                    End If
                End If

            Catch ex As OperationCanceledException
                Throw

            Catch ex As Exception
                operationResult =
                    CheckResult.Fail(ex.Message)
            End Try

            ' VB.NET ไม่อนุญาต Await ภายใน Finally
            ' จึงย้าย Disconnect ออกมาหลัง Try/Catch
            Dim disconnectResult As CheckResult =
                Await RunProcessAsync(
                    "net.exe",
                    String.Format(
                        "use {0} /delete /y",
                        Quote(shareRoot)
                    ),
                    15000,
                    CancellationToken.None,
                    Function(code) True
                )

            If operationResult Is Nothing Then
                operationResult =
                    CheckResult.Fail(
                        "Deploy operation returned no result"
                    )
            End If

            Return operationResult

        End Function

        Public Async Function StartProcessAsync(
            store As StoreInfo,
            executableName As String,
            timeoutMilliseconds As Integer,
            cancellationToken As CancellationToken
        ) As Task(Of CheckResult)

            Dim exeName As String =
                NormalizeProcessName(executableName)

            If String.IsNullOrWhiteSpace(exeName) Then
                Return CheckResult.Success("Start skipped")
            End If

            Dim targetPath As String =
                If(store.TargetPath, "C:\BJCBCPOS").
                    TrimEnd("\"c)

            Dim executablePath As String =
                targetPath & "\" & exeName

            Dim escapedPath As String =
                executablePath.Replace("'", "''")

            Dim command As String =
                "powershell.exe -NoProfile -NonInteractive " &
                "-ExecutionPolicy Bypass -Command " &
                Quote(
                    "$p='" & escapedPath & "';" &
                    "if(-not(Test-Path -LiteralPath $p)){" &
                    "Write-Error 'Executable not found';exit 20};" &
                    "Start-Process -FilePath $p;" &
                    "Start-Sleep -Seconds 2;" &
                    "$n=[IO.Path]::GetFileNameWithoutExtension($p);" &
                    "if(Get-Process -Name $n -ErrorAction SilentlyContinue){" &
                    "Write-Output 'PROCESS_RUNNING'}else{" &
                    "Write-Error 'Process not detected';exit 21}"
                )

            Return Await _remoteCommandService.ExecuteAsync(
                store,
                command,
                timeoutMilliseconds,
                cancellationToken
            )

        End Function

        Private Shared Function NormalizeProcessName(
            value As String
        ) As String

            Dim result As String =
                If(value, String.Empty).Trim()

            If result.IndexOfAny(
                New Char() {
                    "&"c,
                    "|"c,
                    "<"c,
                    ">"c,
                    ChrW(34)
                }
            ) >= 0 Then

                Throw New ArgumentException(
                    "Process name contains unsupported characters"
                )
            End If

            Return result

        End Function

        Private Shared Function ToAdminShare(
            ipAddress As String,
            localPath As String
        ) As String

            If String.IsNullOrWhiteSpace(ipAddress) OrElse
               String.IsNullOrWhiteSpace(localPath) Then

                Return Nothing
            End If

            If localPath.Length < 3 OrElse
               localPath(1) <> ":"c Then

                Return Nothing
            End If

            Dim drive As Char =
                Char.ToUpperInvariant(localPath(0))

            Dim remainder As String =
                localPath.Substring(2).TrimStart("\"c)

            If String.IsNullOrWhiteSpace(remainder) Then
                Return String.Format(
                    "\\{0}\{1}$",
                    ipAddress.Trim(),
                    drive
                )
            End If

            Return String.Format(
                "\\{0}\{1}$\{2}",
                ipAddress.Trim(),
                drive,
                remainder
            )

        End Function

        Private Shared Function RemotePathRoot(
            remotePath As String
        ) As String

            Dim parts() As String =
                remotePath.TrimStart("\"c).Split("\"c)

            If parts.Length < 2 Then
                Return remotePath
            End If

            Return "\\" & parts(0) & "\" & parts(1)

        End Function

        Private Shared Function Quote(
            value As String
        ) As String

            Return ChrW(34) &
                   If(value, String.Empty).
                       Replace(
                           ChrW(34).ToString(),
                           "\" & ChrW(34)
                       ) &
                   ChrW(34)

        End Function

        Private Shared Function QuoteCmd(
            value As String
        ) As String

            Return Quote(value)

        End Function

        Private Shared Async Function RunProcessAsync(
            fileName As String,
            arguments As String,
            timeoutMilliseconds As Integer,
            cancellationToken As CancellationToken,
            isSuccessfulExitCode As Func(Of Integer, Boolean)
        ) As Task(Of CheckResult)

            Try
                Dim startInfo As New ProcessStartInfo With {
                    .FileName = fileName,
                    .Arguments = arguments,
                    .UseShellExecute = False,
                    .CreateNoWindow = True,
                    .RedirectStandardOutput = True,
                    .RedirectStandardError = True,
                    .StandardOutputEncoding = Encoding.Default,
                    .StandardErrorEncoding = Encoding.Default
                }

                Using process As New Process()

                    process.StartInfo = startInfo

                    If Not process.Start() Then
                        Return CheckResult.Fail(
                            "Cannot start " & fileName
                        )
                    End If

                    Dim outputTask As Task(Of String) =
                        process.StandardOutput.ReadToEndAsync()

                    Dim errorTask As Task(Of String) =
                        process.StandardError.ReadToEndAsync()

                    Dim waitTask As Task =
                        Task.Run(
                            Sub()
                                process.WaitForExit()
                            End Sub
                        )

                    Dim timeoutTask As Task =
                        Task.Delay(
                            timeoutMilliseconds,
                            cancellationToken
                        )

                    Dim completedTask As Task =
                        Await Task.WhenAny(
                            waitTask,
                            timeoutTask
                        )

                    If completedTask IsNot waitTask Then

                        cancellationToken.
                            ThrowIfCancellationRequested()

                        Try
                            If Not process.HasExited Then
                                process.Kill()
                            End If
                        Catch
                        End Try

                        Return CheckResult.Fail(
                            fileName & " timeout"
                        )
                    End If

                    Await waitTask

                    Dim output As String =
                        (Await outputTask).Trim()

                    Dim errors As String =
                        (Await errorTask).Trim()

                    If isSuccessfulExitCode(
                        process.ExitCode
                    ) Then

                        Return CheckResult.Success(
                            If(
                                String.IsNullOrWhiteSpace(output),
                                "Exit code " & process.ExitCode,
                                FirstLine(output)
                            )
                        )
                    End If

                    Return CheckResult.Fail(
                        If(
                            String.IsNullOrWhiteSpace(errors),
                            If(
                                String.IsNullOrWhiteSpace(output),
                                "Exit code " &
                                process.ExitCode,
                                FirstLine(output)
                            ),
                            FirstLine(errors)
                        )
                    )

                End Using

            Catch ex As OperationCanceledException
                Throw

            Catch ex As Exception
                Return CheckResult.Fail(ex.Message)
            End Try

        End Function

        Private Shared Function FirstLine(
            value As String
        ) As String

            If String.IsNullOrWhiteSpace(value) Then
                Return String.Empty
            End If

            For Each line As String In
                value.Split(
                    {
                        ControlChars.Cr,
                        ControlChars.Lf
                    },
                    StringSplitOptions.RemoveEmptyEntries
                )

                If Not String.IsNullOrWhiteSpace(line) Then
                    Return line.Trim()
                End If

            Next

            Return String.Empty

        End Function

    End Class

End Namespace