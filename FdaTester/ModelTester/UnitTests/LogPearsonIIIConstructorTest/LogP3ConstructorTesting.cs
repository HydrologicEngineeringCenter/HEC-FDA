using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaTester.ModelTester.UnitTests.LogPearsonIIIConstructorTest
{
    public class LogP3ConstructorTesting
    {
        public LogP3ConstructorTesting()
        {
            double mean = 1;
            double stDev = 1;
            double skew = 1;
            int por = 100;
            FdaModel.FrequencyFunctions.LogPearsonIII cons1 = new FdaModel.FrequencyFunctions.LogPearsonIII(mean, stDev, skew, por);

            double[] cons1Array = new double[100];

            Random rand = new Random();

            for (int i = 0; i < 100; i++)
            {
                cons1Array[i] = cons1.Function.getDistributedVariable(rand.NextDouble()); //next double between 0-1  
            }

            FdaModel.FrequencyFunctions.LogPearsonIII cons2 = new FdaModel.FrequencyFunctions.LogPearsonIII(cons1Array);

            double cons2_Mean = cons2.Function.GetMean;
            double cons2_stDev = cons2.Function.GetStDev;
            double cons2_skew = cons2.Function.GetG;

            cons1 = new FdaModel.FrequencyFunctions.LogPearsonIII(cons2_Mean, cons2_stDev, cons2_skew, por);



            // 100 times then feed into cons 2.
            //then get mean get stdev...then plug into const 1 and test that the 8 probs are identical.I will
                //be able to ask it specifically the 8 probs.


            //for cons 3: the sampleFunction will call cons 3. make sure that everything looks right. check that the values match the values from cons 1 or 2

            //then i can test Frequency Function. test createOrdinatesFunctionFromInterval make sure that t

            cons1.Function.getDistributedVariable(.5);
            cons1.SampleFunction(.2, .5);



        }
    }
}
