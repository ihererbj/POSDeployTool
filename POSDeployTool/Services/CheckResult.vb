Namespace Services
    Public Class CheckResult
        Public Property IsSuccess As Boolean
        Public Property Message As String

        Public Shared Function Success(message As String) As CheckResult
            Return New CheckResult With {.IsSuccess = True, .Message = message}
        End Function

        Public Shared Function Fail(message As String) As CheckResult
            Return New CheckResult With {.IsSuccess = False, .Message = message}
        End Function
    End Class
End Namespace
