using System;
using System.Linq;
using StructureInventoryLibrary;

namespace HEC.FDA.ViewModel.Inventory
{
    //[Author(q0heccdm, 6 / 14 / 2017 4:00:20 PM)]
    public class StructureInventoryBaseElement : StructureInventoryBase
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/14/2017 4:00:20 PM
        #endregion
        #region Fields
        #endregion
        #region Properties
        public static string OccupancyTypeGroup = "OccTypeGroup";
        public static string GroundElevationField = "GroundElevation";
        public static string FirstFloorElevationField = "FirstFloorElevation";
        public static string YearField = "Year";
        public static string ModuleField = "Module";
        public static string fidField = "fid";
        public static string geomField = "geom";
        public static string damCatField = "damage_category";

        public string Description { get; set; }
        public string Name { get; set; }
        #endregion
        #region Constructors
        public StructureInventoryBaseElement(string name,string description, int id):
            base(Saving.PersistenceManagers.StructureInventoryPersistenceManager.STRUCTURE_INVENTORY_TABLE_CONSTANT + id)
        {
            Name = name;
            Description = description;
        }
        #endregion

        #region Interface methods
        public override void DeleteFromLookupTable()
        {           
        }

        public override IStructureBase[] GetStructureInventory()
        {
            throw new NotImplementedException();
        }

        public override string[] GetTableColumnNames()
        {
            return DataBaseView.ColumnNames;
        }

        public override Type[] GetTableColumnTypes()
        {
            return DataBaseView.ColumnTypes;
        }

        public override bool IsDataValid(ref string MessageOut)
        {
            throw new NotImplementedException();
        }

        public override ISerializeToSQLiteRow SaveAs(string newName)
        {
            throw new NotImplementedException();
        }

        public override void SaveToLookupTable()
        {
            throw new NotImplementedException();
        }

        protected override bool ContainsRequiredStructureAttributes(ref string errorMessage)
        {
            string[] columnNames = DataBaseView.ColumnNames;
            // below are all the required headers required from the base class
            if (!columnNames.Contains(OccupancyTypeField))
            {
                return false;
            }
            if (!columnNames.Contains(FoundationHeightField))
            {
                return false;
            }
            if (!columnNames.Contains(StructureValueField))
            {
                return false;
            }
            if (!columnNames.Contains(ContentValueField))
            {
                return false;
            }
            if (!columnNames.Contains(OtherValueField))
            {
                return false;
            }
            if (!columnNames.Contains(VehicleValueField))
            {
                return false;
            }
            // below are all the required headers required from this class
            if (!columnNames.Contains(GroundElevationField))
            {
                return false;
            }
            if (!columnNames.Contains(FirstFloorElevationField))
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}
