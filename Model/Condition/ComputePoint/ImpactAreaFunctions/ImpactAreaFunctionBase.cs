using Functions;
using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    /// <summary>
    /// All impact area functions inherit from this abstract class. 
    /// It contains an IFunctionBase property that allows it to serve as the base implementation for the decorator pattern that is implemented by inherited classes. 
    /// </summary>
    internal abstract class ImpactAreaFunctionBase<YType>:IFdaFunction<YType> 
    {
        #region Properties 
        
        protected ICoordinatesFunction<double,YType> Function;
        public ImpactAreaFunctionEnum Type { get; }

        public abstract string XLabel { get; }

        public abstract string YLabel { get; }

        public List<ICoordinate<double, YType>> Coordinates => Function.Coordinates;

        public InterpolationEnum Interpolator => Function.Interpolator;

        public OrderedSetEnum Order => Function.Order;

        public Tuple<double, double> Domain => throw new NotImplementedException();
        #endregion

        #region Constructor
        protected ImpactAreaFunctionBase(ICoordinatesFunction<double,YType> function, ImpactAreaFunctionEnum type) 
        { 
            Function = function;
            Type = type;
        }
        #endregion


        #region Methods
        //protected IList<Tuple<double, double>> Compose(List<Tuple<double, double>> transformOrdiantes)
        //{
        //    return Function.Compose(transformOrdiantes);
        //}

        //public abstract IFdaFunction< Sample(double p);// => Function.Sample(p);

        public YType F(double x) => Function.F(x);
        
        public double InverseF(YType y) => Function.InverseF(y);
      
        #endregion


    }
}