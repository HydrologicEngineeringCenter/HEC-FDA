using HEC.FDA.Model.structures;
using Xunit;

namespace HEC.FDA.ModelTest.unittests;
public class RASHelperShould
{
    [Fact]
    public void InvalidateInvalidShapefile()
    {
        string erorr="";
        bool valid = RASHelper.ShapefileIsValid(Resources.StringResourcePaths.pathToIAShapefileNODBF, ref erorr);
        Assert.False(valid);
    }
}
