Imports System.Linq
Imports System.Text
Imports System.Threading
Imports POSDeployTool.Contracts.Deployment
Imports POSDeployTool.Models
Imports POSDeployTool.Models.Deployment

Namespace Services.Deployment

    Public Class RemoteBackupService
        Implements IRemoteBackupService

        Private Const BackupPathMarker As String =
            "BACKUP_PATH="

        Private Const ItemCountMarker As String =
            "ITEM_COUNT="

        Private ReadOnly _commandService As IRemoteCommandService

        Public Sub New(
            commandService As IRemoteCommandService
        )
            If commandService Is Nothing Then
                Throw New ArgumentNullException(
                    NameOf(commandService))
            End If

            _commandService = commandService
        End Sub

        Public Async Function BackupAsync(
            store As StoreInfo,
            timeoutMilliseconds As Integer,
            cancellationToken As CancellationToken
        ) As Task(Of BackupResult) _
            Implements IRemoteBackupService.BackupAsync

            If store Is Nothing Then
                Throw New ArgumentNullException(NameOf(store))
            End If

            ValidatePath(store.TargetPath, "TargetPath")
            ValidatePath(store.BackupPath, "BackupPath")

            cancellationToken.ThrowIfCancellationRequested()

            Dim folderName As String =
                String.Format(
                    "{0}_{1:yyyyMMdd_HHmmss_fff}",
                    GetSafeFolderName(store.TargetPath),
                    DateTime.Now)

            Dim backupDestination As String =
                CombineWindowsPath(
                    store.BackupPath,
                    folderName)

            Dim script As String =
                BuildPowerShellScript(
                    store.TargetPath,
                    backupDestination)

            Dim encodedCommand As String =
                Convert.ToBase64String(
                    Encoding.Unicode.GetBytes(script))

            Dim commandText As String =
                "powershell.exe -NoLogo -NoProfile " &
                "-NonInteractive -ExecutionPolicy Bypass " &
                "-EncodedCommand " &
                encodedCommand

            Dim commandResult As RemoteCommandResult =
                Await _commandService.ExecuteAsync(
                    store,
                    commandText,
                    timeoutMilliseconds,
                    cancellationToken)

            Dim result As New BackupResult() With {
                .Success = commandResult.Success,
                .BackupPath = backupDestination,
                .DurationMilliseconds =
                    commandResult.DurationMilliseconds
            }

            If Not commandResult.Success Then
                result.Message =
                    "Backup failed: " &
                    commandResult.Message
                Return result
            End If

            result.BackupPath =
                ParseStringValue(
                    commandResult.StandardOutput,
                    BackupPathMarker,
                    backupDestination)

            result.ItemCount =
                ParseIntegerValue(
                    commandResult.StandardOutput,
                    ItemCountMarker)

            result.Message =
                String.Format(
                    "Backup completed: {0} item(s) to {1}",
                    result.ItemCount,
                    result.BackupPath)

            Return result
        End Function

        Private Shared Function BuildPowerShellScript(
            sourcePath As String,
            destinationPath As String
        ) As String
            Dim sourceLiteral As String =
                EscapePowerShellLiteral(sourcePath)

            Dim destinationLiteral As String =
                EscapePowerShellLiteral(destinationPath)

            Return String.Join(
                "; ",
                New String() {
                    "$ErrorActionPreference='Stop'",
                    "$source='" & sourceLiteral & "'",
                    "$destination='" &
                        destinationLiteral & "'",
                    "if(-not (Test-Path -LiteralPath " &
                        "$source -PathType Container))" &
                        "{throw ('Target path not found: ' " &
                        "+ $source)}",
                    "$items=@(Get-ChildItem " &
                        "-LiteralPath $source -Force)",
                    "if($items.Count -eq 0)" &
                        "{throw ('Target path contains no " &
                        "files: ' + $source)}",
                    "New-Item -ItemType Directory " &
                        "-Path $destination -Force " &
                        "| Out-Null",
                    "$items | Copy-Item " &
                        "-Destination $destination " &
                        "-Recurse -Force",
                    "$backupItems=@(Get-ChildItem " &
                        "-LiteralPath $destination " &
                        "-Recurse -Force)",
                    "Write-Output ('" &
                        BackupPathMarker &
                        "' + $destination)",
                    "Write-Output ('" &
                        ItemCountMarker &
                        "' + $backupItems.Count)"
                })
        End Function

        Private Shared Sub ValidatePath(
            value As String,
            propertyName As String
        )
            If String.IsNullOrWhiteSpace(value) Then
                Throw New ArgumentException(
                    propertyName & " is required.")
            End If

            If value.Length < 3 OrElse
               value(1) <> ":"c OrElse
               (value(2) <> "\"c AndAlso
                value(2) <> "/"c) Then

                Throw New ArgumentException(
                    propertyName &
                    " must be an absolute Windows path.")
            End If
        End Sub

        Private Shared Function GetSafeFolderName(
            targetPath As String
        ) As String
            Dim normalized As String =
                targetPath.TrimEnd("\"c, "/"c)

            Dim separatorIndex As Integer =
                Math.Max(
                    normalized.LastIndexOf("\"c),
                    normalized.LastIndexOf("/"c))

            Dim name As String =
                If(separatorIndex >= 0,
                   normalized.Substring(
                       separatorIndex + 1),
                   normalized)

            If String.IsNullOrWhiteSpace(name) Then
                Return "Deployment"
            End If

            For Each invalidCharacter As Char In
                IO.Path.GetInvalidFileNameChars()

                name = name.Replace(
                    invalidCharacter,
                    "_"c)
            Next

            Return name
        End Function

        Private Shared Function CombineWindowsPath(
            rootPath As String,
            childPath As String
        ) As String
            Return rootPath.TrimEnd("\"c, "/"c) &
                   "\" &
                   childPath.TrimStart("\"c, "/"c)
        End Function

        Private Shared Function EscapePowerShellLiteral(
            value As String
        ) As String
            Return value.Replace("'", "''")
        End Function

        Private Shared Function ParseStringValue(
            output As String,
            marker As String,
            fallbackValue As String
        ) As String
            For Each line As String In SplitLines(output)
                If line.StartsWith(
                    marker,
                    StringComparison.OrdinalIgnoreCase) Then

                    Return line.Substring(
                        marker.Length).Trim()
                End If
            Next

            Return fallbackValue
        End Function

        Private Shared Function ParseIntegerValue(
            output As String,
            marker As String
        ) As Integer
            Dim value As String =
                ParseStringValue(
                    output,
                    marker,
                    "0")

            Dim parsedValue As Integer

            If Integer.TryParse(
                value,
                parsedValue) Then

                Return parsedValue
            End If

            Return 0
        End Function

        Private Shared Function SplitLines(
            value As String
        ) As IEnumerable(Of String)
            If String.IsNullOrWhiteSpace(value) Then
                Return New String() {}
            End If

            Return value.Split(
                New String() {
                    vbCrLf,
                    vbCr,
                    vbLf
                },
                StringSplitOptions.RemoveEmptyEntries).
                Select(Function(line) line.Trim())
        End Function

    End Class

End Namespace
