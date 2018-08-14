using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaModel.Inputs.Functions
{
    public abstract class BaseFunction
    {

        #region Fields
        protected bool _IsValid;
        protected FunctionTypes _Type;
        protected ComputationPoint _IndexPoint;
        #endregion

        #region Properties
        public FunctionTypes FunctionType
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }
        public ComputationPoint IndexPoint
        {
            get
            {
                return _IndexPoint;
            }
            set
            {
                _IndexPoint = value;
            }
        }
        #endregion

        #region Functions
        public abstract bool Validate();
        // check typeof based on enum value.
        public abstract BaseFunction SampleFunction(Random randomNumberGenerator);
        #endregion


    }
}
