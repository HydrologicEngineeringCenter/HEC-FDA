using System;
namespace paireddata
{
    public interface IComposable
    {
        paired_data compose(paired_data g);
    }
}