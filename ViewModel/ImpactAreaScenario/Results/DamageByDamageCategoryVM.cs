using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using metrics;
using Statistics.Histograms;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class DamageByDamageCategoryVM : BaseViewModel
    {

        public List<DamageCategoryRowItem> Rows { get; set; }

        public DamageByDamageCategoryVM(metrics.Results iasResult)
        {
            ExpectedAnnualDamageResults eadResults = iasResult.ExpectedAnnualDamageResults;
            //eadResults.HistogramsOfEADs["blah"].Mean
            LoadDamCatTable(eadResults);
        }


        private void LoadDamCatTable(ExpectedAnnualDamageResults eadResults)
        {
            List<string> xVals = new List<string>();
            List<double> yVals = new List<double>();

            foreach(KeyValuePair<string, ThreadsafeInlineHistogram> entry in eadResults.HistogramsOfEADs)
            {
                xVals.Add(entry.Key);
                yVals.Add(entry.Value.Mean);
            }

            List<DamageCategoryRowItem> rows = new List<DamageCategoryRowItem>();
            for (int i = 0; i < xVals.Count; i++)
            {
                rows.Add(new DamageCategoryRowItem(xVals[i], yVals[i]));
            }

            Rows = rows;
        }

        //private List<string> loadXData()
        //{

        //    List<string> xValues = new List<string>();
        //    xValues.Add("Residential");
        //    xValues.Add("Commercial");
        //    return xValues;
        //}

        //private List<double> loadYData()
        //{
        //    List<double> yValues = new List<double>();
        //    yValues.Add(1);
        //    yValues.Add(2);
        //    return yValues;
        //}

    }
}
