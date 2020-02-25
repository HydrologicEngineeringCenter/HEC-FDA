using Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class IElevationFactory
    {
        //TODO: The factory that combines two: ground and asset height elevations should rethought. Its not the right strategy for the sampling.
        



        #region Factory Methods
        /// <summary>
        /// The simplest generator
        /// </summary>
        /// <param name="height"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IElevation Factory(IOrdinate height, IElevationEnum type)
        {
            return new Elevations.Elevation(type, height);
        }
        /// <summary>
        /// Generates a new asset elevation formed by combining a ground and asset height elevation.
        /// </summary>
        /// <param name="ground"> The ground elevation. </param>
        /// <param name="assetHeight"> The height of the asset above the ground. </param>
        /// <returns> A new asset elevation formed by adding the ground and asset height elevations together. For distributed values the addition is done by independently sampling values from the distributions. </returns>
        public static IElevation Factory(IElevation ground, IElevation assetHeight, int sampleIterations)
        {
            /* There are a few options here. In order of attractiveness:
             *      (1) pass in a functional argument which is a sampler with its own random numbers.
             *      (2) pass in random numbers
             *      (3) pass in an random number generator
             *      (4) this approach
             * The approach here is just to get something working.
             */
             var sampler = new SampleAssetElevation().Sample(new IElevation[2] { ground, assetHeight}, )
        }
        #endregion

    }
}
