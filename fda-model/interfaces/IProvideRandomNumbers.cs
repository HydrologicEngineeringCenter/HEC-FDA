using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interfaces
{
    public interface IProvideRandomNumbers
{
        double NextRandom();
        double[] NextRandomSequence(int size);
        int Seed { get; }
}
}
