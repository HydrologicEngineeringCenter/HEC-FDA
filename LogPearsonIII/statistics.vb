Public Module statistics
    Private Const SQTPI As Double = 2.5066282746310007
    Private Const LOGPI As Double = 1.1447298858494002
    Private Const MACHEP As Double = 0.00000000000000011102230246251565
    Private Const MAXLOG As Double = 709.782712893384
    Private Const MINLOG As Double = -745.13321910194122
    Private Const MAXGAM As Double = 171.62437695630271
    ''' <summary>
    ''' This function sums an array from the start value (as index) to the end value (as index)
    ''' </summary>
    ''' <param name="arr">the array with double data</param>
    ''' <param name="endval">the record index you wish to end the sum for</param>
    ''' <param name="startval">the record index you wish to begin the sum for</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Sum(ByVal arr() As Double, ByVal endval As Int32, ByVal startval As Int32) As Double
        'a function to calculate the sum of an array
        Dim totvalue As Double
        totvalue = 0
        For i As Int32 = startval To endval
            totvalue = totvalue + arr(i)
        Next
        Sum = totvalue
    End Function
    ''' <summary>
    ''' this function calculates the mean of the array from the start value (as an index) to the end value (as an index)
    ''' </summary>
    ''' <param name="arr">the array of data() as double</param>
    ''' <param name="endval">the record index you wish to end the sum for</param>
    ''' <param name="startval">the record index you wish to begin the sum for</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function Mean(ByVal arr() As Double, ByVal endval As Int32, ByVal startval As Int32) As Double
        'a function to calculate the mean of an array (the first moment)
        Dim temp As Double
        Dim number As Long
        number = endval - startval
        temp = Sum(arr, endval, startval)
        Mean = temp / (number + 1) 'check this math...
    End Function
    ''' <summary>
    ''' This function calculates the variance of an array from the start value (as an index) to the end value (as an index)
    ''' </summary>
    ''' <param name="arr">the array as a double</param>
    ''' <param name="endval">the record index you wish to end the sum for</param>
    ''' <param name="startval">the record index you wish to begin the sum for</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function variance(ByVal arr() As Double, ByVal endval As Int32, ByVal startval As Int32) As Double
        'a function to calculate the variance of an array. (the second moment)
        Dim temp As Double
        Dim meanvalue As Double
        Dim number As Int32
        number = endval - startval
        meanvalue = Mean(arr, endval, startval)
        temp = 0
        For i As Int32 = startval To number
            temp = temp + ((arr(i) - meanvalue) ^ 2)
        Next
        variance = temp / (number)
    End Function
    ''' <summary>
    ''' This function calculates the skew of an array from the start value (as an index) to the end value (as an index)
    ''' </summary>
    ''' <param name="arr">the array as a double</param>
    ''' <param name="endval">the record index you wish to end the sum for</param>
    ''' <param name="startval">the record index you wish to begin the sum for</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function skew(ByVal arr() As Double, ByVal endval As Int32, ByVal startval As Int32) As Double
        'a function to calculate skew (the third moment)
        Dim temp As Double
        Dim meanvalue As Double
        Dim sigma As Double
        Dim number As Int32
        number = (endval - startval)
        meanvalue = Mean(arr, endval, startval)
        sigma = variance(arr, endval, startval) ^ (1 / 2)
        temp = 0
        For i As Int32 = startval To number
            temp = temp + ((arr(i) - meanvalue) ^ 3)
        Next
        skew = ((number + 1) * temp) / ((number) * (number - 1) * (sigma ^ 3))
    End Function
    Function Kurtosis(ByVal arr() As Double, ByVal endval As Int32, ByVal startval As Int32) As Double
        Dim s As Double = variance(arr, endval, startval) ^ (1 / 2)
        Dim means As Double = Mean(arr, endval, startval)
        Dim temp As Double
        Dim number As Int32
        number = (endval - startval)
        temp = 0
        For i As Int32 = startval To number
            temp = temp + (((arr(i) - means) / s) ^ 4)
        Next
        number += 1
        temp = temp * (number * number + 1) / ((number - 1) * (number - 2) * number - 3)
        Return (temp) - ((3 * ((number - 1) ^ 2)) / ((number - 2) * (number - 3)))
    End Function
    ''' <summary>
    ''' This function calculates the covariance of two arrays, using the shortest of the two arrays as the range of comparison
    ''' </summary>
    ''' <param name="arr1">the first array as a double</param>
    ''' <param name="arr2">the second array as a double</param>
    ''' <returns></returns>
    ''' <remarks>if you hand this function the same array twice, it will calculate variance.</remarks>
    Function Covariance(ByVal arr1() As Double, ByVal arr2() As Double) As Single
        'a function to calculate the covariance of two arrays that are not necesarrily the same length
        Dim temp As Double
        Dim meanvalue1 As Double
        Dim meanvalue2 As Double
        Dim number As Int32
        If arr1.Length <= arr2.Length Then
            number = arr1.Length
        Else
            number = arr2.Length
        End If
        'If UBound(arr1) <= UBound(arr2) Then
        '    number = UBound(arr1)
        'Else
        '    number = UBound(arr2)
        'End If
        meanvalue1 = Mean(arr1, number, 0)
        meanvalue2 = Mean(arr2, number, 0)
        For i = 0 To number
            temp = temp + ((arr1(i) - meanvalue1) * (arr2(i) - meanvalue2))
        Next
        Return CSng(temp / number)
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="arr1"></param>
    ''' <param name="arr2"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function Correlation(ByVal arr1() As Double, ByVal arr2() As Double) As Single
        Dim cov As Single = Covariance(arr1, arr2)
        Dim number As Int32
        If arr1.Length <= arr2.Length Then
            number = arr1.Length
        Else
            number = arr2.Length
        End If
        'If UBound(arr1) <= UBound(arr2) Then
        '    number = UBound(arr1)
        'Else
        '    number = UBound(arr2)
        'End If
        Dim s1 As Double = variance(arr1, number, 0)
        Dim s2 As Double = variance(arr2, number, 0)
        Return CSng(cov / (s1 * s2))
    End Function
    ''' <summary>
    ''' This function calculates the covariance of two arrays, using the shortest of the two arrays as the range of comparison
    ''' </summary>
    ''' <param name="arr1">the first array as a double</param>
    ''' <param name="arr2">the second array as a double</param>
    ''' <param name="number">The number of records you wish to truncate the sample to</param>
    ''' <returns></returns>
    ''' <remarks>if you hand this function the same array twice, it will calculate variance.</remarks>
    Function Covariance(ByVal arr1() As Double, ByVal arr2() As Double, ByVal number As Int32) As Single
        'a function to calculate the covariance of two arrays that are not necesarrily the same length
        Dim temp As Double
        Dim meanvalue1 As Double
        Dim meanvalue2 As Double
        meanvalue1 = Mean(arr1, number, 0)
        meanvalue2 = Mean(arr2, number, 0)
        For i As Int32 = 0 To number
            temp = temp + ((arr1(i) - meanvalue1) * (arr2(i) - meanvalue2))
        Next
        Return CSng(temp / number)
    End Function
    Function median(ByVal data() As Double) As Double
        Array.Sort(data)
        Dim midpoint As Double = (data.Count - 1) / 2
        If Math.Round(midpoint) = midpoint Then
            Return data(CInt(midpoint))
        Else
            Return (data(CInt(Math.Round(midpoint))) + data(CInt(Math.Round(midpoint) + 1))) / 2
        End If
    End Function
    ''' <summary>
    ''' This function evaluates the mode of a dataset.
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function Mode(ByVal data() As Double) As Double
        'check for duplicates
        'Dim list = data.GroupBy(Function(x) x).[Select](Function(x) New With {Key .Count = x.Count(), Key .Value = x.Key}) _
        '              .GroupBy(Function(x) x.Count) _
        '              .OrderByDescending(Function(x) x.Key) _
        '              .First() _
        '              .ToList()
        'If list.Count > 0 Then
        '    Return list(0)
        'Else
        'create a pdf and find the largest count.
        Dim pdf As New PDF_Factory(data, 250)
        Dim v As Double = pdf.GetBins(pdf.GetPDF.IndexOf(pdf.GetPDF.Max()))
        v = v + v + pdf.GetBinWidth(pdf.GetPDF.IndexOf(pdf.GetPDF.Max()))
        Return v / 2
        'End If
    End Function
    ''' <summary>
    ''' This function creates a covariance matrix from a series of lists of double
    ''' </summary>
    ''' <param name="data">the data (the lists of the actual data do not have to be the same lengths</param>
    ''' <returns>this function returns an NxN matrix with the variances along the diagonal where N is the number of arrays in the data</returns>
    ''' <remarks></remarks>
    Function Covariance_Matrix(ByVal data As List(Of List(Of Double))) As Double(,)
        Dim numarrays As Integer = data.Count - 1
        Dim shortestarray As Integer = Integer.MaxValue
        For i = 0 To numarrays
            If data(i).Count - 1 < shortestarray Then shortestarray = data(i).Count - 1
        Next
        Dim result(numarrays, numarrays) As Double
        For i = 0 To numarrays
            Dim row As New List(Of Double)
            Dim temp1() As Double
            temp1 = data(i).ToArray
            For j = 0 To numarrays
                Dim temp2() As Double
                temp2 = data(j).ToArray
                result(i, j) = Covariance(temp1, temp2, shortestarray)
            Next
        Next
        Return result
    End Function
    ''' <summary>
    ''' This function creates a correlation matrix from a series of lists of double
    ''' </summary>
    ''' <param name="data">the data (the lists of the actual data do not have to be the same lengths, but the data is trimmed from the end...</param>
    ''' <returns>this function returns an NxN matrix with 1's along the diagonal, and correlations in both corners where N is the number of arrays in the data</returns>
    ''' <remarks></remarks>
    Function Correlation_Matrix(ByVal data As List(Of List(Of Double))) As Double(,)
        Dim numarrays As Integer = data.Count - 1
        Dim shortestarray As Integer = Integer.MaxValue
        For i = 0 To numarrays
            If data(i).Count - 1 < shortestarray Then shortestarray = data(i).Count - 1
        Next
        Dim result(numarrays, numarrays) As Double
        For i = 0 To numarrays
            Dim row As New List(Of Double)
            Dim temp1() As Double

            temp1 = data(i).ToArray
            Dim sigma1 As Double = Math.Sqrt(variance(temp1, shortestarray, 0))
            For j = 0 To numarrays
                Dim temp2() As Double
                temp2 = data(j).ToArray
                Dim sigma2 As Double = Math.Sqrt(variance(temp2, shortestarray, 0))
                result(i, j) = Covariance(temp1, temp2, shortestarray) / (sigma1 * sigma2)
            Next
        Next
        Return result
    End Function
    ''' <summary>
    ''' Creates a uniformly distributed number that matches the input random number
    ''' </summary>
    ''' <param name="InputRandom">the uniform random number you wish to have a correlated random number created for</param>
    ''' <param name="ErrorRandom">the uniform error random number that will define how far away the correlated number is from the input</param>
    ''' <param name="correlation">the control on how correlated the two random numbers are.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function CorrelatedUniforms(ByRef InputRandom As Double, ByVal ErrorRandom As Double, ByRef correlation As Double) As Double
        Dim z As New Normal
        Dim z1 = z.getDistributedVariable(InputRandom)
        Dim z2 = z.getDistributedVariable(ErrorRandom)
        Return z.GetCDF((correlation * z1) + (Math.Sqrt(1 - correlation ^ 2)) * z2)
    End Function
    ''' <summary>
    ''' A factorial function
    ''' </summary>
    ''' <param name="N">the number you wish to factoral (N!)</param>
    ''' <param name="lowerbound">a divisor, utilized for the choose function, evaluates (N!/lowerbound!)</param>
    ''' <returns>an integer that is huge!</returns>
    ''' <remarks></remarks>
    Function factorial(ByVal N As UInt64, Optional ByVal lowerbound As UInt64 = 1) As UInt64
        Dim value As UInt64 = 1
        If N <= 1 Then Return 1
        If lowerbound + 1 >= N Then Return N
        For i As UInt64 = CULng(lowerbound + 1) To N
            value = CULng(value * i)
        Next
        Return value
    End Function
    ''' <summary>
    ''' The choose function, which is valuable for probabilty evaluations
    ''' </summary>
    ''' <param name="n">the number of options</param>
    ''' <param name="k">the number of selections from the number of options</param>
    ''' <returns>a value that represents the number of ways k items can be pulled from n number of items regarless of order </returns>
    ''' <remarks></remarks>
    Function Choose(ByVal n As UInt64, ByVal k As UInt64) As Int64
        Return CLng(factorial(n, k) / factorial(n - k, 1))
    End Function
    Function binomial(ByVal probability As Double, ByVal n As UInt64, ByVal k As UInt64) As Double
        Dim value As Double = 0
        Dim i As UInt64
        For i = 0 To k
            value = value + Choose(n, i) * (probability ^ i) * ((1 - probability) ^ (n - i))
        Next
        Return value + 1
    End Function
    Function binomialInv(ByVal n As Int32, ByVal probability As Double, ByVal criteria As Double) As Double
        Dim value(n) As Double
        Dim i As Int32
        For i = 0 To n
            value(i) = binomial(probability, CULng(n), CULng(i))
            If value(i) > criteria Then
                Exit For
            End If
        Next
        Return i
    End Function
    Public Function gamma(ByVal x As Double) As Double
        Dim p As Double() = {0.00016011952247675185, 0.0011913514700658638, 0.010421379756176158, 0.047636780045713721, 0.20744822764843598, 0.49421482680149709, 1.0}
        Dim q As Double() = {-0.000023158187332412014, 0.00053960558049330335, -0.0044564191385179728, 0.011813978522206043, 0.035823639860549865, -0.23459179571824335, 0.0714304917030273, 1.0}
        Dim lp As Double
        Dim lq As Double = Math.Abs(x)
        Dim lz As Double
        If (lq > 33.0) Then
            If (x < 0.0) Then
                lp = Math.Floor(lq)
                If (lp = lq) Then Throw New ArithmeticException("gamma: overflow")
                '//int i = (int)p;
                lz = lq - lp
            If (lz > 0.5) Then lp += 1.0 : lz = lq - lp
            lz = lq * Math.Sin(Math.PI * lz)
            If (lz = 0.0) Then Throw New ArithmeticException("gamma: overflow")
                lz = Math.Abs(lz)
                lz = Math.PI / (lz * stirf(lq))
                Return -lz
            Else
                Return stirf(x)
            End If
        End If
        lz = 1.0
        While (x >= 3.0)
            x -= 1.0
            lz *= x
        End While
        While (x < 0.0)
            If (x = 0.0) Then
                Throw New ArithmeticException("gamma: singular")
            ElseIf (x > -0.000000001) Then
                Return (lz / ((1.0 + 0.57721566490153287 * x) * x))
            End If
            lz /= x
            x += 1.0
        End While

        While (x < 2.0)
            If (x = 0.0) Then
                Throw New ArithmeticException("gamma: singular")
            ElseIf (x < 0.000000001) Then
                Return (lz / ((1.0 + 0.57721566490153287 * x) * x))
            End If
            lz /= x
            x += 1.0
        End While
        If ((x = 2.0) Or (x = 3.0)) Then Return lz
        x -= 2.0
        lp = polevl(x, p, 6)
        lq = polevl(x, q, 7)
        Return lz * lp / lq
    End Function
    Private Function stirf(ByVal x As Double) As Double
        Dim stir As Double() = {0.00078731139579309368, -0.00022954996161337813, -0.0026813261780578124, 0.0034722222160545866, 0.08333333333334822}
        Dim MAXSTIR As Double = 143.01608
        Dim w As Double = 1.0 / x
        Dim y As Double = Math.Exp(x)
        w = 1.0 + w * polevl(w, stir, 4)
        If (x > MAXSTIR) Then
            '* Avoid overflow in Math.Pow() *
            Dim v As Double = Math.Pow(x, 0.5 * x - 0.25)
            y = v * (v / y)
        Else
            y = Math.Pow(x, x - 0.5) / y
        End If
        y = SQTPI * y * w
        Return y
    End Function
    ''' <summary>
    ''' Evaluates a polynomial of n degrees
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="coef"></param>
    ''' <param name="N"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function polevl(ByVal x As Double, ByVal coef As Double(), ByVal N As Integer) As Double
        Dim ans As Double
        ans = coef(0)
        For i = 1 To N
            ans = ans * x + coef(i)
        Next
        Return ans
    End Function
    ''' <summary>
    ''' Evaluates a polynomial of n degrees assumgin coef(N) =1
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="coef"></param>
    ''' <param name="N"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function p1evl(ByVal x As Double, ByVal coef As Double(), ByVal N As Integer) As Double
        Dim ans As Double
        ans = x + coef(0)
        For i = 1 To N - 1
            ans = ans * x + coef(i)
        Next
        Return ans
    End Function
#Region "Ben's RegIncompleteGamma"
    Public Function regIncompleteGamma(ByVal t As Double, ByVal x As Double) As Double
        Return regularizedGammaP(t, x, 0.00000000000001, Integer.MaxValue)
    End Function
    Public Function regularizedGammaP(ByVal a As Double, ByVal x As Double, ByVal epsilon As Double, ByVal maxiterations As Integer) As Double
        If Double.IsNaN(a) Or Double.IsNaN(x) Or a <= 0 Or x < 0 Then
            Return Double.NaN
        ElseIf x = 0 Then
            Return 0
        ElseIf x >= a + 1 Then
            Return 1 - regularizedGammaQ(a, x, epsilon, maxiterations)
        Else
            Dim n As Double = 0
            Dim an As Double = 1 / a
            Dim sum As Double = an
            While Math.Abs(an / sum) > epsilon And n < maxiterations And sum < Double.PositiveInfinity
                n += 1
                an *= x / (a + n)
                sum += an
            End While
            If n > maxiterations Then
                ''exceeded iterations, this is untrustworthy answer...
                Return 0
            ElseIf Double.IsInfinity(sum) Then
                Return 1
            Else
                Return Math.Exp(-x + (a * Math.Log(x)) - gammaln(a)) * sum
            End If
        End If
    End Function
    Public Function regularizedGammaQ(ByVal a As Double, ByVal x As Double, ByVal epsilon As Double, ByVal maxiterations As Integer) As Double
        If Double.IsNaN(a) Or Double.IsNaN(x) Or a <= 0 Or x < 0 Then
            Return Double.NaN
        ElseIf x = 0 Then
            Return 1
        ElseIf x < a + 1 Then
            Return 1 - regularizedGammaP(a, x, epsilon, maxiterations)
        Else
            Dim ret As Double = 0
            ret = 1 / evaluateCFGammaQ(a, x, epsilon, maxiterations)
            Return Math.Exp(-x + (a * Math.Log(x)) - gammaln(a)) * ret
        End If
    End Function
    Public Function evaluateCFGammaQ(ByVal a As Double, ByVal x As Double, ByVal epsilon As Double, ByVal maxiterations As Integer) As Double
        Dim small As Double = 1.0E-50
        Dim hprev As Double = ((2 * 0) + 1) - a + x
        If Math.Abs(hprev - 0) < small Then hprev = small
        Dim n As Integer = 1
        Dim dprev As Double = 0
        Dim cprev As Double = hprev
        Dim hn As Double = hprev
        While n < maxiterations
            Dim aa As Double = ((2 * n) + 1) - a + x
            Dim bb As Double = n * (a - n)
            Dim dn As Double = aa + bb * dprev
            If Math.Abs(dn - 0) < small Then dn = small
            Dim cn As Double = aa + bb / cprev
            If Math.Abs(cn - 0) < small Then cn = small
            dn = 1 / dn
            Dim deltan As Double = cn * dn
            hn = hprev * deltan
            If Double.IsInfinity(hn) Then
                Return Double.PositiveInfinity
                ''not good?
            End If
            If Double.IsNaN(hn) Then
                ''not good?
                Return Double.NaN
            End If
            If Math.Abs(deltan - 1) < epsilon Then Exit While
            dprev = dn
            cprev = cn
            hprev = hn
            n += 1
        End While
        If n >= maxiterations Then
            ''not good?
        End If
        Return hn
    End Function
#End Region

    Function gammaln(ByVal x As Double) As Double
        'If x >= 1 Then
        '    Return x * Math.Log(x) - x - (0.5 * Math.Log(x / (2 * Math.PI))) + (1 / (12 * x)) - (1 / (360 * (x ^ 3))) + (1 / (1260 * (x ^ 5)))

        'Else
        Dim c() As Double = {76.180091729471457, -86.505320329416776, 24.014098240830911, -1.231739572450155, 0.001208650973866179, -0.000005395239384953}
        Dim sqrt2Pi As Double = Math.Sqrt(Math.PI * 2)
        Dim tmp As Double = x + 5.5
        tmp = (x + 0.5) * Math.Log(tmp) - tmp
        Dim err As Double = 1.0000000001900149 'first coefficient and epsilon
        For i = 0 To 5
            err += c(i) / (x + i + 1)
        Next
        Return tmp + Math.Log(sqrt2Pi * err / x)
        'End If
    End Function
    Public Function BetaFunction(ByVal a As Double, ByVal b As Double) As Double
        Return Math.Exp(gammaln(a) + gammaln(b) - gammaln(a + b))
    End Function
    Public Function IncompleteGammaComplement(ByVal a As Double, ByVal x As Double) As Double
        Dim big As Double = 4503599627370496.0
        Dim biginv As Double = 0.00000000000000022204460409250313
        Dim ans, ax, c, yc, r, t, y, z As Double
        Dim pk, pkm1, pkm2, qk, qkm1, qkm2 As Double
        If (x <= 0 Or a <= 0) Then Return 1
        If (x < 1 Or x < a) Then Return IncompleteGamma(a, x)
        ax = a * Math.Log(x) - x - gammaln(a)
        If ax < -MAXLOG Then Return 0
        ax = Math.Exp(ax)
        y = 1 - a
        z = x + y + 1
        c = 0
        pkm2 = 1
        qkm2 = x
        pkm1 = x + 1
        qkm1 = z * x
        ans = pkm1 / qkm1
        Do
            c += 1
            y += 1
            z += 2
            yc = y * c
            pk = pkm1 * z - pkm2 * yc
            qk = qkm1 * z - qkm2 * yc
            If qk <> 0 Then
                r = pk / qk
                t = Math.Abs((ans - r) / r)
            Else
                t = 1
            End If
            pkm2 = pkm1
            pkm1 = pk
            qkm2 = qkm1
            qkm1 = qk
            If Math.Abs(pk) > big Then
                pkm2 *= biginv
                pkm1 *= biginv
                qkm2 *= biginv
                qkm1 *= biginv
            End If
        Loop While t > MACHEP
        Return ans * ax
    End Function
    Public Function IncompleteGamma(ByVal a As Double, ByVal x As Double) As Double
        Dim ans, ax, c, r As Double
        If x <= 0 Or a <= 0 Then Return 0
        If x > 1 And x > a Then Return 1 - IncompleteGammaComplement(a, x)
        ax = a * Math.Log(x) - x - gammaln(a)
        If ax < -MAXLOG Then Return 0
        ax = Math.Exp(ax)
        r = a
        c = 1
        Do
            r += 1
            c *= x / r
            ans += c
        Loop While c / ans > MACHEP
        Return ans * ax / a
    End Function
    Public Function incompletegammalower(ByVal a As Double, ByVal x As Double) As Double
        If x < 0 Then Throw New ArithmeticException("in the lower incomplete gamma function x cannot be less than zero")
        If x = 0 Then Return 0
        Dim sum As Double = 0
        Dim term As Double = 1.0 / a
        Dim n As Integer = 1
        While (term <> 0 And n <= 10000)
            sum = sum + term
            term = term * (x / (a + n))
            n += 1
        End While
        Return Math.Pow(x, a) * Math.Exp(-1 * x) * sum
    End Function
    Public Function incompleteGammaUpper(ByVal a As Double, ByVal x As Double) As Double
        If x < 0 Then Throw New ArithmeticException("in the upper incomplete gamma function x cannot be less than zero")
        If x = 0 Then Return Math.Exp(gammaln(a))
        Const T_0 = 0, T_1 = 1, T_2 = 2, T_3 = 3
        Dim A_prev As Double = T_0
        Dim B_prev As Double = T_1
        Dim A_cur As Double = Math.Pow(x, a) / Math.Exp(x)
        Dim B_cur As Double = x - a + T_1
        Dim avar As Double = a - T_1
        Dim b As Double = B_cur + T_2
        Dim n As Double = a - T_3
        Dim maxit As Integer = 10000
        Dim it As Integer = 0
        While True
            Dim A_next As Double = b * A_cur + avar * A_prev
            Dim B_next As Double = b * B_cur + avar * B_prev
            If (A_next * B_cur = A_cur * B_next) Or it = maxit Then Return A_cur / B_cur
            A_prev = A_cur
            A_cur = A_next
            B_prev = B_cur
            B_cur = B_next
            avar = avar + n
            b = b + T_2
            n = n - T_2
            it += 1
        End While
        Throw New ArithmeticException("This result should never occur in the lower incomplete gamma function")
    End Function
    Public Function mutualprob(ByVal probs As List(Of Double)) As Double
        'http://lethalman.blogspot.com/2011/08/probability-of-union-of-independent.html

        Dim ret As Double = 0
        For i = 0 To probs.Count - 1
            ret += probs(i) * (1 - ret)
        Next
        Return ret
    End Function
    'Public Function RegularizedIncompleteBetaFunction(ByVal a As Double, ByVal b As Double, ByVal x As Double) As Double
    '    Return IncompleteBetaFunction(a, b, x) / BetaFunction(a, b)
    'End Function
    Public Function RegularizedIncompleteBetaFunction(ByVal aa As Double, ByVal bb As Double, ByVal xx As Double) As Double
        Dim a, b, t, x, xc, w, y As Double
        Dim flag As Boolean
        If (aa <= 0.0 Or bb <= 0.0) Then Throw New ArithmeticException("Incomplete Beta Function argument a was less than zero or b was less than zero")
        If ((xx <= 0.0) Or (xx >= 1.0)) Then
            If (xx = 0.0) Then Return 0.0
            If (xx = 1.0) Then Return 1.0
            Throw New ArithmeticException("Incomplete Beta Function x was greater than 1 or less than zero")
        End If
        flag = False
        If ((bb * xx) <= 1.0 And xx <= 0.95) Then
            t = pseries(aa, bb, xx)
            Return t
        End If
        w = 1.0 - xx
        If (xx > (aa / (aa + bb))) Then
            flag = True
            a = bb
            b = aa
            xc = xx
            x = w
        Else
            a = aa
            b = bb
            xc = w
            x = xx
        End If
        If (flag AndAlso (b * x) <= 1.0 AndAlso x <= 0.95) Then
            t = pseries(a, b, x)
            If (t <= MACHEP) Then
                t = 1.0 - MACHEP
            Else : t = 1.0 - t
            End If
            Return t
        End If
        y = x * (a + b - 2.0) - (a - 1.0)
        If (y < 0.0) Then
            w = incbcf(a, b, x)
        Else
            w = incbd(a, b, x) / xc
        End If
        y = a * Math.Log(x)
        t = b * Math.Log(xc)
        If ((a + b) < MAXGAM AndAlso Math.Abs(y) < MAXLOG AndAlso Math.Abs(t) < MAXLOG) Then
            t = Math.Pow(xc, b)
            t *= Math.Pow(x, a)
            t /= a
            t *= w
            t *= gamma(a + b) / (gamma(a) * gamma(b))
            If (flag) Then
                If (t <= MACHEP) Then
                    t = 1.0 - MACHEP
                Else
                    t = 1.0 - t
                End If
            End If
            Return t
        End If
        y += t + gammaln(a + b) - gammaln(a) - gammaln(b) 'beta?
        y += Math.Log(w / a)
        If (y < MINLOG) Then
            t = 0.0
        Else
            t = Math.Exp(y)
        End If
        If (flag) Then
            If (t <= MACHEP) Then
                t = 1.0 - MACHEP
            Else
                t = 1.0 - t
            End If
        End If
        Return t
    End Function
    Private Function incbcf(ByVal a As Double, ByVal b As Double, ByVal x As Double) As Double
        Dim k1 As Double = a
        Dim k2 As Double = a + b
        Dim k3 As Double = a
        Dim k4 As Double = a + 1
        Dim k5 As Double = 1
        Dim k6 As Double = b - 1
        Dim k7 As Double = a + 1
        Dim k8 As Double = a + 2
        Dim pkm2 As Double = 0
        Dim qkm2 As Double = 1
        Dim pkm1 As Double = 1
        Dim qkm1 As Double = 1
        Dim qk As Double = 0
        Dim pk As Double = 0
        Dim xk As Double = 0
        Dim t As Double = 0
        Dim z As Double = 0
        Dim ans As Double = 1
        Dim r As Double = 1
        Dim n As Integer = 0
        Dim thresh As Double = 3 * MACHEP
        Dim big As Double = 4503599627370496.0
        Dim biginv As Double = 0.00000000000000022204460492503131
        Do
            xk = -(z * k1 * k2) / (k3 * k4)
            pk = pkm1 + pkm2 * xk
            qk = qkm1 + qkm2 * xk
            pkm2 = pkm1
            pkm1 = pk
            qkm2 = qkm1
            qkm1 = qk
            xk = (z * k5 * k6) / (k7 * k8)
            pk = pkm1 + pkm2 * xk
            qk = qkm1 + qkm2 * xk
            pkm2 = pkm1
            pkm1 = pk
            qkm2 = qkm1
            qkm1 = qk
            If (qk <> 0) Then r = pk / qk
            If (r <> 0) Then
                t = Math.Abs((ans - r) / r)
                ans = r
            Else
                t = 1.0
            End If
            If (t < thresh) Then Return ans
            k1 += 1.0
            k2 += 1.0
            k3 += 2.0
            k4 += 2.0
            k5 += 1.0
            k6 -= 1.0
            k7 += 2.0
            k8 += 2.0
            If ((Math.Abs(qk) + Math.Abs(pk)) > big) Then
                pkm2 *= biginv
                pkm1 *= biginv
                qkm2 *= biginv
                qkm1 *= biginv
            End If
            If ((Math.Abs(qk) < biginv) Or (Math.Abs(pk) < biginv)) Then
                pkm2 *= big
                pkm1 *= big
                qkm2 *= big
                qkm1 *= big
            End If
        Loop Until (++n < 300)
        Return ans
    End Function
    Private Function incbd(ByVal a As Double, ByVal b As Double, ByVal x As Double) As Double
        Dim k1 As Double = a
        Dim k2 As Double = b - 1
        Dim k3 As Double = a
        Dim k4 As Double = a + 1
        Dim k5 As Double = 1
        Dim k6 As Double = a + b
        Dim k7 As Double = a + 1
        Dim k8 As Double = a + 2
        Dim pkm2 As Double = 0
        Dim qkm2 As Double = 1
        Dim pkm1 As Double = 1
        Dim qkm1 As Double = 1
        Dim qk As Double = 0
        Dim pk As Double = 0
        Dim xk As Double = 0
        Dim t As Double = 0
        Dim z As Double = x / (1 - x)
        Dim ans As Double = 1
        Dim r As Double = 1
        Dim n As Integer = 0
        Dim thresh As Double = 3 * MACHEP
        Dim big As Double = 4503599627370496.0
        Dim biginv As Double = 0.00000000000000022204460492503131
        Do
            xk = -(z * k1 * k2) / (k3 * k4)
            pk = pkm1 + pkm2 * xk
            qk = qkm1 + qkm2 * xk
            pkm2 = pkm1
            pkm1 = pk
            qkm2 = qkm1
            qkm1 = qk
            xk = (z * k5 * k6) / (k7 * k8)
            pk = pkm1 + pkm2 * xk
            qk = qkm1 + qkm2 * xk
            pkm2 = pkm1
            pkm1 = pk
            qkm2 = qkm1
            qkm1 = qk
            If (qk <> 0) Then r = pk / qk
            If (r <> 0) Then
                t = Math.Abs((ans - r) / r)
                ans = r
            Else
                t = 1.0
            End If
            If (t < thresh) Then Return ans
            k1 += 1.0
            k2 -= 1.0
            k3 += 2.0
            k4 += 2.0
            k5 += 1.0
            k6 += 1.0
            k7 += 2.0
            k8 += 2.0
            If ((Math.Abs(qk) + Math.Abs(pk)) > big) Then
                pkm2 *= biginv
                pkm1 *= biginv
                qkm2 *= biginv
                qkm1 *= biginv
            End If
            If ((Math.Abs(qk) < biginv) Or (Math.Abs(pk) < biginv)) Then
                pkm2 *= big
                pkm1 *= big
                qkm2 *= big
                qkm1 *= big
            End If
        Loop Until (++n < 300)
        Return ans
    End Function
    Private Function pseries(ByVal a As Double, ByVal b As Double, ByVal x As Double) As Double
        Dim ai As Double = 1 / a
        Dim u As Double = (1 - b) * x
        Dim v As Double = u / (a + 1)
        Dim t1 As Double = v
        Dim t As Double = u
        Dim n As Double = 2
        Dim s As Double = 0
        Dim z As Double = MACHEP * ai
        While (Math.Abs(v) > z)
            u = (n - b) * x / n
            t *= u
            v = t / (a + n)
            s += v
            n += 1.0
        End While
        s += t1
        s += ai
        u = a * Math.Log(x)

        If ((a + b) < MAXGAM And Math.Abs(u) < MAXLOG) Then
            t = gamma(a + b) / (gamma(a) * gamma(b)) 'this is the beta...
            s = s * t * Math.Pow(x, a)
        Else
            t = gammaln(a + b) - gammaln(a) - gammaln(b) + u + Math.Log(s)
            If (t < MINLOG) Then
                s = 0.0
            Else
                s = Math.Exp(t)
            End If
        End If
        Return s
    End Function
End Module
