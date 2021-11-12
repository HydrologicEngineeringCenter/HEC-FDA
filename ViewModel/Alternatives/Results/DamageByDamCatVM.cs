using System.Collections.Generic;
using ViewModel.ImpactAreaScenario.Results.RowItems;

namespace ViewModel.Alternatives.Results
{
    public class DamageByDamCatVM : AlternativeResultBase
    {
        public List<DamageCategoryRowItem> Rows { get; } = new List<DamageCategoryRowItem>();

        public DamageByDamCatVM():base()
        {
            loadDummyData();
        }

        private void loadDummyData()
        {
            List<string> xVals = loadXData();
            List<double> yVals = loadYData();

            for (int i = 0; i < xVals.Count; i++)
            {
                Rows.Add(new DamageCategoryRowItem(xVals[i], yVals[i]));
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
