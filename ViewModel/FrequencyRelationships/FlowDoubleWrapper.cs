using System;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class FlowDoubleWrapper: BaseViewModel
    {
        public event EventHandler FlowChanged;

        private double _Flow;
        public double Flow 
        {
            get { return _Flow; }
            set { _Flow = value; FlowChanged?.Invoke(this, new EventArgs()); }
        }

        public FlowDoubleWrapper(double flow)
        {
            Flow = flow;
        }
    }
}
