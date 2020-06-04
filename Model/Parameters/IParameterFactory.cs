using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class IParameterFactory
    {
        internal static IParameterSeries Factory(IFdaFunction fx, bool x, UnitsEnum units = UnitsEnum.NotSet, string label = "")
        {
            switch (fx.ParameterType) 
            {                
                case IParameterEnum.LateralStructureElevation:
                    if (x) 
                        return new Parameters.Series.ElevationSeries(fx, true, IParameterEnum.ExteriorElevation, units == UnitsEnum.NotSet ? UnitsEnum.Foot : units, label);
                    else 
                        return new Parameters.Series.FrequencySeries(fx, false, IParameterEnum.FailureProbability, label);                
                case IParameterEnum.InflowFrequency:
                    return x ? new Parameters.Series.FrequencySeries(fx, true, IParameterEnum.InflowFrequency, label) :
                        throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            };
        } 
        
        //public static IFdaParameter<IFdaOrdinate> Factory(double value, UnitsEnum units = UnitsEnum.NotSet,
        //    IFdaParameterEnum parameterEnum = IFdaParameterEnum.NotSet)
        //{
        //    IFdaParameter<IFdaOrdinate> parameter;
        //    IFdaOrdinate ordinate = IFdaOrdinateFactory.Factory(value);
        //    switch (parameterEnum) 
        //    {
        //        case IFdaParameterEnum.NotSet:
        //            if (units == UnitsEnum.NotSet) para
        //        case IFdaParameterEnum.Ground:
        //        case IFdaParameterEnum.AssetHeight:
        //        case IFdaParameterEnum.AssetElevation:
        //        default:
        //            throw new NotImplementedException();
        //    }

            

        //}
    }
}
