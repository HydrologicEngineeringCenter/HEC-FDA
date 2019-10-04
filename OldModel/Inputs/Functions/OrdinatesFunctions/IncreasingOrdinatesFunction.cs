
using System;
using FdaModel.Utilities.Attributes;
using FdaModel.Utilities.Messager;

namespace FdaModel.Inputs.Functions.OrdinatesFunctions
{
    [Author("John Kucharski", "08/10/2016", "08/22/2016")]
    public class IncreasingOrdinatesFunction : OrdinatesFunction
    {
        #region Notes

        #endregion

        #region Fields
        protected float[] _Xs;
        protected float[] _Ys;
        #endregion

        #region Properties
        public float[] Xs
        {
            get
            {
                return _Xs;
            }
            set
            {
                _Xs = value;
            }
        }
        public float[] Ys
        {
            get
            {
                return _Ys;
            }
            set
            {
                _Ys = value;
            }
        }
        #endregion

        #region Constructor
        [Tested(false)]
        /// <summary>
        /// This constructor is not intended for use. If used it will attempt to call the OrdinatesFunctionFactory, provided that the data is incresing along its range.
        /// </summary>
        /// <param name="xs"> Data passed as a representation of the function's domain. </param>
        /// <param name="ys"> Data passed as a represnetation of the function's range.  </param>
        internal IncreasingOrdinatesFunction(ComputationPoint computationPoint, float[] xs, float[] ys, FunctionTypes functionType) : base(computationPoint, functionType)
        {
            if (IsIncreasingFunction(ys) == false)
            {
                //Logger.WriteSingleMessageToLogFile(new ErrorMessage("The data you provided is not monotonically increasing on its range. As a result it cannot be cast as an increasing ordinates function."
                //                                                    , ErrorMessageEnum.Major));
                OrdinatesFunctionFactory(computationPoint, xs, ys, functionType);
            }
            IndexPoint = computationPoint;
            Xs = xs;
            Ys = ys;
            FunctionType = functionType;
        }
        #endregion

        #region Functions
        public override bool Validate()
        {
            throw new NotImplementedException();    
        }

        [Tested(false)]
        /// <summary> Since increasing ordinates functions have no uncertainty parameters the function in memory is copied.  </summary>
        /// <param name="randomNumberGnerator"></param>
        /// <returns> A warning message indicating that the function had no uncertainty parameters and therefore orginal function was returned. </returns>
        public override BaseFunction SampleFunction(Random randomNumberGnerator = null)
        {
            return this;
        }

        [Tested(false)]
        /// <summary> Takes the increasing ordinates function and another function provided as a parameter. Creates a new increasing ordinates function through compositions, such that the new function's ordinates are an array of (x, y = f(g(x)) coordinate pairs. </summary>
        /// <param name="function"> A function that shares a set of ordinates with the increasing ordinates function held in memory. </param>
        /// <returns> A new increasing ordinates function with the X ordinates from one function and the Y ordinates from an increasing ordinates function. </returns>
        public IncreasingOrdinatesFunction ComposeNewFunction(BaseFunction function)
        {
            BaseFunction functionProvidingXOrdinates;
            IncreasingOrdinatesFunction functionProvidingYOrdinates;
            
            //0. Make sure they share a computation point.
            string messageBuilder = "A " + this.FunctionType.ToString() + " function and a " + function.FunctionType.ToString() + " function were provided";
            if (IndexPoint != function.IndexPoint)
            {
                Logger.Instance.ReportMessage(new ErrorMessage(messageBuilder + ", with the following two different computation points: [" +
                                                      IndexPoint.Name.ToString() + ", " + IndexPoint.Stream.ToString() + ", " + IndexPoint.Condition.ToString() + ", " + IndexPoint.AnalysisYear.ToString() + "], and [" +
                                                      function.IndexPoint.Name.ToString() + ", " + function.IndexPoint.Stream.ToString() + ", " + function.IndexPoint.Condition.ToString() + ", " + function.IndexPoint.AnalysisYear.ToString() + "]. " +
                                                      "Composition cannot be performed on functions with different computation points."
                                                      , ErrorMessageEnum.Fatal));
                return null;
            }
            else
            {
                //1. Determine inner and outer functions.                
                if ((int)this.FunctionType == (int)function.FunctionType - 1)
                {
                    //a. object in memory to provide X ordiantes (it is before the parameter function in the compute sequence)
                    functionProvidingXOrdinates = this;
                    functionProvidingYOrdinates = (IncreasingOrdinatesFunction)function;
                }
                else if ((int)this.FunctionType == (int)function.FunctionType + 1)
                {
                    //b. object im memory to provide Y ordinates (it is the next function in the compute sequence).
                    if (function.GetType() == typeof(IncreasingOrdinatesFunction))
                    {
                        functionProvidingXOrdinates = (IncreasingOrdinatesFunction)function;
                        functionProvidingYOrdinates = this;
                    }
                    //c. Y ordinates are not monotonically increasing ordiantes function.
                    else
                    {
                        Logger.Instance.ReportMessage(new ErrorMessage(messageBuilder + "These functions cannot be composed. The " + function.FunctionType.ToString() + " providing the new function's Y ordinates must be an monotonically increasing ordinates function."
                                                              , ErrorMessageEnum.Fatal));
                        return null;
                    }
                }
                //d. the functions probably don't share a set of ordinates... at a minimum an inner and outer don't exist within the context of FDA (they are equal or don't have a x, x + 1 relationship in the compute sequence.
                else
                {
                    Logger.Instance.ReportMessage(new ErrorMessage(messageBuilder + "These functions cannot be composed. Composition can only occur if the range (e.g.Y coordinates) from one function be mapped to the domain (e.g.X coordinates) of the other."
                                                          , ErrorMessageEnum.Fatal));
                    return null;
                }

                float[] newXs = new float[functionProvidingYOrdinates.Ys.Length];
                //2. Perform composition on 2 increasing ordinates functions
                if (functionProvidingXOrdinates.GetType() == typeof(IncreasingOrdinatesFunction))
                {
                    //a. loop over NewYs to fill in matching Xs.
                    IncreasingOrdinatesFunction xOrdinatesFunction = (IncreasingOrdinatesFunction)functionProvidingXOrdinates;
                    for (int i = 0; i < functionProvidingYOrdinates.Xs.Length; i++)
                    {
                        //b. loop over matching ordinates in newXs to find set for interpolation 
                        for (int j = 0; j < xOrdinatesFunction.Ys.Length; j++)
                        {
                            //i. the X associated with the newY is smaller than the smallest new X 
                            if (functionProvidingYOrdinates.Xs[i] < xOrdinatesFunction.Ys[0])
                            {
                                Logger.Instance.ReportMessage(new ErrorMessage("The new function's Y ordinate, " + functionProvidingYOrdinates.Ys[i].ToString() + "is associated with a value of, " + functionProvidingYOrdinates.Xs[i].ToString() + ". " +
                                                                      "The smallest value in the function providing the X ordinates, set of matching (e.g. Y coordinates) is, " + xOrdinatesFunction.Ys[0].ToString() + ". " +
                                                                      "The new function's Y ordiante, " + functionProvidingYOrdinates.Ys[i].ToString() + " will be matched with the minimum X value of, " + xOrdinatesFunction.Xs[0].ToString() + ". " +
                                                                      "Not providing an appropriate set of interpolation coordinates (e.g. ordinates in the function providing X ordiantes that can be positively associated with the new Y value) " +
                                                                      "is likely an error and may result in inaccurate or nonsensical results. " +
                                                                      "It is recommended that you extend the lower limit of the " + functionProvidingXOrdinates.FunctionType.ToString() + " function's domain, to improve your model's performance."
                                                                      , ErrorMessageEnum.Minor));
                                newXs[i] = xOrdinatesFunction.Xs[0];
                                continue;
                            }
                            //ii. the X associated with the newY is larger than the largest new X
                            else if (functionProvidingYOrdinates.Xs[i] > xOrdinatesFunction.Ys[xOrdinatesFunction.Xs.Length])
                            {
                                Logger.Instance.ReportMessage(new ErrorMessage("The new function's Y ordinate, " + functionProvidingYOrdinates.Ys[i].ToString() + "is associated with a value of, " + functionProvidingYOrdinates.Xs[i].ToString() + ". " +
                                                                      "The largest value in the function providing the X ordinates, set of matching coordinates (e.g. Y coordinates) is, " + xOrdinatesFunction.Ys[xOrdinatesFunction.Xs.Length - 1].ToString() + ". " +
                                                                      "The new function's Y ordiante, " + functionProvidingYOrdinates.Ys[i].ToString() + "will be matched with the maximum X value of, " + xOrdinatesFunction.Xs[xOrdinatesFunction.Xs.Length - 1].ToString() + ". " +
                                                                      "This truncation of the new function to the maximum point of the function providing the new function's X ordinates is likely an error and may result in inaccurate or nonsensical results. " +
                                                                      "It is recommended that you extend the upper limit of the " + functionProvidingXOrdinates.FunctionType.ToString() + " function's domain, to improve your model's performance."
                                                                      , ErrorMessageEnum.Minor));
                                newXs[i] = xOrdinatesFunction.Xs[xOrdinatesFunction.Xs.Length - 1];
                                continue;
                            }
                            //iii. the X associated with the newY is between two new Xs... find 2 and interpolate
                            else
                            {
                                while (functionProvidingYOrdinates.Xs[i] > xOrdinatesFunction.Ys[j])
                                {
                                    continue;
                                }
                                double dx = (functionProvidingYOrdinates.Xs[i] - xOrdinatesFunction.Ys[j - 1]) / (xOrdinatesFunction.Ys[j] - xOrdinatesFunction.Ys[j - 1]);
                                newXs[i] = (float)(xOrdinatesFunction.Xs[i - 1] + dx * (xOrdinatesFunction.Xs[i] - xOrdinatesFunction.Xs[i - 1]));
                            }
                        }
                    }
                    return new IncreasingOrdinatesFunction(IndexPoint, newXs, functionProvidingYOrdinates.Ys, functionProvidingYOrdinates.FunctionType + 1);
                }

                //3. Comopose a frequency and increasing ordinates function
                else if (functionProvidingXOrdinates.GetType() == typeof(FrequencyFunctions.FrequencyFunction))
                {
                    FrequencyFunctions.FrequencyFunction XOrdinatesFunction = (FrequencyFunctions.FrequencyFunction)functionProvidingXOrdinates;
                    for (int i = 0; i < functionProvidingYOrdinates.Ys.Length; i++)
                    {
                        newXs[i] = 1 - (float)XOrdinatesFunction.Function.GetCDF(functionProvidingYOrdinates.Xs[i]);
                    }
                    return new IncreasingOrdinatesFunction(IndexPoint, newXs, functionProvidingYOrdinates.Ys, functionProvidingYOrdinates.FunctionType + 1);
                }
                else
                {
                    Logger.Instance.ReportMessage(new ErrorMessage(messageBuilder + "The function providing the X Ordinates is not cast as an increasing ordinates or frequency function, resulting in an error."
                                                          , ErrorMessageEnum.Fatal));
                    return null;
                }
            }   
        }

        #endregion

        #region Voids
        //[Tested(false)]
        ///// <summary>
        ///// Generates an ordinates function on the specified interval of the stage-frequency function domain. Information about the shape of the frequency function between the output paired data ordinates is lost.
        ///// </summary>
        ///// <param name="minimumExceedanceFrequency"> Minimum value on domain of the new ordinates function. </param>
        ///// <param name="maximumExceedanceFrequency"> Maximum value on domain of the new ordinates function. </param>
        ///// <param name="numberOfOrdinates"> Number of paired data ordinates to generate, equally spaced along range of the frequency function. </param>
        ///// <returns> A new ordiantes function object based on the frequency function. </returns>
        //public object CreateOrdinatesFunctionFromInterval<T>(FrequencyFunctions.FrequencyFunction<T> function, double minimumExceedanceFrequency = 0.00001, double maximumExceedanceFrequency = 0.99999, int numberOfOrdinates = 200)
        //{
        //    if (FunctionType != FunctionTypes.InflowFrequency)
        //    {
        //        Utilities.Messager.Logger.ReportMessage(new Utilities.Messager.ErrorMessage("This function is only valid for stage-frequency functions. " +
        //                                                                                    "If the data produces a valid function of paired data a monotonically increasing ordinate function will be generated instead."
        //                                                  , Utilities.Messager.ErrorMessageEnum.Minor));
        //        return null;
        //    }
        //    else
        //    {
        //        // These variable (e.g. maxY, minY) work ONLY 
        //        double maxY = Function.getDistributedVariable(minimumExceedanceFrequency);
        //        double minY = Function.getDistributedVariable(maximumExceedanceFrequency);
        //        double dy = (maxY - minY) / numberOfOrdinates;

        //        float[] xs = new float[numberOfOrdinates];
        //        float[] ys = new float[numberOfOrdinates];
        //        for (int i = 0; i < numberOfOrdinates; i++)
        //        {
        //            ys[i] = (float)(minY + dy * i);               // I know the first and last X & Y values at this point, so I could probably speed this up slightly be defining those directly.
        //            xs[i] = (float)(1 - Function.GetCDF(ys[i]));  // Need to check this: 1. documentation for GetCDF is confusing, 2. I need 1 minus the probability I hope it returns too, I think.
        //        }                                                 // Inconsitencies in statistics are responsible for the implicit type conversion (float).
        //        return OrdinatesFunctionFactory(IndexPoint, xs, ys, FunctionTypes.ExteriorStageFrequency);
        //    }
        //}


        #endregion
    }
}
