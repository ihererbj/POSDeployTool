Namespace Models.Deployment

    Public Class DeploymentQueueItem

        Public Sub New(store As StoreInfo, sequence As Integer)
            If store Is Nothing Then
                Throw New ArgumentNullException(NameOf(store))
            End If

            Me.Store = store
            Me.Sequence = sequence
            EnqueuedAt = DateTime.Now
        End Sub

        Public ReadOnly Property Store As StoreInfo
        Public ReadOnly Property Sequence As Integer
        Public ReadOnly Property EnqueuedAt As DateTime

    End Class

End Namespace
