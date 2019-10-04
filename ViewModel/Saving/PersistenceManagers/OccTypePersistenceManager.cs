using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Inventory.OccupancyTypes;
using FdaViewModel.Utilities;

namespace FdaViewModel.Saving.PersistenceManagers
{
    public class OccTypePersistenceManager : SavingBase, IElementManager
    {
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


        private Statistics.ContinuousDistribution CreateContinuousDistributionFromRow(object[] row, int start, int end)
        {

            if (row[start].ToString() == "Normal")
            {
                Statistics.Normal norm = new Statistics.Normal(0, Convert.ToDouble(row[end]));
                return norm;
            }
            else if (row[start].ToString() == "Uniform")
            {
                Statistics.Uniform uni = new Statistics.Uniform(Convert.ToDouble(row[start + 1]), Convert.ToDouble(row[start + 2]));
                return uni;
            }
            else if (row[start].ToString() == "None")
            {
                Statistics.None non = new Statistics.None();
                return non;

            }
            else if (row[start].ToString() == "Triangular")
            {
                //Statistics.Uniform tri = new Statistics.Uniform(Convert.ToDouble(row[start]), Convert.ToDouble(row[start + 1]));
                Statistics.Triangular tri = new Statistics.Triangular(Convert.ToDouble(row[start+1]), Convert.ToDouble(row[start + 2]), Convert.ToDouble(row[start + 3]));
                return tri;
            }

            return new Statistics.Normal(); // it should never get here.
        }
        /// <summary>
        /// Creates an element base off the row from the parent table
        /// </summary>
        /// <param name="rowData">row from parent table. Only one column of the group name</param>
        /// <returns></returns>
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            int testing = 0;
            Dictionary<string, bool[]> selectedTabsDictionary = new Dictionary<string, bool[]>();
            List<Consequences_Assist.ComputableObjects.OccupancyType> listOfOccTypes = new List<Consequences_Assist.ComputableObjects.OccupancyType>();
            List<Consequences_Assist.ComputableObjects.OccupancyType> TempOccTypes = new List<Consequences_Assist.ComputableObjects.OccupancyType>();
            Dictionary<string, bool[]> dummyDictionary = new Dictionary<string, bool[]>();
            string groupName = rowData[0].ToString();
            //create an empty element. Then loop through all the rows in the table to add the actual occtypes for this elem.
            OccupancyTypesElement ele = new OccupancyTypesElement(groupName, TempOccTypes, dummyDictionary);
            ele.IsSelected = (bool)rowData[1];
            string elementTableName = GroupTablePrefix + groupName;

            if (Storage.Connection.Instance.TableNames().Contains(elementTableName))
            {
                DatabaseManager.DataTableView elementTable = Storage.Connection.Instance.GetTable(elementTableName);
                List<object[]> occTypeRows = elementTable.GetRows(0, elementTable.NumberOfRows - 1);

            foreach (object[] row in occTypeRows)//Storage.Connection.Instance.GetTable(tableName).GetRows(0, lastRow))
            {
                    testing++;
                bool[] selectedTabs = new bool[] { Convert.ToBoolean(row[7]), Convert.ToBoolean(row[13]), Convert.ToBoolean(row[19]), Convert.ToBoolean(row[25]) };
                selectedTabsDictionary.Add(row[0].ToString(), selectedTabs);
                //ele.RelativePathAndProbability.Add(new PathAndProbability(row[0].ToString(), Convert.ToDouble(row[1])));
                Consequences_Assist.ComputableObjects.OccupancyType ot = new Consequences_Assist.ComputableObjects.OccupancyType();
                ot.Name = row[0].ToString();
                ot.Description = row[1].ToString();
                ot.DamageCategoryName = row[2].ToString();

                ot.FoundationHeightUncertainty = CreateContinuousDistributionFromRow(row, 3, 6);

                //***************************
                //structures
                //*****************************

                // if(Convert.ToBoolean(row[7]) == true) // if structures tab is checked
                
                    ot.StructureValueUncertainty = CreateContinuousDistributionFromRow(row, 8, 11);

                    if (row[12].ToString() == "Normal")
                    {
                        Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetNormalDistributionFromXML(row[32].ToString());
                        ot.SetStructurePercentDD = uci;
                    }
                    else if (row[12].ToString() == "None")
                    {
                        Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetNoneDistributionFromXML(row[32].ToString());
                        ot.SetStructurePercentDD = uci;
                    }
                    else if (row[12].ToString() == "Triangular")
                    {
                        Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetTriangularDistributionFromXML(row[32].ToString());
                        ot.SetStructurePercentDD = uci;
                    }
                    else if (row[12].ToString() == "Uniform")
                    {
                        Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetUniformDistributionFromXML(row[32].ToString());
                        ot.SetStructurePercentDD = uci;
                    }

                



                    //*****************************
                    //content
                    //*****************************

                    //if (Convert.ToBoolean(row[13]) == true) // if content tab is checked

                    ot.ContentValueUncertainty = CreateContinuousDistributionFromRow(row, 14, 17);
                    if (row[18].ToString() == "Normal")
                    {
                        Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetNormalDistributionFromXML(row[33].ToString());
                        ot.SetContentPercentDD = uci;
                    }
                    else if (row[18].ToString() == "None")
                    {
                        Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetNoneDistributionFromXML(row[33].ToString());
                        ot.SetContentPercentDD = uci;
                    }
                    else if (row[18].ToString() == "Triangular")
                    {
                        Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetTriangularDistributionFromXML(row[33].ToString());
                        ot.SetContentPercentDD = uci;
                    }
                    else if (row[18].ToString() == "Uniform")
                    {
                        Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetUniformDistributionFromXML(row[33].ToString());
                        ot.SetContentPercentDD = uci;
                    }





                    //*****************************
                    //vehicle
                    //*****************************

                    //if (Convert.ToBoolean(row[19]) == true) // if vehicle tab is checked
                    
                    ot.VehicleValueUncertainty = CreateContinuousDistributionFromRow(row, 20, 23);
                    if (row[24].ToString() == "Normal")
                    {
                        Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetNormalDistributionFromXML(row[34].ToString());
                        ot.SetVehiclePercentDD = uci;
                    }
                    else if (row[24].ToString() == "None")
                    {
                        Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetNoneDistributionFromXML(row[34].ToString());
                        ot.SetVehiclePercentDD = uci;
                    }
                    else if (row[24].ToString() == "Triangular")
                    {
                        Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetTriangularDistributionFromXML(row[34].ToString());
                        ot.SetVehiclePercentDD = uci;
                    }
                    else if (row[24].ToString() == "Uniform")
                    {
                        Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetUniformDistributionFromXML(row[34].ToString());
                        ot.SetVehiclePercentDD = uci;
                    }




                    //*****************************
                    //Other
                    //*****************************

                    //if (Convert.ToBoolean(row[25]) == true) // if other tab is checked

                    ot.OtherValueUncertainty = CreateContinuousDistributionFromRow(row, 26, 29);
                    if (row[30].ToString() == "Normal")
                    {
                        Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetNormalDistributionFromXML(row[35].ToString());
                        ot.SetOtherPercentDD = uci;
                    }
                    else if (row[30].ToString() == "None")
                    {
                        Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetNoneDistributionFromXML(row[35].ToString());
                        ot.SetOtherPercentDD = uci;
                    }
                    else if (row[30].ToString() == "Triangular")
                    {
                        Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetTriangularDistributionFromXML(row[35].ToString());
                        ot.SetOtherPercentDD = uci;
                    }
                    else if (row[30].ToString() == "Uniform")
                    {
                        Statistics.UncertainCurveIncreasing uci = ExtentionMethods.GetUniformDistributionFromXML(row[35].ToString());
                        ot.SetOtherPercentDD = uci;
                    }



                    listOfOccTypes.Add(ot);
            }
            ele.ListOfOccupancyTypes = listOfOccTypes;
            ele.OccTypesSelectedTabsDictionary = selectedTabsDictionary;
            //OccupancyTypesOwnerElement.ListOfOccupancyTypesGroups.Add(ele);
            }
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
            List<Consequences_Assist.ComputableObjects.OccupancyType> ListOfOccupancyTypes = ((OccupancyTypesElement)element).ListOfOccupancyTypes;
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

            foreach (Consequences_Assist.ComputableObjects.OccupancyType ot in ListOfOccupancyTypes)
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
        private List<object> GetOccTypeRowForParentTable(Consequences_Assist.ComputableObjects.OccupancyType ot, Dictionary<string, bool[]> OccTypesSelectedTabsDictionary, string groupName)
        {
            List<object> rowsList = new List<object>();
            bool[] checkedTabs = new bool[4];
            if (OccTypesSelectedTabsDictionary.ContainsKey(ot.Name))
            {
                checkedTabs = OccTypesSelectedTabsDictionary[ot.Name];
            }
            else
            {
                //can't find the key in the dictionary
                throw new Exception();
            }


            //name, description, damacat name
            foreach (object o in GetOccTypeInfoArray(ot))
            {
                rowsList.Add(o);
            }

            //found ht variation type, min, max, st dev
            foreach (object o in GetContinuousDistributionArray(ot.FoundationHeightUncertainty))
            {
                rowsList.Add(o);
            }

            //is struct checked
            rowsList.Add(checkedTabs[0]);

            //structure variation in value type, min, max, st dev
            foreach (object o in GetContinuousDistributionArray(ot.StructureValueUncertainty))
            {
                rowsList.Add(o);
            }

            //structure dist type
            rowsList.Add(ot.GetStructurePercentDD.Distribution);

            //is content checked
            rowsList.Add(checkedTabs[1]);

            //content variation in value type, min, max, st dev
            foreach (object o in GetContinuousDistributionArray(ot.ContentValueUncertainty))
            {
                rowsList.Add(o);
            }

            //cont dist type
            rowsList.Add(ot.GetContentPercentDD.Distribution);

            //is vehicle checked
            rowsList.Add(checkedTabs[2]);

            //vehicle variation in value type, min, max, st dev
            foreach (object o in GetContinuousDistributionArray(ot.VehicleValueUncertainty))
            {
                rowsList.Add(o);
            }

            //vehicle dist type
            rowsList.Add(ot.GetVehiclePercentDD.Distribution);

            //is other checked
            rowsList.Add(checkedTabs[3]);

            //Other variation in value type, min, max, st dev
            foreach (object o in GetContinuousDistributionArray(ot.OtherValueUncertainty))
            {
                rowsList.Add(o);
            }

            //other dist type
            rowsList.Add(ot.GetOtherPercentDD.Distribution);

            //damcats and occtypes group name
            rowsList.Add(groupName);


            //structure curve xml
            rowsList.Add(ExtentionMethods.CreateXMLCurveString(ot.GetStructurePercentDD.Distribution, ot.GetStructurePercentDD.XValues, ot.GetStructurePercentDD.YValues));

            //content curve xml
            rowsList.Add(ExtentionMethods.CreateXMLCurveString(ot.GetContentPercentDD.Distribution, ot.GetContentPercentDD.XValues, ot.GetContentPercentDD.YValues));
            //vehicle curve xml
            rowsList.Add(ExtentionMethods.CreateXMLCurveString(ot.GetVehiclePercentDD.Distribution, ot.GetVehiclePercentDD.XValues, ot.GetVehiclePercentDD.YValues));
            //other curve xml
            rowsList.Add(ExtentionMethods.CreateXMLCurveString(ot.GetOtherPercentDD.Distribution, ot.GetOtherPercentDD.XValues, ot.GetOtherPercentDD.YValues));


            return rowsList;
        }

        private object[] GetOccTypeInfoArray(Consequences_Assist.ComputableObjects.OccupancyType ot)
        {
            return new object[] { ot.Name, ot.Description, ot.DamageCategory.Name };

        }

        private object[] GetContinuousDistributionArray(Statistics.ContinuousDistribution cd)
        {
            object[] rowItems = new object[4];

            if (cd.GetType() == typeof(Statistics.None))
            {
                return new object[] { "None", 0, 0, 0 };
            }
            else if (cd.GetType() == typeof(Statistics.Normal))
            {
                double stDev = ((Statistics.Normal)cd).GetStDev;
                return new object[] { "Normal", 0, 0, stDev };

            }
            else if (cd.GetType() == typeof(Statistics.Triangular))
            {
                double min = ((Statistics.Triangular)cd).getMin;
                double max = ((Statistics.Triangular)cd).getMax;
                return new object[] { "Triangular", min, max, 0 };

            }
            else if (cd.GetType() == typeof(Statistics.Uniform))
            {
                double min = ((Statistics.Uniform)cd).GetMin;
                double max = ((Statistics.Uniform)cd).GetMax;
                return new object[] { "Uniform", min, max, 0 };

            }

            return rowItems;
        }



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
