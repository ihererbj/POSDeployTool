Imports System.Drawing
Imports POSDeployTool.Models

Namespace Presentation

    Public NotInheritable Class ConnectionStatusPresenter

        Private Sub New()
        End Sub

        Public Shared Function BuildPingText(connection As ConnectionState) As String
            If connection Is Nothing Then Return "-"

            Select Case connection.PingStatus
                Case ConnectionStatus.Unknown : Return "-"
                Case ConnectionStatus.Online
                    Return If(connection.PingMilliseconds >= 0, String.Format("Online ({0} ms)", connection.PingMilliseconds), "Online")
                Case ConnectionStatus.Offline : Return "Offline"
                Case ConnectionStatus.Timeout : Return "Timeout"
                Case ConnectionStatus.Cancelled : Return "Cancelled"
                Case ConnectionStatus.Error : Return "Error"
                Case Else : Return connection.PingStatus.ToString()
            End Select
        End Function

        Public Shared Function BuildWinRmText(connection As ConnectionState) As String
            If connection Is Nothing Then Return "-"

            Select Case connection.WinRmStatus
                Case ConnectionStatus.Unknown : Return "-"
                Case ConnectionStatus.Connected
                    Return If(String.IsNullOrWhiteSpace(connection.HostName), "Connected", String.Format("Connected ({0})", connection.HostName))
                Case ConnectionStatus.AccessDenied : Return "Access denied"
                Case ConnectionStatus.Unavailable : Return "Unavailable"
                Case ConnectionStatus.Timeout : Return "Timeout"
                Case ConnectionStatus.NameMismatch
                    Return If(String.IsNullOrWhiteSpace(connection.HostName), "Name mismatch", String.Format("Name mismatch ({0})", connection.HostName))
                Case ConnectionStatus.NotConfigured : Return "Not configured"
                Case ConnectionStatus.Cancelled : Return "Cancelled"
                Case ConnectionStatus.Error : Return "Error"
                Case Else : Return connection.WinRmStatus.ToString()
            End Select
        End Function

        Public Shared Function BuildOverallText(connection As ConnectionState) As String
            If connection Is Nothing Then Return "Ready"
            If Not String.IsNullOrWhiteSpace(connection.OverallStatus) Then Return connection.OverallStatus
            If connection.CanDeploy Then Return "Ready to deploy"
            If connection.PingStatus <> ConnectionStatus.Online Then Return connection.PingStatus.ToString()
            Return connection.WinRmStatus.ToString()
        End Function

        Public Shared Function ResolveBackColor(connection As ConnectionState) As Color
            If connection Is Nothing Then Return SystemColors.Window
            If connection.CanDeploy Then Return Color.Honeydew

            Select Case connection.OverallStatus
                Case "Checking Ping", "Checking WinRM", "Queued" : Return Color.LightYellow
                Case "Cancelled" : Return Color.Gainsboro
            End Select

            If IsFailure(connection) Then Return Color.MistyRose
            Return SystemColors.Window
        End Function

        Private Shared Function IsFailure(connection As ConnectionState) As Boolean
            Return connection.PingStatus = ConnectionStatus.Offline OrElse
                   connection.PingStatus = ConnectionStatus.Timeout OrElse
                   connection.PingStatus = ConnectionStatus.Error OrElse
                   connection.WinRmStatus = ConnectionStatus.AccessDenied OrElse
                   connection.WinRmStatus = ConnectionStatus.Unavailable OrElse
                   connection.WinRmStatus = ConnectionStatus.NameMismatch OrElse
                   connection.WinRmStatus = ConnectionStatus.Timeout OrElse
                   connection.WinRmStatus = ConnectionStatus.NotConfigured OrElse
                   connection.WinRmStatus = ConnectionStatus.Error
        End Function

    End Class

End Namespace
