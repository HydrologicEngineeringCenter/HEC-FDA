using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    /// <summary>
    /// This is for use in the occtype editor. 
    /// </summary>
    public interface IOccupancyTypeEditable
    {
        //event EventHandler UpdateMessagesEvent;

        string Name { get; set; }
        int ID { get; set; }
        int GroupID { get; set; }
        string Description { get; set; }
        string DamageCategory { get; set; }
        bool IsModified { get; set; }

        ValueUncertaintyVM FoundationHeightUncertainty { get; set; }

        ObservableCollection<string> DamageCategoriesList { get; set; }
        /// <summary>
        /// If false, then this occtype was created during the editing process
        /// and has never been saved.
        /// </summary>
        //bool HasBeenSaved { get; }
        //FdaValidationResult HasWarnings();
        FdaValidationResult HasFatalErrors(List<string> occtypeNames);
        IOccupancyType CreateOccupancyType();
    }
}
