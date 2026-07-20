Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports Newtonsoft.Json
Imports POSDeployTool.Models

Namespace Services
    Public Class DeploymentHistoryService
        Private ReadOnly _syncRoot As New Object()

        Public Sub Append(record As DeploymentRecord)
            If record Is Nothing Then Return

            Dim logDirectory As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs")
            Directory.CreateDirectory(logDirectory)

            Dim datePart As String = record.StartedAt.ToString("yyyyMMdd", CultureInfo.InvariantCulture)
            Dim csvPath As String = Path.Combine(logDirectory, "Deploy_" & datePart & ".csv")
            Dim jsonPath As String = Path.Combine(logDirectory, "Deploy_" & datePart & ".json")

            SyncLock _syncRoot
                AppendCsv(csvPath, record)
                AppendJson(jsonPath, record)
            End SyncLock
        End Sub

        Private Shared Sub AppendCsv(path As String, record As DeploymentRecord)
            Dim isNew As Boolean = Not File.Exists(path)
            Using writer As New StreamWriter(path, True, New UTF8Encoding(False))
                If isNew Then
                    writer.WriteLine("StartedAt,FinishedAt,StoreCode,StoreName,IPAddress,ComputerName,OldVersion,ReleaseVersion,Result,Message,ElapsedSeconds,Operator")
                End If

                writer.WriteLine(String.Join(",", New String() {
                    Csv(record.StartedAt.ToString("yyyy-MM-dd HH:mm:ss")),
                    Csv(record.FinishedAt.ToString("yyyy-MM-dd HH:mm:ss")),
                    Csv(record.StoreCode),
                    Csv(record.StoreName),
                    Csv(record.IpAddress),
                    Csv(record.ComputerName),
                    Csv(record.OldVersion),
                    Csv(record.ReleaseVersion),
                    Csv(record.Result),
                    Csv(record.Message),
                    record.ElapsedSeconds.ToString("0.00", CultureInfo.InvariantCulture),
                    Csv(record.OperatorName)
                }))
            End Using
        End Sub

        Private Shared Sub AppendJson(path As String, record As DeploymentRecord)
            Dim records As List(Of DeploymentRecord)
            If File.Exists(path) Then
                Try
                    records = JsonConvert.DeserializeObject(Of List(Of DeploymentRecord))(File.ReadAllText(path, Encoding.UTF8))
                Catch
                    records = New List(Of DeploymentRecord)()
                End Try
            Else
                records = New List(Of DeploymentRecord)()
            End If

            If records Is Nothing Then records = New List(Of DeploymentRecord)()
            records.Add(record)

            Dim tempPath As String = path & ".tmp"
            File.WriteAllText(tempPath, JsonConvert.SerializeObject(records, Formatting.Indented), New UTF8Encoding(False))
            If File.Exists(path) Then File.Delete(path)
            File.Move(tempPath, path)
        End Sub

        Private Shared Function Csv(value As String) As String
            Dim safe As String = If(value, String.Empty).Replace(ChrW(34), ChrW(34) & ChrW(34))
            Return """" & safe & """"
        End Function
    End Class
End Namespace
