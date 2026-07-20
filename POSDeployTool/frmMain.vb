Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports POSDeployTool.Models
Imports POSDeployTool.Services

Public Class frmMain

    Private ReadOnly _storeConfigService As StoreConfigService
    Private ReadOnly _pingService As PingService
    Private ReadOnly _storeBindingSource As BindingSource
    Private ReadOnly _settings As AppSettings

    Private _stores As List(Of StoreInfo)
    Private _operationCancellation As CancellationTokenSource
    Private _isOperationRunning As Boolean

    Public Sub New()

        InitializeComponent()

        _storeConfigService = New StoreConfigService()
        _pingService = New PingService()
        _storeBindingSource = New BindingSource()
        _settings = New AppSettings()
        _stores = New List(Of StoreInfo)()

        dgvStores.DataSource = _storeBindingSource

    End Sub

    Private Sub frmMain_Load(
        ByVal sender As Object,
        ByVal e As EventArgs
    ) Handles MyBase.Load

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

            ApplyStoreFilter()

            AddLog(
                String.Format(
                    "Loaded {0} store(s) from {1}",
                    _stores.Count,
                    AppPaths.StoreConfigFile))

            Me.Text =
                String.Format(
                    "POS Deploy Tool - {0} Store(s)",
                    _stores.Count)

        Finally
            SetBusyState(False, "Ready")
        End Try

    End Sub

    Private Sub ApplyStoreFilter()

        Dim filterText As String = txtFilter.Text.Trim()
        Dim filteredStores As List(Of StoreInfo)

        If String.IsNullOrWhiteSpace(filterText) Then

            filteredStores =
                _stores.
                Where(Function(store) store.Enabled).
                ToList()

        Else

            filteredStores =
                _stores.
                Where(
                    Function(store)
                        Return store.Enabled AndAlso
                               MatchesFilter(store, filterText)
                    End Function).
                ToList()

        End If

        _storeBindingSource.DataSource = filteredStores
        _storeBindingSource.ResetBindings(False)

        InitializeStatusColumns()
        UpdateStoreCounters()
        UpdateActionButtons()

    End Sub

    Private Shared Function MatchesFilter(
        ByVal store As StoreInfo,
        ByVal filterText As String
    ) As Boolean

        Return ContainsIgnoreCase(store.StoreCode, filterText) OrElse
               ContainsIgnoreCase(store.StoreName, filterText) OrElse
               ContainsIgnoreCase(store.ComputerName, filterText) OrElse
               ContainsIgnoreCase(store.IpAddress, filterText)

    End Function

    Private Shared Function ContainsIgnoreCase(
        ByVal value As String,
        ByVal searchValue As String
    ) As Boolean

        If String.IsNullOrEmpty(value) Then
            Return False
        End If

        Return value.IndexOf(
            searchValue,
            StringComparison.OrdinalIgnoreCase) >= 0

    End Function

    Private Sub InitializeStatusColumns()

        For Each row As DataGridViewRow In dgvStores.Rows

            If row.IsNewRow Then
                Continue For
            End If

            row.Cells("colPing").Value = "-"
            row.Cells("colWinRm").Value = "-"
            row.Cells("colVersion").Value = "-"
            row.Cells("colStatus").Value = "Ready"

        Next

    End Sub

    Private Async Sub btnCheckConnection_Click(
        ByVal sender As Object,
        ByVal e As EventArgs
    ) Handles btnCheckConnection.Click

        If _isOperationRunning Then
            Return
        End If

        dgvStores.EndEdit()
        _storeBindingSource.EndEdit()

        Dim selectedStores As List(Of StoreInfo) =
            _stores.
            Where(
                Function(store)
                    Return store.Enabled AndAlso store.Selected
                End Function).
            ToList()

        If selectedStores.Count = 0 Then

            MessageBox.Show(
                "กรุณาเลือก Store อย่างน้อย 1 รายการ",
                "Check Connection",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)

            Return
        End If

        _operationCancellation = New CancellationTokenSource()

        Try
            _isOperationRunning = True
            SetOperationState(True, "Checking connection...")

            AddLog(
                String.Format(
                    "Starting Ping check for {0} store(s). Parallel tasks: {1}, Timeout: {2} ms.",
                    selectedStores.Count,
                    _settings.MaxParallelTasks,
                    _settings.ConnectionTimeoutMilliseconds))

            ResetSelectedStoreStatus(selectedStores)

            Await CheckSelectedStoresAsync(
                selectedStores,
                _operationCancellation.Token)

            lblStatus.Text = "Check completed"
            AddLog("Ping check completed.")

        Catch ex As OperationCanceledException

            lblStatus.Text = "Cancelled"
            AddLog("Ping check was cancelled by user.")

        Catch ex As Exception

            lblStatus.Text = "Check failed"
            AddLog("Ping check failed: " & ex.ToString())

            MessageBox.Show(
                ex.Message,
                "Connection Check Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)

        Finally

            _isOperationRunning = False
            SetOperationState(False, lblStatus.Text)

            If _operationCancellation IsNot Nothing Then
                _operationCancellation.Dispose()
                _operationCancellation = Nothing
            End If

        End Try

    End Sub

    Private Async Function CheckSelectedStoresAsync(
        ByVal stores As IList(Of StoreInfo),
        ByVal cancellationToken As CancellationToken
    ) As Task

        Dim maxParallelTasks As Integer =
            Math.Max(1, _settings.MaxParallelTasks)

        Using semaphore As New SemaphoreSlim(
            maxParallelTasks,
            maxParallelTasks)

            Dim tasks As New List(Of Task)()

            For Each store As StoreInfo In stores

                cancellationToken.ThrowIfCancellationRequested()

                tasks.Add(
                    CheckSingleStoreAsync(
                        store,
                        semaphore,
                        cancellationToken))

            Next

            Await Task.WhenAll(tasks)

        End Using

    End Function

    Private Async Function CheckSingleStoreAsync(
        ByVal store As StoreInfo,
        ByVal semaphore As SemaphoreSlim,
        ByVal cancellationToken As CancellationToken
    ) As Task

        Await semaphore.WaitAsync(cancellationToken)

        Try
            cancellationToken.ThrowIfCancellationRequested()

            UpdateStoreRow(
                store,
                "Checking...",
                "Checking")

            Dim result As PingCheckResult =
                Await _pingService.CheckAsync(
                    store.IpAddress,
                    _settings.ConnectionTimeoutMilliseconds,
                    cancellationToken)

            Dim pingDisplay As String

            If result.Success Then
                pingDisplay =
                    String.Format(
                        "Online ({0} ms)",
                        result.ResponseTimeMilliseconds)
            Else
                pingDisplay = result.Status
            End If

            UpdateStoreRow(
                store,
                pingDisplay,
                result.Status)

            AddLog(
                String.Format(
                    "[{0}/{1}] Ping {2}: {3} - {4}",
                    store.StoreCode,
                    store.ComputerName,
                    store.IpAddress,
                    result.Status,
                    result.Message))

        Catch ex As OperationCanceledException

            UpdateStoreRow(
                store,
                "Cancelled",
                "Cancelled")

            Throw

        Catch ex As Exception

            UpdateStoreRow(
                store,
                "Error",
                "Error")

            AddLog(
                String.Format(
                    "[{0}/{1}] Ping error: {2}",
                    store.StoreCode,
                    store.ComputerName,
                    ex.Message))

        Finally
            semaphore.Release()
        End Try

    End Function

    Private Sub UpdateStoreRow(
        ByVal store As StoreInfo,
        ByVal pingStatus As String,
        ByVal overallStatus As String
    )

        Dim row As DataGridViewRow =
            FindStoreRow(store)

        If row Is Nothing Then
            Return
        End If

        row.Cells("colPing").Value = pingStatus
        row.Cells("colStatus").Value = overallStatus

    End Sub

    Private Function FindStoreRow(
        ByVal store As StoreInfo
    ) As DataGridViewRow

        For Each row As DataGridViewRow In dgvStores.Rows

            If row.IsNewRow Then
                Continue For
            End If

            Dim rowStore As StoreInfo =
                TryCast(row.DataBoundItem, StoreInfo)

            If rowStore Is store Then
                Return row
            End If

            If rowStore IsNot Nothing AndAlso
               String.Equals(
                   rowStore.StoreCode,
                   store.StoreCode,
                   StringComparison.OrdinalIgnoreCase) Then

                Return row

            End If

        Next

        Return Nothing

    End Function

    Private Sub ResetSelectedStoreStatus(
        ByVal selectedStores As IEnumerable(Of StoreInfo)
    )

        For Each store As StoreInfo In selectedStores

            Dim row As DataGridViewRow =
                FindStoreRow(store)

            If row Is Nothing Then
                Continue For
            End If

            row.Cells("colPing").Value = "Waiting..."
            row.Cells("colStatus").Value = "Queued"

        Next

    End Sub

    Private Sub btnStop_Click(
        ByVal sender As Object,
        ByVal e As EventArgs
    ) Handles btnStop.Click

        If _operationCancellation Is Nothing Then
            Return
        End If

        If _operationCancellation.IsCancellationRequested Then
            Return
        End If

        lblStatus.Text = "Cancelling..."
        btnStop.Enabled = False

        AddLog("Cancellation requested by user.")
        _operationCancellation.Cancel()

    End Sub

    Private Sub btnReload_Click(
        ByVal sender As Object,
        ByVal e As EventArgs
    ) Handles btnReload.Click

        If _isOperationRunning Then
            Return
        End If

        Try
            AddLog("Reloading store configuration...")
            LoadStoreConfiguration()

        Catch ex As Exception
            HandleConfigurationError(ex)
        End Try

    End Sub

    Private Sub txtFilter_TextChanged(
        ByVal sender As Object,
        ByVal e As EventArgs
    ) Handles txtFilter.TextChanged

        If _isOperationRunning Then
            Return
        End If

        ApplyStoreFilter()

    End Sub

    Private Sub dgvStores_CurrentCellDirtyStateChanged(
        ByVal sender As Object,
        ByVal e As EventArgs
    ) Handles dgvStores.CurrentCellDirtyStateChanged

        If dgvStores.IsCurrentCellDirty Then
            dgvStores.CommitEdit(
                DataGridViewDataErrorContexts.Commit)
        End If

    End Sub

    Private Sub dgvStores_CellValueChanged(
        ByVal sender As Object,
        ByVal e As DataGridViewCellEventArgs
    ) Handles dgvStores.CellValueChanged

        If e.RowIndex < 0 Then
            Return
        End If

        If dgvStores.Columns(e.ColumnIndex).Name = "colSelected" Then
            UpdateStoreCounters()
            UpdateActionButtons()
        End If

    End Sub

    Private Sub btnClearLog_Click(
        ByVal sender As Object,
        ByVal e As EventArgs
    ) Handles btnClearLog.Click

        rtbLog.Clear()
        AddLog("On-screen log cleared.")

    End Sub

    Private Sub UpdateStoreCounters()

        Dim selectedCount As Integer = 0

        For Each row As DataGridViewRow In dgvStores.Rows

            If row.IsNewRow Then
                Continue For
            End If

            Dim value As Object =
                row.Cells("colSelected").Value

            If value IsNot Nothing AndAlso
               Convert.ToBoolean(value) Then

                selectedCount += 1

            End If

        Next

        lblSelectedCount.Text =
            String.Format(
                "Selected: {0}",
                selectedCount)

        lblTotalCount.Text =
            String.Format(
                "Total: {0}",
                dgvStores.Rows.Count)

    End Sub

    Private Function GetSelectedVisibleStoreCount() As Integer

        Dim selectedCount As Integer = 0

        For Each row As DataGridViewRow In dgvStores.Rows

            If row.IsNewRow Then
                Continue For
            End If

            Dim value As Object =
                row.Cells("colSelected").Value

            If value IsNot Nothing AndAlso
               Convert.ToBoolean(value) Then

                selectedCount += 1

            End If

        Next

        Return selectedCount

    End Function

    Private Sub UpdateActionButtons()

        Dim hasSelectedStores As Boolean =
            GetSelectedVisibleStoreCount() > 0

        btnCheckConnection.Enabled =
            Not _isOperationRunning AndAlso
            hasSelectedStores

        btnDeploy.Enabled = False
        btnStop.Enabled = _isOperationRunning

    End Sub

    Private Sub SetBusyState(
        ByVal isBusy As Boolean,
        ByVal statusText As String
    )

        btnReload.Enabled = Not isBusy
        txtFilter.Enabled = Not isBusy

        lblStatus.Text = statusText
        Me.UseWaitCursor = isBusy

        Application.DoEvents()

    End Sub

    Private Sub SetOperationState(
        ByVal isRunning As Boolean,
        ByVal statusText As String
    )

        btnReload.Enabled = Not isRunning
        btnCheckConnection.Enabled = Not isRunning
        btnDeploy.Enabled = False
        btnStop.Enabled = isRunning

        txtFilter.Enabled = Not isRunning
        dgvStores.Enabled = Not isRunning

        lblStatus.Text = statusText
        Me.UseWaitCursor = False

        If Not isRunning Then
            UpdateActionButtons()
        End If

    End Sub

    Private Sub AddLog(ByVal message As String)

        If Me.IsDisposed Then
            Return
        End If

        If Me.InvokeRequired Then

            Me.BeginInvoke(
                New Action(Of String)(AddressOf AddLog),
                message)

            Return
        End If

        Dim logLine As String =
            String.Format(
                "{0:yyyy-MM-dd HH:mm:ss}  {1}",
                DateTime.Now,
                message)

        rtbLog.AppendText(
            logLine &
            Environment.NewLine)

        rtbLog.SelectionStart = rtbLog.TextLength
        rtbLog.ScrollToCaret()

        FileLogger.Write(message)

    End Sub

    Private Sub HandleConfigurationError(
        ByVal ex As Exception
    )

        AddLog(
            "Failed to load store configuration: " &
            ex.ToString())

        lblStatus.Text = "Configuration Error"

        MessageBox.Show(
            ex.Message &
            Environment.NewLine &
            Environment.NewLine &
            "Configuration file:" &
            Environment.NewLine &
            AppPaths.StoreConfigFile,
            "Configuration Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

    End Sub

    Private Sub frmMain_FormClosing(
        ByVal sender As Object,
        ByVal e As FormClosingEventArgs
    ) Handles MyBase.FormClosing

        If Not _isOperationRunning Then
            Return
        End If

        Dim answer As DialogResult =
            MessageBox.Show(
                "มีการตรวจสอบ Connection กำลังทำงานอยู่" &
                Environment.NewLine &
                "ต้องการยกเลิกและปิดโปรแกรมหรือไม่?",
                "POS Deploy Tool",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question)

        If answer = DialogResult.No Then
            e.Cancel = True
            Return
        End If

        If _operationCancellation IsNot Nothing Then
            _operationCancellation.Cancel()
        End If

    End Sub

    Private Sub frmMain_FormClosed(
        ByVal sender As Object,
        ByVal e As FormClosedEventArgs
    ) Handles MyBase.FormClosed

        If _operationCancellation IsNot Nothing Then
            _operationCancellation.Dispose()
            _operationCancellation = Nothing
        End If

        FileLogger.Write("POSDeployTool closed.")

    End Sub

End Class