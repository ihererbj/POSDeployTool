Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports POSDeployTool.Models

Namespace Services

    Public Class RemoteCommandService

        Public Async Function ExecuteAsync(
            store As StoreInfo,
            remoteCommand As String,
            timeoutMilliseconds As Integer,
            cancellationToken As CancellationToken
        ) As Task(Of CheckResult)

            If store Is Nothing Then Return CheckResult.Fail("Store is null")

            Dim ipAddress As String = If(store.IpAddress, String.Empty).Trim()
            Dim username As String = If(store.Username, String.Empty).Trim()
            Dim password As String = If(store.Password, String.Empty).Trim()

            If String.IsNullOrWhiteSpace(ipAddress) Then Return CheckResult.Fail("IP is empty")
            If String.IsNullOrWhiteSpace(username) Then Return CheckResult.Fail("Username is empty in stores.json")
            If String.IsNullOrEmpty(password) Then Return CheckResult.Fail("Password is empty in stores.json")
            If String.IsNullOrWhiteSpace(remoteCommand) Then Return CheckResult.Fail("Remote command is empty")

            Dim arguments As String = String.Format(
                "-r:http://{0}:5985 -u:{1} -p:{2} {3}",
                ipAddress,
                username,
                password,
                remoteCommand
            )

            Try
                Dim startInfo As New ProcessStartInfo With {
                    .FileName = "winrs.exe",
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
                        Return CheckResult.Fail("Cannot start winrs.exe")
                    End If

                    Dim outputTask As Task(Of String) = process.StandardOutput.ReadToEndAsync()
                    Dim errorTask As Task(Of String) = process.StandardError.ReadToEndAsync()
                    Dim waitTask As Task = Task.Run(Sub() process.WaitForExit())
                    Dim timeoutTask As Task = Task.Delay(timeoutMilliseconds, cancellationToken)
                    Dim completedTask As Task = Await Task.WhenAny(waitTask, timeoutTask)

                    If completedTask IsNot waitTask Then
                        cancellationToken.ThrowIfCancellationRequested()

                        Try
                            If Not process.HasExited Then
                                process.Kill()
                            End If
                        Catch
                            ' Ignore process termination errors.
                        End Try

                        Return CheckResult.Fail(
                            String.Format("Remote command timeout after {0} ms", timeoutMilliseconds)
                        )
                    End If

                    Await waitTask

                    Dim output As String = (Await outputTask).Trim()
                    Dim errorText As String = (Await errorTask).Trim()

                    If process.ExitCode = 0 Then
                        Return CheckResult.Success(CleanMessage(output, "Command completed"))
                    End If

                    Dim message As String =
                        If(String.IsNullOrWhiteSpace(errorText), output, errorText)

                    Return CheckResult.Fail(
                        CleanMessage(message, "Remote command failed")
                    )
                End Using

            Catch ex As OperationCanceledException
                Throw

            Catch ex As Win32Exception
                Return CheckResult.Fail("Cannot start winrs.exe: " & ex.Message)

            Catch ex As Exception
                Return CheckResult.Fail(ex.Message)
            End Try
        End Function


        Public Async Function ExecuteFullAsync(
            store As StoreInfo,
            remoteCommand As String,
            timeoutMilliseconds As Integer,
            cancellationToken As CancellationToken
        ) As Task(Of CheckResult)

            If store Is Nothing Then Return CheckResult.Fail("Store is null")

            Dim ipAddress As String = If(store.IpAddress, String.Empty).Trim()
            Dim username As String = If(store.Username, String.Empty).Trim()
            Dim password As String = If(store.Password, String.Empty).Trim()

            If String.IsNullOrWhiteSpace(ipAddress) Then Return CheckResult.Fail("IP is empty")
            If String.IsNullOrWhiteSpace(username) Then Return CheckResult.Fail("Username is empty in stores.json")
            If String.IsNullOrEmpty(password) Then Return CheckResult.Fail("Password is empty in stores.json")
            If String.IsNullOrWhiteSpace(remoteCommand) Then Return CheckResult.Fail("Remote command is empty")

            Dim arguments As String = String.Format(
                "-r:http://{0}:5985 -u:{1} -p:{2} {3}",
                ipAddress,
                username,
                password,
                remoteCommand
            )

            Try
                Dim startInfo As New ProcessStartInfo With {
                    .FileName = "winrs.exe",
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
                        Return CheckResult.Fail("Cannot start winrs.exe")
                    End If

                    Dim outputTask As Task(Of String) =
                        process.StandardOutput.ReadToEndAsync()

                    Dim errorTask As Task(Of String) =
                        process.StandardError.ReadToEndAsync()

                    Dim waitTask As Task =
                        Task.Run(Sub() process.WaitForExit())

                    Dim timeoutTask As Task =
                        Task.Delay(timeoutMilliseconds, cancellationToken)

                    Dim completedTask As Task =
                        Await Task.WhenAny(waitTask, timeoutTask)

                    If completedTask IsNot waitTask Then
                        cancellationToken.ThrowIfCancellationRequested()

                        Try
                            If Not process.HasExited Then
                                process.Kill()
                            End If
                        Catch
                            ' Ignore process termination errors.
                        End Try

                        Return CheckResult.Fail(
                            String.Format(
                                "Remote command timeout after {0} ms",
                                timeoutMilliseconds
                            )
                        )
                    End If

                    Await waitTask

                    Dim output As String =
                        CleanFullOutput(Await outputTask)

                    Dim errorText As String =
                        CleanFullOutput(Await errorTask)

                    If process.ExitCode = 0 Then
                        Dim successMessage As String =
                            If(
                                String.IsNullOrWhiteSpace(output),
                                "Command completed",
                                output
                            )

                        Return CheckResult.Success(successMessage)
                    End If

                    Dim failureMessage As String =
                        If(
                            String.IsNullOrWhiteSpace(errorText),
                            output,
                            errorText
                        )

                    Return CheckResult.Fail(
                        If(
                            String.IsNullOrWhiteSpace(failureMessage),
                            "Remote command failed",
                            failureMessage
                        )
                    )
                End Using

            Catch ex As OperationCanceledException
                Throw

            Catch ex As Win32Exception
                Return CheckResult.Fail("Cannot start winrs.exe: " & ex.Message)

            Catch ex As Exception
                Return CheckResult.Fail(ex.Message)
            End Try
        End Function


        Private Shared Function CleanFullOutput(value As String) As String
            If String.IsNullOrWhiteSpace(value) Then
                Return String.Empty
            End If

            Dim text As String =
                value.Replace("#< CLIXML", String.Empty).Trim()

            If text.Length > 4000 Then
                text = text.Substring(0, 4000) & "..."
            End If

            Return text
        End Function


        Private Shared Function CleanMessage(
            value As String,
            fallback As String
        ) As String

            If String.IsNullOrWhiteSpace(value) Then
                Return fallback
            End If

            Dim lines() As String = value.Split(
                {ControlChars.Cr, ControlChars.Lf},
                StringSplitOptions.RemoveEmptyEntries
            )

            For Each line As String In lines
                If Not String.IsNullOrWhiteSpace(line) Then
                    Dim result As String = line.Trim()

                    If result.Length > 500 Then
                        result = result.Substring(0, 500) & "..."
                    End If

                    Return result
                End If
            Next

            Return fallback
        End Function

    End Class

End Namespace
