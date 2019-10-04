using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities.Transactions
{
    public enum TransactionEnum : Byte
    {
        CreateNew,
        EditExisting,
        Delete,
        Rename
    }
}
