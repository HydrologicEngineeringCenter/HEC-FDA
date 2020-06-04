using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Functions;
using Functions.CoordinatesFunctions;
using Utilities;

namespace Model.Functions
{
    internal abstract class FdaFunctionBase: IFdaFunction
    {
        internal readonly IFunction _Function;

        public OrderedSetEnum Order => _Function.Order;
        public IRange<double> Range => _Function.Range;
        public IRange<double> Domain => _Function.Domain;

        public abstract IParameterSeries XSeries { get; }
        public abstract IParameterSeries YSeries { get; }
        public abstract IParameterEnum ParameterType { get; }

        public List<ICoordinate> Coordinates => _Function.Coordinates;

        internal FdaFunctionBase(IFunction fx)
        {
            _Function = fx;
        }

        public virtual IOrdinate F(IOrdinate x) => _Function.F(x); 
        public virtual IOrdinate InverseF(IOrdinate y) => _Function.InverseF(y);
        public bool Equals(IFdaFunction fx)
        {
            if (ParameterType == fx.ParameterType && _Function.Equals(fx)) return true;
            else return false;
        }
        public XElement WriteToXML() => throw new NotImplementedException();
    }
}
