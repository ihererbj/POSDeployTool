<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.rootLayout = New System.Windows.Forms.TableLayoutPanel()
        Me.navigationPanel = New System.Windows.Forms.Panel()
        Me.pnlNavMenu = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnNavDashboard = New System.Windows.Forms.Button()
        Me.btnNavConnection = New System.Windows.Forms.Button()
        Me.btnNavDeploy = New System.Windows.Forms.Button()
        Me.btnNavLogs = New System.Windows.Forms.Button()
        Me.btnNavSettings = New System.Windows.Forms.Button()
        Me.lblBrand = New System.Windows.Forms.Label()
        Me.lblVersion = New System.Windows.Forms.Label()
        Me.contentPanel = New System.Windows.Forms.Panel()
        Me.mnuStore = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuDeleteStore = New System.Windows.Forms.ToolStripMenuItem()
        Me.mainLayout = New System.Windows.Forms.TableLayoutPanel()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.configLayout = New System.Windows.Forms.TableLayoutPanel()
        Me.lblConfig = New System.Windows.Forms.Label()
        Me.txtConfigPath = New System.Windows.Forms.TextBox()
        Me.btnBrowseConfig = New System.Windows.Forms.Button()
        Me.btnReload = New System.Windows.Forms.Button()
        Me.btnSelectAll = New System.Windows.Forms.Button()
        Me.btnClearAll = New System.Windows.Forms.Button()
        Me.actionLayout = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnRealVnc = New System.Windows.Forms.Button()
        Me.btnCheckPing = New System.Windows.Forms.Button()
        Me.btnCheckSelected = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.lblParallel = New System.Windows.Forms.Label()
        Me.nudParallel = New System.Windows.Forms.NumericUpDown()
        Me.btnBackupSelected = New System.Windows.Forms.Button()
        Me.btnValidateSelected = New System.Windows.Forms.Button()
        Me.btnDeploySelected = New System.Windows.Forms.Button()
        Me.btnRemoteTools = New System.Windows.Forms.Button()
        Me.btnCheckHealth = New System.Windows.Forms.Button()
        Me.btnExportHealth = New System.Windows.Forms.Button()
        Me.remoteToolsMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuRemoteCheck = New System.Windows.Forms.ToolStripMenuItem()
        Me.sepRemote1 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuRemoteTaskInstall = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuRemoteTaskRemove = New System.Windows.Forms.ToolStripMenuItem()
        Me.sepRemote2 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuRemoteStart = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuRemoteStop = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuRemoteRestart = New System.Windows.Forms.ToolStripMenuItem()
        Me.sepRemote3 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuRemoteReboot = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuRemoteAbort = New System.Windows.Forms.ToolStripMenuItem()
        Me.sepRemote4 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuRemoteServiceStart = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuRemoteServiceStop = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuRemoteServiceRestart = New System.Windows.Forms.ToolStripMenuItem()
        Me.sepRemote5 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuRemoteHealth = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuRemoteNetwork = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuRemoteProcesses = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuRemoteServices = New System.Windows.Forms.ToolStripMenuItem()
        Me.sepRemote6 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuRemoteKillProcess = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuRemoteCustom = New System.Windows.Forms.ToolStripMenuItem()
        Me.filterLayout = New System.Windows.Forms.TableLayoutPanel()
        Me.lblSearch = New System.Windows.Forms.Label()
        Me.txtSearch = New System.Windows.Forms.TextBox()
        Me.lblStatusFilter = New System.Windows.Forms.Label()
        Me.cboStatusFilter = New System.Windows.Forms.ComboBox()
        Me.deployLayout = New System.Windows.Forms.TableLayoutPanel()
        Me.lblPackage = New System.Windows.Forms.Label()
        Me.txtPackagePath = New System.Windows.Forms.TextBox()
        Me.btnBrowsePackage = New System.Windows.Forms.Button()
        Me.lblProcess = New System.Windows.Forms.Label()
        Me.txtProcessName = New System.Windows.Forms.TextBox()
        Me.chkStartAfterDeploy = New System.Windows.Forms.CheckBox()
        Me.chkForceSameVersion = New System.Windows.Forms.CheckBox()
        Me.dgvStores = New System.Windows.Forms.DataGridView()
        Me.statusLayout = New System.Windows.Forms.TableLayoutPanel()
        Me.lblSummary = New System.Windows.Forms.Label()
        Me.lblProgress = New System.Windows.Forms.Label()
        Me.progressCheck = New System.Windows.Forms.ProgressBar()
        Me.grpLog = New System.Windows.Forms.GroupBox()
        Me.txtLog = New System.Windows.Forms.TextBox()
        Me.rootLayout.SuspendLayout()
        Me.navigationPanel.SuspendLayout()
        Me.pnlNavMenu.SuspendLayout()
        Me.contentPanel.SuspendLayout()
        Me.mnuStore.SuspendLayout()
        Me.remoteToolsMenu.SuspendLayout()
        Me.mainLayout.SuspendLayout()
        Me.configLayout.SuspendLayout()
        Me.actionLayout.SuspendLayout()
        CType(Me.nudParallel, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.filterLayout.SuspendLayout()
        Me.deployLayout.SuspendLayout()
        CType(Me.dgvStores, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.statusLayout.SuspendLayout()
        Me.grpLog.SuspendLayout()
        Me.SuspendLayout()
        '
        'rootLayout
        '
        Me.rootLayout.ColumnCount = 2
        Me.rootLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 210.0!))
        Me.rootLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.rootLayout.Controls.Add(Me.navigationPanel, 0, 0)
        Me.rootLayout.Controls.Add(Me.contentPanel, 1, 0)
        Me.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill
        Me.rootLayout.Location = New System.Drawing.Point(0, 0)
        Me.rootLayout.Margin = New System.Windows.Forms.Padding(0)
        Me.rootLayout.Name = "rootLayout"
        Me.rootLayout.RowCount = 1
        Me.rootLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.rootLayout.Size = New System.Drawing.Size(1184, 760)
        Me.rootLayout.TabIndex = 0
        '
        'navigationPanel
        '
        Me.navigationPanel.BackColor = System.Drawing.Color.FromArgb(CType(CType(28, Byte), Integer), CType(CType(39, Byte), Integer), CType(CType(54, Byte), Integer))
        Me.navigationPanel.Controls.Add(Me.pnlNavMenu)
        Me.navigationPanel.Controls.Add(Me.lblVersion)
        Me.navigationPanel.Controls.Add(Me.lblBrand)
        Me.navigationPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.navigationPanel.Location = New System.Drawing.Point(0, 0)
        Me.navigationPanel.Margin = New System.Windows.Forms.Padding(0)
        Me.navigationPanel.Name = "navigationPanel"
        Me.navigationPanel.Padding = New System.Windows.Forms.Padding(12, 14, 12, 12)
        Me.navigationPanel.Size = New System.Drawing.Size(210, 760)
        Me.navigationPanel.TabIndex = 0
        '
        'pnlNavMenu
        '
        Me.pnlNavMenu.Controls.Add(Me.btnNavDashboard)
        Me.pnlNavMenu.Controls.Add(Me.btnNavConnection)
        Me.pnlNavMenu.Controls.Add(Me.btnNavDeploy)
        Me.pnlNavMenu.Controls.Add(Me.btnNavLogs)
        Me.pnlNavMenu.Controls.Add(Me.btnNavSettings)
        Me.pnlNavMenu.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlNavMenu.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.pnlNavMenu.Location = New System.Drawing.Point(12, 86)
        Me.pnlNavMenu.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlNavMenu.Name = "pnlNavMenu"
        Me.pnlNavMenu.Padding = New System.Windows.Forms.Padding(0, 8, 0, 0)
        Me.pnlNavMenu.Size = New System.Drawing.Size(186, 622)
        Me.pnlNavMenu.TabIndex = 1
        Me.pnlNavMenu.WrapContents = False
        '
        'btnNavDashboard
        '
        Me.btnNavDashboard.BackColor = System.Drawing.Color.FromArgb(CType(CType(42, Byte), Integer), CType(CType(105, Byte), Integer), CType(CType(168, Byte), Integer))
        Me.btnNavDashboard.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnNavDashboard.FlatAppearance.BorderSize = 0
        Me.btnNavDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnNavDashboard.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.btnNavDashboard.ForeColor = System.Drawing.Color.White
        Me.btnNavDashboard.Location = New System.Drawing.Point(0, 8)
        Me.btnNavDashboard.Margin = New System.Windows.Forms.Padding(0, 0, 0, 4)
        Me.btnNavDashboard.Name = "btnNavDashboard"
        Me.btnNavDashboard.Size = New System.Drawing.Size(186, 48)
        Me.btnNavDashboard.TabIndex = 0
        Me.btnNavDashboard.Text = "   Dashboard"
        Me.btnNavDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnNavDashboard.UseVisualStyleBackColor = False
        '
        'btnNavConnection
        '
        Me.btnNavConnection.BackColor = System.Drawing.Color.FromArgb(CType(CType(28, Byte), Integer), CType(CType(39, Byte), Integer), CType(CType(54, Byte), Integer))
        Me.btnNavConnection.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnNavConnection.FlatAppearance.BorderSize = 0
        Me.btnNavConnection.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnNavConnection.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnNavConnection.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(234, Byte), Integer))
        Me.btnNavConnection.Location = New System.Drawing.Point(0, 60)
        Me.btnNavConnection.Margin = New System.Windows.Forms.Padding(0, 0, 0, 4)
        Me.btnNavConnection.Name = "btnNavConnection"
        Me.btnNavConnection.Size = New System.Drawing.Size(186, 48)
        Me.btnNavConnection.TabIndex = 1
        Me.btnNavConnection.Text = "   Connection Check"
        Me.btnNavConnection.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnNavConnection.UseVisualStyleBackColor = False
        '
        'btnNavDeploy
        '
        Me.btnNavDeploy.BackColor = System.Drawing.Color.FromArgb(CType(CType(28, Byte), Integer), CType(CType(39, Byte), Integer), CType(CType(54, Byte), Integer))
        Me.btnNavDeploy.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnNavDeploy.FlatAppearance.BorderSize = 0
        Me.btnNavDeploy.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnNavDeploy.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnNavDeploy.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(234, Byte), Integer))
        Me.btnNavDeploy.Location = New System.Drawing.Point(0, 112)
        Me.btnNavDeploy.Margin = New System.Windows.Forms.Padding(0, 0, 0, 4)
        Me.btnNavDeploy.Name = "btnNavDeploy"
        Me.btnNavDeploy.Size = New System.Drawing.Size(186, 48)
        Me.btnNavDeploy.TabIndex = 2
        Me.btnNavDeploy.Text = "   Deploy Program"
        Me.btnNavDeploy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnNavDeploy.UseVisualStyleBackColor = False
        '
        'btnNavLogs
        '
        Me.btnNavLogs.BackColor = System.Drawing.Color.FromArgb(CType(CType(28, Byte), Integer), CType(CType(39, Byte), Integer), CType(CType(54, Byte), Integer))
        Me.btnNavLogs.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnNavLogs.FlatAppearance.BorderSize = 0
        Me.btnNavLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnNavLogs.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnNavLogs.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(234, Byte), Integer))
        Me.btnNavLogs.Location = New System.Drawing.Point(0, 164)
        Me.btnNavLogs.Margin = New System.Windows.Forms.Padding(0, 0, 0, 4)
        Me.btnNavLogs.Name = "btnNavLogs"
        Me.btnNavLogs.Size = New System.Drawing.Size(186, 48)
        Me.btnNavLogs.TabIndex = 3
        Me.btnNavLogs.Text = "   Activity Logs"
        Me.btnNavLogs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnNavLogs.UseVisualStyleBackColor = False
        '
        'btnNavSettings
        '
        Me.btnNavSettings.BackColor = System.Drawing.Color.FromArgb(CType(CType(28, Byte), Integer), CType(CType(39, Byte), Integer), CType(CType(54, Byte), Integer))
        Me.btnNavSettings.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnNavSettings.FlatAppearance.BorderSize = 0
        Me.btnNavSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnNavSettings.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnNavSettings.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(234, Byte), Integer))
        Me.btnNavSettings.Location = New System.Drawing.Point(0, 216)
        Me.btnNavSettings.Margin = New System.Windows.Forms.Padding(0, 0, 0, 4)
        Me.btnNavSettings.Name = "btnNavSettings"
        Me.btnNavSettings.Size = New System.Drawing.Size(186, 48)
        Me.btnNavSettings.TabIndex = 4
        Me.btnNavSettings.Text = "   Settings"
        Me.btnNavSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnNavSettings.UseVisualStyleBackColor = False
        '
        'lblBrand
        '
        Me.lblBrand.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblBrand.Font = New System.Drawing.Font("Segoe UI", 13.0!, System.Drawing.FontStyle.Bold)
        Me.lblBrand.ForeColor = System.Drawing.Color.White
        Me.lblBrand.Location = New System.Drawing.Point(12, 14)
        Me.lblBrand.Name = "lblBrand"
        Me.lblBrand.Padding = New System.Windows.Forms.Padding(8, 0, 0, 0)
        Me.lblBrand.Size = New System.Drawing.Size(186, 72)
        Me.lblBrand.TabIndex = 0
        Me.lblBrand.Text = "POS DEPLOY" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "MANAGEMENT"
        Me.lblBrand.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblVersion
        '
        Me.lblVersion.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.lblVersion.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.lblVersion.ForeColor = System.Drawing.Color.FromArgb(CType(CType(165, Byte), Integer), CType(CType(178, Byte), Integer), CType(CType(194, Byte), Integer))
        Me.lblVersion.Location = New System.Drawing.Point(12, 708)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(186, 40)
        Me.lblVersion.TabIndex = 2
        Me.lblVersion.Text = "Sprint 5.2"
        Me.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'contentPanel
        '
        Me.contentPanel.BackColor = System.Drawing.Color.White
        Me.contentPanel.Controls.Add(Me.mainLayout)
        Me.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.contentPanel.Location = New System.Drawing.Point(210, 0)
        Me.contentPanel.Margin = New System.Windows.Forms.Padding(0)
        Me.contentPanel.Name = "contentPanel"
        Me.contentPanel.Padding = New System.Windows.Forms.Padding(14)
        Me.contentPanel.Size = New System.Drawing.Size(974, 760)
        Me.contentPanel.TabIndex = 1
        '
        'mnuStore
        '
        Me.mnuStore.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuDeleteStore})
        Me.mnuStore.Name = "mnuStore"
        Me.mnuStore.Size = New System.Drawing.Size(150, 26)
        '
        'mnuDeleteStore
        '
        Me.mnuDeleteStore.Name = "mnuDeleteStore"
        Me.mnuDeleteStore.Size = New System.Drawing.Size(149, 22)
        Me.mnuDeleteStore.Text = "ลบรายการที่เลือก"
        '
        'mainLayout
        '
        Me.mainLayout.ColumnCount = 1
        Me.mainLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.mainLayout.Controls.Add(Me.lblTitle, 0, 0)
        Me.mainLayout.Controls.Add(Me.configLayout, 0, 1)
        Me.mainLayout.Controls.Add(Me.actionLayout, 0, 2)
        Me.mainLayout.Controls.Add(Me.filterLayout, 0, 3)
        Me.mainLayout.Controls.Add(Me.deployLayout, 0, 4)
        Me.mainLayout.Controls.Add(Me.dgvStores, 0, 5)
        Me.mainLayout.Controls.Add(Me.statusLayout, 0, 6)
        Me.mainLayout.Controls.Add(Me.progressCheck, 0, 7)
        Me.mainLayout.Controls.Add(Me.grpLog, 0, 8)
        Me.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill
        Me.mainLayout.Location = New System.Drawing.Point(12, 12)
        Me.mainLayout.Margin = New System.Windows.Forms.Padding(0)
        Me.mainLayout.Name = "mainLayout"
        Me.mainLayout.RowCount = 9
        Me.mainLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48.0!))
        Me.mainLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42.0!))
        Me.mainLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44.0!))
        Me.mainLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42.0!))
        Me.mainLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.mainLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 68.0!))
        Me.mainLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.mainLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24.0!))
        Me.mainLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 32.0!))
        Me.mainLayout.Size = New System.Drawing.Size(946, 732)
        Me.mainLayout.TabIndex = 0
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.Location = New System.Drawing.Point(0, 0)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(1160, 48)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "POS Deploy Tool"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'configLayout
        '
        Me.configLayout.ColumnCount = 6
        Me.configLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90.0!))
        Me.configLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.configLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84.0!))
        Me.configLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84.0!))
        Me.configLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108.0!))
        Me.configLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108.0!))
        Me.configLayout.Controls.Add(Me.lblConfig, 0, 0)
        Me.configLayout.Controls.Add(Me.txtConfigPath, 1, 0)
        Me.configLayout.Controls.Add(Me.btnBrowseConfig, 2, 0)
        Me.configLayout.Controls.Add(Me.btnReload, 3, 0)
        Me.configLayout.Controls.Add(Me.btnSelectAll, 4, 0)
        Me.configLayout.Controls.Add(Me.btnClearAll, 5, 0)
        Me.configLayout.Dock = System.Windows.Forms.DockStyle.Fill
        Me.configLayout.Location = New System.Drawing.Point(0, 48)
        Me.configLayout.Margin = New System.Windows.Forms.Padding(0)
        Me.configLayout.Name = "configLayout"
        Me.configLayout.RowCount = 1
        Me.configLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.configLayout.Size = New System.Drawing.Size(1160, 42)
        Me.configLayout.TabIndex = 1
        '
        'lblConfig
        '
        Me.lblConfig.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblConfig.Location = New System.Drawing.Point(0, 0)
        Me.lblConfig.Margin = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.lblConfig.Name = "lblConfig"
        Me.lblConfig.Size = New System.Drawing.Size(82, 42)
        Me.lblConfig.TabIndex = 0
        Me.lblConfig.Text = "stores.json:"
        Me.lblConfig.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtConfigPath
        '
        Me.txtConfigPath.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtConfigPath.Location = New System.Drawing.Point(90, 8)
        Me.txtConfigPath.Margin = New System.Windows.Forms.Padding(0, 8, 8, 8)
        Me.txtConfigPath.Name = "txtConfigPath"
        Me.txtConfigPath.Size = New System.Drawing.Size(678, 23)
        Me.txtConfigPath.TabIndex = 1
        '
        'btnBrowseConfig
        '
        Me.btnBrowseConfig.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnBrowseConfig.Location = New System.Drawing.Point(776, 6)
        Me.btnBrowseConfig.Margin = New System.Windows.Forms.Padding(0, 6, 8, 6)
        Me.btnBrowseConfig.Name = "btnBrowseConfig"
        Me.btnBrowseConfig.Size = New System.Drawing.Size(76, 30)
        Me.btnBrowseConfig.TabIndex = 2
        Me.btnBrowseConfig.Text = "Browse"
        Me.btnBrowseConfig.UseVisualStyleBackColor = True
        '
        'btnReload
        '
        Me.btnReload.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnReload.Location = New System.Drawing.Point(860, 6)
        Me.btnReload.Margin = New System.Windows.Forms.Padding(0, 6, 8, 6)
        Me.btnReload.Name = "btnReload"
        Me.btnReload.Size = New System.Drawing.Size(76, 30)
        Me.btnReload.TabIndex = 3
        Me.btnReload.Text = "Reload"
        Me.btnReload.UseVisualStyleBackColor = True
        '
        'btnSelectAll
        '
        Me.btnSelectAll.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnSelectAll.Location = New System.Drawing.Point(944, 6)
        Me.btnSelectAll.Margin = New System.Windows.Forms.Padding(0, 6, 8, 6)
        Me.btnSelectAll.Name = "btnSelectAll"
        Me.btnSelectAll.Size = New System.Drawing.Size(100, 30)
        Me.btnSelectAll.TabIndex = 4
        Me.btnSelectAll.Text = "เลือกทั้งหมด"
        Me.btnSelectAll.UseVisualStyleBackColor = True
        '
        'btnClearAll
        '
        Me.btnClearAll.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnClearAll.Location = New System.Drawing.Point(1052, 6)
        Me.btnClearAll.Margin = New System.Windows.Forms.Padding(0, 6, 0, 6)
        Me.btnClearAll.Name = "btnClearAll"
        Me.btnClearAll.Size = New System.Drawing.Size(108, 30)
        Me.btnClearAll.TabIndex = 5
        Me.btnClearAll.Text = "ยกเลิกทั้งหมด"
        Me.btnClearAll.UseVisualStyleBackColor = True
        '
        'actionLayout
        '
        Me.actionLayout.Controls.Add(Me.btnRealVnc)
        Me.actionLayout.Controls.Add(Me.btnCheckPing)
        Me.actionLayout.Controls.Add(Me.btnCheckSelected)
        Me.actionLayout.Controls.Add(Me.btnCancel)
        Me.actionLayout.Controls.Add(Me.lblParallel)
        Me.actionLayout.Controls.Add(Me.nudParallel)
        Me.actionLayout.Controls.Add(Me.btnBackupSelected)
        Me.actionLayout.Controls.Add(Me.btnValidateSelected)
        Me.actionLayout.Controls.Add(Me.btnDeploySelected)
        Me.actionLayout.Controls.Add(Me.btnRemoteTools)
        Me.actionLayout.Controls.Add(Me.btnCheckHealth)
        Me.actionLayout.Controls.Add(Me.btnExportHealth)
        Me.actionLayout.Dock = System.Windows.Forms.DockStyle.Fill
        Me.actionLayout.Location = New System.Drawing.Point(0, 90)
        Me.actionLayout.Margin = New System.Windows.Forms.Padding(0)
        Me.actionLayout.Name = "actionLayout"
        Me.actionLayout.Padding = New System.Windows.Forms.Padding(0, 6, 0, 4)
        Me.actionLayout.Size = New System.Drawing.Size(1160, 44)
        Me.actionLayout.TabIndex = 2
        Me.actionLayout.WrapContents = False
        '
        'btnRealVnc
        '
        Me.btnRealVnc.Location = New System.Drawing.Point(8, 6)
        Me.btnRealVnc.Margin = New System.Windows.Forms.Padding(8, 0, 0, 0)
        Me.btnRealVnc.Name = "btnRealVnc"
        Me.btnRealVnc.Size = New System.Drawing.Size(110, 30)
        Me.btnRealVnc.TabIndex = 7
        Me.btnRealVnc.Text = "RealVNC"
        Me.btnRealVnc.UseVisualStyleBackColor = True
        '
        'btnCheckPing
        '
        Me.btnCheckPing.Location = New System.Drawing.Point(118, 6)
        Me.btnCheckPing.Margin = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.btnCheckPing.Name = "btnCheckPing"
        Me.btnCheckPing.Size = New System.Drawing.Size(105, 30)
        Me.btnCheckPing.TabIndex = 0
        Me.btnCheckPing.Text = "Check Ping"
        Me.btnCheckPing.UseVisualStyleBackColor = True
        '
        'btnCheckSelected
        '
        Me.btnCheckSelected.Location = New System.Drawing.Point(231, 6)
        Me.btnCheckSelected.Margin = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.btnCheckSelected.Name = "btnCheckSelected"
        Me.btnCheckSelected.Size = New System.Drawing.Size(175, 30)
        Me.btnCheckSelected.TabIndex = 1
        Me.btnCheckSelected.Text = "Check Ping + WinRM + OS"
        Me.btnCheckSelected.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(414, 6)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(0, 0, 18, 0)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(84, 30)
        Me.btnCancel.TabIndex = 2
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'lblParallel
        '
        Me.lblParallel.AutoSize = True
        Me.lblParallel.Location = New System.Drawing.Point(516, 6)
        Me.lblParallel.Margin = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.lblParallel.Name = "lblParallel"
        Me.lblParallel.Padding = New System.Windows.Forms.Padding(0, 7, 0, 0)
        Me.lblParallel.Size = New System.Drawing.Size(74, 22)
        Me.lblParallel.TabIndex = 3
        Me.lblParallel.Text = "Max Parallel:"
        '
        'nudParallel
        '
        Me.nudParallel.Location = New System.Drawing.Point(598, 9)
        Me.nudParallel.Margin = New System.Windows.Forms.Padding(0, 3, 18, 0)
        Me.nudParallel.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudParallel.Name = "nudParallel"
        Me.nudParallel.Size = New System.Drawing.Size(58, 23)
        Me.nudParallel.TabIndex = 4
        Me.nudParallel.Value = New Decimal(New Integer() {10, 0, 0, 0})
        '
        'btnBackupSelected
        '
        Me.btnBackupSelected.Location = New System.Drawing.Point(674, 6)
        Me.btnBackupSelected.Margin = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.btnBackupSelected.Name = "btnBackupSelected"
        Me.btnBackupSelected.Size = New System.Drawing.Size(125, 30)
        Me.btnBackupSelected.TabIndex = 5
        Me.btnBackupSelected.Text = "Backup Selected"
        Me.btnBackupSelected.UseVisualStyleBackColor = True
        '
        'btnValidateSelected
        '
        Me.btnValidateSelected.Location = New System.Drawing.Point(807, 6)
        Me.btnValidateSelected.Margin = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.btnValidateSelected.Name = "btnValidateSelected"
        Me.btnValidateSelected.Size = New System.Drawing.Size(125, 30)
        Me.btnValidateSelected.TabIndex = 6
        Me.btnValidateSelected.Text = "Validate Selected"
        Me.btnValidateSelected.UseVisualStyleBackColor = True
        '
        'btnDeploySelected
        '
        Me.btnDeploySelected.Location = New System.Drawing.Point(940, 6)
        Me.btnDeploySelected.Margin = New System.Windows.Forms.Padding(0)
        Me.btnDeploySelected.Name = "btnDeploySelected"
        Me.btnDeploySelected.Size = New System.Drawing.Size(130, 30)
        Me.btnDeploySelected.TabIndex = 6
        Me.btnDeploySelected.Text = "Deploy Selected"
        Me.btnDeploySelected.UseVisualStyleBackColor = True
        '
        'btnRemoteTools
        '
        Me.btnRemoteTools.Location = New System.Drawing.Point(1073, 6)
        Me.btnRemoteTools.Margin = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.btnRemoteTools.Name = "btnRemoteTools"
        Me.btnRemoteTools.Size = New System.Drawing.Size(120, 30)
        Me.btnRemoteTools.TabIndex = 8
        Me.btnRemoteTools.Text = "Remote Tools ▼"
        Me.btnRemoteTools.UseVisualStyleBackColor = True
        '
        'btnCheckHealth
        '
        Me.btnCheckHealth.Location = New System.Drawing.Point(1201, 6)
        Me.btnCheckHealth.Margin = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.btnCheckHealth.Name = "btnCheckHealth"
        Me.btnCheckHealth.Size = New System.Drawing.Size(110, 30)
        Me.btnCheckHealth.TabIndex = 9
        Me.btnCheckHealth.Text = "Check Health"
        Me.btnCheckHealth.UseVisualStyleBackColor = True
        '
        'btnExportHealth
        '
        Me.btnExportHealth.Location = New System.Drawing.Point(1319, 6)
        Me.btnExportHealth.Margin = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.btnExportHealth.Name = "btnExportHealth"
        Me.btnExportHealth.Size = New System.Drawing.Size(110, 30)
        Me.btnExportHealth.TabIndex = 10
        Me.btnExportHealth.Text = "Export Health"
        Me.btnExportHealth.UseVisualStyleBackColor = True
        '
        'remoteToolsMenu
        '
        Me.remoteToolsMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuRemoteCheck, Me.sepRemote1, Me.mnuRemoteTaskInstall, Me.mnuRemoteTaskRemove, Me.sepRemote2, Me.mnuRemoteStart, Me.mnuRemoteStop, Me.mnuRemoteRestart, Me.sepRemote3, Me.mnuRemoteReboot, Me.mnuRemoteAbort, Me.sepRemote4, Me.mnuRemoteServiceStart, Me.mnuRemoteServiceStop, Me.mnuRemoteServiceRestart, Me.sepRemote5, Me.mnuRemoteHealth, Me.mnuRemoteNetwork, Me.mnuRemoteProcesses, Me.mnuRemoteServices, Me.sepRemote6, Me.mnuRemoteKillProcess, Me.mnuRemoteCustom})
        Me.remoteToolsMenu.Name = "remoteToolsMenu"
        Me.remoteToolsMenu.Size = New System.Drawing.Size(258, 452)
        '
        'mnuRemoteCheck
        '
        Me.mnuRemoteCheck.Name = "mnuRemoteCheck"
        Me.mnuRemoteCheck.Size = New System.Drawing.Size(257, 22)
        Me.mnuRemoteCheck.Tag = "check"
        Me.mnuRemoteCheck.Text = "Check BJCBCPOS Process"
        '
        'mnuRemoteTaskInstall
        '
        Me.mnuRemoteTaskInstall.Name = "mnuRemoteTaskInstall"
        Me.mnuRemoteTaskInstall.Size = New System.Drawing.Size(257, 22)
        Me.mnuRemoteTaskInstall.Tag = "task_install"
        Me.mnuRemoteTaskInstall.Text = "Install BJCBCPOS Task (No Trigger)"
        '
        'mnuRemoteTaskRemove
        '
        Me.mnuRemoteTaskRemove.Name = "mnuRemoteTaskRemove"
        Me.mnuRemoteTaskRemove.Size = New System.Drawing.Size(257, 22)
        Me.mnuRemoteTaskRemove.Tag = "task_remove"
        Me.mnuRemoteTaskRemove.Text = "Remove BJCBCPOS Task"
        '
        'mnuRemoteStart
        '
        Me.mnuRemoteStart.Name = "mnuRemoteStart"
        Me.mnuRemoteStart.Size = New System.Drawing.Size(257, 22)
        Me.mnuRemoteStart.Tag = "start"
        Me.mnuRemoteStart.Text = "Start BJCBCPOS"
        '
        'mnuRemoteStop
        '
        Me.mnuRemoteStop.Name = "mnuRemoteStop"
        Me.mnuRemoteStop.Size = New System.Drawing.Size(257, 22)
        Me.mnuRemoteStop.Tag = "stop"
        Me.mnuRemoteStop.Text = "Stop BJCBCPOS"
        '
        'mnuRemoteRestart
        '
        Me.mnuRemoteRestart.Name = "mnuRemoteRestart"
        Me.mnuRemoteRestart.Size = New System.Drawing.Size(257, 22)
        Me.mnuRemoteRestart.Tag = "restart"
        Me.mnuRemoteRestart.Text = "Restart BJCBCPOS"
        '
        'mnuRemoteReboot
        '
        Me.mnuRemoteReboot.Name = "mnuRemoteReboot"
        Me.mnuRemoteReboot.Size = New System.Drawing.Size(257, 22)
        Me.mnuRemoteReboot.Tag = "reboot"
        Me.mnuRemoteReboot.Text = "Restart Windows"
        '
        'mnuRemoteAbort
        '
        Me.mnuRemoteAbort.Name = "mnuRemoteAbort"
        Me.mnuRemoteAbort.Size = New System.Drawing.Size(257, 22)
        Me.mnuRemoteAbort.Tag = "abort"
        Me.mnuRemoteAbort.Text = "Cancel Windows Restart"
        '
        'mnuRemoteServiceStart
        '
        Me.mnuRemoteServiceStart.Name = "mnuRemoteServiceStart"
        Me.mnuRemoteServiceStart.Size = New System.Drawing.Size(257, 22)
        Me.mnuRemoteServiceStart.Tag = "service_start"
        Me.mnuRemoteServiceStart.Text = "Start Windows Service..."
        '
        'mnuRemoteServiceStop
        '
        Me.mnuRemoteServiceStop.Name = "mnuRemoteServiceStop"
        Me.mnuRemoteServiceStop.Size = New System.Drawing.Size(257, 22)
        Me.mnuRemoteServiceStop.Tag = "service_stop"
        Me.mnuRemoteServiceStop.Text = "Stop Windows Service..."
        '
        'mnuRemoteServiceRestart
        '
        Me.mnuRemoteServiceRestart.Name = "mnuRemoteServiceRestart"
        Me.mnuRemoteServiceRestart.Size = New System.Drawing.Size(257, 22)
        Me.mnuRemoteServiceRestart.Tag = "service"
        Me.mnuRemoteServiceRestart.Text = "Restart Windows Service..."
        '
        'mnuRemoteHealth
        '
        Me.mnuRemoteHealth.Name = "mnuRemoteHealth"
        Me.mnuRemoteHealth.Size = New System.Drawing.Size(257, 22)
        Me.mnuRemoteHealth.Tag = "health"
        Me.mnuRemoteHealth.Text = "System Health / Hardware Info"
        '
        'mnuRemoteNetwork
        '
        Me.mnuRemoteNetwork.Name = "mnuRemoteNetwork"
        Me.mnuRemoteNetwork.Size = New System.Drawing.Size(257, 22)
        Me.mnuRemoteNetwork.Tag = "network"
        Me.mnuRemoteNetwork.Text = "Network Configuration"
        '
        'mnuRemoteProcesses
        '
        Me.mnuRemoteProcesses.Name = "mnuRemoteProcesses"
        Me.mnuRemoteProcesses.Size = New System.Drawing.Size(257, 22)
        Me.mnuRemoteProcesses.Tag = "processes"
        Me.mnuRemoteProcesses.Text = "List Running Processes"
        '
        'mnuRemoteServices
        '
        Me.mnuRemoteServices.Name = "mnuRemoteServices"
        Me.mnuRemoteServices.Size = New System.Drawing.Size(257, 22)
        Me.mnuRemoteServices.Tag = "services"
        Me.mnuRemoteServices.Text = "List Windows Services"
        '
        'mnuRemoteKillProcess
        '
        Me.mnuRemoteKillProcess.Name = "mnuRemoteKillProcess"
        Me.mnuRemoteKillProcess.Size = New System.Drawing.Size(257, 22)
        Me.mnuRemoteKillProcess.Tag = "killprocess"
        Me.mnuRemoteKillProcess.Text = "Kill Process by Name..."
        '
        'mnuRemoteCustom
        '
        Me.mnuRemoteCustom.Name = "mnuRemoteCustom"
        Me.mnuRemoteCustom.Size = New System.Drawing.Size(257, 22)
        Me.mnuRemoteCustom.Tag = "custom"
        Me.mnuRemoteCustom.Text = "Execute Custom Command..."
        '
        'sepRemote1
        '
        Me.sepRemote1.Name = "sepRemote1"
        Me.sepRemote1.Size = New System.Drawing.Size(254, 6)
        '
        'sepRemote2
        '
        Me.sepRemote2.Name = "sepRemote2"
        Me.sepRemote2.Size = New System.Drawing.Size(254, 6)
        '
        'sepRemote3
        '
        Me.sepRemote3.Name = "sepRemote3"
        Me.sepRemote3.Size = New System.Drawing.Size(254, 6)
        '
        'sepRemote4
        '
        Me.sepRemote4.Name = "sepRemote4"
        Me.sepRemote4.Size = New System.Drawing.Size(254, 6)
        '
        'sepRemote5
        '
        Me.sepRemote5.Name = "sepRemote5"
        Me.sepRemote5.Size = New System.Drawing.Size(254, 6)
        '
        'sepRemote6
        '
        Me.sepRemote6.Name = "sepRemote6"
        Me.sepRemote6.Size = New System.Drawing.Size(254, 6)
        'filterLayout
        '
        Me.filterLayout.ColumnCount = 5
        Me.filterLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.filterLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.filterLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.filterLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180.0!))
        Me.filterLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 360.0!))
        Me.filterLayout.Controls.Add(Me.lblSearch, 0, 0)
        Me.filterLayout.Controls.Add(Me.txtSearch, 1, 0)
        Me.filterLayout.Controls.Add(Me.lblStatusFilter, 2, 0)
        Me.filterLayout.Controls.Add(Me.cboStatusFilter, 3, 0)
        Me.filterLayout.Dock = System.Windows.Forms.DockStyle.Fill
        Me.filterLayout.Location = New System.Drawing.Point(0, 134)
        Me.filterLayout.Margin = New System.Windows.Forms.Padding(0)
        Me.filterLayout.Name = "filterLayout"
        Me.filterLayout.RowCount = 1
        Me.filterLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.filterLayout.Size = New System.Drawing.Size(1160, 42)
        Me.filterLayout.TabIndex = 3
        '
        'lblSearch
        '
        Me.lblSearch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblSearch.Location = New System.Drawing.Point(0, 0)
        Me.lblSearch.Margin = New System.Windows.Forms.Padding(0)
        Me.lblSearch.Name = "lblSearch"
        Me.lblSearch.Size = New System.Drawing.Size(60, 42)
        Me.lblSearch.TabIndex = 0
        Me.lblSearch.Text = "Search:"
        Me.lblSearch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtSearch
        '
        Me.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtSearch.Location = New System.Drawing.Point(60, 8)
        Me.txtSearch.Margin = New System.Windows.Forms.Padding(0, 8, 10, 8)
        Me.txtSearch.Name = "txtSearch"
        Me.txtSearch.Size = New System.Drawing.Size(490, 23)
        Me.txtSearch.TabIndex = 1
        '
        'lblStatusFilter
        '
        Me.lblStatusFilter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblStatusFilter.Location = New System.Drawing.Point(560, 0)
        Me.lblStatusFilter.Margin = New System.Windows.Forms.Padding(0)
        Me.lblStatusFilter.Name = "lblStatusFilter"
        Me.lblStatusFilter.Size = New System.Drawing.Size(60, 42)
        Me.lblStatusFilter.TabIndex = 2
        Me.lblStatusFilter.Text = "Status:"
        Me.lblStatusFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cboStatusFilter
        '
        Me.cboStatusFilter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cboStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboStatusFilter.FormattingEnabled = True
        Me.cboStatusFilter.Location = New System.Drawing.Point(620, 8)
        Me.cboStatusFilter.Margin = New System.Windows.Forms.Padding(0, 8, 0, 8)
        Me.cboStatusFilter.Name = "cboStatusFilter"
        Me.cboStatusFilter.Size = New System.Drawing.Size(180, 23)
        Me.cboStatusFilter.TabIndex = 3
        '
        'deployLayout
        '
        Me.deployLayout.ColumnCount = 7
        Me.deployLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90.0!))
        Me.deployLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.deployLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84.0!))
        Me.deployLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 64.0!))
        Me.deployLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 116.0!))
        Me.deployLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 116.0!))
        Me.deployLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 170.0!))
        Me.deployLayout.Controls.Add(Me.lblPackage, 0, 0)
        Me.deployLayout.Controls.Add(Me.txtPackagePath, 1, 0)
        Me.deployLayout.Controls.Add(Me.btnBrowsePackage, 2, 0)
        Me.deployLayout.Controls.Add(Me.lblProcess, 3, 0)
        Me.deployLayout.Controls.Add(Me.txtProcessName, 4, 0)
        Me.deployLayout.Controls.Add(Me.chkStartAfterDeploy, 5, 0)
        Me.deployLayout.Controls.Add(Me.chkForceSameVersion, 6, 0)
        Me.deployLayout.Dock = System.Windows.Forms.DockStyle.Fill
        Me.deployLayout.Location = New System.Drawing.Point(0, 176)
        Me.deployLayout.Margin = New System.Windows.Forms.Padding(0)
        Me.deployLayout.Name = "deployLayout"
        Me.deployLayout.RowCount = 1
        Me.deployLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.deployLayout.Size = New System.Drawing.Size(1160, 40)
        Me.deployLayout.TabIndex = 3
        '
        'lblPackage
        '
        Me.lblPackage.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblPackage.Location = New System.Drawing.Point(0, 0)
        Me.lblPackage.Margin = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.lblPackage.Name = "lblPackage"
        Me.lblPackage.Size = New System.Drawing.Size(82, 40)
        Me.lblPackage.TabIndex = 0
        Me.lblPackage.Text = "Release folder:"
        Me.lblPackage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtPackagePath
        '
        Me.txtPackagePath.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtPackagePath.Location = New System.Drawing.Point(90, 8)
        Me.txtPackagePath.Margin = New System.Windows.Forms.Padding(0, 8, 8, 8)
        Me.txtPackagePath.Name = "txtPackagePath"
        Me.txtPackagePath.Size = New System.Drawing.Size(512, 23)
        Me.txtPackagePath.TabIndex = 1
        '
        'btnBrowsePackage
        '
        Me.btnBrowsePackage.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnBrowsePackage.Location = New System.Drawing.Point(610, 6)
        Me.btnBrowsePackage.Margin = New System.Windows.Forms.Padding(0, 6, 8, 6)
        Me.btnBrowsePackage.Name = "btnBrowsePackage"
        Me.btnBrowsePackage.Size = New System.Drawing.Size(76, 28)
        Me.btnBrowsePackage.TabIndex = 2
        Me.btnBrowsePackage.Text = "Browse"
        Me.btnBrowsePackage.UseVisualStyleBackColor = True
        '
        'lblProcess
        '
        Me.lblProcess.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblProcess.Location = New System.Drawing.Point(694, 0)
        Me.lblProcess.Margin = New System.Windows.Forms.Padding(0)
        Me.lblProcess.Name = "lblProcess"
        Me.lblProcess.Size = New System.Drawing.Size(64, 40)
        Me.lblProcess.TabIndex = 3
        Me.lblProcess.Text = "Process:"
        Me.lblProcess.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtProcessName
        '
        Me.txtProcessName.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtProcessName.Location = New System.Drawing.Point(758, 8)
        Me.txtProcessName.Margin = New System.Windows.Forms.Padding(0, 8, 8, 8)
        Me.txtProcessName.Name = "txtProcessName"
        Me.txtProcessName.Size = New System.Drawing.Size(108, 23)
        Me.txtProcessName.TabIndex = 4
        Me.txtProcessName.Text = "BJCBCPOS.exe"
        '
        'chkStartAfterDeploy
        '
        Me.chkStartAfterDeploy.AutoSize = True
        Me.chkStartAfterDeploy.Dock = System.Windows.Forms.DockStyle.Fill
        Me.chkStartAfterDeploy.Location = New System.Drawing.Point(874, 0)
        Me.chkStartAfterDeploy.Margin = New System.Windows.Forms.Padding(0)
        Me.chkStartAfterDeploy.Name = "chkStartAfterDeploy"
        Me.chkStartAfterDeploy.Size = New System.Drawing.Size(116, 40)
        Me.chkStartAfterDeploy.TabIndex = 5
        Me.chkStartAfterDeploy.Text = "Start after deploy"
        Me.chkStartAfterDeploy.UseVisualStyleBackColor = True
        '
        'chkForceSameVersion
        '
        Me.chkForceSameVersion.AutoSize = True
        Me.chkForceSameVersion.Dock = System.Windows.Forms.DockStyle.Fill
        Me.chkForceSameVersion.Location = New System.Drawing.Point(996, 0)
        Me.chkForceSameVersion.Margin = New System.Windows.Forms.Padding(6, 0, 0, 0)
        Me.chkForceSameVersion.Name = "chkForceSameVersion"
        Me.chkForceSameVersion.Size = New System.Drawing.Size(164, 40)
        Me.chkForceSameVersion.TabIndex = 6
        Me.chkForceSameVersion.Text = "Deploy same version"
        Me.chkForceSameVersion.UseVisualStyleBackColor = True
        '
        'dgvStores
        '
        Me.dgvStores.AllowUserToAddRows = False
        Me.dgvStores.AllowUserToDeleteRows = False
        Me.dgvStores.AllowUserToResizeRows = False
        Me.dgvStores.BackgroundColor = System.Drawing.SystemColors.ControlDark
        Me.dgvStores.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.dgvStores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvStores.ContextMenuStrip = Me.mnuStore
        Me.dgvStores.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvStores.Location = New System.Drawing.Point(0, 216)
        Me.dgvStores.Margin = New System.Windows.Forms.Padding(0)
        Me.dgvStores.MultiSelect = False
        Me.dgvStores.Name = "dgvStores"
        Me.dgvStores.ContextMenuStrip = Me.remoteToolsMenu
        Me.dgvStores.RowHeadersVisible = False
        Me.dgvStores.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvStores.Size = New System.Drawing.Size(1160, 316)
        Me.dgvStores.TabIndex = 4
        '
        'statusLayout
        '
        Me.statusLayout.ColumnCount = 2
        Me.statusLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.statusLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240.0!))
        Me.statusLayout.Controls.Add(Me.lblSummary, 0, 0)
        Me.statusLayout.Controls.Add(Me.lblProgress, 1, 0)
        Me.statusLayout.Dock = System.Windows.Forms.DockStyle.Fill
        Me.statusLayout.Location = New System.Drawing.Point(0, 532)
        Me.statusLayout.Margin = New System.Windows.Forms.Padding(0)
        Me.statusLayout.Name = "statusLayout"
        Me.statusLayout.RowCount = 1
        Me.statusLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.statusLayout.Size = New System.Drawing.Size(1160, 30)
        Me.statusLayout.TabIndex = 5
        '
        'lblSummary
        '
        Me.lblSummary.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblSummary.Location = New System.Drawing.Point(0, 0)
        Me.lblSummary.Margin = New System.Windows.Forms.Padding(0)
        Me.lblSummary.Name = "lblSummary"
        Me.lblSummary.Size = New System.Drawing.Size(920, 30)
        Me.lblSummary.TabIndex = 0
        Me.lblSummary.Text = "ทั้งหมด 0 | เลือก 0 | พร้อม 0 | ไม่พร้อม 0"
        Me.lblSummary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblProgress
        '
        Me.lblProgress.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblProgress.Location = New System.Drawing.Point(920, 0)
        Me.lblProgress.Margin = New System.Windows.Forms.Padding(0)
        Me.lblProgress.Name = "lblProgress"
        Me.lblProgress.Size = New System.Drawing.Size(240, 30)
        Me.lblProgress.TabIndex = 1
        Me.lblProgress.Text = "พร้อมใช้งาน"
        Me.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'progressCheck
        '
        Me.progressCheck.Dock = System.Windows.Forms.DockStyle.Fill
        Me.progressCheck.Location = New System.Drawing.Point(0, 565)
        Me.progressCheck.Margin = New System.Windows.Forms.Padding(0, 3, 0, 3)
        Me.progressCheck.Name = "progressCheck"
        Me.progressCheck.Size = New System.Drawing.Size(1160, 18)
        Me.progressCheck.TabIndex = 6
        '
        'grpLog
        '
        Me.grpLog.Controls.Add(Me.txtLog)
        Me.grpLog.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpLog.Location = New System.Drawing.Point(0, 586)
        Me.grpLog.Margin = New System.Windows.Forms.Padding(0)
        Me.grpLog.Name = "grpLog"
        Me.grpLog.Padding = New System.Windows.Forms.Padding(8, 6, 8, 8)
        Me.grpLog.Size = New System.Drawing.Size(1160, 150)
        Me.grpLog.TabIndex = 7
        Me.grpLog.TabStop = False
        Me.grpLog.Text = "Log"
        '
        'txtLog
        '
        Me.txtLog.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtLog.Location = New System.Drawing.Point(8, 22)
        Me.txtLog.Multiline = True
        Me.txtLog.Name = "txtLog"
        Me.txtLog.ReadOnly = True
        Me.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtLog.Size = New System.Drawing.Size(1144, 120)
        Me.txtLog.TabIndex = 0
        Me.txtLog.WordWrap = False
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(1184, 760)
        Me.Controls.Add(Me.rootLayout)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.MinimumSize = New System.Drawing.Size(1120, 700)
        Me.Name = "frmMain"
                Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "POS Deploy Tool - Sprint 5.2 System Health"
        Me.rootLayout.ResumeLayout(False)
        Me.navigationPanel.ResumeLayout(False)
        Me.pnlNavMenu.ResumeLayout(False)
        Me.contentPanel.ResumeLayout(False)
        Me.mnuStore.ResumeLayout(False)
        Me.remoteToolsMenu.ResumeLayout(False)
        Me.mainLayout.ResumeLayout(False)
        Me.mainLayout.PerformLayout()
        Me.configLayout.ResumeLayout(False)
        Me.configLayout.PerformLayout()
        Me.actionLayout.ResumeLayout(False)
        Me.actionLayout.PerformLayout()
        CType(Me.nudParallel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.filterLayout.ResumeLayout(False)
        Me.filterLayout.PerformLayout()
        Me.deployLayout.ResumeLayout(False)
        Me.deployLayout.PerformLayout()
        CType(Me.dgvStores, System.ComponentModel.ISupportInitialize).EndInit()
        Me.statusLayout.ResumeLayout(False)
        Me.grpLog.ResumeLayout(False)
        Me.grpLog.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents rootLayout As TableLayoutPanel
    Friend WithEvents navigationPanel As Panel
    Friend WithEvents pnlNavMenu As FlowLayoutPanel
    Friend WithEvents btnNavDashboard As Button
    Friend WithEvents btnNavConnection As Button
    Friend WithEvents btnNavDeploy As Button
    Friend WithEvents btnNavLogs As Button
    Friend WithEvents btnNavSettings As Button
    Friend WithEvents lblBrand As Label
    Friend WithEvents lblVersion As Label
    Friend WithEvents contentPanel As Panel
    Friend WithEvents mnuStore As ContextMenuStrip
    Friend WithEvents mnuDeleteStore As ToolStripMenuItem
    Friend WithEvents mainLayout As TableLayoutPanel
    Friend WithEvents lblTitle As Label
    Friend WithEvents configLayout As TableLayoutPanel
    Friend WithEvents lblConfig As Label
    Friend WithEvents txtConfigPath As TextBox
    Friend WithEvents btnBrowseConfig As Button
    Friend WithEvents btnReload As Button
    Friend WithEvents btnSelectAll As Button
    Friend WithEvents btnClearAll As Button
    Friend WithEvents actionLayout As FlowLayoutPanel
    Friend WithEvents btnCheckPing As Button
    Friend WithEvents btnCheckSelected As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents lblParallel As Label
    Friend WithEvents nudParallel As NumericUpDown
    Friend WithEvents btnBackupSelected As Button
    Friend WithEvents btnDeploySelected As Button
    Friend WithEvents btnValidateSelected As Button
    Friend WithEvents btnRemoteTools As Button
    Friend WithEvents btnCheckHealth As Button
    Friend WithEvents btnExportHealth As Button
    Friend WithEvents remoteToolsMenu As ContextMenuStrip
    Friend WithEvents mnuRemoteCheck As ToolStripMenuItem
    Friend WithEvents sepRemote1 As ToolStripSeparator
    Friend WithEvents mnuRemoteTaskInstall As ToolStripMenuItem
    Friend WithEvents mnuRemoteTaskRemove As ToolStripMenuItem
    Friend WithEvents sepRemote2 As ToolStripSeparator
    Friend WithEvents mnuRemoteStart As ToolStripMenuItem
    Friend WithEvents mnuRemoteStop As ToolStripMenuItem
    Friend WithEvents mnuRemoteRestart As ToolStripMenuItem
    Friend WithEvents sepRemote3 As ToolStripSeparator
    Friend WithEvents mnuRemoteReboot As ToolStripMenuItem
    Friend WithEvents mnuRemoteAbort As ToolStripMenuItem
    Friend WithEvents sepRemote4 As ToolStripSeparator
    Friend WithEvents mnuRemoteServiceStart As ToolStripMenuItem
    Friend WithEvents mnuRemoteServiceStop As ToolStripMenuItem
    Friend WithEvents mnuRemoteServiceRestart As ToolStripMenuItem
    Friend WithEvents sepRemote5 As ToolStripSeparator
    Friend WithEvents mnuRemoteHealth As ToolStripMenuItem
    Friend WithEvents mnuRemoteNetwork As ToolStripMenuItem
    Friend WithEvents mnuRemoteProcesses As ToolStripMenuItem
    Friend WithEvents mnuRemoteServices As ToolStripMenuItem
    Friend WithEvents sepRemote6 As ToolStripSeparator
    Friend WithEvents mnuRemoteKillProcess As ToolStripMenuItem
    Friend WithEvents mnuRemoteCustom As ToolStripMenuItem
    Friend WithEvents filterLayout As TableLayoutPanel
    Friend WithEvents lblSearch As Label
    Friend WithEvents txtSearch As TextBox
    Friend WithEvents lblStatusFilter As Label
    Friend WithEvents cboStatusFilter As ComboBox
    Friend WithEvents btnRealVnc As Button
    Friend WithEvents deployLayout As TableLayoutPanel
    Friend WithEvents lblPackage As Label
    Friend WithEvents txtPackagePath As TextBox
    Friend WithEvents btnBrowsePackage As Button
    Friend WithEvents lblProcess As Label
    Friend WithEvents txtProcessName As TextBox
    Friend WithEvents chkStartAfterDeploy As CheckBox
    Friend WithEvents chkForceSameVersion As CheckBox
    Friend WithEvents dgvStores As DataGridView
    Friend WithEvents statusLayout As TableLayoutPanel
    Friend WithEvents lblSummary As Label
    Friend WithEvents lblProgress As Label
    Friend WithEvents progressCheck As ProgressBar
    Friend WithEvents grpLog As GroupBox
    Friend WithEvents txtLog As TextBox
End Class