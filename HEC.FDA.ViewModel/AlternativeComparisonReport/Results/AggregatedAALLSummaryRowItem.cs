using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results;
public class AggregatedAALLSummaryRowItem
{
    [DisplayAsColumn("Impact Area")]
    public string ImpactArea { get; set; }
    [DisplayAsColumn("Without Project Alternative")]
    public string WithoutProjAlternative { get; set; }
    [DisplayAsColumn("Without Project AALL")]
    public double WithoutProjEAD { get; set; }
    [DisplayAsColumn("With Project Alternative")]
    public string WithProjAlternative { get; set; }
    [DisplayAsColumn("With Project AALL")]
    public double WithProjEAD { get; set; }
    [DisplayAsColumn("Mean AALL Reduced")]
    public double EADDamageReduced { get; set; }
    [DisplayAsColumn("25th Percentile AALL Reduced")] //This is intentionally swapped 1-x 
    public double Point75 { get; set; }
    [DisplayAsColumn("50th Percentile AALL Reduced")]
    public double Point5 { get; set; }
    [DisplayAsColumn("75th Percentile AALL Reduced")] //This is intentionally swapped 1-x 
    public double Point25 { get; set; }


    public AggregatedAALLSummaryRowItem(string impactArea, string withoutName, double withoutEALL, string withProjName, double withProjEALL, double eallReduced, double point75, double point5, double point25)
    {
        ImpactArea = impactArea;
        WithoutProjAlternative = withoutName;
        WithoutProjEAD = withoutEALL;
        WithProjAlternative = withProjName;
        WithProjEAD = withProjEALL;
        EADDamageReduced = eallReduced;
        Point75 = point75;
        Point5 = point5;
        Point25 = point25;
    }
}
