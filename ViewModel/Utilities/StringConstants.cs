namespace HEC.FDA.ViewModel.Utilities
{
    public static class StringConstants
    {
        //menu items
        public static readonly string RENAME_MENU = "Rename...";
        public static readonly string REMOVE_MENU = "Remove";
        public static readonly string ADD_TO_MAP_WINDOW_MENU = "Add to Map Window";
        public static readonly string REMOVE_FROM_MAP_WINDOW_MENU = "Remove From Map Window";

        //curve labels
        //INFLOW-OUTFLOW
        public static readonly string REGULATED_UNREGULATED = "Regulated-Unregulated";
        public static readonly string REGULATED = "Regulated";
        public static readonly string UNREGULATED = "Unregulated";

        //RATING CURVE
        public static readonly string STAGE_DISCHARGE = "Stage-Discharge";
        public static readonly string DISCHARGE = "Discharge";
        public static readonly string STAGE = "Stage";

        //failure function
        public static readonly string SYSTEM_RESPONSE_CURVE = "System Response Curve";
        public static readonly string FAILURE_PROBABILITY = "Failure Probability";
        //y value is stage

        //frequency curve
        public static readonly string ANALYTICAL_FREQUENCY = "Analytical Frequency";
        public static readonly string EXCEEDANCE_PROBABILITY = "Exceedance Probability";
        //y value is discharge

        //stage damage
        public static readonly string STAGE_DAMAGE = "Stage-Damage";
        //x value is stage
        public static readonly string DAMAGE = "Damage";

        //EXT-INT
        public static readonly string EXT_INT = "Exterior-Interior Stage";
        public static readonly string EXT_STAGE = "Exterior Stage";
        public static readonly string INT_STAGE = "Interior Stage";

        public static readonly string FREQUENCY_RELATIONSHIP = "Frequency Relationship";

        public static readonly string DAMAGE_FREQUENCY = "Damage-Frequency";


        public const string IMPORT_FROM_OLD_FDA = "Import Study From HEC-FDA 1.4.3";


        public static string ImportFromOldFda(string elementName)
        {
            return "Import " + elementName + " From HEC-FDA Version 1.4.3...";
        }

    }
}
