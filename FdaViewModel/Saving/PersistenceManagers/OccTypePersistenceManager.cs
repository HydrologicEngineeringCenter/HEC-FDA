using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FdaViewModel.Inventory.DamageCategory;
using FdaViewModel.Inventory.OccupancyTypes;
using FdaViewModel.Utilities;
using Functions;
using Model;
using Model.Inputs.Functions.ImpactAreaFunctions;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class OccTypePersistenceManager : SavingBase, IElementManager
    {
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
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("OccTypePersistenceManager");


        private const string GroupTablePrefix = "OccTypeGroup-";
        private const string ParentTableName = "occupancy_type_groups";
        private const string OCCTYPES_TABLE_NAME = "occupancy_types";

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

        //todo: Refactor: removing Statistics.ContinuousDistribution and making it an IDistribution
        //I think this is taking each row and making a distribution
        //private IDistributedValue CreateContinuousDistributionFromRow(object[] row, int start, int end)
        //{

        //    if (row[start].ToString() == "Normal")
        //    {
        //        IDistributedValue normDist = DistributedValueFactory.FactoryNormal(0, Convert.ToDouble(row[end]));
        //        //ICoordinatesFunction norm = ICoordinatesFunctionsFactory. new Statistics.Normal(0, Convert.ToDouble(row[end]));
        //        return normDist;
        //    }
        //    else if (row[start].ToString() == "Uniform")
        //    {
        //        IDistributedValue uniformDist = DistributedValueFactory.FactoryUniform(Convert.ToDouble(row[start + 1]), Convert.ToDouble(row[start + 2]));
        //        //Statistics.Uniform uni = new Statistics.Uniform(Convert.ToDouble(row[start + 1]), Convert.ToDouble(row[start + 2]));
        //        return uniformDist;
        //    }
        //    else if (row[start].ToString() == "None")
        //    {
        //        //todo: Refactor: Finish this
        //        //IDistributedValue constantDist = DistributedValueFactory.fact(Convert.ToDouble(row[start + 1]), Convert.ToDouble(row[start + 2]));
        //        //Statistics.None non = new Statistics.None();
        //        //return non;

        //    }
        //    else if (row[start].ToString() == "Triangular")
        //    {
        //        //todo: Refactor: Make sure these inputs are in the correct order, min-max-mostlikely?
        //        IDistributedValue tri = DistributedValueFactory.FactoryTriangular(Convert.ToDouble(row[start + 1]), Convert.ToDouble(row[start + 2]), Convert.ToDouble(row[start + 3]));
        //        //Statistics.Uniform tri = new Statistics.Uniform(Convert.ToDouble(row[start]), Convert.ToDouble(row[start + 1]));
        //        //Statistics.Triangular tri = new Statistics.Triangular(Convert.ToDouble(row[start+1]), Convert.ToDouble(row[start + 2]), Convert.ToDouble(row[start + 3]));
        //        return tri;
        //    }

        //    return null;// new Statistics.Normal(); // it should never get here.
        //}
        /// <summary>
        /// Creates an element base off the row from the parent table
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
            OccupancyTypesElement ele = new OccupancyTypesElement(groupName, TempOccTypes);
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


        

        public void Load()
        {
            List<ChildElement> occTypeGroupsToReturn = new List<ChildElement>();
            
            //each row in the parent table will be an occtype element (same as occtype group)
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                if (!Storage.Connection.Instance.IsOpen)
                {
                    Storage.Connection.Instance.Open();
                }
                if (Storage.Connection.Instance.TableNames().Contains(ParentTableName))
                {

                    System.Data.DataTable table = Storage.Connection.Instance.GetDataTable(ParentTableName);

                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        //each of these is a group
                        int groupId = Convert.ToInt32(row[PARENT_GROUP_ID_COL]);
                        string groupName = (string)row[PARENT_GROUP_NAME_COL];
                        bool isGroupSelected = Convert.ToBoolean(row[PARENT_IS_SELECTED_COL]);

                        //now read the child table and grab all the occtypes with this group id
                        List<IOccupancyType> occtypes = LoadOcctypes(groupId);
                        OccupancyTypesElement elem = new OccupancyTypesElement( groupName, occtypes);
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
                }
            }
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

        public void DeleteOcctypeGroup(IOccupancyTypeGroupEditable group)
        {
            //delete the row from the parent table
            DeleteRowWithKey(TableName, group.ID, "ID");

            //delete all the occtypes associated with this group from the occtypes table
            //this one call will delete all the rows with that group id
            DeleteRowWithKey(OCCTYPES_TABLE_NAME, group.ID, "GroupID");

            //remove from the study cache
            RemoveElementFromCache(group); 
        }

        /// <summary>
        /// Removes the specified group from the study cache. This is done when saving the 
        /// occupancy types editor. 
        /// </summary>
        /// <param name="group"></param>
        private void RemoveElementFromCache(IOccupancyTypeGroupEditable group)
        {
            List<OccupancyTypesElement> elems = StudyCacheForSaving.OccTypeElements;
            int indexToRemove = -1;
            for (int i = 0; i < elems.Count(); i++)
            {
                if (elems[i].Name.Equals(group.OriginalName))
                {
                    indexToRemove = i;
                    break;
                }
            }
            if (indexToRemove != -1)
            {
                StudyCacheForSaving.OccTypeElements.RemoveAt(indexToRemove);
            }
        }

        public void DeleteOcctypes(List<IOccupancyTypeEditable> occtypesToDelete)
        {
            foreach (IOccupancyTypeEditable ot in occtypesToDelete)
            {
                int[] keys = new int[] { ot.OccType.GroupID, ot.OccType.ID };
                string[] keyColNames = new string[] { "GroupID", "OcctypeID" };

                DeleteRowWithCompoundKey(OCCTYPES_TABLE_NAME, keys, keyColNames);
            }
        }

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


        public void SaveNewOcctypes(List<IOccupancyTypeEditable> newOcctypes)
        {

            DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(OCCTYPES_TABLE_NAME);
            if (tbl == null)
            {
                Storage.Connection.Instance.CreateTable(OCCTYPES_TABLE_NAME, OcctypeColumns, OcctypeTypes);
                tbl = Storage.Connection.Instance.GetTable(OCCTYPES_TABLE_NAME);
            }

            List<object[]> rows = new List<object[]>();

            foreach (IOccupancyTypeEditable ot in newOcctypes)
            {
                //when this occtype was created it should have gotten a unique id of its own.
                rows.Add(GetOccTypeRowForOccTypesTable(ot.OccType.GroupID, ot.OccType.ID, ot.OccType).ToArray());
            }
            tbl.AddRows(rows);
            tbl.ApplyEdits();
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

        /// <summary>
        /// This removes all the matching elements from the study cache and replaces it with a new
        /// element that is created from the group.
        /// </summary>
        /// <param name="groupsToUpdateInCache"></param>
        public void UpdateStudyCache(List<IOccupancyTypeGroupEditable> groupsToUpdateInCache)
        {
            foreach(IOccupancyTypeGroupEditable group in groupsToUpdateInCache)
            {
                RemoveElementFromCache(group);

                //create new element to add to the cache
                List<IOccupancyType> occtypes = new List<IOccupancyType>();
                foreach(IOccupancyTypeEditable otEdit in group.Occtypes)
                {
                    occtypes.Add(otEdit.OccType);
                }
                OccupancyTypesElement elem = new OccupancyTypesElement(group.Name, occtypes);
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public void SaveModifiedOcctypes(List<IOccupancyTypeEditable> occtypes)
        {
            foreach(IOccupancyTypeEditable ot in occtypes)
            {
                int[] keys = new int[] { ot.OccType.GroupID, ot.OccType.ID };
                string[] keyColNames = new string[] { "GroupID", "OcctypeID" };

                //update the whole row
                string[] columnsToUpdate = OcctypeColumns;
                object[] newValues = GetOccTypeRowForOccTypesTable(ot.OccType.GroupID, ot.OccType.ID, ot.OccType).ToArray();

                UpdateTableRowWithCompoundKey(OCCTYPES_TABLE_NAME, keys, keyColNames, columnsToUpdate, newValues);
            }
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
            return occtypeIds.Max() + 1;
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

        public void SaveNewElements(List<ChildElement> elements)
        {
            OccupancyTypesOwnerElement owner = StudyCacheForSaving.GetParentElementOfType<OccupancyTypesOwnerElement>();
            SaveElementsOnBackGroundThread(elements, owner, (elem) => SavingAction(elem), " - Saving");
        }

        private void SavingAction(ChildElement element)
        {
            //this will save to the parent table
            SaveNewElement(element);

            //save the child table
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
            occtype.FoundationHeightUncertainty = ICoordinateFactory.CreateOrdinate(XElement.Parse(foundHtUncertaintyXML));

            //structures
            occtype.CalculateStructureDamage = isStructTabChecked;
            occtype.StructureValueUncertainty = ICoordinateFactory.CreateOrdinate(XElement.Parse(structValueUncertaintyXML));
            occtype.StructureUncertaintyType = GetValueUncertaintyType(structValueType);
            //todo: this is ugly. I should put a method to read the xml for a coord func down to the icoordinatesfuntionfactory
            //i am just creating a dummy impact area func because i want the icoord func inside it.
            IFdaFunction structFunction = ImpactAreaFunctionFactory.Factory(structDepthDamageXML, ImpactAreaFunctionEnum.Rating);
            occtype.StructureDepthDamageFunction = structFunction.Function;

            //content
            occtype.CalculateContentDamage = isContenTabChecked;
            occtype.ContentValueUncertainty = ICoordinateFactory.CreateOrdinate(XElement.Parse(contentValueUncertaintyXML));
            occtype.ContentUncertaintyType = GetValueUncertaintyType(contValueType);
            //todo: this is ugly. I should put a method to read the xml for a coord func down to the icoordinatesfuntionfactory
            //i am just creating a dummy impact area func because i want the icoord func inside it.
            IFdaFunction contFunction = ImpactAreaFunctionFactory.Factory(contentDepthDamageXML, ImpactAreaFunctionEnum.Rating);
            occtype.ContentDepthDamageFunction = contFunction.Function;

            //vehicle
            occtype.CalculateVehicleDamage = isVehicleTabChecked;
            occtype.VehicleValueUncertainty = ICoordinateFactory.CreateOrdinate(XElement.Parse(vehicleValueUncertaintyXML));
            occtype.VehicleUncertaintyType = GetValueUncertaintyType(vehicleValueType);
            //todo: this is ugly. I should put a method to read the xml for a coord func down to the icoordinatesfuntionfactory
            //i am just creating a dummy impact area func because i want the icoord func inside it.
            IFdaFunction vehFunction = ImpactAreaFunctionFactory.Factory(vehicleDepthDamageXML, ImpactAreaFunctionEnum.Rating);
            occtype.VehicleDepthDamageFunction = vehFunction.Function;

            //other
            occtype.CalculateOtherDamage = isOtherTabChecked;
            occtype.OtherValueUncertainty = ICoordinateFactory.CreateOrdinate(XElement.Parse(vehicleValueUncertaintyXML));
            occtype.OtherUncertaintyType = GetValueUncertaintyType(otherValueType);
            //todo: this is ugly. I should put a method to read the xml for a coord func down to the icoordinatesfuntionfactory
            //i am just creating a dummy impact area func because i want the icoord func inside it.
            IFdaFunction otherFunction = ImpactAreaFunctionFactory.Factory(otherDepthDamageXML, ImpactAreaFunctionEnum.Rating);
            occtype.OtherDepthDamageFunction = otherFunction.Function;

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
            rowsList.Add(ot.FoundationHeightUncertainty.WriteToXML().ToString());


            //is struct checked
            rowsList.Add(ot.CalculateStructureDamage);

            //structure uncertainty type
            rowsList.Add(ot.StructureUncertaintyType.ToString());
            //structure value uncertainty
            rowsList.Add(ot.StructureValueUncertainty.WriteToXML().ToString());

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
            rowsList.Add(ot.ContentValueUncertainty.WriteToXML().ToString());

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
            rowsList.Add(ot.VehicleValueUncertainty.WriteToXML().ToString());

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
            rowsList.Add(ot.OtherValueUncertainty.WriteToXML().ToString());

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



        public ObservableCollection<FdaLogging.LogItem> GetLogMessages(ChildElement element)
        {
            return new ObservableCollection<FdaLogging.LogItem>();
        }

        /// <summary>
        /// This will put a log into the log tables. Logs are only unique by element id and
        /// element type. ie. Rating Curve id=3.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="elementName"></param>
        public void Log(FdaLogging.LoggingLevel level, string message, string elementName)
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
        public ObservableCollection<FdaLogging.LogItem> GetLogMessages(string elementName)
        {
            //int id = GetElementId(TableName, elementName);
            //return FdaLogging.RetrieveFromDB.GetLogMessages(id, ELEMENT_TYPE);
            return new ObservableCollection<FdaLogging.LogItem>();
        }

        /// <summary>
        /// Gets all the log messages for this element from the specified log level table.
        /// This is used by the MessageExpander to filter by log level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public ObservableCollection<FdaLogging.LogItem> GetLogMessagesByLevel(FdaLogging.LoggingLevel level, string elementName)
        {
            //int id = GetElementId(TableName, elementName);
            //return FdaLogging.RetrieveFromDB.GetLogMessagesByLevel(level, id, ELEMENT_TYPE);
            return new ObservableCollection<FdaLogging.LogItem>();

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
