using Functions;
using Functions.CoordinatesFunctions;
using Statistics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace FunctionsTests.ExcelTesting
{
    [ExcludeFromCodeCoverage]

    public class ExcelCoordinatesFunctionVariableYsTests
    {

        private const string _TestDataRelativePath = "ExcelTesting\\ExcelData\\CoordinatesFunctionVariableYs.xlsx";




        [Theory]
        [ExcelVariableYsData(_TestDataRelativePath, 1)]
        public void ExcelOrderTests<T>(
            List<double> xs1,
            string distType,
            List<double> min,
            List<double> max,
            List<double> mode,
            List<double> mean,
            List<double> stDev,
            List<double> alpha,
            List<double> beta,
            List<double> location,
            List<double> scale,
            string interpolation,
            int rowToWriteTo, int columnToWriteTo)
        {
            IDistributionEnum dist = ConvertToDistEnum(distType);
            InterpolationEnum interp = ConvertToInterpolationEnum(interpolation);
            ICoordinatesFunction func = CreateCoordinatesFunction(xs1, dist, min, max, mode, mean, stDev, alpha, beta, location, scale, interp);



            Assert.True(false);

        }



        private ICoordinatesFunction CreateCoordinatesFunction(
            List<double> xs1,
            IDistributionEnum distType,
            List<double> min,
            List<double> max,
            List<double> mode,
            List<double> mean,
            List<double> stDev,
            List<double> alpha,
            List<double> beta,
            List<double> location,
            List<double> scale,
            InterpolationEnum interpolation)
        {
            int numVals = xs1.Count;
            List<IDistributedOrdinate> distOrds = new List<IDistributedOrdinate>();

            switch(distType)
            {
                case IDistributionEnum.Normal:
                    {
                        distOrds = CreateNormalOrds(mean, stDev);
                        break;
                    }
            }
            return ICoordinatesFunctionsFactory.Factory(xs1, distOrds, interpolation);


            //for(int i = 0;i<xs1.Count;i++)
            //{
            //    distOrds.Add(CreateOrdinate(distType, min[i], max[i], mode[i], mean[i], stDev[i], alpha[i], beta[i], location[i], scale[i]));
            //}
        }

        //private void EvenOutLists(ref List<double> vals, int numValsShouldBe)
        //{
        //    //the lists should either be the correct number or zero when they come in here
        //    if(vals.Count == 0)
        //    {
        //        for(int i = 0;i<numValsShouldBe;i++)
        //        {
        //            vals.Add(-1);
        //        }
        //    }
        //}

        private List<IDistributedOrdinate> CreateNormalOrds(List<double> means, List<double> stDevs)
        {
            List<IDistributedOrdinate> distOrds = new List<IDistributedOrdinate>();
            for (int i = 0; i < means.Count; i++)
            {
                distOrds.Add(IDistributedOrdinateFactory.FactoryNormal(means[i], stDevs[i]));
            }
            return distOrds;
        }



        //private IDistributedOrdinate CreateOrdinate(
        //    IDistributionEnum distType,
        //    double min,
        //    double max,
        //    double mode,
        //    double mean,
        //    double stDev,
        //    double alpha,
        //    double beta,
        //    double location,
        //    double scale)
        //{
        //    switch (distType)
        //    {
        //        case IDistributionEnum.Normal:
        //            {
        //                return IDistributedOrdinateFactory.FactoryNormal(mean, stDev);
        //            }
        //        case IDistributionEnum.Triangular:
        //            {
        //                return IDistributedOrdinateFactory.FactoryTriangular(mode, min, max);
        //            }
        //        case IDistributionEnum.Uniform:
        //            {
        //                return IDistributedOrdinateFactory.FactoryUniform(min, max);
        //            }
        //        case IDistributionEnum.TruncatedNormal:
        //            {
        //                return IDistributedOrdinateFactory.FactoryTruncatedNormal(mean, stDev, min, max);
        //            }
        //        default:
        //            {
        //                throw new NotImplementedException("Cody hasn't implemented that dist type in Excel...VariableYsTests");
        //            }
        //    }
        //}

            private IDistributionEnum ConvertToDistEnum(string dist)
        {
            if(dist.ToUpper().Equals("NORMAL"))
            {
                return IDistributionEnum.Normal;
            }
            if (dist.ToUpper().Equals("UNIFORM"))
            {
                return IDistributionEnum.Uniform;
            }
            if (dist.ToUpper().Equals("TRIANGULAR"))
            {
                return IDistributionEnum.Triangular;
            }
            if (dist.ToUpper().Equals("TRUNCATEDNORMAL"))
            {
                return IDistributionEnum.TruncatedNormal;
            }
            throw new NotImplementedException("cody did not implement.");
        }

        private InterpolationEnum ConvertToInterpolationEnum(string interp)
        {

            if (interp.ToUpper().Equals("LINEAR"))
            {
                return InterpolationEnum.Linear;
            }
            else if (interp.ToUpper().Equals("PIECEWISE"))
            {
                return InterpolationEnum.Piecewise;
            }
            else if (interp.ToUpper().Equals("NONE"))
            {
                return InterpolationEnum.None;
            }
            else if (interp.ToUpper().Equals("NATURALCUBICSPLINE"))
            {
                return InterpolationEnum.NaturalCubicSpline;
            }
            else throw new ArgumentException("could not convert '" + interp + "'.");
        }


    }
}
