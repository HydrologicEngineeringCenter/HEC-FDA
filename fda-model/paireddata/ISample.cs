using System;
namespace paireddata
{
    public interface ISample
    {
        double f(double x);
        double f_inverse(double y);
    }
}