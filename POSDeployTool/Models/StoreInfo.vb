Namespace Models

    Public Class StoreInfo

        Public Sub New()
            Selected = True
            Enabled = True

            StoreCode = String.Empty
            StoreName = String.Empty
            ComputerName = String.Empty
            IpAddress = String.Empty

            Username = String.Empty
            Password = String.Empty

            Port = 5985
            UseHttps = False

            TargetPath = "C:\BCHKPOS"
            BackupPath = "C:\Backup"

            Connection = New ConnectionState()
            Deployment = New DeploymentState()
        End Sub

        Public Property Selected As Boolean
        Public Property Enabled As Boolean

        Public Property StoreCode As String
        Public Property StoreName As String
        Public Property IpAddress As String
        Public Property ComputerName As String

        Public Property Username As String
        Public Property Password As String

        Public Property Port As Integer
        Public Property UseHttps As Boolean

        Public Property TargetPath As String
        Public Property BackupPath As String

        Public Property Connection As ConnectionState
        Public Property Deployment As DeploymentState

        Public Overrides Function ToString() As String
            Return String.Format(
                "{0} - {1} ({2})",
                If(StoreCode, String.Empty),
                If(StoreName, String.Empty),
                If(IpAddress, String.Empty))
        End Function

    End Class

End Namespace
