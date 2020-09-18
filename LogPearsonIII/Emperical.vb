Public Class Emperical
    Private _sorteddata As List(Of Double)
    Private _bpm As BasicProductMomentsStats
    Private _median As Double
    Private _a As Double = 0
    Private _b As Double = 0
    ''' <summary>
    ''' This takes a summary dataset sorts it and passes it back to the user in a varaity of ways.
    ''' </summary>
    ''' <param name="data">a list of double</param>
    ''' <remarks></remarks>
    Sub New(ByRef data As List(Of Double))
        data.Sort()
        _sorteddata = data 'ascending
        If data.Count > 0 Then
            _bpm = New BasicProductMomentsStats(_sorteddata.ToArray)
            Dim midpoint As Double = (_bpm.GetSampleSize - 1) / 2
            If Math.Floor(midpoint) = midpoint Then
                _median = _sorteddata(CInt(midpoint))
            Else
                _median = (_sorteddata(CInt(Math.Floor(midpoint))) + _sorteddata(CInt(Math.Ceiling(midpoint)))) / 2
            End If
        Else
            _bpm = New BasicProductMomentsStats()
        End If
    End Sub
    ''' <summary>
    ''' returns the data sorted
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetData As List(Of Double)
        Get
            Return _sorteddata
        End Get
    End Property
    ''' <summary>
    ''' total number of records in the dataset
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetSampleSize As Int32
        Get
            Return _sorteddata.Count
        End Get
    End Property
    ''' <summary>
    ''' the minimum of the dataset
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetMin As Double
        Get
            Return _bpm.GetMin
        End Get
    End Property
    Public ReadOnly Property GetBasicProductMoments As BasicProductMomentsStats
        Get
            Return _bpm
        End Get
    End Property
    ''' <summary>
    ''' the median value of the dataset
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetMedian As Double
        Get
            Return _median
        End Get
    End Property
    ''' <summary>
    ''' this is the A parameter in the plotting position equation.  the default is zero which is consistent with Weibull plotting position.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property A As Double
        Set(value As Double)
            _a = value
        End Set
        Get
            Return _a
        End Get
    End Property
    ''' <summary>
    ''' this is the B parameter in the plotting position equation.  the default is zero which is consistent with Weibull plotting position.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property B As Double
        Set(value As Double)
            _b = value
        End Set
        Get
            Return _b
        End Get
    End Property
    ''' <summary>
    ''' the maximum of the dataset
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetMax As Double
        Get
            Return _bpm.GetMax
        End Get
    End Property
    ''' <summary>
    ''' returns the exceedance probability that which corresponds with this value
    ''' </summary>
    ''' <param name="value">a value you wish to find the exceedance probability of</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCDF(ByRef value As Double) As Double
        Dim n As Integer = _sorteddata.Count - 1
        Dim incriment As Double = 1 / n
        If value >= _sorteddata(n) Then Return 0
        If value <= _sorteddata(0) Then Return 1
        Dim count As Integer = n
        Do Until _sorteddata(count) < value
            count -= 1
        Loop
        Dim lower As Double = _sorteddata(count)
        Dim upper As Double = _sorteddata(count + 1)
        Dim factor As Double = (value - lower) / (upper - lower)
        'check if it rounds up or down
        Return (count - 1) * incriment + (incriment * factor)
    End Function
    ''' <summary>
    ''' returns a value for a given exceedance probability
    ''' </summary>
    ''' <param name="percentile">the exceedance probability you are interested in</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPercentile(ByVal percentile As Double) As Double
        Dim n As Integer = _sorteddata.Count - 1
        Dim incriment As Double = 1 / n
        If percentile < incriment Then Return _sorteddata(0)
        If percentile > 1 - incriment Then Return _sorteddata(n)
        Dim i As Integer = CInt(Math.Floor(percentile / incriment)) 'the index of the value larger than requested
        Dim larger As Double = _sorteddata(i)
        Dim smaller As Double = _sorteddata(i + 1)
        Dim d As Double = (percentile / incriment) - i
        Dim factor As Double = (larger - smaller) * d
        Return larger - factor
    End Function
    ''' <summary>
    ''' calculates the plotting position for each data record in the dataset.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function PlottingPosition() As List(Of Double)
        Dim positions As New List(Of Double)
        Dim n As Long = _sorteddata.Count
        For i = 0 To _sorteddata.Count - 1
            positions.Add(((i + 1) - _a) / (n - 1 - _a - _b))
        Next
        Return positions
    End Function
End Class
