Imports POSDeployTool.Models

Namespace Contracts
    Public Interface IStoreConfigService
        Function LoadStores() As List(Of StoreInfo)
        Function LoadStores(filePath As String) As List(Of StoreInfo)
    End Interface
End Namespace
