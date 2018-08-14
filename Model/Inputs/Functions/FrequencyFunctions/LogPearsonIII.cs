using System;
using FdaModel.Utilities.Attributes;

namespace FdaModel.Inputs.Functions.FrequencyFunctions
{
    [Author("John Kucharski", "06/10/2016", "08/17/2016")]
    public class LogPearsonIII : FrequencyFunction
    {
        #region Notes
        /* This class is a "thin wrapper class" over the LogPearsonIII class in the Statistics assembly. I need to:
         *      1. Add data validation properties (acceptable mean, standard deviation and skew, data lengths, etc.)
         *      2. Validate x values on frequency function base class (values between 0 and 1).
         */
        #endregion

        #region Properties
        public new Statistics.LogPearsonIII Function
        {
            get
            {
                return (Statistics.LogPearsonIII)_Function;
            }
            set
            {
                _Function = value;
            }
        }
        #endregion

        #region Constructors
        [Tested(true, true, "M:\\Kucharski\\Public\\Fda\\2.0\\Testing\\Constructors\\testing_Constructor_1_Flows_vs_Constructor_2_Flows.txt", "8/30/2016","Cody McCoy")]
        public LogPearsonIII(ComputationPoint indexPoint, double mean, double standardDeviation, double skew, int periodOfRecord) :
            base(indexPoint, new Statistics.LogPearsonIII(mean, standardDeviation, skew, periodOfRecord), FunctionTypes.InflowFrequency)
        {
        }

        [Tested(true, true, "M:\\Kucharski\\Public\\Fda\\2.0\\Testing\\Constructors\\testing_Constructor_1_Flows_vs_Constructor_2_Flows.txt", "8/30/2016", "Cody McCoy")]
        public LogPearsonIII(ComputationPoint indexPoint, double[] annualPeakFlowData) :
            base(indexPoint, new Statistics.LogPearsonIII(annualPeakFlowData), FunctionTypes.InflowFrequency)
        {
        }

        /// <summary> Used within the class to create new sampled functions. </summary>
        /// <param name="newFunction"> The sampled function, supplied by the SampleFunction method. </param>
        [Tested(true,true, "M:\\Kucharski\\Public\\Fda\\2.0\\Testing\\Constructors\\testing_Constructor3_vs_Constructor1.txt","9/8/2016","Cody McCoy")]
        private LogPearsonIII(ComputationPoint indexPoint, Statistics.LogPearsonIII newFunction) :
            base(indexPoint, newFunction, FunctionTypes.InflowFrequency)
        {   
        }
        #endregion

        #region Functions
        public override bool Validate()
        {
            throw new NotImplementedException();
        }

        /// <summary> Generates mean and standard deviation deviates, based on a supplied random number generator, used to sample a new function using the GetRealizationLP3 method in the LogPearsonIII class of the Statistics assembly. </summary>
        /// <param name="randomNumberGenerator"></param>
        /// <returns> A new log pearson III function sampled from the existing object's function, based on random pertubation to the existing function's mean and standard deviation. </returns>
        [Tested(false)]
        public override BaseFunction SampleFunction(Random randomNumberGenerator)
        {
            double[] deviates = new double[2] { randomNumberGenerator.NextDouble(), randomNumberGenerator.NextDouble() };
            return SampleFunction(deviates[0], deviates[1]);
        }

        /// <summary> Generates a new log pearson III function, sampled from the existing object's function based on supplied deviations to the exising function's mean and standard deviation. </summary>
        /// <param name="meanDeviate"> A random number on the interval [0,1] that generates a mean pertubation, based on the standard normal distribution. </param>
        /// <param name="standardDeviationDeviate"> A random number on the interval [0,1] that generates a standard deviation pertubation, based on the chi-squared distribution. </param>
        /// <returns> A new log pearson III function, sampled from the existing object's function based on the supplied mean and standard deviation deviates. </returns>
        [Tested(false)]
        public LogPearsonIII SampleFunction(double meanDeviate, double standardDeviationDeviate)
        {
            return new LogPearsonIII(IndexPoint, Function.GetRealizationLP3(meanDeviate, standardDeviationDeviate));
        }
        #endregion
    }
}