Imports System.Threading
Imports System.Threading.Tasks
Imports POSDeployTool.Contracts
Imports POSDeployTool.Models

Namespace Application

    Public Class ConnectionCheckController

        Private ReadOnly _pingService As IPingService
        Private ReadOnly _winRmService As IWinRmService
        Private ReadOnly _settings As AppSettings

        Public Sub New(pingService As IPingService, winRmService As IWinRmService, settings As AppSettings)
            If pingService Is Nothing Then Throw New ArgumentNullException(NameOf(pingService))
            If winRmService Is Nothing Then Throw New ArgumentNullException(NameOf(winRmService))
            If settings Is Nothing Then Throw New ArgumentNullException(NameOf(settings))

            _pingService = pingService
            _winRmService = winRmService
            _settings = settings
        End Sub

        Public Event StoreUpdated As EventHandler(Of ConnectionCheckProgressEventArgs)
        Public Event LogGenerated As EventHandler(Of ConnectionCheckProgressEventArgs)

        Public Async Function CheckAsync(stores As IList(Of StoreInfo), cancellationToken As CancellationToken) As Task
            If stores Is Nothing Then Throw New ArgumentNullException(NameOf(stores))

            Dim maximumParallelism As Integer = Math.Max(1, _settings.MaxParallelTasks)

            Using gate As New SemaphoreSlim(maximumParallelism, maximumParallelism)
                Dim tasks As New List(Of Task)()

                For Each store As StoreInfo In stores
                    cancellationToken.ThrowIfCancellationRequested()
                    tasks.Add(CheckStoreAsync(store, gate, cancellationToken))
                Next

                Await Task.WhenAll(tasks)
            End Using
        End Function

        Private Async Function CheckStoreAsync(store As StoreInfo, gate As SemaphoreSlim, cancellationToken As CancellationToken) As Task
            Dim entered As Boolean = False

            Try
                Await gate.WaitAsync(cancellationToken)
                entered = True

                EnsureConnectionState(store)
                store.Connection.Reset()
                store.Connection.OverallStatus = "Checking Ping"
                RaiseStoreUpdated(store, "Checking Ping")

                Dim pingResult As PingCheckResult = Await _pingService.CheckAsync(
                    store.IpAddress,
                    _settings.ConnectionTimeoutMilliseconds,
                    cancellationToken)

                ApplyPingResult(store.Connection, pingResult)
                RaiseLog(store, String.Format("[{0}/{1}] Ping {2}: {3} - {4}", store.StoreCode, store.ComputerName, store.IpAddress, pingResult.Status, pingResult.Message))

                If Not pingResult.Success Then
                    store.Connection.WinRmStatus = ConnectionStatus.Unknown
                    store.Connection.WinRmSuccess = False
                    store.Connection.OverallStatus = If(String.IsNullOrWhiteSpace(pingResult.Status), "Ping failed", pingResult.Status)
                    RaiseStoreUpdated(store, store.Connection.OverallStatus)
                    Return
                End If

                store.Connection.OverallStatus = "Checking WinRM"
                RaiseStoreUpdated(store, "Checking WinRM")

                Dim winRmResult As WinRmCheckResult = Await _winRmService.CheckAsync(
                    store,
                    _settings.CommandTimeoutMilliseconds,
                    cancellationToken)

                ApplyWinRmResult(store.Connection, winRmResult)
                store.Connection.OverallStatus = If(store.Connection.CanDeploy, "Ready to deploy", If(String.IsNullOrWhiteSpace(winRmResult.Status), "WinRM failed", winRmResult.Status))

                RaiseStoreUpdated(store, store.Connection.OverallStatus)
                RaiseLog(store, String.Format("[{0}/{1}] WinRM {2}: {3} - {4} ({5} ms)", store.StoreCode, store.ComputerName, store.IpAddress, winRmResult.Status, winRmResult.Message, winRmResult.DurationMilliseconds))

            Catch ex As OperationCanceledException
                EnsureConnectionState(store)
                store.Connection.PingSuccess = False
                store.Connection.WinRmSuccess = False
                store.Connection.PingStatus = ConnectionStatus.Cancelled
                store.Connection.WinRmStatus = ConnectionStatus.Cancelled
                store.Connection.OverallStatus = "Cancelled"
                store.Connection.LastChecked = DateTime.Now
                RaiseStoreUpdated(store, "Cancelled")
                Throw

            Catch ex As Exception
                EnsureConnectionState(store)
                If store.Connection.PingStatus = ConnectionStatus.Unknown Then
                    store.Connection.PingStatus = ConnectionStatus.Error
                End If
                store.Connection.WinRmStatus = ConnectionStatus.Error
                store.Connection.PingSuccess = False
                store.Connection.WinRmSuccess = False
                store.Connection.OverallStatus = "Error"
                store.Connection.LastChecked = DateTime.Now
                RaiseStoreUpdated(store, "Error")
                RaiseLog(store, String.Format("[{0}/{1}] Connection check error: {2}", store.StoreCode, store.ComputerName, ex.Message))

            Finally
                If entered Then gate.Release()
            End Try
        End Function

        Private Shared Sub ApplyPingResult(connection As ConnectionState, result As PingCheckResult)
            connection.PingSuccess = result.Success
            connection.PingStatus = MapPingStatus(result)
            connection.PingMilliseconds = result.ResponseTimeMilliseconds
            connection.PingMessage = If(result.Message, String.Empty)
            connection.LastChecked = DateTime.Now
        End Sub

        Private Shared Sub ApplyWinRmResult(connection As ConnectionState, result As WinRmCheckResult)
            connection.WinRmStatus = MapWinRmStatus(result)
            connection.WinRmSuccess = connection.WinRmStatus = ConnectionStatus.Connected
            connection.WinRmMilliseconds = result.DurationMilliseconds
            connection.HostName = If(result.HostName, String.Empty)
            connection.WinRmMessage = If(result.Message, String.Empty)
            connection.LastChecked = DateTime.Now
        End Sub

        Private Shared Function MapPingStatus(result As PingCheckResult) As ConnectionStatus
            If result Is Nothing Then Return ConnectionStatus.Error
            If result.Success Then Return ConnectionStatus.Online

            Select Case Normalize(result.Status)
                Case "timeout" : Return ConnectionStatus.Timeout
                Case "offline", "unreachable", "failed" : Return ConnectionStatus.Offline
                Case "cancelled", "canceled" : Return ConnectionStatus.Cancelled
                Case Else : Return ConnectionStatus.Error
            End Select
        End Function

        Private Shared Function MapWinRmStatus(result As WinRmCheckResult) As ConnectionStatus
            If result Is Nothing Then Return ConnectionStatus.Error
            If result.Success Then Return ConnectionStatus.Connected

            Select Case Normalize(result.Status)
                Case "connected" : Return ConnectionStatus.Connected
                Case "access denied", "accessdenied" : Return ConnectionStatus.AccessDenied
                Case "unavailable", "offline", "unreachable" : Return ConnectionStatus.Unavailable
                Case "name mismatch", "namemismatch" : Return ConnectionStatus.NameMismatch
                Case "timeout" : Return ConnectionStatus.Timeout
                Case "not configured", "notconfigured" : Return ConnectionStatus.NotConfigured
                Case "cancelled", "canceled" : Return ConnectionStatus.Cancelled
                Case Else : Return ConnectionStatus.Error
            End Select
        End Function

        Private Shared Function Normalize(value As String) As String
            If String.IsNullOrWhiteSpace(value) Then Return String.Empty
            Return value.Trim().ToLowerInvariant()
        End Function

        Private Shared Sub EnsureConnectionState(store As StoreInfo)
            If store IsNot Nothing AndAlso store.Connection Is Nothing Then
                store.Connection = New ConnectionState()
            End If
        End Sub

        Private Sub RaiseStoreUpdated(store As StoreInfo, message As String)
            RaiseEvent StoreUpdated(Me, New ConnectionCheckProgressEventArgs(store, message))
        End Sub

        Private Sub RaiseLog(store As StoreInfo, message As String)
            RaiseEvent LogGenerated(Me, New ConnectionCheckProgressEventArgs(store, message))
        End Sub

    End Class

End Namespace
