using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    //TODO: Comment
    //TODO: Factory Method

    public interface ICoordinate<XType, YType>
    {
        IOrdinate<XType> XOrdinate { get; }
        IOrdinate<YType> YOrdinate { get; }
    }
}
