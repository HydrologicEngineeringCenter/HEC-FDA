using Statistics.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Statistics
{
    internal class Data : IData, IValidate<Data>
    {
        #region Properties
        public IRange<double> Range { get; }
        public IOrderedEnumerable<double> Elements { get; }
        public int SampleSize { get; }
        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }
        /// <summary>
        /// Holds invalid data elements (e.g. double.NaN, double.NegativeInfinity, double.PositiveInfinity values)
        /// </summary>
        internal IEnumerable<double> InvalidElements { get; }
        #endregion

        #region Constructor
        public Data(IEnumerable<double> data, bool finiteNumericElementsRequirement = false)
        {
            if (!DataValidator.IsConstructable(data, out string msg)) throw new InvalidConstructorArgumentsException(msg);
            var sets = SplitData(data);
            InvalidElements = sets.Item2;
            Elements = sets.Item1.OrderBy(i => i);
            if (!Elements.Any())
            {
                if (finiteNumericElementsRequirement) throw new InvalidConstructorArgumentsException("The provided data is invalid because it contains 0 finite, numeric values.");
                SampleSize = 0;
                Range = IRangeFactory.Factory(double.NaN, double.NaN, true, true, false, false);
            }
            else
            {
                SampleSize = sets.Item1.Count();
                Range = IRangeFactory.Factory(Elements.First(), Elements.Last(), true, true, false, false);
            }
            State = Validate(new Validation.DataValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }
        #endregion

        #region Functions
        #region Initialization Functions
        /// <summary>
        /// Splits <paramref name="data"/> into finite and non-finite datasets.
        /// </summary>
        /// <param name="data"> The data to be split. </param>
        /// <returns> A Tuple containing the finite and non-finite data sets, respectively. </returns>
        private Tuple<IEnumerable<double>, IEnumerable<double>> SplitData(IEnumerable<double> data)
        {
            List<double> finite = new List<double>(), nonfinite = new List<double>();
            foreach (double x in data)
                if (x.IsFinite()) finite.Add(x);
                else nonfinite.Add(x);
            return new Tuple<IEnumerable<double>, IEnumerable<double>>(finite, nonfinite);
        }
        #endregion 
        public IMessageLevels Validate(IValidator<Data> validator, out IEnumerable<IMessage> errors)
        {
            return validator.IsValid(this, out errors);
        }
        #endregion
    }
}
