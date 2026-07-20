Imports System.IO

Namespace Helpers
    Public NotInheritable Class AppPaths
        Private Sub New()
        End Sub

        Public Shared ReadOnly Property BaseDirectory As String
            Get
                Return AppDomain.CurrentDomain.BaseDirectory
            End Get
        End Property

        Public Shared ReadOnly Property DataDirectory As String
            Get
                Return EnsureDirectory(Path.Combine(BaseDirectory, "Data"))
            End Get
        End Property

        Public Shared ReadOnly Property LogDirectory As String
            Get
                Return EnsureDirectory(Path.Combine(BaseDirectory, "Logs"))
            End Get
        End Property

        Public Shared ReadOnly Property AuditDirectory As String
            Get
                Return EnsureDirectory(Path.Combine(LogDirectory, "Audit"))
            End Get
        End Property

        Public Shared ReadOnly Property CrashDirectory As String
            Get
                Return EnsureDirectory(Path.Combine(LogDirectory, "Crash"))
            End Get
        End Property

        Public Shared ReadOnly Property SettingsFile As String
            Get
                Return Path.Combine(DataDirectory, "appsettings.json")
            End Get
        End Property

        Public Shared Function Resolve(relativeOrAbsolutePath As String) As String
            If String.IsNullOrWhiteSpace(relativeOrAbsolutePath) Then Return BaseDirectory
            If Path.IsPathRooted(relativeOrAbsolutePath) Then Return relativeOrAbsolutePath
            Return Path.GetFullPath(Path.Combine(BaseDirectory, relativeOrAbsolutePath))
        End Function

        Private Shared Function EnsureDirectory(path As String) As String
            Directory.CreateDirectory(path)
            Return path
        End Function
    End Class
End Namespace
