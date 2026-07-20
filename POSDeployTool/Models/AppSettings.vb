Namespace Models

    Public Class AppSettings

        Public Sub New()
            MaxParallelTasks = 10
            ConnectionTimeoutMilliseconds = 5000
            CommandTimeoutMilliseconds = 15000
            BackupTimeoutMilliseconds = 300000
            RetryCount = 2
            BackupBeforeDeploy = True
            StopOnError = False
        End Sub

        Public Property MaxParallelTasks As Integer
        Public Property ConnectionTimeoutMilliseconds As Integer
        Public Property CommandTimeoutMilliseconds As Integer
        Public Property BackupTimeoutMilliseconds As Integer
        Public Property RetryCount As Integer
        Public Property BackupBeforeDeploy As Boolean
        Public Property StopOnError As Boolean

    End Class

End Namespace