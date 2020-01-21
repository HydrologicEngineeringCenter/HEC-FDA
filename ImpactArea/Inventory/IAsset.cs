using System;
using System.Collections.Generic;
using System.Text;

namespace ImpactArea
{     
    public interface IAsset
    {
        IElevation Ground { get; }
        IElevation Height { get; }
        IElevation Elevation { get; }
    }
}
