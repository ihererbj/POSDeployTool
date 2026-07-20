Namespace Models

    Public Enum DeploymentStage
        NotStarted = 0
        Queued = 1
        Validating = 2
        Ready = 3
        ValidationFailed = 4
        Cancelled = 5
        Failed = 6
    End Enum

    Public Class DeploymentState

        Public Sub New()
            Reset()
        End Sub

        Public Property Stage As DeploymentStage
        Public Property StatusText As String
        Public Property Message As String
        Public Property StartedAt As Nullable(Of DateTime)
        Public Property CompletedAt As Nullable(Of DateTime)

        Public ReadOnly Property IsReady As Boolean
            Get
                Return Stage = DeploymentStage.Ready
            End Get
        End Property

        Public Sub Reset()
            Stage = DeploymentStage.NotStarted
            StatusText = String.Empty
            Message = String.Empty
            StartedAt = Nothing
            CompletedAt = Nothing
        End Sub

    End Class

End Namespace
