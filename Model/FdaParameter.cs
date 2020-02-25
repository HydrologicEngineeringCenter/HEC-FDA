using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Functions;
using Utilities;

namespace Model
{
    internal class FdaParameter : IFdaParameter
    {
        #region Fields
        private readonly Functions.IOrdinate _Parameter;
        #endregion

        #region Properties
        public IFdaParameterEnum ParameterName { get; }
        #region IOrdinate Properties
        public IOrdinateEnum Type => _Parameter.Type;
        public IRange<double> Range => _Parameter.Range;
        public IEnumerable<IMessage> Messages => throw new NotImplementedException();
        #endregion
        #endregion

        #region Constructors
        public FdaParameter(double parameter, IFdaParameterEnum parameterName = IFdaParameterEnum.NotSet)
        {
            ParameterName = ParameterName;
            _Parameter = IOrdinateFactory.Factory(parameter);
        }
        public FdaParameter(Statistics.IDistribution distribution, IFdaParameterEnum parameterName = IFdaParameterEnum.NotSet)
        {
            ParameterName = parameterName;
            _Parameter = IOrdinateFactory.Factory(distribution);
        }
        public FdaParameter(Functions.IOrdinate ordinate, IFdaParameterEnum parameterName = IFdaParameterEnum.NotSet)
        {
            ParameterName = parameterName;
            _Parameter = ordinate;
        }
        public FdaParameter(IFdaParameter parameter, IFdaParameterEnum parameterName)
        {
            ParameterName = parameterName;
            _Parameter = parameter;
        }
        #endregion

        #region Functions
        #region IOrdinate Functions
        public double Value(double p = 0.5) => _Parameter.Value();
        public bool Equals(IOrdinate ordinate) => _Parameter.Equals(ordinate);
        public string Print(bool round = false) => _Parameter.Print(round);
        #endregion
        #endregion

        public XElement WriteToXML()
        {
            throw new NotImplementedException();
        }
    }
}
