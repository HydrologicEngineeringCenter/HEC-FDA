using HEC.FDA.ViewModel.FrequencyRelationships;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    /// <summary>
    /// This class exists for the sole purpose of having a blank row in the combobox
    /// </summary>
    public class FrequencyElementWrapper : BaseViewModel
    {
        private FrequencyElement _Element;
        public FrequencyElement Element
        {
            get { return _Element; }
            set { _Element = value; NotifyPropertyChanged(nameof(Name)); }
        }
        public string Name
        {
            get
            {
                if(Element == null)
                {
                    return "";
                }
                else
                {
                    return Element.Name;
                }
            }
        }

        public FrequencyElementWrapper()
        {

        }

        public FrequencyElementWrapper(FrequencyElement elem)
        {
            Element = elem;
        }


    }
}
