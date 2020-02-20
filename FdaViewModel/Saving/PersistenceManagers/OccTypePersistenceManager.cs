using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Inventory.OccupancyTypes;
using FdaViewModel.Utilities;
using Functions;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class OccTypePersistenceManager : SavingBase, IElementManager
    {
        //These are the columns for the parent table
        private const int PARENT_GROUP_NAME_COL = 0;
        private const int PARENT_IS_SELECTED_COL = 1;


        //These are the columns for the child table
        private const int NAME_COL = 0;
        private const int DESC_COL = 1;
        private const int DAM_CAT_COL = 2;
        private const int VAR_FOUND_HT_COL = 3;
        private const int FOUND_HT_MIN_COL = 4;
        private const int FOUND_HT_MAX_COL = 5;
        private const int FOUND_HT_STDEV_COL = 6;

        private const int IS_STRUCT_SELECTED_COL = 7;
        private const int VAR_STRUCT_VALUE_COL = 8;
        private const int STRUCT_MIN_COL = 9;
        private const int STRUCT_MAX_COL = 10;
        private const int STRUCT_STDEV_COL = 11;
        private const int STRUCT_DIST_TYPE_COL = 12;

        private const int IS_CONT_SELECTED_COL = 13;
        private const int VAR_CONT_VALUE_COL = 14;
        private const int CONT_MIN_COL = 15;
        private const int CONT_MAX_COL = 16;
        private const int CONT_STDEV_COL = 17;
        private const int CONT_DIST_TYPE_COL = 18;

        private const int IS_VEH_SELECTED_COL = 19;
        private const int VAR_VEH_VALUE_COL = 20;
        private const int VEH_MIN_COL = 21;
        private const int VEH_MAX_COL = 22;
        private const int VEH_STDEV_COL = 23;
        private const int VEH_DIST_TYPE_COL = 24;

        private const int IS_OTHER_SELECTED_COL = 25;
        private const int VAR_OTHER_VALUE_COL = 26;
        private const int OTHER_MIN_COL = 27;
        private const int OTHER_MAX_COL = 28;
        private const int OTHER_STDEV_COL = 29;
        private const int OTHER_DIST_TYPE_COL = 30;

        private const int GROUP_NAME_COL = 31;
        private const int STRUCT_CURVE_COL = 32;
        private const int CONTENT_CURVE_COL = 33;
        private const int VEHICLE_CURVE_COL = 34;
        private const int OTHER_CURVE_COL = 35;



        //ELEMENT_TYPE is used to store the type in the log tables. Initially i was actually storing the type
        //of the element. But since they get stored as strings if a developer changes the name of the class
        //you would no longer get any of the old logs. So i use this constant.
        private const string ELEMENT_TYPE = "OccType";
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("OccTypePersistenceManager");


        private const string GroupTablePrefix = "OccTypeGroup-";
        private const string ParentTableName = "OccTypeGroups";
        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return new Type[0]; }
        }
        internal override string ChangeTableConstant { get { return "OccType"; } }

        public override string TableName => throw new NotImplementedException();

        public override string[] TableColumnNames => throw new NotImplementedException();

        public OccTypePersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
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
            int testing = 0;
            Dictionary<string, bool[]> selectedTabsDictionary = new Dictionary<string, bool[]>();
            List<IOccupancyType> listOfOccTypes = new List<IOccupancyType>();
            List<IOccupancyType> TempOccTypes = new List<IOccupancyType>();
            Dictionary<string, bool[]> dummyDictionary = new Dictionary<string, bool[]>();
            string groupName = rowData[PARENT_GROUP_NAME_COL].ToString();
            //create an empty element. Then loop through all the rows in the table to add the actual occtypes for this elem.
            OccupancyTypesElement ele = new OccupancyTypesElement(groupName, TempOccTypes, dummyDictionary);
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
                    DatabaseManager.DataTableView dtv = Storage.Connection.Instance.GetTable(ParentTableName);
                    List<object[]> rowData = dtv.GetRows(0, dtv.NumberOfRows - 1);

                    //go over each row in the parent table and create an occtype element
                    for (int i = 0; i < dtv.NumberOfRows; i++)
                    {
                        //OccupancyTypesElement ele = new OccupancyTypesElement(groupName, TempOccTypes, dummyDictionary);
                        
                            OccupancyTypesElement ele = (OccupancyTypesElement)CreateElementFromRowData(dtv.GetRow(i));
                            occTypeGroupsToReturn.Add(ele);

                        


                    }
                }
            }
            foreach (Inventory.OccupancyTypes.OccupancyTypesElement elem in occTypeGroupsToReturn)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public void Remove(ChildElement element)
        {
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }
            //remove row from parent table
            DatabaseManager.DataTableView dtv = Storage.Connection.Instance.GetTable(ParentTableName);
            int index = -1;
            for(int i = 0;i<dtv.NumberOfRows;i++)
            {
                if(dtv.GetRow(i)[0].ToString().Equals(element.Name))
                {
                    index = i;
                    break;
                }
            }
            if(index == -1) { return; }//throw exception?
            dtv.DeleteRow(index);
            dtv.ApplyEdits();
            //remove the associated table
            string elementTableName = GroupTablePrefix + element.Name;
            if(Storage.Connection.Instance.TableNames().Contains(elementTableName))
            {
                Storage.Connection.Instance.DeleteTable(elementTableName);
            }
            //remove from the study cache
            StudyCacheForSaving.RemoveElement((OccupancyTypesElement)element);
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
            OccupancyTypesOwnerElement owner = StudyCacheForSaving.GetParentElementOfType<OccupancyTypesOwnerElement>();
            //because i am lumping all the elements together in one editor, then it is difficult to keep track of old names vs new names vs adding new ones etc.
            //i think it is best to just delete all previous tables (all rows in parent table and all individual tables) and then resave everything.
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }
            DeleteAllOccTypeTables();
            RemoveAllOccTypeElementsFromTheStudyCache();

            //save the occtypes
            //foreach (OccupancyTypesElement element in elements)
            //{
                SaveElementsOnBackGroundThread(elements, owner, (elem) => SavingAction(elem), " - Saving");

              //  SaveNew(element);
            //}
           
        }

        private void RemoveAllOccTypeElementsFromTheStudyCache()
        {
            List<OccupancyTypesElement> elems = StudyCache.GetChildElementsOfType<OccupancyTypesElement>();
            foreach (OccupancyTypesElement elem in elems)
            {
                StudyCacheForSaving.RemoveElement(elem);
            }
        }
        private void DeleteAllOccTypeTables()
        {
            if (Storage.Connection.Instance.TableNames().Contains(ParentTableName))
            {
                DatabaseManager.DataTableView dtv = Storage.Connection.Instance.GetTable(ParentTableName);
                foreach (object[] row in dtv.GetRows(0, dtv.NumberOfRows - 1))
                {
                    //delete the table with the name of row[0]
                    string elementTableName = GroupTablePrefix + row[0].ToString();
                    if (Storage.Connection.Instance.TableNames().Contains(elementTableName))
                    {
                        Storage.Connection.Instance.DeleteTable(elementTableName);
                    }
                }
                //finally delete the parent table itself
                Storage.Connection.Instance.DeleteTable(ParentTableName);
            }
        }


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
            SaveGroupToParentTable(element.Name, ((OccupancyTypesElement)element).IsSelected);
            SaveNewGroupTable((OccupancyTypesElement)element);
            StudyCacheForSaving.AddElement((OccupancyTypesElement)element);

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

        private void SaveNewGroupTable(OccupancyTypesElement element)
        {
            string groupName = element.Name;
            List<IOccupancyType> ListOfOccupancyTypes = ((OccupancyTypesElement)element).ListOfOccupancyTypes;
            Dictionary<string, bool[]> OccTypesSelectedTabsDictionary = ((OccupancyTypesElement)element).OccTypesSelectedTabsDictionary;


            string[] colNames = new string[] { "Name", "Description", "DamageCategory","VarInFoundHtType","FdHtMin","FdHtMax","FdHtStDev",
                    "IsStructChecked","VarInStructValueType","StructMin","StructMax","StructStDev","StructDistType", "IsContChecked",
                    "VarInContValueType", "ContMin", "ContMax", "ContStDev", "ContDistType", "IsVehChecked", "VarInVehValueType",
                    "VehMin", "VehMax", "VehStDev", "VehDistType", "IsOtherChecked", "VarInOtherValueType", "OtherMin",
                    "OtherMax", "OtherStDev", "OtherDistType","GroupName", "StructureCurve","ContentCurve","VehicleCurve",
                    "OtherCurve" };
            Type[] colTypes = new Type[] {
                    typeof(string), typeof(string), typeof(string), typeof(string), typeof(double),
                    typeof(double), typeof(double), typeof(bool), typeof(string), typeof(double), typeof(double), typeof(double),
                    typeof(string), typeof(bool), typeof(string), typeof(double), typeof(double), typeof(double), typeof(string),
                    typeof(bool), typeof(string), typeof(double), typeof(double), typeof(double), typeof(string), typeof(bool),
                    typeof(string), typeof(double), typeof(double), typeof(double), typeof(string),typeof(string),typeof(string),
                    typeof(string),typeof(string),typeof(string)};

           
            Storage.Connection.Instance.CreateTable(GroupTablePrefix + groupName, colNames, colTypes);
            DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(GroupTablePrefix + groupName);

            List<object[]> rows = new List<object[]>();

            foreach (IOccupancyType ot in ListOfOccupancyTypes)
            {

                rows.Add(GetOccTypeRowForParentTable(ot, OccTypesSelectedTabsDictionary, groupName).ToArray());
            }
            tbl.AddRows(rows);
            tbl.ApplyEdits();
        }



        /// <summary>
        /// This method is used to create the row for the parent occtype table. 
        /// This table has a lot of columns
        /// </summary>
        /// <param name="ot"></param>
        /// <returns></returns>
        private List<object> GetOccTypeRowForParentTable(IOccupancyType ot, Dictionary<string, bool[]> OccTypesSelectedTabsDictionary, string groupName)
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


            ////name, description, damacat name
            //foreach (object o in GetOccTypeInfoArray(ot))
            //{
            //    rowsList.Add(o);
            //}

            ////found ht variation type, min, max, st dev
            //foreach (object o in GetContinuousDistributionArray(ot.FoundationHeightUncertaintyFunction))
            //{
            //    rowsList.Add(o);
            //}

            ////is struct checked
            //rowsList.Add(checkedTabs[0]);

            ////structure variation in value type, min, max, st dev
            //foreach (object o in GetContinuousDistributionArray(ot.StructureValueUncertainty))
            //{
            //    rowsList.Add(o);
            //}

            ////structure dist type
            //rowsList.Add(ot.GetStructurePercentDD.Distribution);

            ////is content checked
            //rowsList.Add(checkedTabs[1]);

            ////content variation in value type, min, max, st dev
            //foreach (object o in GetContinuousDistributionArray(ot.ContentValueUncertainty))
            //{
            //    rowsList.Add(o);
            //}

            ////cont dist type
            //rowsList.Add(ot.GetContentPercentDD.Distribution);

            ////is vehicle checked
            //rowsList.Add(checkedTabs[2]);

            ////vehicle variation in value type, min, max, st dev
            //foreach (object o in GetContinuousDistributionArray(ot.VehicleValueUncertainty))
            //{
            //    rowsList.Add(o);
            //}

            ////vehicle dist type
            //rowsList.Add(ot.GetVehiclePercentDD.Distribution);

            ////is other checked
            //rowsList.Add(checkedTabs[3]);

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

        private object[] GetOccTypeInfoArray(Consequences_Assist.ComputableObjects.OccupancyType ot)
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
            throw new NotImplementedException();
        }
    }
}
