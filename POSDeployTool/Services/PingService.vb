Imports System.Net.NetworkInformation
Imports System.Threading
Imports System.Threading.Tasks

Namespace Services
    Public Class PingService
        Public Async Function CheckAsync(ipAddress As String,
                                         timeoutMilliseconds As Integer,
                                         cancellationToken As CancellationToken) As Task(Of CheckResult)
            If String.IsNullOrWhiteSpace(ipAddress) Then Return CheckResult.Fail("IP is empty")
            cancellationToken.ThrowIfCancellationRequested()

            Try
                Using sender As New Ping()
                    Dim pingTask As Task(Of PingReply) = sender.SendPingAsync(ipAddress.Trim(), timeoutMilliseconds)
                    Dim completed As Task = Await Task.WhenAny(pingTask, Task.Delay(timeoutMilliseconds + 500, cancellationToken))
                    If completed IsNot pingTask Then
                        cancellationToken.ThrowIfCancellationRequested()
                        Return CheckResult.Fail("Timeout")
                    End If

                    Dim reply As PingReply = Await pingTask
                    If reply.Status = IPStatus.Success Then
                        Return CheckResult.Success(String.Format("OK ({0} ms)", reply.RoundtripTime))
                    End If
                    Return CheckResult.Fail(reply.Status.ToString())
                End Using
            Catch ex As OperationCanceledException
                Throw
            Catch ex As Exception
                Return CheckResult.Fail(ex.Message)
            End Try
        End Function
    End Class
End Namespace
