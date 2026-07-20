Imports System.Threading
Imports POSDeployTool.Contracts
Imports POSDeployTool.Contracts.Deployment
Imports POSDeployTool.Models
Imports POSDeployTool.Models.Deployment

Namespace Plugins.Deployment

    Public Class PreDeployValidationStep
        Implements IDeploymentStep

        Private ReadOnly _validationService As IPreDeployValidationService

        Public Sub New(
            validationService As IPreDeployValidationService
        )
            If validationService Is Nothing Then
                Throw New ArgumentNullException(
                    NameOf(validationService))
            End If

            _validationService = validationService
        End Sub

        Public ReadOnly Property Name As String _
            Implements IDeploymentStep.Name
            Get
                Return "Validating"
            End Get
        End Property

        Public ReadOnly Property Order As Integer _
            Implements IDeploymentStep.Order
            Get
                Return 100
            End Get
        End Property

        Public Function ExecuteAsync(
            context As DeploymentContext,
            cancellationToken As CancellationToken
        ) As Task(Of DeploymentStepResult) _
            Implements IDeploymentStep.ExecuteAsync

            If context Is Nothing Then
                Throw New ArgumentNullException(
                    NameOf(context))
            End If

            cancellationToken.ThrowIfCancellationRequested()

            Dim validationResult As PreDeployValidationResult =
                _validationService.Validate(context.Store)

            cancellationToken.ThrowIfCancellationRequested()

            If validationResult.Success Then
                Return Task.FromResult(
                    DeploymentStepResult.Passed(
                        validationResult.Message))
            End If

            Return Task.FromResult(
                DeploymentStepResult.Failed(
                    validationResult.Message))
        End Function

    End Class

End Namespace
