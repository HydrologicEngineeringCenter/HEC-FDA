using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaTester.ModelTester.Utilities
{
    public static class DataGenerators
    {

        #region Functionss
        /// <summary>
        /// Generates a set of probabilities for testing.
        /// </summary>
        /// <param name="standardProbabilities"> True if the standard set of 8 exceedance probabilies {0.50, 0.10, 0.04, 0.02, 0.01, 0.005, 0.002} are desired. False if a set of n equally spaced exceedance probabilites are desired.  </param>
        /// <param name="nProbabilities"> The desired number of equally spaced exceedance probabilities. </param>
        /// <returns> An array of double with values between (0, 1) exclusive. </returns>
        public static double[] ProbabilitiesGenerator(bool standardProbabilities = true, int nProbabilities = 8)
        {
            if (standardProbabilities == true)
            {
                return new double[8] { 0.5f, 0.2f, 0.1f, 0.04f, 0.02f, 0.01f, 0.005f, 0.002f };
            }
            else
            {
                double dx = (double) 1 / nProbabilities, p = dx;
                double[] probabilities = new double[nProbabilities];
                for (int i = 0; i < nProbabilities; i ++)
                {
                    if (i == 0)
                    {
                        probabilities[i] = 0.001f;
                    }
                    else
                    {
                        probabilities[i] = p;
                        p += dx;
                    }
                    //dx += dx;
                }
                return probabilities;
            }
        }

        /// <summary>
        /// Calls ProbabilitiesGenerator but returns an array of float suitable for use in Fortran.
        /// </summary>
        /// <param name="standardProbabilities"> True if the standard set of 8 exceedance probabilies {0.50, 0.10, 0.04, 0.02, 0.01, 0.005, 0.002} are desired. False if a set of n equally spaced exceedance probabilites are desired. </param>
        /// <param name="nProbabilities"> The desired number of equally spaced exceedance probabilities. </param>
        /// <returns> An array of float with values between (0, 1) exclusive. </returns>
        public static float[] FortranProbabilitiesGenerator(bool standardProbabilities = true, int nProbabilities = 8)
        {
            return ProbabilitiesGenerator(standardProbabilities, nProbabilities).Cast<float>().ToArray();
        }

        #endregion

    }
}
