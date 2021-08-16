using Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// A factory for the creation of <see cref="IParameterOrdinate"/> parameters.
    /// </summary>
    public class IElevationFactory
    {
        /// <summary>
        /// Creates an <see cref="IParameterOrdinate"/> parameter with a <see cref="IOrdinateEnum.Constant"/> or invariant value.
        /// </summary>
        /// <param name="elevation"> The height of the elevation. </param>
        /// <param name="units"> The unit of measurement for the elevation. </param>
        /// <param name="elevationEnum"> The type of elevation, <see cref="IParameterEnum"/>. </param>
        /// <returns> An invariant <see cref="IParameterOrdinate"/> parameter. </returns>
        public static IParameterOrdinate Factory(double elevation, UnitsEnum units = UnitsEnum.Foot, IParameterEnum elevationEnum = IParameterEnum.GroundElevation, string label = "", bool abbreviatedLabel = true)
        {
            IOrdinate ordinate = IOrdinateFactory.Factory(elevation);
            return Factory(ordinate, elevationEnum, units, label, abbreviatedLabel);
        }
        /// <summary>
        /// Creates an <see cref="IParameterOrdinate"/> parameter.
        /// </summary>
        /// <param name="ordinate"> An <see cref="IOrdinate"/> containing the distributed or constant height of the elevation. </param>
        /// <param name="units"> The unit of measurement for the elevation. </param>
        /// <param name="elevationEnum"> The type of elevation, <see cref="IParameterEnum"/>. </param>
        /// <returns> An <see cref="IParameterOrdinate"/> parameter. </returns>
        public static IParameterOrdinate Factory(IOrdinate ordinate, IParameterEnum elevationEnum = IParameterEnum.GroundElevation, UnitsEnum units = UnitsEnum.Foot, string label = "", bool abbreviatedLabel = true)
        {
            return new Parameters.Elevations.ElevationOrdinate(ordinate, elevationEnum, units, label, abbreviatedLabel);
        }


        ////TODO: The factory that combines two: ground and asset height elevations should rethought. Its not the right strategy for the sampling.
        //#region Factory Methods
        ///// <summary>
        ///// The simplest generator.
        ///// </summary>
        ///// <param name="height"> Elevation in feet. </param>
        ///// <param name="type"> The type of elevation, see <see cref="IElevationEnum"/>. </param>
        ///// <returns> An elevation. </returns>
        //public static IElevation Factory(IOrdinate height, IElevationEnum type)
        //{
        //    return new Elevations.Elevation(type, height);
        //}
        ///// <summary>
        ///// Generates a new asset elevation formed by combining a ground and asset height elevation.
        ///// </summary>
        ///// <param name="ground"> The ground elevation. </param>
        ///// <param name="assetHeight"> The height of the asset above the ground. </param>
        ///// <returns> A new asset elevation formed by adding the ground and asset height elevations together. For distributed values the addition is done by independently sampling values from the distributions. </returns>
        //public static IElevation Factory(IElevation ground, IElevation assetHeight, int sampleIterations)
        //{
        //    /* There are a few options here. In order of attractiveness:
        //     *      (1) pass in a functional argument which is a sampler with its own random numbers.
        //     *      (2) pass in random numbers
        //     *      (3) pass in an random number generator
        //     *      (4) this approach
        //     * The approach here is just to get something working.
        //     */
        //    //var sampler = new SampleAssetElevation().Sample(new IElevation[2] { ground, assetHeight}, )
        //    //todo: don't submit this. This needs to get finished.
        //    return null;
        //}
        //#endregion

    }
}
