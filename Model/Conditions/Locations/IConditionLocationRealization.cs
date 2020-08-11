using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{

    /// <summary>
    /// Provides a interface for computed <see cref="IConditionLocation{In, Out}"/> realizations with the generic parameter value equal to the second generic parameter in computed <see cref="IConditionLocation{In, Out}"/>.
    /// </summary>
    /// <typeparam name="T"> <see cref="string"/> if the computed <see cref="IConditionLocation{In, Out}"/> does <b>not</b> contain a <see cref="ILateralStructure"/>, otherwise use <see cref="ISampledParameter{IParameterOrdinate}"/> to hold the sampled <see cref="ILateralStructure"/> failure elevation. </typeparam>
    public interface IConditionLocationRealization<T>
    {
        /// <summary>
        /// A <see cref="string"/> parameter if the <see cref="IConditionLocation{In, Out}"/> does not contain a <see cref="ILateralStructure"/>, otherwise a <see cref="ISampledParameter{IParameterOrdinate}"/> describing the sampled failure elevation.
        /// </summary>
        T LateralStructureFailureElevation { get; }
        /// <summary>
        /// The computed <see cref="IMetric"/> values.
        /// </summary>
        IReadOnlyDictionary<IMetric, double> Metrics { get; }
        /// <summary>
        /// The sampled or composed <see cref="IFdaFunction"/> generated during the <see cref="IConditionLocation{In, Out}.Compute(IReadOnlyDictionary{IParameterEnum, ISample})"/> or <see cref="IConditionLocation{In, Out}.Compute(IReadOnlyDictionary{IParameterEnum, ISample})"/> command.
        /// </summary>
        IReadOnlyDictionary<IParameterEnum, ISampledParameter<IFdaFunction>> Functions { get; }
    }
}
