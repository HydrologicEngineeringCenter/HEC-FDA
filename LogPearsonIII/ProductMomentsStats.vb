Public Class ProductMomentsStats
    Private _bpm As BasicProductMomentsStats
    Private _skew As Double
    Private _kurtosis As Double
    Private _median As Double
    Private _mode As Double
    ''' <summary>
    ''' Calculates Sum, Mean, Sample Size, Kurtosis, Skew, Median, Mode, Min, Max, and Variance.  Can produce these values and Standard of Deviation
    ''' </summary>
    ''' <param name="data">an array of data records, that get discarded after summary stats are produced</param>
    ''' <remarks></remarks>
    Sub New(ByVal data() As Double)
        _bpm = New BasicProductMomentsStats(data)
        Dim s As Double = GetSampleStDev
        Dim SkewSums As Double = 0
        Dim ksums As Double = 0
        For i As Int32 = 0 To CInt(_bpm.GetSampleSize) - 1
            SkewSums = SkewSums + ((data(i) - GetMean) ^ 3)
            ksums = ksums + (((data(i) - GetMean) / s) ^ 4)
        Next
        ksums = ksums * (_bpm.GetSampleSize * (_bpm.GetSampleSize + 1)) / ((_bpm.GetSampleSize - 1) * (_bpm.GetSampleSize - 2) * (_bpm.GetSampleSize - 3))
        _skew = ((_bpm.GetSampleSize) * SkewSums) / ((_bpm.GetSampleSize - 1) * (_bpm.GetSampleSize - 2) * (s ^ 3))
        _kurtosis = (ksums) - ((3 * ((_bpm.GetSampleSize - 1) ^ 2)) / ((_bpm.GetSampleSize - 2) * (_bpm.GetSampleSize - 3)))
        Array.Sort(data)
        'calculate mode
        Dim pdf As New PDF_Factory(data, 250)
        Dim v As Double = pdf.GetBins(pdf.GetPDF.IndexOf(pdf.GetPDF.Max()))
        v = v + v + pdf.GetBinWidth(pdf.GetPDF.IndexOf(pdf.GetPDF.Max()))
        _mode = v / 2 'im confused why this is right
        Dim midpoint As Double = (_bpm.GetSampleSize - 1) / 2
        If Math.Floor(midpoint) = midpoint Then
            _median = data(CInt(midpoint))
        Else
            _median = (data(CInt(Math.Floor(midpoint))) + data(CInt(Math.Ceiling(midpoint)))) / 2
        End If
    End Sub
    ''' <summary>
    ''' returns the sum of the records in the dataset
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetSum As Double
        Get
            Return _bpm.GetMean * _bpm.GetSampleSize
        End Get
    End Property
    ''' <summary>
    ''' returns the mean of the records in the dataset
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetMean As Double
        Get
            Return _bpm.GetMean
        End Get
    End Property
    ''' <summary>
    ''' returns the variance of the dataset (using N)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetVariance As Double
        Get
            Return _bpm.GetVariance
        End Get
    End Property
    ''' <summary>
    ''' returns the Mean Squared error of the dataset (using N-1)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetSampleVariance As Double
        Get
            Return _bpm.GetSampleVariance
        End Get
    End Property
    ''' <summary>
    ''' returns the standard deviation of the dataset (using N-1)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetSampleStDev As Double
        Get
            Return _bpm.GetSampleVariance ^ (1 / 2)
        End Get
    End Property
    ''' <summary>
    ''' returns the sample size of the dataset
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetSampleSize As Long
        Get
            Return _bpm.GetSampleSize
        End Get
    End Property
    ''' <summary>
    ''' returns the minimum of the dataset
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetMin As Double
        Get
            Return _bpm.GetMin
        End Get
    End Property
    ''' <summary>
    ''' returns the maxium of the dataset
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
    ''' returns the skew of the dataset (The third moment)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetSkew As Double
        Get
            Return _skew
        End Get
    End Property
    ''' <summary>
    ''' returns the kurtosis of the dataset (the fourth moment)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetKurtosis As Double
        Get
            Return _kurtosis
        End Get
    End Property
    ''' <summary>
    ''' returns the mode of the dataset (divides by 2 if it is an even number of records)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetMode As Double
        Get
            Return _mode
        End Get
    End Property
    ''' <summary>
    ''' returns the median of the dataset
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetMedian As Double
        Get
            Return _median
        End Get
    End Property
End Class
