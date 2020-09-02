using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Functions;
using Utilities;

namespace Model.Functions
{
    internal sealed class StageDamage : FdaFunctionBase, ITransformFunction
    {
        #region Properties
        public override string Label { get; }
        public override IParameterRange XSeries { get; }
        public override IParameterRange YSeries { get; }
        public override IParameterEnum ParameterType => IParameterEnum.InteriorStageDamage;

        public override IMessageLevels State { get; }
        public override IEnumerable<IMessage> Messages { get; }
        #endregion

        #region Constructor
        internal StageDamage(IFunction fx, string label, UnitsEnum xUnits = UnitsEnum.Foot, string xlabel = "", UnitsEnum yUnits = UnitsEnum.ThousandDollars, string ylabel = "") : base(fx)
        {
            Label = label == "" ? ParameterType.Print() : label;
            XSeries = IParameterFactory.Factory(fx, IParameterEnum.InteriorElevation, true, true, xUnits, xlabel);
            YSeries = IParameterFactory.Factory(fx, IParameterEnum.FloodDamages, IsConstant, false, yUnits, ylabel);
            State = Validate(new Validation.Functions.FdaFunctionBaseValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
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
