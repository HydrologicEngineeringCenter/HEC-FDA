using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaTester.ModelTester.UnitTests
{
    class FortranCSharpLogP3ComparerObject
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 8/16/2016 4:25:21 PM
        #endregion
        #region Fields
        double _mean;
        double _stDev;
        double _skew;
        double _por;
        bool _testStatus;

        double[] _probabilities;
        double[] _cSharpFlowValues;
        double[] _fortranFlowValues;
        double[] _absDiff;
        double[] _percDiff;
        #endregion



        #region Properties
        public bool testStatus { get; set; }
        public double mean
        {
            get { return _mean; }
            set { _mean = value; }
        }
        public double stDev
        {
            get { return _stDev; }
            set { _stDev = value; }
        }
        public double skew
        {
            get { return _skew; }
            set { _skew = value; }
        }
        public double por
        {
            get { return _por; }
            set { _por = value; }
        }
        public double[] probabilities
        {
            get { return _probabilities; }
            set { _probabilities = value; }
        }
        public double[] cSharpFlowValues
        {
            get { return _cSharpFlowValues; }
            set { _cSharpFlowValues = value; }
        }
        public double[] fortranFlowValues
        {
            get { return _fortranFlowValues; }
            set { _fortranFlowValues = value; }
        }
        public double[] absDiff
        {
            get { return _absDiff; }
            set { _absDiff = value; }
        }
        public double[] percDiff
        {
            get { return _percDiff; }
            set { _percDiff = value; }
        }


        #endregion
        #region Constructors
        public FortranCSharpLogP3ComparerObject()
        {

        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
    }
}
