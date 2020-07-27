using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model 
{ 

    public static class ILateralStructureFactory
    {
        public static ILateralStructure Factory(double top, IFdaFunction failureFx = null, UnitsEnum units = UnitsEnum.Foot, string label = "")
        {
            IFdaFunction fx = failureFx.IsNull() ? IFdaFunctionFactory.Factory(top): failureFx;
            return new Conditions.Locations.LateralStructures.LateralStructure(IElevationFactory.Factory(top, units, IParameterEnum.ExteriorElevation), fx, units, label);
        }
        public static ILateralStructure Factory(double top, double bottom = double.NaN, UnitsEnum units = UnitsEnum.Foot, string label = "")
        {
            IFdaFunction fx = IFdaFunctionFactory.Factory(top, bottom, units);
            return new Conditions.Locations.LateralStructures.LateralStructure(IElevationFactory.Factory(top, units, IParameterEnum.ExteriorElevation), fx, units, label);
        }
        internal static ILateralStructure Factory(IElevation top, IFdaFunction failureFx, UnitsEnum units, string label)
        {
            return new Conditions.Locations.LateralStructures.LateralStructure(top, failureFx, units, label);
        }
    }
}
