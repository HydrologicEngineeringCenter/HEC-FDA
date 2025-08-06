namespace HEC.FDA.Statistics;
public static class Mathematics
{
    /// <summary>
    ///Calcualtes the area under the curve across the range of x values using trapizoidal integration. 
    ///Assumes X an Y vals are increasing from 0. Assumes an additional x ord of 1, and y ordinate equal to the last one in the array.
    /// </summary>
    public static double IntegrateTrapezoidal(double[] xVals, double[] yVals)
    {
        double triangle;
        double square;
        double x1 = 0.0;
        double y1 = 0.0;
        double ead = 0.0;
        for (int i = 0; i < xVals.Length; i++)
        {
            double xdelta = xVals[i] - x1;
            square = xdelta * y1;
            triangle = xdelta * (yVals[i] - y1) / 2.0;
            ead += square + triangle;
            x1 = xVals[i];
            y1 = yVals[i];
        }
        if (x1 != 1)
        {
            double xdelta = 1 - x1;
            ead += xdelta * y1;
        }
        return ead;
    }
}
