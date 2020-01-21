using System.Xml.Linq;
using Functions;

namespace Model.Inputs.Functions.ImpactAreaFunctions
{
    internal sealed class UnUsed : ImpactAreaFunctionBase
    {
        #region Fields and Properties
        public ImpactAreaFunctionEnum TargetType { get; }

        public override string XLabel => throw new System.NotImplementedException();

        public override string YLabel => throw new System.NotImplementedException();
        #endregion

        #region Constructors
        internal UnUsed(ICoordinatesFunction function): base(function, ImpactAreaFunctionEnum.NotSet )
        {
            
        }

        public override XElement WriteToXML()
        {
            throw new System.NotImplementedException();
        }
        //internal UnUsed(IFunctionBase function, ImpactAreaFunctionEnum type): base(function)
        //{
        //    TargetType = type;
        //    Type = ImpactAreaFunctionEnum.NotSet;
        //    IsValid = Function.IsValid;
        //}
        #endregion

        #region BaseImplementation Methods

        #endregion
    }
}
