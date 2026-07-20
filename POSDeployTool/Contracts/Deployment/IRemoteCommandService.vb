Imports System.Threading
Imports POSDeployTool.Models
Imports POSDeployTool.Models.Deployment

Namespace Contracts.Deployment

    Public Interface IRemoteCommandService

        Function ExecuteAsync(
            store As StoreInfo,
            commandText As String,
            timeoutMilliseconds As Integer,
            cancellationToken As CancellationToken
        ) As Task(Of RemoteCommandResult)

    End Interface

End Namespace
