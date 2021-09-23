using System;
using paireddata;
namespace flowtransform
{
    public class FlowTransform :IPairedDataProducer
    {
        //if you want to control how a flow transform works,
        //you can use a class to wrap the uncertainpairedata,
        //with that class you can control access and modify behavior.
        private UncertainPairedData _curve;
        public FlowTransform(UncertainPairedData curve){
            _curve = curve;
        }
        //here is an example where flow transform converts the paireddata
        //into a stepwisepaired data as an example
        public IPairedData SamplePairedData(double probability){
            IPairedData pd = _curve.SamplePairedData(probability);
            StepwisePairedData spd = ((PairedData)pd).ToStepwisePairedData();
            return spd;
        }
    }
}