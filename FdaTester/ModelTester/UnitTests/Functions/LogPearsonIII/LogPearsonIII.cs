using System;
using System.Linq;
using FdaModel.Functions.FrequencyFunctions;

namespace FdaTester.ModelTester.UnitTests
{
    //[System.Diagnostics.DebuggerDisplayAttribute("ToString()")]
    public class LogPearsonIIIFunctionTester : UnitTest
    {
        #region Fields
        static readonly string[] _InputColumnHeaders = { "Parameter", "Test LPIII Parameters", "Test Probabilities" };
        static readonly string[] _InputParameterNames = { "Mean", "Standard Deviation", "Skew" };
        private double[] _TestFunctionParamaters;
        private float[] _TestProbabilities;
        private float[] _CSharpFlowValues;
        private float[] _FortranFlowValues;
        private float[] _absoluteDiffValues;
        private float[] _percentDiffValues;
        private bool _testStatus = true;
        private string _filePath;
        #endregion

        #region Properties
        public bool testStatus { get { return _testStatus; } set { _testStatus = value; } }
        public string[] inputColumnHeaders
        {
            get { return _InputColumnHeaders; }
        }
        public string[] parameterNames
        {
            get { return _InputParameterNames; }
        }
        public double[] parameters
        {
            get { return _TestFunctionParamaters; }
            set { _TestFunctionParamaters = value; }
        }
        public float[] probabilities
        {
            get { return _TestProbabilities; }
            set { _TestProbabilities = value; }
        }
        public float[] cSharpFlowValues
        {
            get { return _CSharpFlowValues; }
            set { _CSharpFlowValues = value; }
        }
        public float[] fortranFlowValues
        {
            get { return _FortranFlowValues; }
            set { _FortranFlowValues = value; }
        }
        public float[] absoluteDiffValues
        {
            get { return _absoluteDiffValues; }
            set { _absoluteDiffValues = value; }
        }
        public float[] percentDiffValues
        {
            get { return _percentDiffValues; }
            set { _percentDiffValues = value; }
        }
        #endregion

        #region Constructor
        public LogPearsonIIIFunctionTester(string fullFilePath, double[] testFunctionParameters, float[] testProbabilities) : base(fullFilePath)
        {
            _filePath = fullFilePath;
            _TestFunctionParamaters = testFunctionParameters;
            _TestProbabilities = testProbabilities;
        }
        #endregion

        #region Data Generators
        public static void FileGenerator(LogPearsonIIIFunctionTester testObject)
        {

        }

        public static void InputFileGenerator(LogPearsonIIIFunctionTester testObject)
        {
            object[][] dataForInputFile = new object[3][];
            dataForInputFile[0] = testObject.parameterNames.Cast<object>().ToArray();
            dataForInputFile[1] = testObject.parameters.Cast<object>().ToArray();
            dataForInputFile[2] = testObject.probabilities.Cast<object>().ToArray();

            Utilities.DataExporter.ExportDelimitedColumns(testObject.filePath, dataForInputFile, testObject.inputColumnHeaders.ToArray());
        }

        //public static void ResultsFileGenerator(LogPearsonIIIFunctionTester testObject, float[][] exportData, string[] columnHeaders)
        //{
        //    object[][] dataForExportFile = new object[exportData.Length][];
        //    for (int i = 0; i < exportData.Length; i++)
        //    {
        //        dataForExportFile[i] = exportData[i].Cast<object>().ToArray();
        //    }

        //    Utilities.DataExporter.ExportDelimitedColumns(testObject.exportFilePath, dataForExportFile, columnHeaders.ToArray());
        //}

        public static LogPearsonIIIFunctionTester TestObjectGenerator(string fileFolderPath, string fileNameWithoutExtension, LogPearsonIII testFunction, float[] testProbabilities)
        {
            double[] testparameters = new double[4] { testFunction.Function.GetMean, testFunction.Function.GetStDev, testFunction.Function.GetG, (double)testFunction.Function.GetSampleSize };
            return new LogPearsonIIIFunctionTester(fileFolderPath + "\\" + fileNameWithoutExtension + ".txt", testparameters, testProbabilities);
        }

        public static LogPearsonIIIFunctionTester TestObjectGenerator(string fileFolderPath, string fileNameWithoutExtension, double testMean, double testStandardDeviation, double testSkew, int testN, float[] testProbabilities)
        {
            double[] testparameters = new double[4] { testMean, testStandardDeviation, testSkew, testN };
            return new LogPearsonIIIFunctionTester(fileFolderPath + "\\" + fileNameWithoutExtension + ".txt", testparameters, testProbabilities);
        }

        public static double[] TestParameterGenerator(int randomSeed = 0)
        {
            Random randomNumberGenerator = new Random(randomSeed);
            return new double[4] { randomNumberGenerator.Next(1, 9) + randomNumberGenerator.NextDouble(), randomNumberGenerator.Next(0, 2) + randomNumberGenerator.NextDouble(), randomNumberGenerator.Next(-2, 2) + randomNumberGenerator.NextDouble(), (int)randomNumberGenerator.Next(1, 300) };
        }

        public static LogPearsonIII TestFunctionGenerator(int randomSeed = 0)
        {
            Random randomNumberGenerator = new Random(randomSeed);
            FdaModel.ComputationPoint indexPoint = new FdaModel.ComputationPoint("aLocationOn", "aRiver", "underWoPConditions", 2016);
            double mean = randomNumberGenerator.Next(1, 9) + randomNumberGenerator.NextDouble();
            double standardDeviation = randomNumberGenerator.Next(0, 2) + randomNumberGenerator.NextDouble();
            double skew = randomNumberGenerator.Next(-2, 2) + randomNumberGenerator.NextDouble();
            int n = randomNumberGenerator.Next(1, 300);
            return TestFunctionGenerator(indexPoint, mean, standardDeviation, skew, n);
        }

        public static LogPearsonIII TestFunctionGenerator(FdaModel.ComputationPoint indexPoint, double testMean, double testStandardDeviation, double testSkew, int testN)
        {
            return new LogPearsonIII( testMean, testStandardDeviation, testSkew, testN);
        }

        public static LogPearsonIII TestFunctionGenerator(double[] data, int randomSeed = 0)
        {
            int maxRandomNumber = data.Length - 1;
            Random randomNumberGenerator = new Random(randomSeed);
            FdaModel.ComputationPoint indexPoint = new FdaModel.ComputationPoint("aLocationOn", "aRiver", "underWoPConditions", 2016);
            double[] bootstrapData = new double[data.Length - 1];
            for (int i = 0; i < data.Length; i++)
            {
                bootstrapData[i] = data[randomNumberGenerator.Next(maxRandomNumber)];
            }
            return new LogPearsonIII( bootstrapData);
        }

        public static LogPearsonIII TestFunctionGenerator(LogPearsonIII logPearsonFunction, int syntheticRecordLength = 100, int randomSeed = 0)
        {
            Random randomNumberGenerator = new Random(randomSeed);
            FdaModel.ComputationPoint indexPoint = new FdaModel.ComputationPoint("aLocationOn", "aRiver", "underWoPConditions", 2016);
            double[] sytheticRecord = new double[syntheticRecordLength - 1];
            for (int i = 0; i < sytheticRecord.Length; i++)
            {
                sytheticRecord[i] = logPearsonFunction.Function.getDistributedVariable(randomNumberGenerator.NextDouble());
            }
            return new LogPearsonIII( sytheticRecord);
        }

        public static float[] TestProbabilitiesGenerator(bool standardProbabilities = true, int nProbabilities = 8, Random randomNumberGenerator = null)
        {
            if (standardProbabilities == true)
            {
                return new float[8] { 0.5f, 0.2f, 0.1f, 0.04f, 0.02f, 0.01f, 0.005f, 0.002f };
            }
            else
            {
                float[] probabilities = new float[nProbabilities];
                for (int i = 0; i < nProbabilities; i++)
                {
                    probabilities[i] = (float)randomNumberGenerator.NextDouble();
                }
                return probabilities;
            }
        }

        public static int nProbabilitiesGenerator(int randomSeed = 0)
        {
            Random randomNumberGenerator = new Random(randomSeed);
            return randomNumberGenerator.Next(100);
        }
        #endregion

        #region Fortran Functions
        /// <summary> Tests a Log Pearson Type III curve from GPEAR.FOR code. </summary>
        /// <param name="testFunction"> Log Pearson III curve to test. </param>
        /// <param name="probabilities"> Probabilites for the flows that are returned. </param>
        /// <returns> Returns a set of flows associated with the supplied array of probabilities. </returns>
        public float[] FortranFlows()
        {
            FdaModel.ComputationPoint indexPoint = new FdaModel.ComputationPoint("aLocationOn", "aRiver", "underWoPConditions", 2016);
            LogPearsonIII testFunction = new LogPearsonIII( parameters[0], parameters[1], parameters[2], (int)parameters[3]);
            int ip = 423;
            int wp = 1;
            int error = 0;
            int debug = 0;
            int ipear = 0;
            int nstand = probabilities.Length;
            int nqfr = probabilities.Length;
            int kstand = probabilities.Length;
            int kfr = probabilities.Length;
            float m = (float)testFunction.Function.GetMean;
            float s = (float)testFunction.Function.GetStDev;
            float k = (float)testFunction.Function.GetG;
            float[] qfit = null;                                // new float[3]; //not used for this method
            float[] pdam = new float[16];
            float[] qfrl = new float[16];
            float[] dqfrl = new float[16];

            float[] fortranFlows = new float[probabilities.Length];
            FortranImports.LPIIICRV(ref ip, ref wp, ref error, ref debug, ref ipear, ref nstand, ref nqfr, ref kstand, ref kfr, ref m, ref s, ref k, qfit, fortranFlows, qfrl, dqfrl, probabilities, pdam);
            return fortranFlows;
        }

        /// <summary> Tests a Log Pearson Type III curve from GPEAR.FOR code. </summary>
        /// <param name="testFunction"> Log Pearson III curve to test. </param>
        /// <param name="probabilities"> Probabilites for the flows that are returned. </param>
        /// <returns> Returns a set of flows associated with the supplied array of probabilities. </returns>
        public static float[] FortranFlows(LogPearsonIII testFunction, float[] probabilities = null)
        {
            if (probabilities == null)
            {
                float[] defaultProbabilities = new float[8] { 0.5f, 0.2f, 0.1f, 0.04f, 0.02f, 0.01f, 0.005f, 0.002f };
                probabilities = defaultProbabilities;
            }

            int ip = 423;
            int wp = 1;
            int error = 0;
            int debug = 0;
            int ipear = 0;
            int nstand = probabilities.Length;
            int nqfr = probabilities.Length;
            int kstand = probabilities.Length;
            int kfr = probabilities.Length;
            float m = (float)testFunction.Function.GetMean;
            float s = (float)testFunction.Function.GetStDev;
            float k = (float)testFunction.Function.GetG;
            float[] qfit = null;                                // new float[3]; //not used for this method
            float[] pdam = new float[16];
            float[] qfrl = new float[16];
            float[] dqfrl = new float[16];

            float[] fortranFlows = new float[probabilities.Length];
            FortranImports.LPIIICRV(ref ip, ref wp, ref error, ref debug, ref ipear, ref nstand, ref nqfr, ref kstand, ref kfr, ref m, ref s, ref k, qfit, fortranFlows, qfrl, dqfrl, probabilities, pdam);
            return fortranFlows;
        }

        /// <summary> Samples a Log Pearson Type III curve from LPIIICRV.FOR code. </summary>
        /// <param name="testFunction"> Log Pearson III function from which the sampled function is based. </param>
        /// <param name="meanDeviate"> Sampled function mean's deviation from test function mean, based on standard normal distribution. </param>
        /// <param name="standardDeviationDeviate"> Sampled function's devaition from base function standard deviation, based on chi-squared distribution. </param>
        /// <param name="nProbabilities"> Number of probabilities values to return. </param>
        /// <returns> Returns a set of uniformly distributed probabilities from the sampled curve, based on the supplied test function and number of desired probability values. </returns>
        public static float[] FortranSampleFunction(LogPearsonIII testFunction, double meanDeviate, double standardDeviationDeviate, int nProbabilities)
        {
            // The baseFunction (lpIII) mean, standard deviation, skew and number of ordinates are passed into the FORTRAN code, presumably to generate an log pearson III function.
            // Two random numbers on the interval (0,1) are passed into the FORTRAN code, presumably to sample an log pearson III curve by perturbing the mean and standard deviation.         
            // The number of desired probabilities is passed in, n equally spaced probabilities are generated on the interval [1/n, 1] (inclusive).
            int nfrq = nProbabilities;   //Get this from the xml file
            int nchia = 3;               //What is this?     
            int kfreq = nfrq;            //Should this be nProbabilities - 1?
            int kchia = nchia;

            Statistics.Normal standardNormalDistribution = new Statistics.Normal();
            float xk = (float)standardNormalDistribution.getDistributedVariable(meanDeviate);                                   // standard normal deviate based on the mean random variable line 38 gencrv
            float p = (float)standardDeviationDeviate;                                                                          // probability for chi squared distribution for standard deviation sampling line 39 gencrv
            float xm = (float)testFunction.Function.GetMean;
            float sdevmn = (float)(testFunction.Function.GetStDev / Math.Sqrt((double)testFunction.Function.GetSampleSize));    //line 178 gencrv
            int xn1 = testFunction.Function.GetSampleSize - 1;                                                                  // is this right?                                                             //gpear notes.

            float sdrtn1 = (float)(Math.Sqrt(xn1) * testFunction.Function.GetStDev);                                            //gpear notes.

            //only passing the one variable from the chisquared distribution to limit exposure for error entry in the intpld function.
            Statistics.ChiSquared chiSquaredDistribution = new Statistics.ChiSquared(ref xn1);
            float xmrmin = 0.0000002f;                                                                                          //line 185 gencrv
            float xmrmax = (float)(testFunction.Function.GetMean + sdevmn * 3.1);                                               //lines 151,178 194 gencrv

            float[] chia = new float[3] { 0.0f, (float)chiSquaredDistribution.getDistributedVariable(standardDeviationDeviate), 1.0f };
            float[] pchia = new float[3] { 0.0f, (float)standardDeviationDeviate, 1.0f };

            float[] probs = new float[kfreq];                                                                                   //get this from input data
            float[] xkpdam = new float[kfreq];
            float[] xnorm = new float[kfreq];
            float[] flows = new float[kfreq];
            for (int i = 0; i < kfreq; i++)
            {
                probs[i] = (i + 1.0f) / kfreq;                                                                                  // 
                xkpdam[i] = (float)standardNormalDistribution.getDistributedVariable(1 - probs[i]);                             //lines 229-233 gencrv
                xnorm[i] = (float)(testFunction.Function.GetMean + (testFunction.Function.GetStDev * xkpdam[i]));
                flows[i] = (float)testFunction.Function.getDistributedVariable(probs[i]);                                       //computed flows from the original lpiii dist
            }
            float[] pr = new float[kfreq - 1];
            FortranImports.GPEAR(ref nfrq, ref nchia, ref kfreq, ref kchia, ref xk, ref p, ref xm, ref sdevmn, ref sdrtn1, ref xmrmin, ref xmrmax, chia, pchia, xnorm, pr);
            return pr;                                                                                                          //_FortranOutputProbabilities = pr; // this should be output of the line above?
        }

        /// <summary> Samples a Log Pearson Type III curve from LPIIICRV.FOR code. </summary>
        /// <param name="testFunction"> Log Pearson III function from which the sampled function is based.  </param>
        /// <param name="randomMeanDeviate"> Sampled function mean's deviation from test function mean, based on standard normal distribution. </param>
        /// <param name="randomStandardDeviationDeviate"> Sampled function's devaition from base function standard deviation, based on chi-squared distribution. </param>
        /// <param name="probabilities"> Set of supplied probabilities from which the array of returned probabilites are based. </param>
        /// <returns> Returns a set of probabilities from the sampled function, based on the test function flows associated with the supplied array of probabilities. </returns>
        public static float[] FortranSampleFunction(LogPearsonIII testFunction, double randomMeanDeviate, double randomStandardDeviationDeviate, float[] probabilities)
        {
            // The baseFunction (lpIII) mean, standard deviation, skew and number of ordinates are passed into the FORTRAN code, presumably to generate an log pearson III function.
            // Two random numbers on the interval (0,1) are passed into the FORTRAN code, presumably to sample an log pearson III curve by perturbing the mean and standard deviation.         
            // A set of probabilites are passed in, these are associated with a flow in the base function, a new function is sampled and the probabilities associated with these base function flows are returned (i think...).
            int nfrq = probabilities.Length;    //Get this from the xml file
            int nchia = 3;                      //What is this?     
            int kfreq = nfrq;                   //Should this be nProbabilities - 1?
            int kchia = nchia;

            Statistics.Normal standardNormalDistribution = new Statistics.Normal();
            float xk = (float)standardNormalDistribution.getDistributedVariable(randomMeanDeviate);                             // standard normal deviate based on the mean random variable line 38 gencrv
            float p = (float)randomStandardDeviationDeviate;                                                                    // probability for chi squared distribution for standard deviation sampling line 39 gencrv
            float xm = (float)testFunction.Function.GetMean;
            float sdevmn = (float)(testFunction.Function.GetStDev / Math.Sqrt((double)testFunction.Function.GetSampleSize));    //line 178 gencrv
            int xn1 = testFunction.Function.GetSampleSize - 1;                                                                  // is this right?                                                             //gpear notes.

            float sdrtn1 = (float)(Math.Sqrt(xn1) * testFunction.Function.GetStDev);                                            //gpear notes.

            //only passing the one variable from the chisquared distribution to limit exposure for error entry in the intpld function.
            Statistics.ChiSquared chiSquaredDistribution = new Statistics.ChiSquared(ref xn1);
            float xmrmin = 0.0000002f;                                                                                          //line 185 gencrv
            float xmrmax = (float)(testFunction.Function.GetMean + sdevmn * 3.1);                                               //lines 151,178 194 gencrv

            float[] chia = new float[3] { 0.0f, (float)chiSquaredDistribution.getDistributedVariable(randomStandardDeviationDeviate), 1.0f };
            float[] pchia = new float[3] { 0.0f, (float)randomStandardDeviationDeviate, 1.0f };

            float[] probs = probabilities;                                                                                      //get this from input data.
            float[] xkpdam = new float[kfreq];
            float[] xnorm = new float[kfreq];
            float[] flows = new float[kfreq];
            for (int i = 0; i < kfreq; i++)
            {
                //probs[i] = (i + 1.0f) / kfreq;                                                                                // generates probabilities to test if nProbabilities is passed into function instead of an array of probabilities.
                xkpdam[i] = (float)standardNormalDistribution.getDistributedVariable(1 - probs[i]);                             //lines 229-233 gencrv
                xnorm[i] = (float)(testFunction.Function.GetMean + (testFunction.Function.GetStDev * xkpdam[i]));
                flows[i] = (float)testFunction.Function.getDistributedVariable(probs[i]);                                       //computed flows from the original lpiii dist
            }
            float[] pr = new float[kfreq - 1];
            FortranImports.GPEAR(ref nfrq, ref nchia, ref kfreq, ref kchia, ref xk, ref p, ref xm, ref sdevmn, ref sdrtn1, ref xmrmin, ref xmrmax, chia, pchia, xnorm, pr);
            return pr;                                                                                                          //_FortranOutputProbabilities = pr; // this should be output of the line above?
        }
        #endregion

        #region C# Fuctions
        public float[] CSharpFlow()
        {
            FdaModel.ComputationPoint indexPoint = new FdaModel.ComputationPoint("aLocationOn", "aRiver", "underWoPConditions", 2016);
            LogPearsonIII testFunction = new LogPearsonIII( parameters[0], parameters[1], parameters[2], (int)parameters[3]);

            float[] flows = new float[probabilities.Length];
            for (int i = 0; i < probabilities.Length; i++)
            {
                flows[i] = (float)testFunction.Function.getDistributedVariable(1 - probabilities[i]);
            }
            return flows;
        }
        #endregion

        #region UnitTests
        public static float[][] CompareFlows(string testFilesFolderPath, string testName, double testMean, double testStandardDeviation, double testSkew, int testPeriodOfRecord)
        {

            LogPearsonIIIFunctionTester testObject = TestObjectGenerator(testFilesFolderPath, testName, testMean, testStandardDeviation, testSkew, testPeriodOfRecord, TestProbabilitiesGenerator());
            float[] cSharpFlows = testObject.CSharpFlow();
            float[] fortranFlows = testObject.FortranFlows();

            float[][] comparisionFlows = new float[2][];
            comparisionFlows[0] = cSharpFlows;
            comparisionFlows[1] = fortranFlows;

            return comparisionFlows;
        }
        private static bool TestCompareFlows(float[] cSharpFlows, float[] fortranFlows)
        {
            double absoluteTolerance = 100;
            double relativeTolerance = 0.10f;

            for (int i = 0; i < cSharpFlows.Length; i++)
            {
                if (cSharpFlows[i] - fortranFlows[i] > absoluteTolerance)
                {
                    if (Math.Abs(cSharpFlows[i] - fortranFlows[i]) / cSharpFlows[i] > relativeTolerance)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public float[] GetAbsoluteDifference()
        {
            float[] absoluteDiff = new float[_CSharpFlowValues.Length];
            for (int i = 0; i < cSharpFlowValues.Length; i++)
            {
                absoluteDiff[i] = (cSharpFlowValues[i] - fortranFlowValues[i]);
            }
            return absoluteDiff;
        }

        public float[] GetPercentDifference() //caution, this must be called after GetAbsoluteDifference
        {
            float[] percentDiff = new float[_CSharpFlowValues.Length];
            for (int i = 0; i < cSharpFlowValues.Length; i++)
            {
                percentDiff[i] = (absoluteDiffValues[i] / cSharpFlowValues[i]) * 100;
                if (percentDiff[i] > 10) { testStatus = false; }
            }

            return percentDiff;
        }

        #endregion
    }
}
