﻿namespace HEC.FDA.ViewModel.Utilities
{
    public static class StringConstants
    {
        #region Generic Menu Items
        public static readonly string RENAME_MENU = "Rename...";
        public static readonly string REMOVE_MENU = "Remove";
        public static readonly string VIEW_RESULTS_MENU = "View Results...";
        public const string CALCULATE_AED_MENU = "Calculate Equivalent Annual Damage...";


        #endregion

        #region curve labels
        //regulated-unregulated (formerly INFLOW-OUTFLOW)
        public static readonly string REGULATED_UNREGULATED = "Regulated-Unregulated";
        //x-axis
        public static readonly string REGULATED = "Regulated";
        //y-axis
        public static readonly string UNREGULATED = "Unregulated";

        //Stage-Discharge (formerly rating)
        public static readonly string STAGE_DISCHARGE = "Stage-Discharge";
        //x-axis
        public static readonly string DISCHARGE = "Discharge";
        //y-axis
        public static readonly string STAGE = "Stage";

        //failure function
        public static readonly string SYSTEM_RESPONSE_CURVE = "System Response Curve";
        //x-axis
        public static readonly string FAILURE_FREQUENCY = "Probability of Failure";
        //y-axis is stage

        //analytical frequency curve
        public static readonly string ANALYTICAL_FREQUENCY = "Analytical Frequency";
        //x-axis
        public static readonly string EXCEEDANCE_PROBABILITY = "Exceedance Probability";
        //y-axis is discharge

        //stage damage
        public static readonly string STAGE_DAMAGE = "Stage-Damage";
        //x-axis is stage
        //y-axis
        public static readonly string DAMAGE = "Damage";

        //EXT-INT
        public static readonly string EXT_INT = "Exterior-Interior Stage";
        //x-axis
        public static readonly string EXT_STAGE = "Exterior Stage";
        //y-axis
        public static readonly string INT_STAGE = "Interior Stage";

        public static readonly string FREQUENCY_RELATIONSHIP = "Frequency Relationship";
        public static readonly string FREQUENCY = "Frequency";
        public static readonly string GRAPHICAL_FREQUENCY = "Graphical Frequency";
        public static readonly string GRAPHICAL_STAGE_FREQUENCY = "Graphical Stage Frequency";

        public static readonly string DAMAGE_FREQUENCY = "Damage-Frequency";

        #endregion

        #region Tree Node Headers and Menus
        public const string TERRAIN = "Terrain";
        public const string IMPORT_TERRAIN_MENU = "Import Terrain";
        public const string IMPORT_TERRAIN_HEADER = "Import Terrain";

        public const string IMPACT_AREAS = "Impact Areas";

        public const string IMPACT_AREA_SET = "Impact Area Set";
        public const string IMPORT_IMPACT_AREA_SET_MENU = "Import Impact Area Set...";
        public const string EDIT_IMPACT_AREA_SET_MENU = "Edit Impact Area Set...";
        public const string EDIT_IMPACT_AREA_SET_HEADER = "Edit Impact Area Set";
        public const string IMPORT_IMPACT_AREA_SET_HEADER = "Import Impact Area Set";

        public const string INDEX_POINTS = "Index Points";
        public const string CREATE_INDEX_POINTS_MENU = "Create New Index Points...";
        public const string CREATE_INDEX_POINTS_HEADER = "Create New Index Points";
        public const string EDIT_INDEX_POINTS_MENU = "Edit Index Points...";
        public const string EDIT_INDEX_POINTS_HEADER = "Edit Index Points";

        public const string HYDRAULICS = "Hydraulics";

        public const string UNSTEADY_HDF = "Unsteady HDF";

        public const string STEADY_HDF = "Steady HDF";

        public const string GRIDDED_DATA = "Gridded Data";
        public const string IMPORT_HYDRAULICS_MENU = "Import Hydraulics...";
        public const string EDIT_HYDRAULICS_MENU = "Edit Hydraulics...";
        public const string IMPORT_HYDRAULICS_HEADER = "Import Hydraulics";

        public const string FREQUENCY_FUNCTIONS = "Frequency Functions";
        public const string CREATE_FREQUENCY_FUNCTIONS_MENU = "Create New Frequency Function...";
        public const string RETRIEVE_GRAPHICAL_FREQUENCY_FUNCTION_MENU = "Retrieve Graphical Frequency Function...";
        public const string RETRIEVE_GRAPHICAL_FREQUENCY_FUNCTION_HEADER = "Retrieve Graphical Frequency Function";
        public const string EDIT_FREQUENCY_FUNCTIONS_MENU = "Edit Frequency Function...";
        public const string CREATE_FREQUENCY_HEADER = "Create Frequency Function";
        public const string IMPORT_FREQUENCY_FROM_OLD_NAME = "Frequency Function";
        public const string IMPORT_SYNTHETIC_FUNTION_FROM_DBF = "Import Synthetic Analytical Flow-Frequency Function from DBF...";

        public const string REG_UNREG_TRANSFORM_FUNCTIONS = "Regulated-Unregulated Transform Functions";
        public const string CREATE_REG_UNREG_MENU = "Create New Regulated-Unregulated Relationship...";
        public const string EDIT_REG_UNREG_MENU = "Edit Regulated-Unregulated Relationship...";
        public const string CREATE_REG_UNREG_HEADER = "Create New Regulated-Unregulated Relationship";
        public const string IMPORT_REG_UNREG_FROM_OLD_NAME = "Regulated-Unregulated";

        public const string STAGE_TRANSFORM_FUNCTIONS = "Stage Transform Functions";

        public const string STAGE_DISCHARGE_FUNCTIONS = "Stage-Discharge Functions";
        public const string CREATE_STAGE_DISCHARGE_MENU = "Create New Stage-Discharge Relationship...";
        public const string EDIT_STAGE_DISCHARGE_MENU = "Edit Stage-Discharge Relationship...";
        public const string CREATE_STAGE_DISCHARGE_HEADER = "Create New Stage-Discharge Relationship";

        public const string IMPORT_STAGE_DISCHARGE_FROM_OLD_NAME = "Stage-Discharge";

        public const string EXTERIOR_INTERIOR_FUNCTIONS = "Exterior-Interior Functions";
        public const string CREATE_EXT_INT_MENU = "Create New Exterior-Interior Relationship...";
        public const string EDIT_EXT_INT_MENU = "Edit Exterior-Interior Relationship...";
        public const string CREATE_EXT_INT_HEADER = "Create New Exterior-Interior Relationship";
        public const string IMPORT_EXT_INT_FROM_OLD_NAME = "Exterior-Interior Relationship";

        public const string LATERAL_STRUCTURES = "Lateral Structures";
        public const string CREATE_LATERAL_STRUCTURES_MENU = "Create New Lateral Structure...";
        public const string EDIT_LATERAL_STRUCTURES_MENU = "Edit Lateral Structure...";
        public const string CREATE_LATERAL_STRUCTURES_HEADER = "Create New Lateral Structure";
        public const string IMPORT_LATERAL_STRUCTURES_FROM_OLD_NAME = "Lateral Structures";

        public const string ECONOMICS = "Economics";

        public const string OCCUPANCY_TYPES = "Occupancy Types";
        public const string EDIT_OCCTYPE_MENU = "Edit Occupancy Types...";
        public const string EDIT_OCCTYPE_HEADER = "Edit Occupancy Types";
        public const string IMPORT_OCCTYPE_FROM_OLD_NAME = "Occupancy Types";
        public const string CREATE_NEW_OCCTYPE_MENU = "Create New Occupancy Types...";
        public const string CREATE_NEW_OCCTYPE_HEADER = "Create New Occupancy Types";

        public const string STRUCTURE_INVENTORIES = "Structure Inventories";
        public const string IMPORT_STRUCTURE_INVENTORIES_MENU = "Import From Shapefile...";
        public const string IMPORT_STRUCTURE_INVENTORIES_HEADER = "Import Structure Inventory";
        public const string EDIT_STRUCTURES_MENU = "Edit Structure Inventory...";


        public const string AGGREGATED_STAGE_DAMAGE_FUNCTIONS = "Aggregated Stage-Damage Functions";
        public const string CREATE_NEW_STAGE_DAMAGE_MENU = "Create New Stage-Damage Functions...";
        public const string CREATE_NEW_STAGE_DAMAGE_HEADER = "Create New Stage-Damage Functions";
        public const string EDIT_STAGE_DAMAGE_MENU = "Edit Stage-Damage Functions...";
        public const string EXPORT_STAGE_DAMAGE_MENU = "Export Structure Detail...";

        public const string IMPORT_STAGE_DAMAGE_FROM_OLD_NAME = "Stage-Damage Functions";

        public const string SCENARIOS = "Scenarios";
        public const string CREATE_NEW_SCENARIO_MENU = "Create New Scenario...";
        public const string COMPUTE_SCENARIOS_MENU = "Compute Scenarios...";
        public const string COMPUTE_SCENARIOS_HEADER = "Compute Scenarios";
        public const string VIEW_SUMMARY_RESULTS_MENU = "View Summary Results...";
        public const string VIEW_SUMMARY_RESULTS_HEADER = "Summary Results";
        public const string EDIT_SCENARIO_MENU = "Edit Scenario...";
        public const string COMPUTE_SCENARIO_MENU = "Compute Scenario...";
        public const string VIEW_THRESHOLDS_MENU = "View Thresholds...";
        public const string CREATE_NEW_SCENARIO_HEADER = "Create New Scenario";

        public const string ALTERNATIVES = "Alternatives";
        public const string CREATE_NEW_ALTERNATIVE_MENU = "Create New Alternative...";
        public const string EDIT_ALTERNATIVE_MENU = "Edit Alternative...";
        public const string CREATE_NEW_ALTERNATIVE_HEADER = "Create New Alternative";

        public const string ALTERNATIVE_COMP_REPORTS = "Alternative Comparison Reports";
        public const string CREATE_NEW_ALTERNATIVE_COMP_REPORTS_MENU = "Create New Alternative Comparison Report...";
        public const string EDIT_ALTERNATIVE_COMP_REPORTS_MENU = "Edit Alternative Comparison Report...";
        public const string ALTERNATIVE_COMP_REPORTS_SUMMARY = "View Alternatives Summary Results...";
        public const string ALTERNATIVE_COMP_REPORTS_SUMMARY_HEADER = "View Alternatives Summary Results";

        public const string CREATE_NEW_ALTERNATIVE_COMP_REPORTS_HEADER = "Create New Alternative Comparison Report";

        #endregion

        #region ias editor labels
        public static readonly string FREQUENCY_RELATIONSHIP_LABEL = "Frequency Relationship:";
        public static readonly string REGULATED_UNREGULATED_LABEL = "Reg-Unreg Flow:";
        public static readonly string STAGE_DISCHARGE_LABEL = "Stage-Discharge:";
        public const string LATERAL_STRUCTURE_LABEL = "Lateral Structure:";
        public static readonly string EXT_INT_SHORT_LABEL = "Ext-Int Stage:";
        public static readonly string STAGE_DAMAGE_LABEL = "Stage-Damage:";
        #endregion

        #region Graphical
        #endregion

        #region Result Histogram Labels
        public static readonly string HISTOGRAM_VALUE = "Value";
        public static readonly string HISTOGRAM_FREQUENCY = "Relative Frequency";
        public static readonly string HISTOGRAM_EXCEEDANCE_PROBABILITY = "Exceedance Probability";
        public static readonly string EXPECTED_ANNUAL_DAMAGE = "Expected Annual Damage";
        public static readonly string EAD_DISTRIBUTION = "EAD Distribution";
        public static readonly string DAMAGE_REDUCED = "Damage Reduced Distribution";
        public static readonly string EqAD_DISTRIBUTION = "EqAD Distribution";
        public static readonly string EQUIVALENT_ANNUAL_DAMAGE = "Equivalent Annual Damage";
        public static readonly string STUDY_PROPERTIES = "Study Properties";
        public static readonly string PROPERTIES = "Properties";


        #endregion

        public const string OCCTYPE_PLOT_TITLE = "Depth-Percent Damage";
        public const string OCCTYPE_DEPTH = "Depth";
        public const string OCCTYPE_PERCENT_DAMAGE = "Percent Damage";

        public const string DEFAULT_UNIT_FORMAT = "N2";
        public const string DETAILED_DECIMAL_FORMAT = "N4";

        public const string QUARTILE_VALUE = "Quartile Value";

        public const string SCENARIOS_EAD_LABEL = "Quartile of EAD Distribution";

        public const string ALTERNATIVE_EAD_LABEL = "Quartile of EAD Distribution";
        public const string ALTERNATIVE_EqAD_LABEL = "Quartile of EqAD Distribution";

        public const string ALTERNATIVE_COMP_REPORT_EAD_LABEL = "Quartile of EAD Reduced Distribution";
        public const string ALTERNATIVE_COMP_REPORT_EqAD_LABEL = "Quartile of EqAD Reduced Distribution";

        public const string SCENARIO_PROGRESS_LABEL = "Computing Impact Area:";
        public const string ALTERNATIVE_PROGRESS_LABEL = "Compute progress:";
        public const string ALT_COMP_REPORT_PROGRESS_LABEL = "Compute progress:";

        public const string ELEMENT_XML_TAG = "ElementXML";

        public const string TAB_DELIMITED = "Tab-Delimited Text File";

        public static string CreateImportFromFileMenuString(string elementName)
        {
            return "Import " + elementName + " From " + TAB_DELIMITED + "...";
        }

        public static string CreateImportHeader(string elementName)
        {
            return "Import " + elementName + " From " + TAB_DELIMITED;

        }

        public static string CreateLastEditTooltip(string lastEditDate)
        {
            string tooltip = null;
            if(lastEditDate != null)
            {
                tooltip = "Last edited " + lastEditDate;
            }
            return tooltip;
        }

    }
}
