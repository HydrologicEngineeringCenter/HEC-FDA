using Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Ordinates
{
    public static class IOrdinateTranslator
    {

        public static IOrdinate TranslateValuesBetweenDistributionTypes(IOrdinate originalOrdinate, IOrdinateEnum newType)
        {
            IOrdinateEnum originalType = originalOrdinate.Type;
            switch (originalType)
            {
                case IOrdinateEnum.Constant:
                    {
                        return TranslateValuesFromConstantTo(originalOrdinate, newType);
                    }
                case IOrdinateEnum.Normal:
                    {
                        return TranslateValuesFromNormalTo(originalOrdinate, newType);
                    }
                case IOrdinateEnum.Uniform:
                    {
                        return TranslateValuesFromUniformTo(originalOrdinate, newType);
                    }
                case IOrdinateEnum.Triangular:
                    {
                        return TranslateValuesFromTriangularTo(originalOrdinate, newType);
                    }
                //case IOrdinateEnum.TruncatedNormal:
                //    {
                //        TranslateValuesFromTruncNormalTo(newType);
                //        break;
                //    }
                //case IOrdinateEnum.Beta4Parameters:
                //    {
                //        TranslateValuesFromBetaTo(newType);
                //        break;
                //    }
                default:
                    {
                       
                        throw new ArgumentException("Could not translate a " + originalType.ToString() + " ordinate into an ordinate of type: '" + newType.ToString() + "' because that type is not supported.");
                    }
            }
        }

        //private void TranslateValuesFromBetaTo(IOrdinateEnum newDist)
        //{
        //    double tempMin = Min;
        //    double tempMax = Max;
        //    ClearAllPropertiesExceptX();
        //    switch (newDist)
        //    {
        //        case IOrdinateEnum.TruncatedNormal:
        //        case IOrdinateEnum.Triangular:
        //        case IOrdinateEnum.Uniform:
        //            {
        //                Min = tempMin;
        //                Max = tempMax;
        //                break;
        //            }
        //    }
        //}

        private static IOrdinate TranslateValuesFromUniformTo(IOrdinate original, IOrdinateEnum newDist)
        {
            int sampleSize = ((IDistributedOrdinate)original).SampleSize;
            double min = ((IDistributedOrdinate)original).Range.Min;
            double max = ((IDistributedOrdinate)original).Range.Max;
            switch (newDist)
            {
                case IOrdinateEnum.Constant:
                    {
                        return IOrdinateFactory.Factory(min);
                    }
                case IOrdinateEnum.Normal:
                    {
                        IDistribution dist = IDistributionFactory.FactoryNormal(min, 0, sampleSize);
                        return IOrdinateFactory.Factory(dist);
                    }
                case IOrdinateEnum.TruncatedNormal:
                    {
                        IDistribution dist = IDistributionFactory.FactoryTruncatedNormal(min, 0, min, max, sampleSize);
                        return IOrdinateFactory.Factory(dist);
                    }
                case IOrdinateEnum.Triangular:
                    {
                        IDistribution dist = IDistributionFactory.FactoryTriangular(min, min, max, sampleSize);
                        return IOrdinateFactory.Factory(dist);
                    }
                default:
                    {
                        throw new ArgumentException("Could not translate a normal ordinate into an ordinate of type: '" + newDist + "' because that type is not supported.");
                    }
                    //case IOrdinateEnum.Beta4Parameters:
                    //    {
                    //        Min = tempMin;
                    //        Max = tempMax;
                    //        break;
                    //    }
            }
        }

        private static IOrdinate TranslateValuesFromTriangularTo(IOrdinate original, IOrdinateEnum newDist)
        {
            double mode = ((IDistributedOrdinate)original).Mode;
            int sampleSize = ((IDistributedOrdinate)original).SampleSize;
            double min = ((IDistributedOrdinate)original).Range.Min;
            double max = ((IDistributedOrdinate)original).Range.Max;
            switch (newDist)
            {
                case IOrdinateEnum.Uniform:
                    {
                        IDistribution dist = IDistributionFactory.FactoryUniform(min, max, sampleSize);
                        return IOrdinateFactory.Factory(dist);
                    }
                case IOrdinateEnum.Constant:
                    {
                        return IOrdinateFactory.Factory(mode);
                    }
                case IOrdinateEnum.Normal:
                    {
                        IDistribution dist = IDistributionFactory.FactoryNormal(mode,0, sampleSize);
                        return IOrdinateFactory.Factory(dist);
                    }
                case IOrdinateEnum.TruncatedNormal:
                    {
                        IDistribution dist = IDistributionFactory.FactoryTruncatedNormal(mode, 0, min, max, sampleSize);
                        return IOrdinateFactory.Factory(dist);
                    }
                default:
                    {
                        throw new ArgumentException("Could not translate a normal ordinate into an ordinate of type: '" + newDist + "' because that type is not supported.");
                    }
                    //case IOrdinateEnum.Beta4Parameters:
                    //    {
                    //        Min = tempMin;
                    //        Max = tempMax;
                    //        break;
                    //    }
            }
        }

        private static IOrdinate TranslateValuesFromTruncNormalTo(IOrdinate original, IOrdinateEnum newDist)
        {

            double mean = ((IDistributedOrdinate)original).Mean;
            double stDev = ((IDistributedOrdinate)original).StandardDeviation;
            double skew = ((IDistributedOrdinate)original).Skewness;
            int sampleSize = ((IDistributedOrdinate)original).SampleSize;
            double min = ((IDistributedOrdinate)original).Range.Min;
            double max = ((IDistributedOrdinate)original).Range.Max;


            switch (newDist)
            {
                case IOrdinateEnum.Constant:
                    {
                        return IOrdinateFactory.Factory(mean);
                    }
                case IOrdinateEnum.TruncatedNormal:
                    {
                        IDistribution dist = IDistributionFactory.FactoryNormal(mean, stDev, sampleSize);
                        return IOrdinateFactory.Factory(dist);
                    }
                case IOrdinateEnum.Triangular:
                    {
                        IDistribution dist = IDistributionFactory.FactoryTriangular(min, mean, max, sampleSize);
                        return IOrdinateFactory.Factory(dist);
                    }
                case IOrdinateEnum.Uniform:
                    {
                        IDistribution dist = IDistributionFactory.FactoryUniform(min, max, sampleSize);
                        return IOrdinateFactory.Factory(dist);
                    }
                default:
                    {
                        throw new ArgumentException("Could not translate a normal ordinate into an ordinate of type: '" + newDist + "' because that type is not supported.");
                    }
            }
        }

        private static IOrdinate TranslateValuesFromConstantTo(IOrdinate original, IOrdinateEnum newDist)
        {
            double value = original.Value();
            switch (newDist)
            {
                case IOrdinateEnum.Normal:
                    {
                        IDistribution dist = IDistributionFactory.FactoryNormal(value, 0);
                        return IOrdinateFactory.Factory(dist);
                    }
                case IOrdinateEnum.Triangular:
                    {
                        IDistribution dist = IDistributionFactory.FactoryTriangular(value - 1, value, value + 1);
                        return IOrdinateFactory.Factory(dist);
                    }
                case IOrdinateEnum.Uniform:
                    {
                        IDistribution dist = IDistributionFactory.FactoryUniform(value - 1, value);
                        return IOrdinateFactory.Factory(dist);
                    }
                default:
                    {
                        throw new ArgumentException("Could not translate a constant ordinate into an ordinate of type: '" + newDist + "' because that type is not supported.");
                    }
            }
        }

        private static IOrdinate TranslateValuesFromNormalTo(IOrdinate original, IOrdinateEnum newDist)
        {

            //todo: i should just grab the value and do the best i can with that. I don't have access to the distribution. get rid of this class

            double mean = ((IDistributedOrdinate)original).Mean;
            double stDev = ((IDistributedOrdinate)original).StandardDeviation;
            double skew = ((IDistributedOrdinate)original).Skewness;
            int sampleSize = ((IDistributedOrdinate)original).SampleSize;

            switch (newDist)
            {
                case IOrdinateEnum.Constant:
                    {
                        return IOrdinateFactory.Factory(mean);
                    }
                case IOrdinateEnum.TruncatedNormal:
                    {
                        IDistribution dist = IDistributionFactory.FactoryTruncatedNormal(mean, stDev, mean - 1, mean + 1, sampleSize);
                        return IOrdinateFactory.Factory(dist);
                    }
                case IOrdinateEnum.Triangular:
                    {
                        IDistribution dist = IDistributionFactory.FactoryTriangular(mean - 1,mean, mean + 1, sampleSize);
                        return IOrdinateFactory.Factory(dist);
                    }
                case IOrdinateEnum.Uniform:
                    {
                        IDistribution dist = IDistributionFactory.FactoryUniform(mean - 1, mean + 1,sampleSize);
                        return IOrdinateFactory.Factory(dist);
                    }
                default:
                    {
                        throw new ArgumentException("Could not translate a normal ordinate into an ordinate of type: '" + newDist + "' because that type is not supported.");
                    }

            }
        }

        

    }
}
