using System;
using System.Numerics;

namespace Statistics;
public static class Mathematics
{
    /// <summary>
    ///Calcualtes the area under the curve across the range of x values using trapizoidal integration. 
    ///Assumes X an Y vals are increasing from 0. Assumes an additional x ord of 1, and y ordinate equal to the last one in the array.
    ///This works because the only thing we're integrating is CDFs of Consequence Frequency, which are always between 0 and 1.
    /// </summary>
    public static double IntegrateTrapezoidal(double[] xVals, double[] yVals)
    {
        double triangle;
        double square;
        double x1 = 0.0;
        double y1 = 0.0;
        double area = 0.0;
        for (int i = 0; i < xVals.Length; i++)
        {
            double xdelta = xVals[i] - x1;
            square = xdelta * y1;
            triangle = xdelta * (yVals[i] - y1) / 2.0;
            area += square + triangle;
            x1 = xVals[i];
            y1 = yVals[i];
        }
        if (x1 != 1)
        {
            double xdelta = 1 - x1;
            area += xdelta * y1;
        }
        return area;
    }

    /// <summary>
    ///Calcualtes the area under the curve across the range of x values using trapizoidal integration. 
    ///Assumes X an Y vals are increasing from 0. Assumes an additional x ord of 1, and y ordinate equal to the last one in the array.
    ///This works because the only thing we're integrating is CDFs of Consequence Frequency, which are always between 0 and 1.
    /// </summary>
    public static T IntegrateCDF<T>(Span<T> xVals, Span<T> yVals) where T : IBinaryFloatingPointIeee754<T>
    {
        T paddedArea = T.Zero;
        //Between 0 and x1 (Triangle)
        if (xVals[0] > T.Zero)
        {
            paddedArea += (xVals[0] * yVals[0]) / T.CreateTruncating(2.0);
        }
        //Between xN and 1 (Rectangle)
        if (xVals[^1] < T.One)
        {
            paddedArea += (T.One - xVals[^1]) * yVals[^1];
        }
        return paddedArea + RealIntegrateTrapezoidal(xVals, yVals);
    }

    /// <summary>
    /// IBinaryFloatingPointIeee754<T> means float and double
    /// Spans work arrays
    /// </summary>
    public static T RealIntegrateTrapezoidal<T>(Span<T> xVals, Span<T> yVals) where T : IBinaryFloatingPointIeee754<T>
    {
        if (xVals.Length != yVals.Length)
            throw new ArgumentException("xVals and yVals must have the same length");

        T area = T.Zero;
        for (int i = 0; i < xVals.Length - 1; i++)
        {
            T dx = xVals[i + 1] - xVals[i];
            area += (yVals[i] + yVals[i + 1]) / T.CreateTruncating(2.0) * dx;
        }
        return area;
    }
}
