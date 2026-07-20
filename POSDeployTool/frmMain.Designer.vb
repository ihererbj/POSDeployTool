<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits Global.System.Windows.Forms.Form

    <Global.System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As Global.System.ComponentModel.IContainer

    <Global.System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.pnlTop = New Global.System.Windows.Forms.Panel()
        Me.flpActions = New Global.System.Windows.Forms.FlowLayoutPanel()
        Me.btnReload = New Global.System.Windows.Forms.Button()
        Me.btnCheckConnection = New Global.System.Windows.Forms.Button()
        Me.btnDeploy = New Global.System.Windows.Forms.Button()
        Me.btnStop = New Global.System.Windows.Forms.Button()
        Me.lblFilter = New Global.System.Windows.Forms.Label()
        Me.txtFilter = New Global.System.Windows.Forms.TextBox()
        Me.lblHeader = New Global.System.Windows.Forms.Label()
        Me.splitMain = New Global.System.Windows.Forms.SplitContainer()
        Me.dgvStores = New Global.System.Windows.Forms.DataGridView()
        Me.colSelected = New Global.System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.colStoreCode = New Global.System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colStoreName = New Global.System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colComputerName = New Global.System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colIpAddress = New Global.System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colPing = New Global.System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colWinRm = New Global.System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colVersion = New Global.System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colStatus = New Global.System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.pnlLogHeader = New Global.System.Windows.Forms.Panel()
        Me.btnClearLog = New Global.System.Windows.Forms.Button()
        Me.lblLog = New Global.System.Windows.Forms.Label()
        Me.rtbLog = New Global.System.Windows.Forms.RichTextBox()
        Me.statusMain = New Global.System.Windows.Forms.StatusStrip()
        Me.lblStatus = New Global.System.Windows.Forms.ToolStripStatusLabel()
        Me.lblStatusSpacer = New Global.System.Windows.Forms.ToolStripStatusLabel()
        Me.lblSelectedCount = New Global.System.Windows.Forms.ToolStripStatusLabel()
        Me.lblTotalCount = New Global.System.Windows.Forms.ToolStripStatusLabel()
        Me.pnlTop.SuspendLayout()
        Me.flpActions.SuspendLayout()
        CType(Me.splitMain, Global.System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitMain.Panel1.SuspendLayout()
        Me.splitMain.Panel2.SuspendLayout()
        Me.splitMain.SuspendLayout()
        CType(Me.dgvStores, Global.System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlLogHeader.SuspendLayout()
        Me.statusMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlTop
        '
        Me.pnlTop.Controls.Add(Me.flpActions)
        Me.pnlTop.Controls.Add(Me.lblFilter)
        Me.pnlTop.Controls.Add(Me.txtFilter)
        Me.pnlTop.Controls.Add(Me.lblHeader)
        Me.pnlTop.Dock = Global.System.Windows.Forms.DockStyle.Top
        Me.pnlTop.Location = New Global.System.Drawing.Point(0, 0)
        Me.pnlTop.Name = "pnlTop"
        Me.pnlTop.Padding = New Global.System.Windows.Forms.Padding(12, 8, 12, 8)
        Me.pnlTop.Size = New Global.System.Drawing.Size(1184, 76)
        Me.pnlTop.TabIndex = 0
        '
        'flpActions
        '
        Me.flpActions.Anchor = CType(
            Global.System.Windows.Forms.AnchorStyles.Top Or
            Global.System.Windows.Forms.AnchorStyles.Right,
            Global.System.Windows.Forms.AnchorStyles)
        Me.flpActions.AutoSize = True
        Me.flpActions.Controls.Add(Me.btnReload)
        Me.flpActions.Controls.Add(Me.btnCheckConnection)
        Me.flpActions.Controls.Add(Me.btnDeploy)
        Me.flpActions.Controls.Add(Me.btnStop)
        Me.flpActions.FlowDirection =
            Global.System.Windows.Forms.FlowDirection.LeftToRight
        Me.flpActions.Location = New Global.System.Drawing.Point(766, 18)
        Me.flpActions.Name = "flpActions"
        Me.flpActions.Size = New Global.System.Drawing.Size(406, 35)
        Me.flpActions.TabIndex = 3
        Me.flpActions.WrapContents = False
        '
        'btnReload
        '
        Me.btnReload.Location = New Global.System.Drawing.Point(3, 3)
        Me.btnReload.Name = "btnReload"
        Me.btnReload.Size = New Global.System.Drawing.Size(90, 29)
        Me.btnReload.TabIndex = 0
        Me.btnReload.Text = "Reload"
        Me.btnReload.UseVisualStyleBackColor = True
        '
        'btnCheckConnection
        '
        Me.btnCheckConnection.Enabled = False
        Me.btnCheckConnection.Location = New Global.System.Drawing.Point(99, 3)
        Me.btnCheckConnection.Name = "btnCheckConnection"
        Me.btnCheckConnection.Size = New Global.System.Drawing.Size(120, 29)
        Me.btnCheckConnection.TabIndex = 1
        Me.btnCheckConnection.Text = "Check Connection"
        Me.btnCheckConnection.UseVisualStyleBackColor = True
        '
        'btnDeploy
        '
        Me.btnDeploy.Enabled = False
        Me.btnDeploy.Location = New Global.System.Drawing.Point(225, 3)
        Me.btnDeploy.Name = "btnDeploy"
        Me.btnDeploy.Size = New Global.System.Drawing.Size(80, 29)
        Me.btnDeploy.TabIndex = 2
        Me.btnDeploy.Text = "Deploy"
        Me.btnDeploy.UseVisualStyleBackColor = True
        '
        'btnStop
        '
        Me.btnStop.Enabled = False
        Me.btnStop.Location = New Global.System.Drawing.Point(311, 3)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New Global.System.Drawing.Size(80, 29)
        Me.btnStop.TabIndex = 3
        Me.btnStop.Text = "Stop"
        Me.btnStop.UseVisualStyleBackColor = True
        '
        'lblFilter
        '
        Me.lblFilter.AutoSize = True
        Me.lblFilter.Location = New Global.System.Drawing.Point(261, 31)
        Me.lblFilter.Name = "lblFilter"
        Me.lblFilter.Size = New Global.System.Drawing.Size(35, 13)
        Me.lblFilter.TabIndex = 1
        Me.lblFilter.Text = "Filter:"
        '
        'txtFilter
        '
        Me.txtFilter.Location = New Global.System.Drawing.Point(302, 27)
        Me.txtFilter.Name = "txtFilter"
        Me.txtFilter.Size = New Global.System.Drawing.Size(260, 20)
        Me.txtFilter.TabIndex = 2
        '
        'lblHeader
        '
        Me.lblHeader.AutoSize = True
        Me.lblHeader.Font =
            New Global.System.Drawing.Font(
                "Segoe UI",
                14.0!,
                Global.System.Drawing.FontStyle.Bold)
        Me.lblHeader.Location = New Global.System.Drawing.Point(12, 21)
        Me.lblHeader.Name = "lblHeader"
        Me.lblHeader.Size = New Global.System.Drawing.Size(162, 25)
        Me.lblHeader.TabIndex = 0
        Me.lblHeader.Text = "POS Deploy Tool"
        '
        'splitMain
        '
        Me.splitMain.Dock = Global.System.Windows.Forms.DockStyle.Fill
        Me.splitMain.Location = New Global.System.Drawing.Point(0, 76)
        Me.splitMain.Name = "splitMain"
        Me.splitMain.Orientation =
            Global.System.Windows.Forms.Orientation.Horizontal
        '
        'splitMain.Panel1
        '
        Me.splitMain.Panel1.Controls.Add(Me.dgvStores)
        '
        'splitMain.Panel2
        '
        Me.splitMain.Panel2.Controls.Add(Me.rtbLog)
        Me.splitMain.Panel2.Controls.Add(Me.pnlLogHeader)
        Me.splitMain.Size = New Global.System.Drawing.Size(1184, 635)
        Me.splitMain.SplitterDistance = 423
        Me.splitMain.TabIndex = 1
        '
        'dgvStores
        '
        Me.dgvStores.AllowUserToAddRows = False
        Me.dgvStores.AllowUserToDeleteRows = False
        Me.dgvStores.AllowUserToResizeRows = False
        Me.dgvStores.AutoGenerateColumns = False
        Me.dgvStores.BackgroundColor =
            Global.System.Drawing.SystemColors.Window
        Me.dgvStores.BorderStyle =
            Global.System.Windows.Forms.BorderStyle.Fixed3D
        Me.dgvStores.ColumnHeadersHeightSizeMode =
            Global.System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvStores.Columns.AddRange(
            New Global.System.Windows.Forms.DataGridViewColumn() {
                Me.colSelected,
                Me.colStoreCode,
                Me.colStoreName,
                Me.colComputerName,
                Me.colIpAddress,
                Me.colPing,
                Me.colWinRm,
                Me.colVersion,
                Me.colStatus})
        Me.dgvStores.Dock = Global.System.Windows.Forms.DockStyle.Fill
        Me.dgvStores.Location = New Global.System.Drawing.Point(0, 0)
        Me.dgvStores.MultiSelect = False
        Me.dgvStores.Name = "dgvStores"
        Me.dgvStores.RowHeadersVisible = False
        Me.dgvStores.SelectionMode =
            Global.System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvStores.Size = New Global.System.Drawing.Size(1184, 423)
        Me.dgvStores.TabIndex = 0
        '
        'colSelected
        '
        Me.colSelected.DataPropertyName = "Selected"
        Me.colSelected.HeaderText = ""
        Me.colSelected.Name = "colSelected"
        Me.colSelected.Width = 35
        '
        'colStoreCode
        '
        Me.colStoreCode.DataPropertyName = "StoreCode"
        Me.colStoreCode.HeaderText = "Store"
        Me.colStoreCode.Name = "colStoreCode"
        Me.colStoreCode.ReadOnly = True
        Me.colStoreCode.Width = 75
        '
        'colStoreName
        '
        Me.colStoreName.AutoSizeMode =
            Global.System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.colStoreName.DataPropertyName = "StoreName"
        Me.colStoreName.HeaderText = "Store Name"
        Me.colStoreName.MinimumWidth = 160
        Me.colStoreName.Name = "colStoreName"
        Me.colStoreName.ReadOnly = True
        '
        'colComputerName
        '
        Me.colComputerName.DataPropertyName = "ComputerName"
        Me.colComputerName.HeaderText = "Computer"
        Me.colComputerName.Name = "colComputerName"
        Me.colComputerName.ReadOnly = True
        Me.colComputerName.Width = 120
        '
        'colIpAddress
        '
        Me.colIpAddress.DataPropertyName = "IpAddress"
        Me.colIpAddress.HeaderText = "IP Address"
        Me.colIpAddress.Name = "colIpAddress"
        Me.colIpAddress.ReadOnly = True
        Me.colIpAddress.Width = 120
        '
        'colPing
        '
        Me.colPing.HeaderText = "Ping"
        Me.colPing.Name = "colPing"
        Me.colPing.ReadOnly = True
        Me.colPing.Width = 70
        '
        'colWinRm
        '
        Me.colWinRm.HeaderText = "WinRM"
        Me.colWinRm.Name = "colWinRm"
        Me.colWinRm.ReadOnly = True
        Me.colWinRm.Width = 75
        '
        'colVersion
        '
        Me.colVersion.HeaderText = "Version"
        Me.colVersion.Name = "colVersion"
        Me.colVersion.ReadOnly = True
        Me.colVersion.Width = 90
        '
        'colStatus
        '
        Me.colStatus.HeaderText = "Status"
        Me.colStatus.Name = "colStatus"
        Me.colStatus.ReadOnly = True
        Me.colStatus.Width = 130
        '
        'pnlLogHeader
        '
        Me.pnlLogHeader.Controls.Add(Me.btnClearLog)
        Me.pnlLogHeader.Controls.Add(Me.lblLog)
        Me.pnlLogHeader.Dock = Global.System.Windows.Forms.DockStyle.Top
        Me.pnlLogHeader.Location = New Global.System.Drawing.Point(0, 0)
        Me.pnlLogHeader.Name = "pnlLogHeader"
        Me.pnlLogHeader.Size = New Global.System.Drawing.Size(1184, 35)
        Me.pnlLogHeader.TabIndex = 0
        '
        'btnClearLog
        '
        Me.btnClearLog.Anchor = CType(
            Global.System.Windows.Forms.AnchorStyles.Top Or
            Global.System.Windows.Forms.AnchorStyles.Right,
            Global.System.Windows.Forms.AnchorStyles)
        Me.btnClearLog.Location = New Global.System.Drawing.Point(1097, 5)
        Me.btnClearLog.Name = "btnClearLog"
        Me.btnClearLog.Size = New Global.System.Drawing.Size(75, 25)
        Me.btnClearLog.TabIndex = 1
        Me.btnClearLog.Text = "Clear"
        Me.btnClearLog.UseVisualStyleBackColor = True
        '
        'lblLog
        '
        Me.lblLog.AutoSize = True
        Me.lblLog.Font =
            New Global.System.Drawing.Font(
                "Segoe UI",
                9.0!,
                Global.System.Drawing.FontStyle.Bold)
        Me.lblLog.Location = New Global.System.Drawing.Point(9, 10)
        Me.lblLog.Name = "lblLog"
        Me.lblLog.Size = New Global.System.Drawing.Size(27, 15)
        Me.lblLog.TabIndex = 0
        Me.lblLog.Text = "Log"
        '
        'rtbLog
        '
        Me.rtbLog.BackColor = Global.System.Drawing.Color.White
        Me.rtbLog.Dock = Global.System.Windows.Forms.DockStyle.Fill
        Me.rtbLog.Font =
            New Global.System.Drawing.Font(
                "Consolas",
                9.0!,
                Global.System.Drawing.FontStyle.Regular)
        Me.rtbLog.Location = New Global.System.Drawing.Point(0, 35)
        Me.rtbLog.Name = "rtbLog"
        Me.rtbLog.ReadOnly = True
        Me.rtbLog.Size = New Global.System.Drawing.Size(1184, 173)
        Me.rtbLog.TabIndex = 1
        Me.rtbLog.Text = ""
        Me.rtbLog.WordWrap = False
        '
        'statusMain
        '
        Me.statusMain.Items.AddRange(
            New Global.System.Windows.Forms.ToolStripItem() {
                Me.lblStatus,
                Me.lblStatusSpacer,
                Me.lblSelectedCount,
                Me.lblTotalCount})
        Me.statusMain.Location = New Global.System.Drawing.Point(0, 711)
        Me.statusMain.Name = "statusMain"
        Me.statusMain.Size = New Global.System.Drawing.Size(1184, 22)
        Me.statusMain.TabIndex = 2
        '
        'lblStatus
        '
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New Global.System.Drawing.Size(39, 17)
        Me.lblStatus.Text = "Ready"
        '
        'lblStatusSpacer
        '
        Me.lblStatusSpacer.Name = "lblStatusSpacer"
        Me.lblStatusSpacer.Size = New Global.System.Drawing.Size(962, 17)
        Me.lblStatusSpacer.Spring = True
        '
        'lblSelectedCount
        '
        Me.lblSelectedCount.Name = "lblSelectedCount"
        Me.lblSelectedCount.Size = New Global.System.Drawing.Size(70, 17)
        Me.lblSelectedCount.Text = "Selected: 0"
        '
        'lblTotalCount
        '
        Me.lblTotalCount.Margin =
            New Global.System.Windows.Forms.Padding(15, 3, 0, 2)
        Me.lblTotalCount.Name = "lblTotalCount"
        Me.lblTotalCount.Size = New Global.System.Drawing.Size(44, 17)
        Me.lblTotalCount.Text = "Total: 0"
        '
        'frmMain
        '
        Me.AutoScaleDimensions =
            New Global.System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode =
            Global.System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New Global.System.Drawing.Size(1184, 733)
        Me.Controls.Add(Me.splitMain)
        Me.Controls.Add(Me.pnlTop)
        Me.Controls.Add(Me.statusMain)
        Me.MinimumSize = New Global.System.Drawing.Size(1000, 650)
        Me.Name = "frmMain"
        Me.StartPosition =
            Global.System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "POS Deploy Tool"
        Me.pnlTop.ResumeLayout(False)
        Me.pnlTop.PerformLayout()
        Me.flpActions.ResumeLayout(False)
        Me.splitMain.Panel1.ResumeLayout(False)
        Me.splitMain.Panel2.ResumeLayout(False)
        CType(Me.splitMain, Global.System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitMain.ResumeLayout(False)
        CType(Me.dgvStores, Global.System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlLogHeader.ResumeLayout(False)
        Me.pnlLogHeader.PerformLayout()
        Me.statusMain.ResumeLayout(False)
        Me.statusMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents pnlTop As Global.System.Windows.Forms.Panel
    Friend WithEvents flpActions As Global.System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnReload As Global.System.Windows.Forms.Button
    Friend WithEvents btnCheckConnection As Global.System.Windows.Forms.Button
    Friend WithEvents btnDeploy As Global.System.Windows.Forms.Button
    Friend WithEvents btnStop As Global.System.Windows.Forms.Button
    Friend WithEvents lblFilter As Global.System.Windows.Forms.Label
    Friend WithEvents txtFilter As Global.System.Windows.Forms.TextBox
    Friend WithEvents lblHeader As Global.System.Windows.Forms.Label
    Friend WithEvents splitMain As Global.System.Windows.Forms.SplitContainer
    Friend WithEvents dgvStores As Global.System.Windows.Forms.DataGridView
    Friend WithEvents colSelected As Global.System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents colStoreCode As Global.System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colStoreName As Global.System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colComputerName As Global.System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colIpAddress As Global.System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colPing As Global.System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colWinRm As Global.System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colVersion As Global.System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colStatus As Global.System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents pnlLogHeader As Global.System.Windows.Forms.Panel
    Friend WithEvents btnClearLog As Global.System.Windows.Forms.Button
    Friend WithEvents lblLog As Global.System.Windows.Forms.Label
    Friend WithEvents rtbLog As Global.System.Windows.Forms.RichTextBox
    Friend WithEvents statusMain As Global.System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As Global.System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents lblStatusSpacer As Global.System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents lblSelectedCount As Global.System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents lblTotalCount As Global.System.Windows.Forms.ToolStripStatusLabel

End Class