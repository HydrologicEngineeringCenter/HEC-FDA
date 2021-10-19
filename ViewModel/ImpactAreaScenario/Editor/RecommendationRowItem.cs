using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ImpactAreaScenario.Editor
{
    public class RecommendationRowItem
    {

        public string Header { get; set; }

        public string Message { get; set; }

        public RecommendationRowItem(string header, string msg)
        {
            Header = header;
            Message = msg;
        }

    }
}
