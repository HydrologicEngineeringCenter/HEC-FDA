using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    internal sealed class StageDamage<YType> : ImpactAreaFunctionBase
    {
        #region Properties
        public override string XLabel => "Interior Stage";

        public override string YLabel => "Damage";

        #endregion

        #region Constructor
        internal StageDamage(Functions.ICoordinatesFunction function) : base(function, ImpactAreaFunctionEnum.InteriorStageDamage)
        {
            
        }

     
        //internal StageDamage(): base(new OrdinatesFunction(new Statistics.CurveIncreasing(true, false)))
        //{
        //    Type = ImpactAreaFunctionEnum.InteriorStageDamage;
        //}
        #endregion

        //#region IFunctionTransform Methods
        //public ITransformFunction Sample(double probability)
        //{
        //    return ImpactAreaFunctionFactory.CreateNew(Function.Sample(probability), Ordinates, Type);
        //}
        //#endregion

        //#region IStageDamageTransform
        ////private void Sum(IFunctionBase toAdd)
        ////{
        ////    var toAddOrdinates = toAdd.GetOrdinates();
        ////    IStageDamageTransform lowerFunction, higherFunction;
        ////    if (Ordinates[0].Item1 <= toAddOrdinates[0].Item1) { lowerFunction = this; higherFunction = (OrdinatesFunction)toAdd; }
        ////    else { lowerFunction = toAdd; higherFunction = this; }

        ////    List<double> stages = new List<double>(), damages = new List<double>();
        ////    List<Tuple<double, double>> newOrdinates = new List<Tuple<double, double>>();
        ////    int i = 0, j = 0, I = lowerFunction.Ordinates.Count, J = higherFunction.Ordinates.Count;
        ////    while (lowerFunction.Ordinates[i].Item1 < higherFunction.Ordinates[0].Item1)
        ////    {
        ////        newOrdinates.Add(lowerFunction.Ordinates[i]);
        ////        i++;
        ////    } 

        ////    while (i < I)
        ////    {
        ////        if (lowerFunction.Ordinates[i].Item1 <= higherFunction.Ordinates[i].Item1)
        ////        {
        ////            newOrdinates.Add(new Tuple<double, double>(lowerFunction.Ordinates[i].Item1, lowerFunction.Ordinates[i].Item2 + higherFunction.GetYFromX(lowerFunction.Ordinates[i].Item1)));
        ////            if (lowerFunction.Ordinates[i].Item1 == higherFunction.Ordinates[j].Item1)
        ////            {
        ////                if (j + 1 < J) j++;
        ////                else
        ////                {
        ////                    while (i < I)
        ////                    {
        ////                        newOrdinates.Add(new Tuple<double, double>(lowerFunction.Ordinates[i].Item1, lowerFunction.Ordinates[i].Item2 + higherFunction.Ordinates[j].Item2));
        ////                        i++;
        ////                    }
        ////                    break;
        ////                }

        ////            }
        ////            i++;
        ////        }
        ////        else
        ////        {

        ////        }
        ////    } 
        ////}
        //private List<Tuple<double, double>> AddPointsAbove(List<Tuple<double, double>> ordinatesToAdd, double constantStage)
        //{ throw new NotImplementedException(); }
        //public double GetYFromX(double x)
        //{
        //    return Function.GetYfromX(x);
        //}
        //#endregion

        //public void Aggregate(IImpactArea wsps, IInventory inventory)
        //{
        //    bool firstPass = true;
        //    StageDamageInventory functions = new StageDamageInventory(wsps, inventory);

        //    foreach (var structure in functions.StageDamageFunctions)
        //    {
        //        foreach (var asset in structure.Value)
        //        {
        //            if (firstPass == true) { Function = asset.Value; firstPass = false; }
        //            //else Sum(asset.Value);
        //        }
        //    }
        //}

    }
}
