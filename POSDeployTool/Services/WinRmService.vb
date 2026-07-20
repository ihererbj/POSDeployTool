Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports POSDeployTool.Models

Namespace Services

    Public Class WinRmService

        Public Async Function CheckAsync(
            ByVal store As StoreInfo,
            ByVal timeoutMilliseconds As Integer,
            ByVal cancellationToken As CancellationToken
        ) As Task(Of WinRmCheckResult)

            If store Is Nothing Then
                Throw New ArgumentNullException(NameOf(store))
            End If

            If timeoutMilliseconds <= 0 Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(timeoutMilliseconds),
                    "Timeout must be greater than zero.")
            End If

            Dim result As New WinRmCheckResult() With {
                .CheckedAt = DateTime.Now
            }

            If String.IsNullOrWhiteSpace(store.IpAddress) Then
                result.Status = "Invalid"
                result.Message = "IP address or host name is empty."
                Return result
            End If

            If String.IsNullOrWhiteSpace(store.Username) Then
                result.Status = "Not configured"
                result.Message = "WinRM username is empty."
                Return result
            End If

            If String.IsNullOrWhiteSpace(store.Password) Then
                result.Status = "Not configured"
                result.Message = "WinRM password is empty."
                Return result
            End If

            cancellationToken.ThrowIfCancellationRequested()

            Dim winRsPath As String =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.System),
                    "winrs.exe")

            If Not File.Exists(winRsPath) Then
                result.Status = "Error"
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

            Dim arguments As String =
                BuildArguments(
                    endpoint,
                    store.Username,
                    store.Password,
                    "hostname")

            Dim startInfo As New ProcessStartInfo() With {
                .FileName = winRsPath,
                .Arguments = arguments,
                .UseShellExecute = False,
                .CreateNoWindow = True,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .StandardOutputEncoding = Encoding.Default,
                .StandardErrorEncoding = Encoding.Default,
                .WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
            }

            Dim stopwatch As Stopwatch = Stopwatch.StartNew()

            Try
                Using process As New Process()

                    process.StartInfo = startInfo
                    process.EnableRaisingEvents = True

                    If Not process.Start() Then
                        result.Status = "Error"
                        result.Message = "Unable to start winrs.exe."
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

                        cancellationToken.ThrowIfCancellationRequested()

                        TryKillProcess(process)

                        stopwatch.Stop()

                        result.Status = "Timeout"
                        result.DurationMilliseconds =
                            stopwatch.ElapsedMilliseconds
                        result.Message =
                            String.Format(
                                "WinRM check timed out after {0} ms.",
                                timeoutMilliseconds)

                        Return result

                    End If

                    Await exitTask

                    Dim standardOutput As String =
                        Await outputTask

                    Dim standardError As String =
                        Await errorTask

                    stopwatch.Stop()

                    result.ExitCode = process.ExitCode
                    result.DurationMilliseconds =
                        stopwatch.ElapsedMilliseconds
                    result.StandardOutput =
                        NormalizeOutput(standardOutput)
                    result.StandardError =
                        NormalizeOutput(standardError)

                    EvaluateResult(store, result)

                End Using

            Catch ex As OperationCanceledException
                stopwatch.Stop()
                Throw

            Catch ex As Exception
                stopwatch.Stop()

                result.Success = False
                result.Status = "Error"
                result.DurationMilliseconds =
                    stopwatch.ElapsedMilliseconds
                result.Message = ex.Message

            End Try

            Return result

        End Function

        Private Shared Sub EvaluateResult(
            ByVal store As StoreInfo,
            ByVal result As WinRmCheckResult
        )

            If result.ExitCode = 0 Then

                result.HostName =
                    ExtractHostName(result.StandardOutput)

                result.Success = True
                result.Status = "Connected"

                If String.IsNullOrWhiteSpace(result.HostName) Then

                    result.Message =
                        "WinRM connected, but hostname was not returned."

                ElseIf Not String.IsNullOrWhiteSpace(
                    store.ComputerName) AndAlso
                    Not String.Equals(
                        result.HostName,
                        store.ComputerName.Trim(),
                        StringComparison.OrdinalIgnoreCase) Then

                    result.Status = "Name mismatch"
                    result.Message =
                        String.Format(
                            "Expected {0}, but remote host returned {1}.",
                            store.ComputerName,
                            result.HostName)

                Else

                    result.Message =
                        String.Format(
                            "Connected to {0}.",
                            result.HostName)

                End If

                Return

            End If

            result.Success = False

            Dim combinedError As String =
                String.Join(
                    " ",
                    New String() {
                        result.StandardError,
                        result.StandardOutput
                    }).Trim()

            If ContainsIgnoreCase(
                combinedError,
                "Access is denied") Then

                result.Status = "Access denied"
                result.Message =
                    "WinRM authentication or authorization failed."

            ElseIf ContainsIgnoreCase(
                combinedError,
                "The WinRM client cannot process the request") Then

                result.Status = "WinRM error"
                result.Message = combinedError

            ElseIf ContainsIgnoreCase(
                combinedError,
                "timed out") OrElse
                ContainsIgnoreCase(
                    combinedError,
                    "cannot complete the operation") Then

                result.Status = "Timeout"
                result.Message = combinedError

            ElseIf ContainsIgnoreCase(
                combinedError,
                "The client cannot connect") Then

                result.Status = "Unavailable"
                result.Message = combinedError

            Else

                result.Status = "Failed"

                If String.IsNullOrWhiteSpace(combinedError) Then
                    result.Message =
                        String.Format(
                            "winrs.exe returned exit code {0}.",
                            result.ExitCode)
                Else
                    result.Message = combinedError
                End If

            End If

        End Sub

        Private Shared Function WaitForExitAsync(
    ByVal process As Process,
    ByVal cancellationToken As CancellationToken
) As Task

            If process Is Nothing Then
                Throw New ArgumentNullException(NameOf(process))
            End If

            If process.HasExited Then
                Return Task.CompletedTask
            End If

            Dim completionSource As New TaskCompletionSource(Of Boolean)()

            Dim exitedHandler As EventHandler = Nothing
            Dim cancellationRegistration As CancellationTokenRegistration

            exitedHandler =
        Sub(sender As Object, e As EventArgs)

            RemoveHandler process.Exited, exitedHandler
            cancellationRegistration.Dispose()

            completionSource.TrySetResult(True)

        End Sub

            AddHandler process.Exited, exitedHandler

            cancellationRegistration =
        cancellationToken.Register(
            Sub()

                TryKillProcess(process)

                RemoveHandler process.Exited, exitedHandler

                completionSource.TrySetCanceled()

            End Sub)

            If process.HasExited Then

                RemoveHandler process.Exited, exitedHandler
                cancellationRegistration.Dispose()

                completionSource.TrySetResult(True)

            End If

            Return completionSource.Task

        End Function

        Private Shared Sub TryKillProcess(
            ByVal process As Process
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

        Private Shared Function BuildArguments(
    ByVal endpoint As String,
    ByVal username As String,
    ByVal password As String,
    ByVal commandText As String
) As String

            If String.IsNullOrWhiteSpace(endpoint) Then
                Throw New ArgumentException(
            "WinRM endpoint is required.",
            NameOf(endpoint))
            End If

            If String.IsNullOrWhiteSpace(username) Then
                Throw New ArgumentException(
            "WinRM username is required.",
            NameOf(username))
            End If

            If password Is Nothing Then
                Throw New ArgumentNullException(NameOf(password))
            End If

            Return String.Format(
        "-r:{0} -u:{1} -p:{2} {3}",
        endpoint.Trim(),
        username.Trim(),
        password,
        commandText)

        End Function

        Private Shared Function QuoteArgument(
            ByVal value As String
        ) As String

            If value Is Nothing Then
                Return ChrW(34) & ChrW(34)
            End If

            Return ChrW(34) &
                   value.Replace(
                       ChrW(34).ToString(),
                       "\" & ChrW(34)) &
                   ChrW(34)

        End Function

        Private Shared Function ExtractHostName(
            ByVal standardOutput As String
        ) As String

            If String.IsNullOrWhiteSpace(standardOutput) Then
                Return String.Empty
            End If

            Dim lines As String() =
                standardOutput.Split(
                    New String() {
                        Environment.NewLine,
                        vbCr,
                        vbLf
                    },
                    StringSplitOptions.RemoveEmptyEntries)

            For Each line As String In lines

                Dim value As String = line.Trim()

                If Not String.IsNullOrWhiteSpace(value) Then
                    Return value
                End If

            Next

            Return String.Empty

        End Function

        Private Shared Function NormalizeOutput(
            ByVal value As String
        ) As String

            If value Is Nothing Then
                Return String.Empty
            End If

            Return value.Trim()

        End Function

        Private Shared Function ContainsIgnoreCase(
            ByVal value As String,
            ByVal searchValue As String
        ) As Boolean

            If String.IsNullOrWhiteSpace(value) OrElse
               String.IsNullOrWhiteSpace(searchValue) Then

                Return False

            End If

            Return value.IndexOf(
                searchValue,
                StringComparison.OrdinalIgnoreCase) >= 0

        End Function

    End Class

End Namespace