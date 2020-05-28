using System;
using System.Text;
using System.Collections.Generic;
using Utilities;
using Functions;
using System.Xml.Linq;

namespace Model.Inputs.Functions.ImpactAreaFunctions
{

    internal sealed class ExteriorInteriorStage: ImpactAreaFunctionBase, ITransformFunction, IValidate<ExteriorInteriorStage>
    {
        #region Properties
        public override string XLabel => "Exterior Stage";

        public override string YLabel => "Interior Stage";

        public bool IsValid => throw new NotImplementedException();

        public IEnumerable<IMessage> Errors => throw new NotImplementedException();

        public IEnumerable<IMessage> Messages => throw new NotImplementedException();

        public IMessageLevels State => throw new NotImplementedException();
        #endregion

        #region Constructor
        internal ExteriorInteriorStage(ICoordinatesFunction function): base(function, ImpactAreaFunctionEnum.ExteriorInteriorStage)
        {
            
        }
        #endregion

        #region IValidateData Methods
        public bool Validate(IValidator<ExteriorInteriorStage> validator, out IEnumerable<IMessage> errors)
        {
            //if (Function.IsValid == false) { ReportValidationErrors(); return false; }
            //for (int i = 0; i < Ordinates.Count; i++)
            //{
            //    if (Ordinates[i].Item1 < Ordinates[i].Item2) { ReportValidationErrors(); return false; }
            //}
            //return true;
            throw new NotImplementedException();

        }
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

        public override XElement WriteToXML()
        {
            return Function.WriteToXML();
        }

        IMessageLevels IValidate<ExteriorInteriorStage>.Validate(IValidator<ExteriorInteriorStage> validator, out IEnumerable<IMessage> errors)
        {
            throw new NotImplementedException();
        }
    }
}
