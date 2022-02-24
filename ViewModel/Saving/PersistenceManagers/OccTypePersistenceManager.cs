using FdaLogging;
using paireddata;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using HEC.FDA.ViewModel.Inventory.DamageCategory;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class OccTypePersistenceManager : SavingBase
    {
        private const string OCCTYPES_TABLE_NAME = "occupancy_types";
        //These are the columns for the parent table
        private const int PARENT_GROUP_ID_COL = 0;
        private const int PARENT_GROUP_NAME_COL = 1;
        private const int PARENT_IS_SELECTED_COL = 2;

        //These are the columns for the child table
        private const int GROUP_ID_COL = 0;
        private const int OCCTYPE_ID_COL = 1;

        private const int NAME_COL = 2;
        private const int DESC_COL = 3;
        private const int DAM_CAT_COL = 4;
        private const int FOUND_HT_UNCERTAINTY_TYPE_COL = 5;
        private const int FOUND_HT_UNCERTAINTY_COL = 6; 

        private const int IS_STRUCT_SELECTED_COL = 7;
        private const int VAR_STRUCT_TYPE_COL = 8;
        private const int VAR_STRUCT_VALUE_COL = 9;
        private const int STRUCT_CURVE_COL = 10;

        private const int IS_CONT_SELECTED_COL = 11;
        private const int VAR_CONT_TYPE_COL = 12;
        private const int VAR_CONT_VALUE_COL = 13;
        private const int CONTENT_CURVE_COL = 14;  

        private const int IS_VEH_SELECTED_COL = 15;
        private const int VAR_VEH_TYPE_COL = 16;
        private const int VAR_VEH_VALUE_COL = 17;
        private const int VEHICLE_CURVE_COL = 18;

        private const int IS_OTHER_SELECTED_COL = 19;
        private const int VAR_OTHER_TYPE_COL = 20;
        private const int VAR_OTHER_VALUE_COL = 21;
        private const int OTHER_CURVE_COL = 22;

        //ELEMENT_TYPE is used to store the type in the log tables. Initially i was actually storing the type
        //of the element. But since they get stored as strings if a developer changes the name of the class
        //you would no longer get any of the old logs. So i use this constant.
        private const string ELEMENT_TYPE = "OccType";
        private static readonly FdaLogger.FdaLogger LOGGER = new FdaLogger.FdaLogger("OccTypePersistenceManager");


        private const string GroupTablePrefix = "OccTypeGroup-";
        private const string ParentTableName = "occupancy_type_groups";

        private const string PARENT_NAME_FIELD = "Name";

        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return new Type[] { typeof(string), typeof(bool) }; }
        }
        internal override string ChangeTableConstant { get { return "OccType"; } }

        public override string TableName => ParentTableName;

        public override string[] TableColumnNames => new string[] { PARENT_NAME_FIELD, "IsSelected" };

        public OccTypePersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        public string[] OcctypeColumns
        {
            get
            {
                return new string[] {"GroupID","OcctypeID", "Name", "Description", "DamageCategory",
                    "FoundHtUncertaintyType", "FoundHtUncertainty",
                    "IsStructChecked", "StructValueType", "StructValueUncertainty", "StuctureFunction",
                    "IsContChecked", "ContValueType", "ContentValueUncertainty", "ContentFunction",
                    "IsVehicleChecked", "VehicleValueType", "VehicleValueUncertainty", "VehicleFunction",
                    "IsOtherChecked", "OtherValueType", "OtherValueUncertainty", "OtherFunction" };
            }
        }

        public Type[] OcctypeTypes
        {
            get
            {
                return new Type[] {
                    typeof(int), typeof(int), typeof(string), typeof(string), typeof(string), 
                    typeof(string), typeof(string),
                    typeof(bool), typeof(string), typeof(string), typeof(string),
                    typeof(bool), typeof(string), typeof(string), typeof(string),
                    typeof(bool), typeof(string), typeof(string), typeof(string),
                    typeof(bool), typeof(string), typeof(string), typeof(string) };
            }
        }

       
        /// <summary>
        /// Creates an element based off the row from the parent table
        /// </summary>
        /// <param name="rowData">row from parent table. Only one column of the group name</param>
        /// <returns></returns>
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            int groupId = (int)rowData[GROUP_ID_COL];
            int occtypId = (int)rowData[OCCTYPE_ID_COL];

            string name = (string)rowData[NAME_COL];
            string desc = (string)rowData[DESC_COL];
            string damCatName = (string)rowData[DAM_CAT_COL];

            string foundHtUncertaintyXML = (string)rowData[FOUND_HT_UNCERTAINTY_COL];
            string foundHtUncertaintyType = (string)rowData[FOUND_HT_UNCERTAINTY_TYPE_COL];

            //structures
            bool isStructTabChecked = Convert.ToBoolean(rowData[IS_STRUCT_SELECTED_COL]);
            string structValueType = (string)rowData[VAR_STRUCT_TYPE_COL];
            string structValueUncertaintyXML = (string)rowData[VAR_STRUCT_VALUE_COL];
            string structDepthDamageXML = (string)rowData[STRUCT_CURVE_COL];

            //content
            bool isContenTabChecked = Convert.ToBoolean(rowData[IS_CONT_SELECTED_COL]);
            string contValueType = (string)rowData[VAR_CONT_TYPE_COL];
            string contentValueUncertaintyXML = (string)rowData[VAR_CONT_VALUE_COL];
            string contentDepthDamageXML = (string)rowData[CONTENT_CURVE_COL];

            //vehicle
            bool isVehicleTabChecked = Convert.ToBoolean(rowData[IS_VEH_SELECTED_COL]);
            string vehicleValueType = (string)rowData[VAR_VEH_TYPE_COL];
            string vehicleValueUncertaintyXML = (string)rowData[VAR_VEH_VALUE_COL];
            string vehicleDepthDamageXML = (string)rowData[VEHICLE_CURVE_COL];

            //other
            bool isOtherTabChecked = Convert.ToBoolean(rowData[IS_OTHER_SELECTED_COL]);
            string otherValueType = (string)rowData[VAR_OTHER_TYPE_COL];
            string otherValueUncertaintyXML = (string)rowData[VAR_OTHER_VALUE_COL];
            string otherDepthDamageXML = (string)rowData[OTHER_CURVE_COL];


            Dictionary<string, bool[]> selectedTabsDictionary = new Dictionary<string, bool[]>();
            List<IOccupancyType> listOfOccTypes = new List<IOccupancyType>();
            List<IOccupancyType> TempOccTypes = new List<IOccupancyType>();
            string groupName = rowData[PARENT_GROUP_NAME_COL].ToString();
            //create an empty element. Then loop through all the rows in the table to add the actual occtypes for this elem.

            int id = Convert.ToInt32( rowData[ID_COL]);
            OccupancyTypesElement ele = new OccupancyTypesElement(groupName, groupId, TempOccTypes, id);
            //ele.IsSelected = (bool)rowData[PARENT_IS_SELECTED_COL];
            //string elementTableName = GroupTablePrefix + groupName;

            //if (Storage.Connection.Instance.TableNames().Contains(elementTableName))
            //{
            //    DatabaseManager.DataTableView elementTable = Storage.Connection.Instance.GetTable(elementTableName);
            //    List<object[]> occTypeRows = elementTable.GetRows(0, elementTable.NumberOfRows - 1);

            //foreach (object[] row in occTypeRows)//Storage.Connection.Instance.GetTable(tableName).GetRows(0, lastRow))
            //{
            //        testing++;
            //    bool[] selectedTabs = new bool[] { Convert.ToBoolean(row[IS_STRUCT_SELECTED_COL]), Convert.ToBoolean(row[IS_CONT_SELECTED_COL]), Convert.ToBoolean(row[IS_VEH_SELECTED_COL]), Convert.ToBoolean(row[IS_OTHER_SELECTED_COL]) };
            //    selectedTabsDictionary.Add(row[NAME_COL].ToString(), selectedTabs);
            //    //ele.RelativePathAndProbability.Add(new PathAndProbability(row[0].ToString(), Convert.ToDouble(row[1])));
            //    IOccupancyType ot = new OccupancyType();
            //    ot.Name = row[NAME_COL].ToString();
            //    ot.Description = row[DESC_COL].ToString();
            //    ot.DamageCategory.Name = row[DAM_CAT_COL].ToString();

            //    ot.FoundationHeightUncertaintyFunction = CreateContinuousDistributionFromRow(row, VAR_FOUND_HT_COL, FOUND_HT_STDEV_COL);

            //    //***************************
            //    //structures
            //    //*****************************

            //    // if(Convert.ToBoolean(row[7]) == true) // if structures tab is checked
                
            //        ot.StructureValueUncertainty = CreateContinuousDistributionFromRow(row, VAR_STRUCT_VALUE_COL, STRUCT_STDEV_COL);

            //        if (row[STRUCT_DIST_TYPE_COL].ToString() == "Normal")
            //        {
            //            Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetNormalDistributionFromXML(row[STRUCT_CURVE_COL].ToString());
            //            ot.StructureDepthDamageFunction = uci;
            //        }
            //        else if (row[STRUCT_DIST_TYPE_COL].ToString() == "None")
            //        {
            //            Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetNoneDistributionFromXML(row[STRUCT_CURVE_COL].ToString());
            //            ot.StructureDepthDamageFunction = uci;
            //        }
            //        else if (row[STRUCT_DIST_TYPE_COL].ToString() == "Triangular")
            //        {
            //            Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetTriangularDistributionFromXML(row[STRUCT_CURVE_COL].ToString());
            //            ot.StructureDepthDamageFunction = uci;
            //        }
            //        else if (row[STRUCT_DIST_TYPE_COL].ToString() == "Uniform")
            //        {
            //            Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetUniformDistributionFromXML(row[STRUCT_CURVE_COL].ToString());
            //            ot.StructureDepthDamageFunction = uci;
            //        }

                



            //        //*****************************
            //        //content
            //        //*****************************

            //        //if (Convert.ToBoolean(row[13]) == true) // if content tab is checked
            //        //14,17
            //        ot.ContentValueUncertainty = CreateContinuousDistributionFromRow(row, VAR_CONT_VALUE_COL, CONT_STDEV_COL);
            //        if (row[CONT_DIST_TYPE_COL].ToString() == "Normal")
            //        {
            //            Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetNormalDistributionFromXML(row[CONTENT_CURVE_COL].ToString());
            //            ot.ContentDepthDamageFunction = uci;
            //        }
            //        else if (row[CONT_DIST_TYPE_COL].ToString() == "None")
            //        {
            //            Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetNoneDistributionFromXML(row[CONTENT_CURVE_COL].ToString());
            //            ot.ContentDepthDamageFunction = uci;
            //        }
            //        else if (row[CONT_DIST_TYPE_COL].ToString() == "Triangular")
            //        {
            //            Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetTriangularDistributionFromXML(row[CONTENT_CURVE_COL].ToString());
            //            ot.ContentDepthDamageFunction = uci;
            //        }
            //        else if (row[CONT_DIST_TYPE_COL].ToString() == "Uniform")
            //        {
            //            Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetUniformDistributionFromXML(row[CONTENT_CURVE_COL].ToString());
            //            ot.ContentDepthDamageFunction = uci;
            //        }





            //        //*****************************
            //        //vehicle
            //        //*****************************

            //        //if (Convert.ToBoolean(row[19]) == true) // if vehicle tab is checked
                    
            //        //20,23
            //        ot.VehicleValueUncertainty = CreateContinuousDistributionFromRow(row, VAR_VEH_VALUE_COL, VEH_STDEV_COL);
            //        if (row[VEH_DIST_TYPE_COL].ToString() == "Normal")
            //        {
            //            Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetNormalDistributionFromXML(row[VEHICLE_CURVE_COL].ToString());
            //            ot.VehicleDepthDamageFunction = uci;
            //        }
            //        else if (row[VEH_DIST_TYPE_COL].ToString() == "None")
            //        {
            //            Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetNoneDistributionFromXML(row[VEHICLE_CURVE_COL].ToString());
            //            ot.VehicleDepthDamageFunction = uci;
            //        }
            //        else if (row[VEH_DIST_TYPE_COL].ToString() == "Triangular")
            //        {
            //            Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetTriangularDistributionFromXML(row[VEHICLE_CURVE_COL].ToString());
            //            ot.VehicleDepthDamageFunction = uci;
            //        }
            //        else if (row[VEH_DIST_TYPE_COL].ToString() == "Uniform")
            //        {
            //            Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetUniformDistributionFromXML(row[VEHICLE_CURVE_COL].ToString());
            //            ot.VehicleDepthDamageFunction = uci;
            //        }




            //        //*****************************
            //        //Other
            //        //*****************************

            //        //if (Convert.ToBoolean(row[25]) == true) // if other tab is checked
            //        //26,29
            //        ot.OtherValueUncertainty = CreateContinuousDistributionFromRow(row, VAR_OTHER_VALUE_COL, OTHER_STDEV_COL);
            //        if (row[OTHER_DIST_TYPE_COL].ToString() == "Normal")
            //        {
            //            Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetNormalDistributionFromXML(row[OTHER_CURVE_COL].ToString());
            //            ot.OtherDepthDamageFunction = uci;
            //        }
            //        else if (row[OTHER_DIST_TYPE_COL].ToString() == "None")
            //        {
            //            Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetNoneDistributionFromXML(row[OTHER_CURVE_COL].ToString());
            //            ot.OtherDepthDamageFunction = uci;
            //        }
            //        else if (row[OTHER_DIST_TYPE_COL].ToString() == "Triangular")
            //        {
            //            Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetTriangularDistributionFromXML(row[OTHER_CURVE_COL].ToString());
            //            ot.OtherDepthDamageFunction = uci;
            //        }
            //        else if (row[OTHER_DIST_TYPE_COL].ToString() == "Uniform")
            //        {
            //            Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetUniformDistributionFromXML(row[OTHER_CURVE_COL].ToString());
            //            ot.OtherDepthDamageFunction = uci;
            //        }



            //        listOfOccTypes.Add(ot);
            //}
            //ele.ListOfOccupancyTypes = listOfOccTypes;
            //ele.OccTypesSelectedTabsDictionary = selectedTabsDictionary;
            ////OccupancyTypesOwnerElement.ListOfOccupancyTypesGroups.Add(ele);
            //}
            return ele;
                //AddElement(ele,false);
        }


        private List<DataRow> GetParentTableRows()
        {
            List<DataRow> retval = new List<DataRow>();
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                if (!Storage.Connection.Instance.IsOpen)
                {
                    Storage.Connection.Instance.Open();
                }
                if (Storage.Connection.Instance.TableNames().Contains(ParentTableName))
                {

                    System.Data.DataTable table = Storage.Connection.Instance.GetDataTable(ParentTableName);
                    foreach(DataRow row in table.Rows)
                    {
                        retval.Add(row);
                    }
                }
            }
            return retval;
        }

        public override void Load()
        {
            List<ChildElement> occTypeGroupsToReturn = new List<ChildElement>();
            
            //each row in the parent table will be an occtype element (same as occtype group)
            //if (!Storage.Connection.Instance.IsConnectionNull)
            //{
                //if (!Storage.Connection.Instance.IsOpen)
                //{
                //    Storage.Connection.Instance.Open();
                //}
                //if (Storage.Connection.Instance.TableNames().Contains(ParentTableName))
                //{

                    //System.Data.DataTable table = Storage.Connection.Instance.GetDataTable(ParentTableName);

                    foreach (DataRow row in GetParentTableRows())
                    {
                        //each of these is a group
                        int groupId = Convert.ToInt32(row[PARENT_GROUP_ID_COL]);
                        string groupName = (string)row[PARENT_GROUP_NAME_COL];
                        bool isGroupSelected = Convert.ToBoolean(row[PARENT_IS_SELECTED_COL]);
                        int id = Convert.ToInt32(row[ID_COL]);

                        //now read the child table and grab all the occtypes with this group id
                        List<IOccupancyType> occtypes = LoadOcctypes(groupId);
                        OccupancyTypesElement elem = new OccupancyTypesElement( groupName,groupId, occtypes, id);
                        occTypeGroupsToReturn.Add(elem);
                    }



                    //        DatabaseManager.DataTableView dtv = Storage.Connection.Instance.GetTable(ParentTableName);
                    //List<object[]> rowData = dtv.GetRows(0, dtv.NumberOfRows - 1);

                    ////go over each row in the parent table and create an occtype element
                    //for (int i = 0; i < dtv.NumberOfRows; i++)
                    //{
                    //    //OccupancyTypesElement ele = new OccupancyTypesElement(groupName, TempOccTypes, dummyDictionary);

                    //        OccupancyTypesElement ele = (OccupancyTypesElement)CreateElementFromRowData(dtv.GetRow(i));
                    //        occTypeGroupsToReturn.Add(ele);




                    //}
               // }
           // }
            foreach (Inventory.OccupancyTypes.OccupancyTypesElement elem in occTypeGroupsToReturn)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public void Remove(ChildElement element)
        {
        //    if (!Storage.Connection.Instance.IsOpen)
        //    {
        //        Storage.Connection.Instance.Open();
        //    }
        //    //remove row from parent table
        //    DatabaseManager.DataTableView dtv = Storage.Connection.Instance.GetTable(ParentTableName);
        //    int index = -1;
        //    for (int i = 0; i < dtv.NumberOfRows; i++)
        //    {
        //        if (dtv.GetRow(i)[PARENT_GROUP_NAME_COL].ToString().Equals(element.Name))
        //        {
        //            index = i;
        //            break;
        //        }
        //    }
        //    if (index == -1) { return; }//throw exception?
        //    dtv.DeleteRow(index);
        //    dtv.ApplyEdits();
        //    //remove the associated table
        //    string elementTableName = GroupTablePrefix + element.Name;
        //    if (Storage.Connection.Instance.TableNames().Contains(elementTableName))
        //    {
        //        Storage.Connection.Instance.DeleteTable(elementTableName);
        //    }
        //    //remove from the study cache
        //    StudyCacheForSaving.RemoveElement((OccupancyTypesElement)element);
        }

        public void DeleteOcctypeGroup(int groupID)
        {
            //delete the row from the parent table
            DeleteRowWithKey(TableName, groupID, "ID");

            //delete all the occtypes associated with this group from the occtypes table
            //this one call will delete all the rows with that group id
            DeleteRowWithKey(OCCTYPES_TABLE_NAME, groupID, "GroupID");

            //remove from the study cache
            RemoveElementFromCache(groupID); 
        }

        /// <summary>
        /// Removes the specified group from the study cache. This is done when saving the 
        /// occupancy types editor. 
        /// </summary>
        /// <param name="group"></param>
        private void RemoveElementFromCache(int groupID)
        {
            List<OccupancyTypesElement> elems = StudyCacheForSaving.OccTypeElements;
            int indexToRemove = -1;
            for (int i = 0; i < elems.Count(); i++)
            {
                if (elems[i].ID == groupID)
                {
                    indexToRemove = i;
                    break;
                }
            }
            if (indexToRemove != -1)
            {
                StudyCacheForSaving.RemoveElement(elems[indexToRemove]);
            }
        }

        public void DeleteOcctype(IOccupancyTypeEditable occtypeToDelete)
        {
            //only update the db if this occtype is actually in there.
            //if the occtype has never been saved then there is nothing to remove.
            if (occtypeToDelete.HasBeenSaved)
            {
                int[] keys = new int[] { occtypeToDelete.GroupID, occtypeToDelete.ID };
                string[] keyColNames = new string[] { "GroupID", "OcctypeID" };

                DeleteRowWithCompoundKey(OCCTYPES_TABLE_NAME, keys, keyColNames);
                DeleteOccTypeFromGroupInCache(occtypeToDelete);
                  
            }
        }

        private void DeleteOccTypeFromGroupInCache(IOccupancyTypeEditable ot)
        {
            OccupancyTypesElement group = GetElementFromGroupID(ot.GroupID);
            if (group == null)
            {
                return;
            }
            else
            {
                foreach (IOccupancyType occtype in group.ListOfOccupancyTypes)
                { 
                    if(occtype.ID == ot.ID)
                    {
                        group.ListOfOccupancyTypes.Remove(occtype);
                        return;
                    }
                }
                //call the update in the study cache so that the occtype element in 
                //the occtype owner element gets updated.
            }
        }
        //public void DeleteOcctypes(List<IOccupancyTypeEditable> occtypesToDelete)
        //{
        //    foreach (IOccupancyTypeEditable ot in occtypesToDelete)
        //    {
        //        //only update the db if this occtype is actually in there.
        //        //if the occtype has never been saved then there is nothing to remove.
        //        if (ot.HasBeenSaved)
        //        {
        //            int[] keys = new int[] { ot.GroupID, ot.ID };
        //            string[] keyColNames = new string[] { "GroupID", "OcctypeID" };

        //            DeleteRowWithCompoundKey(OCCTYPES_TABLE_NAME, keys, keyColNames);
        //        }
        //    }
        //}

        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex)
        {
            Remove(oldElement);
            SaveNew(elementToSave);
        }
        /// <summary>
        /// I don't love the way this works. Not really efficient. I delete ALL the occtype tables
        /// and remove all the elements form the study cache and then i re-save and re-add the elements back.
        /// </summary>
        /// <param name="elements"></param>
        public void SaveExisting(List<ChildElement> elements)
        {
            //OccupancyTypesOwnerElement owner = StudyCacheForSaving.GetParentElementOfType<OccupancyTypesOwnerElement>();
            ////because i am lumping all the elements together in one editor, then it is difficult to keep track of old names vs new names vs adding new ones etc.
            ////i think it is best to just delete all previous tables (all rows in parent table and all individual tables) and then resave everything.
            //if (!Storage.Connection.Instance.IsOpen)
            //{
            //    Storage.Connection.Instance.Open();
            //}
            //DeleteAllOccTypeTables();
            //RemoveAllOccTypeElementsFromTheStudyCache();

            ////save the occtypes
            ////foreach (OccupancyTypesElement element in elements)
            ////{
            //    SaveElementsOnBackGroundThread(elements, owner, (elem) => SavingAction(elem), " - Saving");

            //  //  SaveNew(element);
            ////}
           
        }


        public void SaveNewOcctypes(List<IOccupancyType> newOcctypes, string groupName)
        {
            //i need a group name
            //create the element
            //save it

            //DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(OCCTYPES_TABLE_NAME);
            //if (tbl == null)
            //{
            //    Storage.Connection.Instance.CreateTable(OCCTYPES_TABLE_NAME, OcctypeColumns, OcctypeTypes);
            //    tbl = Storage.Connection.Instance.GetTable(OCCTYPES_TABLE_NAME);
            //}

            //List<object[]> rows = new List<object[]>();

            //foreach (IOccupancyType ot in newOcctypes)
            //{
            //    //when this occtype was created it should have gotten a unique id of its own.
            //    rows.Add(GetOccTypeRowForOccTypesTable(ot.GroupID, ot.ID, ot).ToArray());
            //}
            //tbl.AddRows(rows);
            //tbl.ApplyEdits();


            int newGroupID = PersistenceFactory.GetOccTypeManager().GetUnusedId();
            int id = PersistenceFactory.GetOccTypeManager().GetNextAvailableId(); 
            OccupancyTypesElement elem = new OccupancyTypesElement(groupName, newGroupID, newOcctypes, id);
            SaveNew(elem);
        }

        public void SaveModifiedGroups(List<IOccupancyTypeGroupEditable> groups)
        {
            //these get saved to the parent table.
            foreach (IOccupancyTypeGroupEditable group in groups)
            {
                string[] columnsToUpdate = new string[] { PARENT_NAME_FIELD };
                object[] newValues = new object[] { group.Name };
                UpdateTableRow(ParentTableName, group.ID, "ID", columnsToUpdate, newValues);
            }
      
        }

        //public void UpdateOccTypeInStudyCache(IOccupancyTypeGroupEditable group, IOccupancyTypeEditable ot)
        //{
        //    //find the occtype group

        //    List<OccupancyTypesElement> elems = StudyCacheForSaving.OccTypeElements;
        //    int index = -1;
        //    for (int i = 0; i < elems.Count(); i++)
        //    {
        //        if (elems[i].ID == group.ID)
        //        {
        //            index = i;
        //            break;
        //        }
        //    }
        //    if (index != -1)
        //    {
        //        StudyCacheForSaving.RemoveElement
        //        //update the occtype
        //        UpdateOccType(elems[index], ot.OccType);
        //    }
        //}

        private OccupancyTypesElement GetElementFromGroupID(int groupId)
        {
            List<OccupancyTypesElement> occtypeElems = StudyCacheForSaving.OccTypeElements;
            for (int i = 0; i < occtypeElems.Count; i++)
            {
                if (occtypeElems[i].ID == groupId)
                {
                    return occtypeElems[i];
                }
            }
            return null;
        }

        private void AddNewOccTypeToCache(IOccupancyType ot)
        {
            //Because the study cache and the occtype owner element are hanging on to
            //the same object. All i have to do is replace the occtype in the list of 
            //occtypes and it will show up in all places. There is no need to remove
            //a group and add a new one.
            OccupancyTypesElement group = GetElementFromGroupID(ot.GroupID);
            if (group == null)
            {
                return;
            }
            else
            {
                group.ListOfOccupancyTypes.Add(ot);
                //call the update in the study cache so that the occtype element in 
                //the occtype owner element gets updated.
            }
        }
        private void UpdateOccTypeInCache(IOccupancyType ot)
        {
            //Because the study cache and the occtype owner element are hanging on to
            //the same object. All i have to do is replace the occtype in the list of 
            //occtypes and it will show up in all places. There is no need to remove
            //a group and add a new one.

            OccupancyTypesElement group = GetElementFromGroupID(ot.GroupID);
            string oldName = "";
            string newName = ot.Name;
            if (group != null)
            {
                //now replace the occtype with the new one
                for (int i = 0; i < group.ListOfOccupancyTypes.Count; i++)
                {
                    if (group.ListOfOccupancyTypes[i].ID == ot.ID)
                    {
                        oldName = group.ListOfOccupancyTypes[i].Name;
                        group.ListOfOccupancyTypes[i] = ot;
                        break;
                    }
                }
            }

            if(!oldName.Equals(newName))
            {
                //update the structure inventory occtype names.
                PersistenceFactory.GetStructureInventoryManager().UpdateOccTypeNames(group.Name, group.Name, oldName, newName);
            }

        }

        /// <summary>
        /// This removes all the matching elements from the study cache and replaces it with a new
        /// element that is created from the group. This is done when saving from the occupancy type editor.
        /// </summary>
        /// <param name="groupsToUpdateInCache"></param>
        //public void UpdateOccTypeGroupsInStudyCache(List<IOccupancyTypeGroup> groupsToUpdateInCache)
        //{
        //    foreach(IOccupancyTypeGroup group in groupsToUpdateInCache)
        //    {
        //        RemoveElementFromCache(group.ID);

        //        //create new element to add to the cache
        //        List<IOccupancyType> occtypes = new List<IOccupancyType>();
        //        foreach(IOccupancyType ot in group.OccupancyTypes)
        //        {
        //            occtypes.Add(ot);
        //        }
        //        OccupancyTypesElement elem = new OccupancyTypesElement(group.Name, group.ID, occtypes);
        //        StudyCacheForSaving.AddElement(elem);
        //    }
        //}

        public void SaveModifiedOcctypes(List<IOccupancyType> occtypes)
        {
            foreach(IOccupancyType ot in occtypes)
            {
                SaveModifiedOcctype(ot);
            }
        }

        /// <summary>
        /// Used when creating new occtypeElements. Gets the first unused ID for occtypeGroups so that the in memory version 
        /// knows what it's id is.
        /// </summary>
        /// <returns></returns>
        public int GetUnusedId()
        {
            List<DataRow> rows = GetParentTableRows();
            List<int> idNums = new List<int>();
            foreach(DataRow row in rows)
            {
                idNums.Add(Convert.ToInt32(row[PARENT_GROUP_ID_COL]));
            }
            //now we have all the id's in the parent table
            if (idNums.Count == 0)
            {
                return 1;
            }
            else
            {
                return idNums.Max() + 1;
            }
        }

        public void SaveModifiedOcctype(IOccupancyType ot)
        {
            object[] keys = new object[] { ot.GroupID, ot.ID };
            string[] keyColNames = new string[] { "GroupID", "OcctypeID" };

            //update the whole row
            string[] columnsToUpdate = OcctypeColumns;
            object[] newValues = GetOccTypeRowForOccTypesTable(ot.GroupID, ot.ID, ot).ToArray();

            UpdateTableRowWithCompoundKey(OCCTYPES_TABLE_NAME, keys, keyColNames, columnsToUpdate, newValues);
            UpdateOccTypeInCache(ot);
        }

        public void SaveNewOccType(IOccupancyType ot)
        {
            DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(OCCTYPES_TABLE_NAME);
            if (tbl == null)
            {
                Storage.Connection.Instance.CreateTable(OCCTYPES_TABLE_NAME, OcctypeColumns, OcctypeTypes);
                tbl = Storage.Connection.Instance.GetTable(OCCTYPES_TABLE_NAME);
            }

            int[] keys = new int[] { ot.GroupID, ot.ID };
            string[] keyColNames = new string[] { "GroupID", "OcctypeID" };

            //update the whole row
            string[] columnsToUpdate = OcctypeColumns;
            object[] newValues = GetOccTypeRowForOccTypesTable(ot.GroupID, ot.ID, ot).ToArray();
            Storage.Connection.Instance.AddRowToTableWithPrimaryKey(newValues, OCCTYPES_TABLE_NAME, OcctypeColumns);

            AddNewOccTypeToCache(ot);
        }

        /// <summary>
        /// Looks at all the current occtypes in this group and returns the max ID plus 1.
        /// </summary>
        /// <param name="groupId"></param>
        public int GetIdForNewOccType(int groupId)
        {
            List<IOccupancyType> occtypes = LoadOcctypes(groupId);
            List<int> occtypeIds = new List<int>();
            foreach(IOccupancyType ot in occtypes)
            {
                occtypeIds.Add(ot.ID);
            }
            if(occtypeIds.Count>0)
            {
                return occtypeIds.Max() + 1;
            }
            else
            {
                return 1;
            }
        }

        //private void RemoveAllOccTypeElementsFromTheStudyCache()
        //{
        //    List<OccupancyTypesElement> elems = StudyCache.GetChildElementsOfType<OccupancyTypesElement>();
        //    foreach (OccupancyTypesElement elem in elems)
        //    {
        //        StudyCacheForSaving.RemoveElement(elem);
        //    }
        //}
        //private void DeleteAllOccTypeTables()
        //{
        //    if (Storage.Connection.Instance.TableNames().Contains(ParentTableName))
        //    {
        //        DatabaseManager.DataTableView dtv = Storage.Connection.Instance.GetTable(ParentTableName);
        //        foreach (object[] row in dtv.GetRows(0, dtv.NumberOfRows - 1))
        //        {
        //            //delete the table with the name of row[0]
        //            string elementTableName = GroupTablePrefix + row[0].ToString();
        //            if (Storage.Connection.Instance.TableNames().Contains(elementTableName))
        //            {
        //                Storage.Connection.Instance.DeleteTable(elementTableName);
        //            }
        //        }
        //        //finally delete the parent table itself
        //        Storage.Connection.Instance.DeleteTable(ParentTableName);
        //    }
        //}


        //public void SaveNewOccTypeGroups(List<OccupancyTypesElement> elements)
        //{
        //    //make sure the parent table exists
        //    if (!Storage.Connection.Instance.IsOpen)
        //    {
        //        Storage.Connection.Instance.Open();
        //    }
        //    if (!Storage.Connection.Instance.TableNames().Contains(ParentTableName))
        //    {
        //        //already exists... delete?
        //        string[] colNames = new string[] { "GroupName" };
        //        Type[] colTypes = new Type[] { typeof(String) };
        //        Storage.Connection.Instance.CreateTable(ParentTableName, colNames,colTypes);
        //    }
        //    //add each element to the parent table
        //    //save each element to its own table
        //    foreach (OccupancyTypesElement elem in elements)
        //    {
        //        DataBase_Reader.DataTableView dtv = Storage.Connection.Instance.GetTable(ParentTableName);
        //        object[] rowForParentTable = new object[] { elem.Name };
        //        dtv.AddRow(rowForParentTable);
        //        dtv.ApplyEdits();
        //        SaveNew(elem);
        //    }
        //}

        //private async void SaveOnBackgroundThread(OccupancyTypesElement element)
        //{
        //    OccupancyTypesOwnerElement owner = StudyCacheForSaving.GetParentElementOfType<OccupancyTypesOwnerElement>();
        //        owner.CustomTreeViewHeader = new CustomHeaderVM(owner.Name,"", " -saving", true);


        //    await Task.Run(() =>
        //    {
        //        if (!Storage.Connection.Instance.IsConnectionNull)
        //        {
        //            if (!Storage.Connection.Instance.IsOpen)
        //            {
        //                Storage.Connection.Instance.Open();
        //            }
        //            System.Threading.Thread.Sleep(5000);
        //            SaveGroupToParentTable(element.Name);

        //            SaveNewGroupTable(element);

        //            //add element to cache
        //            StudyCacheForSaving.AddOccTypesElement(element);
        //            owner.CustomTreeViewHeader = new CustomHeaderVM(owner.Name);


        //        }
        //    });

        //}

        #region import from old fda
        //public void SaveFDA1Elements(OccupancyTypeList ots, string groupName)
        //{
        //    List<IOccupancyType> fda2Occtypes = new List<IOccupancyType>();
        //    foreach (Importer.OccupancyType ot in ots.Occtypes)
        //    {
        //        fda2Occtypes.Add(GetFDA2OccupancyType(ot));
        //    }
        //    SaveNewOcctypes(fda2Occtypes, groupName);
        //    //OccupancyTypes = fda2Occtypes;
        //}

        #endregion
        public void SaveNew(ChildElement element)
        {
            //OccupancyTypesOwnerElement owner = StudyCacheForSaving.GetParentElementOfType<OccupancyTypesOwnerElement>();

            //SaveElementOnBackGroundThread(element, owner, (elem) => SavingAction(elem), " - Saving");
            SavingAction(element);
            //OccupancyTypesOwnerElement owner = StudyCacheForSaving.GetParentElementOfType<OccupancyTypesOwnerElement>();

           // //clear the actions while it is saving
           // List<NamedAction> actions = new List<NamedAction>();
           // foreach (NamedAction act in owner.Actions)
           // {
           //     actions.Add(act);
           // }
           // owner.Actions = new List<NamedAction>();
           // SaveOnBackgroundThread((OccupancyTypesElement)element);
           // //restore the actions
           //// owner.Actions = actions;
        }

        /// <summary>
        /// Saves a newly created occtype group from the occtype editor.
        /// </summary>
        /// <param name="newGroupFromOccTypeEditor"></param>
        //public List<LogItem> SaveNew(IOccupancyTypeGroupEditable newGroupFromOccTypeEditor)
        //{
        //    int newGroupID = GetUnusedId();
        //    List<IOccupancyType> occtypes = new List<IOccupancyType>();
        //    List<LogItem> errors = new List<LogItem>();
        //    foreach(IOccupancyTypeEditable otEdit in newGroupFromOccTypeEditor.Occtypes)
        //    {
        //        IOccupancyType occType = otEdit.CreateOccupancyType(out errors);
        //        if(occType != null)
        //        {
        //            occtypes.Add(occType);
        //        }
        //    }
        //    OccupancyTypesElement elem = new OccupancyTypesElement(Name, newGroupID, occtypes);
        //    SaveNew(elem);
        //    return errors;
        //}

        public void SaveNewElements(List<ChildElement> elements)
        {
            OccupancyTypesOwnerElement owner = StudyCacheForSaving.GetParentElementOfType<OccupancyTypesOwnerElement>();
            //SaveElementsOnBackGroundThread(elements, owner, (elem) => SavingAction(elem), " - Saving");
            foreach (ChildElement elem in elements)
            {
                SavingAction(elem);
            }
        }

        private void SavingAction(ChildElement element)
        {
            //this will save to the parent table
            //and add the element to the study cache
            base.SaveNew(element);

            //save to the child table
            SaveNewToOcctypesTable(element);

            //SaveGroupToParentTable(element.Name, ((OccupancyTypesElement)element).IsSelected);
            //SaveNewGroupTable((OccupancyTypesElement)element);
            //StudyCacheForSaving.AddElement((OccupancyTypesElement)element);

        }

        public int GetGroupId(string groupName)
        {
            return GetElementId(ParentTableName, groupName);
        }

        public void SaveNewToOcctypesTable(ChildElement element)
        {
            //we should have already saved the element to the parent table so that we can grab the id from that table
            int elemId = GetElementId(TableName, element.Name);
            DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(OCCTYPES_TABLE_NAME);
            if (tbl == null)
            {
                Storage.Connection.Instance.CreateTable(OCCTYPES_TABLE_NAME, OcctypeColumns, OcctypeTypes);
                tbl = Storage.Connection.Instance.GetTable(OCCTYPES_TABLE_NAME);
            }

            List<IOccupancyType> ListOfOccupancyTypes = ((OccupancyTypesElement)element).ListOfOccupancyTypes;
            //Dictionary<string, bool[]> OccTypesSelectedTabsDictionary = ((OccupancyTypesElement)element).OccTypesSelectedTabsDictionary;
            string groupName = element.Name;

            List<object[]> rows = new List<object[]>();

            int i = 1;
            foreach (IOccupancyType ot in ListOfOccupancyTypes)
            {

                rows.Add(GetOccTypeRowForOccTypesTable(elemId,i, ot).ToArray());
                i++;
            }
            tbl.AddRows(rows);
            tbl.ApplyEdits();

        }

        private void SaveGroupToParentTable(string groupName, bool isSelected)
        {
            //make sure there is a parent table to save this element to also
            if (!Storage.Connection.Instance.TableNames().Contains(ParentTableName))
            {
                string[] parentColNames = new string[] { "GroupName", "IsSelected" };
                Type[] parentColTypes = new Type[] { typeof(String), typeof(bool) };
                Storage.Connection.Instance.CreateTable(ParentTableName, parentColNames, parentColTypes);
            }
            DatabaseManager.DataTableView dtv = Storage.Connection.Instance.GetTable(ParentTableName);
            dtv.AddRow(new object[] { groupName, isSelected });
            dtv.ApplyEdits();
        }

        //private void SaveNewGroupTable(OccupancyTypesElement element)
        //{
        //    string groupName = element.Name;
        //    List<IOccupancyType> ListOfOccupancyTypes = ((OccupancyTypesElement)element).ListOfOccupancyTypes;
        //    Dictionary<string, bool[]> OccTypesSelectedTabsDictionary = ((OccupancyTypesElement)element).OccTypesSelectedTabsDictionary;


        //    string[] colNames = new string[] { "Name", "Description", "DamageCategory","FoundHtUncertainty",
        //            "IsStructChecked","StructValueUncertainty", "StuctureFunction",
        //            "IsContChecked", "ContentValueUncertainty", "ContentFunction",
        //            "IsVehicleChecked", "VehicleValueUncertainty", "VehicleFunction",
        //            "IsOtherChecked", "OtherValueUncertainty", "OtherFunction" };


        //    Type[] colTypes = new Type[] {
        //            typeof(string), typeof(string), typeof(string), typeof(string),
        //            typeof(bool), typeof(string), typeof(string),
        //            typeof(bool), typeof(string), typeof(string),
        //            typeof(bool), typeof(string), typeof(string),
        //            typeof(bool), typeof(string), typeof(string) };



        //    Storage.Connection.Instance.CreateTable(GroupTablePrefix + groupName, colNames, colTypes);
        //    DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(GroupTablePrefix + groupName);

        //    List<object[]> rows = new List<object[]>();

        //    foreach (IOccupancyType ot in ListOfOccupancyTypes)
        //    {

        //        rows.Add(GetOccTypeRowForParentTable(ot, OccTypesSelectedTabsDictionary, groupName).ToArray());
        //    }
        //    tbl.AddRows(rows);
        //    tbl.ApplyEdits();
        //}

        private List<IOccupancyType> LoadOcctypes(int groupId)
        {
            List<IOccupancyType> occtypes = new List<IOccupancyType>();
            if (Storage.Connection.Instance.TableNames().Contains(OCCTYPES_TABLE_NAME))
            {
                System.Data.DataTable table = Storage.Connection.Instance.GetDataTable(OCCTYPES_TABLE_NAME);

                foreach (System.Data.DataRow row in table.Rows)
                {
                    if (Convert.ToInt32(row[GROUP_ID_COL]) == groupId)
                    {
                        occtypes.Add(CreateOcctypeFromRow(row.ItemArray));
                    }
                }
            }
            return occtypes;
        }

        private ValueUncertaintyType GetValueUncertaintyType(string value)
        {
            if (value == ValueUncertaintyType.PercentOfMean.ToString())
            {
                return ValueUncertaintyType.PercentOfMean;
            }
            else if (value == ValueUncertaintyType.DeviationFromMean.ToString())
            {
                return ValueUncertaintyType.DeviationFromMean;
            }
            else if (value == ValueUncertaintyType.Actual.ToString())
            {
                return ValueUncertaintyType.Actual;
            }
            throw new NotImplementedException("The value was of an unknown type: " + value);
        }

        private IOccupancyType CreateOcctypeFromRow(object[] rowData)
        {
            int groupId = Convert.ToInt32( rowData[GROUP_ID_COL]);
            int occtypId = Convert.ToInt32(rowData[OCCTYPE_ID_COL]);

            string name = (string)rowData[NAME_COL];
            string desc = (string)rowData[DESC_COL];
            string damCatName = (string)rowData[DAM_CAT_COL];

            string foundHtUncertaintyXML = (string)rowData[FOUND_HT_UNCERTAINTY_COL];
            string foundHtUncertaintyType = (string)rowData[FOUND_HT_UNCERTAINTY_TYPE_COL];

            //structures
            bool isStructTabChecked = Convert.ToBoolean(rowData[IS_STRUCT_SELECTED_COL]);
            string structValueType = (string)rowData[VAR_STRUCT_TYPE_COL];
            string structValueUncertaintyXML = (string)rowData[VAR_STRUCT_VALUE_COL];
            string structDepthDamageXML = (string)rowData[STRUCT_CURVE_COL];

            //content
            bool isContenTabChecked = Convert.ToBoolean(rowData[IS_CONT_SELECTED_COL]);
            string contValueType = (string)rowData[VAR_CONT_TYPE_COL];
            string contentValueUncertaintyXML = (string)rowData[VAR_CONT_VALUE_COL];
            string contentDepthDamageXML = (string)rowData[CONTENT_CURVE_COL];

            //vehicle
            bool isVehicleTabChecked = Convert.ToBoolean(rowData[IS_VEH_SELECTED_COL]);
            string vehicleValueType = (string)rowData[VAR_VEH_TYPE_COL];
            string vehicleValueUncertaintyXML = (string)rowData[VAR_VEH_VALUE_COL];
            string vehicleDepthDamageXML = (string)rowData[VEHICLE_CURVE_COL];

            //other
            bool isOtherTabChecked = Convert.ToBoolean(rowData[IS_OTHER_SELECTED_COL]);
            string otherValueType = (string)rowData[VAR_OTHER_TYPE_COL];
            string otherValueUncertaintyXML = (string)rowData[VAR_OTHER_VALUE_COL];
            string otherDepthDamageXML = (string)rowData[OTHER_CURVE_COL];

            IOccupancyType occtype = OccupancyTypeFactory.Factory(name, damCatName);
            occtype.GroupID = groupId;
            occtype.ID = occtypId;
            occtype.Name = name;
            occtype.Description = desc;
            occtype.DamageCategory = DamageCategoryFactory.Factory(damCatName);
            occtype.FoundationHtUncertaintyType = GetValueUncertaintyType(foundHtUncertaintyType);
            occtype.FoundationHeightUncertainty = ContinuousDistribution.FromXML(XElement.Parse(foundHtUncertaintyXML));

            //structures
            occtype.CalculateStructureDamage = isStructTabChecked;
            occtype.StructureValueUncertainty = ContinuousDistribution.FromXML(XElement.Parse(structValueUncertaintyXML));
            occtype.StructureUncertaintyType = GetValueUncertaintyType(structValueType);
            //todo: this is ugly. I should put a method to read the xml for a coord func down to the icoordinatesfuntionfactory
            //i am just creating a dummy impact area func because i want the icoord func inside it.
            //IFdaFunction structFunction = IFdaFunctionFactory.Factory(structDepthDamageXML, IParameterEnum.Rating);
            occtype.StructureDepthDamageFunction = UncertainPairedData.ReadFromXML(XElement.Parse(structDepthDamageXML));

            //content
            occtype.CalculateContentDamage = isContenTabChecked;
            occtype.ContentValueUncertainty = ContinuousDistribution.FromXML(XElement.Parse(contentValueUncertaintyXML));
            occtype.ContentUncertaintyType = GetValueUncertaintyType(contValueType);           
            occtype.ContentDepthDamageFunction = UncertainPairedData.ReadFromXML(XElement.Parse(contentDepthDamageXML));

            //vehicle
            occtype.CalculateVehicleDamage = isVehicleTabChecked;
            occtype.VehicleValueUncertainty = ContinuousDistribution.FromXML(XElement.Parse(vehicleValueUncertaintyXML));
            occtype.VehicleUncertaintyType = GetValueUncertaintyType(vehicleValueType);
            occtype.VehicleDepthDamageFunction = UncertainPairedData.ReadFromXML(XElement.Parse(vehicleDepthDamageXML));


            //other
            occtype.CalculateOtherDamage = isOtherTabChecked;
            occtype.OtherValueUncertainty = ContinuousDistribution.FromXML(XElement.Parse(vehicleValueUncertaintyXML));
            occtype.OtherUncertaintyType = GetValueUncertaintyType(otherValueType);
            occtype.OtherDepthDamageFunction = UncertainPairedData.ReadFromXML(XElement.Parse(otherDepthDamageXML));

            //setting all these properties will set the "isModified" to true. But we just created this thing so turn back to false
            occtype.IsModified = false;
            return occtype;
        }


        /// <summary>
        /// This method is used to create the row for the parent occtype table. 
        /// This table has a lot of columns
        /// </summary>
        /// <param name="ot"></param>
        /// <returns></returns>
        private List<object> GetOccTypeRowForOccTypesTable(int elemId, int occtypeId, IOccupancyType ot)
        {
            List<object> rowsList = new List<object>();
            //bool[] checkedTabs = new bool[4];
            //if (OccTypesSelectedTabsDictionary.ContainsKey(ot.Name))
            //{
            //    checkedTabs = OccTypesSelectedTabsDictionary[ot.Name];
            //}
            //else
            //{
            //    //can't find the key in the dictionary
            //    throw new Exception();
            //}
            //add the group id
            rowsList.Add(elemId);

            //add the occtype id
            rowsList.Add(occtypeId);

            //name, description, damacat name
            foreach (object o in GetOccTypeInfoArray(ot))
            {
                rowsList.Add(o);
            }

            //foundation ht uncertainty type
            rowsList.Add(ot.FoundationHtUncertaintyType.ToString());
            //foundation height xml
            rowsList.Add(ot.FoundationHeightUncertainty.ToXML().ToString());


            //is struct checked
            rowsList.Add(ot.CalculateStructureDamage);

            //structure uncertainty type
            rowsList.Add(ot.StructureUncertaintyType.ToString());
            //structure value uncertainty
            rowsList.Add(ot.StructureValueUncertainty.ToXML().ToString());

            //structure depth damage function
            rowsList.Add(ot.StructureDepthDamageFunction.WriteToXML().ToString());

            ////structure variation in value type, min, max, st dev
            //foreach (object o in GetContinuousDistributionArray(ot.StructureValueUncertainty))
            //{
            //    rowsList.Add(o);
            //}

            ////structure dist type
            //rowsList.Add(ot.GetStructurePercentDD.Distribution);

            //is content checked
            rowsList.Add(ot.CalculateContentDamage);

            //content value uncertainty type
            rowsList.Add(ot.ContentUncertaintyType.ToString());
            //content value uncertainty
            rowsList.Add(ot.ContentValueUncertainty.ToXML().ToString());

            //content depth damage function
            rowsList.Add(ot.ContentDepthDamageFunction.WriteToXML().ToString());

            ////content variation in value type, min, max, st dev
            //foreach (object o in GetContinuousDistributionArray(ot.ContentValueUncertainty))
            //{
            //    rowsList.Add(o);
            //}

            ////cont dist type
            //rowsList.Add(ot.GetContentPercentDD.Distribution);

            //is vehicle checked
            rowsList.Add(ot.CalculateVehicleDamage);

            //vehicle uncertainty type
            rowsList.Add(ot.VehicleUncertaintyType.ToString());
            //vehicle value uncertainty
            rowsList.Add(ot.VehicleValueUncertainty.ToXML().ToString());

            //vehicle depth damage function 
            rowsList.Add(ot.VehicleDepthDamageFunction.WriteToXML().ToString());


            ////vehicle variation in value type, min, max, st dev
            //foreach (object o in GetContinuousDistributionArray(ot.VehicleValueUncertainty))
            //{
            //    rowsList.Add(o);
            //}

            ////vehicle dist type
            //rowsList.Add(ot.GetVehiclePercentDD.Distribution);

            //is other checked
            rowsList.Add(ot.CalculateOtherDamage);

            //other uncertainty type
            rowsList.Add(ot.OtherUncertaintyType.ToString());
            //other value uncertainty
            rowsList.Add(ot.OtherValueUncertainty.ToXML().ToString());

            //other depth damage function
            rowsList.Add(ot.OtherDepthDamageFunction.WriteToXML().ToString());

            ////Other variation in value type, min, max, st dev
            //foreach (object o in GetContinuousDistributionArray(ot.OtherValueUncertainty))
            //{
            //    rowsList.Add(o);
            //}

            ////other dist type
            //rowsList.Add(ot.GetOtherPercentDD.Distribution);

            ////damcats and occtypes group name
            //rowsList.Add(groupName);


            ////structure curve xml
            //rowsList.Add(ExtentionMethods.CreateXMLCurveString(ot.GetStructurePercentDD.Distribution, ot.GetStructurePercentDD.XValues, ot.GetStructurePercentDD.YValues));

            ////content curve xml
            //rowsList.Add(ExtentionMethods.CreateXMLCurveString(ot.GetContentPercentDD.Distribution, ot.GetContentPercentDD.XValues, ot.GetContentPercentDD.YValues));
            ////vehicle curve xml
            //rowsList.Add(ExtentionMethods.CreateXMLCurveString(ot.GetVehiclePercentDD.Distribution, ot.GetVehiclePercentDD.XValues, ot.GetVehiclePercentDD.YValues));
            ////other curve xml
            //rowsList.Add(ExtentionMethods.CreateXMLCurveString(ot.GetOtherPercentDD.Distribution, ot.GetOtherPercentDD.XValues, ot.GetOtherPercentDD.YValues));


            return rowsList;
        }

        private object[] GetOccTypeInfoArray(IOccupancyType ot)
        {
            return new object[] { ot.Name, ot.Description, ot.DamageCategory.Name };

        }

        //private object[] GetContinuousDistributionArray(ICoordinatesFunction cd)
        //{
        //    object[] rowItems = new object[4];

        //    if (cd.GetType() == typeof(Statistics.None))
        //    {
        //        return new object[] { "None", 0, 0, 0 };
        //    }
        //    else if (cd.GetType() == typeof(Statistics.Normal))
        //    {
        //        double stDev = ((Statistics.Normal)cd).GetStDev;
        //        return new object[] { "Normal", 0, 0, stDev };

        //    }
        //    else if (cd.GetType() == typeof(Statistics.Triangular))
        //    {
        //        double min = ((Statistics.Triangular)cd).getMin;
        //        double max = ((Statistics.Triangular)cd).getMax;
        //        return new object[] { "Triangular", min, max, 0 };

        //    }
        //    else if (cd.GetType() == typeof(Statistics.Uniform))
        //    {
        //        double min = ((Statistics.Uniform)cd).GetMin;
        //        double max = ((Statistics.Uniform)cd).GetMax;
        //        return new object[] { "Uniform", min, max, 0 };

        //    }

        //    return rowItems;
        //}



        public ObservableCollection<LogItem> GetLogMessages(ChildElement element)
        {
            return new ObservableCollection<LogItem>();
        }

        /// <summary>
        /// This will put a log into the log tables. Logs are only unique by element id and
        /// element type. ie. Rating Curve id=3.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="elementName"></param>
        public override void Log(LoggingLevel level, string message, string elementName)
        {
           // int elementId = GetElementId(TableName, elementName);
            //LOGGER.Log(level, message, ELEMENT_TYPE, elementId);
        }

        /// <summary>
        /// This will look in the parent table for the element id using the element name. 
        /// Then it will sweep through the log tables pulling out any logs with that id
        /// and element type. 
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public override ObservableCollection<LogItem> GetLogMessages(string elementName)
        {
            //int id = GetElementId(TableName, elementName);
            //return RetrieveFromDB.GetLogMessages(id, ELEMENT_TYPE);
            return new ObservableCollection<LogItem>();
        }

        /// <summary>
        /// Gets all the log messages for this element from the specified log level table.
        /// This is used by the MessageExpander to filter by log level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public override ObservableCollection<LogItem> GetLogMessagesByLevel(LoggingLevel level, string elementName)
        {
            //int id = GetElementId(TableName, elementName);
            //return RetrieveFromDB.GetLogMessagesByLevel(level, id, ELEMENT_TYPE);
            return new ObservableCollection<LogItem>();

        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            OccupancyTypesElement occElem = (OccupancyTypesElement)elem;
            return new object[]
            {
                    occElem.Name,
                    occElem.IsSelected
            };
        }

        public IOccupancyType CloneOccType(IOccupancyType ot)
        {
            List<Object> occtypeInRowForm = GetOccTypeRowForOccTypesTable(ot.GroupID, ot.ID, ot);
            return CreateOcctypeFromRow(occtypeInRowForm.ToArray());

        }

    }
}
