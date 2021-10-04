using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.ImpactAreaScenario.Results.RowItems;

namespace ViewModel.ImpactAreaScenario.Results
{
    public class PerformanceAssuranceOfThresholdVM : BaseViewModel
    {

        public List<PerformanceFrequencyRowItem> Rows { get; set; }

        public PerformanceAssuranceOfThresholdVM()
        {

            loadDummyData();

        }


        private void loadDummyData()
        {
            List<double> xVals = loadXData();
            List<double> yVals = loadYData();

            List<PerformanceFrequencyRowItem> rows = new List<PerformanceFrequencyRowItem>();
            for (int i = 0; i < xVals.Count; i++)
            {
                rows.Add(new PerformanceFrequencyRowItem(xVals[i], yVals[i]));
            }

            Rows = rows;
        }

        private List<double> loadXData()
        {

            List<double> xValues = new List<double>();
            xValues.Add(.2);
            xValues.Add(.5);

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
