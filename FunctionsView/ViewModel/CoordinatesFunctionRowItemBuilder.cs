using Functions;
using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsView.ViewModel
{
    class CoordinatesFunctionRowItemBuilder
    {
        private double _X;
        private double _Y;
        private DistributionType _distType;
        private InterpolationEnum _interpType;

        private double _alpha;
        private double _beta;
        private double _standDev;
        private double _min;
        private double _max;
        private double _mostLikely;
        private double _mean;

        internal CoordinatesFunctionRowItemBuilder(double x)
        {
            _X = x;
            _distType = DistributionType.NotSupported;
        }
        internal CoordinatesFunctionRowItemBuilder WithTriangularDist(double mostLikely, double min, double max, InterpolationEnum interpolator)
        {
            _mostLikely = mostLikely;
            _min = min;
            _max = max;
            _distType = DistributionType.Triangular;
            _interpType = interpolator;
            return this;
        }
        internal CoordinatesFunctionRowItemBuilder WithNormalDist(double mean, double standardDeviation, InterpolationEnum interpolator)
        {
            _distType = DistributionType.Normal;
            _mean = mean;
            _standDev = standardDeviation;
            _interpType = interpolator;
            return this;
        }

        internal CoordinatesFunctionRowItemBuilder WithConstantDist(double y, InterpolationEnum interpolator)
        {
            _Y = y;
            _distType = DistributionType.Constant;
            _interpType = interpolator;
            return this;
        }

        internal CoordinatesFunctionRowItemBuilder WithUniformDist(double min, double max, InterpolationEnum interpolator)
        {
            _min = min;
            _max = max;
            _distType = DistributionType.Uniform;
            _interpType = interpolator;
            return this;
        }
        internal CoordinatesFunctionRowItemBuilder WithTruncatedNormalDist(double mean, double stDev, double min, double max, InterpolationEnum interpolator)
        {
            _distType = DistributionType.TruncatedNormal;
            _mean = mean;
            _standDev = stDev;
            _min = min;
            _max = max;
            _interpType = interpolator;
            return this;
        }

        internal CoordinatesFunctionRowItemBuilder WithBetaDist(double alpha, double beta, double min, double max, InterpolationEnum interpolator)
        {
            _distType = DistributionType.Beta4Parameters;
            _alpha = alpha;
            _beta = beta;
            _min = min;
            _max = max;
            _interpType = interpolator;
            return this;
        }

        internal CoordinatesFunctionRowItem Build()
        {
            return new CoordinatesFunctionRowItem(_X, _Y, _standDev, _mean, _min, _max, _mostLikely,_alpha, _beta, _distType, _interpType);
        }
    }
}
