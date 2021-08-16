Public MustInherit Class PairedData
    Public MustOverride Function Verify() As ErrorReport
    Public MustOverride Sub ReadFromXelement(ele As XElement)
    Public MustOverride Function WriteToXElement() As XElement
    Public MustOverride Function Clone() As PairedData
End Class
