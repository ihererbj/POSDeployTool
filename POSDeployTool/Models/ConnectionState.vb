Namespace Models

    Public Enum ConnectionStatus
        Unknown = 0

        Online = 1
        Offline = 2
        Timeout = 3
        [Error] = 4

        Connected = 10
        AccessDenied = 11
        Unavailable = 12
        NameMismatch = 13

        NotConfigured = 20
        Cancelled = 21
    End Enum

    Public Class ConnectionState

        Public Sub New()
            Reset()
        End Sub

        Public Property PingStatus As ConnectionStatus
        Public Property WinRmStatus As ConnectionStatus

        Public Property PingSuccess As Boolean
        Public Property WinRmSuccess As Boolean

        Public Property PingMilliseconds As Long
        Public Property WinRmMilliseconds As Long

        Public Property HostName As String

        Public Property PingMessage As String
        Public Property WinRmMessage As String

        Public Property OverallStatus As String
        Public Property LastChecked As Nullable(Of DateTime)

        Public ReadOnly Property CanDeploy As Boolean
            Get
                Return PingSuccess AndAlso
                       WinRmSuccess AndAlso
                       PingStatus = ConnectionStatus.Online AndAlso
                       WinRmStatus = ConnectionStatus.Connected
            End Get
        End Property

        Public Sub Reset()
            PingStatus = ConnectionStatus.Unknown
            WinRmStatus = ConnectionStatus.Unknown

            PingSuccess = False
            WinRmSuccess = False

            PingMilliseconds = -1
            WinRmMilliseconds = -1

            HostName = String.Empty
            PingMessage = String.Empty
            WinRmMessage = String.Empty

            OverallStatus = "Ready"
            LastChecked = Nothing
        End Sub

    End Class

End Namespace
