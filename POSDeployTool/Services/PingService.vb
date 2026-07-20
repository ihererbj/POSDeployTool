Imports System.Diagnostics
Imports System.Net.NetworkInformation
Imports System.Threading
Imports System.Threading.Tasks
Imports POSDeployTool.Models

Namespace Services

    Public Class PingService

        Public Async Function CheckAsync(
            ByVal host As String,
            ByVal timeoutMilliseconds As Integer,
            ByVal cancellationToken As CancellationToken
        ) As Task(Of PingCheckResult)

            Dim result As New PingCheckResult() With {
                .Host = If(host, String.Empty),
                .CheckedAt = DateTime.Now
            }

            If String.IsNullOrWhiteSpace(host) Then
                result.Status = "Invalid"
                result.Message = "IP address or host name is empty."
                Return result
            End If

            If timeoutMilliseconds <= 0 Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(timeoutMilliseconds),
                    "Timeout must be greater than zero.")
            End If

            cancellationToken.ThrowIfCancellationRequested()

            Dim stopwatch As Stopwatch = Stopwatch.StartNew()

            Try
                Using pingClient As New Ping()

                    Dim pingTask As Task(Of PingReply) =
                        pingClient.SendPingAsync(
                            host.Trim(),
                            timeoutMilliseconds)

                    Dim cancellationTask As Task =
                        Task.Delay(
                            Timeout.Infinite,
                            cancellationToken)

                    Dim completedTask As Task =
                        Await Task.WhenAny(
                            pingTask,
                            cancellationTask)

                    If completedTask Is cancellationTask Then
                        cancellationToken.ThrowIfCancellationRequested()
                    End If

                    Dim reply As PingReply = Await pingTask

                    stopwatch.Stop()

                    result.ResponseTimeMilliseconds =
                        stopwatch.ElapsedMilliseconds

                    If reply.Status = IPStatus.Success Then

                        result.Success = True
                        result.Status = "Online"
                        result.ResponseTimeMilliseconds =
                            reply.RoundtripTime

                        result.Message =
                            String.Format(
                                "Reply from {0}: {1} ms",
                                reply.Address,
                                reply.RoundtripTime)

                    ElseIf reply.Status = IPStatus.TimedOut Then

                        result.Success = False
                        result.Status = "Timeout"
                        result.Message =
                            String.Format(
                                "Ping timed out after {0} ms.",
                                timeoutMilliseconds)

                    Else

                        result.Success = False
                        result.Status = "Offline"
                        result.Message =
                            reply.Status.ToString()

                    End If

                End Using

            Catch ex As OperationCanceledException
                stopwatch.Stop()
                Throw

            Catch ex As PingException
                stopwatch.Stop()

                result.Success = False
                result.Status = "Error"
                result.ResponseTimeMilliseconds =
                    stopwatch.ElapsedMilliseconds
                result.Message = ex.Message

            Catch ex As Exception
                stopwatch.Stop()

                result.Success = False
                result.Status = "Error"
                result.ResponseTimeMilliseconds =
                    stopwatch.ElapsedMilliseconds
                result.Message = ex.Message

            End Try

            Return result

        End Function

    End Class

End Namespace