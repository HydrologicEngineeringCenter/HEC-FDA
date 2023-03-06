using System.Collections.Generic;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
{
    public class RecommendationRowItem
    {

        public string Header { get; set; }

        public List<string> Messages { get;}

        public RecommendationRowItem(string header, List<string> msgs)
        {
            Header = header;
            Messages = msgs;
        }
        public RecommendationRowItem(string header, string msg)
        {
            Header = header;
            Messages = new List<string>() { msg };
        }

        public RecommendationRowItem(string header)
        {
            Header = header;
            Messages = new List<string>() ;
        }

    }
}
