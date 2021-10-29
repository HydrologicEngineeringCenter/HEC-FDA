using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metrics
{
    public class Thresholds
{
        private List<Threshold> _thresholds;
        private Threshold _defaultThreshold;

        public Thresholds(bool hasLevee, ead.Simulation simulation)
        {
            _thresholds = new List<Threshold>();
            bool isDefault = true;
            _defaultThreshold = new Threshold(isDefault, hasLevee, simulation);
            _thresholds.Add(_defaultThreshold);
        
        }      

        public void AddThreshold(Threshold threshold)
        {
            _thresholds.Add(threshold);
        }

        public void RemoveThreshold(Threshold threshold)
        {
            _thresholds.Remove(threshold);
        } 

        public List<Threshold> GetThresholds()
        {
            return _thresholds;
        }
}
}
