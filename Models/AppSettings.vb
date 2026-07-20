Namespace Models
    Public Class AppSettings
        Public Property ApplicationName As String = "POS Deploy Tool"
        Public Property EnvironmentName As String = "Production"
        Public Property MaxParallelism As Integer = 10
        Public Property ConnectionTimeoutMilliseconds As Integer = 10000
        Public Property CommandTimeoutMilliseconds As Integer = 30000
        Public Property DeployTimeoutMilliseconds As Integer = 900000
        Public Property RetryCount As Integer = 3
        Public Property LogRetentionDays As Integer = 90
        Public Property EnableAuditLog As Boolean = True
        Public Property DefaultStoreConfigPath As String = "Config\stores.json"
        Public Property DefaultPackagePath As String = "Release\BJCBCPOS"
    End Class
End Namespace
