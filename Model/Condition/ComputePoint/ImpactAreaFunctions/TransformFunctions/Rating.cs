using Functions;
using System;
using System.Collections.Generic;
using System.Text;

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

         

        //public IComputableTransformFunction Sample(double p)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion





    }
}
