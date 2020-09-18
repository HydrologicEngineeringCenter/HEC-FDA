Public Class MonotonicCurveIncreasing
    Inherits PairedData
    Public Property X As List(Of Single)
    Public Property Y As List(Of Single)
    Public Sub New()
        X = New List(Of Single)
        Y = New List(Of Single)
    End Sub
    Public Sub New(element As XElement)
        'MyBase.New(element)
        'Dim xvalues As New List(Of Single)
        'Dim yvalues As New List(Of Single)
        'For Each o As XElement In element.Elements("Ordinate")
        '    xvalues.Add(CSng(o.Attribute("X").Value))
        '    yvalues.Add(CSng(o.Attribute("Y").Value))
        'Next
        '_X = xvalues.ToArray
        '_Y = yvalues.ToArray
        ReadFromXelement(element)
        ''Test for monotonicity
        'TestForMonotonicity()
    End Sub
    Public Sub New(ByVal XValues() As Single, ByVal YValues() As Single)
        X = XValues.ToList
        Y = YValues.ToList
    End Sub
    Public Sub New(ByVal XValues As List(Of Single), ByVal YValues As List(Of Single))
        X = XValues.ToList
        Y = YValues.ToList
    End Sub
    'Private Function TestForMonotonicity() As Boolean
    '    'Test for monotonicity
    '    For i As Int32 = 1 To _Y.Count - 1
    '        If _Y(i) < _Y(i - 1) Then Throw New Exception("Input Y values are not monotonically increasing.")
    '        If _X(i) < _X(i - 1) Then Throw New Exception("Input X values are not monotonically increasing.")
    '    Next
    '    Return True
    'End Function
    Public Overrides Function Verify() As ErrorReport
        Dim report As New ErrorReport
        If Y.Count <> X.Count Then report.AddError(New CurveError("X and Y arrays are not equal in length.", Math.Max(Y.Count, X.Count), If(X.Count > Y.Count, 0, 1)))
        For i As Int32 = 1 To Math.Min(Y.Count, X.Count) - 1
            If _Y(i) < _Y(i - 1) Then report.AddError(New CurveError("Input Y values are not monotonically increasing", i, 1))
            If _X(i) < _X(i - 1) Then report.AddError(New CurveError("Input X values are not monotonically increasing", i, 0))
        Next
        Return report
    End Function
    ''' <summary>
    ''' Creates a deep copy of the object 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Clone() As PairedData
        Return New MonotonicCurveIncreasing(X, Y)
    End Function
    ''' <summary>
    ''' Samples a Y value for a given X from the X coordinates of the curve, solves the function F(X) = Y
    ''' </summary>
    ''' <param name="XValue">A value that represents the X, if the value is below the lowest x value, it returns the lowest y value, if the value is above the highest x value it returns the highest y value</param>
    ''' <returns>a y value for a given x</returns>
    ''' <remarks></remarks>
    Public Function GetYfromX(ByVal XValue As Double) As Double
        Dim CurveIndex As Int32 = X.BinarySearch(CSng(XValue)) 'Array.BinarySearch(_X, CSng(XValue))
        If CurveIndex < -1 Then
            CurveIndex = -1 * CurveIndex - 1
            If CurveIndex = _Y.Count Then
                Return _Y.Last
            Else
                Return _Y(CurveIndex - 1) + (XValue - _X(CurveIndex - 1)) / (_X(CurveIndex) - _X(CurveIndex - 1)) * (_Y(CurveIndex) - _Y(CurveIndex - 1)) 'Y1 + (x - X1) / (X2 - X1) * (Y2 - Y1)
            End If
        ElseIf CurveIndex = -1 Then
            Return _Y(0)
        Else
            Return _Y(CurveIndex)
        End If
    End Function
    ''' <summary>
    ''' Samples a Y value for a given X from the X coordinates of the curve, solves the function F(X) = Y
    ''' </summary>
    ''' <param name="XValue">A value that represents the X, if the value is below the lowest x value, it returns the lowest y value, if the value is above the highest x value it returns the highest y value</param>
    ''' <returns>a y value for a given x</returns>
    ''' <remarks></remarks>
    Public Function GetYfromX(ByVal XValue As Single) As Double
        Dim CurveIndex As Int32 = X.BinarySearch(XValue)
        If CurveIndex < -1 Then
            CurveIndex = -1 * CurveIndex - 1
            If CurveIndex = _Y.Count Then
                Return _Y.Last
            Else
                Return _Y(CurveIndex - 1) + (XValue - _X(CurveIndex - 1)) / (_X(CurveIndex) - _X(CurveIndex - 1)) * (_Y(CurveIndex) - _Y(CurveIndex - 1)) 'Y1 + (x - X1) / (X2 - X1) * (Y2 - Y1)
            End If
        ElseIf CurveIndex = -1 Then
            Return _Y(0)
        Else
            Return _Y(CurveIndex)
        End If
    End Function
    ''' <summary>
    ''' Samples an X value based on a given Y from the Y coordinates of the curve, solves the inverse function of F(X)=Y
    ''' </summary>
    ''' <param name="YValue">A value that represents the Y, if the value is below the lowest y value, it returns the lowest x value, if the value is above the highest y value it returns the highest x value</param>
    ''' <returns>an x value for a given y</returns>
    ''' <remarks></remarks>
    Public Function GetXfromY(ByVal YValue As Double) As Double
        Dim CurveIndex As Int32 = Y.BinarySearch(CSng(YValue)) 'Array.BinarySearch(_Y, CSng(YValue))
        If CurveIndex < -1 Then
            CurveIndex = -1 * CurveIndex - 1
            If CurveIndex = _Y.Count Then
                Return _X.Last
            Else
                Return _X(CurveIndex - 1) + (YValue - _Y(CurveIndex - 1)) / (_Y(CurveIndex) - _Y(CurveIndex - 1)) * (_X(CurveIndex) - _X(CurveIndex - 1)) 'X1 + (y - y1) / (y2 - y1) * (x2 - x1)
            End If
        ElseIf CurveIndex = -1 Then
            Return _X(0)
        Else
            Return _X(CurveIndex)
        End If
    End Function
    ''' <summary>
    ''' Samples an X value based on a given Y from the Y coordinates of the curve, solves the inverse function of F(X)=Y
    ''' </summary>
    ''' <param name="YValue">A value that represents the Y, if the value is below the lowest y value, it returns the lowest x value, if the value is above the highest y value it returns the highest x value</param>
    ''' <returns>an x value for a given y</returns>
    ''' <remarks></remarks>
    Public Function GetXfromY(ByVal YValue As Single) As Double
        Dim CurveIndex As Int32 = Y.BinarySearch(YValue)
        If CurveIndex < -1 Then
            CurveIndex = -1 * CurveIndex - 1
            If CurveIndex = _Y.Count Then
                Return _X.Last
            Else
                Return _X(CurveIndex - 1) + (YValue - _Y(CurveIndex - 1)) / (_Y(CurveIndex) - _Y(CurveIndex - 1)) * (_X(CurveIndex) - _X(CurveIndex - 1)) 'X1 + (y - y1) / (y2 - y1) * (x2 - x1)
            End If
        ElseIf CurveIndex = -1 Then
            Return _X(0)
        Else
            Return _X(CurveIndex)
        End If
    End Function
    Public Overrides Function WritetoXelement() As XElement
        Dim result As New XElement(Me.GetType.Name)
        Dim ordinate As XElement
        For i = 0 To _X.Count - 1
            ordinate = New XElement("Ordinate")
            ordinate.SetAttributeValue("X", String.Format("{0:0.######}", _X(i)))
            ordinate.SetAttributeValue("Y", String.Format("{0:0.######}", _Y(i)))
            result.Add(ordinate)
        Next
        Return result
    End Function
    'Public Sub readfromXML(element As XElement)
    '    X = New List(Of Single)
    '    Y = New List(Of Single)
    '    For Each o As XElement In element.Elements("Ordinate")
    '        X.Add(CSng(o.Attribute("X").Value))
    '        Y.Add(CSng(o.Attribute("Y").Value))
    '    Next

    '    'Test for monotonicity
    '    'TestForMonotonicity()
    'End Sub

    Public Overrides Sub ReadFromXelement(ele As XElement)
        X = New List(Of Single)
        Y = New List(Of Single)
        For Each o As XElement In ele.Elements("Ordinate")
            X.Add(CSng(o.Attribute("X").Value))
            Y.Add(CSng(o.Attribute("Y").Value))
        Next
    End Sub
    ''' <summary>
    ''' Produces a approximation of the area under the curve based on the supplied ordinates.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks> The approximation is based on a summation of trapizodal areas (e.g. Y-value midpoints) dervived from a histogram of the supplied ordinates. </remarks>
    Public Function ComputeTrapizoidalRiemannSum() As Single
        'Sum (i = 1 to I): (f(x(i+1)) + f(x(i)))/2 * (x(i+1) - x(i))
        'Is this how we'd like to deal wiht negative domains and ranges?
        Dim riemannSum As Single = 0
        For i = 0 To X.Count - 1
            riemannSum += (Y(i + 1) + Y(i)) / 2 * Math.Abs(X(i + 1) - X(i))
        Next
        Return riemannSum
    End Function
    ''' <summary>
    ''' Produces an over-estimated approximation of the area under the monotonically increasing curve based on the supplied orinates. 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks> The approximation is based on a summation of rectangular areas derived from a histogram of the supplied ordinates. </remarks>
    Public Function ComputeLeftSideRiemannSum() As Single
        'Sum (i = 1 to I): f(x(i)) * (x(i+1) - x(i))
        'Is this how we'd like to deal wiht negative domains and ranges?
        Dim riemannSum As Single = 0
        For i = 0 To X.Count - 1
            riemannSum += (Y(i)) * Math.Abs(X(i + 1) - X(i))
        Next
        Return riemannSum
    End Function
    ''' <summary>
    ''' Produces an under-estimated approximation of the area under the monontonically increasing curve based on the the supplied ordiantes.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks> The approximation is based on a summation of rectangular areas derived from a histogram of the supplied ordinates. </remarks>
    Public Function ComputeRightSideRiemannSum() As Single
        'Sum (i = 1 to I): f(x(i+1)) * (x(i+1) - x(i))
        'Is this how we'd like to deal wiht negative domains and ranges?
        Dim riemannSum As Single = 0
        For i = 0 To X.Count - 1
            riemannSum += Y(i + 1) * Math.Abs(X(i + 1) - X(i))
        Next
        Return riemannSum
    End Function
End Class
