using FdaLogging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HEC.FDA.ViewModel.Inventory.DamageCategory;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    /// <summary>
    /// This is for use in the occtype editor. 
    /// </summary>
    public interface IOccupancyTypeEditable
    {
        event EventHandler UpdateMessagesEvent;

        string Name { get; set; }
        int ID { get; set; }
        int GroupID { get; set; }
        string Description { get; set; }
        string DamageCategory { get; set; }
        bool IsModified { get; set; }
        ValueUncertaintyVM StructureValueUncertainty { get; set; }
        ValueUncertaintyVM ContentValueUncertainty { get; set; }
        ValueUncertaintyVM VehicleValueUncertainty { get; set; }
        ValueUncertaintyVM OtherValueUncertainty { get; set; }
        ValueUncertaintyVM FoundationHeightUncertainty { get; set; }

        CoordinatesFunctionEditorVM StructureEditorVM { get; set; }
        CoordinatesFunctionEditorVM ContentEditorVM { get; set; }
        CoordinatesFunctionEditorVM VehicleEditorVM { get; set; }
        CoordinatesFunctionEditorVM OtherEditorVM { get; set; }

        bool CalculateStructureDamage { get; set; }
        bool CalculateContentDamage { get; set; }
        bool CalculateVehicleDamage { get; set; }
        bool CalculateOtherDamage { get; set; }

        ObservableCollection<string> DamageCategoriesList { get; set; }
        /// <summary>
        /// If false, then this occtype was created during the editing process
        /// and has never been saved.
        /// </summary>
        bool HasBeenSaved { get; }
        List<LogItem> SaveOcctype();
        IOccupancyType CreateOccupancyType(out List<LogItem> errors);


    }
}
