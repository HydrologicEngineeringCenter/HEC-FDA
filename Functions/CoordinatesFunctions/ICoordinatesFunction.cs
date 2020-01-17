using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Utilities;
using Utilities.Serialization;

namespace Functions
{
    public interface ICoordinatesFunction : ISerializeToXML<ICoordinatesFunction>, IValidate<ICoordinatesFunction>
    {
        IOrdinate F(IOrdinate x);
        IOrdinate InverseF(IOrdinate y);
        List<ICoordinate> Coordinates { get; }
        OrderedSetEnum Order { get; }
        InterpolationEnum Interpolator { get; }

        //Tuple<double, double> Domain { get; }
        Utilities.IRange<double> Domain { get; }

        bool Equals(ICoordinatesFunction function);

        Functions.DistributionType DistributionType { get; }

        bool IsLinkedFunction { get; }
        //bool IsDistributed { get; }

        //IFunction Sample(double p);
        //IFunction Sample(double p, InterpolationEnum interpolator);

    }
}
