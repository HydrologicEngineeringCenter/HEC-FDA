using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Plots
{
    //[Author(q0heccdm, 3 / 28 / 2017 2:27:37 PM)]
    public class ParentUserControlVM:BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 3/28/2017 2:27:37 PM
        #endregion
        #region Fields
        //private FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction _FailureFunction;
        private IFdaFunction _FailureFunction;
        private List<IFdaFunction> _Curves;
        private string _Title;


        #endregion
        #region Properties
        public IFdaFunction FailureFunction
        {
            get { return _FailureFunction; }
            set { _FailureFunction = value; }
        }

        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }
        public string XAxisLabel { get; set; }
        public string YAxisLabel { get; set; }
        public List<IFdaFunction> Curves
        {
            get { return _Curves; }
            set { _Curves = value; }
            
        }

        #endregion
        #region Constructors
        public ParentUserControlVM():base()
        {
        }

        public ParentUserControlVM(List<IFdaFunction> listOfOrdFuncs)
        {
            _Curves = listOfOrdFuncs;
            //todo: Refactor: commenting out
            //_FailureFunction = listOfOrdFuncs[0].Function ;
        }
        public ParentUserControlVM(List<IFdaFunction> listOfOrdFuncs, string failureFunctionTitle, string xAxisTitle = null, string yAxisTitle = null)
        {
            _Curves = listOfOrdFuncs;
            //todo: Refactor: commenting out
            //_FailureFunction = listOfOrdFuncs[0].Function;
            Title = failureFunctionTitle;
            XAxisLabel = xAxisTitle;
            YAxisLabel = yAxisTitle;
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

     
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
    }
}
