Imports System.Diagnostics


''' <summary>
''' The normal distribution is a child of the continuous distribution class which allows the user to fit, and test goodness of fit, and then generate normally distributed variables based on the input data.
''' </summary>
''' <remarks></remarks>
Public Class Normal '<DebuggerDisplay("Mean = {math.round(GetMean)}")> _
    Inherits ContinuousDistribution
    Private _mean As Double 'the mean of the input data
    Private _StDev As Double 'the standard deviation of the input data
    Private _sampleSize As Int32 'the number of records in the input data
    ''' <summary>
    ''' This takes the data, calculates a mean, standard deviation, and number of records and fits a normal distribution
    ''' </summary>
    ''' <param name="data"></param>
    ''' <remarks></remarks>
    Sub New(ByRef data As Double())
        SetParameters(data)
    End Sub
    ''' <summary>
    ''' This creates a standard normal distribution, mean of zero, standard deviation of 1
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        _sampleSize = 1
        _mean = 0
        _StDev = 1
        'Validate()
    End Sub
    ''' <summary>
    ''' User defined normal distribution. Creates a normal distribution with the defined mean and standard deviation
    ''' </summary>
    ''' <param name="mean"></param>
    ''' <param name="stdev"></param>
    ''' <remarks>Generally it is bad form to utilize parametric bootstraps with a user defined distribution</remarks>
    Sub New(ByVal mean As Double, ByVal stdev As Double)
        _mean = mean
        _StDev = stdev
        _sampleSize = 0
        'Validate()
    End Sub
    Public Overrides Sub SetParameters(data() As Double)
        Dim s As New BasicProductMomentsStats(data)
        _sampleSize = CInt(s.GetSampleSize)
        _mean = s.GetMean
        _StDev = s.GetSampleStDev
        'Validate()
    End Sub
    Public Overrides Function Validate() As String
        If _StDev < 0 Then Return "Normal Distribution Error: The Standard Deviation was less than 0"
        Return Nothing
    End Function

    Public Overrides Function GetCDF(value As Double) As Double
        If value = _mean Then Return 0.5
        Dim dist As Double = value - _mean
        Dim a As Double = 0.5
        Dim stdevs As Double = Math.Floor(Math.Abs(dist / _StDev))
        Dim inc As Double = 1 / 250 'this parameter controls the accuracy(and speed) of the computation
        Dim a1 As Double = 0.682689492137 / 2
        Dim a2 As Double = 0.954499736104 / 2
        Dim a3 As Double = 0.997300203937 / 2
        Dim a4 As Double = 0.999936657516 / 2
        Dim a5 As Double = 0.999999426687 / 2
        Dim a6 As Double = 0.999999998027 / 2
        Dim a7 As Double = (0.5 - a6) / 2

        Select Case stdevs
            Case 0
                If dist < 0 Then a += -a1
                Return FindArea(a, inc, value)
            Case 1

                If dist < 0 Then
                    a += -a2
                Else
                    a += a1
                End If
                Return FindArea(a, inc, value)
            Case 2
                If dist < 0 Then
                    a += -a3
                Else
                    a += a2
                End If

                Return FindArea(a, inc, value)
            Case 3
                If dist < 0 Then
                    a += -a4
                Else
                    a += a3
                End If
                Return FindArea(a, inc, value)
            Case 4
                If dist < 0 Then
                    a += -a5
                Else
                    a += a4
                End If
                Return FindArea(a, inc, value)
            Case 5
                If dist < 0 Then
                    a += -a6
                Else
                    a += a5
                End If
                Return FindArea(a, inc, value)
            Case 6
                If dist < 0 Then
                    a += -a7
                Else
                    a += a6
                End If
                Return FindArea(a, inc, value)
            Case Else
                If dist < 0 Then
                    Return 0
                Else
                    Return 1
                End If
        End Select
    End Function
    Private Function Trapazoidalintegration(ByVal y1 As Double, ByVal y2 As Double, ByVal deltax As Double) As Double
        Dim deltay As Double = 0
        Dim rect As Double = 0
        If y1 > y2 Then
            deltay = y1 - y2
            rect = Math.Abs(y2 * deltax)
        Else
            deltay = y2 - y1
            rect = Math.Abs(y1 * deltax)
        End If
        Dim tri As Double = (1 / 2) * (deltax * deltay)
        Return rect + Math.Abs(tri)
    End Function
    Private Function FindArea(ByVal a As Double, ByVal inc As Double, ByVal x As Double) As Double
        Dim x1 As Double = getDistributedVariable(a)
        Dim x2 As Double = getDistributedVariable(a + inc)
        Do Until x2 >= x
            x1 = x2
            a += inc
            x2 = getDistributedVariable(a + inc)
        Loop
        Dim y1 As Double = GetPDF(x1)
        Dim deltax As Double = Math.Abs(x1 - x2)
        Dim y2 As Double = GetPDF(x2)
        Dim area As Double = Trapazoidalintegration(y1, y2, deltax)
        Dim interpvalue As Double = (x - x1) / (x2 - x1)
        a += area * interpvalue
        Return a
    End Function
    Public Overrides Function GetPDF(Value As Double) As Double
        Return (1 / Math.Sqrt(2 * Math.PI * (_StDev ^ 2))) * Math.Exp((-(Value - _mean) ^ (2)) / (2 * (_StDev ^ 2)))
    End Function
    ''' <summary>
    ''' This function returns a normally distributed variable from the distribution type you have created based on the probability provided
    ''' </summary>
    ''' <param name="probability">A number between zero and 1 not including either zero or 1</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function getDistributedVariable(ByVal probability As Double) As Double
        Dim i As Long
        Dim x As Double
        Dim p As Double
        Dim c0 As Double, c1 As Double, c2 As Double
        Dim d1 As Double, d2 As Double, d3 As Double
        Dim t As Double
        Dim q As Double
        q = probability
        If (q = 0.5) Then
            Return _mean
        End If
        If q <= 0 Then
            q = 0.000000000000001
        End If
        If q >= 1 Then
            q = 0.999999999999999
        End If
        If q < 0.5 Then
            p = q
            i = -1
        Else
            p = 1 - q
            i = 1
        End If
        t = Math.Sqrt(Math.Log(1 / (p * p)))
        c0 = 2.515517
        c1 = 0.802853
        c2 = 0.010328
        d1 = 1.432788
        d2 = 0.189269
        d3 = 0.001308
        x = t - (c0 + c1 * t + c2 * (t ^ 2)) / (1 + d1 * t + d2 * (t ^ 2) + d3 * (t ^ 3))
        x = i * x
        Return (x * _StDev) + _mean
    End Function
    Public Overrides ReadOnly Property GetNumberOfParameters As Short
        Get
            Return 2
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
        Dim result As New Normal
        result._mean = _mean
        result._StDev = _StDev
        result._sampleSize = _sampleSize
        Return result
    End Function

    ' NOTE: Not comparing _sampleSize
    Public Overloads Shared Operator =(left As Normal, right As Normal) As Boolean
        ' Check for null arguments. Keep in mind null == null
        If left Is Nothing AndAlso right Is Nothing Then
            Return True
        ElseIf left Is Nothing Then
            Return False
        ElseIf right Is Nothing Then
            Return False
        End If

        If left._mean = right._mean AndAlso left._StDev = right._StDev Then
            Return True
        Else
            Return False
        End If
    End Operator
    Public Overloads Shared Operator <>(left As Normal, right As Normal) As Boolean
        Return Not (left = right)
    End Operator
    Public Overrides Function Equals(obj As Object) As Boolean
        If TypeOf obj Is Normal Then
            Return Me = CType(obj, Normal)
        Else
            Return False
        End If
    End Function
    Public Overrides Function GetHashCode() As Integer
        Return Hash(_mean, _StDev)
    End Function
End Class
