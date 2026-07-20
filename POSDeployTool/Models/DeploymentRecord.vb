Imports System

Namespace Models
    Public Class DeploymentRecord
        Public Property StartedAt As DateTime
        Public Property FinishedAt As DateTime
        Public Property StoreCode As String
        Public Property StoreName As String
        Public Property IpAddress As String
        Public Property ComputerName As String
        Public Property OldVersion As String
        Public Property ReleaseVersion As String
        Public Property Result As String
        Public Property Message As String
        Public Property ElapsedSeconds As Double
        Public Property OperatorName As String
    End Class
End Namespace
