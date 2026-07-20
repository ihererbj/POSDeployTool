Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports System.Threading
Imports POSDeployTool.Contracts.Deployment
Imports POSDeployTool.Models
Imports POSDeployTool.Models.Deployment

Namespace Services.Deployment

    Public Class WinRmCommandService
        Implements IRemoteCommandService

        Public Async Function ExecuteAsync(
            store As StoreInfo,
            commandText As String,
            timeoutMilliseconds As Integer,
            cancellationToken As CancellationToken
        ) As Task(Of RemoteCommandResult) _
            Implements IRemoteCommandService.ExecuteAsync

            If store Is Nothing Then
                Throw New ArgumentNullException(NameOf(store))
            End If

            If String.IsNullOrWhiteSpace(commandText) Then
                Throw New ArgumentException(
                    "Remote command is required.",
                    NameOf(commandText))
            End If

            If timeoutMilliseconds <= 0 Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(timeoutMilliseconds))
            End If

            ValidateStoreCredentials(store)

            Dim result As New RemoteCommandResult()
            Dim stopwatch As Stopwatch = Stopwatch.StartNew()

            Dim winRsPath As String =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.System),
                    "winrs.exe")

            If Not File.Exists(winRsPath) Then
                result.Message = "winrs.exe was not found."
                Return result
            End If

            Dim protocol As String =
                If(store.UseHttps, "https", "http")

            Dim endpoint As String =
                String.Format(
                    "{0}://{1}:{2}",
                    protocol,
                    store.IpAddress.Trim(),
                    store.Port)

            Dim startInfo As New ProcessStartInfo() With {
                .FileName = winRsPath,
                .Arguments = BuildArguments(
                    endpoint,
                    store.Username,
                    store.Password,
                    commandText),
                .UseShellExecute = False,
                .CreateNoWindow = True,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .StandardOutputEncoding = Encoding.Default,
                .StandardErrorEncoding = Encoding.Default,
                .WorkingDirectory =
                    AppDomain.CurrentDomain.BaseDirectory
            }

            Try
                Using process As New Process()
                    process.StartInfo = startInfo
                    process.EnableRaisingEvents = True

                    If Not process.Start() Then
                        result.Message =
                            "Unable to start winrs.exe."
                        Return result
                    End If

                    Dim outputTask As Task(Of String) =
                        process.StandardOutput.ReadToEndAsync()

                    Dim errorTask As Task(Of String) =
                        process.StandardError.ReadToEndAsync()

                    Dim exitTask As Task =
                        WaitForExitAsync(
                            process,
                            cancellationToken)

                    Dim timeoutTask As Task =
                        Task.Delay(
                            timeoutMilliseconds,
                            cancellationToken)

                    Dim completedTask As Task =
                        Await Task.WhenAny(
                            exitTask,
                            timeoutTask)

                    If completedTask Is timeoutTask Then
                        cancellationToken.
                            ThrowIfCancellationRequested()

                        TryKillProcess(process)
                        stopwatch.Stop()

                        result.Message =
                            String.Format(
                                "Remote command timed out after {0} ms.",
                                timeoutMilliseconds)
                        result.DurationMilliseconds =
                            stopwatch.ElapsedMilliseconds

                        Return result
                    End If

                    Await exitTask

                    result.StandardOutput =
                        NormalizeOutput(Await outputTask)
                    result.StandardError =
                        NormalizeOutput(Await errorTask)
                    result.ExitCode = process.ExitCode
                    result.Success = process.ExitCode = 0

                    stopwatch.Stop()
                    result.DurationMilliseconds =
                        stopwatch.ElapsedMilliseconds

                    If result.Success Then
                        result.Message =
                            "Remote command completed."
                    Else
                        result.Message =
                            BuildFailureMessage(result)
                    End If
                End Using

            Catch ex As OperationCanceledException
                stopwatch.Stop()
                Throw

            Catch ex As Exception
                stopwatch.Stop()
                result.Message = ex.Message
                result.DurationMilliseconds =
                    stopwatch.ElapsedMilliseconds
            End Try

            Return result
        End Function

        Private Shared Sub ValidateStoreCredentials(
            store As StoreInfo
        )
            If String.IsNullOrWhiteSpace(store.IpAddress) Then
                Throw New ArgumentException(
                    "Store IP address is required.")
            End If

            If String.IsNullOrWhiteSpace(store.Username) Then
                Throw New ArgumentException(
                    "WinRM username is required.")
            End If

            If String.IsNullOrWhiteSpace(store.Password) Then
                Throw New ArgumentException(
                    "WinRM password is required.")
            End If
        End Sub

        Private Shared Function BuildArguments(
            endpoint As String,
            username As String,
            password As String,
            commandText As String
        ) As String
            Return String.Format(
                "-r:{0} -u:{1} -p:{2} {3}",
                endpoint.Trim(),
                username.Trim(),
                password,
                commandText)
        End Function

        Private Shared Function BuildFailureMessage(
            result As RemoteCommandResult
        ) As String
            Dim detail As String =
                String.Join(
                    " ",
                    New String() {
                        result.StandardError,
                        result.StandardOutput
                    }).Trim()

            If String.IsNullOrWhiteSpace(detail) Then
                Return String.Format(
                    "Remote command returned exit code {0}.",
                    result.ExitCode)
            End If

            Return detail
        End Function

        Private Shared Function WaitForExitAsync(
            process As Process,
            cancellationToken As CancellationToken
        ) As Task
            If process.HasExited Then
                Return Task.CompletedTask
            End If

            Dim completionSource As New TaskCompletionSource(Of Boolean)()

            Dim exitedHandler As EventHandler = Nothing
            Dim cancellationRegistration As CancellationTokenRegistration

            exitedHandler =
                Sub(sender As Object, e As EventArgs)
                    RemoveHandler process.Exited,
                        exitedHandler
                    cancellationRegistration.Dispose()
                    completionSource.TrySetResult(True)
                End Sub

            AddHandler process.Exited, exitedHandler

            cancellationRegistration =
                cancellationToken.Register(
                    Sub()
                        TryKillProcess(process)
                        RemoveHandler process.Exited,
                            exitedHandler
                        completionSource.TrySetCanceled()
                    End Sub)

            If process.HasExited Then
                RemoveHandler process.Exited,
                    exitedHandler
                cancellationRegistration.Dispose()
                completionSource.TrySetResult(True)
            End If

            Return completionSource.Task
        End Function

        Private Shared Sub TryKillProcess(
            process As Process
        )
            Try
                If process IsNot Nothing AndAlso
                   Not process.HasExited Then

                    process.Kill()
                End If
            Catch
                ' Process may already be closed.
            End Try
        End Sub

        Private Shared Function NormalizeOutput(
            value As String
        ) As String
            If value Is Nothing Then
                Return String.Empty
            End If

            Return value.Trim()
        End Function

    End Class

End Namespace
