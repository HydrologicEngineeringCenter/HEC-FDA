using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using FdaModel.Utilities.Messager;

namespace FdaModel.Functions.OrdinatesFunctions
{
    //[Author(q0heccdm, 3 / 9 / 2017 9:08:23 AM)]
    class OrdinatesFunction2:BaseFunction
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 3/9/2017 9:08:23 AM
        #endregion
        #region Fields
        private Statistics.CurveIncreasing _IncreasingFunction;
        private List<string> _Errors;
        #endregion
        #region Properties
        public List<string> Errors
        {
            get { return _Errors; }
            set { _Errors = value; }
        }
        public Statistics.CurveIncreasing IncreasingFunction
        {
            get { return _IncreasingFunction; }
            set { _IncreasingFunction = value; }
        }
        #endregion
        #region Constructors
        public OrdinatesFunction2(Statistics.CurveIncreasing function, FunctionTypes functionType):base()
        {
            IncreasingFunction = function;
            FunctionType = functionType;
        }
        /// <summary> Creates a representation of a function consisting of paired x and y values. </summary>
        /// <param name="xs"> Array of data passed as a representation of the function's domain. </param>
        /// <param name="ys"> Array of data passed as a representation of the function's range.  </param>
        /// <param name="functionType"> An enum descibing the type of function, the that X and Y parameters describe. </param>
        /// <remarks> The data must produce a function of ordered pairs. In other words the paired data must be sorted with respect to the independent variable (xs) and meet the definition of a function meaning each independent variable value (e.g. x) must be paired with at most one depedent variable value (e.g. y). </remarks>
        public OrdinatesFunction2(float[] xs, float[] ys, FunctionTypes functionType) : base()
        {
            double[] arrayXs = Array.ConvertAll(xs, x => (double)x);
            double[] arrayYs = Array.ConvertAll(ys, y => (double)y);
            IncreasingFunction = new Statistics.CurveIncreasing(arrayXs,arrayYs, true, false); //the function will be validated here
            FunctionType = functionType;

            ValidateOrdinatesFunction();

            if (FunctionType == FunctionTypes.LeveeFailure)
            {
                ValidateFailureFunction();
            }

            if (IncreasingFunction.IsValid == false)
            {
                Errors = IncreasingFunction.GetErrors();
                FunctionType = FunctionTypes.UnUsed;
            }


            


            //Messages = new ModelErrors();

            //Validate();
            //if (Messages.IsValid == true)
            //{
            //    FunctionType = functionType;
            //}
            //else
            //{
            //    FunctionType = FunctionTypes.UnUsed;
            //}
        }
        

       
        #endregion
        #region Voids

        private void ValidateOrdinatesFunction()
        {
            List<ErrorMessage> errors = new List<ErrorMessage>();

            if (IncreasingFunction.Xs.Length < 2 | IncreasingFunction.Ys.Length < 2 )
            {
                Messages.AddMessage(new ErrorMessage("The domain and range (e.g. X and Y ordinates) must contain two or more equal length data pairs. The provided data does not; it will be stored in an unusable state and must be edited before it can be used in a compute.", ErrorMessageEnum.Fatal));
                return;
            }

            for (int i = 0; i < IncreasingFunction.Xs.Length - 1; i++)
            {
                
                if (IncreasingFunction.Xs[i + 1] == IncreasingFunction.Xs[i])
                {
                    if (IncreasingFunction.Ys[i + 1] == IncreasingFunction.Ys[i])
                    {
                        errors.Add(new ErrorMessage("The duplicated ordinate (" + IncreasingFunction.Xs[i] + ", " + IncreasingFunction.Ys[i] + ") has been removed.", ErrorMessageEnum.Major));
                        List<double> XsList = new List<double>(IncreasingFunction.Xs);
                        List<double> YsList = new List<double>(IncreasingFunction.Ys);
                        XsList.RemoveAt(i);
                        YsList.RemoveAt(i);
                        IncreasingFunction = new Statistics.CurveIncreasing(XsList, YsList, true, false);
                        //IncreasingFunction.Xs = XsList.ToArray();
                        //IncreasingFunction.Ys = YsList.ToArray();
                        if (XsList.Count < 2)
                        {
                            errors.Add(new ErrorMessage("The domain and range (e.g. X and Y ordinates) must contain two or more equal length data pairs. After removing duplicate entries the provided data does not; it will be stored in an unusable state and must be edited before it can be used in a compute.", ErrorMessageEnum.Fatal));
                        }
                    }
                    else
                    {
                        errors.Add(new ErrorMessage("In order to represent a function any X value must map to exactly one Y value. However, the X value: " + IncreasingFunction.Xs[i] + " is associated with the Y values: {" + IncreasingFunction.Ys[i] + ", " + IncreasingFunction.Ys[i + 1] + "}. The data will be stored in an unusable state and must be edited before it can be used in a compute.", ErrorMessageEnum.Fatal));
                    }
                }
            }
            // 3. exterior interior stage funciton cannot have interior stages that are greater than exterior stages
            if (FunctionType == FunctionTypes.ExteriorInteriorStage)
            {
                for (int i = 0; i < IncreasingFunction.Xs.Length - 1; i++)
                {
                    if (IncreasingFunction.Xs[i] < IncreasingFunction.Ys[i])
                    {
                        errors.Add(new ErrorMessage("The Exterior Interior Stage function is invalid. It is not possible to have an interior stage that is greater than an exterior stage.", ErrorMessageEnum.Fatal));
                        break;
                    }
                }
            }
            Messages.AddMessages(errors);
        }
        public void ValidateFailureFunction()
        {
            List<ErrorMessage> errors = new List<ErrorMessage>();
            for (int i = 0; i < IncreasingFunction.Xs.Length - 1; i++)
            {
                if (IncreasingFunction.Ys[i] == IncreasingFunction.Ys[i + 1])
                {
                    errors.Add(new ErrorMessage("The values " + IncreasingFunction.Xs[i] + " and " + IncreasingFunction.Xs[i + 1] + " are associated with the same probability of failure (" + IncreasingFunction.Ys[i] + "). The ordinate (" + IncreasingFunction.Xs[i] + ", " + IncreasingFunction.Ys[i] + ") associated wit the lower probability of failure has been removed from the failure function.", ErrorMessageEnum.Major));
                    List<double> XsList = new List<double>(IncreasingFunction.Xs);
                    List<double> YsList = new List<double>(IncreasingFunction.Ys);
                    XsList.RemoveAt(i);
                    YsList.RemoveAt(i);

                    IncreasingFunction = new Statistics.CurveIncreasing(XsList, YsList, true, true);
                    //IncreasingFunction.Xs = XsList.ToArray();
                    //IncreasingFunction.Ys = YsList.ToArray();
                    if (IncreasingFunction.Xs.Length < 2)
                    {
                        errors.Add(new ErrorMessage("The domain and range (e.g. X and Y ordinates) must contain two or more equal length data pairs. After removing duplicate entries the provided data does not; it will be stored in an unusable state and must be edited before it can be used in a compute.", ErrorMessageEnum.Fatal));
                    }
                }
            }


            if (IncreasingFunction.Ys[0] != 0)
            {
                errors.Add(new ErrorMessage("The levee failure function must contain a 0.0 probability of failure point. This function does not, it will saved in an uncomputable state and must be edited before it can be used in a compute.", ErrorMessageEnum.Fatal));
            }
            if (IncreasingFunction.Ys[IncreasingFunction.Ys.Length - 1] != 1)
            {
                errors.Add(new ErrorMessage("The levee failure function must contain a 1.0 probability of failure point, usually a the top of the lateral structure. This function does not, it will be saved in an uncomputable state and must be edited before it can be used in a compute.", ErrorMessageEnum.Fatal));
            }
            Messages.AddMessages(errors);
        }
        public override void Validate()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Functions
        public override BaseFunction SampleFunction(Random randomNumberGenerator = null)
        {
            return this;
        }

        public override OrdinatesFunction GetOrdinatesFunction()
        {
            return this;
        }

        public override OrdinatesFunction Compose(OrdinatesFunction Function, ref List<ErrorMessage> errors)
        {
            //0. Determine which function contributes Xs and which contributes Ys.
            int dT = Function.FunctionType - this.FunctionType;
            OrdinatesFunction xsFunction, ysFunction;
            switch (dT)
            {
                case 1:
                    ysFunction = Function;
                    xsFunction = this;
                    break;
                case -1:
                    ysFunction = this;
                    xsFunction = Function;
                    break;
                default:
                    if (Function.FunctionType == FunctionTypes.LeveeFailure)
                    {
                        ysFunction = Function;
                        xsFunction = this;
                        break;
                    }
                    else
                    {
                        if (this.FunctionType == FunctionTypes.LeveeFailure)
                        {
                            ysFunction = this;
                            xsFunction = Function;
                            break;
                        }
                        else
                        {
                            errors.Add(new ErrorMessage("The two input functions do not share a common set of ordinates, a new function cannot be composed. To perform composition the domain (x-ordiantes) of the input function providing the new function's range (y-ordinates) must match range (y-ordiantes) of the input function providing the new function's domain (x-ordinates).", ErrorMessageEnum.Fatal));
                            return null;
                        }
                    }
            }
            //1. Loop to first usable X value.
            int i, I = xsFunction.Xs.Length - 1;
            if (xsFunction.Ys[0] < ysFunction.Xs[0])
            {
                i = -1;
                do
                {
                    if (i < I)
                    {
                        i++;
                    }
                    else
                    {
                        errors.Add(new ErrorMessage("The two input functions common set of ordinates do not have overlapping values, a new function cannot be composed. Specifically, the range of the input function providing the new function's domain is on the interval: [" + xsFunction.Ys[0] + ",  " + xsFunction.Ys[xsFunction.Ys.Length - 1] + "]. The matching domain of the input function providing the new function's range is on the interval: [" + ysFunction.Xs[0] + ", " + ysFunction.Xs[ysFunction.Xs.Length - 1] + "].", ErrorMessageEnum.Fatal));
                        return null;
                    }
                } while (xsFunction.Ys[i] < ysFunction.Xs[0]);
            }
            else
            {
                i = 0;
            }
            //2. Loop over portions of xsFunction not overlapping (before) ysFunction 
            int j, J = ysFunction.Ys.Length - 1;
            if (xsFunction.Ys[0] > ysFunction.Xs[1])
            {
                j = -1;
                do
                {
                    if (j + 1 < J)
                    {
                        j++;
                    }
                    else
                    {
                        errors.Add(new ErrorMessage("The two input functions common set of ordinates do not have overlapping values, a new function cannot be composed. Specifically, the range of the input function providing the new function's domain is on the interval: [" + xsFunction.Ys[0].ToString() + ",  " + xsFunction.Ys[xsFunction.Ys.Length - 1].ToString() + "]. The matching domain of the input function providing the new function's range is on the interval: [" + ysFunction.Xs[0].ToString() + ", " + ysFunction.Xs[ysFunction.Xs.Length - 1].ToString() + "].", ErrorMessageEnum.Fatal));
                        return null;
                    }
                } while (xsFunction.Ys[0] >= ysFunction.Xs[j + 1]);

            }
            else
            {
                j = 0;
            }
            //3. Loop over overlapping portion of two functions: xsFunction.Ys[i] IS >= ysFunction.Xs[j] AND < ysFunction.Xs[j + 1]
            bool iAdded = false;

            int startingI = i, startingJ = j;
            List<float> newXs = new List<float>();
            List<float> newYs = new List<float>();

            //evaluate the first point points 
            while (ysFunction.Xs[j] < xsFunction.Ys[i] && i > 0)
            {
                newXs.Add(xsFunction.Xs[i] + (ysFunction.Xs[j] - xsFunction.Ys[i]) / (xsFunction.Ys[i + 1] - xsFunction.Ys[i]) * (xsFunction.Xs[i + 1] - xsFunction.Xs[i]));
                newYs.Add(ysFunction.Ys[j]);
                j++;
            }


            for (i = startingI; i < I; i++)
            {
                if (j == J) break;
                iAdded = false;
                for (j = startingJ; j < J; j++)
                {

                    if (ysFunction.Xs[j] < xsFunction.Ys[i])                                                            // sets the first X,Y on the i -> i + 1 interval.
                    {
                        if (ysFunction.Xs[j + 1] > xsFunction.Ys[i])
                        {
                            newXs.Add(xsFunction.Xs[i]);
                            newYs.Add(ysFunction.Ys[j] + (xsFunction.Ys[i] - ysFunction.Xs[j]) / (ysFunction.Xs[j + 1] - ysFunction.Xs[j]) * (ysFunction.Ys[j + 1] - ysFunction.Ys[j]));
                            iAdded = true;
                        }
                        else
                        {
                            if (ysFunction.Xs[j + 1] == xsFunction.Ys[i])                                              // would this mean j == j + 1 ? If so, this should be an invalid (type = 99) function.
                            {
                                newXs.Add(xsFunction.Xs[i]);
                                newYs.Add(ysFunction.Ys[j + 1]);
                                iAdded = true;
                            }
                        }
                    }
                    else
                    {

                        if (ysFunction.Xs[j] == xsFunction.Ys[i])                                                       // setting first X,Y in special case that xsFunction.Ys[i] == ysFunction.Xs[j]
                        {
                            newXs.Add(xsFunction.Xs[i]);
                            newYs.Add(ysFunction.Ys[j]);
                            iAdded = true;
                        }
                        else                                                                             // looking for additional j's on the interval i -> i + 1
                        {
                            //here we need to see if there are points from i that are between points in j
                            if (iAdded == false)
                            {
                                newXs.Add(xsFunction.Xs[i]);
                                newYs.Add(ysFunction.Ys[j - 1] + (ysFunction.Ys[j] - ysFunction.Ys[j - 1]) * (xsFunction.Ys[i] - ysFunction.Xs[j - 1]) / (ysFunction.Xs[j] - ysFunction.Xs[j - 1])); //is it possible to get here with j ==0. it would crash
                                iAdded = true;
                            }

                            if (ysFunction.Xs[j] < xsFunction.Ys[i + 1])
                            {
                                newXs.Add(xsFunction.Xs[i] + (ysFunction.Xs[j] - xsFunction.Ys[i]) / (xsFunction.Ys[i + 1] - xsFunction.Ys[i]) * (xsFunction.Xs[i + 1] - xsFunction.Xs[i]));
                                newYs.Add(ysFunction.Ys[j]);
                            }
                            else
                            {

                                break;                                                                                  // time for i + 1 to become i.
                            }
                        }
                    }
                    //j++;
                }
                startingJ = j;
            }

            //capture the last points


            while (i <= I && ysFunction.Xs[j] < xsFunction.Ys[i])
            {

                newXs.Add(xsFunction.Xs[i - 1] + (ysFunction.Xs[j] - xsFunction.Ys[i - 1]) / (xsFunction.Ys[i] - xsFunction.Ys[i - 1]) * (xsFunction.Xs[i] - xsFunction.Xs[i - 1]));
                newYs.Add(ysFunction.Ys[j]);
                i++;
            }

            while (i <= I && xsFunction.Ys[i] <= ysFunction.Xs[j])
            {
                if (xsFunction.Ys[i] < ysFunction.Xs[j])
                {
                    newXs.Add(xsFunction.Xs[i]);
                    newYs.Add(ysFunction.Ys[j - 1] + (ysFunction.Ys[j] - ysFunction.Ys[j - 1]) * (xsFunction.Ys[i] - ysFunction.Xs[j - 1]) / (ysFunction.Xs[j] - ysFunction.Xs[j - 1])); //is it possible to get here with j ==0. it would crash

                }

                else if (xsFunction.Ys[i] == ysFunction.Xs[j])
                {
                    newXs.Add(xsFunction.Xs[i]);
                    newYs.Add(ysFunction.Ys[j]);
                }
                i++;
            }




            AnalyzeComposition(xsFunction, ysFunction, ref errors);
            return new OrdinatesFunction(newXs.ToArray(), newYs.ToArray(), ysFunction.FunctionType + 1);
        }

        public override double GetXfromY(double Y, ref List<ErrorMessage> errors)
        {
            return IncreasingFunction.GetXfromY(Y);
        }
        public double GetYfromX(double X, ref List<ErrorMessage> errors)
        {
            return IncreasingFunction.GetYfromX(X);
        }
        #endregion
    }
}
