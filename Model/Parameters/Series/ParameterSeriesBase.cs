using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Functions;
using Functions.CoordinatesFunctions;
using Utilities;

namespace Model.Parameters.Series
{
    internal abstract class ParameterSeriesBase : IParameterSeries, IMessagePublisher
    {
        #region Properties
        #region Base Class properties
        public IEnumerable<IOrdinate> Parameter { get; }
        public IRange<double> Range { get; }
        public OrderedSetEnum Order { get; }
        #endregion
        #region Abstract properties
        public abstract UnitsEnum Units { get; }
        public abstract IParameterEnum ParameterType { get; }
        public abstract string Label { get; }
        #endregion
        #region IMessagePublisher Properties
        public abstract IMessageLevels State { get; }
        public abstract IEnumerable<IMessage> Messages { get; }
        #endregion
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs base class for Parameter Series.
        /// </summary>
        /// <param name="fx"> The function containing the parameter series. </param>
        /// <param name="x"> <see langword="true"/> if the desired parameter is the <see cref="IFdaFunction"/> x variable, <see langword="false"/> if the desired parameter is the y variable. </param>
        internal ParameterSeriesBase(IFdaFunction fx, bool x)
        {
            Parameter = Ordinates(fx, x);
            Range = x ? fx.Domain : fx.Range;
            Order = x ? OrderedSetEnum.StrictlyIncreasing : fx.Order;
        }
        private List<IOrdinate> Ordinates(IFdaFunction fx, bool x)
        {
            List<IOrdinate> ordinates = new List<IOrdinate>();
            foreach (ICoordinate coordinate in fx.Coordinates)
            {
                if (x) ordinates.Add(coordinate.X);
                else ordinates.Add(coordinate.Y);
            }
            return ordinates;
        }
        #endregion

        #region Functions
        public string Print(bool round = false, bool abbreviate = false)
        {
            return $"{ParameterType.ToString()}: {PrintValue(round, abbreviate)}.";
        }
        public string PrintValue(bool round = false, bool abbreviate = false)
        {
            return $"{Parameter.Count()} {Order.ToString()} values on the range: {Range.Print(round)} { UnitsUtilities.Print(Units, abbreviate)}";
        }
        #endregion
    }
}
