using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.ImpactAreaScenario.Results.RowItems;

namespace ViewModel.ImpactAreaScenario.Results
{
    public class PerformanceLongTermRiskVM : BaseViewModel
    {

        public List<PerformancePeriodRowItem> Rows { get; set; }

        public PerformanceLongTermRiskVM()
        {

            loadDummyData();

        }


        private void loadDummyData()
        {
            List<int> xVals = loadXData();
            List<int> yVals = loadYData();

            List<PerformancePeriodRowItem> rows = new List<PerformancePeriodRowItem>();
            for (int i = 0; i < xVals.Count; i++)
            {
                rows.Add(new PerformancePeriodRowItem(xVals[i], yVals[i]));
            }

            Rows = rows;
        }

        private List<int> loadXData()
        {

            List<int> xValues = new List<int>();
            xValues.Add(1);
            xValues.Add(2);

            return xValues;
        }

        private List<int> loadYData()
        {
            List<int> yValues = new List<int>();
            yValues.Add(1);
            yValues.Add(2);
    
            return yValues;
        }

    }
}
