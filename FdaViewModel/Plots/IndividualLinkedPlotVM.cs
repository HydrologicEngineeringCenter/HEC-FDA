using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.Plots
{
    //[Author(q0heccdm, 4 / 4 / 2017 2:30:31 PM)]
    public class IndividualLinkedPlotVM:BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 4/4/2017 2:30:31 PM
        #endregion
        #region Fields
        private Statistics.CurveIncreasing _MyCurve;
        private bool _IsVisible;
        #endregion
        #region Properties
        public Statistics.CurveIncreasing Curve
        {
            get { return _MyCurve; }
            set { _MyCurve = value; NotifyPropertyChanged(); }
        }
       
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string XAxisLabel { get; set; }
        public string YAxisLabel { get; set; }

        public bool IsVisible
        {
            get { return _IsVisible; }
            set { _IsVisible = value; NotifyPropertyChanged(); }
        }
        public string SelectedElementName { get; set; }

        public FdaModel.Functions.BaseFunction BaseFunction { get; set; }
        #endregion
        #region Constructors

        public IndividualLinkedPlotVM():base()
        {

        }

        public IndividualLinkedPlotVM(FdaModel.Functions.BaseFunction baseFunction, Statistics.CurveIncreasing curve)
        {
            BaseFunction = baseFunction;
            if(baseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency)
            {
                Curve = ((FdaModel.Functions.FrequencyFunctions.LogPearsonIII)baseFunction).GetOrdinatesFunction().Function;
            }
            else
            {
                Curve = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)baseFunction).Function;
            }
            
            //_MyCurve = curve;
        }
        public IndividualLinkedPlotVM(FdaModel.Functions.BaseFunction baseFunction, Statistics.CurveIncreasing curve,string title, string xAxisLabel,string yAxisLabel, string selectedElementName = "")
        {
            BaseFunction = baseFunction;
           
            SelectedElementName = selectedElementName;
            if (baseFunction.GetType().BaseType == typeof(FdaModel.Functions.FrequencyFunctions.FrequencyFunction))
            {
                //is it a 0,2,4,6,8
                if(baseFunction.GetType() == typeof(FdaModel.Functions.FrequencyFunctions.LogPearsonIII))
                {
                    Curve = ((FdaModel.Functions.FrequencyFunctions.LogPearsonIII)baseFunction).GetOrdinatesFunction().Function;

                }

            }
            else if(baseFunction.GetType() == typeof(FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction))
            {
                Curve = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)baseFunction).Function;

            }
            else if(baseFunction.GetType() == typeof(FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction))
            {
                if (baseFunction.FunctionType == FdaModel.Functions.FunctionTypes.Rating)
                {
                    //switch the xs and ys
                    FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction original = ((FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction)baseFunction).GetOrdinatesFunction();
                    FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction ord = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(original.Function.YValues, original.Function.XValues, FdaModel.Functions.FunctionTypes.Rating);
                    Curve = ord.Function;
                }
                else
                {
                    Curve = ((FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction)baseFunction).GetOrdinatesFunction().Function;
                }

            }



            //_MyCurve = curve;
           
            Title = title;
            XAxisLabel = xAxisLabel;
            YAxisLabel = yAxisLabel;
        }
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
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
