using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Functions;
using Functions.CoordinatesFunctions;
using Utilities;

namespace Model.Functions
{
    internal abstract class FdaFunctionBase: IFdaFunction, IValidate<IFdaFunction>
    {
        #region Field
        public readonly ICoordinatesFunction _Function;
        #endregion
        #region Properties
        //todo: delete this after the friday release
        public ICoordinatesFunction Function =>  _Function;
        public OrderedSetEnum Order => _Function.Order;
        public InterpolationEnum Interpolator => _Function.Interpolator;
        public bool IsConstant { get; }

        public abstract string Label { get; }
        public abstract IParameterRange XSeries { get; }
        public abstract IParameterRange YSeries { get; }
        public abstract IParameterEnum ParameterType { get; }

        public virtual IRange<double> Range => _Function.Range;
        public virtual IRange<double> Domain => _Function.Domain;
        public virtual List<ICoordinate> Coordinates => _Function.Coordinates;

        public bool IsLinkedFunction { get; }
        public IOrdinateEnum DistributionType { get; }

        public abstract IMessageLevels State { get; }
        public abstract IEnumerable<IMessage> Messages { get; }
        #endregion
        #region Constructors
        internal FdaFunctionBase(ICoordinatesFunction fx)
        {
            if (fx.IsNull()) throw new ArgumentNullException(nameof(fx));
            else
            {
                _Function = fx;
                IsConstant = _Function.DistributionType == IOrdinateEnum.Constant ? true : false;
                IsLinkedFunction = _Function.IsLinkedFunction;
                DistributionType = _Function.DistributionType;
            }
        }
        #endregion
        #region Functions
        public IMessageLevels Validate(IValidator<IFdaFunction> validator, out IEnumerable<IMessage> msgs)
        {
            List<IMessage> messages = new List<IMessage>(validator.ReportErrors(this));
            if (_Function.Messages != null)
            {
                messages.AddRange(_Function.Messages);
            }
            msgs = messages;
            return msgs.Max();
        }

        public virtual IOrdinate F(IOrdinate x)
        {
            if (x.Value() < _Function.Domain.Min)
            {
                return IOrdinateFactory.Factory(_Function.Range.Min);
            }
            else if (x.Value() > _Function.Domain.Max)
            {
                return IOrdinateFactory.Factory(_Function.Range.Max);
            }
            else
            {
               return _Function.F(x);
            }
        }
        public virtual IOrdinate InverseF(IOrdinate y)
        {
            if (y.Value() < _Function.Range.Min)
            {
                return IOrdinateFactory.Factory(_Function.Domain.Min);
            }
            else if (y.Value() > _Function.Range.Max)
            {
                return IOrdinateFactory.Factory(_Function.Domain.Max);
            }
            else
            {
                return _Function.InverseF(y);
            }
        }
        public bool Equals(IFdaFunction fx)
        {
            if (ParameterType == fx.ParameterType && _Function.Equals(fx)) return true;
            else return false;
        }

        public string Print(bool round = false, bool abbreviate = false) => $"{ParameterType.Print(abbreviate)}(domain: {Domain.Print(round)} {XSeries.Units.Print(abbreviate)}, range: {Range.Print(round)} {YSeries.Units.Print(abbreviate)})"; //Maybe add distribution type.
        public virtual XElement WriteToXML() => _Function.WriteToXML();
        #endregion
    }
}
