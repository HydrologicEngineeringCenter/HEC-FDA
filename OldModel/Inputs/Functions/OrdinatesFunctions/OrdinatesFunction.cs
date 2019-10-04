using System.Collections.Generic;
using FdaModel.Utilities.Attributes;

namespace FdaModel.Inputs.Functions.OrdinatesFunctions
{
    [Author("John Kucharski", "06/08/2016", "08/22/2016")]
    public abstract class OrdinatesFunction: BaseFunction
    {
        #region Notes
        /*Development Notes: 1. Need a more elegant way of dealing withe exceptions, on the OrdinatesFactory mehtod, or move this to ViewModel data validation class. */
        #endregion

        #region Constructors
        [Tested(false)]
        /// <summary>
        /// Creates a representation of a function consisting of ordered x and paired y values.
        /// </summary>
        /// <param name="computationPoint"> An object, like an index point, which describes the time and place of the compute. </param>
        /// <param name="xs"> Array of data passed as a representation of the function's domain. </param>
        /// <param name="ys"> Array of data passed as a representation of the function's range.  </param>
        /// <param name="functionType"> An enum descibing the type of function, the that X and Y parameters describe. </param>
        /// <remarks> The data must produce a function of ordered pairs. In other words the paired data must be sorted with respect to the independent variable (xs) and meet the definition of a function meaning each independent variable value (e.g. x) must be paired with at most one depedent variable value (e.g. y). </remarks>
        protected OrdinatesFunction(ComputationPoint indexPoint, FunctionTypes functionType)
        {
            IndexPoint = indexPoint;
            FunctionType = functionType;
        }
        #endregion

        #region Functions
        [Tested(false)]
        /// <summary>
        /// Generates an instance of the type of ordinates function that is best describes the provided domain and range data.
        /// </summary>
        /// <param name="xs"> Array of data passed as a representation of the function's domain. </param>
        /// <param name="ys"> Array of data passed as a representation of the function's range.  </param>
        /// <returns></returns>
        public static BaseFunction OrdinatesFunctionFactory(ComputationPoint indexPoint, float[] xs, float[] ys, FunctionTypes functionType)
        {
            if (IsValidFunction(xs, ys) == true)
            {
                if (IsIncreasingFunction(ys) == true)
                {

                    return new IncreasingOrdinatesFunction(indexPoint, xs, ys, functionType);
                }
                else
                {
                    Utilities.Messager.Logger.Instance.ReportMessage(new Utilities.Messager.ErrorMessage("The provided data is not monotoncally increasing. It is saved as an ordinates function, but cannot be used to perform computations. " +
                                                                                                "To clear this error, edit the function to ensure it is increasing across all x,y pairs."
                                                                                                , Utilities.Messager.ErrorMessageEnum.Major));
                    return new NonMonotonicOrdinatesFunction(indexPoint, xs, ys, functionType = FunctionTypes.UnUsed);
                }
            }
            else
            {
                Utilities.Messager.Logger.Instance.ReportMessage(new Utilities.Messager.ErrorMessage("The provided data is not a function of ordered pairs, and therefore cannot be constructed as a ordinates functions. " +
                                                                                                        "To be a function of ordered pairs: (a) more than one ordiante must be supplied, " +
                                                                                                            "(b) the supplied ordinates must be ordered along their domain (e.g. xs), " +
                                                                                                            "(c) and each independent variable value (e.g. x) may be associated with at most one dependent variable (e.g. y) value."
                                                                                                        , Utilities.Messager.ErrorMessageEnum.Major));
                return null;
            }
        }

        [Tested(false)]
        /// <summary>
        /// Tests if supplied domain and range data meet the definition of a function of paired data, with at least two ordinates.
        /// </summary>
        /// <param name="xs"> Domain of function. </param>
        /// <param name="ys"> Range of function. </param>
        /// <returns> True if supplied paired data is ordered along its domain, at least 2 ordinates are provided and each independent variable value (e.g. x value) is mapped to at most one dependent variable value (e.g. y value). </returns>
        public static bool IsValidFunction(float[] xs, float[] ys)
        {
            // 1. Is it long enough and is there an equal set of elements in each array?
            if (xs.Length < 2 |
                ys.Length < 2 |
                xs.Length != ys.Length)
            {
                return false;
            }
            // 2. Are the xs ordered and is each x mapped to at most one y (e.g. is it a function)?
            for (int i = 0; i < xs.Length - 1; i++)
            {
                if (xs[i + 1] < xs[i])
                {
                    return false;
                }
                if (xs[i + 1] == xs[i] && ys[i + 1] != ys[i])
                {
                    return false;
                }
            }
            return true;
        }

        [Tested(false)]
        /// <summary>
        /// Tests if the supplied array is in accending order.  
        /// </summary>
        /// <param name="ys"> Array to test. </param>
        /// <returns> True if the array is in accending order and false otherwise. </returns>
        /// <remarks> Reports if any values in the supplied array are repeated (e.g. the data is not strictly increasing). </remarks>
        public static bool IsIncreasingFunction(float[] ys)
        {
            List<double> repeatedValues = new List<double>();
            for (int i = 0; i < ys.Length - 1; i++)
            {
                if (ys[i + 1] < ys[i])
                {
                    return false;
                }

                else if (ys[i + 1] == ys[i])
                {
                    repeatedValues.Add(ys[i]);
                }
            }
            if (repeatedValues.Count > 0)
            {
                string set = repeatedValues[0].ToString();
                for (int i = 1; i < repeatedValues.Count; i++)
                {
                    set.Insert(set.Length, ", " + repeatedValues[i]);
                }
                Utilities.Messager.Logger.Instance.ReportMessage(new Utilities.Messager.ErrorMessage("The function is not strictly increasing. The following values are repeated: {" + set + "}."
                                                          , Utilities.Messager.ErrorMessageEnum.Minor));
            }
            return true;
        }
        #endregion
    }
}