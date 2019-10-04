using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    public interface IOrdinate<T>
    {
        //TODO: Property describing default value (what I started with VariableDescriptions)

        bool IsVariable { get; }
        Type OrdinateType { get; }

        string Print();
        double GetValue(double sampleProbability = 0.5);
    }
}
