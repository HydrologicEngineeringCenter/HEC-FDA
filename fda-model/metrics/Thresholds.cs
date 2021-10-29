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

        public Thresholds()
        {
            _thresholds = new Dictionary<int, Threshold>();
        
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
