Imports POSDeployTool.Models
Imports POSDeployTool.Services

Public Class frmMain

    Private ReadOnly _storeConfigService As New StoreConfigService()
    Private _stores As New List(Of StoreInfo)()

    Private Sub frmMain_Load(
        sender As Object,
        e As EventArgs
    ) Handles MyBase.Load

        Try
            AppPaths.CreateFolders()
            FileLogger.Write("POSDeployTool started.")

            _stores = _storeConfigService.LoadStores()

            Me.Text = String.Format(
                "POS Deploy Tool - {0} Store(s)",
                _stores.Count)

            FileLogger.Write(
                String.Format(
                    "Loaded {0} store(s).",
                    _stores.Count))

        Catch ex As Exception

            FileLogger.Write(
                "Failed to load stores: " & ex.ToString())

            MessageBox.Show(
                ex.Message,
                "Configuration Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)

        End Try

    End Sub

End Class