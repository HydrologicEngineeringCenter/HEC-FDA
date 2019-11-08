using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Ordinates
{
    public class Constant : IOrdinate
    {
        private double _ConstantValue;
        public Constant(double value)
        {
            _ConstantValue = value;
        }

        public Tuple<double, double> Range
        {
            get { return new Tuple<double, double>(_ConstantValue, _ConstantValue); }
        }

        public bool IsDistributed => false;

        public bool Equals(IOrdinate constant)
        {
            return constant.Print().Equals(this.Print());
        }

        public string Print()
        {
            return $"Double(value: {_ConstantValue}";
        }

        public double Value(double p = 0.5)
        {
            return _ConstantValue;
        }
    }
}
