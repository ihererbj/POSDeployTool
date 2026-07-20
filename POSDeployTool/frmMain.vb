Imports System.Threading
Imports POSDeployTool.Application
Imports POSDeployTool.Contracts
Imports POSDeployTool.Models
Imports POSDeployTool.Presentation
Imports POSDeployTool.Services

Public Class frmMain

    Private ReadOnly _storeConfigService As IStoreConfigService
    Private ReadOnly _connectionController As ConnectionCheckController
    Private ReadOnly _deploymentController As DeploymentController
    Private ReadOnly _storeBindingSource As BindingSource
    Private ReadOnly _settings As AppSettings

    Private _stores As List(Of StoreInfo)
    Private _operationCancellation As CancellationTokenSource
    Private _isOperationRunning As Boolean

    Public Sub New()
        InitializeComponent()

        _settings = New AppSettings()
        _storeConfigService = New StoreConfigService()
        _connectionController = New ConnectionCheckController(New PingService(), New WinRmService(), _settings)
        _deploymentController = New DeploymentController(New PreDeployValidationService(), _settings.MaxParallelTasks)
        _storeBindingSource = New BindingSource()
        _stores = New List(Of StoreInfo)()

        AddHandler _connectionController.StoreUpdated, AddressOf ConnectionController_StoreUpdated
        AddHandler _connectionController.LogGenerated, AddressOf ConnectionController_LogGenerated
        AddHandler _deploymentController.StoreUpdated, AddressOf DeploymentController_StoreUpdated
        AddHandler _deploymentController.LogGenerated, AddressOf DeploymentController_LogGenerated

        dgvStores.DataSource = _storeBindingSource
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            AppPaths.CreateFolders()
            AddLog("POSDeployTool started.")
            LoadStoreConfiguration()
        Catch ex As Exception
            HandleConfigurationError(ex)
        End Try
    End Sub

    Private Sub LoadStoreConfiguration()
        SetBusyState(True, "Loading store configuration...")

        Try
            _stores = _storeConfigService.LoadStores()
            For Each store As StoreInfo In _stores
                EnsureConnectionState(store)
            Next

            ApplyStoreFilter()
            AddLog(String.Format("Loaded {0} store(s) from {1}", _stores.Count, AppPaths.StoreConfigFile))
            Text = String.Format("POS Deploy Tool - {0} Store(s)", _stores.Count)
        Finally
            SetBusyState(False, "Ready")
        End Try
    End Sub

    Private Sub ApplyStoreFilter()
        Dim filterText As String = txtFilter.Text.Trim()
        Dim filteredStores As List(Of StoreInfo)

        If String.IsNullOrWhiteSpace(filterText) Then
            filteredStores = _stores.Where(Function(store) store.Enabled).ToList()
        Else
            filteredStores = _stores.Where(Function(store) store.Enabled AndAlso MatchesFilter(store, filterText)).ToList()
        End If

        _storeBindingSource.DataSource = filteredStores
        _storeBindingSource.ResetBindings(False)
        InitializeStatusColumns()
        UpdateStoreCounters()
        UpdateActionButtons()
    End Sub

    Private Shared Function MatchesFilter(store As StoreInfo, filterText As String) As Boolean
        Return ContainsIgnoreCase(store.StoreCode, filterText) OrElse
               ContainsIgnoreCase(store.StoreName, filterText) OrElse
               ContainsIgnoreCase(store.ComputerName, filterText) OrElse
               ContainsIgnoreCase(store.IpAddress, filterText)
    End Function

    Private Shared Function ContainsIgnoreCase(value As String, searchValue As String) As Boolean
        Return Not String.IsNullOrEmpty(value) AndAlso value.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0
    End Function

    Private Sub InitializeStatusColumns()
        For Each row As DataGridViewRow In dgvStores.Rows
            If row.IsNewRow Then Continue For
            Dim store As StoreInfo = TryCast(row.DataBoundItem, StoreInfo)
            If store Is Nothing Then Continue For

            RefreshStoreRow(store)
            If dgvStores.Columns.Contains("colVersion") Then row.Cells("colVersion").Value = "-"
        Next
    End Sub

    Private Async Sub btnCheckConnection_Click(sender As Object, e As EventArgs) Handles btnCheckConnection.Click
        If _isOperationRunning Then Return

        dgvStores.EndEdit()
        _storeBindingSource.EndEdit()

        Dim selectedStores As List(Of StoreInfo) = _stores.Where(Function(store) store.Enabled AndAlso store.Selected).ToList()

        If selectedStores.Count = 0 Then
            MessageBox.Show("กรุณาเลือก Store อย่างน้อย 1 รายการ", "Check Connection", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        _operationCancellation = New CancellationTokenSource()

        Try
            _isOperationRunning = True
            SetOperationState(True, "Checking connection...")
            AddLog(String.Format("Starting connection check for {0} store(s). Parallel tasks: {1}, Ping timeout: {2} ms, WinRM timeout: {3} ms.", selectedStores.Count, _settings.MaxParallelTasks, _settings.ConnectionTimeoutMilliseconds, _settings.CommandTimeoutMilliseconds))

            ResetSelectedStoreStatus(selectedStores)
            Await _connectionController.CheckAsync(selectedStores, _operationCancellation.Token)

            lblStatus.Text = "Check completed"
            AddLog("Connection check completed.")
        Catch ex As OperationCanceledException
            lblStatus.Text = "Cancelled"
            AddLog("Connection check was cancelled by user.")
        Catch ex As Exception
            lblStatus.Text = "Check failed"
            AddLog("Connection check failed: " & ex.ToString())
            MessageBox.Show(ex.Message, "Connection Check Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            _isOperationRunning = False
            SetOperationState(False, lblStatus.Text)

            If _operationCancellation IsNot Nothing Then
                _operationCancellation.Dispose()
                _operationCancellation = Nothing
            End If
        End Try
    End Sub

    Private Sub ConnectionController_StoreUpdated(sender As Object, e As ConnectionCheckProgressEventArgs)
        RefreshStoreRow(e.Store)
    End Sub

    Private Sub ConnectionController_LogGenerated(sender As Object, e As ConnectionCheckProgressEventArgs)
        AddLog(e.Message)
    End Sub

    Private Async Sub btnDeploy_Click(sender As Object, e As EventArgs) Handles btnDeploy.Click
        If _isOperationRunning Then Return

        dgvStores.EndEdit()
        _storeBindingSource.EndEdit()

        Dim deployableStores As List(Of StoreInfo) = _stores.
            Where(Function(store) store.Enabled AndAlso store.Selected AndAlso IsDeployable(store)).
            ToList()

        If deployableStores.Count = 0 Then
            MessageBox.Show(
                "ไม่มี Store ที่พร้อม Deploy กรุณาตรวจสอบ Ping และ WinRM ก่อน",
                "Deploy",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)
            Return
        End If

        Dim answer As DialogResult = MessageBox.Show(
            String.Format(
                "เตรียม Deployment Queue สำหรับ {0} Store หรือไม่?" & Environment.NewLine &
                "Sprint 2.1 จะตรวจสอบความพร้อมเท่านั้น และยังไม่ Copy หรือแก้ไขไฟล์ปลายทาง",
                deployableStores.Count),
            "Sprint 2.1 - Deployment Foundation",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question)

        If answer <> DialogResult.Yes Then Return

        _operationCancellation = New CancellationTokenSource()

        Try
            _isOperationRunning = True
            SetOperationState(True, "Preparing deployment queue...")
            AddLog(String.Format("Preparing deployment queue for {0} store(s).", deployableStores.Count))

            Await _deploymentController.PrepareAsync(deployableStores, _operationCancellation.Token)

            Dim readyCount As Integer = deployableStores.
                Where(Function(store) store.Deployment IsNot Nothing AndAlso store.Deployment.IsReady).
                Count()

            lblStatus.Text = String.Format("Deployment preparation completed: {0}/{1} ready", readyCount, deployableStores.Count)
            AddLog(lblStatus.Text)

            MessageBox.Show(
                String.Format(
                    "Validation completed" & Environment.NewLine &
                    "Ready: {0}" & Environment.NewLine &
                    "Total: {1}" & Environment.NewLine & Environment.NewLine &
                    "ยังไม่มีการ Copy หรือแก้ไขไฟล์ปลายทาง",
                    readyCount,
                    deployableStores.Count),
                "Sprint 2.1",
                MessageBoxButtons.OK,
                If(readyCount = deployableStores.Count, MessageBoxIcon.Information, MessageBoxIcon.Warning))

        Catch ex As OperationCanceledException
            lblStatus.Text = "Deployment preparation cancelled"
            AddLog(lblStatus.Text)

        Catch ex As Exception
            lblStatus.Text = "Deployment preparation failed"
            AddLog("Deployment preparation failed: " & ex.ToString())
            MessageBox.Show(ex.Message, "Deployment Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            _isOperationRunning = False
            SetOperationState(False, lblStatus.Text)

            If _operationCancellation IsNot Nothing Then
                _operationCancellation.Dispose()
                _operationCancellation = Nothing
            End If
        End Try
    End Sub

    Private Sub DeploymentController_StoreUpdated(sender As Object, e As DeploymentProgressEventArgs)
        RefreshStoreRow(e.Store)
    End Sub

    Private Sub DeploymentController_LogGenerated(sender As Object, e As DeploymentProgressEventArgs)
        AddLog(String.Format(
            "[{0}/{1}] {2}",
            e.Store.StoreCode,
            e.Store.ComputerName,
            e.Message))
    End Sub

    Private Sub RefreshStoreRow(store As StoreInfo)
        If store Is Nothing OrElse IsDisposed OrElse Disposing Then Return

        If InvokeRequired Then
            Try
                BeginInvoke(New Action(Of StoreInfo)(AddressOf RefreshStoreRow), store)
            Catch ex As InvalidOperationException
            Catch ex As ObjectDisposedException
            End Try
            Return
        End If

        Dim row As DataGridViewRow = FindStoreRow(store)
        If row Is Nothing Then Return

        EnsureConnectionState(store)

        If dgvStores.Columns.Contains("colPing") Then row.Cells("colPing").Value = ConnectionStatusPresenter.BuildPingText(store.Connection)
        If dgvStores.Columns.Contains("colWinRm") Then row.Cells("colWinRm").Value = ConnectionStatusPresenter.BuildWinRmText(store.Connection)
        If dgvStores.Columns.Contains("colStatus") Then
            If store.Deployment IsNot Nothing AndAlso store.Deployment.Stage <> DeploymentStage.NotStarted Then
                row.Cells("colStatus").Value = store.Deployment.StatusText
            Else
                row.Cells("colStatus").Value = ConnectionStatusPresenter.BuildOverallText(store.Connection)
            End If
        End If

        row.DefaultCellStyle.BackColor = ConnectionStatusPresenter.ResolveBackColor(store.Connection)
        row.DefaultCellStyle.ForeColor = SystemColors.ControlText

        UpdateStoreCounters()
        UpdateActionButtons()
    End Sub

    Private Function FindStoreRow(store As StoreInfo) As DataGridViewRow
        For Each row As DataGridViewRow In dgvStores.Rows
            If row.IsNewRow Then Continue For
            Dim rowStore As StoreInfo = TryCast(row.DataBoundItem, StoreInfo)
            If rowStore Is store Then Return row
            If rowStore IsNot Nothing AndAlso String.Equals(rowStore.StoreCode, store.StoreCode, StringComparison.OrdinalIgnoreCase) Then Return row
        Next
        Return Nothing
    End Function

    Private Shared Sub EnsureConnectionState(store As StoreInfo)
        If store Is Nothing Then Return
        If store.Connection Is Nothing Then store.Connection = New ConnectionState()
        If store.Deployment Is Nothing Then store.Deployment = New DeploymentState()
    End Sub

    Private Sub ResetSelectedStoreStatus(stores As IEnumerable(Of StoreInfo))
        For Each store As StoreInfo In stores
            EnsureConnectionState(store)
            store.Connection.Reset()
            store.Connection.OverallStatus = "Queued"
            RefreshStoreRow(store)
        Next
    End Sub

    Private Sub btnStop_Click(sender As Object, e As EventArgs) Handles btnStop.Click
        If _operationCancellation Is Nothing OrElse _operationCancellation.IsCancellationRequested Then Return
        lblStatus.Text = "Cancelling..."
        btnStop.Enabled = False
        AddLog("Cancellation requested by user.")
        _operationCancellation.Cancel()
    End Sub

    Private Sub btnReload_Click(sender As Object, e As EventArgs) Handles btnReload.Click
        If _isOperationRunning Then Return
        Try
            AddLog("Reloading store configuration...")
            LoadStoreConfiguration()
        Catch ex As Exception
            HandleConfigurationError(ex)
        End Try
    End Sub

    Private Sub txtFilter_TextChanged(sender As Object, e As EventArgs) Handles txtFilter.TextChanged
        If Not _isOperationRunning Then ApplyStoreFilter()
    End Sub

    Private Sub dgvStores_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles dgvStores.CurrentCellDirtyStateChanged
        If dgvStores.IsCurrentCellDirty Then dgvStores.CommitEdit(DataGridViewDataErrorContexts.Commit)
    End Sub

    Private Sub dgvStores_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvStores.CellValueChanged
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        If dgvStores.Columns(e.ColumnIndex).Name = "colSelected" Then
            UpdateStoreCounters()
            UpdateActionButtons()
        End If
    End Sub

    Private Sub btnClearLog_Click(sender As Object, e As EventArgs) Handles btnClearLog.Click
        rtbLog.Clear()
        AddLog("On-screen log cleared.")
    End Sub

    Private Sub UpdateStoreCounters()
        Dim visibleStores As List(Of StoreInfo) = GetVisibleStores()
        Dim selectedCount As Integer = visibleStores.Where(Function(store) store.Selected).Count()
        Dim connectedCount As Integer = visibleStores.Where(Function(store) IsConnected(store)).Count()
        Dim deployableCount As Integer = visibleStores.Where(Function(store) store.Selected AndAlso IsDeployable(store)).Count()

        lblSelectedCount.Text = String.Format("Selected: {0} | Connected: {1} | Deployable: {2}", selectedCount, connectedCount, deployableCount)
        lblTotalCount.Text = String.Format("Total: {0}", visibleStores.Count)
    End Sub

    Private Shared Function IsConnected(store As StoreInfo) As Boolean
        EnsureConnectionState(store)
        Return store.Connection.WinRmStatus = ConnectionStatus.Connected
    End Function

    Private Shared Function IsDeployable(store As StoreInfo) As Boolean
        EnsureConnectionState(store)
        Return store.Connection.CanDeploy
    End Function

    Private Function GetVisibleStores() As List(Of StoreInfo)
        Dim result As New List(Of StoreInfo)()
        For Each row As DataGridViewRow In dgvStores.Rows
            If row.IsNewRow Then Continue For
            Dim store As StoreInfo = TryCast(row.DataBoundItem, StoreInfo)
            If store IsNot Nothing Then result.Add(store)
        Next
        Return result
    End Function

    Private Function HasDeployableSelectedStores() As Boolean
        Return _stores.Any(Function(store) store IsNot Nothing AndAlso store.Enabled AndAlso store.Selected AndAlso IsDeployable(store))
    End Function

    Private Sub UpdateActionButtons()
        Dim hasSelectedStores As Boolean = GetVisibleStores().Any(Function(store) store.Selected)
        btnCheckConnection.Enabled = Not _isOperationRunning AndAlso hasSelectedStores
        btnDeploy.Enabled = Not _isOperationRunning AndAlso HasDeployableSelectedStores()
        btnStop.Enabled = _isOperationRunning
    End Sub

    Private Sub SetBusyState(isBusy As Boolean, statusText As String)
        btnReload.Enabled = Not isBusy
        txtFilter.Enabled = Not isBusy
        lblStatus.Text = statusText
        UseWaitCursor = isBusy
        System.Windows.Forms.Application.DoEvents()
    End Sub

    Private Sub SetOperationState(isRunning As Boolean, statusText As String)
        btnReload.Enabled = Not isRunning
        btnCheckConnection.Enabled = Not isRunning
        btnDeploy.Enabled = False
        btnStop.Enabled = isRunning
        txtFilter.Enabled = Not isRunning
        dgvStores.Enabled = Not isRunning
        lblStatus.Text = statusText
        UseWaitCursor = False
        If Not isRunning Then UpdateActionButtons()
    End Sub

    Private Sub AddLog(message As String)
        If IsDisposed OrElse Disposing Then Return

        If InvokeRequired Then
            Try
                BeginInvoke(New Action(Of String)(AddressOf AddLog), message)
            Catch ex As InvalidOperationException
            Catch ex As ObjectDisposedException
            End Try
            Return
        End If

        Dim logLine As String = String.Format("{0:yyyy-MM-dd HH:mm:ss}  {1}", DateTime.Now, message)
        rtbLog.AppendText(logLine & Environment.NewLine)
        rtbLog.SelectionStart = rtbLog.TextLength
        rtbLog.ScrollToCaret()
        FileLogger.Write(message)
    End Sub

    Private Sub HandleConfigurationError(ex As Exception)
        AddLog("Failed to load store configuration: " & ex.ToString())
        lblStatus.Text = "Configuration Error"
        MessageBox.Show(ex.Message & Environment.NewLine & Environment.NewLine & "Configuration file:" & Environment.NewLine & AppPaths.StoreConfigFile, "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not _isOperationRunning Then Return

        Dim answer As DialogResult = MessageBox.Show("มีการตรวจสอบ Connection กำลังทำงานอยู่" & Environment.NewLine & "ต้องการยกเลิกและปิดโปรแกรมหรือไม่?", "POS Deploy Tool", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If answer = DialogResult.No Then
            e.Cancel = True
            Return
        End If

        If _operationCancellation IsNot Nothing Then _operationCancellation.Cancel()
    End Sub

    Private Sub frmMain_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        RemoveHandler _connectionController.StoreUpdated, AddressOf ConnectionController_StoreUpdated
        RemoveHandler _connectionController.LogGenerated, AddressOf ConnectionController_LogGenerated
        RemoveHandler _deploymentController.StoreUpdated, AddressOf DeploymentController_StoreUpdated
        RemoveHandler _deploymentController.LogGenerated, AddressOf DeploymentController_LogGenerated

        If _operationCancellation IsNot Nothing Then
            _operationCancellation.Dispose()
            _operationCancellation = Nothing
        End If

        FileLogger.Write("POSDeployTool closed.")
    End Sub

End Class
