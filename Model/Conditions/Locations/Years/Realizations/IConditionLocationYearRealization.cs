using System.Collections.Generic;

namespace Model
{

    /// <summary>
    /// Provides a interface for computed <see cref="IConditionLocationYear{T}"/> realizations.
    /// </summary>   
    public interface IConditionLocationYearRealization: IConditionLocationYearRealizationSummary
    {
        /// <summary>
        /// A <see cref="string"/> parameter describing the sampled failure elevation (or noting its absence if not lateral structure was present in the underlying <see cref="IConditionLocationYear{T}"/>.
        /// </summary>
        ISampledParameter<IParameterOrdinate> LateralStructureFailureElevation { get; }
        /// <summary>
        /// The sampled or composed <see cref="IFdaFunction"/>s generated during the <see cref="IConditionLocationYearSummary.Compute(IReadOnlyDictionary{IParameterEnum, ISample}, int)"/> or <see cref="IConditionLocationYear{T}.ComputePreview"/> command.
        /// </summary>
        IReadOnlyDictionary<IParameterEnum, ISampledParameter<IFdaFunction>> Functions { get; }
        /// <summary>
        /// Indicates if the underlying <see cref="IConditionLocationYear{T}"/> contained an <see cref="ILateralStructure"/> or not.
        /// </summary>
        bool HadLateralStructure { get; }
    }
}
