using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaTester.ModelTester.UnitTests.Functions
{
    public class OrdinatesFunctionGenerator
    {
        #region Fields
        private FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction _OrdinatesFunction;
        #endregion

        #region Properties
        public FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction OrdinatesFunction
        {
            get
            {
                return _OrdinatesFunction;
            }
            private set
            {
                _OrdinatesFunction = value;
            }
        }
        #endregion

        #region Constructors
        public OrdinatesFunctionGenerator(double[] xs, double[] ys, FdaModel.Functions.FunctionTypes functionType)
        {
            OrdinatesFunction = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(xs, ys, functionType);
        }
        #endregion

        #region Functions
        public static OrdinatesFunctionGenerator FromFrequencyFunctionOrdinatesGenerator(FdaModel.Functions.FrequencyFunctions.FrequencyFunction functionToMatch, FdaModel.Functions.FunctionTypes ordinatesType)
        {
            Random randomNumberGenerator = new Random();
            int nPoints = randomNumberGenerator.Next(5, 20);
            List<double> xs = new List<double>(nPoints + 1), ys = new List<double>(nPoints + 1);
            double ysMin, ysEpsilon; 

            switch (ordinatesType)
            {
                case FdaModel.Functions.FunctionTypes.Rating:
                    ysMin = randomNumberGenerator.Next(0, 5000) + randomNumberGenerator.NextDouble();
                    ysEpsilon = randomNumberGenerator.Next(0, 20) + randomNumberGenerator.NextDouble();
                    break;
                case FdaModel.Functions.FunctionTypes.InteriorStageDamage:
                    ysMin = 0;
                    ysEpsilon = randomNumberGenerator.Next(0, 1000) + randomNumberGenerator.NextDouble();
                    break;
                default:
                    throw new Exception("Add a range to the FromFrequencyFunctionOrdinatesGenerator (in the switch case statement) for the type of ordinates function you wish to generate.");
            }



            double p, pEpsilon, pRange;
            for (int i = 0; i < nPoints + 1; i++)
            {
                if (i == 0)
                {
                    
                    p = (double) randomNumberGenerator.Next(1, 20) / 100;
                    xs.Add((float)functionToMatch.Function.getDistributedVariable(p));
                    ys.Add(ysMin);
                }
                else
                {
                    p = functionToMatch.Function.GetCDF(xs[i - 1]);

                    pRange = (1 - p) * 100;
                    if (pRange < 0.1)
                    {
                        break;
                    }
                    else
                    {
                        if (i == nPoints)
                        {
                            pEpsilon = (double)randomNumberGenerator.Next(Math.Min(1, (int)(pRange * 0.5)), (int)pRange) / 100;
                        }
                        else
                        {
                            pEpsilon = (double)randomNumberGenerator.Next(Math.Min(1, (int)(pRange * 0.5)), Math.Min((int)pRange, 30)) / 100;
                        }
                        xs.Add((float)functionToMatch.Function.getDistributedVariable(p + pEpsilon));
                        ys.Add(ys[i - 1] + randomNumberGenerator.Next(1, (int)ysEpsilon) + (float)randomNumberGenerator.NextDouble());
                    }
                }
            }
            return new OrdinatesFunctionGenerator(xs.ToArray(), ys.ToArray(), ordinatesType);
        }

        public static OrdinatesFunctionGenerator FromRelatedOrdinatesFunctionOrdinatesGenerator(FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction functionToMatch, FdaModel.Functions.FunctionTypes ordinatesType)
        {
            Random randomNumberGenerator = new Random();
            int nPoints = randomNumberGenerator.Next(5, 20);
            List<double> xs = new List<double>(nPoints), ys = new List<double>(nPoints);
            double yMin, yEpsilon;

            switch (ordinatesType)
            {
                case FdaModel.Functions.FunctionTypes.Rating:
                    yMin = randomNumberGenerator.Next(0, 5000) + (float)randomNumberGenerator.NextDouble();
                    yEpsilon = randomNumberGenerator.Next(0, 20) + (float)randomNumberGenerator.NextDouble();
                    break;
                case FdaModel.Functions.FunctionTypes.InteriorStageDamage:
                    yMin = 0;
                    yEpsilon = randomNumberGenerator.Next(0, 1000) + (float)randomNumberGenerator.NextDouble();
                    break;
                default:
                    throw new Exception("Add a range to the FromFrequencyFunctionOrdinatesGenerator (in the switch case statement) for the type of ordinates function you wish to generate.");
            }

            double xMin = functionToMatch.Function.get_Y(0), xMax = functionToMatch.Function.get_Y(functionToMatch.Function.Count - 1), xRange, xEpsilon;
            for (int i = 0; i < nPoints + 1; i++)
            {
                if (i == 0)
                {
                    xs.Add(randomNumberGenerator.Next((int)(xMin - ((xMax - xMin) * 0.10)), (int)(functionToMatch.Function.get_Y(2))) + (float)randomNumberGenerator.NextDouble());
                    ys.Add(yMin);
                }
                else
                {
                    xRange = xMax - xs[i - 1];
                    if (i == nPoints)
                    {
                        xEpsilon = (double)randomNumberGenerator.Next(1, (int)xRange * 100) / 100;
                    }
                    else
                    {
                        xEpsilon = (double)randomNumberGenerator.Next(1, (int)(Math.Min(xRange, (xMax - xMin) * 0.10) * 100)) / 100;
                    }
                    xs.Add(xs[i - 1] + (float)xEpsilon);
                    ys.Add(ys[i - 1] + (float)randomNumberGenerator.Next(0, (int)yEpsilon * 100) / 100);
                }

            }
            return new OrdinatesFunctionGenerator(xs.ToArray(), ys.ToArray(), ordinatesType);
        }
        #endregion

    }
}
