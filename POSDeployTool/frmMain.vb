Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows.Forms
Imports POSDeployTool.Models
Imports POSDeployTool.Services

Public Class frmMain

    Private ReadOnly _storeConfigService As StoreConfigService
    Private ReadOnly _storeBindingSource As BindingSource
    Private _stores As List(Of StoreInfo)

    Public Sub New()

        InitializeComponent()

        _storeConfigService = New StoreConfigService()
        _storeBindingSource = New BindingSource()
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

            lblStatus.Text = "Ready"

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

    Private Sub btnReload_Click(
        ByVal sender As Object,
        ByVal e As EventArgs
    ) Handles btnReload.Click

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
            String.Format("Selected: {0}", selectedCount)

        lblTotalCount.Text =
            String.Format("Total: {0}", dgvStores.Rows.Count)

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

    Private Sub AddLog(ByVal message As String)

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

    Private Sub HandleConfigurationError(ByVal ex As Exception)

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

    Private Sub frmMain_FormClosed(
        ByVal sender As Object,
        ByVal e As FormClosedEventArgs
    ) Handles MyBase.FormClosed

        FileLogger.Write("POSDeployTool closed.")

    End Sub

End Class