using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metrics
{
    public class Threshold
    {
        public ThresholdEnum ThresholdType { get; set; }
        public double ThresholdValue { get; set; }
        public ProjectPerformance Performance { get; set; }

        public Threshold(bool isDefault, bool hasLevee, ead.Simulation simulation)
        {
            if (!isDefault)
            {
                throw new Exception("If not setting a default threshold, you must provide a threshold type and value");
            }
            else
            {
                this.SetDefault(hasLevee, simulation);
                Performance = new ProjectPerformance(this.ThresholdType, this.ThresholdValue);
            }
            
        }

        public Threshold(ThresholdEnum thresholdType=0, double thresholdValue=0)
        {
            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;
            Performance = new ProjectPerformance(thresholdType, thresholdValue);
        }

        public void SetDefault(bool hasLevee, ead.Simulation simulation) //or simulation or ISimulation 
        {
            if (hasLevee)
            {
                ThresholdType = ThresholdEnum.ExteriorStage;
                paireddata.IPairedData leveeCurve = new paireddata.PairedData(null, null); //instead, access levee from simulation
                ThresholdValue = leveeCurve.Xvals.Max();
            }
            else
            {
                ThresholdType = ThresholdEnum.InteriorStage;
                paireddata.IPairedData frequencyStage = new paireddata.PairedData(null, null); //instead access from simulation
                paireddata.IPairedData stageDamage = new paireddata.PairedData(null, null); //instead access from simulation
                paireddata.IPairedData damageFrequency = simulation.ComputeDamageFrequency(frequencyStage, stageDamage);
                double percentOfDamage = 0.05;
                double damageRecurrenceInterval = 0.01;
                double significantDamage = percentOfDamage * damageFrequency.f_inverse(damageRecurrenceInterval);
                ThresholdValue = stageDamage.f_inverse(significantDamage);
            }
        }
    }
}