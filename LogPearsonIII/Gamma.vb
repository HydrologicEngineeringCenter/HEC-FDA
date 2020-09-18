Public Class Gamma
    Inherits ContinuousDistribution
    Private _a As Double
    Private _b As Double
    Private _sampleSize As Integer
    Sub New()
        'empty for reflection
    End Sub
    Sub New(ByVal a As Double, ByVal b As Double)
        _a = a
        _b = b
    End Sub
    Sub New(ByVal data() As Double)
        SetParameters(data)
    End Sub
    Public Overrides Sub SetParameters(data() As Double)
        Dim stats As New BasicProductMomentsStats(data)
        'http://www.itl.nist.gov/div898/handbook/eda/section3/eda366b.htm
        _a = (stats.GetMean / stats.GetSampleStDev) ^ 2
        _b = 1 / (stats.GetSampleVariance / stats.GetMean)
        Validate()
        _samplesize = data.Count
    End Sub
    Public ReadOnly Property GetAlpha As Double
        Get
            Return _a
        End Get
    End Property
    Public ReadOnly Property GetBeta As Double
        Get
            Return _b
        End Get
    End Property
    Sub New(ByVal a As Double, ByVal b As Double, ByVal samplesize As Integer)
        _a = a
        _b = b 'should i convert this to 1/b?
        Validate()
        _samplesize = samplesize
    End Sub
    Public Overrides Function Validate() As String
        If _b = 0 Then Return "Gamma Distribution Error: The parameter Beta cannot be equal to zero"
        Return Nothing
    End Function
    Public Overrides Function Clone() As ContinuousDistribution
        Return New Gamma(_a, _b, _sampleSize)
    End Function
    Public Overrides Function GetCDF(value As Double) As Double
        'Dim top As Double = IncompleteGamma(_a, _b * value)
        'Dim bottom As Double = Math.Exp(gammaln(_a))
        'Return top / bottom
        Return regIncompleteGamma(_a, value / _b)
    End Function
    Public Overrides ReadOnly Property GetCentralTendency As Double
        Get
            Return _a / _b
        End Get
    End Property
    Public Overrides Function getDistributedVariable(probability As Double) As Double
        Dim xn As Double
        If _a - 1 < 0.00001 Then
            xn = 0.00001 * _b
            If probability <= GetCDF(xn) Then Return 0
        Else
            xn = (_a - 1) * _b
        End If
        Dim testvalue As Double = GetCDF(xn)
        Dim n As Integer = 0
        Do  'random definition of epsilon
            xn = xn - ((testvalue - probability) / GetPDF(xn))
            testvalue = GetCDF(xn)
            n += 1
        Loop Until (Math.Abs(testvalue - probability) <= 0.000000000000001 Or n = 100)
        Return xn
    End Function

    Public Overrides ReadOnly Property GetNumberOfParameters As Short
        Get
            Return 2
        End Get
    End Property

    Public Overrides Function GetPDF(Value As Double) As Double
        Dim d As Double = Math.Exp(-Value / _b + (_a - 1) * Math.Log(Value) - _a * Math.Log(_b) - gammaln(_a))
        Return d '(((_b ^ _a) * ((Value ^ (_a - 1)) * Math.Exp(-_b * Value))) / Math.Exp(gammaln(_a)))
    End Function

    Public Overrides ReadOnly Property GetSampleSize As Integer
        Get
            Return _sampleSize
        End Get
    End Property
End Class
