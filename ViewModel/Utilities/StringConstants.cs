namespace HEC.FDA.ViewModel.Utilities
{
    public static class StringConstants
    {

        public static readonly string RENAME_MENU = "Rename...";
        public static readonly string REMOVE_MENU = "Remove";
        public static readonly string ADD_TO_MAP_WINDOW_MENU = "Add to Map Window";
        public static readonly string REMOVE_FROM_MAP_WINDOW_MENU = "Remove From Map Window";

        public const string IMPORT_FROM_OLD_FDA = "Import Study From HEC-FDA 1.4.3";


        public static string ImportFromOldFda(string elementName)
        {
            return "Import " + elementName + " From HEC-FDA Version 1.4.3...";
        }

    }
}
