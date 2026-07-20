Namespace Models

    Public Enum DeploymentStage
        NotStarted = 0
        Queued = 1
        Waiting = 2
        Running = 3
        Validating = 4
        Ready = 5
        Completed = 6
        ValidationFailed = 7
        Cancelled = 8
        Failed = 9
    End Enum

    Public Class DeploymentState

        Public Sub New()
            Reset()
        End Sub

        Public Property Stage As DeploymentStage
        Public Property StatusText As String
        Public Property Message As String
        Public Property CurrentStep As String
        Public Property QueueSequence As Integer
        Public Property StartedAt As Nullable(Of DateTime)
        Public Property CompletedAt As Nullable(Of DateTime)
        Public Property BackupPath As String
        Public Property BackupItemCount As Integer
        Public Property BackupCreatedAt As Nullable(Of DateTime)

        Public ReadOnly Property IsReady As Boolean
            Get
                Return Stage = DeploymentStage.Ready OrElse
                       Stage = DeploymentStage.Completed
            End Get
        End Property

        Public ReadOnly Property IsRunning As Boolean
            Get
                Return Stage = DeploymentStage.Queued OrElse
                       Stage = DeploymentStage.Waiting OrElse
                       Stage = DeploymentStage.Running OrElse
                       Stage = DeploymentStage.Validating
            End Get
        End Property

        Public Sub Reset()
            Stage = DeploymentStage.NotStarted
            StatusText = String.Empty
            Message = String.Empty
            CurrentStep = String.Empty
            QueueSequence = 0
            StartedAt = Nothing
            CompletedAt = Nothing
            BackupPath = String.Empty
            BackupItemCount = 0
            BackupCreatedAt = Nothing
        End Sub

    End Class

End Namespace
