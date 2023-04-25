using HEC.FDA.ViewModel.Study;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Alternatives.Results.BatchCompute
{
    public class AlternativeDamCatRowItem : BaseViewModel
    {

        public string Name { get; set; }
        public int BaseYear { get; set; }
        public int FutureYear { get; set; }
        public double DiscountRate { get; set; }
        public int PeriodOfAnalysis { get; set; }
        public double Mean { get; set; }
        public double Q1 { get; set; }
        public double Q2 { get; set; }
        public double Q3 { get; set; }

        public Dictionary<string, double> DamCatMap = new Dictionary<string, double>();

        public AlternativeDamCatRowItem(AlternativeElement altElem)
        {
            Name = altElem.Name;
            BaseYear = altElem.Results.AnalysisYears[0];
            FutureYear = altElem.Results.AnalysisYears[1];
            StudyPropertiesElement studyPropElem = StudyCache.GetStudyPropertiesElement();
            DiscountRate = studyPropElem.DiscountRate;
            PeriodOfAnalysis = altElem.Results.PeriodOfAnalysis;
            Mean = altElem.Results.MeanAAEQDamage();
            Q1 = altElem.Results.AAEQDamageExceededWithProbabilityQ(.75);
            Q2 = altElem.Results.AAEQDamageExceededWithProbabilityQ(.5);
            Q3 = altElem.Results.AAEQDamageExceededWithProbabilityQ(.25);


            List<string> damCats = altElem.Results.GetDamageCategories();
            foreach (string damCat in damCats)
            {
                Empirical damCatValue = altElem.Results.GetAAEQDamageDistribution(damageCategory: damCat);
                DamCatMap.Add(damCat, damCatValue.Mean);
            }
        }

    }
}
