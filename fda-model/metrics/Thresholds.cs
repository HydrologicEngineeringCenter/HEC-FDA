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
            //need to add default here
        }

        public void AddThreshold(int id, string thresholdType, double thresholdValue)
        {
            Threshold threshold = new Threshold(thresholdType, thresholdValue);
            _thresholds.Add(id, threshold);
        }

        public void RemoveThreshold(int id)
        {
            _thresholds.Remove(id);
        } 
}
}
