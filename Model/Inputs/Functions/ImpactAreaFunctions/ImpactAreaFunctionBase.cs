using Functions;
using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Model.Inputs.Functions.ImpactAreaFunctions
{
    /// <summary>
    /// All impact area functions inherit from this abstract class. 
    /// It contains an IFunctionBase property that allows it to serve as the base implementation for the decorator pattern that is implemented by inherited classes. 
    /// </summary>
    internal abstract class ImpactAreaFunctionBase: IFdaFunction 
    {
        #region Properties              
        /// <summary>
        /// An ordinates, uncertain ordinates or frequency function that allows for inherited members to implement the decorator pattern - dynamically extended the IFunctionBase capabilities.
        /// </summary>
        public ICoordinatesFunction Function { get; }
        public ImpactAreaFunctionEnum Type { get; }
        public abstract string XLabel { get; }

        public abstract string YLabel { get; }
        public Tuple<double, double> Domain => throw new NotImplementedException();
       // public bool IsValid { get; protected set; }


        //todo: Why not just get rid of these three props that just call props on the Function which is public.
        //public List<ICoordinate> Coordinates => Function.Coordinates;

        //public InterpolationEnum Interpolator => Function.Interpolator;

        //public OrderedSetEnum Order => Function.Order;
        #endregion

        #region Constructor
        protected ImpactAreaFunctionBase(ICoordinatesFunction function, ImpactAreaFunctionEnum type) 
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
        public IOrdinate F(IOrdinate x) => Function.F(x);

        public IOrdinate InverseF(IOrdinate y) => Function.InverseF(y);


        public abstract XElement WriteToXML();

        public bool Equals(IFdaFunction function)
        {
            if (this.Type != function.Type)
            {
                return false;
            }
            return this.Function.Equals(function.Function);
        }

        #endregion

        //#region IValidateData Methods
        //public virtual bool Validate()
        //{
        //    if (Function.IsValid == false) ReportValidationErrors();
        //    return Function.IsValid;
        //}
        //public virtual IEnumerable<string> ReportValidationErrors() { return Function.ReportValidationErrors(); }
        //#endregion
    }
}