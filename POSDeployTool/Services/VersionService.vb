Imports System.Diagnostics
Imports System.IO

Namespace Services
    Public Class VersionService
        Public Function GetReleaseVersion(packagePath As String, executableName As String) As String
            If String.IsNullOrWhiteSpace(packagePath) OrElse Not Directory.Exists(packagePath) Then Return "Not found"

            Dim exe As String = If(executableName, String.Empty).Trim()
            If String.IsNullOrWhiteSpace(exe) Then Return "Not found"

            Dim fullPath As String = Path.Combine(packagePath, exe)
            If Not File.Exists(fullPath) Then Return "Not found"

            Dim info As FileVersionInfo = FileVersionInfo.GetVersionInfo(fullPath)
            If Not String.IsNullOrWhiteSpace(info.FileVersion) Then Return info.FileVersion
            If Not String.IsNullOrWhiteSpace(info.ProductVersion) Then Return info.ProductVersion
            Return "Unknown"
        End Function
    End Class
End Namespace
