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
        public FlowStage(UncertainPairedData curve){
            _curve = curve;
        }
        //here is an example where flowstage performs compose
        //with flow transform (a similar method with FlowFrequency would be necessary)
        public IPairedData Compose(flowtransform.FlowTransform flowTransform, double probability){
            IPairedData fs = _curve.SamplePairedData(probability);
            //would probably need to not use the same probability
            //so either pass in two probabilities or pass in a seed and create
            //a random number generator (which has a performance hit.)
            return flowTransform.Sample(probability).Compose(fs);
        }
        public IPairedData Compose(flowfrequency.FlowFrequency flowFrequency, double probability){
            IPairedData fs = _curve.SamplePairedData(probability);
            //same notes as above.
            return flowFrequency.Sample(probability).Compose(fs);
        }
    }
}