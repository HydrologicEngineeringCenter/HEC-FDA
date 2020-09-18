''' <summary>
''' The chi squared distribution is a transformation of the normal distribution which relies only on the argument of sample size or degrees of freedom
''' </summary>
''' <remarks></remarks>
Public Class ChiSquared
    Inherits ContinuousDistribution
    Private _K As Int32 'degrees of freedom
    Private _StDev As Double 'the standard deviation of the input data
    Sub New(ByRef k As Int32)
        _StDev = CDbl(2 * k)
        _K = k
        Validate()
    End Sub
    ''' <summary>
    ''' This constructor is not intended for use, it is available for reflection purposes only
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'an empty constructor for reflection
    End Sub
    Sub New(ByVal data() As Double)
        SetParameters(data)
    End Sub
    Public Overrides Sub SetParameters(data() As Double)
        'not sure if this works or not
        _StDev = variance(data, data.Count - 1, 0) ^ 0.5
        _K = CInt(_StDev / 2)
        Validate()
    End Sub
    Public Overrides Function Validate() As String
        If _K = 0 Then Return "ChiSquared Distribution Error: Kappa cannot equal zero"
        Return Nothing
    End Function

    ''' <summary>
    ''' the degrees of freedom
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property getK As Int32
        Get
            Return _K
        End Get
    End Property
    Public Overrides Function getDistributedVariable(probability As Double) As Double
        'the chisquared distribution (in this program it is utilized to sample the standard deviation of the log pearson type 3 based on the period of record)
        Dim standardNormal As New Normal()
        Dim z As Double = standardNormal.getDistributedVariable(probability) 'standard normal z value.
        Dim result As Double = (_K) * (1 - 2 / 9 / (_K) + z * ((2 / 9 / (_K)) ^ (1 / 2))) ^ 3 'wikipedia
        Return result
    End Function
    Public Overrides Function GetCDF(value As Double) As Double
        Dim top As Double = incompletegammalower(_K / 2, value / 2)
        Dim bottom As Double = Math.Exp(gammaln(_K / 2))
        Return top / bottom
    End Function
    Public Overrides Function GetPDF(Value As Double) As Double
        If Value < 0 Then
            Return 0
        Else
            Dim top As Double = (Value ^ ((_K / 2) - 1) * Math.Exp(-Value / 2))
            Dim bottom As Double = (2 ^ (_K / 2)) * Math.Exp(gammaln(_K / 2))
            Return top / bottom
        End If
    End Function
    ''' <summary>
    ''' there is only one descriptive variable for chi squared (the degrees of freedom)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides ReadOnly Property GetNumberOfParameters As Short
        Get
            Return 1
        End Get
    End Property
    ''' <summary>
    ''' returns degrees of freedom for chi squared
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides ReadOnly Property GetCentralTendency As Double
        Get
            Return _K
        End Get
    End Property
    ''' <summary>
    ''' stdev = 2*k in a chi squared distribution
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetStDev() As Double
        Get
            Return _StDev
        End Get
    End Property
    ''' <summary>
    ''' this returns the degrees of freedom in a chi squared distribution
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides ReadOnly Property GetSampleSize() As Int32
        Get
            Return _K
        End Get
    End Property
    Public Overrides Function Clone() As ContinuousDistribution
        Dim result As New ChiSquared
        result._K = _K
        result._StDev = _StDev
        Return result
    End Function

    Public Overloads Shared Operator =(left As ChiSquared, right As ChiSquared) As Boolean
        ' Check for null arguments. Keep in mind null == null
        If left Is Nothing AndAlso right Is Nothing Then
            Return True
        ElseIf left Is Nothing Then
            Return False
        ElseIf right Is Nothing Then
            Return False
        End If

        If left._K = right._K AndAlso left._StDev = right._StDev Then
            Return True
        Else
            Return False
        End If
    End Operator
    Public Overloads Shared Operator <>(left As ChiSquared, right As ChiSquared) As Boolean
        Return Not (left = right)
    End Operator
    Public Overrides Function Equals(obj As Object) As Boolean
        If TypeOf obj Is ChiSquared Then
            Return Me = CType(obj, ChiSquared)
        Else
            Return False
        End If
    End Function
    Public Overrides Function GetHashCode() As Integer
        Return Hash(_K, _StDev)
    End Function
End Class
