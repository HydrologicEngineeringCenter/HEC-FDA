using System;
using System.Collections.Generic;
using System.Text;

using Functions;

namespace Model.ComputePoint
{
    internal class LateralStructure: ILateralStructure
    {
        public IElevation<IOrdinate> TopElevation { get; }
        public IFdaFunction FailureFunction { get; }

        internal LateralStructure(double elevation, UnitsEnum units)
        {
            TopElevation = IElevationFactory.Factory(elevation, units, IParameterEnum.LateralStructureElevation);
        }
    }
}
