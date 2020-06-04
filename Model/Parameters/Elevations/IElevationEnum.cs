namespace Model
{
    /// <summary>
    /// Elevation types.
    /// </summary>
    public enum IElevationEnum
    {
        /// <summary>
        /// Default error value.
        /// </summary>
        NotSet = -1,
        /// <summary>
        /// Ground elevation.
        /// </summary>
        Ground = 0,
        /// <summary>
        /// Levee or flood wall elevation (above ground height).
        /// </summary>
        LateralStructure = 1,
        /// <summary>
        /// Height of asset above ground elevation.
        /// </summary>
        AssetHeight = 2,
        /// <summary>
        /// Asset elevation inclusive of ground elevation.
        /// </summary>
        AssetElevation = 3,
    }
}
