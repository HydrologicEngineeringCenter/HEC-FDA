using HEC.FDA.ViewModel.StageTransforms;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    /// <summary>
    /// This class exists for the sole purpose of having a blank row in the combobox
    /// </summary>
    public class StageDischargeElementWrapper
    {
        public RatingCurveElement Element { get; }
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

        public StageDischargeElementWrapper()
        {

        }

        public StageDischargeElementWrapper(RatingCurveElement elem)
        {
            Element = elem;
        }
    }
}
