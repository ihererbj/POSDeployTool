Imports System.Threading
Imports POSDeployTool.Application.Deployment
Imports POSDeployTool.Contracts
Imports POSDeployTool.Contracts.Deployment
Imports POSDeployTool.Models
Imports POSDeployTool.Plugins.Deployment

Namespace Application

    Public Class DeploymentController

        Private ReadOnly _queueEngine As DeploymentQueueEngine

        Public Sub New(
            validationService As IPreDeployValidationService,
            backupService As IRemoteBackupService,
            maxParallelTasks As Integer,
            backupTimeoutMilliseconds As Integer
        )
            If validationService Is Nothing Then
                Throw New ArgumentNullException(
                    NameOf(validationService))
            End If

            If backupService Is Nothing Then
                Throw New ArgumentNullException(
                    NameOf(backupService))
            End If

            Dim steps As IDeploymentStep() = {
                New PreDeployValidationStep(
                    validationService),
                New RemoteBackupStep(
                    backupService,
                    backupTimeoutMilliseconds)
            }

            _queueEngine =
                New DeploymentQueueEngine(
                    steps,
                    maxParallelTasks)

            AddHandler _queueEngine.StoreUpdated,
                AddressOf QueueEngine_StoreUpdated
            AddHandler _queueEngine.LogGenerated,
                AddressOf QueueEngine_LogGenerated
        End Sub

        Public Event StoreUpdated As EventHandler(
            Of DeploymentProgressEventArgs)

        Public Event LogGenerated As EventHandler(
            Of DeploymentProgressEventArgs)

        Public Function PrepareAsync(
            stores As IList(Of StoreInfo),
            cancellationToken As CancellationToken
        ) As Task
            Return _queueEngine.RunAsync(
                stores,
                cancellationToken)
        End Function

        Private Sub QueueEngine_StoreUpdated(
            sender As Object,
            e As DeploymentProgressEventArgs
        )
            RaiseEvent StoreUpdated(Me, e)
        End Sub

        Private Sub QueueEngine_LogGenerated(
            sender As Object,
            e As DeploymentProgressEventArgs
        )
            RaiseEvent LogGenerated(Me, e)
        End Sub

    End Class

End Namespace
