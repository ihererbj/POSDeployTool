Imports System.Collections.Generic
Imports System.IO
Imports System.Web.Script.Serialization
Imports POSDeployTool.Models

Namespace Services

    Public Class StoreConfigService

        Public Function LoadStores(filePath As String) As List(Of StoreInfo)

            If String.IsNullOrWhiteSpace(filePath) Then
                Throw New ArgumentException("stores.json path is empty")
            End If

            If Not File.Exists(filePath) Then
                Throw New FileNotFoundException("stores.json not found", filePath)
            End If

            Dim json As String =
                File.ReadAllText(filePath, System.Text.Encoding.UTF8)

            Dim serializer As New JavaScriptSerializer()

            Dim stores As List(Of StoreInfo) =
                serializer.Deserialize(Of List(Of StoreInfo))(json)

            If stores Is Nothing Then
                stores = New List(Of StoreInfo)()
            End If

            For Each store As StoreInfo In stores

                If store Is Nothing Then Continue For

                store.StoreCode = SafeTrim(store.StoreCode)
                store.StoreName = SafeTrim(store.StoreName)
                store.IpAddress = SafeTrim(store.IpAddress)
                store.Username = SafeTrim(store.Username)

                If store.Password Is Nothing Then
                    store.Password = String.Empty
                End If

                If String.IsNullOrWhiteSpace(store.TargetPath) Then
                    store.TargetPath = "C:\BJCBCPOS"
                End If

                If String.IsNullOrWhiteSpace(store.BackupPath) Then
                    store.BackupPath = "C:\Backup"
                End If

                store.PingStatus = "Not checked"
                store.WinRmStatus = "Not checked"
                store.Status = "Not checked"
                store.LastChecked = String.Empty

            Next

            Return stores

        End Function

        Private Shared Function SafeTrim(value As String) As String

            If value Is Nothing Then
                Return String.Empty
            End If

            Return value.Trim()

        End Function

    End Class

End Namespace