Public Class CurveError
    Private _row As Integer
    Private _column As Integer
    Private _Message As String
    Public ReadOnly Property GetMessage As String
        Get
            Return _Message
        End Get
    End Property
    Public ReadOnly Property Row As Integer
        Get
            Return _row
        End Get
    End Property
    Public ReadOnly Property Column As Integer
        Get
            Return _column
        End Get
    End Property
    Sub New(ByVal message As String, ByVal r As Integer, ByVal c As Integer)
        _Message = message
        _row = r
        _column = c
    End Sub
End Class
