namespace OccupancyTypes
{
    public static class OccupancyTypeFactory
    {
        public static IOccupancyType Factory(Consequences_Assist.ComputableObjects.OccupancyType ot)
        {
            IOccupancyType occupancyType = new OccupancyType();
            //occupancyType.Name = ot.Name;
            //occupancyType.Description = ot.Description;

            //occupancyType.CalculateStructureDamage = ot.CalcStructDamage;
            //occupancyType.CalcualateContentDamage = ot.CalcContentDamage;
            //occupancyType.CalculateVehicleDamage = ot.CalcVehicleDamage;
            //occupancyType.CalculateOtherDamage = ot.CalcOtherDamage; 

            //occupancyType.StructureDepthDamageFunction = TranslateMonotonicCurveToCoordinatesFunction(ot.GetStructurePercentDD);
            //occupancyType.ContentDepthDamageFunction = TranslateMonotonicCurveToCoordinatesFunction(ot.GetContentPercentDD);
            //occupancyType.VehicleDepthDamageFunction = TranslateMonotonicCurveToCoordinatesFunction(ot.GetVehiclePercentDD);
            //occupancyType.OtherDepthDamageFunction = TranslateMonotonicCurveToCoordinatesFunction(ot.GetOtherPercentDD);

            //occupancyType.StructureValueUncertainty = TranslateContinuousDistributionToIDistributedOrdinate(ot.StructureValueUncertainty);
            //occupancyType.ContentValueUncertainty = TranslateContinuousDistributionToIDistributedOrdinate(ot.ContentValueUncertainty);
            //occupancyType.VehicleValueUncertainty = TranslateContinuousDistributionToIDistributedOrdinate(ot.VehicleValueUncertainty);
            //occupancyType.OtherValueUncertainty = TranslateContinuousDistributionToIDistributedOrdinate(ot.OtherValueUncertainty);
            //occupancyType.FoundationHeightUncertainty = TranslateContinuousDistributionToIDistributedOrdinate(ot.FoundationHeightUncertainty);

            //occupancyType.StructureDepthDamageName = ot.StructureDepthDamageName;
            //occupancyType.ContentDepthDamageName = ot.ContentDepthDamageName;
            //occupancyType.VehicleDepthDamageName = ot.VehicleDepthDamageName;
            //occupancyType.OtherDepthDamageName = ot.OtherDepthDamageName;

            return occupancyType;

        }

        public static IOccupancyType Factory(string name, string selectedDamageCategoryName)
        {
            return new OccupancyType(name, selectedDamageCategoryName);
        }

        public static IOccupancyType Factory()
        {
            return new OccupancyType();
        }

        //private static ICoordinatesFunction TranslateMonotonicCurveToCoordinatesFunction(Statistics.UncertainCurveIncreasing curve)
        //{
        //    List<double> xs = new List<double>();
        //    List<IDistributedOrdinate> ys = new List<IDistributedOrdinate>();

        //    foreach (float f in curve.XValues)
        //    {
        //        xs.Add(f);
        //    }
        //    foreach (ContinuousDistribution c in curve.YValues)
        //    {
        //        ys.Add(TranslateContinuousDistributionToIDistributedOrdinate(c));
        //    }
        //    return ICoordinatesFunctionsFactory.Factory(xs,ys,Functions.CoordinatesFunctions.InterpolationEnum.Linear);
        //}

        //private static IDistributedOrdinate TranslateContinuousDistributionToIDistributedOrdinate(ContinuousDistribution dist)
        //{
        //    IDistributedOrdinate ord = IDistributedOrdinateFactory.FactoryNormal(0, 0);
            
        //    Type distType = dist.GetType();
        //    if(distType == typeof(None))
        //    {
        //        //IDistributedOrdinate ord = IDistributedOrdinateFactory.fac
        //        //todo: for now if it is null then it is "none" or "constant"
        //        return null;
        //    }
        //    else if(distType == typeof(Normal))
        //    {
        //        double stDev = ((Normal)dist).GetStDev;
        //        ord = IDistributedOrdinateFactory.FactoryNormal(0, stDev);
        //    }
        //    else if(distType == typeof(Triangular))
        //    {
        //        double min = ((Triangular)dist).getMin;
        //        double max = ((Triangular)dist).getMin;
        //        ord = IDistributedOrdinateFactory.FactoryTriangular(0, min, max);
        //    }
        //    else if(distType == typeof(Uniform))
        //    {
        //        double min = ((Triangular)dist).getMin;
        //        double max = ((Triangular)dist).getMin;
        //        ord = IDistributedOrdinateFactory.FactoryUniform(min, max);
        //    }

        //    return ord;

        //}



    }
}
