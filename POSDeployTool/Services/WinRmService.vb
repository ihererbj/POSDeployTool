Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports POSDeployTool.Models

Namespace Services

    Public Class WinRmService

        Public Async Function CheckAsync(
            store As StoreInfo,
            timeoutMilliseconds As Integer,
            cancellationToken As CancellationToken
        ) As Task(Of CheckResult)

            If store Is Nothing Then
                Return CheckResult.Fail("Store is null")
            End If

            Dim ipAddress As String =
                If(store.IpAddress, String.Empty).Trim()

            Dim username As String =
                If(store.Username, String.Empty).Trim()

            ' อ่านจาก stores.json
            ' Trim เพื่อป้องกันช่องว่างที่ติดหน้าหรือท้ายโดยไม่ตั้งใจ
            Dim password As String =
                If(store.Password, String.Empty).Trim()

            If String.IsNullOrWhiteSpace(ipAddress) Then
                Return CheckResult.Fail("IP is empty")
            End If

            If String.IsNullOrWhiteSpace(username) Then
                Return CheckResult.Fail("Username is empty in stores.json")
            End If

            If String.IsNullOrEmpty(password) Then
                Return CheckResult.Fail("Password is empty in stores.json")
            End If

            ' ทำให้เหมือนคำสั่งที่ทดสอบผ่าน:
            ' winrs -r:http://172.30.149.21:5985 -u:UPOS05 -p:password hostname
            Dim arguments As String =
                String.Format(
                    "-r:http://{0}:5985 -u:{1} -p:{2} hostname",
                    ipAddress,
                    username,
                    password
                )

            ' Debug โดยไม่แสดง Password จริง
            Debug.WriteLine(
                String.Format(
                    "winrs.exe -r:http://{0}:5985 -u:{1} -p:{2} hostname",
                    ipAddress,
                    username,
                    New String("*"c, password.Length)
                )
            )

            Debug.WriteLine(
                String.Format(
                    "Credential: IP={0}, User=[{1}], PasswordLength={2}",
                    ipAddress,
                    username,
                    password.Length
                )
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
                        Task.Run(
                            Sub()
                                process.WaitForExit()
                            End Sub
                        )

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
                            ' ไม่ให้ Kill error กระทบผลลัพธ์หลัก
                        End Try

                        Return CheckResult.Fail(
                            String.Format(
                                "WinRM timeout after {0} ms",
                                timeoutMilliseconds
                            )
                        )
                    End If

                    Await waitTask

                    Dim output As String =
                        (Await outputTask).Trim()

                    Dim errorText As String =
                        (Await errorTask).Trim()

                    If process.ExitCode = 0 Then

                        Dim hostName As String =
                            FirstLine(output)

                        If String.IsNullOrWhiteSpace(hostName) Then
                            Return CheckResult.Success("WinRM OK")
                        End If

                        Return CheckResult.Success(hostName)
                    End If

                    Dim errorMessage As String =
                        If(
                            String.IsNullOrWhiteSpace(errorText),
                            output,
                            errorText
                        )

                    Return CheckResult.Fail(
                        CleanMessage(errorMessage)
                    )

                End Using

            Catch ex As OperationCanceledException
                Throw

            Catch ex As Win32Exception
                Return CheckResult.Fail(
                    "Cannot start winrs.exe: " & ex.Message
                )

            Catch ex As Exception
                Return CheckResult.Fail(ex.Message)
            End Try

        End Function

        Private Shared Function FirstLine(value As String) As String

            If String.IsNullOrWhiteSpace(value) Then
                Return String.Empty
            End If

            Dim lines() As String =
                value.Split(
                    {ControlChars.Cr, ControlChars.Lf},
                    StringSplitOptions.RemoveEmptyEntries
                )

            For Each line As String In lines
                If Not String.IsNullOrWhiteSpace(line) Then
                    Return line.Trim()
                End If
            Next

            Return String.Empty

        End Function

        Private Shared Function CleanMessage(value As String) As String

            Dim result As String =
                FirstLine(value)

            If String.IsNullOrWhiteSpace(result) Then
                Return "WinRM failed"
            End If

            If result.Length > 300 Then
                result = result.Substring(0, 300) & "..."
            End If

            Return result

        End Function

    End Class

End Namespace