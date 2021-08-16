using Functions;
using System.Collections.Generic;
using System.Xml.Linq;
using Utilities;
using Utilities.Serialization;

namespace Model.Functions.FrequencyFunctions
{
    internal sealed class LogPearsonIII: FdaFunctionBase, IFrequencyFunction
    {
        #region Fields
        internal IFunction _TruncatedFunction;
        #endregion
        #region Properties
        public override ICoordinatesFunction Function => _TruncatedFunction;

        public override IRange<double> Range { get; }
        public override IRange<double> Domain { get; }
        public override List<ICoordinate> Coordinates { get; }

        public override IParameterRange XSeries { get; }
        public override IParameterRange YSeries { get; }

        public override string Label { get; }

        public override IParameterEnum ParameterType => IParameterEnum.InflowFrequency;
        public List<IParameterEnum> ComposeableTypes => new List<IParameterEnum>() { IParameterEnum.InflowOutflow, IParameterEnum.Rating };

        public override IMessageLevels State { get; }
        public override IEnumerable<IMessage> Messages { get; }
        #endregion
        #region Construtor
        internal LogPearsonIII(IFunction fx, string label = "", string xLabel = "", string yLabel = "", UnitsEnum yUnits = UnitsEnum.CubicMeterPerSecond): base(fx)
        {
            /* Set-up:
             *  (1) Construct range 1 - 1,000 yr (or largest finite range).
             *  (2) Truncate LPIII to that range.
             */
            Domain = IRangeFactory.Factory(
                _Function.Domain.Min > 0.01 ? _Function.Domain.Min : 0.01, 
                _Function.Domain.Max < 0.999 ? _Function.Domain.Max: 0.999);
            Range  = IRangeFactory.Factory(
                _Function.F(IOrdinateFactory.Factory(Domain.Min)).Value(), 
                _Function.F(IOrdinateFactory.Factory(Domain.Max)).Value());
            _TruncatedFunction = IFunctionFactory.Factory((IFunction)_Function, Range.Min, Range.Max);
            Coordinates = TruncateCoordinates();

            Label = label == "" ? ParameterType.Print() : label;
            XSeries = IParameterFactory.Factory(_TruncatedFunction, IParameterEnum.NonExceedanceProbability, true, true, UnitsEnum.Probability, xLabel);
            YSeries = IParameterFactory.Factory(_TruncatedFunction, IParameterEnum.UnregulatedAnnualPeakFlow, IsConstant, false, yUnits, yLabel);

            State = Validate(new Validation.Functions.FdaFunctionBaseValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }
        #endregion
        #region Functions
        private List<ICoordinate> TruncateCoordinates()
        {
            bool overMin = false;
            List<ICoordinate> coordinates = new List<ICoordinate>();
            foreach(var xy in _TruncatedFunction.Coordinates)
            {
                if (xy.X.Value() >= Domain.Max) break;
                if (xy.X.Value() <= Domain.Min) continue;
                if (!overMin)
                {
                    coordinates.Add(ICoordinateFactory.Factory(Domain.Min, _TruncatedFunction.Range.Min));
                    overMin = true;
                }
                coordinates.Add(xy);
            }
            coordinates.Add(ICoordinateFactory.Factory(Domain.Max, _TruncatedFunction.Range.Max));
            if(!IsMonotonic(coordinates))
            {
                //todo: what is this?
                int test = 0;
            }
            return coordinates;
        }

        /// <summary>
        /// for testing
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private bool IsMonotonic(List<ICoordinate> coordinates)
        {
            double prevX = coordinates[0].X.Value();
            double prevY = coordinates[0].Y.Value();
            for(int i = 1;i<coordinates.Count;i++)
            {
                double currentX = coordinates[i].X.Value();
                double currentY = coordinates[i].Y.Value();
                if(currentX <= prevX || currentY <= prevY)
                {
                    return false;
                }
                prevX = currentX;
                prevY = currentY;
            }
            return true;
        }

        public override IOrdinate F(IOrdinate x)
        {
            return _TruncatedFunction.F(x);
        }
        public override IOrdinate InverseF(IOrdinate y)
        {
            return _TruncatedFunction.InverseF(y);
        }
        public double Integrate() => _TruncatedFunction.TrapizoidalRiemannSum();
        #endregion

        public override XElement WriteToXML()
        {
            XElement functionsElem = new XElement(SerializationConstants.FUNCTIONS);
            functionsElem.SetAttributeValue(SerializationConstants.TYPE, SerializationConstants.NOT_LINKED);
            XElement funcElem = new XElement(SerializationConstants.FUNCTION);

            funcElem.SetAttributeValue("Interpolator", Interpolator);
            funcElem.SetAttributeValue(SerializationConstants.LOG_PEARSON_III, "True");

            //mean
            //funcElem.SetAttributeValue("Interpolator", stand);

            //st dev
            //skew
            //islogflow
            //por
            //isAnalytical
            //isStandard
            //analyticalFlows


            functionsElem.Add(funcElem);
            return functionsElem;
        }
    }
}
