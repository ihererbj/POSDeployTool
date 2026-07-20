Imports System.IO

Public NotInheritable Class AppPaths

    Private Sub New()
    End Sub

    Public Shared ReadOnly Property ApplicationPath As String
        Get
            Return AppDomain.CurrentDomain.BaseDirectory
        End Get
    End Property

    Public Shared ReadOnly Property ConfigPath As String
        Get
            Return Path.Combine(ApplicationPath, "Config")
        End Get
    End Property

    Public Shared ReadOnly Property LogPath As String
        Get
            Return Path.Combine(ApplicationPath, "Logs")
        End Get
    End Property

    Public Shared ReadOnly Property ReleasePath As String
        Get
            Return Path.Combine(ApplicationPath, "Release")
        End Get
    End Property

    Public Shared Sub CreateFolders()

        Directory.CreateDirectory(ConfigPath)
        Directory.CreateDirectory(LogPath)
        Directory.CreateDirectory(ReleasePath)

    End Sub

    Public Shared ReadOnly Property StoreConfigFile As String
        Get
            Return Path.Combine(ConfigPath, "stores.json")
        End Get
    End Property

End Class