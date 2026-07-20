Imports System.IO
Imports System.Text

Namespace Helpers
    Public Enum LogLevel
        Debug
        Information
        Warning
        [Error]
        Critical
    End Enum

    Public NotInheritable Class FileLogger
        Private ReadOnly _syncRoot As New Object()
        Private ReadOnly _logDirectory As String
        Private ReadOnly _applicationName As String

        Public Sub New(logDirectory As String, applicationName As String)
            _logDirectory = logDirectory
            _applicationName = applicationName
            Directory.CreateDirectory(_logDirectory)
        End Sub

        Public Sub Info(message As String, Optional correlationId As String = Nothing)
            Write(LogLevel.Information, message, Nothing, correlationId)
        End Sub

        Public Sub Warn(message As String, Optional correlationId As String = Nothing)
            Write(LogLevel.Warning, message, Nothing, correlationId)
        End Sub

        Public Sub [Error](message As String, ex As Exception, Optional correlationId As String = Nothing)
            Write(LogLevel.Error, message, ex, correlationId)
        End Sub

        Public Sub Critical(message As String, ex As Exception, Optional correlationId As String = Nothing)
            Write(LogLevel.Critical, message, ex, correlationId)
        End Sub

        Public Sub Write(level As LogLevel, message As String, ex As Exception, Optional correlationId As String = Nothing)
            Dim safeMessage = If(message, String.Empty).Replace(ControlChars.Cr, " ").Replace(ControlChars.Lf, " ")
            Dim id = If(String.IsNullOrWhiteSpace(correlationId), "-", correlationId)
            Dim line = String.Format("{0:yyyy-MM-dd HH:mm:ss.fff}|{1}|{2}|{3}|{4}|{5}",
                                     DateTime.Now,
                                     level.ToString().ToUpperInvariant(),
                                     Environment.UserName,
                                     Environment.MachineName,
                                     id,
                                     safeMessage)
            If ex IsNot Nothing Then
                line &= "|" & ex.ToString().Replace(ControlChars.Cr, " ").Replace(ControlChars.Lf, " ")
            End If

            Dim filePath = Path.Combine(_logDirectory, String.Format("{0}_{1:yyyyMMdd}.log", Sanitize(_applicationName), DateTime.Now))
            SyncLock _syncRoot
                File.AppendAllText(filePath, line & Environment.NewLine, New UTF8Encoding(False))
            End SyncLock
        End Sub

        Public Sub Cleanup(retentionDays As Integer)
            If retentionDays <= 0 Then Return
            Dim cutoff = DateTime.Now.AddDays(-retentionDays)
            For Each file In Directory.GetFiles(_logDirectory, "*.log", SearchOption.AllDirectories)
                Try
                    If File.GetLastWriteTime(file) < cutoff Then File.Delete(file)
                Catch
                    ' Logging cleanup must never stop the application.
                End Try
            Next
        End Sub

        Private Shared Function Sanitize(value As String) As String
            Dim result = If(value, "Application")
            For Each invalidChar In Path.GetInvalidFileNameChars()
                result = result.Replace(invalidChar, "_"c)
            Next
            Return result.Replace(" "c, "_"c)
        End Function
    End Class
End Namespace
