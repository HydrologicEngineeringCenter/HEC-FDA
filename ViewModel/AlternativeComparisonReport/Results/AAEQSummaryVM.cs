using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Alternatives.Results;
using ViewModel.ImpactAreaScenario.Results.RowItems;

namespace ViewModel.AlternativeComparisonReport.Results
{
    public class AAEQSummaryVM : IAlternativeResult
    {
        public List<AAEQSummaryRowItem> Rows { get; } = new List<AAEQSummaryRowItem>();

        public AAEQSummaryVM() 
        {
            loadDummyData();
        }

        private void loadDummyData()
        {
            List<string> xVals = loadXData();
            List<double> yVals = loadYData();

            for (int i = 0; i < xVals.Count; i++)
            {
                Rows.Add(new AAEQSummaryRowItem());
            }
        }

        private List<string> loadXData()
        {
            List<string> xValues = new List<string>();
            xValues.Add("Residential");
            xValues.Add("Commercial");
            return xValues;
        }

        private List<double> loadYData()
        {
            List<double> yValues = new List<double>();
            yValues.Add(1);
            yValues.Add(2);
            return yValues;
        }

    }
}
