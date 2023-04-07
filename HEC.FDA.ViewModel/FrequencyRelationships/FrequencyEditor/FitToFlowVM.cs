using HEC.FDA.ViewModel.TableWithPlot;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor
{
    internal class FitToFlowVM:ParameterEntryVM
    {
        private readonly ObservableCollection<FlowDoubleWrapper> _AnalyticalFlows = new ObservableCollection<FlowDoubleWrapper>();
        public ObservableCollection<FlowDoubleWrapper> AnalyticalFlows
        {
            get { return _AnalyticalFlows; }
        }

        public FitToFlowVM()
        {

        }
        public FitToFlowVM(XElement ele)
        {

        }
        private void LoadFlows(AnalyticalFrequencyElement elem)
        {
            if (elem.AnalyticalFlows.Count == 0)
            {
                LoadDefaultFlows();
            }
            else
            {
                foreach (double flow in elem.AnalyticalFlows)
                {
                    FlowDoubleWrapper fdw = new FlowDoubleWrapper(flow);
                    AnalyticalFlows.Add(fdw);
                }
            }
        }
        private void LoadDefaultFlows()
        {
            for (int i = 1; i < 11; i++)
            {
                FlowDoubleWrapper fdw = new FlowDoubleWrapper(i * 1000);
                AnalyticalFlows.Add(fdw);
            }
        }

    }
}
