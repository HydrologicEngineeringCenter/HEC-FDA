Public Class ErrorReport
    Private _errors As List(Of CurveError)
    Public ReadOnly Property Errors As List(Of CurveError)
        Get
            Return _errors
        End Get
    End Property
    Sub New()
        _errors = New List(Of CurveError)
    End Sub
    Sub New(ByVal err As CurveError)
        _errors = New List(Of CurveError)
        _errors.Add(err)
    End Sub
    Sub AddError(ByVal err As CurveError)
        _errors.Add(err)
    End Sub
End Class
