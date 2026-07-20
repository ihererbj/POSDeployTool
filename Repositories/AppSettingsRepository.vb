Imports System.IO
Imports Newtonsoft.Json
Imports POSDeployTool.Helpers
Imports POSDeployTool.Models

Namespace Repositories
    Public Class AppSettingsRepository
        Public Function Load() As AppSettings
            Dim filePath = AppPaths.SettingsFile
            If Not File.Exists(filePath) Then
                Dim defaults As New AppSettings()
                Save(defaults)
                Return defaults
            End If

            Try
                Dim json = File.ReadAllText(filePath, System.Text.Encoding.UTF8)
                Dim settings = JsonConvert.DeserializeObject(Of AppSettings)(json)
                If settings Is Nothing Then settings = New AppSettings()
                Validate(settings)
                Return settings
            Catch ex As Exception
                Dim backup = filePath & ".invalid_" & DateTime.Now.ToString("yyyyMMddHHmmss")
                File.Copy(filePath, backup, True)
                Dim defaults As New AppSettings()
                Save(defaults)
                Return defaults
            End Try
        End Function

        Public Sub Save(settings As AppSettings)
            If settings Is Nothing Then Throw New ArgumentNullException(NameOf(settings))
            Validate(settings)
            Dim json = JsonConvert.SerializeObject(settings, Formatting.Indented)
            Dim tempFile = AppPaths.SettingsFile & ".tmp"
            File.WriteAllText(tempFile, json, New System.Text.UTF8Encoding(False))
            If File.Exists(AppPaths.SettingsFile) Then
                File.Replace(tempFile, AppPaths.SettingsFile, AppPaths.SettingsFile & ".bak", True)
            Else
                File.Move(tempFile, AppPaths.SettingsFile)
            End If
        End Sub

        Private Shared Sub Validate(settings As AppSettings)
            settings.MaxParallelism = Math.Max(1, Math.Min(settings.MaxParallelism, 100))
            settings.RetryCount = Math.Max(0, Math.Min(settings.RetryCount, 10))
            settings.LogRetentionDays = Math.Max(7, Math.Min(settings.LogRetentionDays, 3650))
            settings.ConnectionTimeoutMilliseconds = Math.Max(1000, settings.ConnectionTimeoutMilliseconds)
            settings.CommandTimeoutMilliseconds = Math.Max(1000, settings.CommandTimeoutMilliseconds)
            settings.DeployTimeoutMilliseconds = Math.Max(10000, settings.DeployTimeoutMilliseconds)
        End Sub
    End Class
End Namespace
