Public Class BasicProductMomentsStats
    Private _mean As Double
    Private _SampleVariance As Double
    Private _min As Double
    Private _max As Double
    Private _N As Long
    Private _Converged As Boolean = False
    Private _ZalphaForConvergence As Double = 1.96039491692543 ' a default value for a 95% CI
    Private _ToleranceForConvergence As Double = 0.01 ' a default
    ''' <summary>
    ''' Calculates Sum, Mean, Sample Size, and Variance.  Can produce these values and Standard of Deviation
    ''' </summary>
    ''' <param name="data">an array of data records that get discarded after summary stats are produced</param>
    ''' <remarks></remarks>
    Sub New(ByVal data() As Double)
        If data.Length = 0 Then
            _mean = 0
            _SampleVariance = 0
            _min = 0
            _max = 0
            _N = 0
        Else
            AddObservations(data)
        End If
    End Sub
  ''' <summary>
  ''' An empty constructor so the user can create a "Running product momments Statistic" utilize "AddObservation" to add a record and update the statistics.
  ''' </summary>
  ''' <remarks></remarks>
  Sub New()
    _mean = 0
    _SampleVariance = 0
    _min = 0
    _max = 0
    _N = 0
  End Sub
  ''' <summary>
  ''' increments the number of records by 1 and updates the sum and sum of squares so that mean and variance can be calculated
  ''' </summary>
  ''' <param name="observation"></param>
  ''' <remarks></remarks>
  Public Sub AddObservation(ByVal observation As Double)
    'http://en.wikipedia.org/wiki/Algorithms_for_calculating_variance
    'http://planetmath.org/onepassalgorithmtocomputesamplevariance
        If _N = 0 Then
            _max = observation
            _min = observation
            _mean = observation
            _SampleVariance = 0
            _N = 1
        Else
            If observation > _max Then _max = observation
            If observation < _min Then _min = observation
            _N += 1
            Dim newmean As Double = _mean + ((observation - _mean) / _N)
            _SampleVariance = ((((_N - 2) / (_N - 1)) * _SampleVariance) + ((observation - _mean) ^ 2) / _N)
            _mean = newmean
        End If
        TestForConvergence()
  End Sub
  ''' <summary>
  ''' Loops through the array of records, increments the number of records by 1 and updates 
  ''' the sum and sum of squares so that mean and variance can be calculated
  ''' </summary>
  ''' <param name="observations"></param>
  ''' <remarks></remarks>
  Public Sub AddObservations(ByVal observations() As Double)
    'http://en.wikipedia.org/wiki/Algorithms_for_calculating_variance
    'http://planetmath.org/onepassalgorithmtocomputesamplevariance
        If _N = 0 Then
            _max = observations(0)
            _min = observations(0)
            _mean = observations(0)
            _SampleVariance = 0
            _N = 1
            For i As Integer = 1 To observations.Length - 1
                Dim observation As Double = observations(i)
                If observation > _max Then _max = observation
                If observation < _min Then _min = observation

                _N += 1
                Dim newmean As Double = _mean + ((observation - _mean) / _N)
                _SampleVariance = ((((_N - 2) / (_N - 1)) * _SampleVariance) + ((observation - _mean) ^ 2) / _N)
                _mean = newmean
            Next
        Else
            For i As Integer = 0 To observations.Length - 1
                Dim observation As Double = observations(i)
                If observation > _max Then _max = observation
                If observation < _min Then _min = observation

                _N += 1
                Dim newmean As Double = _mean + ((observation - _mean) / _N)
                _SampleVariance = ((((_N - 2) / (_N - 1)) * _SampleVariance) + ((observation - _mean) ^ 2) / _N)
                _mean = newmean
            Next
        End If
        TestForConvergence()
    End Sub
    ''' <summary>
    ''' Loops through the array of records, increments the number of records by 1 and updates 
    ''' the sum and sum of squares so that mean and variance can be calculated
    ''' </summary>
    ''' <param name="observations"></param>
    ''' <remarks></remarks>
    Public Sub AddObservations(ByVal observations() As Single)
        'http://en.wikipedia.org/wiki/Algorithms_for_calculating_variance
        'http://planetmath.org/onepassalgorithmtocomputesamplevariance
        Dim observation As Double
        Dim newmean As Double
        If _N = 0 Then
            _max = observations(0)
            _min = observations(0)
            _mean = observations(0)
            _SampleVariance = 0
            _N = 1
            For i As Integer = 1 To observations.Length - 1
                observation = observations(i)
                If observation > _max Then _max = observation
                If observation < _min Then _min = observation

                _N += 1
                newmean = _mean + ((observation - _mean) / _N)
                _SampleVariance = ((((_N - 2) / (_N - 1)) * _SampleVariance) + ((observation - _mean) ^ 2) / _N)
                _mean = newmean
            Next
        Else
            For i As Integer = 0 To observations.Length - 1
                observation = observations(i)
                If observation > _max Then _max = observation
                If observation < _min Then _min = observation

                _N += 1
                newmean = _mean + ((observation - _mean) / _N)
                _SampleVariance = ((((_N - 2) / (_N - 1)) * _SampleVariance) + ((observation - _mean) ^ 2) / _N)
                _mean = newmean
            Next
        End If
        TestForConvergence()
    End Sub
  ''' <summary>
  ''' Loops through the list of records, increments the number of records by 1 and updates 
  ''' the sum and sum of squares so that mean and variance can be calculated
  ''' </summary>
  ''' <param name="observations"></param>
  ''' <remarks></remarks>
  Public Sub AddObservations(ByVal observations As List(Of Double))
    'http://en.wikipedia.org/wiki/Algorithms_for_calculating_variance
    'http://planetmath.org/onepassalgorithmtocomputesamplevariance
        If _N = 0 Then
            _max = observations(0)
            _min = observations(0)
            _mean = observations(0)
            _SampleVariance = 0
            _N = 1
            For i As Integer = 1 To observations.Count - 1
                Dim observation As Double = observations(i)
                If observation > _max Then _max = observation
                If observation < _min Then _min = observation

                _N += 1
                Dim newmean As Double = _mean + ((observation - _mean) / _N)
                _SampleVariance = ((((_N - 2) / (_N - 1)) * _SampleVariance) + ((observation - _mean) ^ 2) / _N)
                _mean = newmean
            Next
        Else
            For i As Integer = 0 To observations.Count - 1
                Dim observation As Double = observations(i)
                If observation > _max Then _max = observation
                If observation < _min Then _min = observation

                _N += 1
                Dim newmean As Double = _mean + ((observation - _mean) / _N)
                _SampleVariance = ((((_N - 2) / (_N - 1)) * _SampleVariance) + ((observation - _mean) ^ 2) / _N)
                _mean = newmean
            Next
        End If
        TestForConvergence()
    End Sub
    Private Sub TestForConvergence()
        ' problems occur when xbar approaches zero.
        If _Converged = True Then Exit Sub
        If GetSampleSize < 100 Then Exit Sub
        Dim var As Double = (_ZalphaForConvergence * GetSampleStDev) / (GetMean * Math.Sqrt(GetSampleSize))
        If Math.Abs(var) <= _ToleranceForConvergence Then _Converged = True
    End Sub
  ''' <summary>
  ''' returns the sum of the records in the dataset
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public ReadOnly Property GetSum As Double
    Get
      Return _mean * _N
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
      Return _mean
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
      Return _SampleVariance * ((_N - 1) / _N) 'divide by n due to the single pass algorithm
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
      Return _SampleVariance
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
      Return GetSampleVariance ^ (1 / 2)
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
            Return _N
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
            Return _min
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
            Return _Max
        End Get
    End Property
    ''' <summary>
    ''' Sets the tolerance for convergence, this value is used in conjunction with the confidence interval for convergence, see SetConfidenceInterval
    ''' </summary>
    ''' <value>a number between 0 and 1, smaller numbers are more stringent, the default value is .01</value>
    ''' <remarks></remarks>
    Public WriteOnly Property SetTolerance As Double
        Set(value As Double)
            _ToleranceForConvergence = value
        End Set
    End Property
    ''' <summary>
    ''' Sets the confidence interval for convergence.  Convergence assumes normal distribution.
    ''' </summary>
    ''' <value>a centered confidence interval, this value will be used to derive an upper tail z_alpha value</value>
    ''' <remarks></remarks>
    Public WriteOnly Property SetConfidenceInterval As Double
        Set(value As Double)
            Dim n As New Normal
            _ZalphaForConvergence = n.getDistributedVariable(value + ((1 - value) / 2))
        End Set
    End Property
    Public ReadOnly Property IsConverged As Boolean
        Get
            Return _Converged
        End Get
    End Property
End Class
