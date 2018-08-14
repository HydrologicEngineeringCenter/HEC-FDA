using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Study
{
    public enum MonetaryUnitsEnum : byte
    {
        #region Notes
        #endregion
        Dollars = 0x01,
        Thousands = 0x02,
        Millions = 0x04,
        Billions = 0x08,
    }
}
