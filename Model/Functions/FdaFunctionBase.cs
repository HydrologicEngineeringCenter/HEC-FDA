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
        public InterpolationEnum Interpolator => _Function.Interpolator;
        public bool IsConstant { get; }

        public abstract string Label { get; }
        public abstract UnitsEnum Units { get; }
        public abstract IParameter XSeries { get; }
        public abstract IParameter YSeries { get; }
        public abstract IParameterEnum ParameterType { get; }

        public List<ICoordinate> Coordinates => _Function.Coordinates;

        public bool IsLinkedFunction { get; }
        public IOrdinateEnum DistributionType { get; }

        internal FdaFunctionBase(IFunction fx)
        {
            _Function = fx;
            IsConstant = _Function.DistributionType == IOrdinateEnum.Constant ? true : false;
            //Interpolator = _Function.Interpolator;
            IsLinkedFunction = _Function.IsLinkedFunction;
            DistributionType = _Function.DistributionType;
        }

        public virtual IOrdinate F(IOrdinate x) => _Function.F(x); 
        public virtual IOrdinate InverseF(IOrdinate y) => _Function.InverseF(y);
        public bool Equals(IFdaFunction fx)
        {
            if (ParameterType == fx.ParameterType && _Function.Equals(fx)) return true;
            else return false;
        }
        public string Print(bool round = false, bool abbreviate = false) => throw new NotImplementedException();
        public string PrintValue(bool round = false, bool abbreviate = false) => throw new NotImplementedException();
        public virtual XElement WriteToXML() => _Function.WriteToXML();
    }
}
