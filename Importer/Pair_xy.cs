using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importer
{
    [Serializable]
    public class Pair_xy
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        protected double _X;
        protected double _Y;
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public Pair_xy()
        {
            _X = Study.badNumber;
            _Y = Study.badNumber;
        }
        public Pair_xy(double x, double y)
        {
            _X = x;
            _Y = y;
        }
        #endregion
        #region Voids
        public void SetX(double x) => _X = x;
        public void SetY(double y) => _Y = y;
        #endregion
        #region Functions
        public double GetX() => _X;
        public double GetY() => _Y;
        #endregion
    }
}
