Imports System.Threading
Imports System.Threading.Tasks
Imports POSDeployTool.Models

Namespace Services
    Public Class ConnectionCheckService
        Private ReadOnly _pingService As New PingService()
        Private ReadOnly _winRmService As New WinRmService()

        Public Function CheckPingAsync(ipAddress As String,
                                       timeoutMilliseconds As Integer,
                                       cancellationToken As CancellationToken) As Task(Of CheckResult)
            Return _pingService.CheckAsync(ipAddress, timeoutMilliseconds, cancellationToken)
        End Function

        Public Function CheckWinRmAsync(store As StoreInfo,
                                        timeoutMilliseconds As Integer,
                                        cancellationToken As CancellationToken) As Task(Of CheckResult)
            Return _winRmService.CheckAsync(store, timeoutMilliseconds, cancellationToken)
        End Function
    End Class
End Namespace
