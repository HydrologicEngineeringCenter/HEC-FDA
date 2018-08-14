using System;
using FdaModel.Utilities.Attributes;

namespace FdaModel.Inputs.Functions.OrdinatesFunctions
{
    [Author("John Kucharski", "06/10/2016", "08/17/2016")]
    class NonMonotonicOrdinatesFunction : OrdinatesFunction
    {

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

        #region Constructors
        [Tested(false)]
        /// <summary>
        /// This constructor is not intended for use. If used it will attempt to call the OrdinatesFunctionFactory. Note: A monotonically increasing function will be generated if the function of paired data is increasing on its range.
        /// </summary>
        /// <param name="xs"> Array of data passed as a representation of the function's domain. </param>
        /// <param name="ys"> Array of data passed as a representation of the function's range.  </param>
        internal NonMonotonicOrdinatesFunction(ComputationPoint indexPoint, float[] xs, float[] ys, FunctionTypes functionType) : base(indexPoint,functionType)
        {
            if (IsIncreasingFunction(ys) == true)
            {
                Utilities.Messager.Logger.Instance.ReportMessage(new Utilities.Messager.ErrorMessage("The provided data is monotonically increasing on its range (e.g. y values)." +
                                                                                            "If the data produces a valid function of paired data a monotonically increasing ordinate function will be generated instead."
                                                          , Utilities.Messager.ErrorMessageEnum.Minor));
                OrdinatesFunctionFactory(indexPoint, xs, ys, functionType);
            }
            else
            {
                Utilities.Messager.Logger.Instance.ReportMessage(new Utilities.Messager.ErrorMessage("Only monotonically increasing functions can be used in the compute. The input function will be stored but not used."
                                                        , Utilities.Messager.ErrorMessageEnum.Minor));
                OrdinatesFunctionFactory(indexPoint, xs, ys, functionType = FunctionTypes.UnUsed);
            }  
        }
        #endregion

        #region Functions
        public override bool Validate()
        {
            throw new NotImplementedException();
        }

        public override BaseFunction SampleFunction(Random randomNumberGnerator)
        {
            return this;
        }

        

      
        #endregion
    }
}
