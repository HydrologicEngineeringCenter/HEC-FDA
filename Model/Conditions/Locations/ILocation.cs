using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// Describes a location where computations are performed.
    /// </summary>
    public interface ILocation
    {
        /// <summary>
        /// A unique name for the location.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// An optional description of the location.
        /// </summary>
        string Description { get; }
    }
}
