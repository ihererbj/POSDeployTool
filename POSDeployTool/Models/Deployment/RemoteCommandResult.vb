Namespace Models.Deployment

    Public Class RemoteCommandResult

        Public Sub New()
            StandardOutput = String.Empty
            StandardError = String.Empty
            Message = String.Empty
        End Sub

        Public Property Success As Boolean
        Public Property ExitCode As Integer
        Public Property StandardOutput As String
        Public Property StandardError As String
        Public Property Message As String
        Public Property DurationMilliseconds As Long

    End Class

End Namespace
