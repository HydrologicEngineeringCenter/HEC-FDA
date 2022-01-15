using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compute
{
    public class LeveeMessage : Base.Interfaces.IMessage
{
    private double _topOfLeveeElevation;
    public string Message
    {
        get
        {
                return $"The top of levee elevation of {_topOfLeveeElevation} in the fragility function does not have a certain probability of failure";
        }
    }
    public double TopOfLeveeElevation
    {
        get
        {
            return _topOfLeveeElevation;
        }
    }
    public LeveeMessage(double topOfLeveeElevation)
    {
            _topOfLeveeElevation = topOfLeveeElevation;
    }
}
}

