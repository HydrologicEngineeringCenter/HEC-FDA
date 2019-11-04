using Model.Condition.ComputePoint.ComputableFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    internal sealed class InflowOutflow<YType>: ImpactAreaFunctionBase<YType>, ITransformFunction<YType>
    {
        #region Properties
        public override string XLabel => "Inflow";

        public override string YLabel => "Outflow";
        #endregion

        #region Constructor
        internal InflowOutflow(Functions.ICoordinatesFunction<double, YType> function) : base(function, ImpactAreaFunctionEnum.InflowOutflow)
        {
       
        }

        //public IComputableTransformFunction Sample(double p)
        //{
        //    return new InflowOutflowComputable(Function.Sample(p));
        //}
        #endregion

        //#region IFunctionTransform Methods
        //public ITransformFunction Sample(double probability)
        //{
        //    return ImpactAreaFunctionFactory.CreateNew(Function.Sample(probability), Ordinates, Type);
        //}
        //#endregion

        //#region IValidateData Method
        //public override bool Validate()
        //{
        //    if (Ordinates[0].Item1 < Ordinates[0].Item2)
        //    {
        //        ReportValidationErrors();
        //        return false;
        //    }
        //    for (int i = 0; i < Ordinates.Count; i++)
        //    {
        //        if (Ordinates[i].Item1 < 0 ||
        //            Ordinates[i].Item2 < 0 ||
        //            Ordinates[i].Item1 > 30000000 ||
        //            Ordinates[i].Item2 > 30000000)
        //        {
        //            ReportValidationErrors();
        //            return false;
        //        }
        //    }
        //    if (Function.IsValid == false)
        //    {
        //        ReportValidationErrors();
        //        return false;
        //    }
        //    else return true; 
        //}
        //public override IEnumerable<string> ReportValidationErrors()
        //{
        //    List<string> messages = new List<string>();
        //    if (Ordinates[0].Item1 < Ordinates[0].Item2)
        //        messages.Add(new StringBuilder("The first inflow ordinate must exceed the first outflow ordinate. In the provided function the first inflow value, ").Append(Ordinates[0].Item1).Append(" is exceed by the first outflow value, ").Append(Ordinates[0].Item2).ToString());
        //    for (int i = 0; i < Ordinates.Count; i++)
        //    {
        //        if (Ordinates[i].Item1 < 0) messages.Add(new StringBuilder("The function contains the invalid negative inflow value of ").Append(Ordinates[i].Item1).ToString());
        //        if (Ordinates[i].Item1 < 0) messages.Add(new StringBuilder("The function contains the invalid negative outflow value of ").Append(Ordinates[i].Item1).ToString());
        //        if (Ordinates[i].Item1 > 30000000) messages.Add(new StringBuilder("Flow values may not exceed 30,000,000. The function contains the invalid inflow value of ").Append(Ordinates[i].Item1).ToString());
        //        if (Ordinates[i].Item1 > 30000000) messages.Add(new StringBuilder("Flow values may not exceed 30,000,000. The function contains the invalid outflow value of ").Append(Ordinates[i].Item2).ToString());
        //    }
        //    messages.AddRange(base.ReportValidationErrors());
        //    return messages;
        //}
        //#endregion
    }
}
