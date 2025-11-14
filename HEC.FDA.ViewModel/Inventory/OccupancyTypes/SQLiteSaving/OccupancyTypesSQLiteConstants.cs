namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes.SQLiteSaving;
public static class OccupancyTypesSQLiteConstants
{
    // Table names
    public const string METADATA_TABLE_NAME = "Metadata";
    public const string OCCTYPES_TABLE_NAME = "OccupanyTypes";
    public const string ASSETS_TABLE_NAME = "Assets";

    // Metadata headers
    public const string METADATA_ID_HEADER = "ID";
    public const string METADATA_NAME_HEADER = "Name";
    public const string METADATA_DESCRIPTION_HEADER = "Description";
    public const string METADATA_LASTEDIT_HEADER = "LastEditDate";

    // Occtypes headers
    public const string OCCTYPES_ID_HEADER = "ID";
    public const string OCCTYPES_NAME_HEADER = "Name"; // primary key
    public const string OCCTYPES_DESCRIPTION_HEADER = "Description";
    public const string OCCTYPES_DAMCAT_HEADER = "DamCat";
    public const string OCCTYPES_FOUND_UNCERTAINTY_HEADER = "FoundationUncertainty";

    // Asset headers
    public const string ASSETS_OCCTYPE_HEADER = "OccType"; // foreign key references occtypes name
    public const string ASSETS_ISSELECTED_HEADER = "IsSelected";
    public const string ASSETS_TYPE_HEADER = "Type";
    public const string ASSETS_BYVALUE_HEADER = "ByValue";
    public const string ASSETS_SELECTEDCURVE_HEADER = "SelectedCurve";
    public const string ASSETS_SELECTEDCURVENAME_HEADER = "SelectedCurveName";
    public const string ASSETS_DISTOPTIONS_HEADER = "DistributionOptions";
    public const string ASSETS_DESCRIPTION_HEADER = "Description";
    public const string ASSETS_CURVEDATA_HEADER = "CurveData";
    public const string ASSETS_UNCERTAINTY_HEADER = "Uncertainty";
    public const string ASSETS_RATIOUNCERTAINTY_HEADER = "RatioUncertainty";

    // Metadata parameters
    public const string METADATA_NAME_PARAMETER = "@m_name";
    public const string METADATA_DESCRIPTION_PARAMETER = "@m_desc";
    public const string METADATA_LASTEDIT_PARAMETER = "@m_date";

    // Occtypes parameters
    public const string OCCTYPES_ID_PARAMETER = "@o_id";
    public const string OCCTYPES_NAME_PARAMETER = "@o_name";
    public const string OCCTYPES_DESCRIPTION_PARAMETER = "@o_desc";
    public const string OCCTYPES_DAMCAT_PARAMETER = "@o_damcat";
    public const string OCCTYPES_FOUND_UNCERTAINTY_PARAMETER = "@o_found";

    // Asset parameters
    public const string ASSETS_OCCTYPE_PARAMETER = "@a_occtype";
    public const string ASSETS_ISSELECTED_PARAMETER = "@a_selected";
    public const string ASSETS_TYPE_PARAMETER = "@a_type";
    public const string ASSETS_BYVALUE_PARAMETER = "@a_byval";
    public const string ASSETS_SELECTEDCURVE_PARAMETER = "@a_curve";
    public const string ASSETS_SELECTEDCURVENAME_PARAMETER = "@a_curvename";
    public const string ASSETS_DISTOPTIONS_PARAMETER = "@a_distoptions";
    public const string ASSETS_DESCRIPTION_PARAMETER = "@a_desc";
    public const string ASSETS_CURVEDATA_Parameter = "@a_curvedata";
    public const string ASSETS_UNCERTAINTY_PARAMETER = "@a_uncertainty";
    public const string ASSETS_RATIOUNCERTAINTY_PARAMETER = "@a_ratiouncertainty";
}
