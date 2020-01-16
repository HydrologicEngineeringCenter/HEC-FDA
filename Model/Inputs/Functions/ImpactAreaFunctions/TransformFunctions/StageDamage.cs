using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Functions;

namespace Model.Inputs.Functions.ImpactAreaFunctions
{
    internal sealed class StageDamage : ImpactAreaFunctionBase, ITransformFunction
    {
        #region Properties
        public override string XLabel => "Interior Stage";

        public override string YLabel => "Damage";
    
        #endregion

        #region Constructor
        internal StageDamage(ICoordinatesFunction function) : base(function, ImpactAreaFunctionEnum.InteriorStageDamage)
        {

        }
       
        #endregion

 
        
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

        public override XElement WriteToXML()
        {
            throw new NotImplementedException();
        }

    }
}
