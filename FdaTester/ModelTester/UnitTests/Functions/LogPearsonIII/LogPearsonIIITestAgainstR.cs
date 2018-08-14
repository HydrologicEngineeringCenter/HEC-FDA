using System;
using System.Collections.Generic;
using System.Linq;

namespace FdaTester.ModelTester.UnitTests
{
    public class LogPearsonIIITestAgainstR : UnitTest
    {

        #region Fields
        internal double[][] _TestsParameters;
        internal double[][] _TestsFlows;
        internal double[] _TestsProbabilities;
        #endregion


        #region Properties
        public double[][] testsParameters
        {
            get { return _TestsParameters; }
            set { _TestsParameters = value; }
        }
        public double[][] testsFlows
        {
            get { return _TestsFlows; }
            set { _TestsFlows = value; }
        }
        public double[] testsProbabilities
        {
            get { return _TestsProbabilities; }
            set { _TestsProbabilities = value; }
        }
        #endregion


        #region Constructor
        public LogPearsonIIITestAgainstR(string FilePath, int nTests, int nProbabilities = 8): base(FilePath)
        {
            filePath = FilePath;
            testsParameters = GenerateTests(GenerateParameters(nTests));
            testsProbabilities = GenerateTestProbabilities(nProbabilities);
            testsFlows = GenerateTestsFlows(testsParameters, testsProbabilities);
        }
        #endregion


        #region Functions
        /// <summary>
        /// Generates set of log pearson III function means, standard deviations and skews to be tested.
        /// </summary>
        /// <param name="N"> Desired number of tests. </param>
        /// <remarks> 4^x = N tests will be generated. If no integer x returns N, x is rounded up to the next integer. </remarks>
        /// <returns></returns>
        public static double[][] GenerateParameters(int N)
        {
            //1. Number of mean, standard deviation and skew parameters.
            int J = (int)Math.Ceiling(Math.Pow(N, (double)1/3));    // Number of values for each parameter
            
            //2. Ranges to test and increments in each range.
            double minMean = 1, maxMean = 9;
            double minStandardDeviation = 0.01, maxStandardDeviation = 2;
            double minSkew = -2, maxSkew = 2;

            double dMean = (maxMean - minMean) / J;
            double dStandardDeviation = (maxStandardDeviation - minStandardDeviation) / J;
            double dSkew = (maxSkew - minSkew) / J;

            //2. Generate means, standard deviaitons and skews to test
            double[] means = new double[J + 1];
            double[] standardDeviations = new double[J + 1];
            double[] skews = new double[J + 1];
            double[][] parameters = new double[3][];
            for (int j = 0; j < J + 1; j++)
            {
                means[j] = minMean + dMean * j;
                standardDeviations[j] = minStandardDeviation + dStandardDeviation * j;
                skews[j] = minSkew + dSkew * j;
            }
            parameters[0] = means;
            parameters[1] = standardDeviations;
            parameters[2] = skews;

            return parameters;          
        }


        private static double[][] GenerateTests(double[][] parameters)
        {
            int numberOfTests = parameters[0].Length * parameters[1].Length * parameters[2].Length;
            double[][] tests = new double[numberOfTests][];

            int t = 0;
            for (int i = 0; i < parameters[0].Length; i++)
            {
                double mean = parameters[0][i];
                for (int j = 0; j < parameters[1].Length; j++)
                {
                    double standardDeviation = parameters[1][j];
                    for (int k = 0; k < parameters[2].Length; k++)
                    {
                        double skew = parameters[2][k];
                        tests[t] = new double[3] { mean, standardDeviation, skew };
                        t++;
                    }
                }
            }
            return tests;
        }

        private static double[] GenerateTestProbabilities(int nProbabilities = 8)
        {
            double[] probabilities = new double[nProbabilities];
            if (nProbabilities == 8)
            {
                //Assumes non-equally spaced is desired.
                probabilities = Utilities.DataGenerators.ProbabilitiesGenerator(true, nProbabilities);
            }
            else
            {
                probabilities = Utilities.DataGenerators.ProbabilitiesGenerator(false, nProbabilities);
            }
            return probabilities;
        }

        private static double[][] GenerateTestsFlows(double[][] tests, double[] probabilities)
        {
            int periodOfRecord = 100;

            int nTests = tests.Length;
            double[][] cSharpTestsFlows = new double[nTests][];
            for (int t = 0; t < nTests; t++)
            {
                double[] testFlows = new double[probabilities.Length];
                
                Statistics.LogPearsonIII testFunction = new Statistics.LogPearsonIII(tests[t][0], tests[t][1], tests[t][2], periodOfRecord);
                for (int p = 0; p < probabilities.Length; p++)
                {
                    testFlows[p] = testFunction.getDistributedVariable(1 - probabilities[p]);
                }
                cSharpTestsFlows[t] = testFlows;
            }
            return cSharpTestsFlows;
        }

        #endregion


        #region Voids
        public void GenerateRTestFile()
        {
            int numberOfTests = testsFlows.Length;

            string[] columnHeaders = new string[numberOfTests + 1];
            for (int t = 0; t < columnHeaders.Length; t++)
            {
                if (t != 0)
                {
                    columnHeaders[t] = "Test " + t;
                }
                else
                {
                    columnHeaders[t] = "Test Parameters";
                }
            }

            object[][] exportData = new object[numberOfTests + 1][];
            for (int t = 0 ; t < numberOfTests; t++)
            {
                List<object> columnData = new List<object>();
                if (t == 0)
                {
                    List<object> description = new List<object>();
                    description.AddRange(new object[4] { "mean", "standard deviation", "skew", "period of record" });
                    description.AddRange(testsProbabilities.Cast<object>().ToArray());
                    exportData[t] = description.ToArray();
                    columnData.AddRange(testsParameters[t].Cast<object>().ToArray());
                    columnData.Add("C# Flows");
                    columnData.AddRange(testsFlows[t].Cast<object>().ToArray());
                }
                else
                {
                    columnData.AddRange(testsParameters[t].Cast<object>().ToArray());
                    columnData.Add("C# Flows");
                    columnData.AddRange(testsFlows[t].Cast<object>().ToArray());
                }
                exportData[t + 1] = columnData.ToArray();    
            }
            Utilities.DataExporter.ExportDelimitedColumns(filePath, exportData, columnHeaders);   
        }
        #endregion

    }
}
