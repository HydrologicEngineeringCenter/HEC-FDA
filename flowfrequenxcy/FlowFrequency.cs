using System;
using paireddata;
namespace flowfrequency
{
    public class FlowFrequency
    {
        private statistics.IBootstrap _dist;
        public FlowFrequency(statistics.IBootstrap dist){
            _dist = dist;
        }
    }
}