using HEC.FDA.ViewModel.Utilities;
using System;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.LifeLoss;
public class StageLifeLossElement : ChildElement, IHaveStudyFiles
{
    public StageLifeLossElement() : base("", "", "", 0) { }
    public override XElement ToXML()
    {
        throw new NotImplementedException();
    }
}

