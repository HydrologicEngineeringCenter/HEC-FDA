''' <summary>
''' The Log Pearson type III distribution is utilized for extream value statistics, particularly in USACE flow frequency calculations.  It uses mean standard diviation and skew to describe the randomly distributed variates. 
''' </summary>
''' <remarks>do not provide negative values in the data argument for constructing this class.</remarks>
Public Class LogPearsonIII
    Inherits ContinuousDistribution
    Private _mean As Double 'the mean of the input data
    Private _StDev As Double 'the standard deviation of the input data
    Private _sampleSize As Int32 'the number of records in the input data
    Private _G As Double
    ''' <summary>
    ''' This expects data in linear space to be provided, it is then transformed to logbase 10 space, and summarized into Mean, Stdev, and G(skew)
    ''' </summary>
    ''' <param name="data"></param>
    ''' <remarks></remarks>
    Sub New(ByVal data() As Double)
        SetParameters(data)
    End Sub
    ''' <summary>
    ''' The user defined lp3 allows the user to create an LP3 distribution by describing the fit parameters
    ''' </summary>
    ''' <param name="mean">The mean of the log base 10 of the flow data</param>
    ''' <param name="stdev">The standard deviation of the log base 10 of the flow data</param>
    ''' <param name="g">The skew of the log base 10 of the flow data</param>
    ''' <param name="samplesize">The number of records in the data base used to create the mean standard deviation and skew, or the equivalent length of record. this is utilized for uncertianty about the curve with the non central t distribution.</param>
    ''' <remarks></remarks>
    Sub New(ByVal mean As Double, ByVal stdev As Double, ByVal g As Double, ByVal samplesize As Int32)
        _mean = mean
        _StDev = stdev
        _sampleSize = samplesize
        _G = g 'use the input skew
    End Sub
    ''' <summary>
    ''' This constructor is not intended for use, it is available for reflection purposes only
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'an empty constructor for reflection
    End Sub
    Public Overrides Sub SetParameters(data() As Double)
        For i = 0 To data.Count - 1
            data(i) = Math.Log(data(i), 10)
        Next
        Dim s As New ProductMomentsStats(data)
        _sampleSize = CInt(s.GetSampleSize)
        _mean = s.GetMean
        _StDev = s.GetSampleStDev
        _G = s.GetSkew
    End Sub
    Public Overrides Function Validate() As String
        Return Nothing    'can the mean be negative?
    End Function

    ''' <summary>
    ''' This is a method to sample a new Log Pearson type III Distribution from the original fitted distribution.  The mean is sampled using the normal distribution and the standard deviation is sampled using the ChiSquared distribution.  Skew is unchanged.
    ''' </summary>
    ''' <param name="meanRV">a random number between zero and 1 exclusive, for sampling the mean</param>
    ''' <param name="SigmaRV">a random number between zero and 1 exclusive, for sampling the standard deviation.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetRealizationLP3(ByVal meanRV As Double, ByVal SigmaRV As Double) As LogPearsonIII
        Dim mean As Double
        Dim stdev As Double
        Dim g As Double = _G
        Dim sampleNormal As New Normal(_mean, (_StDev / (Math.Sqrt(_sampleSize))))
        mean = sampleNormal.getDistributedVariable(meanRV)
        Dim samplechisquare As New ChiSquared(_sampleSize - 1)
        Dim denominator As Double = samplechisquare.getDistributedVariable(SigmaRV)
        stdev = (((_sampleSize - 1) * (_StDev ^ 2)) / denominator) ^ (1 / 2)
        Dim outputLP3 As New LogPearsonIII(mean, stdev, g, _sampleSize)
        Return outputLP3
    End Function
    ''' <summary>
    ''' returns a value that is distributed with a log pearson type III distribution.
    ''' </summary>
    ''' <param name="probability">a random number between zero and 1 exclusive, this represents natural variability alone.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function getDistributedVariable(ByVal probability As Double) As Double
        Dim result As Double = 0
        Dim StandardNormal As New Normal()
        Dim z As Double = StandardNormal.getDistributedVariable(probability)
        Dim K As Double = (2 / _G) * (((z - _G / 6) * _G / 6 + 1) ^ 3 - 1) 'k value from the bulletin 17b document
        Dim logflow As Double = _mean + (K * _StDev) 'the log flow for the log pearson type 3 distribution
        result = logflow
        Return 10 ^ result
    End Function
    Public Function getDistributedVariableWithUncertainty(ByVal probability As Double, ByVal uncertaintyprobability As Double) As Double
        Dim result As Double = 0
        Dim StandardNormal As New Normal()
        Dim zn As Double = StandardNormal.getDistributedVariable(probability)
        Dim zk As Double = StandardNormal.getDistributedVariable(uncertaintyprobability)
        Dim K As Double = (2 / _G) * (((zn - _G / 6) * _G / 6 + 1) ^ 3 - 1) 'k value from the bulletin 17b document
        Dim Avalue As Double = (1 - ((zn ^ 2) / (2 * (GetSampleSize - 1))))
        Dim bvalue As Double = (K ^ 2) - ((zn ^ 2) / (GetSampleSize))
        Dim rootvalue As Double = ((K ^ 2) - (Avalue * bvalue)) ^ (1 / 2)
        If uncertaintyprobability > 0.5 Then
            'upper
            result = _mean + (((K + rootvalue) / Avalue) * _StDev)
        Else
            'lower
            result = _mean + (((K - rootvalue) / Avalue) * _StDev)
        End If
        Return 10 ^ result
    End Function
    Public Overrides Function GetCDF(value As Double) As Double
        If value <= 0 Then Return 0
        Dim d As New PearsonIII(_mean, _StDev, _G)
        Return d.GetCDF(Math.Log10(value))
    End Function
    Public Overrides Function GetPDF(Value As Double) As Double
        Dim d As New PearsonIII(_mean, _StDev, _G)
        Return d.GetPDF(Math.Log10(Value)) / Value / Math.Log(10.0)
    End Function
    Public Overrides ReadOnly Property GetNumberOfParameters As Short
        Get
            Return 3
        End Get
    End Property
    Public ReadOnly Property GetG As Double
        Get
            Return _G
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
    Public Overrides ReadOnly Property GetCentralTendency As Double
        Get
            Return _mean
        End Get
    End Property
    Public Overrides ReadOnly Property GetSampleSize() As Int32
        Get
            Return _sampleSize
        End Get
    End Property
    Public Overrides Function Clone() As ContinuousDistribution
        Dim result As New LogPearsonIII
        result._mean = _mean
        result._StDev = _StDev
        result._G = _G
        result._sampleSize = _sampleSize
        Return result
    End Function

    Public Overloads Shared Operator =(left As LogPearsonIII, right As LogPearsonIII) As Boolean
        ' Check for null arguments. Keep in mind null == null
        If left Is Nothing AndAlso right Is Nothing Then
            Return True
        ElseIf left Is Nothing Then
            Return False
        ElseIf right Is Nothing Then
            Return False
        End If

        If left._mean = right._mean AndAlso left._StDev = right._StDev AndAlso left._G = right._G Then
            Return True
        Else
            Return False
        End If
    End Operator
    Public Overloads Shared Operator <>(left As LogPearsonIII, right As LogPearsonIII) As Boolean
        Return Not (left = right)
    End Operator
    Public Overrides Function Equals(obj As Object) As Boolean
        If TypeOf obj Is LogPearsonIII Then
            Return Me = CType(obj, LogPearsonIII)
        Else
            Return False
        End If
    End Function
    Public Overrides Function GetHashCode() As Integer
        Return Hash(_mean, _StDev, _G)
    End Function
End Class
