namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class FlowDoubleWrapper: BaseViewModel
    {

        private double _Flow;
        public double Flow 
        {
            get { return _Flow; }
            set { _Flow = value; }
        }

        public FlowDoubleWrapper(double flow)
        {
            _Flow = flow;
        }
    }
}
