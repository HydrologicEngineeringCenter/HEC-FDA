using HEC.FDA.Model.metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ModelTest.unittests.metrics;

[Trait("RunsOn", "Remote")]
public class ThresholdShould
{
    [Fact]
    public void ReadAndWriteToXML()
    {
        Threshold threshold = new Threshold(0,new Statistics.ConvergenceCriteria(),ThresholdEnum.TopOfLevee,999);
        XElement ele = threshold.WriteToXML();
        Assert.NotNull(ele);
        Threshold newThresh = Threshold.ReadFromXML(ele);
        Assert.True(newThresh.Equals(threshold));
    }
}
