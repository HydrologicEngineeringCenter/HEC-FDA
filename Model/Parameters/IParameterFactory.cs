using Functions;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model
{
    /// <summary>
    /// Methods for constructing objects implementing the <see cref="IParameter"/> interface.
    /// </summary>
    public class IParameterFactory
    {
        //internal static IParameter Factory(IFdaFunction fx, bool x, UnitsEnum units = UnitsEnum.NotSet, string label = "")
        //{
        //    switch (fx.ParameterT        //            if (x) throw new NotImplementedException();
        //    {
        //        case IParameterEnum.LateralStructureElevation:
        //            //return new Parameters.Series.ElevationSeries(fx, true, IParameterEnum.ExteriorElevation, units == UnitsEnum.NotSet ? UnitsEnum.Foot : units, label);
        //            else
        //                return new Parameters.Series.FrequencySeries(fx, false, IParameterEnum.FailureProbability, label);
        //        case IParameterEnum.InflowFrequency:
        //            return x ? new Parameters.Series.FrequencySeries(fx, true, IParameterEnum.InflowFrequency, label) :
        //                throw new NotImplementedException();
        //        default:
        //            throw new NotImplementedException();
        //    };
        //}
        internal static IParameterRange Factory(IFunction fx, IParameterEnum fType, bool isConstant, bool x = true, UnitsEnum units = UnitsEnum.NotSet, string label = "", bool abbreviate = true)
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
                    case IParameterEnum.NonExceedanceProbability:
                    case IParameterEnum.FailureProbability:
                       
                        // probability / frequency.
                        return new Parameters.Probabilities.Probability(fx.Domain, isConstant: true, IParameterEnum.NonExceedanceProbability, units, label, abbreviate);
                    case IParameterEnum.InflowOutflow:
                    case IParameterEnum.UnregulatedAnnualPeakFlow:
                        //flow
                        return new Parameters.Flows.Flow(fx.Domain, isConstant: true, IParameterEnum.UnregulatedAnnualPeakFlow, units, label, abbreviate);
                    case IParameterEnum.Rating:
                    case IParameterEnum.RegulatedAnnualPeakFlow:
                        // flow
                        return new Parameters.Flows.Flow(fx.Domain, isConstant: true, IParameterEnum.RegulatedAnnualPeakFlow, units, label, abbreviate);
                    case IParameterEnum.ExteriorInteriorStage:
                    //case IParameterEnum.LateralStructureFailure:
                    case IParameterEnum.ExteriorElevation:
                        return new Parameters.Elevations.ElevationRange(fx.Domain, isConstant: true, IParameterEnum.ExteriorElevation, units == UnitsEnum.NotSet ? IParameterEnum.ExteriorElevation.UnitsDefault() : units, label, abbreviate);
                    case IParameterEnum.InteriorStageDamage:
                    case IParameterEnum.InteriorElevation:
                        return new Parameters.Elevations.ElevationRange(fx.Domain, isConstant: true, IParameterEnum.InteriorElevation, units == UnitsEnum.NotSet ? IParameterEnum.InteriorElevation.UnitsDefault() : units, label, abbreviate);
                    case IParameterEnum.YearExteriorStageAEP:
                    case IParameterEnum.YearInteriorStageAEP:
                    case IParameterEnum.YearDamageAEP:
                    case IParameterEnum.YearEAD:
                        return new Parameters.Years.Year(range: IRangeFactory.Factory((int)fx.Domain.Min, (int)fx.Domain.Max, true, true, false), isConstant: true, IParameterEnum.Year, label, abbreviate);
                    default:
                        throw new ArgumentOutOfRangeException($"The specified parameter type: {fType.Print()} is not one of the required a {typeof(IFdaFunction)} {typeof(IParameterEnum)} types.");
                }
            }
            else //the Y axis
            {
                switch (fType) 
                {
                    case IParameterEnum.InflowFrequency:
                    case IParameterEnum.UnregulatedAnnualPeakFlow:
                        // flow.
                        return new Parameters.Flows.Flow(fx.Range, isConstant, IParameterEnum.UnregulatedAnnualPeakFlow, units, label, abbreviate);
                    case IParameterEnum.InflowOutflow:
                    case IParameterEnum.OutflowFrequency:
                    case IParameterEnum.RegulatedAnnualPeakFlow:
                        // flow.
                        return new Parameters.Flows.Flow(fx.Range, isConstant, IParameterEnum.RegulatedAnnualPeakFlow, units, label, abbreviate);
                    case IParameterEnum.ExteriorStageFrequency:
                    case IParameterEnum.Rating:
                    case IParameterEnum.ExteriorElevation:
                        return new Parameters.Elevations.ElevationRange(fx.Range, isConstant, IParameterEnum.ExteriorElevation, units == UnitsEnum.NotSet ? IParameterEnum.ExteriorElevation.UnitsDefault() : units, label, abbreviate);
                    case IParameterEnum.ExteriorInteriorStage:
                    case IParameterEnum.InteriorStageFrequency:
                    case IParameterEnum.InteriorElevation:
                        return new Parameters.Elevations.ElevationRange(fx.Range, isConstant, IParameterEnum.InteriorElevation, units == UnitsEnum.NotSet ? IParameterEnum.InteriorElevation.UnitsDefault() : units, label, abbreviate);
                    case IParameterEnum.InteriorStageDamage:
                    case IParameterEnum.DamageFrequency:
                        // damage.
                        return new Parameters.Damages.Damage(fx.Range, true, IParameterEnum.FloodDamages, units, label, abbreviate);
                    case IParameterEnum.LateralStructureFailure:
                    case IParameterEnum.FailureProbability:
                        // probability / frequency.
                        return new Parameters.Probabilities.Probability(fx.Range, isConstant, fType, units, label, abbreviate);
                    case IParameterEnum.FloodDamages:
                        return new Parameters.Damages.Damage(fx.Range, isConstant, fType, units, label, abbreviate);
                    default:
                        throw new ArgumentOutOfRangeException($"The specified parameter type: {fType.Print()} is not one of the required a {typeof(IFdaFunction)} {typeof(IParameterEnum)} types.");
                }
            }
        }
        internal static IParameterOrdinate Factory(double ordinate, IParameterEnum type, UnitsEnum units = UnitsEnum.NotSet, string label = "", bool abbreviatedLabel = true)
        {
            switch (type) 
            {
                case IParameterEnum.LateralStructureElevation:
                    units = units == UnitsEnum.NotSet ? UnitsEnum.Foot : units;
                    return new Model.Parameters.Elevations.ElevationOrdinate(IOrdinateFactory.Factory(ordinate), type, units, label, abbreviatedLabel);
                default:
                    throw new NotImplementedException();
            }

        }

        //internal static IParameter Factory(IFunction fx, IParameterEnum fType, bool x = true, UnitsEnum units = UnitsEnum.NotSet, string label = "")
        //{
        //    if (x)
        //    {
        //        switch (fType)
        //        {
        //            case IParameterEnum.InflowFrequency:
        //            case IParameterEnum.OutflowFrequency:
        //            case IParameterEnum.ExteriorStageFrequency:
        //            case IParameterEnum.InteriorStageFrequency:
        //            case IParameterEnum.DamageFrequency:
        //                // frequency.
        //                throw new NotImplementedException();
        //            case IParameterEnum.InflowOutflow:
        //            case IParameterEnum.Rating:
        //                // flow.
        //                throw new NotImplementedException();
        //            case IParameterEnum.ExteriorInteriorStage:
        //            case IParameterEnum.LateralStructureFailure:
        //                return new Parameters.Elevations.Elevation(fx, IParameterEnum.ExteriorElevation, label, units == UnitsEnum.NotSet ? IParameterEnum.ExteriorElevation.UnitsDefault() : units);
        //            case IParameterEnum.InteriorStageDamage:
        //                return new Parameters.Elevations.Elevation(fx, IParameterEnum.InteriorElevation, label, units == UnitsEnum.NotSet ? IParameterEnum.InteriorElevation.UnitsDefault() : units);
        //            default:
        //                throw new ArgumentOutOfRangeException($"The specified parameter type: {fType.Print()} is not one of the required a {typeof(IFdaFunction)} {typeof(IParameterEnum)} types.");
        //        }
        //    }
        //    else //the Y axis
        //    {
        //        switch (fType)
        //        {
        //            case IParameterEnum.InflowFrequency:
        //            case IParameterEnum.InflowOutflow:
        //            case IParameterEnum.OutflowFrequency:
        //                // flow.
        //                throw new NotImplementedException();
        //            case IParameterEnum.ExteriorStageFrequency:
        //            case IParameterEnum.Rating:
        //                return new Parameters.Elevations.Elevation(fx, IParameterEnum.ExteriorElevation, label, units == UnitsEnum.NotSet ? IParameterEnum.ExteriorElevation.UnitsDefault() : units);
        //            case IParameterEnum.ExteriorInteriorStage:
        //            case IParameterEnum.InteriorStageFrequency:
        //                return new Parameters.Elevations.Elevation(fx, IParameterEnum.InteriorElevation, label, units == UnitsEnum.NotSet ? IParameterEnum.InteriorElevation.UnitsDefault() : units);
        //            case IParameterEnum.InteriorStageDamage:
        //            case IParameterEnum.DamageFrequency:
        //                // damage.
        //                throw new NotImplementedException();
        //            case IParameterEnum.LateralStructureFailure:
        //                // frequency.
        //                throw new NotImplementedException();
        //            default:
        //                throw new ArgumentOutOfRangeException($"The specified parameter type: {fType.Print()} is not one of the required a {typeof(IFdaFunction)} {typeof(IParameterEnum)} types.");
        //        }
        //    }
        //}

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
