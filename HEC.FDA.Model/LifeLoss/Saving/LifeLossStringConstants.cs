namespace HEC.FDA.Model.LifeLoss.Saving;
public static class LifeLossStringConstants
{
    // Table names
    public const string LL_LOOKUP_TABLE_NAME = "stage_life_loss_relationships_lookup";
    public const string LL_TABLE_NAME = "stage_life_loss_relationships";
    public const string LL_COMBINED_TABLE_NAME = "stage_life_loss_combined_relationships";

    // Column headers 
    public const string ID_HEADER = "ID";
    public const string ELEMENT_ID_HEADER = "Element_ID";
    public const string FUNCTION_ID_HEADER = "Function_ID";
    public const string SIMULATION_HEADER = "Simulation";
    public const string ALTERNATIVE_HEADER = "Alternative";
    public const string STAGE_HEADER = "Stage";
    public const string HAZARD_TIME_HEADER = "Hazard_Time";
    public const string SUMMARY_ZONE_HEADER = "Summary_Zone";
    public const string MIN_HEADER = "Min";
    public const string BIN_WIDTH_HEADER = "Bin_Width";
    public const string SAMPLE_MEAN_HEADER = "Sample_Mean";
    public const string SAMPLE_VARIANCE_HEADER = "Sample_Variance";
    public const string SAMPLE_MIN_HEADER = "Sample_Min";
    public const string SAMPLE_MAX_HEADER = "Sample_Max";
    public const string BIN_COUNTS_HEADER = "Bin_Counts";

    // Parameter Names
    public const string ELEMENT_ID_PARAMETER = "@elem_id";
    public const string FUNCTION_ID_PARAMETER = "@func_id";
    public const string SIMULATION_PARAMETER = "@sim";
    public const string ALTERNATIVE_PARAMETER = "@alt";
    public const string STAGE_PARAMETER = "@stg";
    public const string HAZARD_TIME_PARAMETER = "@hz_t";
    public const string SUMMARY_ZONE_PARAMETER = "@s_z";
    public const string MIN_PARAMETER = "@min";
    public const string BIN_WIDTH_PARAMETER = "@bin_w";
    public const string SAMPLE_MEAN_PARAMETER = "@s_mean";
    public const string SAMPLE_VARIANCE_PARAMETER = "@s_var";
    public const string SAMPLE_MIN_PARAMETER = "@s_min";
    public const string SAMPLE_MAX_PARAMETER = "@s_max";
    public const string BIN_COUNTS_PARAMETER = "@bin_cts";

    // Combined column headers
    public const string PROBABILITIES_HEADER = "Cumulative_Probabilities";
    public const string QUANTILES_HEADER = "Quantiles";

    // Combined parameter names
    public const string PROBABILITIES_PARAMETER = "@probs";
    public const string QUANTILES_PARAMETER = "@quantiles";

    public const string COMBINED_MAGIC_STRING = "Combined";

}
