Namespace Models
    Public Class AuditRecord
        Public Property TimestampUtc As DateTime
        Public Property UserName As String
        Public Property MachineName As String
        Public Property ActionName As String
        Public Property Target As String
        Public Property Result As String
        Public Property Detail As String
        Public Property CorrelationId As String
    End Class
End Namespace
