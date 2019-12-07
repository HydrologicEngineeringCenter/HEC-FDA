using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using Statistics.Validation;

using Utilities;

namespace Statistics.Histograms
{
    /// <summary>
    /// A bin for the histogram class.
    /// </summary>
    /// <remarks>
    /// This class and its constructor are ont accessible through the API.
    /// </remarks>
    internal sealed class Bin: IBin, IValidate<IBin>
    {
        #region Properties
        #region IValidate Properties
        public bool IsValid { get; }
        public IEnumerable<IMessage> Messages { get; }
        #endregion

        /// <summary>
        /// The number of observation in the bin.
        /// </summary>
        public int Count { get; }
        /// <summary>
        /// The minimum value for the histogram bin, inclusive.
        /// </summary>
        public double Minimum { get; }
        /// <summary>
        /// The exclusive maximum value for the histogram bin.
        /// </summary>
        public double Maximum { get; }
        /// <summary>
        /// The midpoint value computed with equation: (Maximum - Minimum) / 2
        /// </summary>
        public double MidPoint { get; }
        public double Width { get; }
        #endregion

        #region Constructors
        internal Bin (double min, double max, int n)
        {
            Minimum =  min;
            Maximum = max;
            Width = max - min;
            MidPoint = (Maximum - Minimum) / 2d + Minimum;
            Count = n;
            IsValid = Validate(new BinValidator(), out IEnumerable<IMessage> errors);
            Messages = errors;
        }
        /// <summary>
        /// Creates a new bin by adding <paramref name="n"/> observations to a pre-existing bin count.
        /// </summary>
        /// <param name="oldBin"> A <see cref="Bin"/> with the desired <see cref="Bin.Minimum"/>, <see cref="Bin.MidPoint"/> and <see cref="Bin.Maximum"/> property values, to which <paramref name="n"/> observations are added to the <see cref="Bin.Count"/>. </param>
        /// <param name="addN"> The number of observations to add to the <paramref name="oldBin"/> <see cref="Bin.Count"/>. </param>
        internal Bin (IBin oldBin, int addN)
        {
            Minimum = oldBin.Minimum;
            Maximum = oldBin.Maximum;
            Width = oldBin.Maximum - oldBin.Minimum;
            MidPoint = oldBin.MidPoint;
            Count = oldBin.Count + addN;
            IsValid = Validate(new BinValidator(), out IEnumerable<IMessage> errors);
            Messages = errors;
        }
        #endregion

        #region Functions
        #region IValidate Functions
        public bool Validate(IValidator<IBin> validator, out IEnumerable<IMessage> errors)
        {
            return validator.IsValid(this, out errors);
        }
        #endregion

        /// <summary>
        /// Prints the <see cref="MidPoint"/> value to an array the number of times specified by the <see cref="Count"/>.
        /// </summary>
        /// <returns> A <see cref="double"/> <see cref="Array"/> of the <see cref="MidPoint"/> value the length of the <see cref="Count"/></returns>
        internal double[] PrintData()
        {
            double[] data = new double[Count];
            for (int i = 0; i < Count; i++) data[i] = MidPoint;
            return data;
        }
        public string Print() => $"Bin(count: {Count}, range: [{Minimum}, {Maximum}])";
        public bool Equals(IBin bin) => Minimum == bin.Minimum && Maximum == bin.Maximum && Count == bin.Count ? true : false;
        #endregion
    }
}
