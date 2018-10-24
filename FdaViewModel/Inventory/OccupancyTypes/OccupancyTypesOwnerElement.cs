using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using System.ComponentModel;
using FdaViewModel.Utilities;
using System.Xml.Linq;
using System.Xml;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    //[Author(q0heccdm, 7 / 6 / 2017 10:22:36 AM)]
    public class OccupancyTypesOwnerElement : Utilities.ParentElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 7/6/2017 10:22:36 AM
        #endregion
        #region Fields
        //private static List<Consequences_Assist.ComputableObjects.OccupancyType> _ListOfOccupancyTypes;
        #endregion
        #region Properties
        //private Dictionary<string, bool[]> _OcctypeTabsSelectedDictionary;

       
        public static List<OccupancyTypesElement> ListOfOccupancyTypesGroups { get; set; } = new List<OccupancyTypesElement>();
        //public static List<Consequences_Assist.ComputableObjects.OccupancyType> ListOfOccupancyTypes
        //{
        //    get { return _ListOfOccupancyTypes; }
        //    set { _ListOfOccupancyTypes = value; }
        //}
        #endregion
        #region Constructors
        public OccupancyTypesOwnerElement(BaseFdaElement owner):base(owner)
        {
            Name = "Occupancy Types";
            IsBold = false;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);


            Utilities.NamedAction editOccupancyTypes = new Utilities.NamedAction();
            editOccupancyTypes.Header = "Edit Occupancy Types";
            editOccupancyTypes.Action = EditOccupancyTypes;

            Utilities.NamedAction importFromFile = new Utilities.NamedAction();
            importFromFile.Header = "Import From File";
            importFromFile.Action = ImportFromFile;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(importFromFile);
            localActions.Add(editOccupancyTypes);

            Actions = localActions;
        }

       


        #endregion
        #region Voids
        private void EditOccupancyTypes(object arg1, EventArgs arg2)
        {

            //dont open the editor if there are no occtype groups to edit
            if(ListOfOccupancyTypesGroups.Count<1)
            {
                Utilities.CustomMessageBoxVM messageBox = new Utilities.CustomMessageBoxVM(Utilities.CustomMessageBoxVM.ButtonsEnum.OK,"There are no occupancy types to edit. You must first import a group of occupancy types.");
                Navigate(messageBox);
                return;
            }

            OccupancyTypesEditorVM vm = new OccupancyTypesEditorVM();
            Navigate(vm);
            if (!vm.WasCanceled)
            {
                if (!vm.HasError)
                {
                    //foreach (OccupancyTypesElement ote in ListOfOccupancyTypesGroups)
                    for(int i = 0;i<ListOfOccupancyTypesGroups.Count;i++)
                    {
                        //foreach (OccupancyTypesElement ote in vm.OccTypeGroups)
                        for(int j = 0;j<vm.OccTypeGroups.Count;j++)
                        {
                            if (ListOfOccupancyTypesGroups[i].Name == vm.OccTypeGroups[j].Name)
                            {
                                ListOfOccupancyTypesGroups[i] = vm.OccTypeGroups[j];
                            }
                        }
                    }
                    //now save the changes
                    //CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "", "Loading...");

                    //SaveFilesOnBackgroundThread(this, new DoWorkEventArgs(ListOfOccupancyTypesGroups));
                    foreach (OccupancyTypesElement elem in ListOfOccupancyTypesGroups)
                    {
                        elem.Save();
                    }
                }
            }

        }


        private async void SaveFilesOnBackgroundThread(object sender, DoWorkEventArgs e)
        {
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "", " -Saving",true);
            List<Utilities.NamedAction> tempActions = new List<Utilities.NamedAction>(Actions);
            Actions = new List<Utilities.NamedAction>();

            object[] args = (object[])e.Argument;
            List<OccupancyTypesElement> elementsToSave = (List<OccupancyTypesElement>)args[0];
            List<Utilities.NamedAction> actions = (List < Utilities.NamedAction >) args[1];

            actions.Clear();

            await Task.Run(() =>
            {
                foreach (OccupancyTypesElement elem in elementsToSave)
                {
                    elem.Save();
                }
                    //owner.AddElement(ote);
                    //AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(ote.Name, Utilities.Transactions.TransactionEnum.CreateNew, "", nameof(OccupancyTypesElement)));
                
            });

            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);
            SaveTableWithoutSavingElements();
            Actions = tempActions;
        }
        private void ImportFromFile(object arg1, EventArgs arg2)
        {

            ImportOccupancyTypesVM vm = new ImportOccupancyTypesVM();
            Navigate(vm);
            if (!vm.WasCanceled)
            {
                if (!vm.HasError)
                {
                    //object[] arguments = new object[] { vm, this };


                    List<OccupancyTypesElement> elementsToSave = new List<OccupancyTypesElement>();
                    foreach (OccupancyTypesGroupRowItemVM row in vm.ListOfRowVMs)
                    {
                        //create a dummy tabs checked dictionary
                        Dictionary<string, bool[]> _OcctypeTabsSelectedDictionary = new Dictionary<string, bool[]>();

                        foreach (Consequences_Assist.ComputableObjects.OccupancyType ot in row.ListOfOccTypes)
                        {
                            bool[] tabsCheckedArray = new bool[] { true, true, true, false };
                            _OcctypeTabsSelectedDictionary.Add(ot.Name, tabsCheckedArray);

                        }

                        OccupancyTypesElement elem = new OccupancyTypesElement(row.Name, row.ListOfOccTypes, _OcctypeTabsSelectedDictionary, this);
                        OccupancyTypesOwnerElement.ListOfOccupancyTypesGroups.Add(elem);
                        elementsToSave.Add(elem);
                    }
                    object[] args = new object[] { elementsToSave, Actions};
                    SaveFilesOnBackgroundThread(this, new DoWorkEventArgs(args));
                }
            }
        }

       
        #endregion
        #region Functions
        #endregion
        public  string TableName
        {
            get
            {
                return "OccupancyTypeGroups";
            }
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        private void SaveTableWithoutSavingElements()
        {
            Storage.Connection.Instance.DeleteTable(TableName); // always delete owner tables, and rewrite them.  This simplifies checking for removal, sorting, or adding owned elements.
            string[] names = new string[] { "Group Name" };
            Type[] types = new Type[] { typeof(string) };
            Storage.Connection.Instance.CreateTable(TableName, names, types);
            DataBase_Reader.DataTableView tbl = Storage.Connection.Instance.GetTable(TableName);
            foreach (OccupancyTypesElement ele in ListOfOccupancyTypesGroups)
            {
                tbl.AddRow(new object[] { ele.Name });
            }
            tbl.ApplyEdits();
        }

        public  void Save()
        {
            Storage.Connection.Instance.DeleteTable(TableName); // always delete owner tables, and rewrite them.  This simplifies checking for removal, sorting, or adding owned elements.
            string[] names =new string[] { "Group Name" };
            Type[] types = new Type[] { typeof(string) };
            Storage.Connection.Instance.CreateTable(TableName, names, types);
            DataBase_Reader.DataTableView tbl = Storage.Connection.Instance.GetTable(TableName);
            foreach (OccupancyTypesElement ele in ListOfOccupancyTypesGroups)
            {
                tbl.AddRow(new object[] { ele.Name });
                ele.Save();
            }
            tbl.ApplyEdits();
        }

      

        //public override void AddBaseElements()
        //{
        //    throw new NotImplementedException();
        //}

        //public override string[] TableColumnNames()
        //{
        //    return new string[] { "Occtype Group Name" };
        //}

        //public override Type[] TableColumnTypes()
        //{
        //    return new Type[] { typeof(string)};
        //}

        //public override ChildElement CreateElementFromRowData(object[] rowData)
        //{
        //    return null;
        //}

        private Statistics.UncertainCurveIncreasing GetNormalDistributionFromXML(string xmlString)
        {
            List<double> xValues = new List<double>();
            List<Statistics.ContinuousDistribution> yValues = new List<Statistics.ContinuousDistribution>();

            XDocument xDoc = XDocument.Parse(xmlString);
            XElement normalElement = xDoc.Element("NormalDistribution");
            foreach(XElement ele in normalElement.Elements("Ordinate"))
            {
                xValues.Add((double)ele.Attribute("x"));
                yValues.Add(new Statistics.Normal(Convert.ToDouble(ele.Attribute("mean").Value), Convert.ToDouble(ele.Attribute("stDev").Value)));

            }

            return new Statistics.UncertainCurveIncreasing(xValues, yValues, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal); 
        }

        private Statistics.UncertainCurveIncreasing GetTriangularDistributionFromXML(string xmlString)
        {
            List<double> xValues = new List<double>();
            List<Statistics.ContinuousDistribution> yValues = new List<Statistics.ContinuousDistribution>();

            XDocument xDoc = XDocument.Parse(xmlString);
            XElement normalElement = xDoc.Element("TriangularDistribution");
            foreach (XElement ele in normalElement.Elements("Ordinate"))
            {
                xValues.Add((double)ele.Attribute("x"));
                yValues.Add(new Statistics.Triangular(Convert.ToDouble(ele.Attribute("min").Value), Convert.ToDouble(ele.Attribute("max").Value), Convert.ToDouble(ele.Attribute("mostLikely").Value)));
            }
            return new Statistics.UncertainCurveIncreasing(xValues, yValues, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular);
        }

        private Statistics.UncertainCurveIncreasing GetUniformDistributionFromXML(string xmlString)
        {
            List<double> xValues = new List<double>();
            List<Statistics.ContinuousDistribution> yValues = new List<Statistics.ContinuousDistribution>();

            XDocument xDoc = XDocument.Parse(xmlString);
            XElement normalElement = xDoc.Element("UniformDistribution");
            foreach (XElement ele in normalElement.Elements("Ordinate"))
            {
                xValues.Add((double)ele.Attribute("x"));
                yValues.Add(new Statistics.Uniform(Convert.ToDouble(ele.Attribute("min").Value), Convert.ToDouble(ele.Attribute("max").Value)));
            }
            return new Statistics.UncertainCurveIncreasing(xValues, yValues, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform);
        }

        private Statistics.UncertainCurveIncreasing GetNoneDistributionFromXML(string xmlString)
        {
            List<double> xValues = new List<double>();
            List<Statistics.ContinuousDistribution> yValues = new List<Statistics.ContinuousDistribution>();

            XDocument xDoc = XDocument.Parse(xmlString);
            XElement normalElement = xDoc.Element("NoneDistribution");
            foreach (XElement ele in normalElement.Elements("Ordinate"))
            {
                xValues.Add((double)ele.Attribute("x"));
                yValues.Add(new Statistics.None(Convert.ToDouble(ele.Attribute("y").Value)));
            }
            return new Statistics.UncertainCurveIncreasing(xValues, yValues, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
        }

        /// <summary>
        /// This gets called when loading the study.
        /// </summary>
        /// <param name="rowData"></param>
        //public  void AddElementFromRowData(object[] rowData)
        //{
        //    List<Consequences_Assist.ComputableObjects.OccupancyType> TempOccTypes = new List<Consequences_Assist.ComputableObjects.OccupancyType>();
        //    Dictionary<string, bool[]> dummyDictionary = new Dictionary<string, bool[]>();
        //    OccupancyTypesElement ele = new OccupancyTypesElement(rowData[0].ToString(), TempOccTypes,dummyDictionary, this);

        //    int lastRow = Storage.Connection.Instance.GetTable(ele.TableName).NumberOfRows - 1;

        
        //    Dictionary<string, bool[]> selectedTabsDictionary = new Dictionary<string, bool[]>();    


        //List<Consequences_Assist.ComputableObjects.OccupancyType> listOfOccTypes = new List<Consequences_Assist.ComputableObjects.OccupancyType>();

        //    foreach (object[] row in Storage.Connection.Instance.GetTable(ele.TableName).GetRows(0, lastRow))
        //    {
        //        bool[] selectedTabs = new bool[] { Convert.ToBoolean(row[7]), Convert.ToBoolean(row[13]), Convert.ToBoolean(row[19]), Convert.ToBoolean(row[25]) };
        //        selectedTabsDictionary.Add(row[0].ToString(), selectedTabs);
        //        //ele.RelativePathAndProbability.Add(new PathAndProbability(row[0].ToString(), Convert.ToDouble(row[1])));
        //        Consequences_Assist.ComputableObjects.OccupancyType ot = new Consequences_Assist.ComputableObjects.OccupancyType();
        //        ot.Name = row[0].ToString();
        //        ot.Description = row[1].ToString();
        //        ot.DamageCategoryName = row[2].ToString();
                
        //        ot.FoundationHeightUncertainty = CreateContinuousDistributionFromRow(row, 3, 6);

        //        //***************************
        //        //structures
        //        //*****************************

        //       // if(Convert.ToBoolean(row[7]) == true) // if structures tab is checked
        //        {
        //            ot.StructureValueUncertainty = CreateContinuousDistributionFromRow(row, 8, 11);
        //            if (row[12].ToString() == "Normal")
        //            {
        //                Statistics.UncertainCurveIncreasing uci = GetNormalDistributionFromXML(row[32].ToString());
        //                ot.SetStructurePercentDD = uci;
        //            }
        //            else if (row[12].ToString() == "None")
        //            {
        //                Statistics.UncertainCurveIncreasing uci = GetNoneDistributionFromXML(row[32].ToString());
        //                ot.SetStructurePercentDD = uci;
        //            }
        //            else if (row[12].ToString() == "Triangular")
        //            {
        //                Statistics.UncertainCurveIncreasing uci = GetTriangularDistributionFromXML(row[32].ToString());
        //                ot.SetStructurePercentDD = uci;
        //            }
        //            else if (row[12].ToString() == "Uniform")
        //            {
        //                Statistics.UncertainCurveIncreasing uci = GetUniformDistributionFromXML(row[32].ToString());
        //                ot.SetStructurePercentDD = uci;
        //            }

        //        }



        //        //*****************************
        //        //content
        //        //*****************************

        //        //if (Convert.ToBoolean(row[13]) == true) // if content tab is checked
        //        {
        //            ot.ContentValueUncertainty = CreateContinuousDistributionFromRow(row, 14, 17);
        //            if (row[18].ToString() == "Normal")
        //            {
        //                DataBase_Reader.DataTableView dtv = Storage.Connection.Instance.GetTable(row[31] + " - " + ot.Name + " - ContDDCurve");
        //                if (dtv != null)
        //                {
        //                    List<object> listOfXValues = dtv.GetColumn(0).ToList();
        //                    List<double> listOfXValuesAsDoubles = new List<double>();
        //                    foreach (object o in listOfXValues) { listOfXValuesAsDoubles.Add(Convert.ToDouble(o)); }
        //                    List<Statistics.ContinuousDistribution> ys = new List<Statistics.ContinuousDistribution>();
        //                    for (int k = 0; k < dtv.NumberOfRows; k++)
        //                    {
        //                        ys.Add(new Statistics.Normal(Convert.ToDouble(dtv.GetCell(1, k)), Convert.ToDouble(dtv.GetCell(2, k))));
        //                    }
        //                    Statistics.UncertainCurveIncreasing uci = new Statistics.UncertainCurveIncreasing(listOfXValuesAsDoubles, ys, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal);
        //                    ot.SetContentPercentDD = uci;
        //                    //ot.SetContentPercentDD = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal);
        //                }
        //            }
        //            else if (row[18].ToString() == "None")
        //            {
        //                ot.SetContentPercentDD = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

        //            }
        //            else if (row[18].ToString() == "Triangular")
        //            {
        //                DataBase_Reader.DataTableView dtv = Storage.Connection.Instance.GetTable(row[31] + " - " + ot.Name + " - ContDDCurve");
        //                if (dtv != null)
        //                {
        //                    List<object> listOfXValues = dtv.GetColumn(0).ToList();
        //                    List<double> listOfXValuesAsDoubles = new List<double>();
        //                    foreach (object o in listOfXValues) { listOfXValuesAsDoubles.Add(Convert.ToDouble(o)); }
        //                    List<Statistics.ContinuousDistribution> ys = new List<Statistics.ContinuousDistribution>();
        //                    for (int k = 0; k < dtv.NumberOfRows; k++)
        //                    {
        //                        ys.Add(new Statistics.Triangular(Convert.ToDouble(dtv.GetCell(1, k)), Convert.ToDouble(dtv.GetCell(2, k)), 0));
        //                    }
        //                    Statistics.UncertainCurveIncreasing uci = new Statistics.UncertainCurveIncreasing(listOfXValuesAsDoubles, ys, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular);
        //                    ot.SetContentPercentDD = uci;
        //                    //ot.SetContentPercentDD = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular);
        //                }
        //            }
        //            else if (row[18].ToString() == "Uniform")
        //            {
        //                DataBase_Reader.DataTableView dtv = Storage.Connection.Instance.GetTable(row[31] + " - " + ot.Name + " - ContDDCurve");
        //                if (dtv != null)
        //                {
        //                    List<object> listOfXValues = dtv.GetColumn(0).ToList();
        //                    List<double> listOfXValuesAsDoubles = new List<double>();
        //                    foreach (object o in listOfXValues) { listOfXValuesAsDoubles.Add(Convert.ToDouble(o)); }
        //                    List<Statistics.ContinuousDistribution> ys = new List<Statistics.ContinuousDistribution>();
        //                    for (int k = 0; k < dtv.NumberOfRows; k++)
        //                    {
        //                        ys.Add(new Statistics.Uniform(Convert.ToDouble(dtv.GetCell(1, k)), Convert.ToDouble(dtv.GetCell(2, k))));
        //                    }
        //                    Statistics.UncertainCurveIncreasing uci = new Statistics.UncertainCurveIncreasing(listOfXValuesAsDoubles, ys, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform);
        //                    ot.SetContentPercentDD = uci;
        //                    //ot.SetContentPercentDD = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform);
        //                }
        //            }

        //        }



        //        //*****************************
        //        //vehicle
        //        //*****************************

        //        //if (Convert.ToBoolean(row[19]) == true) // if vehicle tab is checked
        //        {
        //            ot.VehicleValueUncertainty = CreateContinuousDistributionFromRow(row, 20, 23);
        //            if (row[24].ToString() == "Normal")
        //            {
        //                DataBase_Reader.DataTableView dtv = Storage.Connection.Instance.GetTable(row[31] + " - " + ot.Name + " - VehDDCurve");
        //                if (dtv != null)
        //                {
        //                    List<object> listOfXValues = dtv.GetColumn(0).ToList();
        //                    List<double> listOfXValuesAsDoubles = new List<double>();
        //                    foreach (object o in listOfXValues) { listOfXValuesAsDoubles.Add(Convert.ToDouble(o)); }
        //                    List<Statistics.ContinuousDistribution> ys = new List<Statistics.ContinuousDistribution>();
        //                    for (int k = 0; k < dtv.NumberOfRows; k++)
        //                    {
        //                        ys.Add(new Statistics.Normal(Convert.ToDouble(dtv.GetCell(1, k)), Convert.ToDouble(dtv.GetCell(2, k))));
        //                    }
        //                    Statistics.UncertainCurveIncreasing uci = new Statistics.UncertainCurveIncreasing(listOfXValuesAsDoubles, ys, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal);
        //                    ot.SetVehiclePercentDD = uci;
        //                    //ot.SetVehiclePercentDD = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal);
        //                }
        //            }
        //            else if (row[24].ToString() == "None")
        //            {
        //                ot.SetVehiclePercentDD = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

        //            }
        //            else if (row[24].ToString() == "Triangular")
        //            {
        //                DataBase_Reader.DataTableView dtv = Storage.Connection.Instance.GetTable(row[31] + " - " + ot.Name + " - VehDDCurve");
        //                if (dtv != null)
        //                {
        //                    List<object> listOfXValues = dtv.GetColumn(0).ToList();
        //                    List<double> listOfXValuesAsDoubles = new List<double>();
        //                    foreach (object o in listOfXValues) { listOfXValuesAsDoubles.Add(Convert.ToDouble(o)); }
        //                    List<Statistics.ContinuousDistribution> ys = new List<Statistics.ContinuousDistribution>();
        //                    for (int k = 0; k < dtv.NumberOfRows; k++)
        //                    {
        //                        ys.Add(new Statistics.Triangular(Convert.ToDouble(dtv.GetCell(1, k)), Convert.ToDouble(dtv.GetCell(2, k)), 0));
        //                    }
        //                    Statistics.UncertainCurveIncreasing uci = new Statistics.UncertainCurveIncreasing(listOfXValuesAsDoubles, ys, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular);
        //                    ot.SetVehiclePercentDD = uci;
        //                    //ot.SetVehiclePercentDD = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular);
        //                }
        //            }
        //            else if (row[24].ToString() == "Uniform")
        //            {
        //                DataBase_Reader.DataTableView dtv = Storage.Connection.Instance.GetTable(row[31] + " - " + ot.Name + " - VehDDCurve");
        //                if (dtv != null)
        //                {
        //                    List<object> listOfXValues = dtv.GetColumn(0).ToList();
        //                    List<double> listOfXValuesAsDoubles = new List<double>();
        //                    foreach (object o in listOfXValues) { listOfXValuesAsDoubles.Add(Convert.ToDouble(o)); }
        //                    List<Statistics.ContinuousDistribution> ys = new List<Statistics.ContinuousDistribution>();
        //                    for (int k = 0; k < dtv.NumberOfRows; k++)
        //                    {
        //                        ys.Add(new Statistics.Uniform(Convert.ToDouble(dtv.GetCell(1, k)), Convert.ToDouble(dtv.GetCell(2, k))));
        //                    }
        //                    Statistics.UncertainCurveIncreasing uci = new Statistics.UncertainCurveIncreasing(listOfXValuesAsDoubles, ys, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform);
        //                    ot.SetVehiclePercentDD = uci;
        //                    //ot.SetVehiclePercentDD = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform);
        //                }
        //            }

        //        }


        //        //*****************************
        //        //Other
        //        //*****************************

        //        //if (Convert.ToBoolean(row[25]) == true) // if other tab is checked
        //        {
        //            ot.VehicleValueUncertainty = CreateContinuousDistributionFromRow(row, 26, 29);
        //            if (row[30].ToString() == "Normal")
        //            {
        //                DataBase_Reader.DataTableView dtv = Storage.Connection.Instance.GetTable(row[31] + " - " + ot.Name + " - OtherDDCurve");
        //                if (dtv != null)
        //                {
        //                    List<object> listOfXValues = dtv.GetColumn(0).ToList();
        //                    List<double> listOfXValuesAsDoubles = new List<double>();
        //                    foreach (object o in listOfXValues) { listOfXValuesAsDoubles.Add(Convert.ToDouble(o)); }
        //                    List<Statistics.ContinuousDistribution> ys = new List<Statistics.ContinuousDistribution>();
        //                    for (int k = 0; k < dtv.NumberOfRows; k++)
        //                    {
        //                        ys.Add(new Statistics.Normal(Convert.ToDouble(dtv.GetCell(1, k)), Convert.ToDouble(dtv.GetCell(2, k))));
        //                    }
        //                    Statistics.UncertainCurveIncreasing uci = new Statistics.UncertainCurveIncreasing(listOfXValuesAsDoubles, ys, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal);
        //                    ot.SetOtherPercentDD = uci;
        //                    //ot.SetVehiclePercentDD = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal);
        //                }
        //            }
        //            else if (row[30].ToString() == "None")
        //            {
        //                ot.SetOtherPercentDD = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

        //            }
        //            else if (row[30].ToString() == "Triangular")
        //            {
        //                DataBase_Reader.DataTableView dtv = Storage.Connection.Instance.GetTable(row[31] + " - " + ot.Name + " - OtherDDCurve");
        //                if (dtv != null)
        //                {
        //                    List<object> listOfXValues = dtv.GetColumn(0).ToList();
        //                    List<double> listOfXValuesAsDoubles = new List<double>();
        //                    foreach (object o in listOfXValues) { listOfXValuesAsDoubles.Add(Convert.ToDouble(o)); }
        //                    List<Statistics.ContinuousDistribution> ys = new List<Statistics.ContinuousDistribution>();
        //                    for (int k = 0; k < dtv.NumberOfRows; k++)
        //                    {
        //                        ys.Add(new Statistics.Triangular(Convert.ToDouble(dtv.GetCell(1, k)), Convert.ToDouble(dtv.GetCell(2, k)), 0));
        //                    }
        //                    Statistics.UncertainCurveIncreasing uci = new Statistics.UncertainCurveIncreasing(listOfXValuesAsDoubles, ys, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular);
        //                    ot.SetOtherPercentDD = uci;
        //                    //ot.SetVehiclePercentDD = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular);
        //                }
        //            }
        //            else if (row[30].ToString() == "Uniform")
        //            {
        //                DataBase_Reader.DataTableView dtv = Storage.Connection.Instance.GetTable(row[31] + " - " + ot.Name + " - OtherDDCurve");
        //                if (dtv != null)
        //                {
        //                    List<object> listOfXValues = dtv.GetColumn(0).ToList();
        //                    List<double> listOfXValuesAsDoubles = new List<double>();
        //                    foreach (object o in listOfXValues) { listOfXValuesAsDoubles.Add(Convert.ToDouble(o)); }
        //                    List<Statistics.ContinuousDistribution> ys = new List<Statistics.ContinuousDistribution>();
        //                    for (int k = 0; k < dtv.NumberOfRows; k++)
        //                    {
        //                        ys.Add(new Statistics.Uniform(Convert.ToDouble(dtv.GetCell(1, k)), Convert.ToDouble(dtv.GetCell(2, k))));
        //                    }
        //                    Statistics.UncertainCurveIncreasing uci = new Statistics.UncertainCurveIncreasing(listOfXValuesAsDoubles, ys, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform);
        //                    ot.SetOtherPercentDD = uci;
        //                    //ot.SetVehiclePercentDD = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform);
        //                }
        //            }

        //        }

        //        listOfOccTypes.Add(ot);
        //    }
        //    ele.ListOfOccupancyTypes = listOfOccTypes;
        //    ele.OccTypesSelectedTabsDictionary = selectedTabsDictionary;
        //    ListOfOccupancyTypesGroups.Add(ele);
        //    //AddElement(ele,false);
        //}

       
        private Statistics.ContinuousDistribution CreateContinuousDistributionFromRow(object[] row,int start, int end)
        {

            if (row[start].ToString() == "Normal")
            {
                Statistics.Normal norm = new Statistics.Normal(0, Convert.ToDouble(row[end]));
                return norm;
            }
            else if (row[start].ToString() == "Uniform")
            {
                Statistics.Uniform uni = new Statistics.Uniform(Convert.ToDouble(row[start]) , Convert.ToDouble(row[start+1]));
                return uni;
            }
            else if (row[start].ToString() == "None")
            {
                Statistics.None non = new Statistics.None();
                return non;

            }
            else if (row[start].ToString() == "Triangular")
            {
                Statistics.Uniform tri = new Statistics.Uniform(Convert.ToDouble(row[start]), Convert.ToDouble(row[start + 1]));
                return tri;
            }

                return new Statistics.Normal(); // it should never get here.
        }


    }
}
