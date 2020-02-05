using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbMemory
{
    [Serializable]
    public class WspSectionData : FdObjectDataLook
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        //Contains data for one cross-section
        #endregion
        #region Fields
        private int _NumProfiles = 0;
        private double[] _Stage = null;
        private double[] _Flow = null;
        private double _Invert;
        private double _Station;
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public WspSectionData(int numProfiles)
        {
            this._NumProfiles = numProfiles;
            _Stage = new double[numProfiles];
            _Flow = new double[numProfiles];
            for (int i = 0; i < numProfiles; i++)
                _Stage[i] = _Flow[i] = Study.badNumber;
        }
        #endregion
        #region Voids
        public void SetStation(double station)
        {
            this._Station = station;
        }
        public void SetInvert(double invert)
        {
            this._Invert = invert;
        }
        public void SetPoint(double station, double invert)
        {
            this._Station = station;
            this._Invert = invert;
        }
        public void SetPoint(int ixProfile, double stage, double flow)
        {
            this._Stage[ixProfile] = stage;
            this._Flow[ixProfile] = flow;
        }
        #endregion
        #region Functions
        public int GetNumProfiles()
        { return _NumProfiles; }
        public double GetStation()
        { return _Station; }
        public double GetInvert()
        { return _Invert; }
        public double GetPointStage(int ixProfile)
        { return _Stage[ixProfile]; }
        public double GetPointFlow(int ixProfile)
        { return _Flow[ixProfile]; }
        #endregion
    }
}
