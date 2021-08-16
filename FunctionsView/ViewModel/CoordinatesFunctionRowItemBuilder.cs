using Functions;
using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsView.ViewModel
{
    public class CoordinatesFunctionRowItemBuilder
    {
        private double _X;
        private double _Y;
        private IOrdinateEnum _distType;
        private InterpolationEnum _interpType;

        private double _alpha;
        private double _beta;
        private double _standDev;
        private double _min;
        private double _max;
        private double _mostLikely;
        private double _mean;

        private bool _isReadOnly;

        public CoordinatesFunctionRowItemBuilder(double x, bool isReadOnly)
        {
            _X = x;
            _distType = IOrdinateEnum.NotSupported;
            _isReadOnly = isReadOnly;
        }
        public CoordinatesFunctionRowItemBuilder WithTriangularDist(double mostLikely, double min, double max, InterpolationEnum interpolator)
        {
            _mostLikely = mostLikely;
            _min = min;
            _max = max;
            _distType = IOrdinateEnum.Triangular;
            _interpType = interpolator;
            return this;
        }
        public CoordinatesFunctionRowItemBuilder WithNormalDist(double mean, double standardDeviation, InterpolationEnum interpolator)
        {
            _distType = IOrdinateEnum.Normal;
            _mean = mean;
            _standDev = standardDeviation;
            _interpType = interpolator;
            return this;
        }

        public CoordinatesFunctionRowItemBuilder WithConstantDist(double y, InterpolationEnum interpolator)
        {
            _Y = y;
            _distType = IOrdinateEnum.Constant;
            _interpType = interpolator;
            return this;
        }

        public CoordinatesFunctionRowItemBuilder WithUniformDist(double min, double max, InterpolationEnum interpolator)
        {
            _min = min;
            _max = max;
            _distType = IOrdinateEnum.Uniform;
            _interpType = interpolator;
            return this;
        }
        public CoordinatesFunctionRowItemBuilder WithTruncatedNormalDist(double mean, double stDev, double min, double max, InterpolationEnum interpolator)
        {
            _distType = IOrdinateEnum.TruncatedNormal;
            _mean = mean;
            _standDev = stDev;
            _min = min;
            _max = max;
            _interpType = interpolator;
            return this;
        }

        public CoordinatesFunctionRowItemBuilder WithBetaDist(double alpha, double beta, double min, double max, InterpolationEnum interpolator)
        {
            _distType = IOrdinateEnum.Beta4Parameters;
            _alpha = alpha;
            _beta = beta;
            _min = min;
            _max = max;
            _interpType = interpolator;
            return this;
        }

        public CoordinatesFunctionRowItem Build()
        {
            return new CoordinatesFunctionRowItem(_X, _Y, _standDev, _mean, _min, _max, _mostLikely,_alpha, _beta, _distType, _interpType, _isReadOnly);
        }
    }
}
