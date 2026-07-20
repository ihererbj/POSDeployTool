Imports System.IO
Imports Newtonsoft.Json
Imports POSDeployTool.Helpers
Imports POSDeployTool.Models

Namespace Services
    Public Class AuditService
        Private ReadOnly _syncRoot As New Object()
        Private ReadOnly _enabled As Boolean

        Public Sub New(enabled As Boolean)
            _enabled = enabled
        End Sub

        Public Sub Write(actionName As String,
                         target As String,
                         result As String,
                         Optional detail As String = Nothing,
                         Optional correlationId As String = Nothing)
            If Not _enabled Then Return

            Dim record As New AuditRecord With {
                .TimestampUtc = DateTime.UtcNow,
                .UserName = Environment.UserDomainName & "\" & Environment.UserName,
                .MachineName = Environment.MachineName,
                .ActionName = If(actionName, String.Empty),
                .Target = If(target, String.Empty),
                .Result = If(result, String.Empty),
                .Detail = If(detail, String.Empty),
                .CorrelationId = If(correlationId, String.Empty)
            }

            Dim filePath = Path.Combine(AppPaths.AuditDirectory, String.Format("audit_{0:yyyyMMdd}.jsonl", DateTime.Now))
            Dim line = JsonConvert.SerializeObject(record, Formatting.None)
            SyncLock _syncRoot
                File.AppendAllText(filePath, line & Environment.NewLine, New System.Text.UTF8Encoding(False))
            End SyncLock
        End Sub
    End Class
End Namespace
