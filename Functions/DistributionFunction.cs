using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Functions.CoordinatesFunctions;
using Utilities;

namespace Functions
{
    internal class DistributionFunction : IFunction, IValidate<DistributionFunction>
    {
        private readonly IDistributedValue _Distribution;
        
        public bool IsInvertible => throw new NotImplementedException();
        public IRange<double> Range => throw new NotImplementedException();
        public List<ICoordinate> Coordinates => throw new NotImplementedException();
        public OrderedSetEnum Order => throw new NotImplementedException();
        public InterpolationEnum Interpolator => throw new NotImplementedException();
        public IRange<double> Domain => throw new NotImplementedException();
        public IEnumerable<IMessage> Messages { get; }

        public DistributionType DistributionType => throw new NotImplementedException();

        public bool IsLinkedFunction => throw new NotImplementedException();

        public bool IsValid => throw new NotImplementedException();

        internal DistributionFunction(IDistributedValue distribution)
        {
            _Distribution = distribution;
            //Messages = _Distribution.
        }

        public IFunction Compose(IFunction g)
        {
            throw new NotImplementedException();
        }
        public bool Equals(ICoordinatesFunction function)
        {
            throw new NotImplementedException();
        }
        public IOrdinate F(IOrdinate x)
        {
            throw new NotImplementedException();
        }
        public IOrdinate InverseF(IOrdinate y)
        {
            throw new NotImplementedException();
        }
        public double RiemannSum()
        {
            throw new NotImplementedException();
        }
        public XElement WriteToXML()
        {
            throw new NotImplementedException();
        }

        public bool Validate(IValidator<ICoordinatesFunction> validator, out IEnumerable<IMessage> errors)
        {
            throw new NotImplementedException();
        }

        public bool Validate(IValidator<DistributionFunction> validator, out IEnumerable<IMessage> errors)
        {
            throw new NotImplementedException();
        }
    }
}
