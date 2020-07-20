using System;
using System.Text;
using System.Collections.Generic;
using Utilities;
using Functions;
using System.Xml.Linq;

namespace Model.Functions
{

    internal sealed class ExteriorInteriorStage: FdaFunctionBase, ITransformFunction
    {
        #region Properties
        public override string Label { get; }
        public override IParameter XSeries { get; }
        public override IParameter YSeries { get; }
        public override UnitsEnum Units { get; }
        public override IParameterEnum ParameterType => IParameterEnum.ExteriorInteriorStage;
        #endregion

        #region Constructor
        internal ExteriorInteriorStage(IFunction fx, string label, UnitsEnum xUnits = UnitsEnum.Foot, string xLabel = "", UnitsEnum yUnits = UnitsEnum.Foot, string ylabel = ""): base(fx)
        {
            Label = label == "" ? ParameterType.Print() : label;
            XSeries = IParameterFactory.Factory(this, true, xUnits, xLabel);
            YSeries = IParameterFactory.Factory(this, false, yUnits, ylabel);
            Units = YSeries.Units;
        }
        #endregion

        #region IValidateData Methods
        //public bool Validate(IValidator<ExteriorInteriorStage> validator, out IEnumerable<IMessage> errors)
        //{
        //    //if (Function.IsValid == false) { ReportValidationErrors(); return false; }
        //    //for (int i = 0; i < Ordinates.Count; i++)
        //    //{
        //    //    if (Ordinates[i].Item1 < Ordinates[i].Item2) { ReportValidationErrors(); return false; }
        //    //}
        //    //return true;
        //    throw new NotImplementedException();

        //}
       // public override IEnumerable<string> ReportValidationErrors()
       // {
            //List<string> messages = new List<string>();
            //StringBuilder exteriorInteriorMessages = new StringBuilder();
            //for (int i = 0; i < Ordinates.Count; i++)
            //{
            //    if (Ordinates[i].Item1 < Ordinates[i].Item2)
            //    {
            //        IsValid = false;
            //        messages.Add(exteriorInteriorMessages
            //                                .AppendLine("The interior(e.g.land side) water surface elevation must less than or equal to the exterior(e.g.river side) water surface elevation.At the exterior stage ordinate: ")
            //                                .Append(Ordinates[i].Item1)
            //                                .Append(" the interior stage is listed at: ")
            //                                .Append(Ordinates[i].Item2)
            //                                .Append(", causing an error.").ToString()
            //                                );
            //    }
            //}
            //if (Function.IsValid == false)
            //{
            //    IsValid = false;
            //    messages.AddRange(Function.ReportValidationErrors());
            //}
            //return messages;
        //}
        #endregion
        //public override XElement WriteToXML()
        //{
        //    throw new NotImplementedException();
        //}

        //IMessageLevels IValidate<ExteriorInteriorStage>.Validate(IValidator<ExteriorInteriorStage> validator, out IEnumerable<IMessage> errors)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
