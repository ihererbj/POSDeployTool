Imports System.ComponentModel
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports POSDeployTool.Models
Imports POSDeployTool.Services
Imports POSDeployTool.Infrastructure
Imports POSDeployTool.Helpers
Imports Newtonsoft.Json

Public Class frmMain

    Private ReadOnly _services As AppServices = AppServices.Current
    Private ReadOnly _configService As StoreConfigService = _services.StoreConfig
    Private ReadOnly _connectionService As ConnectionCheckService = _services.Connection
    Private ReadOnly _backupService As BackupService = _services.Backup
    Private ReadOnly _deployService As DeployService = _services.Deploy
    Private ReadOnly _validationService As PreDeployValidationService = _services.Validation
    Private ReadOnly _retryService As RetryService = _services.Retry
    Private ReadOnly _historyService As DeploymentHistoryService = _services.History
    Private ReadOnly _versionService As VersionService = _services.Version
    Private ReadOnly _remoteInfoService As RemoteSystemInfoService = _services.RemoteInfo
    Private ReadOnly _remoteToolsService As RemoteToolsService = _services.RemoteTools
    Private ReadOnly _healthService As HealthCheckService = _services.Health

    ' Left navigation UI

    Private _allStores As New List(Of StoreInfo)()
    Private _stores As BindingList(Of StoreInfo)
    Private _checkCancellation As CancellationTokenSource
    Private _lastSortProperty As String = String.Empty
    Private _sortAscending As Boolean = True

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "POS Deploy Tool - Sprint 6 Enterprise Foundation [" & _services.Settings.EnvironmentName & "]"
        ShowDashboard()
        SetActiveNavigation(btnNavDashboard)

        txtConfigPath.Text = AppPaths.Resolve(_services.Settings.DefaultStoreConfigPath)
        txtPackagePath.Text = AppPaths.Resolve(_services.Settings.DefaultPackagePath)
        _services.Audit.Write("FORM_OPEN", NameOf(frmMain), "SUCCESS")

        ConfigureGrid()
        AddHandler dgvStores.CellFormatting, AddressOf dgvStores_HealthCellFormatting
        cboStatusFilter.Items.Clear()
        cboStatusFilter.Items.AddRange(New Object() {"All", "Online", "Offline", "Ready to deploy", "Validation failed", "Deploy OK", "Deploy failed", "Deploy skipped"})
        cboStatusFilter.SelectedIndex = 0
        LoadStoreData()
        SetCheckingState(False)
    End Sub

    Private Sub dgvStores_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgvStores.ColumnHeaderMouseClick
        If _stores Is Nothing OrElse e.ColumnIndex < 0 Then Return
        Dim prop = dgvStores.Columns(e.ColumnIndex).DataPropertyName
        If String.IsNullOrWhiteSpace(prop) OrElse prop = NameOf(StoreInfo.Selected) Then Return
        If String.Equals(_lastSortProperty, prop, StringComparison.Ordinal) Then
            _sortAscending = Not _sortAscending
        Else
            _lastSortProperty = prop
            _sortAscending = True
        End If
        Dim list = _stores.ToList()
        Dim key As Func(Of StoreInfo, Object) = Function(x) GetSortValue(x, prop)
        list = If(_sortAscending, list.OrderBy(key).ToList(), list.OrderByDescending(key).ToList())
        _stores = New BindingList(Of StoreInfo)(list)
        dgvStores.DataSource = _stores
        dgvStores.Columns(e.ColumnIndex).HeaderCell.SortGlyphDirection = If(_sortAscending, SortOrder.Ascending, SortOrder.Descending)
    End Sub

    Private Shared Function GetSortValue(item As StoreInfo, propertyName As String) As Object
        If propertyName = NameOf(StoreInfo.IpAddress) Then
            Dim bytes As Byte() = Nothing
            Dim address As System.Net.IPAddress = Nothing
            If System.Net.IPAddress.TryParse(item.IpAddress, address) Then
                bytes = address.GetAddressBytes()
                If bytes.Length = 4 Then Return CLng(bytes(0)) << 24 Or CLng(bytes(1)) << 16 Or CLng(bytes(2)) << 8 Or CLng(bytes(3))
            End If
        End If
        Return Convert.ToString(CallByName(item, propertyName, CallType.Get))
    End Function


    Private Sub SetActiveNavigation(button As Button)
        Dim navigationButtons As Button() = {
            btnNavDashboard,
            btnNavConnection,
            btnNavDeploy,
            btnNavLogs,
            btnNavSettings
        }

        For Each item As Button In navigationButtons
            item.BackColor = Color.FromArgb(28, 39, 54)
            item.ForeColor = Color.FromArgb(220, 226, 234)
            item.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular)
        Next

        button.BackColor = Color.FromArgb(42, 105, 168)
        button.ForeColor = Color.White
        button.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
    End Sub

    Private Sub btnNavDashboard_Click(sender As Object, e As EventArgs) Handles btnNavDashboard.Click
        SetActiveNavigation(btnNavDashboard)
        ShowDashboard()
    End Sub

    Private Sub btnNavConnection_Click(sender As Object, e As EventArgs) Handles btnNavConnection.Click
        SetActiveNavigation(btnNavConnection)
        ShowConnectionPage()
    End Sub

    Private Sub btnNavDeploy_Click(sender As Object, e As EventArgs) Handles btnNavDeploy.Click
        SetActiveNavigation(btnNavDeploy)
        ShowDeployPage()
    End Sub

    Private Sub btnNavLogs_Click(sender As Object, e As EventArgs) Handles btnNavLogs.Click
        SetActiveNavigation(btnNavLogs)
        ShowLogPage()
    End Sub

    Private Sub btnNavSettings_Click(sender As Object, e As EventArgs) Handles btnNavSettings.Click
        SetActiveNavigation(btnNavSettings)
        ShowSettingsPage()
    End Sub

    Private Sub ShowDashboard(Optional sender As Object = Nothing, Optional e As EventArgs = Nothing)
        lblTitle.Text = "Dashboard"
        SetPageRows(True, True, True, True, True, True, True, True)
    End Sub

    Private Sub ShowConnectionPage(Optional sender As Object = Nothing, Optional e As EventArgs = Nothing)
        lblTitle.Text = "Connection Check"
        SetPageRows(True, True, True, False, True, True, True, False)
    End Sub

    Private Sub ShowDeployPage(Optional sender As Object = Nothing, Optional e As EventArgs = Nothing)
        lblTitle.Text = "Deploy Program"
        SetPageRows(True, True, True, True, True, True, True, False)
    End Sub

    Private Sub ShowLogPage(Optional sender As Object = Nothing, Optional e As EventArgs = Nothing)
        lblTitle.Text = "Activity Logs"
        SetPageRows(False, False, False, False, False, False, False, True)
    End Sub

    Private Sub ShowSettingsPage(Optional sender As Object = Nothing, Optional e As EventArgs = Nothing)
        lblTitle.Text = "Settings"
        SetPageRows(True, False, False, True, False, False, False, False)
    End Sub

    Private Sub SetPageRows(showConfig As Boolean,
                            showActions As Boolean,
                            showFilter As Boolean,
                            showDeploy As Boolean,
                            showGrid As Boolean,
                            showStatus As Boolean,
                            showProgress As Boolean,
                            showLog As Boolean)
        configLayout.Visible = showConfig
        actionLayout.Visible = showActions
        filterLayout.Visible = showFilter
        deployLayout.Visible = showDeploy
        dgvStores.Visible = showGrid
        statusLayout.Visible = showStatus
        progressCheck.Visible = showProgress
        grpLog.Visible = showLog

        mainLayout.RowStyles(1).Height = If(showConfig, 42.0F, 0.0F)
        mainLayout.RowStyles(2).Height = If(showActions, 44.0F, 0.0F)
        mainLayout.RowStyles(3).Height = If(showFilter, 42.0F, 0.0F)
        mainLayout.RowStyles(4).Height = If(showDeploy, 40.0F, 0.0F)
        mainLayout.RowStyles(5).SizeType = SizeType.Percent
        mainLayout.RowStyles(5).Height = If(showGrid, If(showLog, 68.0F, 100.0F), 0.0F)
        mainLayout.RowStyles(6).Height = If(showStatus, 30.0F, 0.0F)
        mainLayout.RowStyles(7).Height = If(showProgress, 24.0F, 0.0F)
        mainLayout.RowStyles(8).SizeType = SizeType.Percent
        mainLayout.RowStyles(8).Height = If(showLog, If(showGrid, 32.0F, 100.0F), 0.0F)
    End Sub

    Private Sub ConfigureGrid()
        dgvStores.AutoGenerateColumns = False
        dgvStores.AllowUserToOrderColumns = True
        dgvStores.Columns.Clear()

        dgvStores.Columns.Add(New DataGridViewCheckBoxColumn() With {
            .DataPropertyName = NameOf(StoreInfo.Selected),
            .HeaderText = "เลือก",
            .Width = 50
        })

        dgvStores.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = NameOf(StoreInfo.StoreCode),
            .HeaderText = "Store",
            .Width = 75,
            .ReadOnly = True
        })

        dgvStores.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = NameOf(StoreInfo.StoreName),
            .HeaderText = "ชื่อสาขา",
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            .MinimumWidth = 140,
            .ReadOnly = True
        })

        dgvStores.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = NameOf(StoreInfo.IpAddress),
            .HeaderText = "IP Address",
            .Width = 120,
            .ReadOnly = True
        })

        dgvStores.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = NameOf(StoreInfo.Username),
            .HeaderText = "User",
            .Width = 90,
            .ReadOnly = True
        })

        dgvStores.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = NameOf(StoreInfo.PingStatus),
            .HeaderText = "Ping",
            .Width = 105,
            .ReadOnly = True
        })

        dgvStores.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = NameOf(StoreInfo.WinRmStatus),
            .HeaderText = "WinRM",
            .Width = 150,
            .ReadOnly = True
        })

        dgvStores.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = NameOf(StoreInfo.Status),
            .HeaderText = "Status",
            .Width = 110,
            .ReadOnly = True
        })

        dgvStores.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = NameOf(StoreInfo.ComputerName),
            .HeaderText = "Computer",
            .Width = 120,
            .ReadOnly = True
        })

        dgvStores.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = NameOf(StoreInfo.WindowsOS),
            .HeaderText = "Windows OS",
            .Width = 210,
            .ReadOnly = True
        })

        dgvStores.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = NameOf(StoreInfo.OSVersion),
            .HeaderText = "OS Version",
            .Width = 95,
            .ReadOnly = True
        })

        dgvStores.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = NameOf(StoreInfo.OSBuild),
            .HeaderText = "Build",
            .Width = 70,
            .ReadOnly = True
        })

        dgvStores.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = NameOf(StoreInfo.OSArchitecture),
            .HeaderText = "Architecture",
            .Width = 95,
            .ReadOnly = True
        })

        dgvStores.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = NameOf(StoreInfo.CurrentVersion),
            .HeaderText = "Current Version",
            .Width = 115,
            .ReadOnly = True
        })

        dgvStores.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = NameOf(StoreInfo.ReleaseVersion),
            .HeaderText = "Release Version",
            .Width = 115,
            .ReadOnly = True
        })

        dgvStores.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = NameOf(StoreInfo.FreeDiskGb),
            .HeaderText = "Free C: GB",
            .Width = 85,
            .ReadOnly = True
        })

        AddHealthColumn(NameOf(StoreInfo.CpuName), "CPU", 220)
        AddHealthColumn(NameOf(StoreInfo.CpuLoad), "CPU %", 65)
        AddHealthColumn(NameOf(StoreInfo.FreeRamGb), "RAM Free GB", 90)
        AddHealthColumn(NameOf(StoreInfo.TotalRamGb), "RAM Total GB", 90)
        AddHealthColumn(NameOf(StoreInfo.FreeDiskGb), "Disk C Free GB", 95)
        AddHealthColumn(NameOf(StoreInfo.TotalDiskGb), "Disk C Total GB", 95)
        AddHealthColumn(NameOf(StoreInfo.LastBoot), "Last Boot", 140)
        AddHealthColumn(NameOf(StoreInfo.Uptime), "Uptime", 85)
        AddHealthColumn(NameOf(StoreInfo.FirewallStatus), "Firewall", 75)
        AddHealthColumn(NameOf(StoreInfo.PosProcessStatus), "BJCBCPOS", 95)
        AddHealthColumn(NameOf(StoreInfo.HealthScore), "Score", 60)
        AddHealthColumn(NameOf(StoreInfo.HealthStatus), "Health", 75)

        dgvStores.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = NameOf(StoreInfo.ValidationStatus),
            .HeaderText = "Validation",
            .Width = 190,
            .ReadOnly = True
        })

        dgvStores.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = NameOf(StoreInfo.LastChecked),
            .HeaderText = "Last checked",
            .Width = 145,
            .ReadOnly = True
        })
    End Sub

    Private Sub LoadStoreData()
        Try
            Dim stores = _configService.LoadStores(txtConfigPath.Text.Trim())
            _allStores = stores.ToList()
            ApplyFilter()
            UpdateSummary()
            AddLog("โหลด stores.json สำเร็จ")
        Catch ex As Exception
            dgvStores.DataSource = Nothing
            lblSummary.Text = "โหลดข้อมูลไม่สำเร็จ"
            AddLog("ERROR: " & ex.Message)
            MessageBox.Show(
                ex.Message,
                "Load configuration error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )
        End Try
    End Sub

    Private Async Sub btnCheckSelected_Click(sender As Object, e As EventArgs) Handles btnCheckSelected.Click
        Await RunConnectionChecksAsync(False)
    End Sub

    Private Async Sub btnCheckPing_Click(sender As Object, e As EventArgs) Handles btnCheckPing.Click
        Await RunConnectionChecksAsync(True)
    End Sub

    Private Async Function RunConnectionChecksAsync(pingOnly As Boolean) As Task
        If _stores Is Nothing Then Return

        dgvStores.EndEdit()

        Dim selectedStores = _stores.
            Where(Function(item) item.Selected).
            ToList()

        If selectedStores.Count = 0 Then
            MessageBox.Show(
                "กรุณาเลือก Store อย่างน้อย 1 รายการ",
                "POS Deploy Tool",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            )
            Return
        End If

        _checkCancellation = New CancellationTokenSource()

        Dim token = _checkCancellation.Token
        Dim maxParallel = CInt(nudParallel.Value)
        Dim semaphore As New SemaphoreSlim(maxParallel)
        Dim completed As Integer = 0

        progressCheck.Minimum = 0
        progressCheck.Maximum = selectedStores.Count
        progressCheck.Value = 0

        SetCheckingState(True)

        AddLog(
            String.Format(
                "เริ่มตรวจสอบ {0} เครื่อง, Parallel={1}, Mode={2}",
                selectedStores.Count,
                maxParallel,
                If(pingOnly, "Ping", "Ping + WinRM + OS + Version")
            )
        )

        Try
            Dim tasks = selectedStores.
                Select(
                    Async Function(store)
                        Await semaphore.WaitAsync(token)

                        Try
                            Await CheckOneStoreAsync(store, pingOnly, token)
                        Finally
                            semaphore.Release()

                            Dim current =
                                Interlocked.Increment(completed)

                            UpdateProgress(
                                current,
                                selectedStores.Count,
                                store.IpAddress
                            )
                        End Try
                    End Function
                ).
                ToArray()

            Await Task.WhenAll(tasks)
            AddLog("ตรวจสอบ Connection เสร็จสิ้น")

        Catch ex As OperationCanceledException
            AddLog("ยกเลิกการตรวจสอบแล้ว")

        Catch ex As Exception
            AddLog("ERROR: " & ex.Message)

            MessageBox.Show(
                ex.Message,
                "Connection check error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )

        Finally
            semaphore.Dispose()

            If _checkCancellation IsNot Nothing Then
                _checkCancellation.Dispose()
                _checkCancellation = Nothing
            End If

            SetCheckingState(False)
            UpdateSummary()
        End Try
    End Function

    Private Async Function CheckOneStoreAsync(
        store As StoreInfo,
        pingOnly As Boolean,
        token As CancellationToken
    ) As Task

        store.Status = "Checking..."
        store.PingStatus = "Checking..."
        store.WinRmStatus = If(pingOnly, "Skipped", "Waiting...")

        AddLogThreadSafe(
            String.Format(
                "[{0}/{1}] Ping {2}",
                store.StoreCode,
                store.StoreName,
                store.IpAddress
            )
        )

        Dim pingResult =
            Await _connectionService.CheckPingAsync(
                store.IpAddress,
                2000,
                token
            )

        store.PingStatus = pingResult.Message

        If Not pingResult.IsSuccess Then
            store.WinRmStatus = "Skipped"
            store.Status = "Offline"
            store.LastChecked = Date.Now.ToString("yyyy-MM-dd HH:mm:ss")

            AddLogThreadSafe(
                String.Format(
                    "[{0}] Ping failed: {1}",
                    store.StoreCode,
                    pingResult.Message
                )
            )

            Return
        End If

        If pingOnly Then
            store.Status = "Online"
            store.LastChecked = Date.Now.ToString("yyyy-MM-dd HH:mm:ss")

            AddLogThreadSafe(
                String.Format(
                    "[{0}] Ping OK: {1}",
                    store.StoreCode,
                    pingResult.Message
                )
            )

            Return
        End If

        store.WinRmStatus = "Checking..."

        AddLogThreadSafe(
            String.Format(
                "[{0}/{1}] Test WinRM {2}",
                store.StoreCode,
                store.StoreName,
                store.IpAddress
            )
        )

        Dim winRmResult =
            Await _connectionService.CheckWinRmAsync(
                store,
                10000,
                token
            )

        store.WinRmStatus = winRmResult.Message
        If Not winRmResult.IsSuccess Then
            store.Status = "WinRM failed"
            store.LastChecked = Date.Now.ToString("yyyy-MM-dd HH:mm:ss")
            AddLogThreadSafe(String.Format("[{0}] WinRM failed: {1}", store.StoreCode, winRmResult.Message))
            Return
        End If

        store.Status = "Reading OS..."
        Dim remoteInfo = Await _retryService.ExecuteAsync(
            "Windows OS and BJCBCPOS version",
            Function() _remoteInfoService.GetSystemInfoAsync(store, txtProcessName.Text.Trim(), 30000, token),
            3, 1000, token,
            Sub(attempt, detail) AddLogThreadSafe(String.Format("[{0}] Retry {1}: {2}", store.StoreCode, attempt, detail)))

        If remoteInfo.IsSuccess Then
            Dim info = RemoteSystemInfoService.ParsePayload(remoteInfo.Message)
            store.ComputerName = info.ComputerName
            store.WindowsOS = info.WindowsOS
            store.OSVersion = info.OSVersion
            store.OSBuild = info.OSBuild
            store.OSArchitecture = info.OSArchitecture
            store.CurrentVersion = info.AppVersion
            store.Status = "Ready"
            AddLogThreadSafe(String.Format("[{0}] {1} | OS={2} {3} Build {4} {5} | BJCBCPOS={6}",
                                           store.StoreCode, info.ComputerName, info.WindowsOS, info.OSVersion,
                                           info.OSBuild, info.OSArchitecture, info.AppVersion))
        Else
            store.Status = "Remote info failed"
            AddLogThreadSafe(String.Format("[{0}] Remote info failed: {1}", store.StoreCode, remoteInfo.Message))
        End If
        store.LastChecked = Date.Now.ToString("yyyy-MM-dd HH:mm:ss")
    End Function

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        ApplyFilter()
    End Sub

    Private Sub cboStatusFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboStatusFilter.SelectedIndexChanged
        ApplyFilter()
    End Sub

    Private Sub ApplyFilter()
        If _allStores Is Nothing Then Return
        Dim search = txtSearch.Text.Trim()
        Dim status = If(cboStatusFilter.SelectedItem, "All").ToString()
        Dim filtered = _allStores.Where(Function(item)
                                            Dim matchText = String.IsNullOrWhiteSpace(search) OrElse item.StoreCode.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 OrElse item.StoreName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 OrElse item.IpAddress.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 OrElse item.WindowsOS.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 OrElse item.OSVersion.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 OrElse item.OSBuild.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 OrElse item.OSArchitecture.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 OrElse item.CurrentVersion.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                                            Dim matchStatus = status = "All" OrElse item.Status = status OrElse item.ValidationStatus = status
                                            Return matchText AndAlso matchStatus
                                        End Function).ToList()
        _stores = New BindingList(Of StoreInfo)(filtered)
        dgvStores.DataSource = _stores
        UpdateSummary()
    End Sub

    Private Async Sub btnValidateSelected_Click(sender As Object, e As EventArgs) Handles btnValidateSelected.Click
        If _stores Is Nothing Then Return
        dgvStores.EndEdit()
        Dim selected = _stores.Where(Function(x) x.Selected).ToList()
        If selected.Count = 0 Then
            MessageBox.Show("กรุณาเลือก Store อย่างน้อย 1 รายการ", "Validate", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim releasePath = txtPackagePath.Text.Trim()
        If Not Directory.Exists(releasePath) Then
            MessageBox.Show("ไม่พบ Release folder: " & releasePath, "Validate", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If Directory.GetFiles(releasePath, "*", SearchOption.AllDirectories).Length = 0 Then
            MessageBox.Show("Release folder ไม่มีไฟล์สำหรับ Deploy", "Validate", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        _checkCancellation = New CancellationTokenSource()
        Dim token = _checkCancellation.Token
        Dim semaphore As New SemaphoreSlim(CInt(nudParallel.Value))
        Dim completed As Integer = 0
        progressCheck.Minimum = 0 : progressCheck.Maximum = selected.Count : progressCheck.Value = 0
        SetCheckingState(True)
        AddLog(String.Format("เริ่ม Validate {0} เครื่อง", selected.Count))
        Try
            Dim tasks = selected.Select(Async Function(store)
                                            Await semaphore.WaitAsync(token)
                                            Try
                                                store.Status = "Validating..." : store.ValidationStatus = "Validating..."
                                                Dim r = Await _validationService.ValidateAsync(store, txtProcessName.Text.Trim(), releasePath, 1D, 3, token, Sub(attempt, detail) AddLogThreadSafe(String.Format("[{0}] Retry {1}: {2}", store.StoreCode, attempt, detail)))
                                                store.ComputerName = r.ComputerName
                                                store.CurrentVersion = r.CurrentVersion
                                                store.ReleaseVersion = r.ReleaseVersion
                                                store.FreeDiskGb = r.FreeDiskGb
                                                store.ValidationStatus = If(r.IsSuccess, "Ready to deploy", "Validation failed")
                                                store.Status = store.ValidationStatus
                                                store.LastChecked = Date.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                                AddLogThreadSafe(String.Format("[{0}/{1}] {2}: {3}", store.StoreCode, store.StoreName, store.ValidationStatus, r.Message))
                                            Finally
                                                semaphore.Release()
                                                Dim current = Interlocked.Increment(completed)
                                                UpdateProgress(current, selected.Count, store.IpAddress)
                                            End Try
                                        End Function).ToArray()
            Await Task.WhenAll(tasks)
            AddLog("Validate เสร็จสิ้น")
        Catch ex As OperationCanceledException
            AddLog("ยกเลิกการ Validate แล้ว")
        Catch ex As Exception
            AddLog("Validate ERROR: " & ex.Message)
            MessageBox.Show(ex.Message, "Validate error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            semaphore.Dispose()
            If _checkCancellation IsNot Nothing Then _checkCancellation.Dispose() : _checkCancellation = Nothing
            SetCheckingState(False)
            UpdateSummary()
        End Try
    End Sub

    Private Async Sub btnBackupSelected_Click(sender As Object, e As EventArgs) Handles btnBackupSelected.Click
        Await RunBackupAsync()
    End Sub

    Private Async Function RunBackupAsync() As Task
        If _stores Is Nothing Then Return

        dgvStores.EndEdit()

        Dim selectedStores = _stores.
            Where(Function(item) item.Selected).
            ToList()

        If selectedStores.Count = 0 Then
            MessageBox.Show(
                "กรุณาเลือก Store อย่างน้อย 1 รายการ",
                "POS Deploy Tool",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            )
            Return
        End If

        Dim backupDate As Date = Date.Today
        Dim backupName As String =
            "C:\BJCBCPOS_" & backupDate.ToString("yyyyMMdd")

        Dim confirmMessage As String =
            String.Format(
                "ต้องการ Backup {0} เครื่องหรือไม่?{1}{1}" &
                "C:\BJCBCPOS{1}ไปเป็น{1}{2}{1}{1}" &
                "หากโฟลเดอร์ Backup มีอยู่แล้ว ระบบจะไม่เขียนทับ",
                selectedStores.Count,
                Environment.NewLine,
                backupName
            )

        If MessageBox.Show(
            confirmMessage,
            "Confirm backup",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        ) <> DialogResult.Yes Then
            Return
        End If

        _checkCancellation = New CancellationTokenSource()

        Dim token = _checkCancellation.Token
        Dim maxParallel = CInt(nudParallel.Value)
        Dim semaphore As New SemaphoreSlim(maxParallel)
        Dim completed As Integer = 0

        progressCheck.Minimum = 0
        progressCheck.Maximum = selectedStores.Count
        progressCheck.Value = 0

        SetCheckingState(True)

        AddLog(
            String.Format(
                "เริ่ม Backup {0} เครื่อง, Parallel={1}, วันที่={2:yyyyMMdd}",
                selectedStores.Count,
                maxParallel,
                backupDate
            )
        )

        Try
            Dim tasks = selectedStores.
                Select(
                    Async Function(store)
                        Await semaphore.WaitAsync(token)

                        Try
                            Await BackupOneStoreAsync(
                                store,
                                backupDate,
                                token
                            )
                        Finally
                            semaphore.Release()

                            Dim current =
                                Interlocked.Increment(completed)

                            UpdateProgress(
                                current,
                                selectedStores.Count,
                                store.IpAddress
                            )
                        End Try
                    End Function
                ).
                ToArray()

            Await Task.WhenAll(tasks)
            AddLog("Backup เสร็จสิ้น")

        Catch ex As OperationCanceledException
            AddLog("ยกเลิกการ Backup แล้ว")

        Catch ex As Exception
            AddLog("ERROR: " & ex.Message)

            MessageBox.Show(
                ex.Message,
                "Backup error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )

        Finally
            semaphore.Dispose()

            If _checkCancellation IsNot Nothing Then
                _checkCancellation.Dispose()
                _checkCancellation = Nothing
            End If

            SetCheckingState(False)
            UpdateSummary()
        End Try
    End Function

    Private Async Function BackupOneStoreAsync(
        store As StoreInfo,
        backupDate As Date,
        token As CancellationToken
    ) As Task

        store.Status = "Backing up..."
        store.LastChecked = Date.Now.ToString("yyyy-MM-dd HH:mm:ss")

        AddLogThreadSafe(
            String.Format(
                "[{0}/{1}] Backup {2} -> {2}_{3}",
                store.StoreCode,
                store.StoreName,
                store.TargetPath,
                backupDate.ToString("yyyyMMdd")
            )
        )

        Dim result As CheckResult =
            Await _backupService.BackupAsync(
                store,
                backupDate,
                600000,
                token
            )

        store.Status =
            If(result.IsSuccess, "Backup OK", "Backup failed")

        store.LastChecked =
            Date.Now.ToString("yyyy-MM-dd HH:mm:ss")

        AddLogThreadSafe(
            String.Format(
                "[{0}/{1}] {2}: {3}",
                store.StoreCode,
                store.StoreName,
                store.Status,
                result.Message
            )
        )
    End Function

    Private Sub btnBrowsePackage_Click(sender As Object, e As EventArgs) Handles btnBrowsePackage.Click
        Using dialog As New FolderBrowserDialog()
            dialog.Description =
                "เลือกโฟลเดอร์ Release ที่จะ Deploy"

            dialog.SelectedPath =
                txtPackagePath.Text

            If dialog.ShowDialog(Me) = DialogResult.OK Then
                txtPackagePath.Text = dialog.SelectedPath
            End If
        End Using
    End Sub

    Private Async Sub btnDeploySelected_Click(sender As Object, e As EventArgs) Handles btnDeploySelected.Click
        Await RunDeployAsync()
    End Sub

    Private Async Function RunDeployAsync() As Task
        If _stores Is Nothing Then Return

        dgvStores.EndEdit()

        Dim packagePath As String = txtPackagePath.Text.Trim()
        Dim processName As String = txtProcessName.Text.Trim()

        If Not Directory.Exists(packagePath) Then
            MessageBox.Show("ไม่พบ Release folder: " & packagePath, "Deploy", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Directory.GetFiles(packagePath, "*", SearchOption.AllDirectories).Length = 0 Then
            MessageBox.Show("Release folder ไม่มีไฟล์สำหรับ Deploy", "Deploy", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim selectedStores = _stores.Where(Function(item) item.Selected).ToList()
        If selectedStores.Count = 0 Then
            MessageBox.Show("กรุณาเลือก Store อย่างน้อย 1 รายการ", "Deploy", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim notReady = selectedStores.Where(Function(item) item.ValidationStatus <> "Ready to deploy").ToList()
        If notReady.Count > 0 Then
            MessageBox.Show(String.Format("มี {0} เครื่องที่ยังไม่ผ่าน Validate กรุณากด Validate Selected ก่อน Deploy", notReady.Count), "Deploy validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim releaseVersion As String = _versionService.GetReleaseVersion(packagePath, processName)
        For Each store In selectedStores
            store.ReleaseVersion = releaseVersion
        Next

        Dim sameVersion = selectedStores.Where(Function(item) VersionsEqual(item.CurrentVersion, releaseVersion)).ToList()
        Dim skippedStores As New List(Of StoreInfo)()
        Dim forceSameVersion As Boolean = chkForceSameVersion.Checked

        If sameVersion.Count > 0 Then
            If forceSameVersion Then
                Dim forceMessage As String = String.Format(
                    "เลือก Option Deploy same version อยู่{0}{0}" &
                    "มี {1} เครื่องที่ Version เท่ากับ Release Version ({2}){0}" &
                    "ระบบจะ Backup และ Deploy ซ้ำให้เครื่องเหล่านี้{0}{0}" &
                    "ยืนยัน Force Deploy หรือไม่?",
                    Environment.NewLine,
                    sameVersion.Count,
                    releaseVersion)

                If MessageBox.Show(
                    forceMessage,
                    "Confirm same-version deploy",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                ) <> DialogResult.Yes Then
                    Return
                End If

                AddLog(String.Format(
                    "Force Deploy same version enabled: {0} เครื่อง, Version={1}",
                    sameVersion.Count,
                    releaseVersion))
            Else
                skippedStores = sameVersion
                selectedStores = selectedStores.Except(skippedStores).ToList()

                For Each store In skippedStores
                    store.Status = "Deploy skipped"
                    store.ValidationStatus = "Skipped - same version"
                    store.LastChecked = Date.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    WriteSkippedHistory(store, releaseVersion, "Current version equals release version; force option not selected")
                    AddLog(String.Format(
                        "[{0}/{1}] Deploy skipped: Current Version = Release Version ({2})",
                        store.StoreCode,
                        store.StoreName,
                        releaseVersion))
                Next
            End If
        End If

        If selectedStores.Count = 0 Then
            MessageBox.Show(String.Format("ไม่มีเครื่องที่ต้อง Deploy{0}Skipped: {1}", Environment.NewLine, skippedStores.Count), "Deploy summary", MessageBoxButtons.OK, MessageBoxIcon.Information)
            UpdateSummary()
            Return
        End If

        Dim message As String = String.Format(
            "Deploy {0} เครื่องหรือไม่?{1}{1}Release: {2}{1}Release Version: {3}{1}Target: C:\BJCBCPOS{1}Process: {4}{1}Start after deploy: {5}{1}Deploy same version: {6}{1}{1}ระบบจะ Backup ก่อน Deploy ทุกครั้ง และ Retry สูงสุด 3 ครั้ง",
            selectedStores.Count, Environment.NewLine, packagePath, releaseVersion, processName, If(chkStartAfterDeploy.Checked, "Yes", "No"), If(forceSameVersion, "Yes (Force Deploy)", "No (Skip same version)"))

        If MessageBox.Show(message, "Confirm deploy", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) <> DialogResult.Yes Then Return

        _checkCancellation = New CancellationTokenSource()
        Dim token = _checkCancellation.Token
        Dim maxParallel = CInt(nudParallel.Value)
        Dim semaphore As New SemaphoreSlim(maxParallel)
        Dim completed As Integer = 0
        Dim successCount As Integer = 0
        Dim failedCount As Integer = 0
        Dim runWatch As Stopwatch = Stopwatch.StartNew()

        progressCheck.Minimum = 0
        progressCheck.Maximum = selectedStores.Count
        progressCheck.Value = 0
        SetCheckingState(True)
        AddLog(String.Format("เริ่ม Deploy {0} เครื่อง, Parallel={1}, ReleaseVersion={2}", selectedStores.Count, maxParallel, releaseVersion))

        Try
            Dim tasks = selectedStores.Select(
                Async Function(store)
                    Await semaphore.WaitAsync(token)
                    Try
                        Dim result = Await DeployOneStoreAsync(store, packagePath, processName, chkStartAfterDeploy.Checked, releaseVersion, token)
                        If result.IsSuccess Then
                            Interlocked.Increment(successCount)
                        Else
                            Interlocked.Increment(failedCount)
                        End If
                    Finally
                        semaphore.Release()
                        Dim current = Interlocked.Increment(completed)
                        UpdateProgress(current, selectedStores.Count, store.IpAddress)
                    End Try
                End Function).ToArray()

            Await Task.WhenAll(tasks)
            AddLog("Deploy เสร็จสิ้น")
        Catch ex As OperationCanceledException
            AddLog("ยกเลิกการ Deploy แล้ว")
        Catch ex As Exception
            AddLog("ERROR: " & ex.Message)
            MessageBox.Show(ex.Message, "Deploy error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            runWatch.Stop()
            semaphore.Dispose()
            If _checkCancellation IsNot Nothing Then
                _checkCancellation.Dispose()
                _checkCancellation = Nothing
            End If
            SetCheckingState(False)
            UpdateSummary()
        End Try

        MessageBox.Show(
            String.Format("Deploy Finished{0}{0}Success: {1}{0}Failed: {2}{0}Skipped: {3}{0}Elapsed: {4}", Environment.NewLine, successCount, failedCount, skippedStores.Count, runWatch.Elapsed.ToString("hh\:mm\:ss")),
            "Deploy summary",
            MessageBoxButtons.OK,
            If(failedCount = 0, MessageBoxIcon.Information, MessageBoxIcon.Warning))
    End Function

    Private Async Function DeployOneStoreAsync(
        store As StoreInfo,
        packagePath As String,
        processName As String,
        startAfterDeploy As Boolean,
        releaseVersion As String,
        token As CancellationToken
    ) As Task(Of CheckResult)

        Dim startedAt As DateTime = Date.Now
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim finalResult As CheckResult = Nothing
        Dim oldVersion As String = store.CurrentVersion

        store.Status = "Deploying..."
        store.LastChecked = Date.Now.ToString("yyyy-MM-dd HH:mm:ss")

        Try
            AddLogThreadSafe(String.Format("[{0}/{1}] Step 1/4 Backup", store.StoreCode, store.StoreName))
            Dim backupResult = Await _retryService.ExecuteAsync(
                "Backup",
                Function() _backupService.BackupAsync(store, Date.Today, 600000, token),
                3, 2000, token,
                Sub(attempt, detail) AddLogThreadSafe(String.Format("[{0}] Retry {1}/3 {2}", store.StoreCode, attempt, detail)))

            If Not backupResult.IsSuccess Then
                finalResult = CheckResult.Fail("Backup failed: " & backupResult.Message)
            End If

            If finalResult Is Nothing Then
                AddLogThreadSafe(String.Format("[{0}/{1}] Step 2/4 Stop {2}", store.StoreCode, store.StoreName, processName))
                Dim stopResult = Await _retryService.ExecuteAsync(
                    "Stop process",
                    Function() _deployService.StopProcessAsync(store, processName, 30000, token),
                    3, 1500, token,
                    Sub(attempt, detail) AddLogThreadSafe(String.Format("[{0}] Retry {1}/3 {2}", store.StoreCode, attempt, detail)))
                If Not stopResult.IsSuccess Then finalResult = CheckResult.Fail("Stop failed: " & stopResult.Message)
            End If

            If finalResult Is Nothing Then
                AddLogThreadSafe(String.Format("[{0}/{1}] Step 3/4 Copy + Verify", store.StoreCode, store.StoreName))
                Dim copyResult = Await _retryService.ExecuteAsync(
                    "Copy package",
                    Function() _deployService.CopyPackageAsync(store, packagePath, 900000, token),
                    3, 3000, token,
                    Sub(attempt, detail) AddLogThreadSafe(String.Format("[{0}] Retry {1}/3 {2}", store.StoreCode, attempt, detail)))
                If Not copyResult.IsSuccess Then finalResult = CheckResult.Fail("Copy failed: " & copyResult.Message)
            End If

            If finalResult Is Nothing AndAlso startAfterDeploy Then
                AddLogThreadSafe(String.Format("[{0}/{1}] Step 4/4 Start + Verify process", store.StoreCode, store.StoreName))
                Dim startResult = Await _retryService.ExecuteAsync(
                    "Start process",
                    Function() _deployService.StartProcessAsync(store, processName, 30000, token),
                    3, 2000, token,
                    Sub(attempt, detail) AddLogThreadSafe(String.Format("[{0}] Retry {1}/3 {2}", store.StoreCode, attempt, detail)))
                If Not startResult.IsSuccess Then finalResult = CheckResult.Fail("Start failed: " & startResult.Message)
            ElseIf finalResult Is Nothing Then
                AddLogThreadSafe(String.Format("[{0}/{1}] Step 4/4 Start skipped", store.StoreCode, store.StoreName))
            End If

            If finalResult Is Nothing Then finalResult = CheckResult.Success("Deploy completed")
        Catch ex As OperationCanceledException
            Throw
        Catch ex As Exception
            finalResult = CheckResult.Fail(ex.Message)
        End Try

        watch.Stop()
        store.Status = If(finalResult.IsSuccess, "Deploy OK", "Deploy failed")
        store.LastChecked = Date.Now.ToString("yyyy-MM-dd HH:mm:ss")
        If finalResult.IsSuccess Then store.CurrentVersion = releaseVersion

        AddLogThreadSafe(String.Format("[{0}/{1}] {2}: {3} ({4:0.00}s)", store.StoreCode, store.StoreName, store.Status, finalResult.Message, watch.Elapsed.TotalSeconds))

        Try
            _historyService.Append(New DeploymentRecord With {
                .StartedAt = startedAt,
                .FinishedAt = Date.Now,
                .StoreCode = store.StoreCode,
                .StoreName = store.StoreName,
                .IpAddress = store.IpAddress,
                .ComputerName = store.ComputerName,
                .OldVersion = oldVersion,
                .ReleaseVersion = releaseVersion,
                .Result = store.Status,
                .Message = finalResult.Message,
                .ElapsedSeconds = watch.Elapsed.TotalSeconds,
                .OperatorName = Environment.UserName
            })
        Catch ex As Exception
            AddLogThreadSafe(String.Format("[{0}] History warning: {1}", store.StoreCode, ex.Message))
        End Try

        Return finalResult
    End Function

    Private Shared Function VersionsEqual(currentVersion As String, releaseVersion As String) As Boolean
        If String.IsNullOrWhiteSpace(currentVersion) OrElse String.IsNullOrWhiteSpace(releaseVersion) Then Return False
        If String.Equals(currentVersion, "Not found", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(releaseVersion, "Not found", StringComparison.OrdinalIgnoreCase) Then Return False
        Return String.Equals(currentVersion.Trim(), releaseVersion.Trim(), StringComparison.OrdinalIgnoreCase)
    End Function

    Private Sub WriteSkippedHistory(store As StoreInfo, releaseVersion As String, reason As String)
        Try
            _historyService.Append(New DeploymentRecord With {
                .StartedAt = Date.Now,
                .FinishedAt = Date.Now,
                .StoreCode = store.StoreCode,
                .StoreName = store.StoreName,
                .IpAddress = store.IpAddress,
                .ComputerName = store.ComputerName,
                .OldVersion = store.CurrentVersion,
                .ReleaseVersion = releaseVersion,
                .Result = "Deploy skipped",
                .Message = reason,
                .ElapsedSeconds = 0,
                .OperatorName = Environment.UserName
            })
        Catch ex As Exception
            AddLog("History warning: " & ex.Message)
        End Try
    End Sub

    Private Sub UpdateProgress(
        completed As Integer,
        total As Integer,
        ipAddress As String
    )

        If InvokeRequired Then
            BeginInvoke(
                New Action(Of Integer, Integer, String)(
                    AddressOf UpdateProgress
                ),
                completed,
                total,
                ipAddress
            )
            Return
        End If

        progressCheck.Value =
            Math.Min(completed, progressCheck.Maximum)

        lblProgress.Text =
            String.Format(
                "{0}/{1} - {2}",
                completed,
                total,
                ipAddress
            )

        UpdateSummary()
    End Sub


    Private Sub btnRemoteTools_Click(sender As Object, e As EventArgs) Handles btnRemoteTools.Click
        remoteToolsMenu.Show(btnRemoteTools, New Point(0, btnRemoteTools.Height))
    End Sub

    Private Async Sub RemoteToolMenuItem_Click(sender As Object, e As EventArgs) Handles _
        mnuRemoteCheck.Click, mnuRemoteTaskInstall.Click, mnuRemoteTaskRemove.Click, _
        mnuRemoteStart.Click, mnuRemoteStop.Click, mnuRemoteRestart.Click, _
        mnuRemoteReboot.Click, mnuRemoteAbort.Click, _
        mnuRemoteServiceStart.Click, mnuRemoteServiceStop.Click, mnuRemoteServiceRestart.Click, _
        mnuRemoteHealth.Click, mnuRemoteNetwork.Click, mnuRemoteProcesses.Click, mnuRemoteServices.Click, _
        mnuRemoteKillProcess.Click, mnuRemoteCustom.Click

        Dim item As ToolStripMenuItem = TryCast(sender, ToolStripMenuItem)
        If item Is Nothing Then Return
        Await RunRemoteToolAsync(Convert.ToString(item.Tag))
    End Sub

    Private Async Function RunRemoteToolAsync(actionName As String) As Task
        If _stores Is Nothing Then Return

        dgvStores.EndEdit()
        Dim selectedStores As List(Of StoreInfo) = _stores.Where(Function(x) x.Selected).ToList()

        If selectedStores.Count = 0 AndAlso dgvStores.CurrentRow IsNot Nothing Then
            Dim currentStore As StoreInfo = TryCast(dgvStores.CurrentRow.DataBoundItem, StoreInfo)
            If currentStore IsNot Nothing Then selectedStores.Add(currentStore)
        End If

        If selectedStores.Count = 0 Then
            MessageBox.Show("กรุณาเลือก Store อย่างน้อย 1 รายการ", "Remote Tools", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim processName As String = txtProcessName.Text.Trim()
        Dim serviceName As String = String.Empty
        Dim customCommand As String = String.Empty
        Dim displayName As String = GetRemoteActionDisplayName(actionName)

        If actionName = "service" OrElse actionName = "service_start" OrElse actionName = "service_stop" Then
            serviceName = Microsoft.VisualBasic.Interaction.InputBox(
                "กรอก Service Name เช่น Spooler, jetty6-service หรือ LanmanServer",
                displayName,
                "Spooler")
            If String.IsNullOrWhiteSpace(serviceName) Then Return
            displayName &= " (" & serviceName.Trim() & ")"
        ElseIf actionName = "killprocess" Then
            processName = Microsoft.VisualBasic.Interaction.InputBox(
                "กรอก Process Name เช่น notepad.exe หรือ BJCBCPOS.exe",
                "Kill Process by Name",
                If(String.IsNullOrWhiteSpace(processName), "BJCBCPOS.exe", processName))
            If String.IsNullOrWhiteSpace(processName) Then Return
            displayName &= " (" & processName.Trim() & ")"
        ElseIf actionName = "custom" Then
            customCommand = Microsoft.VisualBasic.Interaction.InputBox(
                "กรอกคำสั่ง CMD หรือ PowerShell ที่ต้องการทำงานบนเครื่องปลายทาง",
                "Execute Custom Command",
                "whoami")
            If String.IsNullOrWhiteSpace(customCommand) Then Return
        End If

        If actionName = "stop" OrElse actionName = "restart" OrElse actionName = "reboot" OrElse
           actionName = "task_install" OrElse actionName = "task_remove" OrElse
           actionName = "service" OrElse actionName = "service_start" OrElse actionName = "service_stop" OrElse
           actionName = "killprocess" OrElse actionName = "custom" Then
            Dim confirmText As String = String.Format(
                "ยืนยันคำสั่ง {0} จำนวน {1} เครื่องหรือไม่?",
                displayName,
                selectedStores.Count)
            If actionName = "reboot" Then
                confirmText &= Environment.NewLine & Environment.NewLine & "เครื่อง POS จะ Restart ภายใน 5 วินาที"
            End If
            If MessageBox.Show(confirmText, "Confirm Remote Tool", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) <> DialogResult.Yes Then Return
        End If

        _checkCancellation = New CancellationTokenSource()
        Dim token As CancellationToken = _checkCancellation.Token
        Dim maxParallel As Integer = CInt(nudParallel.Value)
        Dim semaphore As New SemaphoreSlim(maxParallel)
        Dim completed As Integer = 0

        progressCheck.Minimum = 0
        progressCheck.Maximum = selectedStores.Count
        progressCheck.Value = 0
        SetCheckingState(True)
        AddLog(String.Format("Remote Tools: {0}, Store={1}, Parallel={2}", displayName, selectedStores.Count, maxParallel))

        Try
            Dim tasks = selectedStores.Select(
                Async Function(store)
                    Await semaphore.WaitAsync(token)
                    Try
                        Await RunRemoteToolForStoreAsync(store, actionName, processName, serviceName, customCommand, token)
                    Finally
                        semaphore.Release()
                        Dim current As Integer = Interlocked.Increment(completed)
                        UpdateProgress(current, selectedStores.Count, store.IpAddress)
                    End Try
                End Function).ToArray()

            Await Task.WhenAll(tasks)
            AddLog("Remote Tools เสร็จสิ้น: " & displayName)

        Catch ex As OperationCanceledException
            AddLog("ยกเลิก Remote Tools แล้ว")
        Catch ex As Exception
            AddLog("Remote Tools ERROR: " & ex.Message)
            MessageBox.Show(ex.Message, "Remote Tools", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            semaphore.Dispose()
            If _checkCancellation IsNot Nothing Then
                _checkCancellation.Dispose()
                _checkCancellation = Nothing
            End If
            SetCheckingState(False)
            UpdateSummary()
        End Try
    End Function

    Private Async Function RunRemoteToolForStoreAsync(store As StoreInfo,
                                                       actionName As String,
                                                       processName As String,
                                                       serviceName As String,
                                                       customCommand As String,
                                                       token As CancellationToken) As Task
        store.Status = "Remote Tool..."
        store.LastChecked = Date.Now.ToString("yyyy-MM-dd HH:mm:ss")
        AddLogThreadSafe(String.Format("[{0}/{1}] {2}", store.StoreCode, store.StoreName, GetRemoteActionDisplayName(actionName)))

        Dim result As CheckResult
        Select Case actionName
            Case "check"
                result = Await _remoteToolsService.CheckProcessAsync(store, processName, 30000, token)
            Case "task_install"
                result = Await _remoteToolsService.InstallBjcTaskAsync(store, 60000, token)
            Case "task_remove"
                result = Await _remoteToolsService.RemoveBjcTaskAsync(store, 30000, token)
            Case "start"
                result = Await _remoteToolsService.StartProcessAsync(store, processName, 30000, token)
            Case "stop"
                result = Await _remoteToolsService.StopProcessAsync(store, processName, 30000, token)
            Case "restart"
                result = Await _remoteToolsService.RestartProcessAsync(store, processName, 30000, token)
            Case "reboot"
                result = Await _remoteToolsService.RestartWindowsAsync(store, 30000, token)
            Case "abort"
                result = Await _remoteToolsService.CancelShutdownAsync(store, 30000, token)
            Case "service_start"
                result = Await _remoteToolsService.StartServiceAsync(store, serviceName, 60000, token)
            Case "service_stop"
                result = Await _remoteToolsService.StopServiceAsync(store, serviceName, 60000, token)
            Case "service"
                result = Await _remoteToolsService.RestartServiceAsync(store, serviceName, 60000, token)
            Case "health"
                result = Await _remoteToolsService.GetSystemHealthAsync(store, 60000, token)
            Case "network"
                result = Await _remoteToolsService.GetNetworkInfoAsync(store, 60000, token)
            Case "processes"
                result = Await _remoteToolsService.ListProcessesAsync(store, 60000, token)
            Case "services"
                result = Await _remoteToolsService.ListServicesAsync(store, 60000, token)
            Case "killprocess"
                result = Await _remoteToolsService.KillProcessByNameAsync(store, processName, 30000, token)
            Case "custom"
                result = Await _remoteToolsService.ExecuteCustomCommandAsync(store, customCommand, 120000, token)
            Case Else
                result = CheckResult.Fail("Unknown remote action: " & actionName)
        End Select

        store.Status = If(result.IsSuccess, "Remote Tool OK", "Remote Tool failed")
        store.LastChecked = Date.Now.ToString("yyyy-MM-dd HH:mm:ss")
        AddLogThreadSafe(String.Format("[{0}/{1}] {2}: {3}", store.StoreCode, store.StoreName, store.Status, result.Message))
    End Function

    Private Shared Function GetRemoteActionDisplayName(actionName As String) As String
        Select Case actionName
            Case "check" : Return "Check BJCBCPOS Process"
            Case "task_install" : Return "Install BJCBCPOS Task (No Trigger)"
            Case "task_remove" : Return "Remove BJCBCPOS Task"
            Case "start" : Return "Start BJCBCPOS"
            Case "stop" : Return "Stop BJCBCPOS"
            Case "restart" : Return "Restart BJCBCPOS"
            Case "reboot" : Return "Restart Windows"
            Case "abort" : Return "Cancel Windows Restart"
            Case "service_start" : Return "Start Windows Service"
            Case "service_stop" : Return "Stop Windows Service"
            Case "service" : Return "Restart Windows Service"
            Case "health" : Return "System Health / Hardware Info"
            Case "network" : Return "Network Configuration"
            Case "processes" : Return "List Running Processes"
            Case "services" : Return "List Windows Services"
            Case "killprocess" : Return "Kill Process by Name"
            Case "custom" : Return "Execute Custom Command"
            Case Else : Return actionName
        End Select
    End Function

    Private Sub AddHealthColumn(propertyName As String, headerText As String, width As Integer)
        dgvStores.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = propertyName, .HeaderText = headerText, .Width = width, .ReadOnly = True
        })
    End Sub

    Private Async Sub btnCheckHealth_Click(sender As Object, e As EventArgs) Handles btnCheckHealth.Click
        Await RunHealthCheckAsync()
    End Sub

    Private Async Function RunHealthCheckAsync() As Task
        If _stores Is Nothing Then Return
        dgvStores.EndEdit()
        Dim selectedStores = _stores.Where(Function(x) x.Selected).ToList()
        If selectedStores.Count = 0 Then
            MessageBox.Show("กรุณาเลือก Store อย่างน้อย 1 รายการ", "System Health", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        _checkCancellation = New CancellationTokenSource()
        Dim token = _checkCancellation.Token
        Dim semaphore As New SemaphoreSlim(CInt(nudParallel.Value))
        Dim completed As Integer = 0
        progressCheck.Minimum = 0 : progressCheck.Maximum = selectedStores.Count : progressCheck.Value = 0
        SetCheckingState(True)
        AddLog(String.Format("เริ่ม System Health {0} เครื่อง", selectedStores.Count))
        Try
            Dim tasks = selectedStores.Select(Async Function(store)
                Await semaphore.WaitAsync(token)
                Try
                    store.HealthStatus = "Checking"
                    Dim result = Await _healthService.CheckAsync(store, txtProcessName.Text.Trim(), 45000, token)
                    If result.IsSuccess Then
                        Dim h = HealthCheckService.Parse(result.Message)
                        store.CpuName = h.CpuName
                        store.CpuLoad = If(h.CpuLoadPercent < 0, "N/A", h.CpuLoadPercent.ToString())
                        store.TotalRamGb = h.TotalRamGb.ToString("0.00")
                        store.FreeRamGb = h.FreeRamGb.ToString("0.00")
                        store.TotalDiskGb = h.TotalDiskGb.ToString("0.00")
                        store.FreeDiskGb = h.FreeDiskGb.ToString("0.00")
                        store.LastBoot = h.LastBoot
                        store.Uptime = h.Uptime
                        store.FirewallStatus = h.FirewallStatus
                        store.PosProcessStatus = h.PosProcessStatus
                        store.HealthScore = h.HealthScore.ToString()
                        store.HealthStatus = h.HealthStatus
                        store.Status = "Health " & h.HealthStatus
                        AddLogThreadSafe(String.Format("[{0}] Health={1} Score={2} CPU={3}% RAM Free={4}GB Disk Free={5}GB POS={6}", store.StoreCode, h.HealthStatus, h.HealthScore, h.CpuLoadPercent, h.FreeRamGb, h.FreeDiskGb, h.PosProcessStatus))
                    Else
                        store.HealthStatus = "Failed"
                        store.Status = "Health failed"
                        AddLogThreadSafe(String.Format("[{0}] Health failed: {1}", store.StoreCode, result.Message))
                    End If
                    store.LastChecked = Date.Now.ToString("yyyy-MM-dd HH:mm:ss")
                Finally
                    semaphore.Release()
                    Dim current = Interlocked.Increment(completed)
                    UpdateProgress(current, selectedStores.Count, store.IpAddress)
                End Try
            End Function).ToArray()
            Await Task.WhenAll(tasks)
            AddLog("System Health เสร็จสิ้น")
        Catch ex As OperationCanceledException
            AddLog("ยกเลิก System Health แล้ว")
        Catch ex As Exception
            AddLog("System Health ERROR: " & ex.Message)
            MessageBox.Show(ex.Message, "System Health", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            semaphore.Dispose()
            If _checkCancellation IsNot Nothing Then _checkCancellation.Dispose() : _checkCancellation = Nothing
            SetCheckingState(False)
            UpdateSummary()
            dgvStores.Refresh()
        End Try
    End Function

    Private Sub ExportHealth_Click(sender As Object, e As EventArgs) Handles btnExportHealth.Click
        If _allStores Is Nothing OrElse _allStores.Count = 0 Then Return
        Using dialog As New SaveFileDialog()
            dialog.Filter = "CSV files (*.csv)|*.csv"
            dialog.FileName = "HealthCheck_" & Date.Now.ToString("yyyyMMdd_HHmmss") & ".csv"
            If dialog.ShowDialog(Me) <> DialogResult.OK Then Return
            Using writer As New StreamWriter(dialog.FileName, False, New System.Text.UTF8Encoding(True))
                writer.WriteLine("StoreCode,StoreName,IP,Computer,WindowsOS,CPUPercent,TotalRAMGB,FreeRAMGB,TotalDiskGB,FreeDiskGB,Uptime,Firewall,BJCBCPOS,HealthScore,HealthStatus,LastChecked")
                For Each x In _allStores
                    writer.WriteLine(String.Join(",", New String() {Csv(x.StoreCode), Csv(x.StoreName), Csv(x.IpAddress), Csv(x.ComputerName), Csv(x.WindowsOS), Csv(x.CpuLoad), Csv(x.TotalRamGb), Csv(x.FreeRamGb), Csv(x.TotalDiskGb), Csv(x.FreeDiskGb), Csv(x.Uptime), Csv(x.FirewallStatus), Csv(x.PosProcessStatus), Csv(x.HealthScore), Csv(x.HealthStatus), Csv(x.LastChecked)}))
                Next
            End Using
            AddLog("Export Health: " & dialog.FileName)
            MessageBox.Show("Export สำเร็จ", "System Health", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Using
    End Sub

    Private Shared Function Csv(value As String) As String
        Return ChrW(34) & If(value, String.Empty).Replace(ChrW(34), ChrW(34) & ChrW(34)) & ChrW(34)
    End Function

    Private Sub dgvStores_HealthCellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        Dim prop = dgvStores.Columns(e.ColumnIndex).DataPropertyName
        Dim store = TryCast(dgvStores.Rows(e.RowIndex).DataBoundItem, StoreInfo)
        If store Is Nothing Then Return
        Dim value As Double
        If prop = NameOf(StoreInfo.CpuLoad) AndAlso Double.TryParse(store.CpuLoad, value) Then
            e.CellStyle.BackColor = If(value >= 80, Color.LightCoral, If(value >= 60, Color.Khaki, Color.LightGreen))
        ElseIf prop = NameOf(StoreInfo.FreeDiskGb) AndAlso Double.TryParse(store.FreeDiskGb, value) Then
            e.CellStyle.BackColor = If(value < 5, Color.LightCoral, If(value < 10, Color.Khaki, Color.LightGreen))
        ElseIf prop = NameOf(StoreInfo.HealthStatus) Then
            e.CellStyle.BackColor = If(store.HealthStatus = "Critical", Color.LightCoral, If(store.HealthStatus = "Warning", Color.Khaki, If(store.HealthStatus = "Healthy", Color.LightGreen, e.CellStyle.BackColor)))
        End If
    End Sub

    Private Sub SetCheckingState(isChecking As Boolean)
        btnCheckHealth.Enabled = Not isChecking
        btnExportHealth.Enabled = Not isChecking
        btnCheckPing.Enabled = Not isChecking
        btnCheckSelected.Enabled = Not isChecking
        btnValidateSelected.Enabled = Not isChecking
        btnReload.Enabled = Not isChecking
        btnBrowseConfig.Enabled = Not isChecking
        btnSelectAll.Enabled = Not isChecking
        btnClearAll.Enabled = Not isChecking
        nudParallel.Enabled = Not isChecking
        txtSearch.Enabled = Not isChecking
        cboStatusFilter.Enabled = Not isChecking
        btnBackupSelected.Enabled = Not isChecking
        btnDeploySelected.Enabled = Not isChecking
        btnRealVnc.Enabled = Not isChecking
        btnRemoteTools.Enabled = Not isChecking
        mnuDeleteStore.Enabled = Not isChecking
        btnBrowsePackage.Enabled = Not isChecking
        txtPackagePath.Enabled = Not isChecking
        txtProcessName.Enabled = Not isChecking
        chkStartAfterDeploy.Enabled = Not isChecking
        btnCancel.Enabled = isChecking
        dgvStores.Enabled = Not isChecking

        If Not isChecking Then
            lblProgress.Text = "พร้อมใช้งาน"
        End If
    End Sub

    Private Sub UpdateSummary()
        If _stores Is Nothing Then
            lblSummary.Text = "จำนวน 0 เครื่อง"
            Return
        End If

        Dim selected As Integer =
            Enumerable.Count(
                _stores,
                Function(item) item.Selected
            )

        Dim ready As Integer =
            Enumerable.Count(
                _stores,
                Function(item)
                    Return item.Status = "Ready" OrElse
                           item.Status = "Online" OrElse
                           item.Status = "Backup OK" OrElse
                           item.Status = "Deploy OK" OrElse
                           item.Status = "Ready to deploy"
                End Function
            )

        Dim failed As Integer =
            Enumerable.Count(
                _stores,
                Function(item)
                    Return item.Status = "Offline" OrElse
                           item.Status = "WinRM failed" OrElse
                           item.Status = "Backup failed" OrElse
                           item.Status = "Deploy failed" OrElse
                           item.Status = "Validation failed"
                End Function
            )

        lblSummary.Text =
            String.Format(
                "ทั้งหมด {0} | แสดง {1} | เลือก {2} | พร้อม {3} | ไม่พร้อม {4}",
                _allStores.Count,
                _stores.Count,
                selected,
                ready,
                failed
            )
    End Sub

    Private Sub dgvStores_CellMouseDown(
        sender As Object,
        e As DataGridViewCellMouseEventArgs
    ) Handles dgvStores.CellMouseDown

        If e.Button <> MouseButtons.Right OrElse e.RowIndex < 0 Then
            Return
        End If

        dgvStores.ClearSelection()
        dgvStores.Rows(e.RowIndex).Selected = True
        dgvStores.CurrentCell = dgvStores.Rows(e.RowIndex).Cells(
            Math.Max(e.ColumnIndex, 0)
        )
    End Sub

    Private Sub mnuDeleteStore_Click(
        sender As Object,
        e As EventArgs
    ) Handles mnuDeleteStore.Click

        If _checkCancellation IsNot Nothing Then
            MessageBox.Show(
                "ไม่สามารถลบรายการระหว่างที่โปรแกรมกำลังทำงาน",
                "Delete store",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            )
            Return
        End If

        If _stores Is Nothing OrElse dgvStores.CurrentRow Is Nothing Then
            Return
        End If

        Dim store As StoreInfo =
            TryCast(dgvStores.CurrentRow.DataBoundItem, StoreInfo)

        If store Is Nothing Then
            MessageBox.Show(
                "ไม่พบข้อมูล Store จากแถวที่เลือก",
                "Delete store",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            )
            Return
        End If

        Dim message As String =
            String.Format(
                "ต้องการลบรายการนี้หรือไม่?{0}{0}" &
                "Store: {1}{0}" &
                "POS Name: {2}{0}" &
                "IP Address: {3}{0}{0}" &
                "รายการจะถูกลบออกจาก stores.json ทันที",
                Environment.NewLine,
                store.StoreCode,
                store.StoreName,
                store.IpAddress
            )

        If MessageBox.Show(
            message,
            "Confirm delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2
        ) <> DialogResult.Yes Then
            Return
        End If

        Try
            _allStores.Remove(store)
            ApplyFilter()
            SaveStoresToJson()

            dgvStores.ClearSelection()
            UpdateSummary()

            AddLog(
                String.Format(
                    "[{0}/{1}] Deleted from stores.json: {2}",
                    store.StoreCode,
                    store.StoreName,
                    store.IpAddress
                )
            )

        Catch ex As Exception
            AddLog("Delete store ERROR: " & ex.Message)

            MessageBox.Show(
                "ลบรายการไม่สำเร็จ: " & ex.Message,
                "Delete store error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )

            ' โหลดไฟล์ใหม่เพื่อคืนค่าหน้าจอให้ตรงกับ stores.json
            LoadStoreData()
        End Try
    End Sub

    Private Sub SaveStoresToJson()
        Dim configPath As String = txtConfigPath.Text.Trim()

        If String.IsNullOrWhiteSpace(configPath) Then
            Throw New InvalidOperationException(
                "ไม่ได้ระบุ Path ของ stores.json"
            )
        End If

        Dim directoryPath As String =
            Path.GetDirectoryName(configPath)

        If Not String.IsNullOrWhiteSpace(directoryPath) AndAlso
           Not Directory.Exists(directoryPath) Then

            Directory.CreateDirectory(directoryPath)
        End If

        Dim json As String =
            JsonConvert.SerializeObject(
                _allStores,
                Formatting.Indented
            )

        Dim tempPath As String = configPath & ".tmp"

        File.WriteAllText(
            tempPath,
            json,
            New System.Text.UTF8Encoding(False)
        )

        If File.Exists(configPath) Then
            File.Delete(configPath)
        End If

        File.Move(tempPath, configPath)
    End Sub

    Private Sub btnRealVnc_Click(sender As Object, e As EventArgs) Handles btnRealVnc.Click
        Try
            If dgvStores.CurrentRow Is Nothing Then
                MessageBox.Show(
                    "กรุณาเลือกแถวของเครื่องที่ต้องการ Remote",
                    "RealVNC Viewer",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                )
                Return
            End If

            Dim store As StoreInfo =
                TryCast(dgvStores.CurrentRow.DataBoundItem, StoreInfo)

            If store Is Nothing OrElse
               String.IsNullOrWhiteSpace(store.IpAddress) Then

                MessageBox.Show(
                    "ไม่พบ IP Address จากแถวที่เลือก",
                    "RealVNC Viewer",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )
                Return
            End If

            Dim viewerPath As String = FindRealVncViewer()

            If String.IsNullOrWhiteSpace(viewerPath) Then
                MessageBox.Show(
                    "ไม่พบ RealVNC Viewer กรุณาตรวจสอบว่าติดตั้งโปรแกรมแล้ว",
                    "RealVNC Viewer",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                )
                Return
            End If

            Dim startInfo As New ProcessStartInfo With {
                .FileName = viewerPath,
                .Arguments = store.IpAddress.Trim(),
                .UseShellExecute = True
            }

            Process.Start(startInfo)

            AddLog(
                String.Format(
                    "[{0}/{1}] Open RealVNC: {2}",
                    store.StoreCode,
                    store.StoreName,
                    store.IpAddress
                )
            )

        Catch ex As Exception
            AddLog("RealVNC ERROR: " & ex.Message)

            MessageBox.Show(
                ex.Message,
                "RealVNC Viewer error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )
        End Try
    End Sub

    Private Shared Function FindRealVncViewer() As String
        Dim candidatePaths As String() = {
            IO.Path.Combine(
                Environment.GetEnvironmentVariable("ProgramW6432"),
                "RealVNC",
                "VNC Viewer",
                "vncviewer.exe"
            ),
            IO.Path.Combine(
                Environment.GetEnvironmentVariable("ProgramFiles(x86)"),
                "RealVNC",
                "VNC Viewer",
                "vncviewer.exe"
            ),
            IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RealVNC",
                "VNC Viewer",
                "vncviewer.exe"
            )
        }

        For Each candidatePath As String In candidatePaths
            If File.Exists(candidatePath) Then
                Return candidatePath
            End If
        Next

        Return Nothing
    End Function

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If _checkCancellation IsNot Nothing Then
            btnCancel.Enabled = False
            AddLog("กำลังยกเลิก...")
            _checkCancellation.Cancel()
        End If
    End Sub

    Private Sub btnReload_Click(sender As Object, e As EventArgs) Handles btnReload.Click
        LoadStoreData()
    End Sub

    Private Sub btnBrowseConfig_Click(sender As Object, e As EventArgs) Handles btnBrowseConfig.Click
        Using dialog As New OpenFileDialog()
            dialog.Filter =
                "JSON files (*.json)|*.json|All files (*.*)|*.*"

            dialog.FileName = "stores.json"

            If dialog.ShowDialog(Me) = DialogResult.OK Then
                txtConfigPath.Text = dialog.FileName
                LoadStoreData()
            End If
        End Using
    End Sub

    Private Sub btnSelectAll_Click(sender As Object, e As EventArgs) Handles btnSelectAll.Click
        If _stores Is Nothing Then Return

        For Each store In _stores
            store.Selected = True
        Next

        dgvStores.Refresh()
        UpdateSummary()
    End Sub

    Private Sub btnClearAll_Click(sender As Object, e As EventArgs) Handles btnClearAll.Click
        If _stores Is Nothing Then Return

        For Each store In _stores
            store.Selected = False
        Next

        dgvStores.Refresh()
        UpdateSummary()
    End Sub

    Private Sub dgvStores_CellValueChanged(
        sender As Object,
        e As DataGridViewCellEventArgs
    ) Handles dgvStores.CellValueChanged

        If e.ColumnIndex = 0 Then
            UpdateSummary()
        End If
    End Sub

    Private Sub dgvStores_CurrentCellDirtyStateChanged(
        sender As Object,
        e As EventArgs
    ) Handles dgvStores.CurrentCellDirtyStateChanged

        If dgvStores.IsCurrentCellDirty Then
            dgvStores.CommitEdit(
                DataGridViewDataErrorContexts.Commit
            )
        End If
    End Sub

    Private Sub dgvStores_CellFormatting(
        sender As Object,
        e As DataGridViewCellFormattingEventArgs
    ) Handles dgvStores.CellFormatting

        If e.ColumnIndex < 0 Then Return

        Dim propertyName = dgvStores.Columns(e.ColumnIndex).DataPropertyName
        If propertyName <> NameOf(StoreInfo.Status) AndAlso propertyName <> NameOf(StoreInfo.ValidationStatus) Then Return

        Dim value = Convert.ToString(e.Value)

        Select Case value
            Case "Ready", "Online", "Backup OK", "Deploy OK", "Ready to deploy"
                e.CellStyle.BackColor = Color.LightGreen

            Case "Checking...", "Backing up...", "Deploying..."
                e.CellStyle.BackColor = Color.LightYellow

            Case "Offline", "WinRM failed", "Backup failed", "Deploy failed", "Validation failed"
                e.CellStyle.BackColor = Color.MistyRose

            Case "Deploy warning"
                e.CellStyle.BackColor = Color.LightSalmon

            Case "Deploy skipped"
                e.CellStyle.BackColor = Color.LightGray
        End Select
    End Sub

    Private Sub AddLog(message As String)
        Dim normalizedMessage = If(message, String.Empty)
        txtLog.AppendText(
            String.Format(
                "{0:yyyy-MM-dd HH:mm:ss}  {1}{2}",
                Date.Now,
                normalizedMessage,
                Environment.NewLine
            )
        )
        _services.Logger.Info(normalizedMessage)
    End Sub

    Private Sub AddLogThreadSafe(message As String)
        If InvokeRequired Then
            BeginInvoke(
                New Action(Of String)(
                    AddressOf AddLogThreadSafe
                ),
                message
            )
            Return
        End If

        AddLog(message)
    End Sub

    Private Sub frmMain_FormClosing(
        sender As Object,
        e As FormClosingEventArgs
    ) Handles MyBase.FormClosing

        If _checkCancellation IsNot Nothing Then
            _checkCancellation.Cancel()
        End If
        _services.Audit.Write("FORM_CLOSE", NameOf(frmMain), "SUCCESS")
    End Sub

End Class