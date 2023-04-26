using HEC.FDA.ViewModel.FlowTransforms;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class RegulatedUnregulatedElementWrapper : BaseViewModel
    {
        private InflowOutflowElement _Element;
        public InflowOutflowElement Element
        {
            get { return _Element; }
            set { _Element = value; NotifyPropertyChanged(nameof(Name)); }
        }
        public string Name
        {
            get
            {
                if (Element == null)
                {
                    return "";
                }
                else
                {
                    return Element.Name;
                }
            }
        }

        public RegulatedUnregulatedElementWrapper()
        {

        }

        public RegulatedUnregulatedElementWrapper(InflowOutflowElement elem)
        {
            Element = elem;
        }


    }
}
