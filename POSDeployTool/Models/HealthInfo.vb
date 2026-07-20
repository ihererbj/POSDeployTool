Namespace Models
    Public Class HealthInfo
        Public Property CpuName As String = String.Empty
        Public Property CpuLoadPercent As Integer = -1
        Public Property TotalRamGb As Double = 0
        Public Property FreeRamGb As Double = 0
        Public Property TotalDiskGb As Double = 0
        Public Property FreeDiskGb As Double = 0
        Public Property LastBoot As String = String.Empty
        Public Property Uptime As String = String.Empty
        Public Property FirewallStatus As String = String.Empty
        Public Property PosProcessStatus As String = String.Empty
        Public Property HealthScore As Integer = 0
        Public Property HealthStatus As String = String.Empty
    End Class
End Namespace
