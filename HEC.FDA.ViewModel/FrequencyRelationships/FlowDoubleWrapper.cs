using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class FlowDoubleWrapper: BaseViewModel
    {

        private double _Flow;

        [DisplayAsColumn("Flow (cfs)")]
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
