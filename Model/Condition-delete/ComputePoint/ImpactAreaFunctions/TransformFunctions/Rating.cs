using Functions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    internal sealed class Rating : ImpactAreaFunctionBase, ITransformFunction
    {
        #region Properties
        public override string XLabel => "Stage";

        public override string YLabel => "Flow";
        #endregion

        #region Constructor
        internal Rating(ICoordinatesFunction function) : base(function, ImpactAreaFunctionEnum.Rating)
        {
        }

        #endregion



        public override XElement WriteToXML()
        {
            return Function.WriteToXML();
        }


    }
}
