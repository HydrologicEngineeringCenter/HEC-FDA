using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results;
public class AggregatedEALLSummaryRowItem
{
    [DisplayAsColumn("Impact Area")]
    public string ImpactArea { get; set; }
    [DisplayAsColumn("Without Project Alternative")]
    public string WithoutProjAlternative { get; set; }
    [DisplayAsColumn("Without Project EALL")]
    public double WithoutProjEAD { get; set; }
    [DisplayAsColumn("With Project Alternative")]
    public string WithProjAlternative { get; set; }
    [DisplayAsColumn("With Project EALL")]
    public double WithProjEAD { get; set; }
    [DisplayAsColumn("Mean EALL Reduced")]
    public double EADDamageReduced { get; set; }
    [DisplayAsColumn("25th Percentile EALL Reduced")] //This is intentionally swapped 1-x 
    public double Point75 { get; set; }
    [DisplayAsColumn("50th Percentile EALL Reduced")]
    public double Point5 { get; set; }
    [DisplayAsColumn("75th Percentile EALL Reduced")] //This is intentionally swapped 1-x 
    public double Point25 { get; set; }


    public AggregatedEALLSummaryRowItem(string impactArea, string withoutName, double withoutEALL, string withProjName, double withProjEALL, double eallReduced, double point75, double point5, double point25)
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
