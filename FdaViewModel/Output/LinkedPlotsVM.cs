using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;


namespace FdaViewModel.Output
{
    //[Author(q0heccdm, 2 / 2 / 2017 11:42:33 AM)]
    public class LinkedPlotsVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 2/2/2017 11:42:33 AM
        #endregion
        #region Fields
        private FdaModel.ComputationPoint.Outputs.Result _Result;
        private List<FdaModel.Functions.BaseFunction> _FunctionsList;
        private FdaModel.ComputationPoint.Outputs.Realization _Realization;
        #endregion
        #region Properties
        public FdaModel.ComputationPoint.Outputs.Result Result
        {
            get { return _Result; }
            set { _Result = value; }
        }
        public List<FdaModel.Functions.BaseFunction> FunctionsList
        {
            get { return _FunctionsList; }
            set { _FunctionsList = value; }
        }
       
        public FdaModel.ComputationPoint.Outputs.Realization Realization
        {
            get { return _Realization; }
            set { _Realization = value; }
        }
        #endregion
        #region Constructors
        public LinkedPlotsVM() : base()
        {

        }

        public LinkedPlotsVM(FdaModel.ComputationPoint.Outputs.Realization realization)// List<FdaModel.Functions.BaseFunction> realizationFunctions)
        {

            FunctionsList = realization.Functions;//realizationFunctions;
        }
        public LinkedPlotsVM(FdaModel.ComputationPoint.Outputs.Result result)
        {

            Result = result;
        }
        #endregion
        #region Voids
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }
        #endregion
        #region Functions
        #endregion
    }
}
