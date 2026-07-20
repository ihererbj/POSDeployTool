Namespace Models

    Public Class PreDeployValidationResult

        Public Sub New()
            Errors = New List(Of String)()
        End Sub

        Public Property Errors As List(Of String)

        Public ReadOnly Property Success As Boolean
            Get
                Return Errors.Count = 0
            End Get
        End Property

        Public ReadOnly Property Message As String
            Get
                If Success Then
                    Return "Validation passed"
                End If

                Return String.Join("; ", Errors)
            End Get
        End Property

    End Class

End Namespace
