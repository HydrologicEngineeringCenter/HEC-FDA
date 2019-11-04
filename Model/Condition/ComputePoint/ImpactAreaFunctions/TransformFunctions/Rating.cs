using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    internal sealed class Rating<YType> : ImpactAreaFunctionBase<YType>
    {
        #region Properties
        public override string XLabel => "Stage";

        public override string YLabel => "Flow";
        #endregion

        #region Constructor
        internal Rating(Functions.ICoordinatesFunction<double,YType> function) : base(function, ImpactAreaFunctionEnum.Rating)
        {
        }

        //public IComputableTransformFunction Sample(double p)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion





    }
}
