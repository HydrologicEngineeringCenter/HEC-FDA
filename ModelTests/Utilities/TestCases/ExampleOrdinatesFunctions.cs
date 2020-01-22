using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Inputs.Functions;

namespace ModelTests.Utilities.TestCases
{
    public class ExampleOrdinatesFunctions
    {
        #region Fields
        private OrdinatesFunction _OrdinatesFunctionLowExample;
        private OrdinatesFunction _OrdinatesFunctionHighExample;
        private OrdinatesFunction _OrdinatesFunctionBearCreekRatingExample;
        private OrdinatesFunction _OrdinatesFunctionBearCreekStageDamageExample;
        #endregion

        #region Properties
        public IList<Tuple<double, double>> OrdinatesLowExample
        {
            get
            {
                return _OrdinatesFunctionLowExample.Ordinates;
            }
        }
        public IList<Tuple<double, double>> OrdinatesHighExample
        {
            get
            {
                return _OrdinatesFunctionHighExample.Ordinates;
            }
        }
        public IList<Tuple<double, double>> OrdinatesBearCreekRating
        {
            get
            {
                return _OrdinatesFunctionBearCreekRatingExample.Ordinates;
            }
        }
        public IList<Tuple<double, double>> OrdinatesBearCreekStageDamage
        {
            get
            {
                return _OrdinatesFunctionBearCreekStageDamageExample.Ordinates;
            }
        }
        public IFunctionBase LowExample
        {
            get
            {
                return _OrdinatesFunctionLowExample;
            }
        }
        public IFunctionBase HighExample
        {
            get
            {
                return _OrdinatesFunctionHighExample;
            }
        }
        public IFunctionBase BearCreekRatingExample
        {
            get
            {
                return _OrdinatesFunctionBearCreekRatingExample;
            }
        }
        public IFunctionBase BearCreekStageDamageExample
        {
            get
            {
                return _OrdinatesFunctionBearCreekStageDamageExample;
            }
        }
        #endregion

        #region Constructor
        public ExampleOrdinatesFunctions()
        {
            _OrdinatesFunctionLowExample = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 1, 2 }, new double[] { 3, 4 }, true, false));
            _OrdinatesFunctionHighExample = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { 100, 200 }, new double[] { 300, 400 }, true, false));
            _OrdinatesFunctionBearCreekRatingExample = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[] { }, new double[] { }, true, false));
        }
        #endregion
    }
}
