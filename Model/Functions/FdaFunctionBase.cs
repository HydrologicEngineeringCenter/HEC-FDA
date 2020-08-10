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
        internal readonly IFunction _Function;
        #endregion
        #region Properties
        public OrderedSetEnum Order => _Function.Order;
        public IRange<double> Range => _Function.Range;
        public IRange<double> Domain => _Function.Domain;
        public InterpolationEnum Interpolator => _Function.Interpolator;
        public bool IsConstant { get; }

        public abstract string Label { get; }
        public abstract IParameterRange XSeries { get; }
        public abstract IParameterRange YSeries { get; }
        public abstract IParameterEnum ParameterType { get; }

        public List<ICoordinate> Coordinates => _Function.Coordinates;

        public bool IsLinkedFunction { get; }
        public IOrdinateEnum DistributionType { get; }

        public abstract IMessageLevels State { get; }
        public abstract IEnumerable<IMessage> Messages { get; }
        #endregion
        #region Constructors
        internal FdaFunctionBase(IFunction fx)
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
            messages.AddRange(_Function.Messages);
            msgs = messages;
            return msgs.Max();
        }

        public virtual IOrdinate F(IOrdinate x) => _Function.F(x); 
        public virtual IOrdinate InverseF(IOrdinate y) => _Function.InverseF(y);
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
