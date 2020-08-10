using System;
using System.Collections.Generic;
using System.Text;
using Functions;
using Utilities;

namespace Model
{
    /// <summary>
    /// Provides a data interface for keeping track of sampled and constant parameters in complex functions, for instance <see cref="IConditionLocation.Compute()"/>
    /// </summary>
    public interface ISample
    {
        /// <summary>
        /// <see langword="true"/> if the <see cref="ISample.Probability"/> property represents a randomly sampled value, <see langword="false"/> otherwise.
        /// </summary>
        bool IsSample { get; }
        /// <summary>
        /// The probability to be used to sample the desired <see cref="IParameter"/>, for instance <see cref="ModelUtilities.Sample(IFdaFunction, double)"/>.
        /// </summary>
        double Probability { get; }
    }
}
