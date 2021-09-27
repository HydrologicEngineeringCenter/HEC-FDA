using System;
namespace paireddata
{
    public interface IComposable
    {
        IPairedData compose(IPairedData g);
    }
}