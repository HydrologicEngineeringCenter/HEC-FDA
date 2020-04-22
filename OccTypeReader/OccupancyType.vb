Imports Functions
Imports Statistics
Namespace ComputableObjects
    Public Class OccupancyType
        Private _Name As String = ""
        Private _Description As String = ""
        Private _DamageCategory As DamageCategory
        Private _CalcStructDamage As Boolean = False
        Private _CalcContentDamage As Boolean = False
        Private _CalcOtherDamage As Boolean = False
        Private _CalcVehicleDamage As Boolean = False
        Private _StructureDDPercent As MonotonicCurveUSingle
        Private _ContentDDPercent As MonotonicCurveUSingle
        Private _OtherDDPercent As MonotonicCurveUSingle
        Private _VehicleDDPercent As MonotonicCurveUSingle
        Private _FndHeightAsPcntOfMean As ContinuousDistribution
        Private _StructureValAsPcntOfMean As ContinuousDistribution
        Private _ContentValAsPcntOfMean As ContinuousDistribution
        Private _OtherValAsPcntOfMean As ContinuousDistribution
        Private _VehicleValAsPcntOfMean As ContinuousDistribution
        Public Event ReportMessage(ByVal message As String)
        '
#Region "Properties"
        Public Property Name As String
            Get
                Return _Name
            End Get
            Set(value As String)
                _Name = value
            End Set
        End Property
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property
        Public Property DamageCategory As DamageCategory
            Get
                Return _DamageCategory
            End Get
            Set(value As DamageCategory)
                _DamageCategory = value
            End Set
        End Property
        Public Property FoundationHeightUncertainty As ContinuousDistribution
            Get
                Return _FndHeightAsPcntOfMean
            End Get
            Set(value As ContinuousDistribution)
                If Not IsNothing(value) Then _FndHeightAsPcntOfMean = value.Clone
            End Set
        End Property
        'Public Property FoundationHeightUncertaintyFunction As Functions.IDistributedOrdinate
        '    Get
        '        _FndHeightAsPcntOfMean.retu

        '        Return _FndHeightAsPcntOfMean
        '    End Get
        '    Set(value As ContinuousDistribution)
        '        If Not IsNothing(value) Then _FndHeightAsPcntOfMean = value.Clone
        '    End Set
        'End Property
        Public Property StructureValueUncertainty As ContinuousDistribution
            Get
                Return _StructureValAsPcntOfMean
            End Get
            Set(value As ContinuousDistribution)
                If Not IsNothing(value) Then _StructureValAsPcntOfMean = value.Clone
            End Set
        End Property
        Public Property ContentValueUncertainty As ContinuousDistribution
            Get
                Return _ContentValAsPcntOfMean
            End Get
            Set(value As ContinuousDistribution)
                If Not IsNothing(value) Then _ContentValAsPcntOfMean = value.Clone
            End Set
        End Property
        Public Property OtherValueUncertainty As ContinuousDistribution
            Get
                Return _OtherValAsPcntOfMean
            End Get
            Set(value As ContinuousDistribution)
                If Not IsNothing(value) Then _OtherValAsPcntOfMean = value.Clone
            End Set
        End Property
        Public Property VehicleValueUncertainty As ContinuousDistribution
            Get
                Return _VehicleValAsPcntOfMean
            End Get
            Set(value As ContinuousDistribution)
                If Not IsNothing(value) Then _VehicleValAsPcntOfMean = value.Clone
            End Set
        End Property
        Public Property CalcStructDamage As Boolean
            Get
                Return _CalcStructDamage
            End Get
            Set(value As Boolean)
                _CalcStructDamage = value
            End Set
        End Property
        Public Property CalcContentDamage As Boolean
            Get
                Return _CalcContentDamage
            End Get
            Set(value As Boolean)
                _CalcContentDamage = value
            End Set
        End Property
        Public Property CalcOtherDamage As Boolean
            Get
                Return _CalcOtherDamage
            End Get
            Set(value As Boolean)
                _CalcOtherDamage = value
            End Set
        End Property
        Public Property CalcVehicleDamage As Boolean
            Get
                Return _CalcVehicleDamage
            End Get
            Set(value As Boolean)
                _CalcVehicleDamage = value
            End Set
        End Property
        Public ReadOnly Property GetStructurePercentDD As MonotonicCurveUSingle
            Get
                If IsNothing(_StructureDDPercent) Then Return Nothing
                Return _StructureDDPercent.Clone
            End Get
        End Property
        Public WriteOnly Property SetStructurePercentDD As MonotonicCurveUSingle
            Set(value As MonotonicCurveUSingle)
                If Not IsNothing(value) Then _StructureDDPercent = value.Clone
            End Set
        End Property
        Public ReadOnly Property GetContentPercentDD As MonotonicCurveUSingle
            Get
                If IsNothing(_ContentDDPercent) Then Return Nothing
                Return _ContentDDPercent.Clone
            End Get
        End Property
        Public WriteOnly Property SetContentPercentDD As MonotonicCurveUSingle
            Set(value As MonotonicCurveUSingle)
                If Not IsNothing(value) Then _ContentDDPercent = value.Clone
            End Set
        End Property
        Public ReadOnly Property GetOtherPercentDD As MonotonicCurveUSingle
            Get
                If IsNothing(_OtherDDPercent) Then Return Nothing
                Return _OtherDDPercent.Clone
            End Get
        End Property
        Public WriteOnly Property SetOtherPercentDD As MonotonicCurveUSingle
            Set(value As MonotonicCurveUSingle)
                If Not IsNothing(value) Then _OtherDDPercent = value.Clone
            End Set
        End Property
        Public ReadOnly Property GetVehiclePercentDD As MonotonicCurveUSingle
            Get
                If IsNothing(_VehicleDDPercent) Then Return Nothing
                Return _VehicleDDPercent.Clone
            End Get
        End Property
        Public WriteOnly Property SetVehiclePercentDD As MonotonicCurveUSingle
            Set(value As MonotonicCurveUSingle)
                If Not IsNothing(value) Then _VehicleDDPercent = value.Clone
            End Set
        End Property
#End Region
#Region "Constructors"
        Public Sub New()
            _Name = ""
            _DamageCategory = New DamageCategory
            _FndHeightAsPcntOfMean = New None
            _StructureValAsPcntOfMean = New None
            _ContentValAsPcntOfMean = New None
            _OtherValAsPcntOfMean = New None
            _VehicleValAsPcntOfMean = New None
            _StructureDDPercent = New MonotonicCurveUSingle
            _ContentDDPercent = New MonotonicCurveUSingle
            _OtherDDPercent = New MonotonicCurveUSingle
            _VehicleDDPercent = New MonotonicCurveUSingle
        End Sub
        Public Sub New(ByVal OccupancyTypeName As String, ByVal DamageCategoryName As String)
            _Name = OccupancyTypeName
            _DamageCategory = New DamageCategory(DamageCategoryName)
            _FndHeightAsPcntOfMean = New None
            _StructureValAsPcntOfMean = New None
            _ContentValAsPcntOfMean = New None
            _OtherValAsPcntOfMean = New None
            _VehicleValAsPcntOfMean = New None
            _StructureDDPercent = New MonotonicCurveUSingle
            _ContentDDPercent = New MonotonicCurveUSingle
            _OtherDDPercent = New MonotonicCurveUSingle
            _VehicleDDPercent = New MonotonicCurveUSingle
        End Sub
        Sub New(ByVal occtype As System.Text.StringBuilder, ByVal startdata As Integer, ByVal parameter As Integer)
            _DamageCategory = New DamageCategory
            _FndHeightAsPcntOfMean = New None
            _StructureValAsPcntOfMean = New None
            _ContentValAsPcntOfMean = New None
            _OtherValAsPcntOfMean = New None
            _VehicleValAsPcntOfMean = New None
            _StructureDDPercent = New MonotonicCurveUSingle
            _ContentDDPercent = New MonotonicCurveUSingle
            _OtherDDPercent = New MonotonicCurveUSingle
            _VehicleDDPercent = New MonotonicCurveUSingle
            Dim lines() As String = Split(occtype.ToString, vbNewLine)
            Dim line() As String
            Dim tmparray()() As String
            Dim stagecount As Integer = 0
            Dim stages As New List(Of Single)
            Dim curvetype As String = ""
            Dim val As Single
            For i = 0 To lines.Count - 1
                If i = 0 Then
                    line = Split(lines(i), vbTab)
                    If i = 0 Then
                        _Name = line(0)
                        _Description = line(1)
                        _DamageCategory = New DamageCategory(line(2))
                    End If
                Else
                End If
                Select Case line(parameter)
                    Case "Stage"
                        If stages.Count > 0 Then stages.Clear()
                        For j = parameter + 1 To line.Count - 1
                            If Single.TryParse(line(j), val) Then stages.Add(val)
                        Next
                        i += 1
                        line = Split(lines(i), vbTab)
                        curvetype = line(parameter).Chars(0)
                        Do Until line(parameter) = "Stage" Or line(parameter) = "Struct" Or curvetype <> line(parameter).Chars(0)
                            If IsNothing(tmparray) Then
                                ReDim tmparray(0)
                            Else
                                ReDim Preserve tmparray(tmparray.Count)
                            End If
                            tmparray(tmparray.Count - 1) = line
                            i += 1
                            line = Split(lines(i), vbTab)
                        Loop
                        Select Case curvetype
                            Case "S"
                                _CalcStructDamage = True
                                _StructureDDPercent = ConvertFDAStringToCurve(tmparray, stages, parameter)
                            Case "C"
                                _CalcContentDamage = True
                                _ContentDDPercent = ConvertFDAStringToCurve(tmparray, stages, parameter)
                            Case "O"
                                _CalcOtherDamage = True
                                _OtherDDPercent = ConvertFDAStringToCurve(tmparray, stages, parameter)
                            Case Else
                        End Select
                        tmparray = Nothing
                        i -= 1
                    Case "Struct"
                        'parse struct line
                        ConvertFDAStructLineToContDists(line, parameter)
                    Case Else
                        'could cause potential issues

                        Do Until line(parameter) = "Stage" Or line(parameter) = "Struct" Or curvetype <> line(parameter).Chars(0)
                            If IsNothing(tmparray) Then
                                ReDim tmparray(0)
                                curvetype = line(parameter).Chars(0)
                            Else
                                ReDim Preserve tmparray(tmparray.Count)
                            End If
                            tmparray(tmparray.Count - 1) = line
                            i += 1
                            line = Split(lines(i), vbTab)
                        Loop
                End Select
            Next
        End Sub
        Public Sub New(Xel As XElement)
            _Name = Xel.Attribute("Name").Value
            If Xel.Elements("Description").Any Then
                _Description = Xel.Element("Description").Value
            Else
                _Description = ""
            End If
            If Xel.Elements("DamageCategory").Any Then
                _DamageCategory = New DamageCategory(Xel.Element("DamageCategory"))
            Else
                _DamageCategory = New DamageCategory
            End If
            If Xel.Elements("FoundationHeightUncertainty").Any Then
                _FndHeightAsPcntOfMean = ContinuousDistribution.readfromXElement(Xel.Element("FoundationHeightUncertainty").Descendants.First)
            Else
                _FndHeightAsPcntOfMean = New None
            End If
            '
            If Xel.Elements("StructureUncertainty").Any Then
                _StructureValAsPcntOfMean = ContinuousDistribution.readfromXElement(Xel.Element("StructureUncertainty").Descendants.First)
            Else
                _StructureValAsPcntOfMean = New None()
            End If
            '
            If Xel.Elements("ContentUncertainty").Any Then
                _ContentValAsPcntOfMean = ContinuousDistribution.readfromXElement(Xel.Element("ContentUncertainty").Descendants.First)
            Else
                _ContentValAsPcntOfMean = New None
            End If
            '
            If Xel.Elements("OtherUncertainty").Any Then
                _OtherValAsPcntOfMean = ContinuousDistribution.readfromXElement(Xel.Element("OtherUncertainty").Descendants.First)
            Else
                _OtherValAsPcntOfMean = New None
            End If
            '
            If Xel.Elements("VehicleUncertainty").Any Then
                _VehicleValAsPcntOfMean = ContinuousDistribution.readfromXElement(Xel.Element("VehicleUncertainty").Descendants.First)
            Else
                _VehicleValAsPcntOfMean = New None
            End If
            '
            Dim DDCurve As XElement = Xel.Element("StructureDD")
            _CalcStructDamage = CBool(DDCurve.Attribute("CalculateDamage").Value)
            If DDCurve.Elements.Count > 0 Then
                _StructureDDPercent = New MonotonicCurveUSingle(DDCurve.Element("MonotonicCurveUSingle"))
            Else
                _StructureDDPercent = New MonotonicCurveUSingle
            End If
            '
            DDCurve = Xel.Element("ContentDD")
            _CalcContentDamage = CBool(DDCurve.Attribute("CalculateDamage").Value)
            If DDCurve.Elements.Count > 0 Then
                _ContentDDPercent = New MonotonicCurveUSingle(DDCurve.Element("MonotonicCurveUSingle"))
            Else
                _ContentDDPercent = New MonotonicCurveUSingle
            End If
            '
            DDCurve = Xel.Element("OtherDD")
            _CalcOtherDamage = CBool(DDCurve.Attribute("CalculateDamage").Value)
            If DDCurve.Elements.Count > 0 Then
                _OtherDDPercent = New MonotonicCurveUSingle(DDCurve.Element("MonotonicCurveUSingle"))
            Else
                _OtherDDPercent = New MonotonicCurveUSingle
            End If
            '
            DDCurve = Xel.Element("VehicleDD")
            _CalcVehicleDamage = CBool(DDCurve.Attribute("CalculateDamage").Value)
            If DDCurve.Elements.Count > 0 Then
                _VehicleDDPercent = New MonotonicCurveUSingle(DDCurve.Element("MonotonicCurveUSingle"))
            Else
                _VehicleDDPercent = New MonotonicCurveUSingle
            End If

        End Sub
#End Region
#Region "FDA Text Manipulations"
        Public Sub LoadFromFDAInformation(ByVal occtype As System.Text.StringBuilder, ByVal startdata As Integer, ByVal parameter As Integer)
            Dim lines() As String = Split(occtype.ToString, vbNewLine)
            Dim line() As String
            Dim tmparray()() As String
            Dim stagecount As Integer = 0
            Dim stages As New List(Of Single)
            Dim curvetype As String = ""
            Dim val As Single
            For i = 0 To lines.Count - 1
                If i = 0 Then
                    line = Split(lines(i), vbTab)
                    If i = 0 Then
                        _Name = line(0)
                        _Description = line(1)
                        _DamageCategory = New DamageCategory(line(2))
                    End If
                Else
                End If
                Select Case line(parameter)
                    Case "Stage"
                        If stages.Count > 0 Then stages.Clear()
                        For j = parameter + 1 To line.Count - 1
                            If Single.TryParse(line(j), val) Then stages.Add(val)
                        Next
                        i += 1
                        line = Split(lines(i), vbTab)
                        curvetype = line(parameter).Chars(0)
                        Do Until line(parameter) = "Stage" Or line(parameter) = "Struct" Or curvetype <> line(parameter).Chars(0)
                            If IsNothing(tmparray) Then
                                ReDim tmparray(0)
                            Else
                                ReDim Preserve tmparray(tmparray.Count)
                            End If
                            tmparray(tmparray.Count - 1) = line
                            i += 1
                            line = Split(lines(i), vbTab)
                        Loop
                        Select Case curvetype
                            Case "S"
                                _CalcStructDamage = True
                                _StructureDDPercent = ConvertFDAStringToCurve(tmparray, stages, parameter)
                            Case "C"
                                _CalcContentDamage = True
                                _ContentDDPercent = ConvertFDAStringToCurve(tmparray, stages, parameter)
                            Case "O"
                                _CalcOtherDamage = True
                                _OtherDDPercent = ConvertFDAStringToCurve(tmparray, stages, parameter)
                            Case Else
                        End Select
                        tmparray = Nothing
                        i -= 1
                    Case "Struct"
                        'parse struct line
                        ConvertFDAStructLineToContDists(line, parameter)
                    Case Else
                        'could cause potential issues

                        Do Until line(parameter) = "Stage" Or line(parameter) = "Struct" Or curvetype <> line(parameter).Chars(0)
                            If IsNothing(tmparray) Then
                                ReDim tmparray(0)
                                curvetype = line(parameter).Chars(0)
                            Else
                                ReDim Preserve tmparray(tmparray.Count)
                            End If
                            tmparray(tmparray.Count - 1) = line
                            i += 1
                            line = Split(lines(i), vbTab)
                        Loop
                End Select
            Next
        End Sub
        Private Function ConvertFDAStringToCurve(ByVal lines()() As String, ByVal stages As List(Of Single), ByVal parameter As Integer) As Statistics.MonotonicCurveUSingle
            Dim xvalues As List(Of Single) = stages
            Dim yvalues As New List(Of Statistics.ContinuousDistribution)
            Dim disttype As String = ""
            Dim means As New List(Of Single)
            Dim firstitems As New List(Of Single)
            Dim seconditems As New List(Of Single)
            Dim val As Single = 0
            For i = 0 To lines.Count - 1
                Select Case Right(lines(i)(parameter), lines(i)(parameter).Length - 1).ToUpper
                    'Case "TAGE"
                    '    For j = parameter + 1 To lines(i).Count - 1
                    '        If Single.TryParse(lines(i)(j), val) Then xvalues.Add(val)
                    '    Next
                    Case ""
                        disttype = "none"
                        For j = parameter + 1 To lines(i).Count - 1
                            If Single.TryParse(lines(i)(j), val) Then means.Add(val)
                        Next
                    Case " "
                        disttype = "none"
                        For j = parameter + 1 To lines(i).Count - 1
                            If Single.TryParse(lines(i)(j), val) Then means.Add(val)
                        Next
                    Case "$"
                        disttype = "none"
                        For j = parameter + 1 To lines(i).Count - 1
                            If Single.TryParse(lines(i)(j), val) Then means.Add(val)
                        Next
                    Case "N"
                        disttype = "normal"
                        For j = parameter + 1 To lines(i).Count - 1
                            If Single.TryParse(lines(i)(j), val) Then firstitems.Add(val)
                        Next
                    Case "TU"
                        disttype = "triangular"
                        For j = parameter + 1 To lines(i).Count - 1
                            If Single.TryParse(lines(i)(j), val) Then seconditems.Add(val)
                        Next
                    Case "TL"
                        For j = parameter + 1 To lines(i).Count - 1
                            If Single.TryParse(lines(i)(j), val) Then firstitems.Add(val)
                        Next
                    Case "L"
                        disttype = "lognorm"
                        For j = parameter + 1 To lines(i).Count - 1
                            If Single.TryParse(lines(i)(j), val) Then firstitems.Add(val)
                        Next
                    Case "U"
                        disttype = "uniform"
                        For j = parameter + 1 To lines(i).Count - 1
                            seconditems.Add(CSng(lines(i)(j)))
                        Next
                End Select
            Next
            Select Case disttype
                Case "none"
                    For j = 0 To means.Count - 1
                        yvalues.Add(New Statistics.None(means(j)))
                    Next
                Case "normal"
                    For j = 0 To firstitems.Count - 1
                        yvalues.Add(New Statistics.Normal(means(j), firstitems(j)))
                    Next
                Case "triangular"
                    For j = 0 To firstitems.Count - 1
                        yvalues.Add(New Statistics.Triangular(firstitems(j), seconditems(j), means(j)))
                    Next
                Case "lognorm"
                    For j = 0 To firstitems.Count - 1
                        yvalues.Add(New Statistics.LogNormal(means(j), firstitems(j)))
                    Next
                Case "uniform"
                    For j = 0 To seconditems.Count - 1
                        yvalues.Add(New Statistics.Uniform(means(j) - seconditems(j), means(j) + seconditems(j)))
                    Next
            End Select
            Dim s As Statistics.MonotonicCurveUSingle
            Try
                s = New Statistics.MonotonicCurveUSingle(xvalues.ToArray, yvalues.ToArray)
                Return s
            Catch ex As Exception
                Select Case ex.Message
                    Case "X and Y arrays are not equal in length."
                        RaiseEvent ReportMessage(ex.Message) : Return New MonotonicCurveUSingle
                    Case Else
                        MsgBox(ex.Message) : Return New MonotonicCurveUSingle
                End Select
            End Try
        End Function
        Private Function ConvertTabularFunctionToFDAString(ByVal tabularfunction As Statistics.MonotonicCurveUSingle, ByVal Type As String) As String
            'determine if it is a curveU or not
            'determine the type of uncertianty (N = Normal, U = Uniform, L = LogNormal, TU = Triangular Upper, TL = Triangular Lower)
            Dim str As New System.Text.StringBuilder
            Dim FrontPart As String = ""
            If Type = "S" Then
                FrontPart = _Name & vbTab & _Description & vbTab & _DamageCategory.Name & vbTab 'only put this if you are on the Structure damage function
            Else
                FrontPart = _Name & vbTab & vbTab & vbTab
            End If
            str.Append(FrontPart & "Stage" & vbTab)
            For i = 0 To tabularfunction.X.Count - 1
                str.Append(tabularfunction.X(i) & vbTab)
            Next
            str.Append(vbNewLine)

            'all subsequent front parts dont need the description or dam cat
            FrontPart = _Name & vbTab & vbTab & vbTab
            'write out the percents
            Select Case tabularfunction.Y(0).GetType
                Case GetType(Statistics.Normal)
                    Dim ordinate As Statistics.Normal
                    Dim Means As New System.Text.StringBuilder
                    Dim Stdevs As New System.Text.StringBuilder
                    Means.Append(FrontPart & Type & vbTab)
                    Stdevs.Append(FrontPart & Type & "N" & vbTab)
                    For i = 0 To tabularfunction.Y.Count - 1
                        ordinate = DirectCast(tabularfunction.Y(i), Statistics.Normal)
                        Means.Append(String.Format("{0:0.###}", ordinate.GetCentralTendency) & vbTab)
                        Stdevs.Append(String.Format("{0:0.###}", ordinate.GetStDev) & vbTab) 'what if they supply them as values greater than 1?
                    Next
                    str.AppendLine(Means.ToString)
                    str.AppendLine(Stdevs.ToString)
                Case GetType(Statistics.LogNormal)
                    Dim ordinate As Statistics.LogNormal
                    Dim Means As New System.Text.StringBuilder
                    Dim Stdevs As New System.Text.StringBuilder
                    Means.Append(FrontPart & Type & vbTab)
                    Stdevs.Append(FrontPart & Type & "L" & vbTab)
                    For i = 0 To tabularfunction.Y.Count - 1
                        ordinate = DirectCast(tabularfunction.Y(i), Statistics.LogNormal)
                        Means.Append(String.Format("{0:0.###}", ordinate.GetCentralTendency) & vbTab)
                        Stdevs.Append(String.Format("{0:0.###}", ordinate.GetStDev) & vbTab) 'is this right? should i multiply by the log of 100 or something? base e or base 10?
                    Next
                    str.AppendLine(Means.ToString)
                    str.AppendLine(Stdevs.ToString)
                Case GetType(Statistics.Triangular)
                    Dim ordinate As Statistics.Triangular
                    Dim ML As New System.Text.StringBuilder
                    Dim TL As New System.Text.StringBuilder
                    Dim TU As New System.Text.StringBuilder
                    ML.Append(FrontPart & Type & vbTab)
                    TL.Append(FrontPart & Type & "TL" & vbTab)
                    TU.Append(FrontPart & Type & "TU" & vbTab)
                    For i = 0 To tabularfunction.Y.Count - 1
                        ordinate = DirectCast(tabularfunction.Y(i), Statistics.Triangular)
                        ML.Append(String.Format("{0:0.###}", ordinate.GetCentralTendency) & vbTab)
                        TU.Append(String.Format("{0:0.###}", ordinate.getMax) & vbTab)
                        TL.Append(String.Format("{0:0.###}", ordinate.getMin) & vbTab)
                    Next
                    str.AppendLine(ML.ToString)
                    str.AppendLine(TU.ToString)
                    str.AppendLine(TL.ToString)
                Case GetType(Statistics.Uniform)
                    Dim ordinate As Statistics.Uniform
                    Dim Median As New System.Text.StringBuilder
                    Dim Stdevs As New System.Text.StringBuilder
                    Median.Append(FrontPart & Type & vbTab)
                    Stdevs.Append(FrontPart & Type & "U" & vbTab)
                    For i = 0 To tabularfunction.Y.Count - 1
                        ordinate = DirectCast(tabularfunction.Y(i), Statistics.Uniform)
                        Median.Append(String.Format("{0:0.###}", ordinate.GetCentralTendency) & vbTab)
                        Stdevs.Append(String.Format("{0:0.###}", ((ordinate.GetMax - ordinate.GetMin) / 2)) & vbTab)
                    Next
                    str.AppendLine(Median.ToString)
                    str.AppendLine(Stdevs.ToString)
                Case GetType(Statistics.None)
                    Dim ordinate As Statistics.None
                    Dim Median As New System.Text.StringBuilder
                    Median.Append(FrontPart & Type & vbTab)
                    For i = 0 To tabularfunction.Y.Count - 1
                        ordinate = DirectCast(tabularfunction.Y(i), Statistics.None)
                        Median.Append(String.Format("{0:0.###}", ordinate.GetCentralTendency) & vbTab)
                    Next
                    str.AppendLine(Median.ToString)
                Case Else
                    Throw New ArgumentException("The first ordinate of the curve was not of type normal log normal triangular or uniform")
            End Select
            Return str.ToString
        End Function
        Private Sub ConvertFDAStructLineToContDists(ByVal Tabbedline() As String, ByVal Parmeter As Integer)
            Dim values(3) As String
            If Tabbedline(Parmeter + 1) = "" OrElse Tabbedline(Parmeter + 1) = " " OrElse Tabbedline(Parmeter + 1) = "-901" Then
                'no first floor
            Else
                'parameter+2 = "" or else!
                For i = 0 To 3
                    values(i) = Tabbedline(Parmeter + i + 1)
                Next
                _FndHeightAsPcntOfMean = structlinetocontdist(values, True, "")

            End If
            If Tabbedline(Parmeter + 1 + 4) = "" OrElse Tabbedline(Parmeter + 1 + 4) = " " OrElse Tabbedline(Parmeter + 1 + 4) = "-901" Then
                'no structure

            Else
                'parameter+2 = "" or else!
                For i = 0 To 3
                    values(i) = Tabbedline(Parmeter + i + 4 + 1)
                Next
                _StructureValAsPcntOfMean = structlinetocontdist(values, True, "")
            End If
            If Tabbedline(Parmeter + 1 + (2 * 4)) = "" OrElse Tabbedline(Parmeter + 1 + (2 * 4)) = " " OrElse Tabbedline(Parmeter + 1 + (2 * 4)) = "-901" Then
                'no content

            Else 'parameter+2 = median as a ratio of structure value or else!
                'not sure how to handle value as ratio of structure value.
                For i = 0 To 3
                    values(i) = Tabbedline(Parmeter + i + (2 * 4) + 1)
                Next
                _ContentValAsPcntOfMean = structlinetocontdist(values, False, "ContentValueRatio")
            End If
            If Tabbedline(Parmeter + 1 + (3 * 4)) = "" OrElse Tabbedline(Parmeter + 1 + (3 * 4)) = " " OrElse Tabbedline(Parmeter + 1 + (3 * 4)) = "-901" Then
                'no other
            Else
                'parameter+2 = median as a ratio of structure value or else!
                'not sure how to handle value as ratio of structure value.
                For i = 0 To 3
                    values(i) = Tabbedline(Parmeter + i + (3 * 4) + 1)
                Next
                _OtherValAsPcntOfMean = structlinetocontdist(values, False, "OtherValueRatio")
            End If
        End Sub
        Private Function structlinetocontdist(ByVal values() As String, ByVal isstructorfound As Boolean, ByVal type As String) As Statistics.ContinuousDistribution
            Dim dist As Statistics.ContinuousDistribution
            Select Case values(0)
                Case "N"
                    dist = New Statistics.Normal(0, CDbl(values(2)))
                    If Not isstructorfound Then
                        If values(1) = "" OrElse values(1) = 0 Then
                        Else
                            RaiseEvent ReportMessage(Name & ":" & type & ":" & values(1))
                        End If
                    End If
                Case "LN"
                    dist = New Statistics.LogNormal(0, CDbl(values(2)))
                    If Not isstructorfound Then
                        If values(1) = "" OrElse values(1) = 0 Then
                        Else
                            RaiseEvent ReportMessage(Name & ":" & type & ":" & values(1))
                        End If
                    End If
                Case "T"
                    dist = New Statistics.Triangular(CDbl(values(2)), CDbl(values(3)), 0)
                    If Not isstructorfound Then
                        If values(1) = "" OrElse values(1) = 0 Then
                        Else
                            RaiseEvent ReportMessage(Name & ":" & type & ":" & values(1))
                        End If
                    End If
                Case "U"
                    dist = New Statistics.Uniform(CDbl(values(1)), CDbl(values(2))) 'ask bob if it is min max or error...
                    If Not isstructorfound Then
                        If values(1) = "" OrElse values(1) = 0 Then
                        Else
                            RaiseEvent ReportMessage(Name & ":" & type & ":" & values(1))
                        End If
                    End If
                Case Else
                    'ug.
                    Throw New ArgumentException("Did not recognize distribution type " & values(1))
            End Select
            Return dist
        End Function
        Private Function CreateStructLine() As String
            Dim str As New System.Text.StringBuilder
            str.Append(_Name & vbTab & vbTab & vbTab & "STRUCT" & vbTab)
            If IsNothing(_FndHeightAsPcntOfMean) Then
                str.Append(vbTab & vbTab & vbTab & vbTab)
            Else
                Select Case _FndHeightAsPcntOfMean.GetType
                    Case GetType(Statistics.Normal)
                        Dim ff As Statistics.Normal = DirectCast(_FndHeightAsPcntOfMean, Statistics.Normal)
                        str.Append("N" & vbTab & vbNull & vbTab & ff.GetStDev & vbTab & vbTab)
                    Case GetType(Statistics.LogNormal)
                        Dim ff As Statistics.LogNormal = DirectCast(_FndHeightAsPcntOfMean, Statistics.LogNormal)
                        str.Append("LN" & vbTab & vbNull & vbTab & ff.GetStDev & vbTab & vbTab)
                    Case GetType(Statistics.Triangular)
                        Dim ff As Statistics.Triangular = DirectCast(_FndHeightAsPcntOfMean, Statistics.Triangular)
                        str.Append("T" & vbTab & vbNull & vbTab & ff.getMin & vbTab & ff.getMax & vbTab)
                    Case GetType(Statistics.Uniform)
                        Dim ff As Statistics.Uniform = DirectCast(_FndHeightAsPcntOfMean, Statistics.Uniform)
                        str.Append("U" & vbTab & vbNull & vbTab & ((ff.GetMax - ff.GetMin) / 2) & vbTab & vbTab) 'Ask bob if it is min max or error
                    Case GetType(Statistics.None)
                        str.Append(vbTab & vbTab & vbTab & vbTab)
                    Case Else
                        Throw New ArgumentException("The First Floor Distribution was not normal log normal triangular or uniform")
                End Select
            End If
            If IsNothing(_StructureValAsPcntOfMean) Then
                str.Append(vbTab & vbTab & vbTab & vbTab)
            Else
                Select Case _StructureValAsPcntOfMean.GetType
                    Case GetType(Statistics.Normal)
                        Dim SV As Statistics.Normal = DirectCast(_StructureValAsPcntOfMean, Statistics.Normal)
                        str.Append("N" & vbTab & vbNull & vbTab & SV.GetStDev & vbTab & vbTab)
                    Case GetType(Statistics.LogNormal)
                        Dim SV As Statistics.LogNormal = DirectCast(_StructureValAsPcntOfMean, Statistics.LogNormal)
                        str.Append("LN" & vbTab & vbNull & vbTab & SV.GetStDev & vbTab & vbTab)
                    Case GetType(Statistics.Triangular)
                        Dim SV As Statistics.Triangular = DirectCast(_StructureValAsPcntOfMean, Statistics.Triangular)
                        str.Append("T" & vbTab & vbNull & vbTab & SV.getMin & vbTab & SV.getMax & vbTab)
                    Case GetType(Statistics.Uniform)
                        Dim SV As Statistics.Uniform = DirectCast(_StructureValAsPcntOfMean, Statistics.Uniform)
                        str.Append("U" & vbTab & vbNull & vbTab & ((SV.GetMax - SV.GetMin) / 2) & vbTab & vbTab) 'Ask bob if it is min max or error
                    Case GetType(Statistics.None)
                        str.Append(vbTab & vbTab & vbTab & vbTab) 'consider revision
                    Case Else
                        Throw New ArgumentException("The structure value Distribution was not normal log normal triangular or uniform")
                End Select
            End If
            If IsNothing(_ContentValAsPcntOfMean) OrElse Not _CalcContentDamage Then
                str.Append(vbTab & vbTab & vbTab & vbTab)
            Else
                Select Case _ContentValAsPcntOfMean.GetType
                    Case GetType(Statistics.Normal)
                        Dim CV As Statistics.Normal = DirectCast(_ContentValAsPcntOfMean, Statistics.Normal)
                        str.Append("N" & vbTab & CV.GetCentralTendency & vbTab & CV.GetStDev & vbTab & vbTab)
                    Case GetType(Statistics.LogNormal)
                        Dim CV As Statistics.LogNormal = DirectCast(_ContentValAsPcntOfMean, Statistics.LogNormal)
                        str.Append("LN" & vbTab & CV.GetCentralTendency & vbTab & CV.GetStDev & vbTab & vbTab)
                    Case GetType(Statistics.Triangular)
                        Dim CV As Statistics.Triangular = DirectCast(_ContentValAsPcntOfMean, Statistics.Triangular)
                        str.Append("T" & vbTab & CV.GetCentralTendency & vbTab & CV.getMin & vbTab & CV.getMax & vbTab)
                    Case GetType(Statistics.Uniform)
                        Dim CV As Statistics.Uniform = DirectCast(_ContentValAsPcntOfMean, Statistics.Uniform)
                        str.Append("U" & vbTab & CV.GetCentralTendency & vbTab & ((CV.GetMax - CV.GetMin) / 2) & vbTab & vbTab) 'Ask bob if it is min max or error
                    Case GetType(Statistics.None)
                        str.Append(vbTab & vbTab & vbTab & vbTab) 'consider revision
                    Case Else
                        Throw New ArgumentException("The Content value Distribution was not normal log normal triangular or uniform")
                End Select
            End If
            If IsNothing(_OtherValAsPcntOfMean) OrElse Not _CalcOtherDamage Then
                str.Append(vbTab & vbTab & vbTab & vbTab)
            Else
                Select Case _OtherValAsPcntOfMean.GetType
                    Case GetType(Statistics.Normal)
                        Dim OV As Statistics.Normal = DirectCast(_OtherValAsPcntOfMean, Statistics.Normal)
                        str.Append("N" & vbTab & OV.GetCentralTendency & vbTab & OV.GetStDev & vbTab & vbTab)
                    Case GetType(Statistics.LogNormal)
                        Dim OV As Statistics.LogNormal = DirectCast(_OtherValAsPcntOfMean, Statistics.LogNormal)
                        str.Append("LN" & vbTab & OV.GetCentralTendency & vbTab & OV.GetStDev & vbTab & vbTab)
                    Case GetType(Statistics.Triangular)
                        Dim OV As Statistics.Triangular = DirectCast(_OtherValAsPcntOfMean, Statistics.Triangular)
                        str.Append("T" & vbTab & OV.GetCentralTendency & vbTab & OV.getMin & vbTab & OV.getMax & vbTab)
                    Case GetType(Statistics.Uniform)
                        Dim OV As Statistics.Uniform = DirectCast(_OtherValAsPcntOfMean, Statistics.Uniform)
                        str.Append("U" & vbTab & OV.GetCentralTendency & vbTab & ((OV.GetMax - OV.GetMin) / 2) & vbTab & vbTab) 'Ask bob if it is min max or error
                    Case GetType(Statistics.None)
                        str.Append(vbTab & vbTab & vbTab & vbTab) 'consider revision
                    Case Else
                        Throw New ArgumentException("The Other value Distribution was not normal log normal triangular or uniform")
                End Select
            End If
            Return str.ToString
        End Function
        Public Function WriteToFDAString() As String
            Dim str As New System.Text.StringBuilder
            If IsNothing(_StructureDDPercent) OrElse Not _CalcStructDamage Then
            Else
                str.Append(ConvertTabularFunctionToFDAString(_StructureDDPercent, "S"))
            End If
            If IsNothing(_ContentDDPercent) OrElse Not _CalcContentDamage Then
            Else
                str.Append(ConvertTabularFunctionToFDAString(_ContentDDPercent, "C"))
            End If
            If IsNothing(_OtherDDPercent) OrElse Not _CalcOtherDamage Then
            Else
                str.Append(ConvertTabularFunctionToFDAString(_OtherDDPercent, "O"))
            End If
            str.Append(CreateStructLine)
            Return str.ToString
        End Function
#End Region

        'Public Function GenerateSampledOccupancyType(ByRef Randy As Random) As SampledOccupancyType
        '    Dim result As New SampledOccupancyType
        '    With result
        '        .FndHeightAsPcntOfMean = _FndHeightAsPcntOfMean.getDistributedVariable(Randy.NextDouble) / 100
        '        If _CalcStructDamage Then
        '            .StructureValAsPcntOfMean = _StructureValAsPcntOfMean.getDistributedVariable(Randy.NextDouble) / 100
        '            .StructureFractionDD = _StructureDDPercent.CurveSample(Randy.NextDouble)
        '            For i As Int32 = 0 To .StructureFractionDD.X.Count - 1
        '                .StructureFractionDD.Y(i) /= 100
        '            Next
        '        End If
        '        If _CalcContentDamage Then
        '            .ContentValAsPcntOfMean = _ContentValAsPcntOfMean.getDistributedVariable(Randy.NextDouble) / 100
        '            .ContentFractionDD = _ContentDDPercent.CurveSample(Randy.NextDouble)
        '            For i As Int32 = 0 To .ContentFractionDD.X.Count - 1
        '                .ContentFractionDD.Y(i) /= 100
        '            Next
        '        End If
        '        If _CalcOtherDamage Then
        '            .OtherValAsPcntOfMean = _OtherValAsPcntOfMean.getDistributedVariable(Randy.NextDouble) / 100
        '            .OtherFractionDD = _OtherDDPercent.CurveSample(Randy.NextDouble)
        '            For i As Int32 = 0 To .OtherFractionDD.X.Count - 1
        '                .OtherFractionDD.Y(i) /= 100
        '            Next
        '        End If
        '        If _CalcVehicleDamage Then
        '            .VehicleValAsPcntOfMean = _VehicleValAsPcntOfMean.getDistributedVariable(Randy.NextDouble) / 100
        '            .VehicleFractionDD = _VehicleDDPercent.CurveSample(Randy.NextDouble)
        '            For i As Int32 = 0 To .VehicleFractionDD.X.Count - 1
        '                .VehicleFractionDD.Y(i) /= 100
        '            Next
        '        End If
        '    End With
        '    Return result
        'End Function
        Public Function Clone() As OccupancyType
            Dim OccTypeCopy As New OccupancyType(_Name, _DamageCategory.Name)
            OccTypeCopy.CalcStructDamage = _CalcStructDamage
            OccTypeCopy.CalcContentDamage = _CalcContentDamage
            OccTypeCopy.CalcOtherDamage = _CalcOtherDamage
            OccTypeCopy.CalcVehicleDamage = _CalcVehicleDamage
            OccTypeCopy.DamageCategory = _DamageCategory 'for the other parameters.
            '
            OccTypeCopy.FoundationHeightUncertainty = _FndHeightAsPcntOfMean
            OccTypeCopy.StructureValueUncertainty = _StructureValAsPcntOfMean
            OccTypeCopy.ContentValueUncertainty = _ContentValAsPcntOfMean
            OccTypeCopy.OtherValueUncertainty = _OtherValAsPcntOfMean
            OccTypeCopy.VehicleValueUncertainty = _VehicleValAsPcntOfMean
            '
            If IsNothing(_StructureDDPercent) = False Then OccTypeCopy.SetStructurePercentDD = _StructureDDPercent.Clone
            '
            If IsNothing(_ContentDDPercent) = False Then OccTypeCopy.SetContentPercentDD = _ContentDDPercent.Clone
            '
            If IsNothing(_OtherDDPercent) = False Then OccTypeCopy.SetOtherPercentDD = _OtherDDPercent.Clone
            '
            If IsNothing(_VehicleDDPercent) = False Then OccTypeCopy.SetVehiclePercentDD = _VehicleDDPercent.Clone
            '
            Return OccTypeCopy
        End Function
        Public Function WriteToXElement() As XElement
            Dim OccElement As New XElement("OccupancyType")
            OccElement.SetAttributeValue("Name", _Name)
            '
            Dim description As New XElement("Description")
            description.Value = _Description '
            OccElement.Add(description)

            OccElement.Add(_DamageCategory.writetoXMlElement)

            Dim FndHeightUncertainty As New XElement("FoundationHeightUncertainty")
            FndHeightUncertainty.Add(_FndHeightAsPcntOfMean.writetoXElement)
            OccElement.Add(FndHeightUncertainty)
            '
            Dim StructUncertainty As New XElement("StructureUncertainty")
            StructUncertainty.Add(_StructureValAsPcntOfMean.writetoXElement)
            OccElement.Add(StructUncertainty)
            '
            Dim ContUncertainty As New XElement("ContentUncertainty")
            ContUncertainty.Add(_ContentValAsPcntOfMean.writetoXElement)
            OccElement.Add(ContUncertainty)
            '
            Dim OtherUncertainty As New XElement("OtherUncertainty")
            OtherUncertainty.Add(_OtherValAsPcntOfMean.writetoXElement)
            OccElement.Add(OtherUncertainty)
            '
            Dim VehicleUncertainty As New XElement("VehicleUncertainty")
            VehicleUncertainty.Add(_VehicleValAsPcntOfMean.writetoXElement)
            OccElement.Add(VehicleUncertainty)
            '
            Dim StructDD As New XElement("StructureDD")
            StructDD.SetAttributeValue("CalculateDamage", _CalcStructDamage)
            If IsNothing(_StructureDDPercent) = False Then
                If _StructureDDPercent.X.Count > 0 Then StructDD.Add(_StructureDDPercent.WriteToXElement)
            End If
            OccElement.Add(StructDD)

            Dim ContDD As New XElement("ContentDD")
            ContDD.SetAttributeValue("CalculateDamage", _CalcContentDamage)
            If IsNothing(_ContentDDPercent) = False Then
                If _ContentDDPercent.X.Count > 0 Then ContDD.Add(_ContentDDPercent.WriteToXElement)
            End If
            OccElement.Add(ContDD)

            Dim OthDD As New XElement("OtherDD")
            OthDD.SetAttributeValue("CalculateDamage", _CalcOtherDamage)
            If IsNothing(_OtherDDPercent) = False Then
                If _OtherDDPercent.X.Count > 0 Then OthDD.Add(_OtherDDPercent.WriteToXElement)
            End If
            OccElement.Add(OthDD)

            Dim MotorDD As New XElement("VehicleDD")
            MotorDD.SetAttributeValue("CalculateDamage", _CalcVehicleDamage)
            If IsNothing(_VehicleDDPercent) = False Then
                If _VehicleDDPercent.X.Count > 0 Then MotorDD.Add(_VehicleDDPercent.WriteToXElement)
            End If
            OccElement.Add(MotorDD)
            '
            Return OccElement
        End Function
    End Class


    '    Public Class OccupancyType
    '        Implements System.ComponentModel.INotifyPropertyChanged
    '        Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
    '        Private _Name As String
    '        Private _DamCat As DamageCategory
    '        Private _Stories As Int16
    '        Private _Dayhouseholds As Single
    '        Private _nightHouseholds As Single
    '        Private _Cars As Int16
    '        Private _carCarryCapacity As Single
    '#Region "Random Variables"
    '        Private _StructureDD As Statistics.MonotonicCurveUSingle
    '        Private _ContentDD As Statistics.MonotonicCurveUSingle
    '        Private _CarDD As Statistics.MonotonicCurveUSingle
    '        Private _FH As Statistics.ContinuousDistribution
    '        Private _compzonethreshold As Statistics.ContinuousDistribution
    '        Private _chancethreshold As Statistics.ContinuousDistribution
    '        Private _StructureValue As Statistics.ContinuousDistribution 'stdev as percent of the mean
    '        Private _ContentValue As Statistics.ContinuousDistribution  'stdev as percent of the mean
    '        Private _CarValue As Statistics.ContinuousDistribution 'stdev as percent of the mean
    '#End Region
    '#Region "Random Number Memory Allocations"
    '        Private _LossofLifeRandoms As Random
    '        Private _StructureDDRV As Double = 0.5
    '        Private _ContentDDRV As Double = 0.5
    '        Private _CarDDRV As Double = 0.5
    '        Private _FHRV As Double = 0.5
    '        Private _CompZoneThresholdRV As Double = 0.5
    '        Private _ChanceZoneThresholdRV As Double = 0.5
    '        Private _StructureValueRV As Double = 0.5
    '        Private _ContentValueRV As Double = 0.5
    '        Private _CarValueRV As Double = 0.5
    '#End Region
    '        'Private _HZ As HazardZone
    '        Sub New(ByVal name As String, ByVal Damcat As DamageCategory, ByVal Stories As Int16, ByVal Dayhouseholds As Single, ByVal nighthouseholds As Single, ByVal cars As Int16, ByVal carcarrycapacity As Single, ByVal sd As Statistics.MonotonicCurveUSingle, ByVal cond As Statistics.MonotonicCurveUSingle, ByVal card As Statistics.MonotonicCurveUSingle, ByVal FH As Statistics.ContinuousDistribution, ByVal compromizedThreshold As Statistics.ContinuousDistribution, ByVal chancezonethreshold As Statistics.ContinuousDistribution, ByVal sv As Statistics.ContinuousDistribution, ByVal conv As Statistics.ContinuousDistribution, ByVal carv As Statistics.ContinuousDistribution)
    '            _Name = name
    '            _DamCat = Damcat
    '            _Stories = Stories
    '            _Dayhouseholds = Dayhouseholds
    '            _nightHouseholds = nighthouseholds
    '            _Cars = cars
    '            _carCarryCapacity = carcarrycapacity
    '            _StructureDD = sd
    '            _ContentDD = cond
    '            _CarDD = card
    '            _FH = FH
    '            _compzonethreshold = compromizedThreshold
    '            _chancethreshold = chancezonethreshold
    '            _StructureValue = sv
    '            _ContentValue = conv
    '            _CarValue = carv
    '            _LossofLifeRandoms = New Random 'needs to be seeded or something
    '            '_HZ = New HazardZone(True) 'fix this later
    '        End Sub
    '        Sub New()
    '            'empty constructor for reflection
    '        End Sub
    '        ''' <summary>
    '        ''' 
    '        ''' </summary>
    '        ''' <value></value>
    '        ''' <returns></returns>
    '        ''' <remarks></remarks>
    '        Public ReadOnly Property GetContentValue As Statistics.ContinuousDistribution
    '            Get
    '                Return _ContentValue
    '            End Get
    '        End Property
    '        Public WriteOnly Property SetContentValue As Statistics.ContinuousDistribution
    '            Set(value As Statistics.ContinuousDistribution)
    '                _ContentValue = value
    '            End Set
    '        End Property
    '        Public ReadOnly Property GetCarValue As Statistics.ContinuousDistribution
    '            Get
    '                Return _CarValue
    '            End Get
    '        End Property
    '        Public WriteOnly Property SetCarValue As Statistics.ContinuousDistribution
    '            Set(value As Statistics.ContinuousDistribution)
    '                _CarValue = value
    '            End Set
    '        End Property
    '        Public Property Name As String
    '            Get
    '                Return _Name
    '            End Get
    '            Set(value As String)
    '                _Name = value
    '                NotifyPropertyChanged("Name")
    '            End Set
    '        End Property
    '        Public WriteOnly Property SetName As String
    '            Set(value As String)
    '                _Name = value
    '            End Set
    '        End Property
    '        Public ReadOnly Property GetDamCat As DamageCategory
    '            Get
    '                Return _DamCat
    '            End Get
    '        End Property
    '        Public WriteOnly Property SetDamCat As DamageCategory
    '            Set(value As DamageCategory)
    '                _DamCat = value
    '            End Set
    '        End Property
    '        Public ReadOnly Property GetDayHouseholds As Single
    '            Get
    '                Return _Dayhouseholds
    '            End Get
    '        End Property
    '        Public WriteOnly Property SetDayHouseholds As Single
    '            Set(value As Single)
    '                _Dayhouseholds = value
    '            End Set
    '        End Property
    '        Public ReadOnly Property GetNightHouseholds As Single
    '            Get
    '                Return _nightHouseholds
    '            End Get
    '        End Property
    '        Public WriteOnly Property SetNightHouseholds As Single
    '            Set(value As Single)
    '                _nightHouseholds = value
    '            End Set
    '        End Property
    '        Public ReadOnly Property GetNumCars As Int16
    '            Get
    '                Return _Cars
    '            End Get
    '        End Property
    '        Public WriteOnly Property SetNumCars As Int16
    '            Set(value As Int16)
    '                _Cars = value
    '            End Set
    '        End Property
    '        Public ReadOnly Property GetCarCarryCapacity As Single
    '            Get
    '                Return _carCarryCapacity
    '            End Get
    '        End Property
    '        Public WriteOnly Property SetCarCarryCapacity As Single
    '            Set(value As Single)
    '                _carCarryCapacity = value
    '            End Set
    '        End Property
    '        Public ReadOnly Property GetStructureDamageCurve As Statistics.MonotonicCurveUSingle
    '            Get
    '                Return _StructureDD
    '            End Get
    '        End Property
    '        Public WriteOnly Property SetStructureDamageCurve As Statistics.MonotonicCurveUSingle
    '            Set(value As Statistics.MonotonicCurveUSingle)
    '                _StructureDD = value
    '            End Set
    '        End Property
    '        Public ReadOnly Property GetContentDamageCurve As Statistics.MonotonicCurveUSingle
    '            Get
    '                Return _ContentDD
    '            End Get
    '        End Property
    '        Public WriteOnly Property SetContentDamageCurve As Statistics.MonotonicCurveUSingle
    '            Set(value As Statistics.MonotonicCurveUSingle)
    '                _ContentDD = value
    '            End Set
    '        End Property
    '        Public ReadOnly Property GetCarDamageCurve As Statistics.MonotonicCurveUSingle
    '            Get
    '                Return _CarDD
    '            End Get
    '        End Property
    '        Public WriteOnly Property SetCarDamageCurve As Statistics.MonotonicCurveUSingle
    '            Set(value As Statistics.MonotonicCurveUSingle)
    '                _CarDD = value
    '            End Set
    '        End Property
    '        Public ReadOnly Property GetChanceZoneThreshold As Double
    '            Get
    '                Return _chancethreshold.GetCentralTendency
    '            End Get
    '        End Property
    '        Public WriteOnly Property SetChanceZoneThreshold As Statistics.ContinuousDistribution
    '            Set(value As Statistics.ContinuousDistribution)
    '                _chancethreshold = value
    '            End Set
    '        End Property
    '        Public ReadOnly Property GetCompZoneThreshold As Double
    '            Get
    '                Return _compzonethreshold.GetCentralTendency
    '            End Get
    '        End Property
    '        Public WriteOnly Property SetCompZoneThreshold As Statistics.ContinuousDistribution
    '            Set(value As Statistics.ContinuousDistribution)
    '                _compzonethreshold = value
    '            End Set
    '        End Property
    '        Public ReadOnly Property GetFoundationHeight As Double
    '            Get
    '                Return _FH.getDistributedVariable(_FHRV)
    '            End Get
    '        End Property
    '        Public WriteOnly Property SetFoundationHeight As Statistics.ContinuousDistribution
    '            Set(value As Statistics.ContinuousDistribution)
    '                _FH = value
    '            End Set
    '        End Property
    '        Public ReadOnly Property GetStructureDDFunctionMin As Double
    '            Get
    '                Return _StructureDD.X(0)
    '            End Get
    '        End Property
    '        Public ReadOnly Property GetContentDDFunctionMin As Double
    '            Get
    '                Return _ContentDD.X(0)
    '            End Get
    '        End Property
    '        Public ReadOnly Property GetCarDDFunctionMin As Double
    '            Get
    '                Return _CarDD.X(0)
    '            End Get
    '        End Property
    '        Private Sub NotifyPropertyChanged(ByVal info As String)
    '            RaiseEvent PropertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(info))
    '        End Sub
    '        ''' <summary>
    '        ''' returns a structure value, it is the value passed if there is no uncertianty defined, returns a sampled value based of the distance from the mean...
    '        ''' </summary>
    '        ''' <param name="StructureValue"></param>
    '        ''' <value></value>
    '        ''' <returns></returns>
    '        ''' <remarks></remarks>
    '        Public ReadOnly Property GetStructureValue(Optional ByVal StructureValue As Double = Nothing) As Double
    '            Get 'this may need some work.
    '                If _StructureValue.GetType = GetType(Statistics.None) Then
    '                    If IsNothing(StructureValue) Then Return _StructureValue.GetCentralTendency
    '                    Return StructureValue
    '                Else
    '                    If IsNothing(StructureValue) Then Throw New Exception("The Structure Value is not defined at this structure")
    '                    Return StructureValue + (_StructureValue.getDistributedVariable(_StructureValueRV) * StructureValue)
    '                End If
    '            End Get
    '        End Property
    '        ''' <summary>
    '        ''' sets the structure value as a continuous distribution, should be a mean of zero with a standard deviation as a percentage of the mean
    '        ''' </summary>
    '        ''' <value></value>
    '        ''' <remarks></remarks>
    '        Public WriteOnly Property SetStructureValue As Statistics.ContinuousDistribution
    '            Set(value As Statistics.ContinuousDistribution)
    '                _StructureValue = value
    '            End Set
    '        End Property
    '        ''' <summary>
    '        ''' returns the content value as a deterministic value or as a distance from the mean
    '        ''' </summary>
    '        ''' <param name="ContentValue"></param>
    '        ''' <value></value>
    '        ''' <returns></returns>
    '        ''' <remarks></remarks>
    '        Public ReadOnly Property GetContV(Optional ByVal ContentValue As Double = Nothing) As Double
    '            Get 'this may need some work.
    '                If _ContentValue.GetType = GetType(Statistics.None) Then 'if not a distributed variable
    '                    If IsNothing(ContentValue) Then Return _ContentValue.GetCentralTendency
    '                    Return ContentValue
    '                Else
    '                    If IsNothing(ContentValue) Then Throw New Exception("The Content Value is not defined at this structure")
    '                    Return ContentValue + (_ContentValue.getDistributedVariable(_ContentValueRV) * ContentValue) 'need to determine which type of uncertianty to use
    '                End If
    '            End Get
    '        End Property
    '        ''' <summary>
    '        ''' gets car value based off of a random distance from the central tendency or by returning a fixed value
    '        ''' </summary>
    '        ''' <param name="CarValue"></param>
    '        ''' <value></value>
    '        ''' <returns></returns>
    '        ''' <remarks></remarks>
    '        Public ReadOnly Property GetCarV(Optional ByVal CarValue As Double = Nothing) As Double
    '            Get 'this may need some work.
    '                If _CarValue.GetType = GetType(Statistics.None) Then
    '                    If IsNothing(CarValue) Then Return _StructureValue.GetCentralTendency
    '                    Return CarValue
    '                Else
    '                    If IsNothing(CarValue) Then Throw New Exception("The Car Value is not defined at this structure")
    '                    Return CarValue + (_CarValue.getDistributedVariable(_CarValueRV) * CarValue) 'need to determine which type of uncertianty to use
    '                End If
    '            End Get
    '        End Property
    '        Public WriteOnly Property SetRandomVariables() As Double()
    '            Set(RVs As Double())
    '                _StructureDDRV = RVs(0)
    '                _ContentDDRV = RVs(1)
    '                _CarDDRV = RVs(2)
    '                _FHRV = RVs(3)
    '                _CompZoneThresholdRV = RVs(4)
    '                _ChanceZoneThresholdRV = RVs(5)
    '                _StructureValueRV = RVs(6)
    '                _ContentValueRV = RVs(7)
    '                _CarValueRV = RVs(8)
    '            End Set
    '        End Property
    '        ' ''' <summary>
    '        ' ''' calculates the fatality rate at the structure based on depth or depth times velocity.  uses 3 random numbers regardless of how the function is called.  if it is a deterministic compute the fatality rate curves are only one value
    '        ' ''' </summary>
    '        ' ''' <param name="H">the hydraulic event</param>
    '        ' ''' <param name="fh">the foundation height of the structure in question</param>
    '        ' ''' <param name="totaldamage">a boolean determining if there was total damage or not</param>
    '        ' ''' <param name="partaldamage">a boolean determining if there was partal damage or not</param>
    '        ' ''' <returns></returns>
    '        ' ''' <remarks></remarks>
    '        'Public Function calculateFatalityrate(H As HYDRAULIC_EVENT, fh As Double, totaldamage As Boolean, partaldamage As Boolean) As Double
    '        'Throw New NotImplementedException
    '        'Dim compzone As Double = _compzonethreshold.getDistributedVariable(_CompZoneThresholdRV) + fh 'need to determine what type of uncertianty to use
    '        'Dim chancezone As Double = _chancethreshold.getDistributedVariable(_ChanceZoneThresholdRV) + fh
    '        ''spin off the same number of randoms every time for consistency
    '        'Dim nv1 As Double = _LossofLifeRandoms.NextDouble
    '        'Dim nv2 As Double = _LossofLifeRandoms.NextDouble
    '        'Dim nv3 As Double = _LossofLifeRandoms.NextDouble
    '        'Dim d As Double = H.getdepth
    '        'If totaldamage = True Then Return _HZ.GetChanceFatalityRate(nv3)

    '        'If partaldamage = True Then
    '        '    If d <= Math.Min(2, fh) Then
    '        '        Return 0
    '        '    ElseIf d <= chancezone Then
    '        '        Return _HZ.GetCompromizedFatalityRate(nv2)
    '        '    Else
    '        '        Return _HZ.GetChanceFatalityRate(nv3)
    '        '    End If
    '        'End If
    '        'If d <= Math.Min(2, fh) Then
    '        '    Return 0
    '        'ElseIf d <= compzone Then
    '        '    Return _HZ.GetSafeFatalityRate(nv1)
    '        'ElseIf d <= chancezone Then
    '        '    Return _HZ.GetCompromizedFatalityRate(nv2)
    '        'Else
    '        '    Return _HZ.GetChanceFatalityRate(nv3)
    '        'End If
    '        'Throw New Exception("Somehow no fatality rate was calculated")
    '        'Return Nothing
    '        'End Function
    '        ' ''' <summary>
    '        ' ''' calculates the percentage loss to a structure based on depth above first floor elevation, returns values between zero and 1.
    '        ' ''' </summary>
    '        ' ''' <param name="Stage"></param>
    '        ' ''' <returns></returns>
    '        ' ''' <remarks></remarks>
    '        'Public Function StructurePercentDamage(Stage As Double) As Double
    '        '    Return _StructureDD.sampleXaxis(Stage, _StructureDDRV) / 100 'update to select the right uncertianty type..
    '        'End Function
    '        ' ''' <summary>
    '        ' ''' returns content damages as a percent based on stage above first floor elevation, values are between zero and 1
    '        ' ''' </summary>
    '        ' ''' <param name="Stage">feet above first floor elevation</param>
    '        ' ''' <returns></returns>
    '        ' ''' <remarks></remarks>
    '        'Public Function ContentPercentDamage(Stage As Double) As Double
    '        '    Return _ContentDD.SampleXAxis(Stage, _ContentDDRV) / 100 'update to select the right uncertianty type..
    '        'End Function
    '        ' ''' <summary>
    '        ' ''' calculates car damage as a percent based on stage above first floor elevation returns values between zero and 1
    '        ' ''' </summary>
    '        ' ''' <param name="Stage">feet above first floor elevation</param>
    '        ' ''' <returns></returns>
    '        ' ''' <remarks></remarks>
    '        'Public Function CarPercentDamage(Stage As Double) As Double
    '        '    Return _CarDD.SampleXAxis(Stage, _CarDDRV) / 100 'update to select the right uncertianty type..
    '        'End Function
    '    End Class
End Namespace
