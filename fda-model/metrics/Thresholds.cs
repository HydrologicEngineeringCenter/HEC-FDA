using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metrics
{
    public class Thresholds
{       //TODO: I think this should be a dictioary where the key is the threshold ID. 
        //this would probably be the easiest way to grab each threshold by ID
        private List<Threshold> _thresholds; 
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
