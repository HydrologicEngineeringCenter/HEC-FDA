using HEC.FDA.ViewModel.FrequencyRelationships;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    /// <summary>
    /// This class exists for the sole purpose of having a blank row in the combobox
    /// </summary>
    public class FrequencyElementWrapper
    {

        public AnalyticalFrequencyElement Element { get; }
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

        public FrequencyElementWrapper(AnalyticalFrequencyElement elem)
        {
            Element = elem;
        }


    }
}
