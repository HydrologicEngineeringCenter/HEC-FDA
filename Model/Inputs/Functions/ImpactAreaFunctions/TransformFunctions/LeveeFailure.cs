using Model.Inputs.Functions;
using Model.Inputs.Functions.ImpactAreaFunctions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Utilities;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    internal sealed class LeveeFailure : ImpactAreaFunctionBase, ITransformFunction, IValidate<LeveeFailure>
    {
        public override string XLabel => throw new NotImplementedException();

        public override string YLabel => throw new NotImplementedException();

        public bool IsValid => throw new NotImplementedException();

        public IEnumerable<IMessage> Messages => throw new NotImplementedException();

        public IMessageLevels State => throw new NotImplementedException();

        internal LeveeFailure(Functions.ICoordinatesFunction function):base(function, ImpactAreaFunctionEnum.LeveeFailure)
        { 
        }

        public bool Validate(IValidator<LeveeFailure> validator, out IEnumerable<IMessage> errors)
        {
            throw new NotImplementedException();
        }

        public override XElement WriteToXML()
        {
            throw new NotImplementedException();
        }

        IMessageLevels IValidate<LeveeFailure>.Validate(IValidator<LeveeFailure> validator, out IEnumerable<IMessage> errors)
        {
            throw new NotImplementedException();
        }
    }
}
