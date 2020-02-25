using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model
{
    public interface ISample<Input, Output>
    {
        int Iterations { get; }
        Output Sample(Input input, List<double[]> randomNumberPackets);
    }
}
