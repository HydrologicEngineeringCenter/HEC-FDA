using Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Model.Conditions.Locations
{
    internal sealed class ConditionLocationRealization : IConditionLocationRealization
    {
        public IReadOnlyDictionary<int, IConditionLocationYearRealizationSummary> Years { get; }
        internal ConditionLocationRealization(IReadOnlyDictionary<int, IConditionLocationYearRealizationSummary> years)
        {
            //TODO: Validation
            //same metrics available throughout, only one EAD metric, reasonable span of years
            Years = years;
        }
        public IFdaFunction GetMetricYearFunction(IMetric metric, InterpolationEnum interpolation = InterpolationEnum.Linear)
        {
            IParameterEnum type = metric.ParameterType;
            List<ICoordinate> coordinates = new List<ICoordinate>();
            foreach (var yr in Years) if (yr.Value.Metrics.ContainsKey(metric)) coordinates.Add(ICoordinateFactory.Factory(Convert.ToDouble(yr.Key), yr.Value.Metrics[metric]));
            coordinates = coordinates.OrderBy(i => i.X.Value()).ToList();
            return IFdaFunctionFactory.Factory((IParameterEnum)((int)type + 10), IFunctionFactory.Factory(coordinates, interpolation));
        }
        public IFdaFunction GetEquivalentAnnualDamagesFunction(double discountRate, InterpolationEnum interpolation = InterpolationEnum.Linear, IMetric metric = null)
        {
            metric = metric == null ? IMetricFactory.Factory() : metric;
            if (metric.ParameterType != IParameterEnum.EAD) throw new ArgumentException($"The specified metric: {metric.Print(true, true)} is invalid. Only {IMetricFactory.Factory().Print(true, true)} can be used to compute equivalent annual damages.");
            IFdaFunction EadFx = GetMetricYearFunction(metric, interpolation); // YearEAD + 1
            return GetEquivalentAnnualDamagesFunction(EadFx, discountRate);
        }
        public IFdaFunction GetEquivalentAnnualDamagesFunction(IFdaFunction EadFx, double discountRate)
        {
            if (EadFx.ParameterType != IParameterEnum.YearEAD) throw new ArgumentException($"The specified {EadFx.ParameterType.Print(true)} function is invalid because it is not an {IParameterEnum.YearEAD.Print(true)} function.");
            if (!discountRate.IsOnRange(0, 1)) throw new ArgumentException($"The specified discount rate: {discountRate} is invalid because it is not on the range: [0, 1].");

            int lastYr = Convert.ToInt32(EadFx.Coordinates.Last().X.Value()),
                firstYr = Convert.ToInt32(EadFx.Coordinates.First().X.Value());
            List<ICoordinate> coordinates = new List<ICoordinate>();
            for (int yr = firstYr; yr < lastYr + 1; yr++)
            {
                coordinates.Add(ICoordinateFactory.Factory(
                    yr,
                    EadFx.F(IOrdinateFactory.Factory(
                        Convert.ToDouble(yr))).Value() / Math.Pow(1 + discountRate, yr - firstYr)));
            }
            return IFdaFunctionFactory.Factory(IParameterEnum.YearEquavalentAnnualDamages, IFunctionFactory.Factory(coordinates, EadFx.Interpolator));
        }
        public double GetEquivalentAnnualDamages(double discountRate, InterpolationEnum interpolation = InterpolationEnum.Linear, IMetric metric = null)
        {
            return GetEquivalentAnnualDamages(GetEquivalentAnnualDamagesFunction(discountRate, interpolation, metric));
        }
        public double GetEquivalentAnnualDamages(IFdaFunction EquivalentAnnualDamagesFx)
        {
            double sum = 0;
            foreach (var coordinate in EquivalentAnnualDamagesFx.Coordinates) sum += coordinate.Y.Value();
            return sum;
        }
    }
}
