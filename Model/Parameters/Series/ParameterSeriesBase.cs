using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Functions;
using Functions.CoordinatesFunctions;
using Utilities;

namespace Model.Parameters.Series
{
    internal abstract class ParameterSeriesBase : IParameter, IMessagePublisher
    {
        private readonly int _Elements;
        #region Properties
        #region Base Class properties
        //public IEnumerable<IOrdinate> Parameter { get; }
        public IRange<double> Range { get; }
        public OrderedSetEnum Order { get; }
        public bool IsConstant { get; }
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
            _Elements = fx.Coordinates.Count;
            IsConstant = fx.IsConstant;
            Range = x ? fx.Domain : fx.Range;
            Order = x ? OrderedSetEnum.StrictlyIncreasing : fx.Order;
        }
        #endregion

        #region Functions
        public string Print(bool round = false, bool abbreviate = false)
        {
            return $"{ParameterType.ToString()}: {PrintValue(round, abbreviate)}.";
        }
        public string PrintValue(bool round = false, bool abbreviate = false)
        {
            return $"{_Elements} {Order.ToString()} values on the range: {Range.Print(round)} { UnitsUtilities.Print(Units, abbreviate)}";
        }
        #endregion
    }
}
