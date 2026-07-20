Namespace Models.Deployment

    Public Class BackupResult

        Public Sub New()
            BackupPath = String.Empty
            Message = String.Empty
        End Sub

        Public Property Success As Boolean
        Public Property BackupPath As String
        Public Property ItemCount As Integer
        Public Property Message As String
        Public Property DurationMilliseconds As Long

    End Class

End Namespace
