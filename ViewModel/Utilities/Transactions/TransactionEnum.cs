using System;

namespace HEC.FDA.ViewModel.Utilities.Transactions
{
    public enum TransactionEnum : Byte
    {
        CreateNew,
        EditExisting,
        Delete,
        Rename
    }
}
