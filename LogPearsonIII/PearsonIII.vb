Public Class PearsonIII
    Inherits ContinuousDistribution
    Protected _Min_Stdev As Double = 0.00001
    Protected _BND_Skew As Double = 0.00001
    Private _Mean As Double
    Private _Stdev As Double
    Private _Skew As Double
    Private _sampleSize As Integer
    Public Sub New(ByVal mean As Double, ByVal stdev As Double, ByVal skew As Double)
        _Mean = mean
        _Stdev = stdev
        _Skew = skew
    End Sub
    Public Sub New(ByVal data() As Double)
        SetParameters(data)
    End Sub
    Public ReadOnly Property GetG As Double
        Get
            Return _Skew
        End Get
    End Property

    Public ReadOnly Property GetStDev() As Double
        Get
            Return _StDev
        End Get
    End Property
    Public ReadOnly Property GetMean() As Double
        Get
            Return _mean
        End Get
    End Property
    Public Overrides Function Clone() As ContinuousDistribution
        Return New PearsonIII(_Mean, _Stdev, _Skew)
    End Function
    Public Overrides Function GetCDF(value As Double) As Double
        If Math.Abs(_Skew) < _BND_Skew Then
            Return getZeroEquivalentDist.GetCDF(value)
        ElseIf _Skew > 0 Then
            Return getPosEquivalentDist.GetCDF(value)
        Else
            Return 1 - getNegEquivalentDist.GetCDF(-value)
            'TODO: Check my super sketch fix for when Regular Gamma returns Double.NAN because x is negative.
            'I return 1 only because this seems to happen where p is approaching 1.
            'Dim val = getNegEquivalentDist.GetCDF(-value)
            'If Double.IsNaN(val) Then
            '    Return 1
            'Else
            '    Return 1 - val
            'End If
        End If
    End Function
    Public Overrides ReadOnly Property GetCentralTendency As Double
        Get
            Return _Mean
        End Get
    End Property
    Public Overrides Function getDistributedVariable(probability As Double) As Double
        If Math.Abs(_Skew) < _BND_Skew Then
            Return getZeroEquivalentDist.getDistributedVariable(probability)
        ElseIf _Skew > 0 Then
            Return getPosEquivalentDist.getDistributedVariable(probability)
        Else
            Return -getNegEquivalentDist.getDistributedVariable(1 - probability)
        End If
    End Function
    Public Overrides ReadOnly Property GetNumberOfParameters As Short
        Get
            Return 3
        End Get
    End Property

    Public Overrides Function GetPDF(Value As Double) As Double
        If Math.Abs(_Skew) < _BND_Skew Then
            Return getZeroEquivalentDist.GetPDF(Value)
        ElseIf _Skew > 0 Then
            Return getPosEquivalentDist.GetPDF(Value)
        Else
            Return -getNegEquivalentDist.GetPDF(Value)
        End If
    End Function
    Public Overrides ReadOnly Property GetSampleSize As Integer
        Get
            Return _sampleSize
        End Get
    End Property
    Public Overrides Sub SetParameters(data() As Double)
        Dim p As New ProductMomentsStats(data)
        _sampleSize = CInt(p.GetSampleSize)
        If p.GetSampleStDev < _Min_Stdev Then
            _Mean = Double.NaN
            _Stdev = Double.NaN
            _Skew = Double.NaN
        Else
            _Mean = p.GetMean
            _Stdev = p.GetSampleStDev
            _Skew = p.GetSkew
        End If
    End Sub
    Public Overrides Function Validate() As String
        Dim s As New System.Text.StringBuilder
        If _Mean = Double.NaN Then s.AppendLine("Mean is not defined")
        If _Stdev = Double.NaN Then s.AppendLine("Stdev is not defined")
        If _Skew = Double.NaN Then s.AppendLine("Skew is not defined")
        Return s.ToString
    End Function
    Private Function getZeroEquivalentDist() As ContinuousDistribution
        Return New Normal(_Mean, _Stdev)
    End Function
    Private Function getPosEquivalentDist() As ContinuousDistribution
        Return New ShiftedGamma(4.0 / (_Skew * _Skew), 0.5 * (_Stdev * _Skew), _Mean - 2.0 * _Stdev / _Skew)
    End Function
    Private Function getNegEquivalentDist() As ContinuousDistribution
        Return New ShiftedGamma(4.0 / (_Skew * _Skew), -0.5 * (_Stdev * _Skew), -_Mean + 2.0 * _Stdev / _Skew)
    End Function
End Class
