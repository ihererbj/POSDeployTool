Imports System.Threading
Imports System.Threading.Tasks
Imports POSDeployTool.Models

Namespace Contracts
    Public Interface IPingService
        Function CheckAsync(host As String, timeoutMilliseconds As Integer, cancellationToken As CancellationToken) As Task(Of PingCheckResult)
    End Interface
End Namespace
