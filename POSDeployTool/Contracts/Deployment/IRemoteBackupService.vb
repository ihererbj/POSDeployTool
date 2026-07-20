Imports System.Threading
Imports POSDeployTool.Models
Imports POSDeployTool.Models.Deployment

Namespace Contracts.Deployment

    Public Interface IRemoteBackupService

        Function BackupAsync(
            store As StoreInfo,
            timeoutMilliseconds As Integer,
            cancellationToken As CancellationToken
        ) As Task(Of BackupResult)

    End Interface

End Namespace
