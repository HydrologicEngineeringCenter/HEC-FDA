using System.Collections.Generic;

namespace Model
{

    /// <summary>
    /// Provides a interface for computed <see cref="IConditionLocationTime{T}"/> realizations.
    /// </summary>   
    public interface IConditionLocationTimeRealization
    {
        /// <summary>
        /// A <see cref="string"/> parameter describing the sampled failure elevation (or noting its absence if not lateral structure was present in the underlying <see cref="IConditionLocationTime{T}"/>.
        /// </summary>
        ISampledParameter<IParameterOrdinate> LateralStructureFailureElevation { get; }
        /// <summary>
        /// The sampled or composed <see cref="IFdaFunction"/> generated during the <see cref="IConditionLocationTime{T}.Compute(IReadOnlyDictionary{IParameterEnum, ISample})"/> or <see cref="IConditionLocationTime{T}.Compute(IReadOnlyDictionary{IParameterEnum, ISample})"/> command.
        /// </summary>
        IReadOnlyDictionary<IParameterEnum, ISampledParameter<IFdaFunction>> Functions { get; }
        /// <summary>
        /// Indicates if the underlying <see cref="IConditionLocationTime{T}"/> contained an <see cref="ILateralStructure"/> or not.
        /// </summary>
        bool HadLateralStructure { get; }
    }
}
