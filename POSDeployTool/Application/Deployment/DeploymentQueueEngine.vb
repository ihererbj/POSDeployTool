Imports System.Threading
Imports POSDeployTool.Contracts.Deployment
Imports POSDeployTool.Models
Imports POSDeployTool.Models.Deployment

Namespace Application.Deployment

    Public Class DeploymentQueueEngine

        Private ReadOnly _steps As IReadOnlyList(Of IDeploymentStep)
        Private ReadOnly _maxParallelTasks As Integer

        Public Sub New(
            steps As IEnumerable(Of IDeploymentStep),
            maxParallelTasks As Integer
        )
            If steps Is Nothing Then
                Throw New ArgumentNullException(NameOf(steps))
            End If

            _steps = steps.
                OrderBy(Function(stepItem) stepItem.Order).
                ToList().
                AsReadOnly()

            If _steps.Count = 0 Then
                Throw New ArgumentException(
                    "At least one deployment step is required.",
                    NameOf(steps))
            End If

            _maxParallelTasks = Math.Max(1, maxParallelTasks)
        End Sub

        Public Event StoreUpdated As EventHandler(Of DeploymentProgressEventArgs)
        Public Event LogGenerated As EventHandler(Of DeploymentProgressEventArgs)

        Public Async Function RunAsync(
            stores As IList(Of StoreInfo),
            cancellationToken As CancellationToken
        ) As Task
            If stores Is Nothing Then
                Throw New ArgumentNullException(NameOf(stores))
            End If

            Dim queue As List(Of DeploymentQueueItem) =
                BuildQueue(stores)

            For Each item As DeploymentQueueItem In queue
                EnsureDeploymentState(item.Store)
                item.Store.Deployment.Reset()
                item.Store.Deployment.QueueSequence = item.Sequence
                item.Store.Deployment.Stage = DeploymentStage.Queued
                item.Store.Deployment.StatusText =
                    String.Format("Queued #{0}", item.Sequence)
                RaiseStoreUpdated(item.Store, "Deployment queued")
            Next

            Using semaphore As New SemaphoreSlim(
                _maxParallelTasks,
                _maxParallelTasks)

                Dim tasks As New List(Of Task)()

                For Each item As DeploymentQueueItem In queue
                    cancellationToken.ThrowIfCancellationRequested()

                    tasks.Add(
                        ProcessQueueItemAsync(
                            item,
                            semaphore,
                            cancellationToken))
                Next

                Await Task.WhenAll(tasks)
            End Using
        End Function

        Private Shared Function BuildQueue(
            stores As IList(Of StoreInfo)
        ) As List(Of DeploymentQueueItem)
            Dim queue As New List(Of DeploymentQueueItem)()
            Dim sequence As Integer = 1

            For Each store As StoreInfo In stores
                If store Is Nothing Then
                    Continue For
                End If

                queue.Add(
                    New DeploymentQueueItem(
                        store,
                        sequence))

                sequence += 1
            Next

            Return queue
        End Function

        Private Async Function ProcessQueueItemAsync(
            item As DeploymentQueueItem,
            semaphore As SemaphoreSlim,
            cancellationToken As CancellationToken
        ) As Task
            Dim entered As Boolean = False
            Dim store As StoreInfo = item.Store

            Try
                store.Deployment.Stage = DeploymentStage.Waiting
                store.Deployment.StatusText = "Waiting"
                RaiseStoreUpdated(store, "Waiting for deployment worker")

                Await semaphore.WaitAsync(cancellationToken)
                entered = True
                cancellationToken.ThrowIfCancellationRequested()

                store.Deployment.Stage = DeploymentStage.Running
                store.Deployment.StatusText = "Running"
                store.Deployment.StartedAt = DateTime.Now
                RaiseStoreUpdated(store, "Deployment pipeline started")

                Dim context As New DeploymentContext(store)

                For Each stepItem As IDeploymentStep In _steps
                    cancellationToken.ThrowIfCancellationRequested()

                    store.Deployment.CurrentStep = stepItem.Name
                    store.Deployment.StatusText = stepItem.Name
                    RaiseStoreUpdated(
                        store,
                        String.Format(
                            "Executing step: {0}",
                            stepItem.Name))

                    Dim result As DeploymentStepResult =
                        Await stepItem.ExecuteAsync(
                            context,
                            cancellationToken)

                    store.Deployment.Message = result.Message

                    If Not result.Success Then
                        store.Deployment.Stage =
                            DeploymentStage.Failed
                        store.Deployment.StatusText =
                            String.Format(
                                "{0} failed",
                                stepItem.Name)
                        store.Deployment.CompletedAt =
                            DateTime.Now

                        RaiseStoreUpdated(
                            store,
                            result.Message)

                        Return
                    End If

                    RaiseStoreUpdated(
                        store,
                        result.Message)
                Next

                store.Deployment.Stage = DeploymentStage.Ready
                store.Deployment.StatusText =
                    "Ready for deployment"
                store.Deployment.CurrentStep = String.Empty
                store.Deployment.CompletedAt = DateTime.Now

                RaiseStoreUpdated(
                    store,
                    "Deployment pipeline completed")

            Catch ex As OperationCanceledException
                EnsureDeploymentState(store)
                store.Deployment.Stage =
                    DeploymentStage.Cancelled
                store.Deployment.StatusText = "Cancelled"
                store.Deployment.Message =
                    "Deployment queue was cancelled"
                store.Deployment.CompletedAt = DateTime.Now

                RaiseStoreUpdated(
                    store,
                    store.Deployment.Message)

                Throw

            Catch ex As Exception
                EnsureDeploymentState(store)
                store.Deployment.Stage =
                    DeploymentStage.Failed
                store.Deployment.StatusText =
                    "Pipeline failed"
                store.Deployment.Message = ex.Message
                store.Deployment.CompletedAt = DateTime.Now

                RaiseStoreUpdated(
                    store,
                    "Deployment pipeline failed: " &
                    ex.Message)

            Finally
                If entered Then
                    semaphore.Release()
                End If
            End Try
        End Function

        Private Shared Sub EnsureDeploymentState(
            store As StoreInfo
        )
            If store IsNot Nothing AndAlso
               store.Deployment Is Nothing Then

                store.Deployment =
                    New DeploymentState()
            End If
        End Sub

        Private Sub RaiseStoreUpdated(
            store As StoreInfo,
            message As String
        )
            Dim args As New DeploymentProgressEventArgs(
                store,
                message)

            RaiseEvent StoreUpdated(Me, args)
            RaiseEvent LogGenerated(Me, args)
        End Sub

    End Class

End Namespace
