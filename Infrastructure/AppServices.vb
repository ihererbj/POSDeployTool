Imports POSDeployTool.Helpers
Imports POSDeployTool.Models
Imports POSDeployTool.Repositories
Imports POSDeployTool.Services

Namespace Infrastructure
    Public NotInheritable Class AppServices
        Private Shared ReadOnly _current As New Lazy(Of AppServices)(Function() New AppServices())

        Public Shared ReadOnly Property Current As AppServices
            Get
                Return _current.Value
            End Get
        End Property

        Public ReadOnly Property Settings As AppSettings
        Public ReadOnly Property Logger As FileLogger
        Public ReadOnly Property Audit As AuditService
        Public ReadOnly Property StoreConfig As StoreConfigService
        Public ReadOnly Property Connection As ConnectionCheckService
        Public ReadOnly Property Backup As BackupService
        Public ReadOnly Property Deploy As DeployService
        Public ReadOnly Property Validation As PreDeployValidationService
        Public ReadOnly Property Retry As RetryService
        Public ReadOnly Property History As DeploymentHistoryService
        Public ReadOnly Property Version As VersionService
        Public ReadOnly Property RemoteInfo As RemoteSystemInfoService
        Public ReadOnly Property RemoteTools As RemoteToolsService
        Public ReadOnly Property Health As HealthCheckService

        Private Sub New()
            Dim settingsRepository As New AppSettingsRepository()
            Settings = settingsRepository.Load()
            Logger = New FileLogger(AppPaths.LogDirectory, Settings.ApplicationName)
            Audit = New AuditService(Settings.EnableAuditLog)

            StoreConfig = New StoreConfigService()
            Connection = New ConnectionCheckService()
            Backup = New BackupService()
            Deploy = New DeployService()
            Validation = New PreDeployValidationService()
            Retry = New RetryService()
            History = New DeploymentHistoryService()
            Version = New VersionService()
            RemoteInfo = New RemoteSystemInfoService()
            RemoteTools = New RemoteToolsService()
            Health = New HealthCheckService()

            Logger.Cleanup(Settings.LogRetentionDays)
            Logger.Info("Application services initialized. Environment=" & Settings.EnvironmentName)
        End Sub
    End Class
End Namespace
