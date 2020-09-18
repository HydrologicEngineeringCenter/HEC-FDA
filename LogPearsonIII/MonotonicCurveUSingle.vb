Public Class MonotonicCurveUSingle
    Inherits PairedData
    Public Property X As List(Of Single)
    Public Property Y As List(Of ContinuousDistribution)
    Sub New()
        X = New List(Of Single)
        Y = New List(Of ContinuousDistribution)
    End Sub
    Public Sub New(ByVal xel As XElement)
        ReadFromXelement(xel)
    End Sub
    Public Sub New(XValues() As Single, YValues() As ContinuousDistribution)
        X = New List(Of Single)
        Y = New List(Of ContinuousDistribution)
        'Copy the data
        For i As Int32 = 0 To YValues.Count - 1
            X.Add(XValues(i))
            Y.Add(YValues(i).Clone)
        Next
    End Sub
    Public Sub New(XValues As List(Of Single), YValues As List(Of ContinuousDistribution))
        X = New List(Of Single)
        Y = New List(Of ContinuousDistribution)
        'Copy the data
        For i As Int32 = 0 To YValues.Count - 1
            X.Add(XValues(i))
            Y.Add(YValues(i).Clone)
        Next
    End Sub
    'Private Function TestForMonotonicity() As Boolean
    '    Test for monotonicity
    '    For i As Int32 = 1 To _Y.Count - 1
    '        If _Y(i).GetType <> _Y(i - 1).GetType Then Throw New MonotonicCurveUException("Input Y distribution types do not match.", i, 1)
    '        If _Y(i).GetCentralTendency < _Y(i - 1).GetCentralTendency Then Throw New MonotonicCurveUException("Input Y value central tendencies are not monotonically increasing.", i, 1)
    '        If _Y(i).getDistributedVariable(0.95) < _Y(i - 1).getDistributedVariable(0.95) Then Throw New MonotonicCurveUException("Input Y value 95% values are not monotonically increasing.", i, 2)
    '        If _Y(i).getDistributedVariable(0.05) < _Y(i - 1).getDistributedVariable(0.05) Then Throw New MonotonicCurveUException("Input Y value 5% values are not monotonically increasing.", i, 2)
    '        If _X(i) < _X(i - 1) Then Throw New MonotonicCurveUException("Input X values are not monotonically increasing.", i, 0)
    '    Next
    '    Return True
    'End Function
    ''' <summary>
    ''' Creates a deep copy of the object 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Clone() As PairedData
        Dim NewCurve As New MonotonicCurveUSingle()
        For i As Int32 = 0 To _Y.Count - 1
            NewCurve.X.Add(_X(i))
            NewCurve.Y.Add(_Y(i).Clone)
        Next
        '
        Return NewCurve

    End Function
    ''' <summary>
    ''' This function samples a curve using the probability axis of the continuous distribution in the Y value for each x ordinate, which will result in a new monotonically increasing curve. 
    ''' </summary>
    ''' <param name="Probability">A value between 0 and 1 representing the value to sample from each y continuous distribution</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CurveSample(ByVal Probability As Double) As MonotonicCurveIncreasing
        If Probability < 0 Then Probability = 0
        If Probability > 1 Then Probability = 1
        Dim X(_X.Count - 1) As Single, Y(_Y.Count - 1) As Single
        For i = 0 To _X.Count - 1
            X(i) = _X(i)
            Y(i) = CSng(_Y(i).getDistributedVariable(Probability))
        Next
        CurveSample = New MonotonicCurveIncreasing(X, Y)
    End Function
    Public Overrides Function WriteToXElement() As XElement
        Dim result As New XElement(Me.GetType.Name)
        result.SetAttributeValue("UncertaintyType", _Y(0).GetType.Name)
        Dim fi As System.Reflection.FieldInfo() = _Y(0).GetType.GetFields(Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance) 'if not nonpublic the field will be skipped.
        Dim ord As XElement
        For j = 0 To _Y.Count - 1
            ord = New XElement("Ordinate")
            ord.SetAttributeValue("X", String.Format("{0:0.######}", _X(j)))
            For i = 0 To fi.Count - 1
                ord.SetAttributeValue(fi(i).Name, fi(i).GetValue(Y(j)))
            Next
            result.Add(ord)
        Next
        Return result
    End Function
    'Public Sub readfromXML(element As XElement)
    '    _X = New List(Of Single)
    '    _Y = New List(Of ContinuousDistribution)
    '    Dim n As String = element.Attribute("UncertaintyType").Value
    '    Dim ns As String = Me.GetType.Namespace
    '    Dim oh As System.Runtime.Remoting.ObjectHandle = System.Activator.CreateInstance(ns, ns & "." & n)
    '    Dim dist As ContinuousDistribution = CType(oh.Unwrap, ContinuousDistribution)
    '    Dim fi As System.Reflection.FieldInfo() = dist.GetType.GetFields(Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)
    '    For Each o As XElement In element.Elements("Ordinate")
    '        _X.Add(CSng(o.Attribute("X").Value))
    '        For i = 0 To fi.Count - 1
    '            fi(i).SetValue(dist, Convert.ChangeType(o.Attribute(fi(i).Name).Value, fi(i).FieldType))
    '        Next
    '        _Y.Add(dist.Clone)
    '    Next
    '    '
    '    ' TestForMonotonicity()
    'End Sub

    Public Overrides Sub ReadFromXelement(ele As XElement)
        Throw New NotImplementedException()
        'X = New List(Of Single)
        'Y = New List(Of ContinuousDistribution)
        'Dim n As String = ele.Attribute("UncertaintyType").Value
        'Dim ns As String = Me.GetType.Namespace
        'Dim oh As System.Runtime.Remoting.ObjectHandle = System.Activator.CreateInstance(ns, ns & "." & n)
        'Dim dist As ContinuousDistribution = CType(oh.Unwrap, ContinuousDistribution)
        'Dim fi As System.Reflection.FieldInfo() = dist.GetType.GetFields(Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)
        'For Each o As XElement In ele.Elements("Ordinate")
        '    _X.Add(CSng(o.Attribute("X").Value))
        '    For i = 0 To fi.Count - 1
        '        fi(i).SetValue(dist, Convert.ChangeType(o.Attribute(fi(i).Name).Value, fi(i).FieldType))
        '    Next
        '    _Y.Add(dist.Clone)
        'Next
    End Sub
    Public Overrides Function Verify() As ErrorReport
        Throw New NotImplementedException()
        'Dim report As New ErrorReport
        'If Y.Count <> X.Count Then report.AddError(New CurveError("X and Y arrays are not equal in length.", Math.Max(Y.Count, X.Count), If(X.Count > Y.Count, 0, 1)))
        'For i As Int32 = 1 To Math.Min(Y.Count, X.Count) - 1
        '    If _Y(i).GetType <> _Y(i - 1).GetType Then report.AddError(New CurveError("Input Y distribution types do not match", i, 1))
        '    If _Y(i).GetCentralTendency < _Y(i - 1).GetCentralTendency Then report.AddError(New CurveError("Input Y value central tendencies are not monotonically increasing", i, 1))
        '    Dim m2 As Double = _Y(i).getDistributedVariable(1)
        '    Dim m1 As Double = _Y(i - 1).getDistributedVariable(1)
        '    If m2 < m1 Then
        '        Select Case _Y(i).GetType
        '            Case GetType(Triangular)
        '                report.AddError(New CurveError("Input Y value max values are not monotonically increasing", i, 3))
        '            Case Else
        '                report.AddError(New CurveError("Input Y value max values are not monotonically increasing", i, 2))
        '        End Select
        '    End If

        '    m2 = _Y(i).getDistributedVariable(0)
        '    m1 = _Y(i - 1).getDistributedVariable(0)
        '    If m2 < m1 Then report.AddError(New CurveError("Input Y value min values are not monotonically increasing", i, 2))
        '    If _X(i) < _X(i - 1) Then report.AddError(New CurveError("Input X values are not monotonically increasing", i, 0))
        'Next
        'Return report
    End Function
    Public Shared Operator =(left As MonotonicCurveUSingle, right As MonotonicCurveUSingle) As Boolean
        Throw New NotImplementedException()
        '' Check for null arguments. Keep in mind null == null
        'If left Is Nothing AndAlso right Is Nothing Then
        '    Return True
        'ElseIf left Is Nothing Then
        '    Return False
        'ElseIf right Is Nothing Then
        '    Return False
        'End If
        ''
        'If IsNothing(left.X) AndAlso IsNothing(right.X) AndAlso IsNothing(left.Y) AndAlso IsNothing(right.Y) Then
        '    Return True
        'ElseIf IsNothing(left.X) Then
        '    Return False
        'ElseIf IsNothing(right.X) Then
        '    Return False
        'ElseIf IsNothing(left.Y) Then
        '    Return False
        'ElseIf IsNothing(right.Y) Then
        '    Return False
        'End If
        ''
        'If left.X.Count <> right.X.Count Then Return False
        'If left.Y.Count <> right.Y.Count Then Return False
        'For i As Int32 = 0 To left.X.Count - 1
        '    If left.X(i) <> right.X(i) Then Return False
        'Next
        'For i As Int32 = 0 To left.Y.Count - 1
        '    If left.Y(i).GetType <> right.Y(i).GetType Then Return False
        '    If left.Y(i) <> right.Y(i) Then Return False
        'Next

        'Return True
    End Operator
    Public Shared Operator <>(left As MonotonicCurveUSingle, right As MonotonicCurveUSingle) As Boolean
        Return Not (left = right)
    End Operator

End Class
