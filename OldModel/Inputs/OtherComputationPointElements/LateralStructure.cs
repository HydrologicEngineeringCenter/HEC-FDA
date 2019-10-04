using FdaModel.Utilities.Attributes;
using FdaModel.Utilities.Messager;
using FdaModel.Functions;
using FdaModel.Functions.OrdinatesFunctions;

namespace FdaModel.Inputs.OtherComputationPointElements
{
    [Author("John Kucharski", "10/11/2016")]
    public class LateralStructure
    {
        #region Notes
        /* This class provides a damage truncation point which is necessary for levee projects and those with perched rivers. Levee fragility curves are stored as seperate (increasing ordinate function) objects. */
        #endregion

        #region Fields
        private bool _IsComputable;
        private bool _IsLevee;          //true = levee, false = bank
        private double _Elevation;
        private BaseFunction _FailureFunction;
        #endregion

        #region Properties
        public bool IsComputable
        {
            get
            {
                return _IsComputable;
            }
            set
            {
                _IsComputable = value;
            }
        }
        public bool IsLevee
        {
            get
            {
                return _IsLevee;
            }
            set
            {
                _IsLevee = value;
            }
        }
        public double Elevation
        {
            get
            {
                return _Elevation;
            }
            set
            {
                _Elevation = value;
            }
        }
        public BaseFunction FailureFunction
        {
            get
            {
                return _FailureFunction;
            }
            set
            {
                _FailureFunction = value;
            }
        }
        #endregion


        #region Constructor
        [Tested(false)]
        /// <summary> Constructor for structure with no fragility. </summary>
        /// <param name="isLevee"></param>
        /// <param name="elevation"></param>
        public LateralStructure(bool isLevee, double elevation)
        {
            IsLevee = isLevee;
            Elevation = elevation;
            IsComputable = true;
        }

        public LateralStructure(bool isLevee, double elevation, FdaModel.ComputationPoint indexPoint, float[] xs, float[] ys)
        {
            IsLevee = isLevee;
            Elevation = elevation;
            FailureFunction = OrdinatesFunction.OrdinatesFunctionFactory(indexPoint, xs, ys, FdaModel.Functions.FunctionTypes.LeveeFailure);
            IsComputable = Validate();
        }
        #endregion

        #region Voids
        public bool Validate()
        {
            if (FailureFunction.GetType() == typeof(IncreasingOrdinatesFunction))
            {
                IncreasingOrdinatesFunction failureFunction = (IncreasingOrdinatesFunction)FailureFunction;
                for (int i = 0; i < failureFunction.Xs.Length; i++)
                {
                    if (failureFunction.Xs[i] > Elevation)
                    {
                        double dx = (Elevation - failureFunction.Xs[i - 1]) / (failureFunction.Xs[i] - failureFunction.Xs[i - 1]);
                        double probability = failureFunction.Ys[i - 1] + dx * (failureFunction.Ys[i] - failureFunction.Ys[i - 1]);
                        if (probability < 1)
                        {
                            Logger.Instance.ReportMessage(new ErrorMessage("The probability of failure at the top of the levee is estimated to be " + probability + ", " +
                                                                  "by convention the probability of failure above the top of the levee must equal 1." +
                                                                  "Consequently, the lateral structure cannot be use in the compute."
                                                                  , ErrorMessageEnum.Model & ErrorMessageEnum.Major));
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (i == failureFunction.Xs.Length - 1)
                    {
                        if (failureFunction.Ys[i] < 1)
                        {
                            Logger.Instance.ReportMessage(new ErrorMessage("The lateral structure fragility function does not contain a maximum (e.g. 100%) failure point, " +
                                                                  "by convention the probability of failure above the top of levee must equal 1. " +
                                                                  "A maximum failure point (e.g. [" + Elevation + " , 1]) must be added to the fragility function, before it can be used in the compute."
                                                                  , ErrorMessageEnum.Model & ErrorMessageEnum.Major));
                            return false;
                        }
                    }
                }
            }
            else
            {
                Logger.Instance.ReportMessage(new ErrorMessage("The lateral structure fragility function ordiantes are not monotonically increasing. " +
                                                      "Consequently, the lateral structure cannot be used in the compute."
                                                      , ErrorMessageEnum.Model & ErrorMessageEnum.Major));
                return false;
            }
            return true;
        }
        #endregion
    }
}
