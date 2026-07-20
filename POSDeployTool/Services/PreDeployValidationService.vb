Imports POSDeployTool.Contracts
Imports POSDeployTool.Models

Namespace Services

    Public Class PreDeployValidationService
        Implements IPreDeployValidationService

        Public Function Validate(store As StoreInfo) As PreDeployValidationResult Implements IPreDeployValidationService.Validate
            Dim result As New PreDeployValidationResult()

            If store Is Nothing Then
                result.Errors.Add("Store information is missing")
                Return result
            End If

            If Not store.Enabled Then
                result.Errors.Add("Store is disabled")
            End If

            If Not store.Selected Then
                result.Errors.Add("Store is not selected")
            End If

            If String.IsNullOrWhiteSpace(store.StoreCode) Then
                result.Errors.Add("Store code is required")
            End If

            If String.IsNullOrWhiteSpace(store.IpAddress) Then
                result.Errors.Add("IP address is required")
            End If

            If String.IsNullOrWhiteSpace(store.Username) Then
                result.Errors.Add("WinRM username is required")
            End If

            If String.IsNullOrWhiteSpace(store.TargetPath) Then
                result.Errors.Add("Target path is required")
            ElseIf Not IsAbsoluteWindowsPath(store.TargetPath) Then
                result.Errors.Add("Target path must be an absolute Windows path")
            End If

            If String.IsNullOrWhiteSpace(store.BackupPath) Then
                result.Errors.Add("Backup path is required")
            ElseIf Not IsAbsoluteWindowsPath(store.BackupPath) Then
                result.Errors.Add("Backup path must be an absolute Windows path")
            End If

            If store.Connection Is Nothing OrElse Not store.Connection.CanDeploy Then
                result.Errors.Add("Ping and WinRM checks must pass before deployment")
            End If

            Return result
        End Function

        Private Shared Function IsAbsoluteWindowsPath(value As String) As Boolean
            If String.IsNullOrWhiteSpace(value) OrElse value.Length < 3 Then
                Return False
            End If

            Return Char.IsLetter(value(0)) AndAlso
                   value(1) = ":"c AndAlso
                   (value(2) = "\"c OrElse value(2) = "/"c)
        End Function

    End Class

End Namespace
