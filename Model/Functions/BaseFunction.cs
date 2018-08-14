using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaModel.Functions.OrdinatesFunctions;
using FdaModel.Utilities.Messager;

namespace FdaModel.Functions
{
    public abstract class BaseFunction : IValidate
    {

        #region Fields
        //protected bool _IsValid;
        protected FunctionTypes _FunctionType;
        protected ModelErrors _Messages;
        #endregion

        #region Properties
        public FunctionTypes FunctionType
        {
            get
            {
                return _FunctionType;
            }
            set
            {
                _FunctionType = value;
            }
        }
        public ModelErrors Messages
        {
            get
            {
                return _Messages;
            }
            set
            {
                _Messages = value;
            }
        }
        #endregion

        #region Constructor
        public BaseFunction( )
        {
        }
        #endregion

        #region Functions       
        // check typeof based on enum value.
        public abstract BaseFunction SampleFunction(Random randomNumberGenerator);

        public abstract OrdinatesFunction GetOrdinatesFunction( );

        public abstract OrdinatesFunction Compose(OrdinatesFunction YsFunction, ref List<ErrorMessage> errors);

        public abstract double GetXfromY(double Y, ref List<ErrorMessage> errors);
        #endregion

        #region IValidateMembers
        public abstract void Validate( );
        #endregion

    }
}
