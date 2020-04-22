using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Saving.PersistenceManagers;
using FunctionsView.ViewModel;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    public class OccupancyTypeEditable : IOccupancyTypeEditable
    {
        public IOccupancyType OccType { get; set; }
        public ValueUncertaintyVM StructureValueUncertainty { get; set; }
        public ValueUncertaintyVM ContentValueUncertainty { get; set; }
        public ValueUncertaintyVM VehicleValueUncertainty { get; set; }
        public ValueUncertaintyVM OtherValueUncertainty { get; set; }
        public ValueUncertaintyVM FoundationHeightUncertainty { get; set; }
        public CoordinatesFunctionEditorVM StructureEditorVM { get; set; }
        public CoordinatesFunctionEditorVM ContentEditorVM { get; set; }
        public CoordinatesFunctionEditorVM VehicleEditorVM { get; set; }
        public CoordinatesFunctionEditorVM OtherEditorVM { get; set; }
        public bool IsModified
        {
            get { return OccType.IsModified; }
        }

        public OccupancyTypeEditable(IOccupancyType occtype)
        {
            //clone the occtype so that changes to it will not go into effect
            //unless the user saves.
            OccType = Saving.PersistenceFactory.GetOccTypeManager().CloneOccType(occtype);

            //create the curve editors
            string xLabel = "XLabel";
            string yLabel = "YLabel";
            string chartTitle = "ChartTitle";
            StructureEditorVM = new CoordinatesFunctionEditorVM(OccType.StructureDepthDamageFunction, xLabel, yLabel, chartTitle);
            ContentEditorVM = new CoordinatesFunctionEditorVM(OccType.ContentDepthDamageFunction, xLabel, yLabel, chartTitle);
            VehicleEditorVM = new CoordinatesFunctionEditorVM(OccType.VehicleDepthDamageFunction, xLabel, yLabel, chartTitle);
            OtherEditorVM = new CoordinatesFunctionEditorVM(OccType.OtherDepthDamageFunction, xLabel, yLabel, chartTitle);

            //create the value uncertainty vm's
            StructureValueUncertainty = new ValueUncertaintyVM(OccType.StructureValueUncertainty, OccType.StructureUncertaintyType);
            ContentValueUncertainty = new ValueUncertaintyVM(OccType.ContentValueUncertainty, OccType.ContentUncertaintyType);
            VehicleValueUncertainty = new ValueUncertaintyVM(OccType.VehicleValueUncertainty, OccType.VehicleUncertaintyType);
            OtherValueUncertainty = new ValueUncertaintyVM(OccType.OtherValueUncertainty, OccType.OtherUncertaintyType);


        }

    }
}
