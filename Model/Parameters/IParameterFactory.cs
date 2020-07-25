using Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class IParameterFactory
    {
        internal static IParameter Factory(IFdaFunction fx, bool x, UnitsEnum units = UnitsEnum.NotSet, string label = "")
        {
            switch (fx.ParameterType)
            {
                case IParameterEnum.LateralStructureElevation:
                    if (x) throw new NotImplementedException();
                    //return new Parameters.Series.ElevationSeries(fx, true, IParameterEnum.ExteriorElevation, units == UnitsEnum.NotSet ? UnitsEnum.Foot : units, label);
                    else
                        return new Parameters.Series.FrequencySeries(fx, false, IParameterEnum.FailureProbability, label);
                case IParameterEnum.InflowFrequency:
                    return x ? new Parameters.Series.FrequencySeries(fx, true, IParameterEnum.InflowFrequency, label) :
                        throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            };
        }
        internal static IParameter Factory(IFunction fx, IParameterEnum fType, bool x = true, UnitsEnum units = UnitsEnum.NotSet, string label = "")
        {
            if (x)
            {
                switch (fType) 
                {
                    case IParameterEnum.InflowFrequency:
                    case IParameterEnum.OutflowFrequency:
                    case IParameterEnum.ExteriorStageFrequency:
                    case IParameterEnum.InteriorStageFrequency:
                    case IParameterEnum.DamageFrequency:
                        // frequency.
                        throw new NotImplementedException();
                    case IParameterEnum.InflowOutflow:
                    case IParameterEnum.Rating:
                        // flow.
                        throw new NotImplementedException();
                    case IParameterEnum.ExteriorInteriorStage:
                    case IParameterEnum.LateralStructureFailure:
                        return new Parameters.Elevations.Elevation(fx, IParameterEnum.ExteriorElevation, label, units == UnitsEnum.NotSet ? IParameterEnum.ExteriorElevation.DefaultUnits() : units);
                    case IParameterEnum.InteriorStageDamage:
                        return new Parameters.Elevations.Elevation(fx, IParameterEnum.InteriorElevation, label, units == UnitsEnum.NotSet ? IParameterEnum.InteriorElevation.DefaultUnits() : units);
                    default:
                        throw new ArgumentOutOfRangeException($"The specified parameter type: {fType.Print()} is not one of the required a {typeof(IFdaFunction)} {typeof(IParameterEnum)} types.");
                }
            }
            else //the Y axis
            {
                switch (fType) 
                {
                    case IParameterEnum.InflowFrequency:
                    case IParameterEnum.InflowOutflow:
                    case IParameterEnum.OutflowFrequency:
                        // flow.
                        throw new NotImplementedException();
                    case IParameterEnum.ExteriorStageFrequency:
                    case IParameterEnum.Rating:
                        return new Parameters.Elevations.Elevation(fx, IParameterEnum.ExteriorElevation, label, units == UnitsEnum.NotSet ? IParameterEnum.ExteriorElevation.DefaultUnits() : units);
                    case IParameterEnum.ExteriorInteriorStage:
                    case IParameterEnum.InteriorStageFrequency:
                        return new Parameters.Elevations.Elevation(fx, IParameterEnum.InteriorElevation, label, units == UnitsEnum.NotSet ? IParameterEnum.InteriorElevation.DefaultUnits() : units);
                    case IParameterEnum.InteriorStageDamage:
                    case IParameterEnum.DamageFrequency:
                        // damage.
                        throw new NotImplementedException();
                    case IParameterEnum.LateralStructureFailure:
                        // frequency.
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentOutOfRangeException($"The specified parameter type: {fType.Print()} is not one of the required a {typeof(IFdaFunction)} {typeof(IParameterEnum)} types.");
                }
            }
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
