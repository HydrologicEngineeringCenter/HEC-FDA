Public Class ShiftedGamma
    Inherits Gamma
    Private _Shift As Double
    Sub New(ByVal a As Double, ByVal b As Double, ByVal shift As Double)
        MyBase.New(a, b)
        _Shift = shift
    End Sub
    Public Overrides Function GetCDF(value As Double) As Double
        Return MyBase.GetCDF(value - _Shift)
    End Function
    Public Overrides Function getDistributedVariable(probability As Double) As Double
        Return MyBase.getDistributedVariable(probability) + _Shift
    End Function
    Public Overrides ReadOnly Property GetNumberOfParameters As Short
        Get
            Return 3
        End Get
    End Property
    Public Overrides Function GetPDF(Value As Double) As Double
        Return MyBase.GetPDF(Value - _Shift)
    End Function
    'Public Overrides Sub SetParameters(data() As Double)
    '    Dim p As New ProductMomentsStats(data)
    '    'MyBase = New Gamma(4.0 / (p.GetSkew() * p.GetSkew()), 0.5 * p.GetSampleStDev() * p.GetSkew())
    'End Sub
End Class
