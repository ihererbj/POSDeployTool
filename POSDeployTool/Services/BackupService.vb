Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks
Imports POSDeployTool.Models

Namespace Services

    Public Class BackupService

        Private ReadOnly _remoteCommandService As New RemoteCommandService()

        Public Async Function BackupAsync(
            store As StoreInfo,
            backupDate As Date,
            timeoutMilliseconds As Integer,
            cancellationToken As CancellationToken
        ) As Task(Of CheckResult)

            If store Is Nothing Then Return CheckResult.Fail("Store is null")

            Dim sourcePath As String = If(store.TargetPath, String.Empty).Trim()
            If String.IsNullOrWhiteSpace(sourcePath) Then sourcePath = "C:\BJCBCPOS"

            Dim validationError As String = ValidateWindowsPath(sourcePath)
            If validationError IsNot Nothing Then Return CheckResult.Fail(validationError)

            Dim backupPath As String = sourcePath.TrimEnd("\"c) & "_" & backupDate.ToString("yyyyMMdd")
            Dim powerShellScript As String = BuildPowerShellScript(sourcePath, backupPath)
            Dim remoteCommand As String = "powershell.exe -NoProfile -NonInteractive -ExecutionPolicy Bypass -Command " & QuoteArgument(powerShellScript)

            Dim result As CheckResult = Await _remoteCommandService.ExecuteAsync(
                store,
                remoteCommand,
                timeoutMilliseconds,
                cancellationToken
            )

            If result.IsSuccess Then
                Return CheckResult.Success(String.Format("{0} -> {1}", sourcePath, backupPath))
            End If

            Return result
        End Function

        Private Shared Function BuildPowerShellScript(sourcePath As String, backupPath As String) As String
            Dim src As String = EscapePowerShellSingleQuoted(sourcePath)
            Dim dst As String = EscapePowerShellSingleQuoted(backupPath)

            Return String.Join(";", New String() {
                "$ErrorActionPreference='Stop'",
                "$src='" & src & "'",
                "$dst='" & dst & "'",
                "if(-not (Test-Path -LiteralPath $src -PathType Container)){Write-Error 'Source folder not found';exit 10}",
                "if(Test-Path -LiteralPath $dst){Write-Output ('BACKUP_EXISTS|'+$dst);exit 0}",
                "Copy-Item -LiteralPath $src -Destination $dst -Recurse -Force",
                "$sourceCount=@(Get-ChildItem -LiteralPath $src -Recurse -Force | Where-Object {-not $_.PSIsContainer}).Count",
                "$backupCount=@(Get-ChildItem -LiteralPath $dst -Recurse -Force | Where-Object {-not $_.PSIsContainer}).Count",
                "if($sourceCount -ne $backupCount){Write-Error ('File count mismatch. Source='+$sourceCount+' Backup='+$backupCount);exit 12}",
                "Write-Output ('BACKUP_OK|'+$dst+'|Files='+$backupCount)"
            })
        End Function

        Private Shared Function ValidateWindowsPath(value As String) As String
            If Not Path.IsPathRooted(value) Then Return "TargetPath must be an absolute Windows path"
            If value.IndexOfAny(New Char() {"&"c, "|"c, "<"c, ">"c, ChrW(34)}) >= 0 Then
                Return "TargetPath contains unsupported command characters"
            End If
            Return Nothing
        End Function

        Private Shared Function EscapePowerShellSingleQuoted(value As String) As String
            Return value.Replace("'", "''")
        End Function

        Private Shared Function QuoteArgument(value As String) As String
            If value Is Nothing Then value = String.Empty
            If value.Contains(ChrW(34)) Then
                Throw New ArgumentException("Remote command cannot contain a double quote character")
            End If
            Return ChrW(34) & value & ChrW(34)
        End Function

    End Class

End Namespace
