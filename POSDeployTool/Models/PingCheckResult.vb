Namespace Models

    Public Class PingCheckResult

        Public Sub New()
            Success = False
            Status = "Unknown"
            Message = String.Empty
            ResponseTimeMilliseconds = -1
            CheckedAt = DateTime.Now
        End Sub

        Public Property Host As String

        Public Property Success As Boolean

        Public Property Status As String

        Public Property Message As String

        Public Property ResponseTimeMilliseconds As Long

        Public Property CheckedAt As DateTime

    End Class

End Namespace