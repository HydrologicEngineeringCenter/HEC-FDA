using FdaViewModel.Inventory.DamageCategory;
using Functions;
using FunctionsView.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    /// <summary>
    /// This is for use in the occtype editor. 
    /// </summary>
    public interface IOccupancyTypeEditable
    {
        IOccupancyType OccType { get; set; }

        ValueUncertaintyVM StructureValueUncertainty { get; set; }
        ValueUncertaintyVM ContentValueUncertainty { get; set; }
        ValueUncertaintyVM VehicleValueUncertainty { get; set; }
        ValueUncertaintyVM OtherValueUncertainty { get; set; }
        ValueUncertaintyVM FoundationHeightUncertainty { get; set; }

        CoordinatesFunctionEditorVM StructureEditorVM { get; set; }
        CoordinatesFunctionEditorVM ContentEditorVM { get; set; }
        CoordinatesFunctionEditorVM VehicleEditorVM { get; set; }
        CoordinatesFunctionEditorVM OtherEditorVM { get; set; }
        bool IsModified { get; }
    }
}
