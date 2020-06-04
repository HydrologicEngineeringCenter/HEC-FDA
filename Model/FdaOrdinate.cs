using System.Collections.Generic;
using Functions;
using Utilities;

namespace Model
{
    internal class FdaOrdinate: IFdaOrdinate
    {
        #region Fields
        internal readonly IOrdinate _Ordinate;
        #endregion
        #region Properties
        public IRange<double> Range => _Ordinate.Range;
        public IOrdinateEnum Type => _Ordinate.Type;
        public IMessageLevels State => _Ordinate.State;
        public IEnumerable<IMessage> Messages => _Ordinate.Messages;
        #endregion
        #region Constructor
        public FdaOrdinate(IOrdinate ordinate)
        {
            if (ordinate.IsNull()) throw new InvalidConstructorArgumentsException("The specified ordinate cannot be constructed because it is null.");
            _Ordinate = ordinate;
        }
        #endregion

        #region Functions
        public double Value(double p = 0.5) => _Ordinate.Value(p);
        public bool Equals(IFdaOrdinate ordinate) => string.Equals(_Ordinate.Print(), ordinate.Print());
        public string Print(bool round = false) => _Ordinate.Print(round);
        #endregion
    }
}
