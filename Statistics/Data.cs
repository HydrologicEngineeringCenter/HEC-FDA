using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Statistics
{
    internal class Data : IData, IValidate<IData>
    {
        #region Properties
        public bool IsValid { get; }
        public IEnumerable<IMessage> Messages { get; }
        //public double Minimum { get; }
        //public double Maximum { get; }
        public IRange<double> Range { get; }
        public IOrderedEnumerable<double> Elements { get; }
        public int SampleSize { get; }
        #endregion

        #region Constructor
        public Data(IEnumerable<double> data)
        {
            var datasets = SplitData(data);
            Elements = datasets.Item1.OrderBy(i => i);
            Range = IRangeFactory.Factory(Elements.First(), Elements.Last());
            SampleSize = Elements.Count();
            IsValid = Validate(new Validation.DataValidator(), out IEnumerable<IMessage> errors);
            Messages = AddDataMessages(errors, datasets.Item2);
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
        /// <summary>
        /// Adds data messages to the validator error messages.
        /// </summary>
        /// <param name="errors"> The validator error messages. </param>
        /// <param name="nonFiniteData"> The non-finite data elements returned from the <see cref="SplitData(IEnumerable{double})"/> method. </param>
        /// <param name="requestedRange"> The requested histogram range expressed as a Tuple. </param>
        /// <returns></returns>
        private IEnumerable<IMessage> AddDataMessages(IEnumerable<IMessage> errors, IEnumerable<double> nonFiniteData)
        {
            var msgs = errors.ToList();
            if (!nonFiniteData.IsNullOrEmpty()) msgs.Add(IMessageFactory.Factory(IMessageLevels.Message, $"{nonFiniteData.Count()} {double.NegativeInfinity}, {double.PositiveInfinity}, {double.NaN} elements where removed from the provided data."));
            return msgs;
        }
        #endregion 
        public bool Validate(IValidator<IData> validator, out IEnumerable<IMessage> errors)
        {
            return validator.IsValid(this, out errors);
        }
        #endregion
    }
}
