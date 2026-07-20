Imports System.Threading
Imports POSDeployTool.Contracts.Deployment
Imports POSDeployTool.Models.Deployment

Namespace Plugins.Deployment

    Public Class RemoteBackupStep
        Implements IDeploymentStep

        Private ReadOnly _backupService As IRemoteBackupService

        Private ReadOnly _timeoutMilliseconds As Integer

        Public Sub New(
            backupService As IRemoteBackupService,
            timeoutMilliseconds As Integer
        )
            If backupService Is Nothing Then
                Throw New ArgumentNullException(
                    NameOf(backupService))
            End If

            If timeoutMilliseconds <= 0 Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(timeoutMilliseconds))
            End If

            _backupService = backupService
            _timeoutMilliseconds = timeoutMilliseconds
        End Sub

        Public ReadOnly Property Name As String _
            Implements IDeploymentStep.Name
            Get
                Return "Backing up"
            End Get
        End Property

        Public ReadOnly Property Order As Integer _
            Implements IDeploymentStep.Order
            Get
                Return 200
            End Get
        End Property

        Public Async Function ExecuteAsync(
            context As DeploymentContext,
            cancellationToken As CancellationToken
        ) As Task(Of DeploymentStepResult) _
            Implements IDeploymentStep.ExecuteAsync

            If context Is Nothing Then
                Throw New ArgumentNullException(
                    NameOf(context))
            End If

            Dim result As BackupResult =
                Await _backupService.BackupAsync(
                    context.Store,
                    _timeoutMilliseconds,
                    cancellationToken)

            If Not result.Success Then
                Return DeploymentStepResult.Failed(
                    result.Message)
            End If

            context.Items("BackupResult") = result
            context.Store.Deployment.BackupPath =
                result.BackupPath
            context.Store.Deployment.BackupItemCount =
                result.ItemCount
            context.Store.Deployment.BackupCreatedAt =
                DateTime.Now

            Return DeploymentStepResult.Passed(
                result.Message)
        End Function

    End Class

End Namespace
