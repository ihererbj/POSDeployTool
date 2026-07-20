Imports System.Diagnostics
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports POSDeployTool.Models

Namespace Services

    Public Class SmbCheckService

        Public Async Function CheckAdminShareAsync(
    store As StoreInfo,
    timeoutMilliseconds As Integer,
    token As CancellationToken
) As Task(Of CheckResult)

            If store Is Nothing Then
                Return CheckResult.Fail("Store information is missing")
            End If

            If String.IsNullOrWhiteSpace(store.IpAddress) Then
                Return CheckResult.Fail("IP address is missing")
            End If

            Dim share As String =
        "\\" & store.IpAddress.Trim() & "\C$"

            Dim arguments As String = String.Format(
        "use {0} /user:{1} {2} /persistent:no",
        Quote(share),
        Quote(store.Username),
        Quote(store.Password)
    )

            Dim result As CheckResult = Nothing
            Dim wasCancelled As Boolean = False

            Try
                result = Await RunNetAsync(
            arguments,
            timeoutMilliseconds,
            token
        )

            Catch ex As OperationCanceledException
                wasCancelled = True

            Catch ex As Exception
                result = CheckResult.Fail(ex.Message)
            End Try

            ' Await ต้องอยู่นอก Catch และ Finally
            Await DisconnectShareAsync(share)

            If wasCancelled Then
                Throw New OperationCanceledException(token)
            End If

            If result Is Nothing Then
                result = CheckResult.Fail("SMB check returned no result")
            End If

            Return result
        End Function


        Private Shared Async Function DisconnectShareAsync(share As String) As Task

            Try
                Await RunNetAsync(
            "use " & Quote(share) & " /delete /y",
            10000,
            CancellationToken.None
        )

            Catch
                ' Ignore cleanup failure
            End Try

        End Function


        Private Shared Async Function RunNetAsync(
            arguments As String,
            timeoutMilliseconds As Integer,
            token As CancellationToken
        ) As Task(Of CheckResult)

            Try
                Dim startInfo As New ProcessStartInfo With {
                    .FileName = "net.exe",
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
                        Return CheckResult.Fail("Cannot start net.exe")
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
                        Task.Delay(timeoutMilliseconds, token)

                    Dim completedTask As Task =
                        Await Task.WhenAny(waitTask, timeoutTask)

                    If completedTask IsNot waitTask Then

                        token.ThrowIfCancellationRequested()

                        Try
                            If Not process.HasExited Then
                                process.Kill()
                            End If

                        Catch
                            ' Ignore process termination errors.
                        End Try

                        Return CheckResult.Fail("SMB timeout")
                    End If

                    Await waitTask

                    Dim errorText As String =
                        (Await errorTask).Trim()

                    Dim outputText As String =
                        (Await outputTask).Trim()

                    Dim message As String =
                        If(
                            String.IsNullOrWhiteSpace(errorText),
                            outputText,
                            errorText
                        )

                    If process.ExitCode = 0 Then
                        Return CheckResult.Success("SMB OK")
                    End If

                    Return CheckResult.Fail(
                        FirstLine(message, "SMB failed")
                    )

                End Using

            Catch ex As OperationCanceledException
                Throw

            Catch ex As Exception
                Return CheckResult.Fail(ex.Message)
            End Try

        End Function


        Private Shared Function Quote(
            value As String
        ) As String

            Return ChrW(34) &
                   If(value, String.Empty).
                       Replace(ChrW(34), String.Empty) &
                   ChrW(34)

        End Function


        Private Shared Function FirstLine(
            value As String,
            fallback As String
        ) As String

            If String.IsNullOrWhiteSpace(value) Then
                Return fallback
            End If

            Dim lines As String() =
                value.Split(
                    {ControlChars.Cr, ControlChars.Lf},
                    StringSplitOptions.RemoveEmptyEntries
                )

            If lines.Length = 0 Then
                Return fallback
            End If

            Return lines(0).Trim()

        End Function

    End Class

End Namespace