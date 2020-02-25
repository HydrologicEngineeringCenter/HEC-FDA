Namespace ComputableObjects
    Public Class DamageCategory
        Private _Name As String = "" ' the Name of the damage category
        Private _Description As String = " "
        Private _Rebuild_Period As Int16 = 365 ' how many days it takes to be rebuilt from a previous flood
        Private _CostFactor As Double = 1
        Sub New(ByVal name As String, ByVal description As String, ByVal rebuildperiod As Int16, ByVal costfactor As Double)
            _Name = name
            _Description = description
            _Rebuild_Period = rebuildperiod
            _CostFactor = costfactor
        End Sub
        Sub New(ByVal name As String)
            _Name = name
        End Sub
        Sub New()

        End Sub
        Sub New(ByVal xel As XElement)
            readfromXML(xel)
        End Sub
        ''' <summary>
        ''' This function returns the number of days required to rebuild structures of this occupancy type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GetRebuildPeriod
            Get
                Return _Rebuild_Period
            End Get
        End Property
        Public Property Name As String
            Get
                Return _Name
            End Get
            Set(value As String)
                _Name = value
            End Set
        End Property
        Public ReadOnly Property GetCostFactor As Double
            Get
                Return _CostFactor
            End Get
        End Property
        Public ReadOnly Property GetDescription As String
            Get
                Return _Description
            End Get
        End Property
        Public Function CalculateNewValue(ByVal Value As Double, ByVal percentdamage As Double, ByVal lastdamagedate As DateTime, ByVal currentdamagedate As DateTime) As Double
            Dim ts As TimeSpan = currentdamagedate - lastdamagedate
            Dim adjustedvalue As Double = Value
            Dim rebuildprecent As Double = ts.Days / _Rebuild_Period
            If 1 - percentdamage <= rebuildprecent Then
                'there is more rebuilding to do
                adjustedvalue = adjustedvalue * (percentdamage + rebuildprecent) 'assumes linear rebuild

            Else
                'there isnt
            End If
            Return adjustedvalue
        End Function
        Function writetoXMlElement() As XElement
            Dim result As New XElement("DamageCategory")
            Dim Name As New XElement("Name", _Name)
            Dim Description As New XElement("Description", _Description)
            Dim Rebuild As New XElement("Rebuild", _Rebuild_Period)
            Dim Costfactor As New XElement("CostFactor", _CostFactor)
            result.Add(Name)
            result.Add(Description)
            result.Add(Rebuild)
            result.Add(Costfactor)
            Return result
        End Function
        Sub readfromXML(ByVal el As XElement)
            _Name = el.Element("Name")
            _Description = el.Element("Description")
            _Rebuild_Period = CInt(el.Element("Rebuild"))
            _CostFactor = CDbl(el.Element("CostFactor"))
        End Sub
        Public Overrides Function Equals(obj As Object) As Boolean
            Dim other As DamageCategory = TryCast(obj, DamageCategory)
            If IsNothing(other) Then Return False
            Return (other.Name = Name AndAlso other.GetDescription = GetDescription AndAlso other.GetRebuildPeriod = GetRebuildPeriod AndAlso other.GetCostFactor = GetCostFactor)
        End Function
        Function WriteToFDAString() As String
            Return _Name & vbTab & _Description & vbTab & _CostFactor & vbTab 'why the extra vb tab?
        End Function
    End Class
End Namespace

