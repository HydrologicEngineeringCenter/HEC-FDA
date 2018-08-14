using System;
using System.Text;
using System.Collections.Generic;
using FdaModel.Utilities.Messager;
using FdaModel.Utilities.Attributes;
using FdaModel.Functions.FrequencyFunctions;


namespace FdaModel.Functions.OrdinatesFunctions
{
    [Author("John Kucharski", "06/08/2016", "02/09/2017")]
    public class OrdinatesFunction : BaseFunction
    {
        #region Notes
        /*Development Notes: 1. Need a more elegant way of dealing withe exceptions, on the OrdinatesFactory mehtod, or move this to ViewModel data validation class. */
        #endregion

        #region Fields
        private Statistics.CurveIncreasing _Function;
        private List<string> _Errors;
        private bool _IsUseable;
        
        
        #endregion


        #region Properties
        
        public bool IsUseable
        {
            get { return _IsUseable; }
            set { _IsUseable = value; }
        }
        
        public List<string> Errors
        {
            get { return _Errors; }
            set { _Errors = value; }
        }
        public Statistics.CurveIncreasing Function
        {
            get { return _Function; }
            set { _Function = value; ValidateOrdinatesFunction(); }
        }
        #endregion
        #region Constructors
        public OrdinatesFunction(Statistics.CurveIncreasing function, FunctionTypes functionType):base()
        {
           
            
            Function = function;
            FunctionType = functionType;
        }
        public OrdinatesFunction(System.Collections.ObjectModel.ReadOnlyCollection<double> xs, System.Collections.ObjectModel.ReadOnlyCollection<double> ys, FunctionTypes functionType) : base()
        {
            double[] newXs = new double[xs.Count];
            xs.CopyTo(newXs, 0);

            double[] newYs = new double[xs.Count];
            ys.CopyTo(newYs, 0);
            
            if (functionType == FunctionTypes.LeveeFailure)
            {
                Function = new Statistics.CurveIncreasing(newXs, newYs, true, true);
                
            }
            else
            {
                Function = new Statistics.CurveIncreasing(newXs, newYs, true, false);
            }
     
            FunctionType = functionType;
        }
        /// <summary> Creates a representation of a function consisting of paired x and y values. </summary>
        /// <param name="xs"> Array of data passed as a representation of the function's domain. </param>
        /// <param name="ys"> Array of data passed as a representation of the function's range.  </param>
        /// <param name="functionType"> An enum descibing the type of function, the that X and Y parameters describe. </param>
        /// <remarks> The data must produce a function of ordered pairs. In other words the paired data must be sorted with respect to the independent variable (xs) and meet the definition of a function meaning each independent variable value (e.g. x) must be paired with at most one depedent variable value (e.g. y). </remarks>
        public OrdinatesFunction(double[] xs, double[] ys, FunctionTypes functionType) : base()
        {
            //Messages = new ModelErrors();

            FunctionType = functionType;

            if(functionType == FunctionTypes.LeveeFailure)
            {
                Function = new Statistics.CurveIncreasing(xs, ys, true, true);
            }
            else
            {
                Function = new Statistics.CurveIncreasing(xs, ys, true, false);
            }


            if (IsUseable == false)
            {
                FunctionType = FunctionTypes.UnUsed;
            }

        }



        #endregion
        #region Voids

        public List<string> ReportErrors()
        {
            List<string> errors = Function.GetErrors();

            if (Function.Count < 2)
            {
                errors.Add("The domain and range (e.g. X and Y ordinates) must contain two or more equal length data pairs. The provided data does not; it will be stored in an unusable state and must be edited before it can be used in a compute.");
                
            }

            for (int i = 0; i < Function.Count - 1; i++)
            {

                if (Function.get_X(i + 1) == Function.get_X(i))
                {
                    if (Function.get_Y(i + 1) == Function.get_Y(i))
                    {

                        // is it possible to get here after validateOrdinatesFunction? Yes, if the user adds a point.
                        errors.Add("A duplicate point (" + Function.get_X(i) + "," + Function.get_Y(i) + ")  was found. This function is now unuseable.");
                    }

                }  

            }

            // 3. exterior interior stage funciton cannot have interior stages that are greater than exterior stages
            if (FunctionType == FunctionTypes.ExteriorInteriorStage)
            {
                for (int j = 0; j < Function.Count; j++)
                {
                    if (Function.get_X(j) < Function.get_Y(j))
                    {
                        errors.Add("An exterior-interior stage function cannot have interior stages that are greater than exterior stages (" + Function.get_X(j) + "," + Function.get_Y(j) + ").");
                    }
                }
            }

            if (FunctionType == FunctionTypes.LeveeFailure)
            {
                if (Function.get_Y(0) != 0)
                {
                    errors.Add("Levee failure functions must have the first point be associated with a zero probability. The current value is (" + Function.get_Y(0) + ").");

                }
            }

            return errors;
        }

       
        [Tested(true,true, @"M:\Kucharski\Public\Fda\2.0\Testing\Functions\Ordinates Functions\OrdinatesFunction\ValidateOrdinatesFunction.docx","3/21/2017","Cody McCoy")]

        private void ValidateOrdinatesFunction()
        {
            //start with isuseable true
            IsUseable = true;
            //if the function has 0 or 1 point it is not useable
            if (Function.Count < 2)
            {
                IsUseable = false;
                return;
            }

            //if the function has repeat points, it will start out false, but we will 
            //remove the duplicates wich will trigger the stats curve increasing function
            //to re validate itself.
            for (int i = 0; i < Function.Count - 1; i++)
            {
                
                if (Function.get_X(i+1) == Function.get_X(i))
                {
                    if (Function.get_Y(i + 1) == Function.get_Y(i))
                    {
                        
                        Function.RemoveAt(i);// = new Statistics.CurveIncreasing(XsList, YsList, true, false);
                        i--;
                       
                        if (Function.Count < 2)
                        {
                            IsUseable = false;
                            return;
                        }
                    }
                    
                }

                
            }

            // 3. exterior interior stage funciton cannot have interior stages that are greater than exterior stages
            if (FunctionType == FunctionTypes.ExteriorInteriorStage)
            {
                for (int j = 0; j < Function.Count; j++)
                {
                    if (Function.get_X(j) < Function.get_Y(j))
                    {
                        IsUseable = false;
                        return;
                    }
                }
            }

            if (FunctionType == FunctionTypes.LeveeFailure)
            {
                if (Function.get_Y(0) != 0)
                {
                    IsUseable = false;
                    return;
                }
            }

            IsUseable = Function.IsValid;                 
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

        [Tested(true,true, @"Functions\Ordinates Functions\OrdinatesFunction\Composition_Testing_All_Ranges.xlsx","5/16/17","Cody McCoy")]
        public override OrdinatesFunction Compose(OrdinatesFunction OrdFunction, ref List<ErrorMessage> errors)
        {
            //0. Determine which function contributes Xs and which contributes Ys.
            int dT = OrdFunction.FunctionType - this.FunctionType;
            Statistics.CurveIncreasing xsFunction, ysFunction;

            if (this.FunctionType == FunctionTypes.LeveeFailure) { }

            FunctionTypes yFunctionType; //I store this value to use at the very end when i call analyze
            switch (dT)
            {
                case 1:
                    ysFunction = OrdFunction.Function;
                    xsFunction = Function;
                    yFunctionType = OrdFunction.FunctionType;
                    break;
                case -1:
                    ysFunction = Function;
                    xsFunction = OrdFunction.Function;
                    yFunctionType = FunctionType;

                    break;
                default:
                    if (OrdFunction.FunctionType == FunctionTypes.LeveeFailure)
                    {
                        ysFunction = OrdFunction.Function;
                        xsFunction = Function;
                        yFunctionType = OrdFunction.FunctionType;

                        break;
                    }
                    else
                    {
                        if (this.FunctionType == FunctionTypes.LeveeFailure)
                        {
                            ysFunction = Function;
                            xsFunction = OrdFunction.Function;
                            yFunctionType = FunctionType;

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
            int i, I = xsFunction.Count - 1;
            if (xsFunction.get_Y(0) < ysFunction.get_X(0))
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
                        errors.Add(new ErrorMessage("The two input functions common set of ordinates do not have overlapping values, a new function cannot be composed. Specifically, the range of the input function providing the new function's domain is on the interval: [" + xsFunction.get_Y(0) + ",  " + xsFunction.get_Y(xsFunction.Count - 1) + "]. The matching domain of the input function providing the new function's range is on the interval: [" + ysFunction.get_X(0) + ", " + ysFunction.get_X(ysFunction.Count - 1) + "].", ErrorMessageEnum.Fatal));
                        return null;
                    }
                } while (xsFunction.get_Y(i) < ysFunction.get_X(0));
            }
            else
            {
                i = 0;
            }
            //2. Loop over portions of xsFunction not overlapping (before) ysFunction 
            int j, J = ysFunction.Count - 1;
            if (xsFunction.get_Y(0) > ysFunction.get_X(1))
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
                        errors.Add(new ErrorMessage("The two input functions common set of ordinates do not have overlapping values, a new function cannot be composed. Specifically, the range of the input function providing the new function's domain is on the interval: [" + xsFunction.get_Y(0).ToString() + ",  " + xsFunction.get_Y(xsFunction.Count - 1).ToString() + "]. The matching domain of the input function providing the new function's range is on the interval: [" + ysFunction.get_X(0).ToString() + ", " + ysFunction.get_X(ysFunction.Count - 1).ToString() + "].", ErrorMessageEnum.Fatal));
                        return null;
                    }
                } while (xsFunction.get_Y(0) >= ysFunction.get_X(j + 1));

            }
            else
            {
                j = 0;
            }
            //3. Loop over overlapping portion of two functions: xsFunction.Ys[i] IS >= ysFunction.Xs[j] AND < ysFunction.Xs[j + 1]
            bool iAdded = false;

            int startingI = i, startingJ = j;
            List<double> newXs = new List<double>();
            List<double> newYs = new List<double>();

            //evaluate the first point points 
            while (ysFunction.get_X(j) < xsFunction.get_Y(i) && i > 0)
            {
                newXs.Add(xsFunction.get_X(i-1) + (ysFunction.get_X(j) - xsFunction.get_Y(i-1)) / (xsFunction.get_Y(i) - xsFunction.get_Y(i-1)) * (xsFunction.get_X(i) - xsFunction.get_X(i-1)));
                newYs.Add(ysFunction.get_Y(j));
                j++;

            }


            for (i = startingI; i < I; i++)
            {
                if (j == J) break;
                iAdded = false;
                for (j = startingJ; j < J; j++)
                {

                    if (ysFunction.get_X(j) < xsFunction.get_Y(i))                                                            // sets the first X,Y on the i -> i + 1 interval.
                    {
                        if (ysFunction.get_X(j + 1) > xsFunction.get_Y(i))
                        {
                            newXs.Add(xsFunction.get_X(i));
                            newYs.Add(ysFunction.get_Y(j) + (xsFunction.get_Y(i) - ysFunction.get_X(j)) / (ysFunction.get_X(j + 1) - ysFunction.get_X(j)) * (ysFunction.get_Y(j + 1) - ysFunction.get_Y(j)));
                            iAdded = true;
                        }
                        else
                        {
                            if (ysFunction.get_X(j + 1) == xsFunction.get_Y(i))                                              // would this mean j == j + 1 ? If so, this should be an invalid (type = 99) function.
                            {
                                newXs.Add(xsFunction.get_X(i));
                                newYs.Add(ysFunction.get_Y(j + 1));
                                iAdded = true;
                                j++;
                            }
                        }
                    }
                    else
                    {

                        if (ysFunction.get_X(j) == xsFunction.get_Y(i))                                                       // setting first X,Y in special case that xsFunction.Ys[i] == ysFunction.Xs[j]
                        {
                            newXs.Add(xsFunction.get_X(i));
                            newYs.Add(ysFunction.get_Y(j));
                            iAdded = true;
                        }
                        else                                                                             // looking for additional j's on the interval i -> i + 1
                        {
                            //here we need to see if there are points from i that are between points in j
                            if (iAdded == false)
                            {
                                newXs.Add(xsFunction.get_X(i));
                                newYs.Add(ysFunction.get_Y(j - 1) + (ysFunction.get_Y(j) - ysFunction.get_Y(j - 1)) * (xsFunction.get_Y(i) - ysFunction.get_X(j - 1)) / (ysFunction.get_X(j) - ysFunction.get_X(j - 1))); //is it possible to get here with j ==0. it would crash
                                iAdded = true;
                            }

                            if (ysFunction.get_X(j) < xsFunction.get_Y(i + 1))
                            {
                                newXs.Add(xsFunction.get_X(i) + (ysFunction.get_X(j) - xsFunction.get_Y(i)) / (xsFunction.get_Y(i + 1) - xsFunction.get_Y(i)) * (xsFunction.get_X(i + 1) - xsFunction.get_X(i)));
                                newYs.Add(ysFunction.get_Y(j));
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
            //how did we get here: either j = J or i = I

            if(j == J)
            {
                //In this case there is only one more j point. We need to evaluate if there are 
                // i points between the last two j points, and then if the last j point needs to be added.
                while (i <= I && xsFunction.get_Y(i) < ysFunction.get_X(j))
                {

                    newXs.Add(xsFunction.get_X(i));
                    newYs.Add(ysFunction.get_Y(j - 1) + (ysFunction.get_Y(j) - ysFunction.get_Y(j - 1)) * (xsFunction.get_Y(i) - ysFunction.get_X(j - 1)) / (ysFunction.get_X(j) - ysFunction.get_X(j - 1))); //is it possible to get here with j ==0. it would crash
                    i++;

                }

                //evaluate the last j point if there are still more i values
                if (i <= I)
                {
                    if (ysFunction.get_X(j) < xsFunction.get_Y(i))
                    {
                        newXs.Add(xsFunction.get_X(i - 1) + (ysFunction.get_X(j) - xsFunction.get_Y(i - 1)) / (xsFunction.get_Y(i) - xsFunction.get_Y(i - 1)) * (xsFunction.get_X(i) - xsFunction.get_X(i - 1)));
                        newYs.Add(ysFunction.get_Y(j));
                        i++;
                    }
                    else if (ysFunction.get_X(j) == xsFunction.get_Y(i))
                    {
                        newXs.Add(xsFunction.get_X(i)); //test this
                        newYs.Add(ysFunction.get_Y(j));
                        i++;
                    }
                }

            }
            else // this is the case that i = I
            {
                //there is one more i value left. if there are more j values then add the last i
                if (j<=J && xsFunction.get_Y(i) < ysFunction.get_X(j))
                {
                    newXs.Add(xsFunction.get_X(i));
                    newYs.Add(ysFunction.get_Y(j - 1) + (ysFunction.get_Y(j) - ysFunction.get_Y(j - 1)) * (xsFunction.get_Y(i) - ysFunction.get_X(j - 1)) / (ysFunction.get_X(j) - ysFunction.get_X(j - 1))); //is it possible to get here with j ==0. it would crash

                }
                else if(j<=J && xsFunction.get_Y(i) == ysFunction.get_X(j))
                {
                    newXs.Add(xsFunction.get_X(i));
                    newYs.Add(ysFunction.get_Y(j));
                }
            }

            //while (i <= I && ysFunction.get_X(j) < xsFunction.get_Y(i))
            //{

            //    newXs.Add(xsFunction.get_X(i - 1) + (ysFunction.get_X(j) - xsFunction.get_Y(i - 1)) / (xsFunction.get_Y(i) - xsFunction.get_Y(i - 1)) * (xsFunction.get_X(i) - xsFunction.get_X(i - 1)));
            //    newYs.Add(ysFunction.get_Y(j));
            //    i++;
            //}

            //while (i <= I && xsFunction.get_Y(i) <= ysFunction.get_X(j))
            //{
            //    if (xsFunction.get_Y(i) < ysFunction.get_X(j))
            //    {
            //        newXs.Add(xsFunction.get_X(i));
            //        newYs.Add(ysFunction.get_Y(j - 1) + (ysFunction.get_Y(j) - ysFunction.get_Y(j - 1)) * (xsFunction.get_Y(i) - ysFunction.get_X(j - 1)) / (ysFunction.get_X(j) - ysFunction.get_X(j - 1))); //is it possible to get here with j ==0. it would crash

            //    }

            //    else if (xsFunction.get_Y(i) == ysFunction.get_X(j))
            //    {
            //        newXs.Add(xsFunction.get_X(i));
            //        newYs.Add(ysFunction.get_Y(j));
            //    }
            //    i++;
            //}




            AnalyzeComposition(xsFunction, ysFunction, ref errors);
            double[] arrayXs = newXs.ToArray();  //Array.ConvertAll(newXs.ToArray(), x => (float)x);
            double[] arrayYs = newYs.ToArray();  //Array.ConvertAll(newYs.ToArray(), y => (float)y);
            return new OrdinatesFunction( arrayXs,arrayYs, yFunctionType + 1);
        }
        //public override OrdinatesFunction Compose(OrdinatesFunction Function, ref List<ErrorMessage> errors)
        //{
        //    //0. Determine which function contributes Xs and which contributes Ys.
        //    int dT = Function.FunctionType - this.FunctionType;
        //    OrdinatesFunction xsFunction, ysFunction;
        //    switch (dT)
        //    {
        //        case 1:
        //            ysFunction = Function;
        //            xsFunction = this;
        //            break;
        //        case -1:
        //            ysFunction = this;
        //            xsFunction = Function;
        //            break;
        //        default:
        //            if (Function.FunctionType == FunctionTypes.LeveeFailure)
        //            {
        //                ysFunction = Function;
        //                xsFunction = this;
        //                break;
        //            }
        //            else
        //            {
        //                if (this.FunctionType == FunctionTypes.LeveeFailure)
        //                {
        //                    ysFunction = this;
        //                    xsFunction = Function;
        //                    break;
        //                }
        //                else
        //                {
        //                    errors.Add(new ErrorMessage("The two input functions do not share a common set of ordinates, a new function cannot be composed. To perform composition the domain (x-ordiantes) of the input function providing the new function's range (y-ordinates) must match range (y-ordiantes) of the input function providing the new function's domain (x-ordinates).", ErrorMessageEnum.Fatal));
        //                    return null;
        //                }
        //            }
        //    }
        //    //1. Loop to first usable X value.
        //    int i, I = xsFunction.Xs.Length - 1;
        //    if (xsFunction.Ys[0] < ysFunction.Xs[0])
        //    {
        //        i = -1;
        //        do
        //        {
        //            if (i < I)
        //            {
        //                i++;
        //            }
        //            else
        //            {
        //                errors.Add(new ErrorMessage("The two input functions common set of ordinates do not have overlapping values, a new function cannot be composed. Specifically, the range of the input function providing the new function's domain is on the interval: [" + xsFunction.Ys[0] + ",  " + xsFunction.Ys[xsFunction.Ys.Length - 1] + "]. The matching domain of the input function providing the new function's range is on the interval: [" + ysFunction.Xs[0] + ", " + ysFunction.Xs[ysFunction.Xs.Length - 1] + "].", ErrorMessageEnum.Fatal));
        //                return null;
        //            }
        //        } while (xsFunction.Ys[i] < ysFunction.Xs[0]);
        //    }
        //    else
        //    {
        //        i = 0;
        //    }
        //    //2. Loop over portions of xsFunction not overlapping (before) ysFunction 
        //    int j, J = ysFunction.Ys.Length - 1;
        //    if (xsFunction.Ys[0] > ysFunction.Xs[1])
        //    {
        //        j = -1;
        //        do
        //        {
        //            if (j + 1 < J)
        //            {
        //                j++;
        //            }
        //            else
        //            {
        //                errors.Add(new ErrorMessage("The two input functions common set of ordinates do not have overlapping values, a new function cannot be composed. Specifically, the range of the input function providing the new function's domain is on the interval: [" + xsFunction.Ys[0].ToString() + ",  " + xsFunction.Ys[xsFunction.Ys.Length - 1].ToString() + "]. The matching domain of the input function providing the new function's range is on the interval: [" + ysFunction.Xs[0].ToString() + ", " + ysFunction.Xs[ysFunction.Xs.Length - 1].ToString() + "].", ErrorMessageEnum.Fatal));
        //                return null;
        //            }
        //        } while (xsFunction.Ys[0] >= ysFunction.Xs[j + 1]);

        //    }
        //    else
        //    {
        //        j = 0;
        //    }
        //    //3. Loop over overlapping portion of two functions: xsFunction.Ys[i] IS >= ysFunction.Xs[j] AND < ysFunction.Xs[j + 1]
        //    bool iAdded = false;

        //    int startingI = i, startingJ = j;
        //    List<float> newXs = new List<float>();
        //    List<float> newYs = new List<float>();

        //    //evaluate the first point points 
        //    while (ysFunction.Xs[j] < xsFunction.Ys[i] && i > 0)
        //    {
        //        newXs.Add(xsFunction.Xs[i] + (ysFunction.Xs[j] - xsFunction.Ys[i]) / (xsFunction.Ys[i + 1] - xsFunction.Ys[i]) * (xsFunction.Xs[i + 1] - xsFunction.Xs[i]));
        //        newYs.Add(ysFunction.Ys[j]);
        //        j++;
        //    }


        //    for (i = startingI; i < I; i++)
        //    {
        //        if (j == J) break;
        //        iAdded = false;
        //        for (j = startingJ; j < J; j++)
        //        {

        //            if (ysFunction.Xs[j] < xsFunction.Ys[i])                                                            // sets the first X,Y on the i -> i + 1 interval.
        //            {
        //                if (ysFunction.Xs[j + 1] > xsFunction.Ys[i])
        //                {
        //                    newXs.Add(xsFunction.Xs[i]);
        //                    newYs.Add(ysFunction.Ys[j] + (xsFunction.Ys[i] - ysFunction.Xs[j]) / (ysFunction.Xs[j + 1] - ysFunction.Xs[j]) * (ysFunction.Ys[j + 1] - ysFunction.Ys[j]));
        //                    iAdded = true;
        //                }
        //                else
        //                {
        //                    if (ysFunction.Xs[j + 1] == xsFunction.Ys[i])                                              // would this mean j == j + 1 ? If so, this should be an invalid (type = 99) function.
        //                    {
        //                        newXs.Add(xsFunction.Xs[i]);
        //                        newYs.Add(ysFunction.Ys[j + 1]);
        //                        iAdded = true;
        //                    }
        //                }
        //            }
        //            else
        //            {

        //                if (ysFunction.Xs[j] == xsFunction.Ys[i])                                                       // setting first X,Y in special case that xsFunction.Ys[i] == ysFunction.Xs[j]
        //                {
        //                    newXs.Add(xsFunction.Xs[i]);
        //                    newYs.Add(ysFunction.Ys[j]);
        //                    iAdded = true;
        //                }
        //                else                                                                             // looking for additional j's on the interval i -> i + 1
        //                {
        //                    //here we need to see if there are points from i that are between points in j
        //                    if (iAdded == false)
        //                    {
        //                        newXs.Add(xsFunction.Xs[i]);
        //                        newYs.Add(ysFunction.Ys[j - 1] + (ysFunction.Ys[j] - ysFunction.Ys[j - 1]) * (xsFunction.Ys[i] - ysFunction.Xs[j - 1]) / (ysFunction.Xs[j] - ysFunction.Xs[j - 1])); //is it possible to get here with j ==0. it would crash
        //                        iAdded = true;
        //                    }

        //                    if (ysFunction.Xs[j] < xsFunction.Ys[i + 1])
        //                    {
        //                        newXs.Add(xsFunction.Xs[i] + (ysFunction.Xs[j] - xsFunction.Ys[i]) / (xsFunction.Ys[i + 1] - xsFunction.Ys[i]) * (xsFunction.Xs[i + 1] - xsFunction.Xs[i]));
        //                        newYs.Add(ysFunction.Ys[j]);
        //                    }
        //                    else
        //                    {

        //                        break;                                                                                  // time for i + 1 to become i.
        //                    }
        //                }
        //            }
        //            //j++;
        //        }
        //        startingJ = j;
        //    }

        //    //capture the last points


        //    while (i <= I && ysFunction.Xs[j] < xsFunction.Ys[i])
        //    {

        //        newXs.Add(xsFunction.Xs[i - 1] + (ysFunction.Xs[j] - xsFunction.Ys[i - 1]) / (xsFunction.Ys[i] - xsFunction.Ys[i - 1]) * (xsFunction.Xs[i] - xsFunction.Xs[i - 1]));
        //        newYs.Add(ysFunction.Ys[j]);
        //        i++;
        //    }

        //    while (i <= I && xsFunction.Ys[i] <= ysFunction.Xs[j])
        //    {
        //        if (xsFunction.Ys[i] < ysFunction.Xs[j])
        //        {
        //            newXs.Add(xsFunction.Xs[i]);
        //            newYs.Add(ysFunction.Ys[j - 1] + (ysFunction.Ys[j] - ysFunction.Ys[j - 1]) * (xsFunction.Ys[i] - ysFunction.Xs[j - 1]) / (ysFunction.Xs[j] - ysFunction.Xs[j - 1])); //is it possible to get here with j ==0. it would crash

        //        }

        //        else if (xsFunction.Ys[i] == ysFunction.Xs[j])
        //        {
        //            newXs.Add(xsFunction.Xs[i]);
        //            newYs.Add(ysFunction.Ys[j]);
        //        }
        //        i++;
        //    }




        //    AnalyzeComposition(xsFunction, ysFunction, ref errors);
        //    return new OrdinatesFunction(newXs.ToArray(), newYs.ToArray(), ysFunction.FunctionType + 1);
        //}

        public override double GetXfromY(double Y, ref List<ErrorMessage> errors)
        {
            return Function.GetXfromY(Y);
        }
        public double GetYfromX(double X, ref List<ErrorMessage> errors)
        {
            return Function.GetYfromX(X);
        }
        //#region Fields
        //protected float[] _Xs;
        //protected float[] _Ys;
        //#endregion


        //#region Properties
        //public float[] Xs
        //{
        //    get
        //    {
        //        return _Xs;
        //    }
        //    set
        //    {
        //        _Xs = value;
        //    }
        //}
        //public float[] Ys
        //{
        //    get
        //    {
        //        return _Ys;
        //    }
        //    set
        //    {
        //        _Ys = value;
        //    }
        //}
        //#endregion


        //#region Constructors
        //[Tested(false)]
        ///// <summary> Creates a representation of a function consisting of paired x and y values. </summary>
        ///// <param name="xs"> Array of data passed as a representation of the function's domain. </param>
        ///// <param name="ys"> Array of data passed as a representation of the function's range.  </param>
        ///// <param name="functionType"> An enum descibing the type of function, the that X and Y parameters describe. </param>
        ///// <remarks> The data must produce a function of ordered pairs. In other words the paired data must be sorted with respect to the independent variable (xs) and meet the definition of a function meaning each independent variable value (e.g. x) must be paired with at most one depedent variable value (e.g. y). </remarks>
        //public OrdinatesFunction(float[] xs, float[] ys, FunctionTypes functionType) : base()
        //{
        //    Xs = xs;
        //    Ys = ys;
        //    FunctionType = functionType;
        //    Messages = new ModelErrors();

        //    Validate();
        //    if (Messages.IsValid == true)
        //    {
        //        FunctionType = functionType;
        //    }
        //    else
        //    {
        //        FunctionType = FunctionTypes.UnUsed;
        //    }
        //}
        //public OrdinatesFunction(Statistics.CurveDataCollection s, FunctionTypes functionType) : base()
        //{
        //    Xs = new float[s.Count];
        //    Ys = new float[s.Count];
        //    for (int i = 0; i < s.Count; i++)
        //    {
        //        Xs[i] = (float)s.get_X(i);
        //        Ys[i] = (float)s.get_Y(i);
        //    }

        //    FunctionType = functionType;
        //    Messages = new ModelErrors();

        //    Validate();
        //    if (Messages.IsValid == true)
        //    {
        //        FunctionType = functionType;
        //    }
        //    else
        //    {
        //        FunctionType = FunctionTypes.UnUsed;
        //    }
        //}
        //#endregion


        //#region IValidateMembers
        //public override void Validate()
        //{
        //    Messages.ClearMessages();
        //    ValidateFunction();
        //    ValidateMonotonicIncreasing();

        //    if (FunctionType == FunctionTypes.LeveeFailure)
        //    {
        //        ValidateFailureFunction();
        //    }

        //    Messages.ReportMessages();
        //}

        //[Tested(false)]
        ///// <summary> Tests if supplied domain and range data meet the definition of a function of paired data, with at least two ordinates. </summary>
        ///// <param name="xs"> Domain of function. </param>
        ///// <param name="ys"> Range of function. </param>
        ///// <returns> True if supplied paired data is ordered along its domain, at least 2 ordinates are provided and each independent variable value (e.g. x value) is mapped to at most one dependent variable value (e.g. y value). </returns>
        //private void ValidateFunction()
        //{
        //    List<ErrorMessage> errors = new List<ErrorMessage>();
        //    // 1. Is it long enough and is there an equal set of elements in each array?
        //    if (Xs.Length < 2 |
        //        Ys.Length < 2 |
        //        Xs.Length != Ys.Length)
        //    {
        //        Messages.AddMessage(new ErrorMessage("The domain and range (e.g. X and Y ordinates) must contain two or more equal length data pairs. The provided data does not; it will be stored in an unusable state and must be edited before it can be used in a compute.", ErrorMessageEnum.Fatal));
        //        return;
        //    }
        //    // 2. Are the xs ordered and is each x mapped to at most one y (e.g. is it a function)?
        //    for (int i = 0; i < Xs.Length - 1; i++)
        //    {
        //        if (Xs[i + 1] < Xs[i])
        //        {
        //            errors.Add(new ErrorMessage("The domain (e.g. X ordinates) must be ordered in an increasing fashion (e.g. from smallest to largest). However an ordinate with a value of " + Xs[i] + " proceeds an ordinate with a value of " + Xs[i + 1] + ". The data will be stored in an unusable state and must be edited before it can be used in a compute.", ErrorMessageEnum.Fatal));
        //        }
        //        if (Xs[i + 1] == Xs[i])
        //        {
        //            if (Ys[i + 1] == Ys[i])
        //            {
        //                errors.Add(new ErrorMessage("The duplicated ordinate (" + Xs[i] + ", " + Ys[i] + ") has been removed.", ErrorMessageEnum.Major));
        //                List<float> XsList = new List<float>(Xs);
        //                List<float> YsList = new List<float>(Ys);
        //                XsList.RemoveAt(i);
        //                YsList.RemoveAt(i);
        //                Xs = XsList.ToArray();
        //                Ys = YsList.ToArray();
        //                if (XsList.Count < 2)
        //                {
        //                    errors.Add(new ErrorMessage("The domain and range (e.g. X and Y ordinates) must contain two or more equal length data pairs. After removing duplicate entries the provided data does not; it will be stored in an unusable state and must be edited before it can be used in a compute.", ErrorMessageEnum.Fatal));
        //                }
        //            }
        //            else
        //            {
        //                errors.Add(new ErrorMessage("In order to represent a function any X value must map to exactly one Y value. However, the X value: " + Xs[i] + " is associated with the Y values: {" + Ys[i] + ", " + Ys[i + 1] + "}. The data will be stored in an unusable state and must be edited before it can be used in a compute.", ErrorMessageEnum.Fatal));
        //            }                    
        //        }
        //    }

        //    // 3. exterior interior stage funciton cannot have interior stages that are greater than exterior stages
        //    if (FunctionType == FunctionTypes.ExteriorInteriorStage)
        //    {
        //        for (int i = 0; i < Xs.Length - 1; i++)
        //        {
        //            if (Xs[i] < Ys[i])
        //            {
        //                errors.Add(new ErrorMessage("The Exterior Interior Stage function is invalid. It is not possible to have an interior stage that is greater than an exterior stage.", ErrorMessageEnum.Fatal));
        //                break;
        //            }
        //        }
        //    }
        //    Messages.AddMessages(errors);
        //}

        //[Tested(false)]
        ///// <summary>
        ///// Tests if the supplied array is in accending order.  
        ///// </summary>
        ///// <param name="ys"> Array to test. </param>
        ///// <returns> True if the array is in accending order and false otherwise. </returns>
        ///// <remarks> Reports if any values in the supplied array are repeated (e.g. the data is not strictly increasing). </remarks>
        //private void ValidateMonotonicIncreasing()
        //{
        //    List<float> repeatedValues = new List<float>();
        //    List<ErrorMessage> errors = new List<ErrorMessage>();
        //    for (int i = 0; i < Ys.Length - 1; i++)
        //    {
        //        if (Ys[i + 1] < Ys[i])
        //        {
        //            errors.Add(new ErrorMessage("The function is non monotonically increasing, and therefore cannont be used in the compute. A range (e.g. Y) ordinate with a value of " + Ys[i] + " preceeds one with a value of " + Ys[i + 1] + ". The data will be stored in an unusable state and must be edited before it can be used in a compute.", ErrorMessageEnum.Fatal));
        //        }
        //        else if (Ys[i + 1] == Ys[i])
        //        {
        //            repeatedValues.Add(Ys[i]);
        //        }
        //    }
        //    if (repeatedValues.Count > 0)
        //    {
        //        StringBuilder repeatedValuesMessage = new StringBuilder("The function is not strictly increasing. The following values are repeated: {");
        //        for (int i = 0; i < repeatedValues.Count - 1; i++)
        //        {
        //            repeatedValuesMessage.Append(repeatedValues[i].ToString() + ", ");
        //        }
        //        errors.Add(new ErrorMessage(repeatedValuesMessage.ToString() + repeatedValues[repeatedValues.Count - 1].ToString() + "}.", ErrorMessageEnum.Minor));
        //    }
        //    Messages.AddMessages(errors);
        //}


        //public void ValidateFailureFunction()
        //{
        //    List<ErrorMessage> errors = new List<ErrorMessage>();
        //    for (int i = 0; i < Xs.Length - 1; i++)
        //    {
        //        if (Ys[i] == Ys[i + 1])
        //        {
        //            errors.Add(new ErrorMessage("The values " + Xs[i] + " and " + Xs[i + 1] + " are associated with the same probability of failure (" + Ys[i] + "). The ordinate (" + Xs[i] + ", " + Ys[i] + ") associated wit the lower probability of failure has been removed from the failure function.", ErrorMessageEnum.Major));
        //            List<float> XsList = new List<float>(Xs);
        //            List<float> YsList = new List<float>(Ys);
        //            XsList.RemoveAt(i);
        //            YsList.RemoveAt(i);
        //            Xs = XsList.ToArray();
        //            Ys = YsList.ToArray();
        //            if (Xs.Length < 2)
        //            {
        //                errors.Add(new ErrorMessage("The domain and range (e.g. X and Y ordinates) must contain two or more equal length data pairs. After removing duplicate entries the provided data does not; it will be stored in an unusable state and must be edited before it can be used in a compute.", ErrorMessageEnum.Fatal));
        //            }
        //        }
        //    }


        //    if (Ys[0] != 0)
        //    {
        //        errors.Add(new ErrorMessage("The levee failure function must contain a 0.0 probability of failure point. This function does not, it will saved in an uncomputable state and must be edited before it can be used in a compute.", ErrorMessageEnum.Fatal));
        //    }
        //    if (Ys[Ys.Length - 1] != 1)
        //    {
        //        errors.Add(new ErrorMessage("The levee failure function must contain a 1.0 probability of failure point, usually a the top of the lateral structure. This function does not, it will be saved in an uncomputable state and must be edited before it can be used in a compute.", ErrorMessageEnum.Fatal));
        //    }
        //    Messages.AddMessages(errors);
        //}
        //#endregion


        //#region Functions
        //[Tested(false)]
        ///// <summary> Since increasing ordinates functions have no uncertainty parameters the function in memory is copied.  </summary>
        ///// <param name="randomNumberGnerator"></param>
        ///// <returns> A warning message indicating that the function had no uncertainty parameters and therefore orginal function was returned. </returns>
        //public override BaseFunction SampleFunction(Random randomNumberGenerator = null)
        //{
        //    return this;
        //}


        //[Tested(false)]
        //public override OrdinatesFunction GetOrdinatesFunction()
        //{
        //    return this;
        //}


        //[Tested(false)]
        //public override double GetXfromY(double Y, ref List<ErrorMessage> errors)
        //{
        //    if (Y < Ys[0])
        //    {
        //        errors.Add(new ErrorMessage("The target value " + Y + " is smaller than " + Ys[0] + ", the smallest value on the range of the function. The smallest domain ordinate, " + Xs[0] + " (e.g. X value) associated with the smallest range ordinate has been returned.", ErrorMessageEnum.Major));
        //        return Xs[0];
        //    }

        //    for (int i = 0; i < Ys.Length - 1; i++)
        //    {
        //        if (Ys[i] < Y)
        //        {
        //            if (Ys[i + 1] > Y)
        //            {
        //                double dx = (Y - Ys[i]) / (Ys[i + 1] - Ys[i]);
        //                return Xs[i] + dx * (Xs[i + 1] - Xs[i]);
        //            }
        //            if (Ys[i + 1] == Y)
        //            {
        //                return Xs[i + 1];
        //            }

        //        }
        //        if (Ys[i] == Y)
        //        {
        //            return Xs[i];
        //        }
        //    }

        //    errors.Add(new ErrorMessage("The target value " + Y + " is larger than the largest value on the range of the function, " + Ys[Ys.Length - 1] + ". The domain ordinate, " + Xs[Xs.Length - 1] + " (e.g. X value) associated with the largest range ordinate has been returned.", ErrorMessageEnum.Major));
        //    return Xs[Xs.Length - 1];
        //}


        //[Tested(false)]
        //public double GetYfromX(double X, ref List<ErrorMessage> errors)
        //{
        //    if (X < Xs[0])
        //    {
        //        errors.Add(new ErrorMessage("The target value " + X + " is smaller than " + Xs[0] + ", the smallest value on the domain of the function. The smallest range ordinate, " + Ys[0] + " (e.g. Y value) associated with the smallest domain ordinate has been returned.", ErrorMessageEnum.Major));
        //        return Ys[0];
        //    }

        //    for (int i = 0; i < Xs.Length - 1; i++)
        //    {
        //        if (Xs[i] < X)
        //        {
        //            if (Xs[i + 1] > X)
        //            {
        //                double dy = (X - Xs[i]) / (Xs[i + 1] - Xs[i]);
        //                return Ys[i] + dy * (Ys[i + 1] - Ys[i]);
        //            }
        //            if (Xs[i + 1] == X)
        //            {
        //                return Ys[i + 1];
        //            }

        //        }
        //        if (Xs[i] == X)
        //        {
        //            return Ys[i];
        //        }   
        //    }

        //    errors.Add(new ErrorMessage("The target value " + X + " is larger than the largest value on the domain of the function, " + Xs[Xs.Length - 1] + ". The range ordinate, " + Ys[Ys.Length - 1] + " (e.g. Y value) associated with the largest domain ordinate has been returned.", ErrorMessageEnum.Major));
        //    return Ys[Ys.Length - 1];
        //}


        //[Tested(false)]
        //public override OrdinatesFunction Compose(OrdinatesFunction Function, ref List<ErrorMessage> errors)
        //{
        //    //0. Determine which function contributes Xs and which contributes Ys.
        //    int dT = Function.FunctionType - this.FunctionType;
        //    OrdinatesFunction xsFunction, ysFunction;
        //    switch (dT)
        //    {
        //        case 1:
        //            ysFunction = Function;
        //            xsFunction = this;
        //            break;
        //        case -1:
        //            ysFunction = this;
        //            xsFunction = Function;
        //            break;
        //        default:
        //            if (Function.FunctionType == FunctionTypes.LeveeFailure)
        //            {
        //                ysFunction = Function;
        //                xsFunction = this;
        //                break;
        //            }
        //            else
        //            {
        //                if (this.FunctionType == FunctionTypes.LeveeFailure)
        //                {
        //                    ysFunction = this;
        //                    xsFunction = Function;
        //                    break;
        //                }
        //                else
        //                {
        //                    errors.Add(new ErrorMessage("The two input functions do not share a common set of ordinates, a new function cannot be composed. To perform composition the domain (x-ordiantes) of the input function providing the new function's range (y-ordinates) must match range (y-ordiantes) of the input function providing the new function's domain (x-ordinates).", ErrorMessageEnum.Fatal));
        //                    return null;
        //                }
        //            }
        //    }
        //    //1. Loop to first usable X value.
        //    int i, I = xsFunction.Xs.Length - 1;
        //    if (xsFunction.Ys[0] < ysFunction.Xs[0])
        //    {
        //        i = -1;
        //        do
        //        {
        //            if (i < I)
        //            {
        //                i++;
        //            }
        //            else
        //            {
        //                errors.Add(new ErrorMessage("The two input functions common set of ordinates do not have overlapping values, a new function cannot be composed. Specifically, the range of the input function providing the new function's domain is on the interval: [" + xsFunction.Ys[0] + ",  " + xsFunction.Ys[xsFunction.Ys.Length - 1] + "]. The matching domain of the input function providing the new function's range is on the interval: [" + ysFunction.Xs[0] + ", " + ysFunction.Xs[ysFunction.Xs.Length - 1] + "].", ErrorMessageEnum.Fatal));
        //                return null;
        //            }
        //        } while (xsFunction.Ys[i] < ysFunction.Xs[0]);
        //    }
        //    else
        //    {
        //        i = 0;
        //    }
        //    //2. Loop over portions of xsFunction not overlapping (before) ysFunction 
        //    int j, J = ysFunction.Ys.Length - 1;
        //    if (xsFunction.Ys[0] > ysFunction.Xs[1])
        //    {
        //        j = -1;
        //        do
        //        {
        //            if (j + 1 < J)
        //            {
        //                j++;
        //            }
        //            else
        //            {
        //                errors.Add(new ErrorMessage("The two input functions common set of ordinates do not have overlapping values, a new function cannot be composed. Specifically, the range of the input function providing the new function's domain is on the interval: [" + xsFunction.Ys[0].ToString() + ",  " + xsFunction.Ys[xsFunction.Ys.Length - 1].ToString() + "]. The matching domain of the input function providing the new function's range is on the interval: [" + ysFunction.Xs[0].ToString() + ", " + ysFunction.Xs[ysFunction.Xs.Length - 1].ToString() + "].", ErrorMessageEnum.Fatal));
        //                return null;
        //            }
        //        } while (xsFunction.Ys[0] >= ysFunction.Xs[j + 1]);

        //    }
        //    else
        //    {
        //        j = 0;
        //    }
        //    //3. Loop over overlapping portion of two functions: xsFunction.Ys[i] IS >= ysFunction.Xs[j] AND < ysFunction.Xs[j + 1]
        //    bool iAdded = false;

        //    int startingI = i, startingJ = j;
        //    List<float> newXs = new List<float>();
        //    List<float> newYs = new List<float>();

        //    //evaluate the first point points 
        //    while (ysFunction.Xs[j] < xsFunction.Ys[i] && i > 0)
        //    {
        //        newXs.Add(xsFunction.Xs[i] + (ysFunction.Xs[j] - xsFunction.Ys[i]) / (xsFunction.Ys[i + 1] - xsFunction.Ys[i]) * (xsFunction.Xs[i + 1] - xsFunction.Xs[i]));
        //        newYs.Add(ysFunction.Ys[j]);
        //        j++;
        //    }


        //    for (i = startingI; i < I; i++)
        //    {
        //        if (j == J) break;
        //        iAdded = false;
        //        for (j = startingJ; j < J; j++)
        //        {

        //            if (ysFunction.Xs[j] < xsFunction.Ys[i])                                                            // sets the first X,Y on the i -> i + 1 interval.
        //            {
        //                if (ysFunction.Xs[j + 1] > xsFunction.Ys[i])
        //                {
        //                    newXs.Add(xsFunction.Xs[i]);
        //                    newYs.Add(ysFunction.Ys[j] + (xsFunction.Ys[i] - ysFunction.Xs[j]) / (ysFunction.Xs[j + 1] - ysFunction.Xs[j]) * (ysFunction.Ys[j + 1] - ysFunction.Ys[j]));
        //                    iAdded = true;
        //                }
        //                else
        //                {
        //                    if (ysFunction.Xs[j + 1] == xsFunction.Ys[i])                                              // would this mean j == j + 1 ? If so, this should be an invalid (type = 99) function.
        //                    {
        //                        newXs.Add(xsFunction.Xs[i]);
        //                        newYs.Add(ysFunction.Ys[j + 1]);
        //                        iAdded = true;
        //                    }
        //                }
        //            }
        //            else
        //            {

        //                if (ysFunction.Xs[j] == xsFunction.Ys[i])                                                       // setting first X,Y in special case that xsFunction.Ys[i] == ysFunction.Xs[j]
        //                {
        //                    newXs.Add(xsFunction.Xs[i]);
        //                    newYs.Add(ysFunction.Ys[j]);
        //                    iAdded = true;
        //                }
        //                else                                                                             // looking for additional j's on the interval i -> i + 1
        //                {
        //                    //here we need to see if there are points from i that are between points in j
        //                    if (iAdded == false)
        //                    {
        //                        newXs.Add(xsFunction.Xs[i]);
        //                        newYs.Add(ysFunction.Ys[j - 1] + (ysFunction.Ys[j] - ysFunction.Ys[j - 1]) * (xsFunction.Ys[i] - ysFunction.Xs[j - 1]) / (ysFunction.Xs[j] - ysFunction.Xs[j - 1])); //is it possible to get here with j ==0. it would crash
        //                        iAdded = true;
        //                    }

        //                    if (ysFunction.Xs[j] < xsFunction.Ys[i + 1])
        //                    {
        //                        newXs.Add(xsFunction.Xs[i] + (ysFunction.Xs[j] - xsFunction.Ys[i]) / (xsFunction.Ys[i + 1] - xsFunction.Ys[i]) * (xsFunction.Xs[i + 1] - xsFunction.Xs[i]));
        //                        newYs.Add(ysFunction.Ys[j]);
        //                    }
        //                    else
        //                    {

        //                        break;                                                                                  // time for i + 1 to become i.
        //                    }
        //                }
        //            }
        //            //j++;
        //        }
        //        startingJ = j;
        //    }

        //    //capture the last points


        //    while (i <= I && ysFunction.Xs[j] < xsFunction.Ys[i])
        //    {

        //        newXs.Add(xsFunction.Xs[i - 1] + (ysFunction.Xs[j] - xsFunction.Ys[i - 1]) / (xsFunction.Ys[i] - xsFunction.Ys[i - 1]) * (xsFunction.Xs[i] - xsFunction.Xs[i - 1]));
        //        newYs.Add(ysFunction.Ys[j]);
        //        i++;
        //    }

        //    while (i<=I && xsFunction.Ys[i] <= ysFunction.Xs[j])
        //    {
        //        if (xsFunction.Ys[i] < ysFunction.Xs[j])
        //        {
        //            newXs.Add(xsFunction.Xs[i]);
        //            newYs.Add(ysFunction.Ys[j - 1] + (ysFunction.Ys[j] - ysFunction.Ys[j - 1]) * (xsFunction.Ys[i] - ysFunction.Xs[j - 1]) / (ysFunction.Xs[j] - ysFunction.Xs[j - 1])); //is it possible to get here with j ==0. it would crash

        //        }

        //        else if (xsFunction.Ys[i] == ysFunction.Xs[j])
        //        {
        //            newXs.Add(xsFunction.Xs[i]);
        //            newYs.Add(ysFunction.Ys[j]);
        //        }
        //        i++;
        //    }

            


        //    AnalyzeComposition(xsFunction, ysFunction, ref errors);
        //    return new OrdinatesFunction(newXs.ToArray(), newYs.ToArray(), ysFunction.FunctionType + 1);
        //}

        //below is an old version of compose
        //[Tested(false)]
        //public override OrdinatesFunction Compose(OrdinatesFunction Function, ref List<ErrorMessage> errors)
        //{
        //    //0. Make sure they share a set of ordinaes.
        //    OrdinatesFunction YsFunction, XsFunction;
        //    int dT = Function.FunctionType - this.FunctionType;
        //    switch (dT)
        //    {
        //        case 1:
        //            YsFunction = Function;
        //            XsFunction = this;
        //            break;
        //        case -1:
        //            YsFunction = this;
        //            XsFunction = Function;
        //            break;
        //        default:
        //            errors.Add(new ErrorMessage("The two input functions do not share a common set of ordinates, a new function cannot be composed. To perform composition the domain (x-ordiantes) of the input function providing the new function's range (y-ordinates) must match range (y-ordiantes) of the input function providing the new function's domain (x-ordinates).", ErrorMessageEnum.Fatal));
        //            return null;
        //    }

        //    //1. Advance YFunction to area of overlap with the XFunction
        //    int i = 0, I = YsFunction.Xs.Length - 1;
        //    while (XsFunction.Ys[0] > YsFunction.Xs[i])
        //    {
        //        if (i < I)
        //        {
        //            i++;
        //        }
        //        else
        //        {
        //            errors.Add(new ErrorMessage("The two input functions common set of ordinates do not have overlapping values, a new function cannot be composed. Specifically, the range of the input function providing the new function's domain is on the interval: [" + XsFunction.Ys[0] + ",  " + XsFunction.Ys[XsFunction.Ys.Length - 1] + "]. The matching domain of the input function providing the new function's range is on the interval: [" + YsFunction.Xs[0] + ", " + YsFunction.Xs[YsFunction.Xs.Length - 1] + "].", ErrorMessageEnum.Fatal));
        //            return null;
        //        }
        //    }

        //    //2. Advance XFunction to area of overlap with the YFunction.
        //    int j = 0, J = XsFunction.Ys.Length - 1;
        //    while (XsFunction.Ys[j] < YsFunction.Xs[i])
        //    {
        //        if (j < J)
        //        {
        //            j++;
        //        }
        //        else
        //        {
        //            errors.Add(new ErrorMessage("The two input functions common set of ordinates do not have overlapping values, a new function cannot be composed. Specifically, the range of the input function providing the new function's domain is on the interval: [" + XsFunction.Ys[0].ToString() + ",  " + XsFunction.Ys[XsFunction.Ys.Length - 1].ToString() + "]. The matching domain of the input function providing the new function's range is on the interval: [" + YsFunction.Xs[0].ToString() + ", " + YsFunction.Xs[YsFunction.Xs.Length - 1].ToString() + "].", ErrorMessageEnum.Fatal));
        //            return null;
        //        }
        //    }

        //    //3. Compose accross overlapping interval.
        //    if (j != 0)
        //    {
        //        j--;
        //    }

        //    List<float> newXs = new List<float>();
        //    List<float> newYs = new List<float>();
        //    while (i <= I)
        //    {
        //        if (XsFunction.Ys[j] <= YsFunction.Xs[i])
        //        {
        //            if (j + 1 > J)
        //            {
        //                //unusable portion of the range (from YFunction) b/c it is outside of the domain (and range of the XFunction).
        //                break;
        //            }
        //            else
        //            {
        //                if (XsFunction.Ys[j + 1] >= YsFunction.Xs[i])
        //                {
        //                    float dx = (YsFunction.Xs[i] - XsFunction.Ys[j]) / (XsFunction.Ys[j + 1] - XsFunction.Ys[j]);
        //                    if (float.IsNaN(dx)) { dx = 0; }
        //                    newXs.Add(XsFunction.Xs[j] + dx * (XsFunction.Xs[j + 1] - XsFunction.Xs[j]));
        //                    newYs.Add(YsFunction.Ys[i]);
        //                    i++;
        //                    continue;
        //                }
        //                else
        //                {
        //                    j++;
        //                    continue;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //unusable portion of the domain (from XFunction) b/c it is outside the range (and domain of the YFunction)
        //            break;
        //        }
        //    }

        //    //4. Analysis of lost information.
        //    double unusedRange = 1 - (newXs[0] - newXs[newXs.Count - 1]);
        //    if (unusedRange > 0.5)
        //    {
        //        errors.Add(new ErrorMessage("The composed function contains the domain: [" + newXs[0] + ", " + newXs[newXs.Count - 1] + "]. Higher or lower annual exceedancce probablilites will result in a value of: " + newYs[0] + ", " + newYs[newYs.Count - 1] + "], respectively.", ErrorMessageEnum.Major));
        //    }
        //    else
        //    {
        //        errors.Add(new ErrorMessage("The composed function contains the domain: [" + newXs[0] + ", " + newXs[newXs.Count - 1] + "]. Higher or lower annual exceedancce probablilites will result in a value of: " + newYs[0] + ", " + newYs[newYs.Count - 1] + "], respectively.", ErrorMessageEnum.Minor));
        //    }

        //    return new OrdinatesFunction(newXs.ToArray(), newYs.ToArray(), YsFunction.FunctionType + 1);
        //}


        [Tested(false)]
        public double TrapizoidalRiemannSum(double a, double b)
        {
            int lastX = Function.Count - 1;
            // 1. Throw Exceptions for out of bounds ranges.
            if (a > b || a < Function.get_X(0) || b > Function.get_X(lastX) || lastX < 1)
                throw new Exception("Invalid interval.");

            // 2. Loop until the first in range occurance of X is found.
            int i = -1;
            do
            {
                i++;
            }
            while (Function.get_X(i) < a);              //If (xs.max < a) an array out of bounds exception should be thrown here. 

            // 3. Find first area.
            double riemannSum = 0;
            if (Function.get_X(i) == a)
            {
                if (a == b)
                {
                    // 3a. The area is 0.
                    return riemannSum;
                }
            }
            else
            {
                double slope = (Function.get_Y(i) - Function.get_Y(i - 1)) / (Function.get_X(i) - Function.get_X(i - 1));
                double ysa = Function.get_Y(i - 1) + slope * (a - Function.get_X(i - 1));

                if (Function.get_X(i) <= b)
                {
                    // 3b. First area is from a to an ordinate.
                    riemannSum = (Function.get_Y(i) + ysa) / 2 * (Function.get_X(i) - a);

                    if (Function.get_X(i) == b)
                    {
                        return riemannSum;
                    }
                }
                else
                {
                    // 3c. TheFirst area = total area if [a,b] are between two ordinates.
                    // Can get rid of next local variables.
                    double ysb = Function.get_Y(i - 1) + slope * (b - Function.get_X(i - 1));

                    return (ysb + ysa) / 2 * (b - a);
                }
            }
            // 4. Integrate across ordinates within range.
            if (Function.get_X(i + 1) < b)
            {
                do
                {
                    riemannSum += (Function.get_Y(i + 1) + Function.get_Y(i)) / 2 * (Function.get_X(i + 1) - Function.get_X(i));
                    i++;
                }
                while (Function.get_X(i + 1) < b);
            }
            // 5. Find last area.
            if (Function.get_X(i + 1) == b)
            {
                // 5a. Last area is a Full ordinate 
                return riemannSum;
            }
            else
            {
                // Can get rid of next two local variables.
                double slope = (Function.get_Y(i + 1) - Function.get_Y(i)) / (Function.get_X(i + 1) - Function.get_X(i));
                double ysb = Function.get_Y(i) + slope * (b - Function.get_X(i));
                // 5b. Last area is a Partial ordinate
                return riemannSum += (ysb + Function.get_Y(i)) / 2 * (b - Function.get_X(i));
            }
        }


        [Tested(true, true, @"Functions\Ordinates Functions\OrdinatesFunction\TrapizoidalRiemannSum.xlsx","5/16/17","Cody McCoy")]
        public double TrapizoidalRiemannSum()
        {
            double riemannSum = 0;
            for (int i = 0; i < Function.Count - 1; i++)
            {
                riemannSum += (Function.get_Y(i + 1) + Function.get_Y(i)) * (Function.get_X(i + 1) - Function.get_X(i)) / 2;
            }
            return riemannSum;
        }


        /// <summary> Computes a weighted trapizoidal riemann sum for a frequency function, using the probability of lateral structure failure as weights. </summary>
        /// <param name="failureFunction"> A failure function, the domain (e.g. Xs) of which is in the same units as the frequency function range (e.g. Ys).</param>
        /// <returns> A weighted trapizoidal Riemann sum. </returns>
        [Tested(true,true, @"Functions\Ordinates Functions\OrdinatesFunction\WeightedTrapezoidalRiemannSumTesting.xlsx","5/25/17","Cody McCoy")]
        public double WeightedTrapizoidalRiemannSum(OrdinatesFunction failureFunction)
        {
            double weightedRiemannSum = 0;
            for (int i = 0; i < Function.Count - 1; i++)
            {
                double areaWeight = GetAreaWeight(failureFunction, Function.get_Y(i), Function.get_Y(i + 1));
                double RSum = ((Function.get_Y(i) + Function.get_Y(i + 1)) / 2) * (Function.get_X(i + 1) - Function.get_X(i));
                weightedRiemannSum += RSum * areaWeight;
            }
            return weightedRiemannSum;
        }

        [Tested(true,true, @"Functions\Ordinates Functions\OrdinatesFunction\WeightedAEPTesting.xlsx","5/25/17","Cody McCoy")]
        public double WeightedAEP(OrdinatesFunction failureFunction)
        {
        
            double weightedAEP = 0;
            for (int i = 0; i < Function.Count - 1; i++)
            {
                double areaWeight = GetAreaWeight(failureFunction, Function.get_X(i), Function.get_X(i + 1));
                double interval = (Function.get_X(i+1) - Function.get_X(i )) ;
                weightedAEP += interval * areaWeight;
                //double singleRunWeightedAEP = interval * areaWeight;//this is just for testing. Delete me!
            }
            return weightedAEP;

        }

        /// <summary> Calculates the average probability of lateral structure failure, on the provided frequency function range [a, b], given a failure function domain in the units of [a, b]. </summary>
        /// <param name="failureFunction"> Failure function, from which an average probability is calculated. The units of failure function domain (e.g. Xs) and frequency function's range (e.g. Ys) must match. </param>
        /// <param name="a"> Minimum value on range. </param>
        /// <param name="b"> Maximum value on range. </param>
        /// <returns> An average probability of lateral structure failure. </returns>
        [Tested(true, true, @"Functions\Ordinates Functions\OrdinatesFunction\GetAreaWeightTesting.xlsx","5/25/17","Cody McCoy")]
        private double GetAreaWeight(OrdinatesFunction failureFunction, double a, double b)
        {
            double lP, uP, avgP = -999999999999, range = b - a; // The giant negative number is just for testing. Remove if test is validated.
            for (int j = 0; j < failureFunction.Function.Count - 1; j++)
            {
                if (failureFunction.Function.get_X(j) <= a)     
                {
                    if (j == failureFunction.Function.Count - 1)     // Special Case: failurefunction.Xs[Xs.Length - 1] <= a
                    {
                        return avgP = failureFunction.Function.get_Y(j);
                    }
                    else
                    {
                        if (failureFunction.Function.get_X(j + 1) > a)
                        {
                            lP = failureFunction.Function.get_Y(j) + (a - failureFunction.Function.get_X(j)) / (failureFunction.Function.get_X(j + 1) - failureFunction.Function.get_X(j)) * (failureFunction.Function.get_Y(j + 1) - failureFunction.Function.get_Y(j));

                            if (failureFunction.Function.get_X(j + 1) < b)
                            {
                                avgP = (lP + failureFunction.Function.get_Y(j + 1)) / 2 * (failureFunction.Function.get_X(j + 1) - a) / range;
                            }
                            else
                            {
                                uP = failureFunction.Function.get_Y(j) + (b - failureFunction.Function.get_X(j)) / (failureFunction.Function.get_X(j + 1) - failureFunction.Function.get_X(j)) * (failureFunction.Function.get_Y(j + 1) - failureFunction.Function.get_Y(j));
                                return avgP = (uP + lP) / 2;
                            }
                        }
                    }   
                }
                else
                {
                    if (j == failureFunction.Function.Count - 1)     // Special Case: failurefunction.Xs[Xs.Length - 1] < b
                    {
                        return avgP += failureFunction.Function.get_Y(j) * (b - failureFunction.Function.get_X(j)) / range;
                    }
                    else
                    {
                        if (j == 0)
                        {
                            if (failureFunction.Function.get_Y(0) < b)      // Special Case: failurefunction.Xs[0] > a
                            {
                                avgP = failureFunction.Function.get_Y(j) * (failureFunction.Function.get_X(0) - a) / range;
                            }
                            else                                // Special Case: failurefunction.Xs[0] >=b
                            {
                                return avgP = failureFunction.Function.get_Y(j);
                            }
                        }

                        if (failureFunction.Function.get_X(j + 1) < b)
                        {
                            avgP += (failureFunction.Function.get_Y(j) + failureFunction.Function.get_Y(j + 1)) / 2 * (failureFunction.Function.get_X(j + 1) - failureFunction.Function.get_X(j)) / range;
                        }
                        else
                        {
                            uP = failureFunction.Function.get_Y(j) + (b - failureFunction.Function.get_X(j)) / (failureFunction.Function.get_X(j + 1) - failureFunction.Function.get_X(j)) * (failureFunction.Function.get_Y(j + 1) - failureFunction.Function.get_Y(j));
                            return avgP += (failureFunction.Function.get_Y(j) + uP) / 2 * (b - failureFunction.Function.get_X(j)) / range;
                        }
                    }
                }
                //j++;
            }
            throw new Exception("This should be unreachable.");
        }
        #endregion

        #region Voids
        [Tested(true,true, @"Functions\Ordinates Functions\OrdinatesFunction\AnalyzeComposition.xlsx","5/18/2017","Cody McCoy")]
        public void AnalyzeComposition(Statistics.CurveIncreasing xsFunction, Statistics.CurveIncreasing ysFunction, ref List<ErrorMessage> errors)
        {
            //need to catch half cases.

            string lostIntervalMessage = "";
            double maxLowerLostValue = 0;
            double minLowerLostValue = 0;
            double minUpperLostValue = 0;
            double maxUpperLostValue = 0;
            double lostInterval;
            double lowerInterval = 0;
            double upperInterval = 0;
            double lostPercentThreshold = .25;

            bool lostLowerXValues = false;
            bool lostLowerYValues = false;
            bool lostUpperXValues = false;
            bool lostUpperYValues = false;

            int I = xsFunction.Count - 1, J = ysFunction.Count - 1;
            double domain = xsFunction.get_X(I) - xsFunction.get_X(0), range = ysFunction.get_Y(J) - ysFunction.get_Y(0);

            //determine the lost lower range
            if (xsFunction.get_Y(0) < ysFunction.get_X(0))
            {
                maxLowerLostValue = xsFunction.GetXfromY(ysFunction.get_X(0));
                minLowerLostValue = xsFunction.get_X(0);
                lowerInterval = (maxLowerLostValue - minLowerLostValue) / domain;
                lostLowerXValues = true;
            }
            else if (ysFunction.get_X(0) < xsFunction.get_Y(0))
            {

                maxLowerLostValue = ysFunction.GetYfromX(xsFunction.get_Y(0));
                minLowerLostValue = ysFunction.get_Y(0);
                lowerInterval = (maxLowerLostValue - minLowerLostValue) / range;
                lostLowerYValues = true;
            }

            //determine the lost upper range
            if (xsFunction.get_Y(I) > ysFunction.get_X(J))
            {
                minUpperLostValue = xsFunction.GetXfromY(ysFunction.get_X(J));
                maxUpperLostValue = xsFunction.get_X(I);
                upperInterval = (maxUpperLostValue - minUpperLostValue) / domain;
                lostUpperXValues = true;
                //lostInterval = ((maxLowerLostValue - xsFunction.get_X(0)) + (xsFunction.get_X(I) - minUpperLostValue)) / domain;
                //lostIntervalMessage = "Domain values (e.g. Xs) on the intervals [" + xsFunction.get_X(0) + ", " + maxLowerLostValue + "] and [" + minUpperLostValue + ", " + xsFunction.get_X(I) + "] were lost during composition because the component functions do not overlap on these intervals.";
            }
            else if (ysFunction.get_X(J) > xsFunction.get_Y(I))
            {
                minUpperLostValue = ysFunction.GetYfromX(xsFunction.get_Y(I));
                maxUpperLostValue = ysFunction.get_Y(J);
                upperInterval = (maxUpperLostValue - minUpperLostValue) / range;
                lostUpperYValues = true;
                //lostInterval = (maxLowerLostValue - xsFunction.get_X(0)) / domain;
                //lostIntervalMessage = "Domain values (e.g. Xs) on the interval [" + xsFunction.get_X(0) + ", " + maxLowerLostValue + "] were lost during composition because the component functions do not overlap on this interval."; 
            }

           
            //write out the message
            //both xs
            if (lostLowerXValues == true && lostUpperXValues == true)
            {
                lostIntervalMessage = "Domain values (e.g. Xs) on the intervals [" + minLowerLostValue + ", " + maxLowerLostValue + "] and [" + minUpperLostValue + ", " + maxUpperLostValue + "] were lost during composition because the component functions do not overlap on these intervals.";


            }
            //both ys
            else if (lostLowerYValues == true && lostUpperYValues == true)
            {
                lostIntervalMessage = "Range values (e.g. Ys) on the intervals [" + minLowerLostValue + ", " + maxLowerLostValue + "] and [" + minUpperLostValue + ", " + maxUpperLostValue + "] were lost during composition because the component functions do not overlap on these intervals.";


            }
            //lower x upper y
            else if (lostLowerXValues == true && lostUpperYValues == true)
            {
                lostIntervalMessage = "Domain values (e.g. Xs) on the intervals [" + minLowerLostValue + ", " + maxLowerLostValue + "] and range values (e.g. Ys) [" + minUpperLostValue + ", " + maxUpperLostValue + "] were lost during composition because the component functions do not overlap on these intervals.";

            }
            //lower y upper x
            else if (lostLowerYValues==true && lostUpperXValues == true)
            {
                lostIntervalMessage = "Range values (e.g. Ys) on the intervals [" + minLowerLostValue + ", " + maxLowerLostValue + "] and domain values (e.g. Xs) [" + minUpperLostValue + ", " + maxUpperLostValue + "] were lost during composition because the component functions do not overlap on these intervals.";

            }
            // lower x
            else if (lostLowerXValues ==true)
            {
                lostIntervalMessage = "Domain values (e.g. Xs) on the interval [" + minLowerLostValue + ", " + maxLowerLostValue + "] were lost during composition because the component functions do not overlap on this interval.";


            }
            // lower y
            else if (lostLowerYValues == true)
            {
                lostIntervalMessage = "Range values (e.g. Ys) on the interval [" + minLowerLostValue + ", " + maxLowerLostValue + "] were lost during composition because the component functions do not overlap on this interval.";

            }
            // upper x
            else if (lostUpperXValues == true)
            {
                lostIntervalMessage = "Domain values (e.g. Xs) on the interval [" + minLowerLostValue + ", " + maxLowerLostValue + "] were lost during composition because the component functions do not overlap on this interval.";

            }
            // upper y
            else if (lostUpperYValues ==true)
            {
                lostIntervalMessage = "Range values (e.g. Ys) on the interval [" + minLowerLostValue + ", " + maxLowerLostValue + "] were lost during composition because the component functions do not overlap on this interval.";

            }


            lostInterval = lowerInterval + upperInterval;
            if (lostInterval > lostPercentThreshold)
            {
                errors.Add(new ErrorMessage(lostIntervalMessage, ErrorMessageEnum.Major));
            }
            else
            {
                errors.Add(new ErrorMessage(lostIntervalMessage, ErrorMessageEnum.Minor));
            }


        }


        // Johns version. cody is re-writing this function above on 5/9/17
        //public void AnalyzeComposition(Statistics.CurveIncreasing xsFunction, Statistics.CurveIncreasing ysFunction, ref List<ErrorMessage> errors)
        //{
        //    //need to catch half cases.

        //    string lostIntervalMessage = "";
        //    double maxLowerLostValue;
        //    double minUpperLostValue;
        //    double lowerLostInterval;
        //    double upperLostInterval = 0;
        //    int I = xsFunction.Count - 1, J = ysFunction.Count - 1;
        //    double domain = xsFunction.get_X(I) - xsFunction.get_X(0), range = ysFunction.get_Y(J) - ysFunction.get_Y(0);
            
        //    //lower xs are lost
        //    if (xsFunction.get_Y(0) < ysFunction.get_X(0))
        //    {
        //        maxLowerLostValue = xsFunction.GetXfromY(ysFunction.get_X(0));
        //        lowerLostInterval = (maxLowerLostValue - xsFunction.get_X(0))/domain;
        //        //upper xs are lost
        //        if (xsFunction.get_Y(I) > ysFunction.get_X(J))
        //        {
        //            minUpperLostValue = xsFunction.GetXfromY(ysFunction.get_X(J));
        //            upperLostInterval = ((xsFunction.get_X(I) - minUpperLostValue)) / domain;
        //            lostIntervalMessage = "Domain values (e.g. Xs) on the intervals [" + xsFunction.get_X(0) + ", " + maxLowerLostValue + "] and [" + minUpperLostValue + ", " + xsFunction.get_X(I) + "] were lost during composition because the component functions do not overlap on these intervals.";
        //        }
        //        //upper ys are lost
        //        else if(ysFunction.get_X(J) > xsFunction.get_Y(I))
        //        {
        //            minUpperLostValue = ysFunction.GetYfromX(xsFunction.get_Y(I));
        //            upperLostInterval = (ysFunction.get_Y(J) - minUpperLostValue) / range;
        //            lostIntervalMessage = "Domain values (e.g. Xs) on the interval [" + xsFunction.get_X(0) + ", " + maxLowerLostValue + "] were lost during composition because the component functions do not overlap on this interval.";
        //        }

        //        double lostInterval = lowerLostInterval + upperLostInterval;
        //        if (lostInterval > 0.25)
        //        {
        //            errors.Add(new ErrorMessage(lostIntervalMessage, ErrorMessageEnum.Major));
        //        }
        //        else
        //        {
        //            errors.Add(new ErrorMessage(lostIntervalMessage, ErrorMessageEnum.Minor));
        //        }
        //    }
        //    else if(ysFunction.get_X(0)< xsFunction.get_Y(0))
        //    {
        //        maxLowerLostValue = ysFunction.GetYfromX(xsFunction.get_Y(0));
        //        lowerLostInterval = maxLowerLostValue - ysFunction.get_Y(0)/range;
        //        //upper ys are lost
        //        if (xsFunction.get_Y(I) < ysFunction.get_X(J))
        //        {
        //            minUpperLostValue = ysFunction.GetYfromX(xsFunction.get_Y(I));
        //            upperLostInterval = (ysFunction.get_Y(J) - minUpperLostValue) / range;
        //            lostIntervalMessage = "Range values (e.g. Ys) on the intervals [" + ysFunction.get_Y(0) + ", " + maxLowerLostValue + "] and [" + minUpperLostValue + ", " + ysFunction.get_Y(J) + "] were lost during composition because the component functions do not overlap on these intervals.";
        //        }
        //        //upper xs are lost
        //        else if(xsFunction.get_Y(I)>ysFunction.get_X(J))
        //        {
        //            minUpperLostValue = xsFunction.GetXfromY(ysFunction.get_X(J));
        //            upperLostInterval = (xsFunction.get_X(I) - minUpperLostValue) / domain;
        //            lostIntervalMessage = "Range values (e.g. Ys) on the interval [" + ysFunction.get_Y(0) + ", " + maxLowerLostValue + "] were lost during composition because the component functions do not overlap on this interval.";
        //        }

        //        double lostInterval = lowerLostInterval + upperLostInterval;
        //        if (lostInterval > 0.25)
        //        {
        //            errors.Add(new ErrorMessage(lostIntervalMessage, ErrorMessageEnum.Major));
        //        }
        //        else
        //        {
        //            errors.Add(new ErrorMessage(lostIntervalMessage, ErrorMessageEnum.Minor));
        //        }
        //    }
        //    else //the bottom values are equal so we did not lose anything there. Check the upper regions
        //    {

        //    }
        //}


        //public static OrdinatesFunction CombineCurves(Statistics.CurveIncreasing curve1, Statistics.CurveIncreasing curve2, bool xAxesAreShared)
        //{
        //    // the xs will come from curve1 and the ys from curve2



        //    bool allPointsAdded = false;
        //    List<double> xValues = new List<double>();
        //    List<double> yValues = new List<double>();

        //    if(xAxesAreShared == true)
        //    {
        //        int i = 0;
        //        int j = 0;
        //        bool curve1IsLower = false;
        //        bool jSubsetDone = false;
        //        bool iSubsetDone = false;
        //        bool jIsAtTheEnd = false;
        //        bool iIsAtTheEnd = false;

        //        if(curve1.get_X(0) < curve2.get_X(0))
        //        {
        //            curve1IsLower = true;
        //            //increment i up to the one before the first j point
        //            for (int k = 0; k < curve1.Count; k++)
        //            {
        //                if (curve1.get_X(k) < curve2.get_X(0))
        //                {
        //                    i++;
        //                }
        //            }
        //            i--; //you just have to do this to make it work right

        //        }
        //        else if (curve1.get_X(0) == curve2.get_X(0))
        //        {
        //            curve1IsLower = true;
        //        }



        //        if (curve1IsLower == true)
        //        {

        //            while (jIsAtTheEnd == false && iIsAtTheEnd == false)
        //            {
        //                while (jSubsetDone == false)
        //                {
        //                    if (curve1.get_X(i) < curve2.get_X(j) && curve2.get_X(j) < curve1.get_X(i + 1))
        //                    {
        //                        xValues.Add(getLinearInterpolatedValue(curve1.get_X(i), curve2.get_X(j), curve1.get_X(i + 1), curve1.get_Y(i), curve1.get_Y(i + 1)));
        //                        yValues.Add(curve2.get_Y(j));
        //                    }
        //                    else if (curve1.get_X(i) == curve2.get_X(j))
        //                    {
        //                        xValues.Add(curve1.get_Y(i));
        //                        yValues.Add(curve2.get_Y(j));
        //                        i++;
        //                        if (i >= curve1.Count-1) { iIsAtTheEnd = true; break; }
        //                    }
        //                    j++;
        //                    if (j >= curve2.Count) { jIsAtTheEnd = true; break; }
        //                    if (curve2.get_X(j) > curve1.get_X(i + 1))
        //                    {
        //                        jSubsetDone = true;
        //                    }
        //                }
        //                //reset jIsDone
        //                jSubsetDone = false;
        //                if (jIsAtTheEnd == true) { break; }
        //                while (iSubsetDone == false)
        //                {
        //                    if (curve1.get_X(i) > curve2.get_X(j - 1) && curve1.get_X(i) < curve2.get_X(j))
        //                    {
        //                        xValues.Add(curve1.get_Y(i));
        //                        yValues.Add(getLinearInterpolatedValue(curve2.get_X(j - 1), curve1.get_X(i), curve2.get_X(j), curve2.get_Y(j - 1), curve2.get_Y(j)));
        //                    }
        //                    i++;
        //                    if (i >= curve1.Count) { iIsAtTheEnd = true; break; }
        //                    if (curve1.get_X(i) > curve2.get_X(j)) { iSubsetDone = true; }
        //                }
        //                iSubsetDone = false;
        //                i--;//this is to get it correct for the j loop

        //            }





        //            //should be correct here!!!!!!!!!!!!!!!!!!!!!!!!
        //            //need to handle the last point(s)
        //            if (iIsAtTheEnd == true)
        //            {
        //                //evaluate all the end j's
        //                while (jSubsetDone == false)
        //                {
        //                    if (curve1.get_X(i - 1) < curve2.get_X(j) && curve2.get_X(j) < curve1.get_X(i))
        //                    {
        //                        xValues.Add(getLinearInterpolatedValue(curve1.get_X(i - 1), curve2.get_X(j), curve1.get_X(i), curve1.get_Y(i - 1), curve1.get_Y(i)));
        //                        yValues.Add(curve2.get_Y(j));
        //                        //a j point has been added


        //                    }
        //                    j++;
        //                    if (j >= curve2.Count) { jIsAtTheEnd = true; break; }
        //                    if (curve2.get_X(j) > curve1.get_X(i))
        //                    {
        //                        jSubsetDone = true;
        //                    }
        //                }
        //                //potentially need to get the last i
        //                if (jIsAtTheEnd == false && curve1.get_X(i) < curve2.get_X(j) && curve1.get_X(i) > curve2.get_X(j - 1))
        //                {
        //                    //add the last i
        //                    xValues.Add(curve1.get_Y(i));
        //                    yValues.Add(getLinearInterpolatedValue(curve2.get_X(j - 1), curve1.get_X(i), curve2.get_X(j), curve2.get_Y(j - 1), curve2.get_Y(j)));

        //                }

        //            }


        //            if (jIsAtTheEnd == true)
        //            {
        //                //j--;
        //                //evaluate all the end i's
        //                while (iSubsetDone == false)
        //                {
        //                    if (curve1.get_X(i) > curve2.get_X(j - 1) && curve1.get_X(i) < curve2.get_X(j))
        //                    {
        //                        //i++;
        //                        xValues.Add(curve1.get_Y(i));
        //                        yValues.Add(getLinearInterpolatedValue(curve2.get_X(j - 1), curve1.get_X(i), curve2.get_X(j), curve2.get_Y(j - 1), curve2.get_Y(j)));
        //                        //an i point has been added
        //                        //i++;

        //                    }
        //                    i++;
        //                    if (i >= curve1.Count - 1) { iIsAtTheEnd = true; break; }
        //                    if (curve1.get_X(i) > curve2.get_X(j)) { iSubsetDone = true; }
        //                }
        //            }


        //        }
        //    }




        //    OrdinatesFunction ord = new OrdinatesFunction(curve1,FunctionTypes.LeveeFailure);



        //    return ord;
        //}



        //private static double getLinearInterpolatedValue(double a, double b, double c, double aPairedValue, double cPairedValue)
        //{
        //    double answer = 0;

        //    if(!(b<=c && b>=a))
        //    {
        //        throw new Exception("Not proper format. b must be between a and c");
        //    }
        //    //add more validation

        //    double interval = c - a;
        //    double howFarIsbIntoRange = b - a;
        //    double percentIntoRange = howFarIsbIntoRange / interval;
        //    double valueToAddToaPairedValue = (cPairedValue- aPairedValue) * percentIntoRange;
        //    answer = aPairedValue + valueToAddToaPairedValue;


        //    return answer;
        //}

        #endregion
    }
}