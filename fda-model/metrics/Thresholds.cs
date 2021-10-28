using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metrics
{
    public class Thresholds
{
        private Dictionary<int, Threshold> _thresholds;
        internal Threshold _defaultThreshold;

        public Thresholds(bool hasLevee, ead.Simulation simulation)
        {
            _thresholds = new Dictionary<int, Threshold>();
            this.SetDefault(hasLevee, simulation);
        }

        public void SetDefault(bool hasLevee, ead.Simulation simulation) //or simulation or ISimulation 
        {
            if (hasLevee)
            {
                ThresholdEnum thresholdType = ThresholdEnum.ExteriorStage;
                paireddata.IPairedData leveeCurve = new paireddata.PairedData(null,null); //instead, access levee from simulation
                double thresholdValue = leveeCurve.Xvals.Max();
                _defaultThreshold = new Threshold(thresholdType, thresholdValue);
            } else
            {
                ThresholdEnum thresholdType = ThresholdEnum.InteriorStage;
                paireddata.IPairedData frequencyStage = new paireddata.PairedData(null, null); //instead access from simulation
                paireddata.IPairedData stageDamage = new paireddata.PairedData(null, null); //instead access from simulation
                paireddata.IPairedData damageFrequency = simulation.ComputeDamageFrequency(frequencyStage,stageDamage);
                double percentOfDamage = 0.05;
                double damageRecurrenceInterval = 0.01;
                double significantDamage = percentOfDamage * damageFrequency.f_inverse(damageRecurrenceInterval);
                double thresholdValue = stageDamage.f_inverse(significantDamage);
                _defaultThreshold = new Threshold(thresholdType, thresholdValue);
            }
            _thresholds.Add(0, _defaultThreshold);
        }

        public void AddThreshold(int id, Threshold threshold)
        {
            _thresholds.Add(id, threshold);
        }

        public void RemoveThreshold(int id)
        {
            _thresholds.Remove(id);
        } 

        public Dictionary<int, Threshold> GetThresholds()
        {
            return _thresholds;
        }
}
}
