using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Alternatives.Results
{
    public class DamageByImpactAreaVM : AlternativeResultBase
    {

        public List<ImpactAreaRowItem> Rows { get; set; }


        public DamageByImpactAreaVM():base("Damage by Impact Area")
        {
            loadDummyData();
        }


        private void loadDummyData()
        {
            List<string> xVals = loadXData();
            List<double> yVals = loadYData();

            List<ImpactAreaRowItem> rows = new List<ImpactAreaRowItem>();
            for (int i = 0; i < xVals.Count; i++)
            {
                rows.Add(new ImpactAreaRowItem(xVals[i], yVals[i]));
            }

            Rows = rows;
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
            yValues.Add(1);
            yValues.Add(2);
            return yValues;
        }

    }
}
