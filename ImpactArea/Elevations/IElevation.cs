using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace ImpactArea
{
    public interface IElevation: IMessagePublisher
    {
        IOrdinate Height { get; }
        ElevationEnum Type { get; }
    }
}
