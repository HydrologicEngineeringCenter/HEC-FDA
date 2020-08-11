//using Functions;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Utilities;

//namespace Model.Conditions.Locations.LateralStructures
//{
//    internal class LateralStructureRealization: ILateralStructureRealization
//    {
//        public string Label { get; } 
//        public UnitsEnum Units { get; }
//        public IParameterEnum ParameterType => IParameterEnum.LateralStructure;

//        public IElevation FailureElevation { get; }
        
//        public IElevation TopElevation { get; }
//        public IRange<double> Range => TopElevation.Range;
//        public IFdaFunction FailureFunction { get; }
//        public bool IsConstant => true;

//        internal LateralStructureRealization(ILateralStructure lateralStructure, IElevation failureElevation)
//        {
//            //todo: validation (constant, consistent units, failure elevation < top, values, etc.)
//            Units = lateralStructure.Units;
//            Label = lateralStructure.Label;
//            TopElevation = lateralStructure.TopElevation;
//            FailureFunction = lateralStructure.FailureFunction;
//            FailureElevation = failureElevation;
//        }

//        public ITransformFunction InteriorExteriorGenerator(ITransformFunction sampledExtIntFx)
//        {
//            //todo: validate (check units, check ranges)
//            List<ICoordinate> coordinates = new List<ICoordinate>();
//            foreach (var pair in sampledExtIntFx.Coordinates) coordinates.Add(ICoordinateFactory.Factory(pair.X.Value(), pair.X.Value() < FailureElevation.Parameter.Value() ? 0.0 : pair.Y.Value()));
//            return ITransformFunctionFactory.Factory(IFunctionFactory.Factory(coordinates, sampledExtIntFx.Interpolator), IParameterEnum.ExteriorInteriorStage, sampledExtIntFx.Label, sampledExtIntFx.Units, sampledExtIntFx.XSeries.Label, sampledExtIntFx.YSeries.Units, sampledExtIntFx.YSeries.Label);
//        }
//        public ITransformFunction InteriorExteriorGenerator(IFrequencyFunction sampledExtFreqFx)
//        {
//            List<ICoordinate> coordinates = new List<ICoordinate>();
//            foreach (var pair in sampledExtFreqFx.Coordinates) coordinates.Add(ICoordinateFactory.Factory(pair.Y.Value(), pair.Y.Value() < FailureElevation.Parameter.Value() ? 0.0 : pair.Y.Value()));
//            return ITransformFunctionFactory.Factory(IFunctionFactory.Factory(coordinates, sampledExtFreqFx.Interpolator), IParameterEnum.ExteriorInteriorStage, sampledExtFreqFx.Label, sampledExtFreqFx.Units, sampledExtFreqFx.XSeries.Label, sampledExtFreqFx.YSeries.Units, sampledExtFreqFx.YSeries.Label);
//        }

//        public string Print(bool round = false, bool abbreviate = false)
//        {
//            throw new NotImplementedException();
//        }

//        public string PrintValue(bool round = false, bool abbreviate = false)
//        {
//            throw new NotImplementedException();
//        }

//        public ILateralStructureRealization Compute(double failureFxProbability, IFrequencyFunction extFreqFx, double extFailElevProbability)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
