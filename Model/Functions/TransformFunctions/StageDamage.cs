using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Functions;

namespace Model.Functions
{
    internal sealed class StageDamage : FdaFunctionBase, ITransformFunction
    {
        #region Properties
        public override string Label { get; }
        public override IParameter XSeries { get; }
        public override IParameter YSeries { get; }
        public override UnitsEnum Units { get; }
        public override IParameterEnum ParameterType => IParameterEnum.InteriorStageDamage;
        #endregion

        #region Constructor
        internal StageDamage(IFunction fx, string label, UnitsEnum xUnits = UnitsEnum.Foot, string xlabel = "", UnitsEnum yUnits = UnitsEnum.Dollars, string ylabel = "") : base(fx)
        {
            Label = label == "" ? ParameterType.Print() : label;
            xlabel = xlabel == "" ? $"Interior Stage ({xUnits.Print(true)})" : xlabel;
            ylabel = ylabel == "" ? $"Damages ({yUnits.Print(true)})" : ylabel;
            XSeries = IParameterFactory.Factory(fx, IParameterEnum.InteriorElevation, true, true, xUnits, xlabel);
            YSeries = IParameterFactory.Factory(fx, IParameterEnum.FloodDamages, IsConstant, false, yUnits, ylabel);
            Units = YSeries.Units;
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
        //public override XElement WriteToXML()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
