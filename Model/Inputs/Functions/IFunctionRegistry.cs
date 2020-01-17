using System;
using System.Collections.Generic;

namespace Model.Inputs.Functions
{
    public interface IFunctionRegistry
    {
        IReadOnlyCollection<string> NamedFunctions { get; }
        //Data Reading and Writing...
    }
}
