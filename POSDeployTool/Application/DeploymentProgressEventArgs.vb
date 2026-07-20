Imports POSDeployTool.Models

Namespace Application

    Public Class DeploymentProgressEventArgs
        Inherits EventArgs

        Public Sub New(store As StoreInfo, message As String)
            Me.Store = store
            Me.Message = If(message, String.Empty)
        End Sub

        Public ReadOnly Property Store As StoreInfo
        Public ReadOnly Property Message As String

    End Class

End Namespace
