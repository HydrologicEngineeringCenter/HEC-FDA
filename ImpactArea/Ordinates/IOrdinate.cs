using System;
using System.Collections.Generic;
using System.Text;

namespace ImpactArea
{
    public interface IOrdinate: Utilities.IValidate<IOrdinate>
    {
        double Value(double p = 0.50);
    }
}
