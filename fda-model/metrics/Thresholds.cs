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

        public List<Threshold> ListOfThresholds
        {
            get
            {
                return _thresholds;
            }
            set
            {
                _thresholds = value;
            }
        }

        public Thresholds()
        {
            _thresholds = new List<Threshold>();
                   
        }      

        public void AddThreshold(Threshold threshold)
        {
            _thresholds.Add(threshold);
        }

        public void RemoveThreshold(Threshold threshold)
        {
            _thresholds.Remove(threshold);
        } 


}
}
