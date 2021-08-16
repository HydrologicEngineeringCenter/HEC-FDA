using FdaLogging;
using ViewModel.Inventory.DamageCategory;
using Functions;
using FunctionsView.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Inventory.OccupancyTypes
{
    /// <summary>
    /// This is for use in the occtype editor. 
    /// </summary>
    public interface IOccupancyTypeEditable
    {
        event EventHandler UpdateMessagesEvent;

        //IOccupancyType OccType { get; set; }
        string Name { get; set; }
        int ID { get; set; }
        int GroupID { get; set; }
        string Description { get; set; }
        IDamageCategory DamageCategory { get; set; }
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
        bool SaveWithReturnValue();
        IOccupancyType CreateOccupancyType(out List<LogItem> errors);


    }
}
