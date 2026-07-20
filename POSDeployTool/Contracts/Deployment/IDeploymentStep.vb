Imports System.Threading
Imports POSDeployTool.Models.Deployment

Namespace Contracts.Deployment

    Public Interface IDeploymentStep

        ReadOnly Property Name As String
        ReadOnly Property Order As Integer

        Function ExecuteAsync(
            context As DeploymentContext,
            cancellationToken As CancellationToken
        ) As Task(Of DeploymentStepResult)

    End Interface

End Namespace
