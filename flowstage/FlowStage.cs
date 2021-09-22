using System;
using paireddata;
namespace flowstage
{
    public class FlowStage
    {
        //if you want to control how a flowstage curve works,
        //you can use a class to wrap the uncertainpairedata,
        //with that class you can control access and modify behavior.
        private UncertainPairedData _curve;
        public FlowTStage(UncertainPairedData curve){
            _curve = curve;
        }
        //here is an example where flowstage performs compose
        //with flow transform (a similar method with FlowFrequency would be necessary)
        public IPairedData Compose(flowtransform flowTransform, double probability){
            IPairedData fs = _curve.Sample(probability);
            //would probably need to not use the same probability
            //so either pass in two probabilities or pass in a seed and create
            //a random number generator (which has a performance hit.)
            return flowTransform.Sample(probability).Compose(fs);
        }
    }
}