using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Statistics.Coordinates
{
    internal class ScalarCoordinateVariableX//: ICoordinate<IDistribution, double>
    {
        //TODO: Validate
        
        private readonly Func<double, double> Map;

        public IDistribution X { get; }
       // public double Y => Sample(p: 0.50).Y;

        public ScalarCoordinateVariableX(IDistribution x, Func<double, double> mapXToY)
        {
            X = x;
            Map = mapXToY;
        }

        //public ICoordinate<double, double> Sample(double p)
        //{
        //    double x = X.InverseCDF(p);
        //    return new ScalarCoordinateConstants(x, y: Map(x));
        //}
            
            
    }
}
