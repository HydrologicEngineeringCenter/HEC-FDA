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
    /// This class and its constructor are not accessible through the API.
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
        public Utilities.IRange<double> Range { get; }
        /// <summary>
        /// The midpoint value computed with equation: (<see cref="IBin.Range.Max"/> - <see cref="IBin.Range.Min"/>) / 2
        /// </summary>
        public double MidPoint { get; }
        public double Width { get; }
        #endregion

        #region Constructors
        internal Bin (double min, double max, int n)
        {
            Count = n;
            Width = max - min;
            Range = IRangeFactory.Factory(min, max);           
            MidPoint = Width / 2d + Range.Min;           
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
            Range = oldBin.Range;
            Width = oldBin.Range.Max - oldBin.Range.Min;
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
        internal static string Print(double min, double max, int n) => $"Bin(count: {n.Print()}, range: [{min.Print()}, {max.Print()}]).";
        public static string Requirements(bool printNotes = true)
        {
            string s = $"Histogram bins require the following parameterization: {Parameterization()}.";
            if (printNotes) s += RequirementNotes();
            return s;
        }
        internal static string Parameterization() => $"Bin(count: [{0}, {int.MaxValue.Print()}], {Validation.Resources.DoubleRangeRequirements()})";
        internal static string RequirementNotes() => $"The number of observation in the bin, represented by the count parameter cannot exceed the maximum integer: {int.MaxValue.Print()} value.";
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
        public string Print(bool round = false) => round ? Print(Range.Min, Range.Max, Count) : $"Bin(count: {Count}, range: [{Range.Min}, {Range.Max}])";
        public bool Equals(IBin bin) => Range.Min == bin.Range.Min && Range.Max == bin.Range.Max && Count == bin.Count ? true : false;
        #endregion
    }
}
