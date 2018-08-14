using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using StructureInventoryLibrary;

namespace FdaViewModel.Inventory
{
    //[Author(q0heccdm, 6 / 14 / 2017 4:00:20 PM)]
    public class StructureInventoryBaseElement : StructureInventoryLibrary.StructureInventoryBase
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/14/2017 4:00:20 PM
        #endregion
        #region Fields

        #endregion
        #region Properties
        //public static string BldgTypeField = "BldgType";
        public static string OccupancyTypeGroupName = "OccTypeGroupName";
        public static string GroundElevationField = "GroundElevation";
        public static string FirstFloorElevationField = "FirstFloorElevation";
        public static string YearField = "Year";
        public static string ModuleField = "Module";

        //public string Path { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        #endregion
        #region Constructors
        public StructureInventoryBaseElement(string name,string description):base("Structure Inventory - "+name)
        {
            //Path = path;
            Name = name;
            Description = description;
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
        public override void DeleteFromLookupTable()
        {
            // The look up table is a table in the sqlite file that holds the names of your struc inventories
            
        }

        public override IStructureBase[] GetStructureInventory()
        {
            // this will be my computable object. It will be the entire SI with values that we can do math on.
            throw new NotImplementedException();
        }

        public override string[] GetTableColumnNames()
        {
            //Woody uses this to get the column names of the lookup table. I have written it to get the column names of the SI
            return DataBaseView.ColumnNames;
        }

        public override Type[] GetTableColumnTypes()
        {
            //Woody uses this to get the column types of the lookup table. I have written it to get the column types of the SI

            return DataBaseView.ColumnTypes;
        }

        public override bool IsDataValid(ref string MessageOut)
        {
            //first call "ContainsRequiredStructureAttributes"
            //loop through all rows and evaluate each cell to see if it has acceptable values. Mostly checking against negative values.
            throw new NotImplementedException();
        }

        public override ISerializeToSQLiteRow SaveAs(string newName)
        {
            //this function is copying an existing SI
            //first - geopackage copy features
            //add the new name to the lookup table. 
            //return the new structureinventorybaseElement
            throw new NotImplementedException();
        }

        public override void SaveToLookupTable()
        {
            //This will add the newly created SI name to the lookup table.
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
    }
}
