using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private IFdaFunction _MyCurve;
        private bool _IsVisible;
        //public event EventHandler CurveUpdated;
        #endregion
        #region Properties
        public IFdaFunction Curve
        {
            get { return _MyCurve; }
            set { _MyCurve = value; NotifyPropertyChanged(); }
                //CurveUpdated?.Invoke(this, new EventArgs()); }
        }
        public IFdaFunction NonStandardDeviationCurve { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string XAxisLabel { get; set; } 
        public string YAxisLabel { get; set; }
        public bool XAxisIsStandardDeviation { get; set; }

        public bool IsVisible
        {
            get { return _IsVisible; }
            set { _IsVisible = value; NotifyPropertyChanged(); }
        }
        public string SelectedElementName { get; set; }
       

        public IFdaFunction BaseFunction { get; set; }
        #endregion
        #region Constructors
        public IndividualLinkedPlotVM():base()
        {
        }

        public IndividualLinkedPlotVM(IFdaFunction baseFunction, string selectedElementName) : this(baseFunction,"","","",false,selectedElementName)
        {
          
            //BaseFunction = baseFunction;
            //if(baseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency)
            //{
            //    Curve = ((FdaModel.Functions.FrequencyFunctions.LogPearsonIII)baseFunction).GetOrdinatesFunction().Function;
            //}
            //else if(BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InteriorStageDamage)
            //{
            //    Curve = TrimTrailingZeroes(((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)baseFunction).Function);
            //}
            //else
            //{
            //    Curve = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)baseFunction).Function;
            //}
            
            //_MyCurve = curve;
        }
        public IndividualLinkedPlotVM(IFdaFunction baseFunction,string title, string xAxisLabel,string yAxisLabel, bool isXAxisStandardDeviations = false, string selectedElementName = "")
        {
            XAxisIsStandardDeviation = isXAxisStandardDeviations;
            BaseFunction = baseFunction;
            SelectedElementName = selectedElementName;
            SubTitle = selectedElementName;
            //todo: Refactor: Commenting out
            //if (baseFunction.GetType().BaseType == typeof(FdaModel.Functions.FrequencyFunctions.FrequencyFunction))
            //{
            //    //is it a 0,2,4,6,8
            //    if(baseFunction.GetType() == typeof(FdaModel.Functions.FrequencyFunctions.LogPearsonIII))
            //    {
            //        Curve = ((FdaModel.Functions.FrequencyFunctions.LogPearsonIII)baseFunction).GetOrdinatesFunction().Function;
            //        //Curve = ConvertXValuesToStandardDeviation(((FdaModel.Functions.FrequencyFunctions.LogPearsonIII)baseFunction).GetOrdinatesFunction().Function);
            //    }

            //}
            //else if(baseFunction.GetType() == typeof(FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction))
            //{
            //    if (BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InteriorStageDamage)
            //    {
            //        Curve = TrimTrailingZeroes(((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)baseFunction).Function);
            //    }
            //    else
            //    {
            //        if(XAxisIsStandardDeviation)
            //        {
            //            Curve = ConvertXValuesToStandardDeviation(((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)baseFunction).Function);
            //            NonStandardDeviationCurve = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)baseFunction).Function;
            //        }
            //        else
            //        {
            //            Curve = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)baseFunction).Function;
            //        }
            //    }
            //}
            //else if(baseFunction.GetType() == typeof(FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction))
            //{
            //    if (baseFunction.FunctionType == FdaModel.Functions.FunctionTypes.Rating)
            //    {
            //        //switch the xs and ys
            //        FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction original = ((FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction)baseFunction).GetOrdinatesFunction();
            //        FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction ord = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(original.Function.YValues, original.Function.XValues, FdaModel.Functions.FunctionTypes.Rating);
            //        Curve = ord.Function;
            //    }
            //    else
            //    {
            //        Curve = ((FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction)baseFunction).GetOrdinatesFunction().Function;
            //    }

            //}
           
            Title = title;
            XAxisLabel = xAxisLabel;
            YAxisLabel = yAxisLabel;
        }

        #endregion
        #region Voids
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }


        #endregion
        #region Functions
        //todo: Refactor: Commenting out
        //public Statistics.CurveIncreasing ConvertXValuesToStandardDeviation(Statistics.CurveIncreasing curve)
        //{
        //    double meanXValue = curve.XValues.Average();

        //    List<double> newXValues = new List<double>();
        //    List<double> newYValues = new List<double>();
        //    double j = 0;
        //    for (int i = 0; i < curve.XValues.Count; i++)
        //    {
        //        double val = curve.XValues[i];
        //        if(val<=.001)
        //        {
        //            newXValues.Add(-3 + (j/100));
        //        }
        //        else if(val<=.0228 && val>.001)
        //        {
        //            newXValues.Add(-2 + (j / 100));
        //        }
        //        else if (val <= .1587 && val > .0228)
        //        {
        //            newXValues.Add(-1 + (j / 100));
        //        }
        //        else if (val <= .5 && val > .1587)
        //        {
        //            newXValues.Add(0 + (j / 100));
        //        }

        //        else if (val <= .8413 && val > .5)
        //        {
        //            newXValues.Add(1 + (j / 100));
        //        }
        //        else if (val <= .9772 && val > .8413)
        //        {
        //            newXValues.Add(2 + (j / 100));
        //        }
        //        else if (val <= .9987 && val > .9772)
        //        {
        //            newXValues.Add(3 +(j / 100));
        //        }
        //        else if (val > .9987)
        //        {
        //            newXValues.Add(4 + (j / 100));
        //        }
        //        else
        //        {
        //            newXValues.Add(4 + (j/100));
        //        }

        //        //newXValues.Add(Math.Sqrt(Math.Pow(curve.XValues[i] - meanXValue, 2)));
        //        newYValues.Add(curve.YValues[i]);
        //        j++;
        //    }
        //   // newXValues.Sort();
        //    Statistics.CurveIncreasing newCurve = TrimTrailingZeroes( new Statistics.CurveIncreasing(newXValues, newYValues, true, true));
        //    return newCurve;
        //}
        //todo: Refactor: Commenting out
        //private Statistics.CurveIncreasing TrimTrailingZeroes(Statistics.CurveIncreasing curve)
        //{
        //    if(curve.Count > 0 && curve.YValues[0] > 0) { return curve; }//early exit if there aren't any zeros at the beginning
        //    Statistics.CurveIncreasing damageCurveWithNoZeroes = new Statistics.CurveIncreasing();
        //    for (int i = 0; i < curve.Count; i++)
        //    {
        //        if (curve.YValues[i] != 0)
        //        {
        //            damageCurveWithNoZeroes.Add(curve.XValues[i], curve.YValues[i]);
        //        }
        //    }

        //    return damageCurveWithNoZeroes;
        //}
        #endregion
    }
}
