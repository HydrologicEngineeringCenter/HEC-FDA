using System;
namespace paireddata
{
    public interface IComposable
    {
        PairedData compose(PairedData g);
    }
}