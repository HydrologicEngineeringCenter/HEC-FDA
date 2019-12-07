using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    public static class IDataFactory
    {
        public static IData Factory(IEnumerable<double> data) => new Data(data);
    }
}
