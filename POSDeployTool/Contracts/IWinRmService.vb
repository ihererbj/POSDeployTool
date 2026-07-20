Imports System.Threading
Imports System.Threading.Tasks
Imports POSDeployTool.Models

Namespace Contracts
    Public Interface IWinRmService
        Function CheckAsync(store As StoreInfo, timeoutMilliseconds As Integer, cancellationToken As CancellationToken) As Task(Of WinRmCheckResult)
    End Interface
End Namespace
