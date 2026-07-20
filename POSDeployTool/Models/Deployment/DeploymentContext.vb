Namespace Models.Deployment

    Public Class DeploymentContext

        Public Sub New(store As StoreInfo)
            If store Is Nothing Then
                Throw New ArgumentNullException(NameOf(store))
            End If

            Me.Store = store
            Items = New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
        End Sub

        Public ReadOnly Property Store As StoreInfo
        Public ReadOnly Property Items As IDictionary(Of String, Object)

    End Class

End Namespace
