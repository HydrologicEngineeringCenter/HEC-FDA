using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Inputs.Functions.ImpactAreaFunctions
{
    public interface IStageDamageTransform: ITransformFunction
    {
        //IStageDamageTransform Aggregate(IStageDamageTransform toAdd);
        double GetYFromX(double x);
    }
}
