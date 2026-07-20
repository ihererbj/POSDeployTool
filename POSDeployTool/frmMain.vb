Public Class frmMain
    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AppPaths.CreateFolders()

        FileLogger.Write("POSDeployTool Started")
    End Sub
End Class
