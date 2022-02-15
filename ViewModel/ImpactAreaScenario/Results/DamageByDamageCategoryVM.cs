using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class DamageByDamageCategoryVM : BaseViewModel
    {

        public List<DamageCategoryRowItem> Rows { get; set; }

        public DamageByDamageCategoryVM()
        {
            loadDummyData();
        }


        private void loadDummyData()
        {
            List<string> xVals = loadXData();
            List<double> yVals = loadYData();

            List<DamageCategoryRowItem> rows = new List<DamageCategoryRowItem>();
            for (int i = 0; i < xVals.Count; i++)
            {
                rows.Add(new DamageCategoryRowItem(xVals[i], yVals[i]));
            }

            Rows = rows;
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
