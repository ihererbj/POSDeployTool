Imports System.IO
Imports Newtonsoft.Json
Imports POSDeployTool.Models

Namespace Services

    Public Class StoreConfigService

        Public Function LoadStores() As List(Of StoreInfo)
            Return LoadStores(AppPaths.StoreConfigFile)
        End Function

        Public Function LoadStores(ByVal filePath As String) As List(Of StoreInfo)

            If String.IsNullOrWhiteSpace(filePath) Then
                Throw New ArgumentException(
                    "Store configuration file path is required.",
                    NameOf(filePath))
            End If

            If Not File.Exists(filePath) Then
                Throw New FileNotFoundException(
                    "Store configuration file was not found.",
                    filePath)
            End If

            Try
                Dim json As String = File.ReadAllText(filePath)

                If String.IsNullOrWhiteSpace(json) Then
                    Return New List(Of StoreInfo)()
                End If

                Dim stores As List(Of StoreInfo) =
                    JsonConvert.DeserializeObject(Of List(Of StoreInfo))(json)

                If stores Is Nothing Then
                    Return New List(Of StoreInfo)()
                End If

                Return stores

            Catch ex As JsonException
                Throw New InvalidDataException(
                    "The stores.json file contains invalid JSON.",
                    ex)
            End Try

        End Function

    End Class

End Namespace