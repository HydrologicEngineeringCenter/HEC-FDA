using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// Performance metrics.
    /// </summary>
    public enum MetricEnum
    {
        /// <summary>
        /// Default value representing an error.
        /// </summary>
        NotSet = 0,
        /// <summary>
        /// In-channel or leveed area elevation.
        /// </summary>
        ExteriorStage = 1,
        /// <summary>
        /// Floodplain or out of channel elevation.
        /// </summary>
        InteriorStage = 2,
        /// <summary>
        /// Flood damages.
        /// </summary>
        Damages = 3,
        /// <summary>
        /// Probability weighted average annual flood damages.
        /// </summary>
        ExpectedAnnualDamage = 4,
    }
}
