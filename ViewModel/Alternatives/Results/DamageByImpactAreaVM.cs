using System.Collections.Generic;

namespace ViewModel.Alternatives.Results
{
    public class DamageByImpactAreaVM : IAlternativeResult
    {

        public List<ImpactAreaRowItem> Rows { get; } = new List<ImpactAreaRowItem>();


        public DamageByImpactAreaVM():base()
        {
            loadDummyData();
        }

        private void loadDummyData()
        {
            List<string> xVals = loadXData();
            List<double> yVals = loadYData();

            for (int i = 0; i < xVals.Count; i++)
            {
                Rows.Add(new ImpactAreaRowItem(xVals[i], yVals[i]));
            }
        }

        private List<string> loadXData()
        {
            List<string> xValues = new List<string>();
            xValues.Add("Impact1");
            xValues.Add("Impact2");
            return xValues;
        }

        private List<double> loadYData()
        {
            List<double> yValues = new List<double>();
            yValues.Add(11);
            yValues.Add(22);
            return yValues;
        }

    }
}
