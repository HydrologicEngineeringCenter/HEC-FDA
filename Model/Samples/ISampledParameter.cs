using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// Extends the <see cref="ISample"/> interface to contain the sampled parameter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISampledParameter<T> : ISample
    {
        /// <summary>
        /// The sampled parameter.
        /// </summary>
        T Parameter { get; }
    }
}
