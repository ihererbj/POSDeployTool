Namespace Models.Deployment

    Public Class DeploymentStepResult

        Public Sub New(success As Boolean, message As String)
            Me.Success = success
            Me.Message = If(message, String.Empty)
        End Sub

        Public Property Success As Boolean
        Public Property Message As String

        Public Shared Function Passed(message As String) As DeploymentStepResult
            Return New DeploymentStepResult(True, message)
        End Function

        Public Shared Function Failed(message As String) As DeploymentStepResult
            Return New DeploymentStepResult(False, message)
        End Function

    End Class

End Namespace
