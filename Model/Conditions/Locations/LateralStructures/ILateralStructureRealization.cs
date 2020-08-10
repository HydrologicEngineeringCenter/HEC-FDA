using Functions;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model
{
    /// <summary>
    /// A sampled realization of the underlying <see cref="ILateralStructure"/>
    /// </summary>
    public interface ILateralStructureRealization: ILateralStructure
    {
        /// <summary>
        /// The <see cref="IParameterEnum.ExteriorElevation"/> at which the <see cref="ILateralStructure"/> is assumed to fail in the realization.
        /// </summary>
        IParameterOrdinate FailureElevation { get; }

        /// <summary>
        /// Creates an new <see cref="IParameterEnum.ExteriorInteriorStage"/> <see cref="ITransformFunction"/> taking into account the <see cref="FailureElevation"/> and specified constant <see cref="IParameterEnum.ExteriorInteriorStage"/> <see cref="ITransformFunction"/>.
        /// </summary>
        /// <param name="sampledExtIntFx"> A sampled or otherwise constant <see cref="IParameterEnum.ExteriorInteriorStage"/> <see cref="ITransformFunction"/> that describes the relationship between exterior (in-channel) and interior (floodplain) water surface elevations in the absence or complete failure of the <see cref="ILateralStructure"/>. </param>
        /// <returns> A new <see cref="IParameterEnum.ExteriorInteriorStage"/> <see cref="ITransformFunction"/> taking into account the <see cref="ILateralStructure"/>'s sampled <see cref="FailureElevation"/>. </returns>
        ITransformFunction InteriorExteriorGenerator(ITransformFunction sampledExtIntFx);
        /// <summary>
        /// Creates an <see cref="IParameterEnum.ExteriorInteriorStage"/> <see cref="ITransformFunction"/> showing the truncation of interior (floodplain) water surface elevations below the <see cref="ILateralStructureRealization"/>'s <see cref="FailureElevation"/>. 
        /// </summary>
        /// <param name="sampledExtFreqFx"> A sampled or otherwise constant <see cref="IParameterEnum.ExteriorStageFrequency"/> <see cref="IFrequencyFunction"/> used to generate the stage values for the output <see cref="IParameterEnum.ExteriorInteriorStage"/> <see cref="ITransformFunction"/>. </param>
        /// <returns> A <see cref="IParameterEnum.ExteriorInteriorStage"/> <see cref="ITransformFunction"/> describing the truncation of interior (floodplain) water surface elevations below the <see cref="ILateralStructureRealization"/>'s <see cref="FailureElevation"/>. </returns>
        ITransformFunction InteriorExteriorGenerator(IFrequencyFunction sampledExtFreqFx);
    }
}
