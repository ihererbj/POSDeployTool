Imports System.IO

Public Class FileLogger

    Public Shared Sub Write(message As String)

        AppPaths.CreateFolders()

        Dim logfile As String =
            Path.Combine(
                AppPaths.LogPath,
                Date.Today.ToString("yyyyMMdd") & ".log")

        File.AppendAllText(
            logfile,
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") &
            "  " &
            message &
            Environment.NewLine)

    End Sub

End Class