using System;
using System.Collections.Generic;
using System.Text;

using Functions;

namespace Model.Conditions.Locations.LateralStructures
{
    internal class LateralStructure: ILateralStructure
    {
        public IElevation TopElevation { get; }
        public IFdaFunction FailureFunction { get; }

        internal LateralStructure(double elevation, UnitsEnum units = UnitsEnum.Foot)
        {
            TopElevation = IElevationFactory.Factory(elevation, units, IParameterEnum.LateralStructureElevation);
            FailureFunction = IFdaFunctionFactory.Factory(elevation, bottomElevation: double.NaN, units);
        }
        internal LateralStructure(IElevation elevation, FailureFunction fx)
        {
            TopElevation = elevation;
            FailureFunction = fx;
        }
    }
}
