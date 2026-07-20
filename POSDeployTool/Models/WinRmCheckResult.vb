Namespace Models

    Public Class WinRmCheckResult

        Public Sub New()
            Success = False
            Status = "Unknown"
            HostName = String.Empty
            StandardOutput = String.Empty
            StandardError = String.Empty
            Message = String.Empty
            ExitCode = -1
            DurationMilliseconds = 0
            CheckedAt = DateTime.Now
        End Sub

        Public Property Success As Boolean

        Public Property Status As String

        Public Property HostName As String

        Public Property StandardOutput As String

        Public Property StandardError As String

        Public Property Message As String

        Public Property ExitCode As Integer

        Public Property DurationMilliseconds As Long

        Public Property CheckedAt As DateTime

    End Class

End Namespace