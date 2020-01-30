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
        public IRange<double> Range { get; }
        public IOrderedEnumerable<double> Elements { get; }
        public int SampleSize { get; }
        #endregion

        #region Constructor
        public Data(IEnumerable<double> data)
        {         
            var sets = SplitData(data);
            Elements = sets.Item1.OrderBy(i => i);
            SampleSize = Elements.Count();
            Range = IRangeFactory.Factory(Elements.First(), Elements.Last());
            IMessageBoard msgBoard = IMessageBoardFactory.Factory(DataMessages(sets.Item2));
            IsValid = Validate(new Validation.DataValidator(), out IEnumerable<IMessage> errors);
            msgBoard.PostMessages(errors);
            Messages = msgBoard.ReadMessages();
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
        /// Generates data message if one is necessary.
        /// </summary>
        /// <param name="nonFiniteData"> The non-finite data elements returned from the <see cref="SplitData(IEnumerable{double})"/> method. </param>
        /// <returns> An <see cref="IMessage"/> or an empty message. </returns>
        private IMessage DataMessages(IEnumerable<double> nonFiniteData)
        {
            if (!nonFiniteData.IsNullOrEmpty()) return IMessageFactory.Factory(IMessageLevels.Message, $"{nonFiniteData.Count()} {double.NegativeInfinity}, {double.PositiveInfinity} or {double.NaN} elements where removed from the provided data elements.");
            else return IMessageFactory.Factory(IMessageLevels.NotSet, "");
        }
        #endregion 
        public bool Validate(IValidator<IData> validator, out IEnumerable<IMessage> errors)
        {
            return validator.IsValid(this, out errors);
        }
        #endregion
    }
}
