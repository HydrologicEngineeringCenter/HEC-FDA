using System;
using System.Text;
using System.Collections.Generic;
using Utilities;
using System.Xml.Linq;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    internal sealed class ExteriorInteriorStage: ImpactAreaFunctionBase, ITransformFunction, IValidate<ExteriorInteriorStage>
    {
        #region Properties
        public override string XLabel => "Exterior Stage";

        public override string YLabel => "Interior Stage";

        public bool IsValid => throw new NotImplementedException();

        public IEnumerable<IMessage> Errors => throw new NotImplementedException();
        #endregion

        #region Constructor
        internal ExteriorInteriorStage(Functions.ICoordinatesFunction function) : base(function, ImpactAreaFunctionEnum.ExteriorInteriorStage)
        {

        }

        public bool Validate(IValidator<ExteriorInteriorStage> validator, out IEnumerable<IMessage> errors)
        {
            throw new NotImplementedException();
        }

        

        public override XElement WriteToXML()
        {
            throw new NotImplementedException();
        }

        //public IComputableTransformFunction Sample(double p)
        //{
        //    return new ExteriorInteriorStageComputable(Function.Sample(p));
        //}
        #endregion

        //#region IFunctionTransform Methods
        //public ITransformFunction Sample(double probability)
        //{
        //    return ImpactAreaFunctionFactory.CreateNew(Function.Sample(probability), Ordinates, Type);
        //}
        //#endregion

        //#region IValidateData Methods
        //public override bool Validate()
        //{
        //    if (Function.IsValid == false) { ReportValidationErrors(); return false; }
        //    for (int i = 0; i < Ordinates.Count; i++)
        //    {
        //        if (Ordinates[i].Item1 < Ordinates[i].Item2) { ReportValidationErrors(); return false; }
        //    }
        //    return true;
        //}
        //public override IEnumerable<string> ReportValidationErrors()
        //{
        //    List<string> messages = new List<string>();
        //    StringBuilder exteriorInteriorMessages = new StringBuilder();
        //    for (int i = 0; i < Ordinates.Count; i++)
        //    {
        //        if (Ordinates[i].Item1 < Ordinates[i].Item2)
        //        {
        //            IsValid = false;
        //            messages.Add(exteriorInteriorMessages
        //                                    .AppendLine("The interior(e.g.land side) water surface elevation must less than or equal to the exterior(e.g.river side) water surface elevation.At the exterior stage ordinate: ")
        //                                    .Append(Ordinates[i].Item1)
        //                                    .Append(" the interior stage is listed at: ")
        //                                    .Append(Ordinates[i].Item2)
        //                                    .Append(", causing an error.").ToString()
        //                                    );
        //        }
        //    }
        //    if (Function.IsValid == false)
        //    {
        //        IsValid = false;
        //        messages.AddRange(Function.ReportValidationErrors());
        //    }
        //    return messages;
        //}
        //#endregion
    }
}
