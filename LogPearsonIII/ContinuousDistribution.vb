''' <summary>
''' This is the parent of many continuous distributions, each will have a property to return a distributed variable based on a
''' random number between zero and 1 exclusive
''' </summary>
''' <remarks></remarks>
Public MustInherit Class ContinuousDistribution
#Region "GoodnessOfFit"
    ''' <summary>
    ''' The Kolmogorov Smirnov Test. 
    ''' </summary>
    ''' <param name="Fe">The data that defines the summary statistics, I am asking for it again since I do not store the data as an array in the continuous distribution class.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Kolmogorov_SmirnovTest(ByRef Fe As Emperical) As Double
        Dim n As Int32 = Fe.GetSampleSize
        Dim diff(n - 1) As Double 'data needs to be ranked?
        Dim incriment As Double = 1 / n
        For i = 0 To n - 1
            diff(i) = Fe.GetCDF(i * incriment) - getDistributedVariable(i * incriment)
        Next
        Return diff.Max
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data">the data used to estimate the parameters for the distribution</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AkaikeInformationCriterionTest(ByVal data() As Double) As Double
        Dim LLmax As Double
        For i = 0 To data.Count - 1
            LLmax += Math.Log(GetPDF(data(i)))
        Next
        Return ((2 * data.Count * GetNumberOfParameters) / (data.Count - GetNumberOfParameters - 1)) - (LLmax * 2)
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data">the data used to estimate the parameters for the distribution</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function BayesianInformationCriterionTest(ByVal data() As Double) As Double
        Dim LLmax As Double
        For i = 0 To data.Count - 1
            LLmax += Math.Log(GetPDF(data(i)))
        Next
        Return (-2 * LLmax) + (GetNumberOfParameters * (Math.Log(data.Count))) ' - Math.Log(2 * Math.PI)))
    End Function
    ''' <summary>
    ''' The probability plot correlation test is a comparison described by correlation of the emperical distribution to the numerically approximated distribution
    ''' </summary>
    ''' <param name="Fe">the emperical distribution of the input data</param>
    ''' <returns>a number representing the correlation of the input data to the computed numerical distribution</returns>
    ''' <remarks></remarks>
    Public Function ProbabilityPlotCorrelationtest(ByRef Fe As Emperical) As Double
        Dim n As Int32 = Fe.GetSampleSize
        Dim incriment As Double = 1 / n
        Dim fx(n - 1) As Double
        Dim fnx() As Double = Fe.GetData.ToArray 'the data is sorted (decending)
        For i = 0 To fx.Count - 1
            fx(i) = getDistributedVariable(((i * incriment)))
        Next
        Dim result As Double = Correlation(fnx, fx)
        Return result
    End Function
    ''' <summary>
    ''' The Pearson Chi Squared test takes the square of the difference between the emperical data and the analytical data across probability divided by the anylitical, and sums the errors. it then samples a chi squared distribution where n = the number of variables in the anylitical distribution with the sum of the errors.
    ''' </summary>
    ''' <param name="Fe">the emperical distribution function</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function PearsonChiSquaredTest(ByRef Fe As Emperical) As Double
        Dim err As New List(Of Double)
        Dim chisquaretest As Double = 0
        Dim n As Int32 = Fe.GetSampleSize
        Dim incriment As Double = 1 / n
        For i As Int32 = 0 To n - 1
            Dim value As Double = Fe.GetData(i)
            Dim analytical As Double = GetCDF(value)
            Dim emperical As Double = Fe.GetCDF(value)
            err.Add((((emperical - analytical) ^ 2) / analytical))
            chisquaretest += err(i)
        Next
        Dim c As New ChiSquared(n - GetNumberOfParameters)
        Return c.GetCDF(chisquaretest)
    End Function
    ''' <summary>
    ''' this is a test that weights the tails. (not sure how it works, nor if it is coded correctly)
    ''' </summary>
    ''' <param name="Fe">an emperical distribution function</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AndersonDarling(ByVal Fe As Emperical) As Double
        Dim n As Integer = Fe.GetSampleSize
        Dim incriment As Double = 1 / n
        Dim Diff As Double
        Dim bottom As Double
        Dim Fx As Double
        Dim sum As Double = 0
        For i = 0 To n - 1
            Dim value As Double = Fe.GetData(i)
            Fx = (GetCDF(value))
            Diff = Fe.GetCDF(value) - Fx 'make sure the cdf is in the correct order.
            bottom = Fx * (1 - Fx)
            sum += ((Diff ^ 2) / bottom) * incriment
        Next
        Return sum * n
    End Function
    ''' <summary>
    ''' this test is much like the anderson darling, with a different weighting function, not sure how to use it, nor if it is coded correctly.
    ''' </summary>
    ''' <param name="fe">the emperical distribution</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Cramer_VonMiserTest(ByRef Fe As Emperical) As Double
        'http://www.mathworks.com/matlabcentral/fileexchange/3579-smirnov-cramer-von-mises-test/content/mtest.m
        Dim n As Integer = Fe.GetSampleSize
        Dim incriment As Double = 1 / n
        Dim Diff As Double
        Dim Fx As Double
        Dim sum As Double = 0
        For i = 0 To n - 1
            Dim value As Double = Fe.GetData(i)
            Fx = (GetCDF(value))
            Diff = Fe.GetCDF(value) - Fx 'make sure the cdf is in the correct order.
            sum += ((Diff ^ 2)) * incriment
        Next
        Return sum * n
    End Function

#End Region
    Public Function CreateAnalyticalBootstrap(Optional ByVal seed As Integer = Nothing) As Double()
        Dim ret(GetSampleSize - 1) As Double
        Dim r As Random
        If seed = Nothing Then
            r = New Random
        Else
            r = New Random(seed)
        End If
        For i = 0 To GetSampleSize - 1
            ret(i) = getDistributedVariable(r.NextDouble)
        Next
        Return ret
    End Function
    Public Function CreateConfidenceInterval(ByVal probabilities As List(Of Double), ByVal upperval As Double, ByVal lowerval As Double) As MonotonicCurveUSingle
        Dim r As New Random()
        Dim bpmlist As New List(Of BasicProductMomentsStats)
        For i = 0 To probabilities.Count - 1
            bpmlist.Add(New BasicProductMomentsStats)
        Next
        Dim complete As Boolean = False
        Dim contdist As ContinuousDistribution = Clone()
        Do Until complete
            contdist.SetParameters(CreateAnalyticalBootstrap(r.Next))
            For i = 0 To probabilities.Count - 1
                bpmlist(i).AddObservation(contdist.getDistributedVariable(probabilities(i)))
            Next
            complete = checkforcompleteness(bpmlist)
        Loop
        Dim d(bpmlist.Count - 1) As ContinuousDistribution
        For i = 0 To bpmlist.Count - 1
            d(i) = New Normal(bpmlist(i).GetMean, bpmlist(i).GetSampleStDev)
        Next

        Return New MonotonicCurveUSingle(probabilities.Select(Function(s) CSng(s)).ToArray, d)
    End Function
    Public Function CreateConfidenceIntervalHEAVY(ByVal probabilitys As List(Of Double), ByVal upperval As Double, ByVal lowerval As Double) As List(Of List(Of Double))
        Dim r As New Random()
        Dim outputData As New List(Of List(Of Double))
        For i = 0 To probabilitys.Count - 1
            outputData.Add(New List(Of Double))
        Next
        Dim bpmlist As New List(Of BasicProductMomentsStats)
        For i = 0 To probabilitys.Count - 1
            bpmlist.Add(New BasicProductMomentsStats)
        Next
        Dim complete As Boolean = False
        Dim contdist As ContinuousDistribution = Clone()
        Dim tmparray(probabilitys.Count - 1) As Double
        Do Until complete
            contdist.SetParameters(CreateAnalyticalBootstrap(r.Next))
            For i = 0 To probabilitys.Count - 1
                tmparray(i) = contdist.getDistributedVariable(probabilitys(i))
                bpmlist(i).AddObservation(tmparray(i))
                outputData(i).Add(tmparray(i))
            Next
            complete = checkforcompleteness(bpmlist)
        Loop
        Dim result As New List(Of List(Of Double))

        Dim d(bpmlist.Count - 1) As ContinuousDistribution
        Dim tmpemp As Emperical
        For i = 0 To probabilitys.Count - 1
            result.Add(New List(Of Double))
            tmpemp = New Emperical(outputData(i))
            result(i).Add(tmpemp.GetPercentile(0.95))
            result(i).Add(tmpemp.GetPercentile(0.5))
            result(i).Add(tmpemp.GetPercentile(0.05))
            d(i) = New Normal(bpmlist(i).GetMean, bpmlist(i).GetSampleStDev)
        Next

        Return result
    End Function
    Private Function checkforcompleteness(ByVal bpmlist As List(Of BasicProductMomentsStats)) As Boolean
        Return bpmlist.Where(Function(stat) stat.IsConverged = False).Count = 0
        'Dim count As Integer = 0
        'For i = 0 To bpmlist.Count - 1
        '    If bpmlist(i).IsConverged Then count += 1
        'Next
        'Return bpmlist.Count = count
    End Function
    ''' <summary>
    ''' This is how many parameters are utilized to generate random numbers of this type (excluding probability)(necessary for the chisquared test)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public MustOverride ReadOnly Property GetNumberOfParameters As Int16
    ''' <summary>
    ''' This is the number of records from the input dataset
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public MustOverride ReadOnly Property GetSampleSize As Int32
    ''' <summary>
    ''' This is the most representative value for the input dataset, it varys by distribution type
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public MustOverride ReadOnly Property GetCentralTendency As Double
    Public MustOverride Sub SetParameters(ByVal data() As Double)
    ''' <summary>
    ''' Returns a variable which will be determined by the distribution of the actual object that this continuous distribution represents
    ''' </summary>
    ''' <param name="probability">a random number between zero and one exclusive that represents a probability of occurance</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public MustOverride Function getDistributedVariable(ByVal probability As Double) As Double
    ''' <summary>
    ''' Returns a probability between zero and 1 which is determined based on the provided value and the pdf function
    ''' </summary>
    ''' <param name="Value">a value that is presumed to be from the distribution</param>
    ''' <returns>a value between zero and 1</returns>
    ''' <remarks></remarks>
    Public MustOverride Function GetPDF(ByVal Value As Double) As Double
    ''' <summary>
    ''' returns a value for a given probability
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public MustOverride Function GetCDF(ByVal value As Double) As Double
    ''' <summary>
    ''' Creates a copy in memory of the continuous distribution.
    ''' </summary>
    ''' <returns>Continuous distribution</returns>
    ''' <remarks></remarks>
    Public MustOverride Function Clone() As ContinuousDistribution
    ''' <summary>
    ''' This function creates an Xelement that represents the data contained in the concrete class. 
    ''' </summary>
    ''' <returns>An Xelement that represents the data in the concrete class.</returns>
    ''' <remarks></remarks>
    Public Function writetoXElement() As XElement
        Dim result As New XElement(Me.GetType.Name)
        Dim fi As System.Reflection.FieldInfo() = Me.GetType.GetFields(Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance) 'if not nonpublic the field will be skipped.
        For i = 0 To fi.Count - 1
            result.SetAttributeValue(fi(i).Name, fi(i).GetValue(Me))
        Next
        Return result
    End Function
    Public MustOverride Function Validate() As String
    ''' <summary>
    ''' this function reads in a distribution from an xml element using reflection.
    ''' </summary>
    ''' <param name="el">the element that is a continuous distribution</param>
    ''' <returns>A concrete continuous distribution.</returns>
    ''' <remarks></remarks>
    Public Shared Function readfromXElement(ByVal el As XElement) As ContinuousDistribution
        Throw New NotImplementedException()
        'Dim n As String = el.Name.ToString
        'Dim ns As String = "Statistics"
        'Dim oh As System.Runtime.Remoting.ObjectHandle = System.Activator.CreateInstance(ns, ns & "." & n)
        'Dim dist As ContinuousDistribution = CType(oh.Unwrap, ContinuousDistribution)
        'Dim fi As System.Reflection.FieldInfo() = dist.GetType.GetFields(Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)
        'For i = 0 To fi.Count - 1
        '    fi(i).SetValue(dist, Convert.ChangeType(el.Attribute(fi(i).Name).Value, fi(i).FieldType))
        'Next
        'Return dist
    End Function
    ''' <summary>
    ''' this sub reads in a distribution from an xml element using reflection.
    ''' </summary>
    ''' <param name="el">the element that is a continuous distribution</param>
    ''' <remarks></remarks>
    Sub readfromXML(ByVal el As XElement)
        Dim fi As System.Reflection.FieldInfo() = Me.GetType.GetFields(Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)
        For i = 0 To fi.Count - 1
            fi(i).SetValue(Me, Convert.ChangeType(el.Attribute(fi(i).Name).Value, fi(i).FieldType))
        Next
        'Dim NS As String = Me.GetType.Namespace
        'Dim n As String = Me.GetType.Name
        'Dim t As Type = System.Type.GetType(NS & "." & n)
        'Dim fi As System.Reflection.FieldInfo() = t.GetFields(Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance) 'if not nonpublic the field will be skipped.
        'For i = 0 To fi.Count - 1
        '    Dim name As String = fi(i).Name
        '    Dim val As String = fi(i).ToString
        '    Dim ty As Type = fi(i).FieldType
        '    Dim strvalue As String = el.Element(name).Value
        '    fi(i).SetValue(Me, Convert.ChangeType(strvalue, ty))
        'Next
    End Sub
    ''' <summary>
    ''' creates an array of uniformly spaced distributed variables from zero to 1
    ''' </summary>
    ''' <param name="bins">how many bins you want</param>
    ''' <returns>returns a variable at the midpoint of each bin</returns>
    ''' <remarks></remarks>
    Public Function getDistributedArray(ByRef bins As Int32) As Double()
        Dim randy As New Random()
        Dim array As New List(Of Double)
        Dim delta As Double = 1 - 0 'should i store this from the real data??
        Dim alpha As Double = delta / (bins)
        Dim midpoint As Double = alpha / 2
        For i = 0 To bins - 1
            array.Add(getDistributedVariable(midpoint))
            midpoint += alpha
        Next
        Return array.ToArray
    End Function
    ''' <summary>
    ''' This function returns an array of sample size equivalent to the data that generated this distribution, but the data is based on a random number and the parameteric distribution rather than the underlying data
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getParametricBootStrapSample() As Double()
        Dim values As Int32 = GetSampleSize - 1
        Dim outputData(values) As Double
        Dim randy As New Random()
        For i = 0 To values
            outputData(i) = getDistributedVariable(randy.NextDouble)
        Next
        Return outputData
    End Function
    ''' <summary>
    ''' This function returns an array of sample size equivalent to the data that generated this distribution, but the data is based on a random number and the parameteric distribution rather than the underlying data
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getParametricBootStrapSample(ByVal randoms As List(Of Double)) As Double()
        Dim values As Int32 = GetSampleSize - 1
        Dim outputData(values) As Double
        For i = 0 To values
            outputData(i) = getDistributedVariable(randoms(i))
        Next
        Return outputData
    End Function
    Public Shared Operator =(left As ContinuousDistribution, right As ContinuousDistribution) As Boolean
        ' Check for null arguments. Keep in mind null == null
        If left Is Nothing AndAlso right Is Nothing Then
            Return True
        ElseIf left Is Nothing Then
            Return False
        ElseIf right Is Nothing Then
            Return False
        Else
            Return left.Equals(right)
        End If
    End Operator
    Public Shared Operator <>(left As ContinuousDistribution, right As ContinuousDistribution) As Boolean
        Return Not (left = right)
    End Operator
    ' Child class Hashcode Override
    'http://stackoverflow.com/questions/371328/why-is-it-important-to-override-gethashcode-when-equals-method-is-overridden
    'http://dobrzanski.net/2010/09/13/csharp-gethashcode-cause-overflowexception/
    Public Function Hash(ByVal ParamArray args() As Object) As Integer
        Dim h As Integer = 13
        For Each o As Object In args
            h = h Xor o.GetHashCode()
        Next
        Return h
    End Function
End Class

