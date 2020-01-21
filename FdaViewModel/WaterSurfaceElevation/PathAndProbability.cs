using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace FdaViewModel.WaterSurfaceElevation
{
    //[Author(q0heccdm, 9 / 11 / 2017 8:48:18 AM)]
    public class PathAndProbability
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 9/11/2017 8:48:18 AM
        #endregion
        #region Fields
        #endregion
        #region Properties
        public string Path { get; set; }
        public double Probability { get; set;}
        #endregion
        #region Constructors
        public PathAndProbability(string path, double probability)
        {
            Path = path;
            Probability = probability;
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
    }
}
