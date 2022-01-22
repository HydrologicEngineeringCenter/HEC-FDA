using Statistics;
using System;
namespace impactarea
{
    public class ImpactAreaSimulation
    {
        public string Name { get; }
        public int ID { get; }
        public compute.Simulation Simulation { get; }
        public ImpactArea ImpactArea { get; }
        /// <summary>
        /// The impact area scenario consists of a simulation, the name of the impact area simulation, and an ID
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sim"></param>
        /// <param name="id"></param> The ID should be the ID of the impact area 
        public ImpactAreaSimulation(String name, compute.Simulation sim, int id, ImpactArea impactArea){
            Name = name;
            Simulation = sim;
            ID = id;
            ImpactArea = impactArea;
        }
        public metrics.Results Compute(interfaces.IProvideRandomNumbers rp, Int64 iterations){
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 1, maxIterations: iterations);
            return Simulation.Compute(rp,cc);
        }

    }
}