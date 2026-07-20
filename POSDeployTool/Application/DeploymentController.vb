Imports System.Threading
Imports POSDeployTool.Contracts
Imports POSDeployTool.Models

Namespace Application

    Public Class DeploymentController

        Private ReadOnly _validationService As IPreDeployValidationService
        Private ReadOnly _maxParallelTasks As Integer

        Public Sub New(validationService As IPreDeployValidationService, maxParallelTasks As Integer)
            If validationService Is Nothing Then Throw New ArgumentNullException(NameOf(validationService))

            _validationService = validationService
            _maxParallelTasks = Math.Max(1, maxParallelTasks)
        End Sub

        Public Event StoreUpdated As EventHandler(Of DeploymentProgressEventArgs)
        Public Event LogGenerated As EventHandler(Of DeploymentProgressEventArgs)

        Public Async Function PrepareAsync(stores As IList(Of StoreInfo), cancellationToken As CancellationToken) As Task
            If stores Is Nothing Then Throw New ArgumentNullException(NameOf(stores))

            Using semaphore As New SemaphoreSlim(_maxParallelTasks, _maxParallelTasks)
                Dim tasks As New List(Of Task)()

                For Each store As StoreInfo In stores
                    cancellationToken.ThrowIfCancellationRequested()
                    tasks.Add(PrepareSingleStoreAsync(store, semaphore, cancellationToken))
                Next

                Await Task.WhenAll(tasks)
            End Using
        End Function

        Private Async Function PrepareSingleStoreAsync(
            store As StoreInfo,
            semaphore As SemaphoreSlim,
            cancellationToken As CancellationToken
        ) As Task

            Dim entered As Boolean = False

            Try
                Await semaphore.WaitAsync(cancellationToken)
                entered = True
                cancellationToken.ThrowIfCancellationRequested()

                EnsureDeploymentState(store)
                store.Deployment.Reset()
                store.Deployment.Stage = DeploymentStage.Queued
                store.Deployment.StatusText = "Queued"
                RaiseStoreUpdated(store, "Deployment queued")

                Await Task.Yield()
                cancellationToken.ThrowIfCancellationRequested()

                store.Deployment.Stage = DeploymentStage.Validating
                store.Deployment.StatusText = "Validating"
                store.Deployment.StartedAt = DateTime.Now
                RaiseStoreUpdated(store, "Validating deployment prerequisites")

                Dim validationResult As PreDeployValidationResult = _validationService.Validate(store)
                cancellationToken.ThrowIfCancellationRequested()

                store.Deployment.Message = validationResult.Message
                store.Deployment.CompletedAt = DateTime.Now

                If validationResult.Success Then
                    store.Deployment.Stage = DeploymentStage.Ready
                    store.Deployment.StatusText = "Ready for deployment"
                    RaiseStoreUpdated(store, validationResult.Message)
                Else
                    store.Deployment.Stage = DeploymentStage.ValidationFailed
                    store.Deployment.StatusText = "Validation failed"
                    RaiseStoreUpdated(store, validationResult.Message)
                End If

            Catch ex As OperationCanceledException
                EnsureDeploymentState(store)
                store.Deployment.Stage = DeploymentStage.Cancelled
                store.Deployment.StatusText = "Cancelled"
                store.Deployment.Message = "Deployment preparation was cancelled"
                store.Deployment.CompletedAt = DateTime.Now
                RaiseStoreUpdated(store, store.Deployment.Message)
                Throw

            Catch ex As Exception
                EnsureDeploymentState(store)
                store.Deployment.Stage = DeploymentStage.Failed
                store.Deployment.StatusText = "Preparation failed"
                store.Deployment.Message = ex.Message
                store.Deployment.CompletedAt = DateTime.Now
                RaiseStoreUpdated(store, "Deployment preparation failed: " & ex.Message)

            Finally
                If entered Then semaphore.Release()
            End Try
        End Function

        Private Shared Sub EnsureDeploymentState(store As StoreInfo)
            If store IsNot Nothing AndAlso store.Deployment Is Nothing Then
                store.Deployment = New DeploymentState()
            End If
        End Sub

        Private Sub RaiseStoreUpdated(store As StoreInfo, message As String)
            Dim args As New DeploymentProgressEventArgs(store, message)
            RaiseEvent StoreUpdated(Me, args)
            RaiseEvent LogGenerated(Me, args)
        End Sub

    End Class

End Namespace
