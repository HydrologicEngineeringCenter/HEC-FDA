using Functions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Utilities;

namespace Model.Functions
{
    internal sealed class Rating : FdaFunctionBase, ITransformFunction
    {
        #region Properties
        public override string Label { get; }
        public override IParameterRange XSeries { get; }
        public override IParameterRange YSeries { get; }
        public override IParameterEnum ParameterType => IParameterEnum.Rating;

        public override IMessageLevels State { get; }
        public override IEnumerable<IMessage> Messages { get; }
        #endregion

        #region Constructor
        internal Rating(ICoordinatesFunction fx, string label, UnitsEnum xUnits = UnitsEnum.CubicFootPerSecond, string xlabel = "", UnitsEnum yUnits = UnitsEnum.Foot, string ylabel = "") : base(fx)
        {
            Label = label == "" ? ParameterType.Print() : label;
            XSeries = IParameterFactory.Factory(fx, IParameterEnum.RegulatedAnnualPeakFlow, true, true, xUnits, xlabel);
            YSeries = IParameterFactory.Factory(fx, IParameterEnum.ExteriorElevation, IsConstant, false, yUnits, ylabel);
            State = Validate(new Validation.Functions.FdaFunctionBaseValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }
        #endregion

        #region IFunctionTransform Methods

        #endregion

        #region IValidateData Methods
        //public override bool Validate()
        //{
        //    if (Function.IsValid == false) ReportValidationErrors();
        //    if (Ordinates[0].Item1 < 0 ||
        //        Ordinates[0].Item2 < -1500d ||
        //        Ordinates[Ordinates.Count - 1].Item1 > 30000000d ||
        //        Ordinates[Ordinates.Count - 1].Item2 > 30000d) { ReportValidationErrors(); return false; }
        //    return Function.IsValid;
        //}
        //public override IEnumerable<string> ReportValidationErrors()
        //{
        //    List<string> messages = new List<string>();
        //    if (Ordinates[0].Item1 < 0) messages.Add("The rating function contains a flow that is less than 0, only positive flow values are permitted.");
        //    if (Ordinates[0].Item2 < -1500d) messages.Add(new StringBuilder("The rating function contains a stage value of ").Append(Ordinates[0].Item2).Append(" this is below the lowest allowable value of -1,371 (which is the lowest land elvation on earth in feet). Also John Kucharski owes William Lehman a beer, please call him to inform him of this fact.").ToString());
        //    if (Ordinates[Ordinates.Count - 1].Item1 > 30000000d) messages.Add("The rating function contains a flow value that is greater than largest permisible flow value of 30,000,000.");
        //    if (Ordinates[Ordinates.Count - 1].Item2 > 30000d) messages.Add(new StringBuilder("The rating function contains a stage value of ").Append(Ordinates[0].Item2).Append(" this is above the highest allowable value of 29,029 (which is the hightest land elvation on earth in feet). Also John Kucharski owes William Lehman a beer, please call him to inform him of this fact.").ToString());
        //    messages.AddRange(Function.ReportValidationErrors());
        //    return messages;
        //}
        #endregion
        //public override XElement WriteToXML()
        //{
        //    return _Function.WriteToXML();
        //}
    }
}
