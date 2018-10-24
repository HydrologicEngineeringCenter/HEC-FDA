using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.Plots
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
        private Statistics.CurveIncreasing _FailureFunction;
        private List<FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction> _Curves;
        private string _Title;


        #endregion
        #region Properties
        public Statistics.CurveIncreasing FailureFunction
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
        public List<FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction> Curves
        {
            get { return _Curves; }
            set { _Curves = value; }
            
        }

        #endregion
        #region Constructors
        public ParentUserControlVM():base()
        {
        }

        public ParentUserControlVM(List<FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction> listOfOrdFuncs)
        {
            _Curves = listOfOrdFuncs;
            _FailureFunction = listOfOrdFuncs[0].Function ;
        }
        public ParentUserControlVM(List<FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction> listOfOrdFuncs, string failureFunctionTitle, string xAxisTitle = null, string yAxisTitle = null)
        {
            _Curves = listOfOrdFuncs;
            _FailureFunction = listOfOrdFuncs[0].Function;
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
