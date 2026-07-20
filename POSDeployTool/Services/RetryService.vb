Imports System
Imports System.Threading
Imports System.Threading.Tasks

Namespace Services
    Public Class RetryService
        Public Async Function ExecuteAsync(
            operationName As String,
            operation As Func(Of Task(Of CheckResult)),
            maxAttempts As Integer,
            delayMilliseconds As Integer,
            cancellationToken As CancellationToken,
            Optional onRetry As Action(Of Integer, String) = Nothing
        ) As Task(Of CheckResult)

            If operation Is Nothing Then Throw New ArgumentNullException(NameOf(operation))
            If maxAttempts < 1 Then maxAttempts = 1

            Dim lastResult As CheckResult = Nothing

            For attempt As Integer = 1 To maxAttempts
                cancellationToken.ThrowIfCancellationRequested()

                Try
                    lastResult = Await operation()
                Catch ex As OperationCanceledException
                    Throw
                Catch ex As Exception
                    lastResult = CheckResult.Fail(ex.Message)
                End Try

                If lastResult IsNot Nothing AndAlso lastResult.IsSuccess Then
                    Return lastResult
                End If

                If attempt < maxAttempts Then
                    Dim message As String = If(lastResult Is Nothing, "No result", lastResult.Message)
                    If onRetry IsNot Nothing Then onRetry(attempt + 1, operationName & ": " & message)
                    Await Task.Delay(delayMilliseconds, cancellationToken)
                End If
            Next

            If lastResult Is Nothing Then
                Return CheckResult.Fail(operationName & " failed without a result")
            End If

            Return CheckResult.Fail(String.Format("{0} failed after {1} attempts: {2}", operationName, maxAttempts, lastResult.Message))
        End Function
    End Class
End Namespace
