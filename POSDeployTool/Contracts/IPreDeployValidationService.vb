Imports POSDeployTool.Models

Namespace Contracts

    Public Interface IPreDeployValidationService

        Function Validate(store As StoreInfo) As PreDeployValidationResult

    End Interface

End Namespace
