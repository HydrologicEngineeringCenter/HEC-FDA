namespace HEC.FDA.Model.paireddata
{
    public interface ISample
    {
        double f(double x);
        double f_inverse(double y);
        double f(double x, ref int index);
    }
}