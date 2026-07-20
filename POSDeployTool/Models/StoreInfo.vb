Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Namespace Models
    Public Class StoreInfo
        Implements INotifyPropertyChanged

        Private _selected As Boolean = True
        Private _pingStatus As String = "Not checked"
        Private _winRmStatus As String = "Not checked"
        Private _status As String = "Not checked"
        Private _lastChecked As String = String.Empty
        Private _computerName As String = String.Empty
        Private _currentVersion As String = String.Empty
        Private _windowsOS As String = String.Empty
        Private _osVersion As String = String.Empty
        Private _osBuild As String = String.Empty
        Private _osArchitecture As String = String.Empty
        Private _releaseVersion As String = String.Empty
        Private _freeDiskGb As String = String.Empty
        Private _validationStatus As String = "Not validated"
        Private _cpuName As String = String.Empty
        Private _cpuLoad As String = String.Empty
        Private _totalRamGb As String = String.Empty
        Private _freeRamGb As String = String.Empty
        Private _totalDiskGb As String = String.Empty
        Private _lastBoot As String = String.Empty
        Private _uptime As String = String.Empty
        Private _firewallStatus As String = String.Empty
        Private _posProcessStatus As String = String.Empty
        Private _healthScore As String = String.Empty
        Private _healthStatus As String = String.Empty

        Public Property Selected As Boolean
            Get
                Return _selected
            End Get
            Set(value As Boolean)
                SetField(_selected, value)
            End Set
        End Property

        Public Property StoreCode As String
        Public Property StoreName As String
        Public Property IpAddress As String
        Public Property Username As String
        Public Property Password As String
        Public Property TargetPath As String = "C:\BJCBCPOS"
        Public Property BackupPath As String = "C:\Backup"

        Public Property PingStatus As String
            Get
                Return _pingStatus
            End Get
            Set(value As String)
                SetField(_pingStatus, value)
            End Set
        End Property

        Public Property WinRmStatus As String
            Get
                Return _winRmStatus
            End Get
            Set(value As String)
                SetField(_winRmStatus, value)
            End Set
        End Property

        Public Property Status As String
            Get
                Return _status
            End Get
            Set(value As String)
                SetField(_status, value)
            End Set
        End Property

        Public Property ComputerName As String
            Get
                Return _computerName
            End Get
            Set(value As String)
                SetField(_computerName, value)
            End Set
        End Property


        Public Property WindowsOS As String
            Get
                Return _windowsOS
            End Get
            Set(value As String)
                SetField(_windowsOS, value)
            End Set
        End Property

        Public Property OSVersion As String
            Get
                Return _osVersion
            End Get
            Set(value As String)
                SetField(_osVersion, value)
            End Set
        End Property

        Public Property OSBuild As String
            Get
                Return _osBuild
            End Get
            Set(value As String)
                SetField(_osBuild, value)
            End Set
        End Property

        Public Property OSArchitecture As String
            Get
                Return _osArchitecture
            End Get
            Set(value As String)
                SetField(_osArchitecture, value)
            End Set
        End Property

        Public Property CurrentVersion As String
            Get
                Return _currentVersion
            End Get
            Set(value As String)
                SetField(_currentVersion, value)
            End Set
        End Property


        Public Property ReleaseVersion As String
            Get
                Return _releaseVersion
            End Get
            Set(value As String)
                SetField(_releaseVersion, value)
            End Set
        End Property

        Public Property FreeDiskGb As String
            Get
                Return _freeDiskGb
            End Get
            Set(value As String)
                SetField(_freeDiskGb, value)
            End Set
        End Property

        Public Property ValidationStatus As String
            Get
                Return _validationStatus
            End Get
            Set(value As String)
                SetField(_validationStatus, value)
            End Set
        End Property

        Public Property LastChecked As String
            Get
                Return _lastChecked
            End Get
            Set(value As String)
                SetField(_lastChecked, value)
            End Set
        End Property


        Public Property CpuName As String
            Get
                Return _cpuName
            End Get
            Set(value As String)
                SetField(_cpuName, value)
            End Set
        End Property
        Public Property CpuLoad As String
            Get
                Return _cpuLoad
            End Get
            Set(value As String)
                SetField(_cpuLoad, value)
            End Set
        End Property
        Public Property TotalRamGb As String
            Get
                Return _totalRamGb
            End Get
            Set(value As String)
                SetField(_totalRamGb, value)
            End Set
        End Property
        Public Property FreeRamGb As String
            Get
                Return _freeRamGb
            End Get
            Set(value As String)
                SetField(_freeRamGb, value)
            End Set
        End Property
        Public Property TotalDiskGb As String
            Get
                Return _totalDiskGb
            End Get
            Set(value As String)
                SetField(_totalDiskGb, value)
            End Set
        End Property
        Public Property LastBoot As String
            Get
                Return _lastBoot
            End Get
            Set(value As String)
                SetField(_lastBoot, value)
            End Set
        End Property
        Public Property Uptime As String
            Get
                Return _uptime
            End Get
            Set(value As String)
                SetField(_uptime, value)
            End Set
        End Property
        Public Property FirewallStatus As String
            Get
                Return _firewallStatus
            End Get
            Set(value As String)
                SetField(_firewallStatus, value)
            End Set
        End Property
        Public Property PosProcessStatus As String
            Get
                Return _posProcessStatus
            End Get
            Set(value As String)
                SetField(_posProcessStatus, value)
            End Set
        End Property
        Public Property HealthScore As String
            Get
                Return _healthScore
            End Get
            Set(value As String)
                SetField(_healthScore, value)
            End Set
        End Property
        Public Property HealthStatus As String
            Get
                Return _healthStatus
            End Get
            Set(value As String)
                SetField(_healthStatus, value)
            End Set
        End Property

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Private Sub SetField(Of T)(ByRef field As T,
                                   value As T,
                                   <CallerMemberName> Optional propertyName As String = Nothing)
            If EqualityComparer(Of T).Default.Equals(field, value) Then Return
            field = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
    End Class
End Namespace
