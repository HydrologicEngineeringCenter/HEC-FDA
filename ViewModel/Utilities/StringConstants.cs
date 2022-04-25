namespace HEC.FDA.ViewModel.Utilities
{
    public static class StringConstants
    {
        #region Menu Items
        public static readonly string RENAME_MENU = "Rename...";
        public static readonly string REMOVE_MENU = "Remove";
        public static readonly string ADD_TO_MAP_WINDOW_MENU = "Add to Map Window";
        public static readonly string REMOVE_FROM_MAP_WINDOW_MENU = "Remove From Map Window";
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
        public static readonly string FAILURE_FREQUENCY = "Failure Frequency";
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


        public const string IMPORT_FROM_OLD_FDA = "Import Study From HEC-FDA 1.4.3";


        public static string ImportFromOldFda(string elementName)
        {
            return "Import " + elementName + " From HEC-FDA Version 1.4.3...";
        }

    }
}
