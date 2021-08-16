using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Statistics
{
    /// <summary>
    /// Defines criteria for testing <see cref="IDistribution"/> convergence.
    /// </summary>
    public interface IConvergenceCriteria
    {
        /// <summary>
        /// The <see cref="IDistribution"/> quantile to be tested for convergence.
        /// </summary>
        double Quantile { get; }
        /// <summary>
        /// The maximum tolerable absolute deviation in the <see cref="IDistribution"/> <see cref="Quantile"/> value as a portion the same <see cref="Quantile"/> value prior to the addition of a sample.
        /// </summary>
        double Tolerance { get; }
        ///// <summary>
        ///// Prints a string of the minimum number of <see cref="IDistribution"/> observations that must be amassed before convergence is evaluated and maximum number of observations that are amassed before convergence is assumed (<seealso cref="IDistribution.SampleSize"/> and <seealso cref="Utilities.IRange{int}"/>).  
        ///// </summary>
        //string TestRange { get; }
        /// <summary>
        /// The <see cref="IRange{T}.Min"/> number of observations required for convergence, and the <see cref="IRange{T}.Max"/> number of observations before convergence is assumed (and not numerically tested). 
        /// </summary>
        IRange<int> TestRange { get; }
        /// <summary>
        /// The minimum number of new observations that must be fit the distribution in order for convergence to be evaluated.
        /// </summary>
        int MinSampleSize { get; }
        /// <summary>
        /// Tests the criteria for convergence.
        /// </summary>
        /// <param name="qValueBefore"> The <see cref="IDistribution"/> <see cref="Quantile"/> value prior fitting the new <see cref="IData"/> sample to the <see cref="IDistribution"/> (<seealso cref="IDistribution.InverseCDF(double)"/>). </param>
        /// <param name="qValueAfter"> The <see cref="IDistribution"/> <see cref="Quantile"/> value after fitting the new <see cref="IData"/> sample to the <see cref="IDistribution"/>. </param>
        /// <param name="newObservations"> The number of observations (<seealso cref="IData.SampleSize"/>) being fit to the <see cref="IDistribution"/>. </param>
        /// <param name="totalObservations"> The <see cref="IDistribution.SampleSize"/> with the addition of the new sample observaitons. </param>
        /// <returns> The result of the convergence test. </returns>
        IConvergenceResult Test(double qValueBefore, double qValueAfter, int newObservations, int totalObservations);
    }
}

