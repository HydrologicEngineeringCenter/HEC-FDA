using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaTester.ModelTester.UnitTests.Functions
{
    public class FrequencyFunctionGenerator
    {
        #region Fields
        private FdaModel.Functions.FrequencyFunctions.LogPearsonIII _LogPearsonIIIFunction;
        #endregion

        #region Properties
        public FdaModel.Functions.FrequencyFunctions.LogPearsonIII LPIIIFunction
        {
            get
            {
                return _LogPearsonIIIFunction;
            }
            private set
            {
                _LogPearsonIIIFunction = value;
            }
        }
        #endregion

        #region Constructor
        public FrequencyFunctionGenerator(double mean, double standardDeviation, double skew, int periodOfRecord)
        {
            LPIIIFunction = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(mean, standardDeviation, skew, periodOfRecord);
        }
        #endregion

        public static FrequencyFunctionGenerator LogPearsonGenerator()
        {
            Random randomNumberGenerator = new Random();
            int periodOfRecord = randomNumberGenerator.Next(200);
            double mean = randomNumberGenerator.Next(3) + randomNumberGenerator.NextDouble(), standardDeviation = randomNumberGenerator.Next(2) + randomNumberGenerator.NextDouble(), skew = randomNumberGenerator.Next(-2, 2) + randomNumberGenerator.NextDouble();

            return new FrequencyFunctionGenerator(mean, standardDeviation, skew, periodOfRecord);
        }
    }
}
