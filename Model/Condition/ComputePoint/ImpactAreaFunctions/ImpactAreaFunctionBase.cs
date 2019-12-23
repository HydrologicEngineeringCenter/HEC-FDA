using Functions;
using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    /// <summary>
    /// All impact area functions inherit from this abstract class. 
    /// It contains an IFunctionBase property that allows it to serve as the base implementation for the decorator pattern that is implemented by inherited classes. 
    /// </summary>
    internal abstract class ImpactAreaFunctionBase:IFdaFunction
    {
        #region Properties 
        
        public ICoordinatesFunction Function { get; }
        public ImpactAreaFunctionEnum Type { get; }

        public abstract string XLabel { get; }

        public abstract string YLabel { get; }

        public List<ICoordinate> Coordinates => Function.Coordinates;

        public InterpolationEnum Interpolator => Function.Interpolator;

        public OrderedSetEnum Order => Function.Order;

        public Tuple<double, double> Domain => throw new NotImplementedException();
        #endregion

        #region Constructor
        protected ImpactAreaFunctionBase(ICoordinatesFunction function, ImpactAreaFunctionEnum type) 
        {
            //have to convert the const and the dist funcs to ordinate ys
            //todo: put try catch around this?
            Function = function;// ICoordinatesFunctionsFactory.Factory(function);
                //Function = (ICoordinatesFunction<Constant, IOrdinate>)function;
            

            Type = type;
        }
        #endregion


        #region Methods
        //protected IList<Tuple<double, double>> Compose(List<Tuple<double, double>> transformOrdiantes)
        //{
        //    return Function.Compose(transformOrdiantes);
        //}

        //public abstract IFdaFunction< Sample(double p);// => Function.Sample(p);

        public IOrdinate F(IOrdinate x) => Function.F(x);
        
        public IOrdinate InverseF(IOrdinate y) => Function.InverseF(y);
       

        public abstract XElement WriteToXML();

        public bool Equals(IFdaFunction function)
        {
            if(this.Type != function.Type)
            {
                return false;
            }
            return this.Function.Equals(function.Function);
        }


        #endregion


    }
}