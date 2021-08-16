''' <summary>
''' This is a class that creates PDFs and CDFs from an array of data
''' </summary>
''' <remarks></remarks>
Public Class PDF_Factory
    Private _PDF As List(Of Double)
    Private _CDF As List(Of Double)
    Private _bins As List(Of Double)
    Private _Min As Double
    Private _Max As Double
    Private _maxvalue As Double
    ''' <summary>
    ''' This constructor allows the user to specify the number of bins, and the range of the bins.  Each bin is of uniform width.
    ''' </summary>
    ''' <param name="data">the data that is to be turned into a cdf or pdf</param>
    ''' <param name="numbins">the number of bins desired</param>
    ''' <param name="max">the max value for the last bin</param>
    ''' <param name="min">the minimum value for the first bin</param>
    ''' <remarks></remarks>
    Sub New(ByRef data() As Double, ByVal numbins As Int32, ByRef max As Double, ByRef min As Double)
        _maxvalue = data.Max
        _Max = max
        _Min = min
        createbins(numbins)
        createpdfandcdf(data)
    End Sub
    ''' <summary>
    ''' This constructor allows the user to specify the number of bins, but uses the range of the data for the range of the bins, each bin is uniform width
    ''' </summary>
    ''' <param name="data">the data that is to be turned into a cdf or a pdf</param>
    ''' <param name="numbins">the number of bins desired</param>
    ''' <remarks></remarks>
    Sub New(ByVal data() As Double, ByVal numbins As Int32)
        _maxvalue = data.Max
        _Max = data.Max
        _Min = data.Min
        createbins(numbins)
        createpdfandcdf(data)
    End Sub
    ''' <summary>
    ''' this constructor allows the user to specify the number and size of each bin, each bin can be varying in size.
    ''' </summary>
    ''' <param name="data">the data desired to be converted into a pdf or cdf</param>
    ''' <param name="bins">the bins as an array</param>
    ''' <remarks></remarks>
    Sub New(ByVal data() As Double, ByVal bins() As Double)
        _Max = bins(0)
        _Min = bins(bins.Count - 1)
        _bins = bins.ToList
        createpdfandcdf(data)
    End Sub
    Private Sub createbins(ByVal numbins As Int32)
        Dim bins(numbins - 1) As Double
        Dim alpha As Double = (_Max - _Min) / (numbins - 1)
        bins(0) = _Max
        For i = 1 To numbins - 2
            bins(i) = bins(i - 1) - alpha
        Next
        bins(numbins - 1) = _Min
        _bins = bins.ToList
    End Sub
    Private Sub createpdfandcdf(ByVal data() As Double)
        Dim pdfint(_bins.Count - 1) As Int64
        Dim cdfint(_bins.Count - 1) As Int64
        Dim pdf As New List(Of Double)
        Dim cdf As New List(Of Double)
        Dim sumdatapoints As Long = data.Count
        For i = 0 To data.Count - 1 'determine the count for bin 0
            If data(i) >= _bins(0) Then cdfint(0) = cdfint(0) + 1
        Next
        pdfint(0) = cdfint(0)
        sumdatapoints = sumdatapoints - pdfint(0)
        For j = 1 To _bins.Count - 1
            For i = 0 To data.Count - 1 'determine the count for each bin
                If data(i) >= _bins(j) Then cdfint(j) = cdfint(j) + 1
            Next
            pdfint(j) = cdfint(j) - cdfint(j - 1)
            sumdatapoints = sumdatapoints - pdfint(j)
        Next
        For i = 0 To _bins.Count - 1
            pdf.Add(pdfint(i) / data.Count)
            cdf.Add(cdfint(i) / data.Count)
        Next
        _CDF = cdf
        _PDF = pdf
    End Sub
    Public ReadOnly Property GetMin As Double
        Get
            Return _Min
        End Get
    End Property
    Public ReadOnly Property GetMax As Double
        Get
            Return _Max
        End Get
    End Property
    Public ReadOnly Property GetDelta As Double
        Get
            Return _Max - _Min
        End Get
    End Property
    Public ReadOnly Property GetBinWidth(ByVal index As Integer) As Double
        Get
            If index = 0 Then
                Return _maxvalue - _Max
            Else
                Return _bins(index - 1) - _bins(index)
            End If
        End Get
    End Property
    Public ReadOnly Property GetPDF As List(Of Double)
        Get
            Return _PDF
        End Get
    End Property
    Public ReadOnly Property GetCDF As List(Of Double)
        Get
            Return _CDF
        End Get
    End Property
    Public ReadOnly Property GetBins As List(Of Double)
        Get
            Return _bins
        End Get
    End Property
End Class
