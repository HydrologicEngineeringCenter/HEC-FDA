using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    //[Author(q0heccdm, 7 / 11 / 2017 3:23:11 PM)]
    public class OccupancyTypesElement : Utilities.OwnedElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 7/11/2017 3:23:11 PM
        #endregion
        #region Fields
        #endregion
        #region Properties
            public Dictionary<string,bool[]> OccTypesSelectedTabsDictionary { get; set; }
            //public string OccTypesGroupName { get; set; }
        public List<Consequences_Assist.ComputableObjects.OccupancyType> ListOfOccupancyTypes { get; set; }

        

        #endregion
        #region Constructors
        public OccupancyTypesElement(BaseFdaElement owner):base(owner)
        {

        }
        public OccupancyTypesElement(string occTypesGroupName, List<Consequences_Assist.ComputableObjects.OccupancyType> listOfOccTypes, Dictionary<string,bool[]> occtypesSelectedTabs, BaseFdaElement owner):base(owner)
        {
            Name = occTypesGroupName;
            OccTypesSelectedTabsDictionary = occtypesSelectedTabs;
            //OccTypesGroupName = occTypesGroupName;
            ListOfOccupancyTypes = listOfOccTypes;
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                if (Storage.Connection.Instance.TableNames().Contains(TableName))
                {
                    //already exists... delete?
                    Storage.Connection.Instance.DeleteTable(TableName);
                }

                string[] colNames = new string[] { "Name", "Description", "DamageCategory","VarInFoundHtType","FdHtMin","FdHtMax","FdHtStDev","IsStructChecked","VarInStructValueType","StructMin","StructMax","StructStDev","StructDistType", "IsContChecked", "VarInContValueType", "ContMin", "ContMax", "ContStDev", "ContDistType", "IsVehChecked", "VarInVehValueType", "VehMin", "VehMax", "VehStDev", "VehDistType", "IsOtherChecked", "VarInOtherValueType", "OtherMin", "OtherMax", "OtherStDev", "OtherDistType","GroupName" };
                Type[] colTypes = new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(double), typeof(double), typeof(double), typeof(bool), typeof(string), typeof(double), typeof(double), typeof(double), typeof(string), typeof(bool), typeof(string), typeof(double), typeof(double), typeof(double), typeof(string), typeof(bool), typeof(string), typeof(double), typeof(double), typeof(double), typeof(string), typeof(bool), typeof(string), typeof(double), typeof(double), typeof(double), typeof(string),typeof(string) };

                Storage.Connection.Instance.CreateTable(TableName, colNames, colTypes);
                DataBase_Reader.DataTableView tbl = Storage.Connection.Instance.GetTable(TableName);

                List<object[]> rows = new List<object[]>();
                List<object> rowsList = new List<object>();

                int i = 0;
                foreach (Consequences_Assist.ComputableObjects.OccupancyType ot in ListOfOccupancyTypes)
                {


                    //write a table for this occtypes structure depth damage curve

                    System.Data.DataTable dtable = new System.Data.DataTable(Name + " - " + ot.Name + " - StructDDCurve");
                    if (Storage.Connection.Instance.TableNames().Contains(dtable.TableName))
                    {
                        //already exists... delete?
                        Storage.Connection.Instance.DeleteTable(dtable.TableName);
                    }

                    if (ot.GetStructurePercentDD.YValues.Count > 0)
                    {
                        if (ot.GetStructurePercentDD.YValues[0].GetType() == typeof(Statistics.Normal))
                        {
                            dtable.Columns.Add("X", typeof(double));
                            dtable.Columns.Add("Mean", typeof(double));
                            dtable.Columns.Add("StDev", typeof(double));

                            for (int j = 0; j < ot.GetStructurePercentDD.XValues.Count; j++)
                            {
                                object[] ddRow = new object[3];
                                ddRow[0] = ot.GetStructurePercentDD.XValues[j];
                                ddRow[1] = ((Statistics.Normal)ot.GetStructurePercentDD.YValues[j]).GetMean;
                                ddRow[2] = ((Statistics.Normal)ot.GetStructurePercentDD.YValues[j]).GetStDev;
                                dtable.Rows.Add(ddRow);
                            }
                            Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }
                        else if (ot.GetStructurePercentDD.YValues[0].GetType() == typeof(Statistics.Triangular))
                        {
                            dtable.Columns.Add("X", typeof(double));
                            dtable.Columns.Add("Min", typeof(double));
                            dtable.Columns.Add("Max", typeof(double));
                            dtable.Columns.Add("MostLikely", typeof(double));


                            for (int j = 0; j < ot.GetStructurePercentDD.XValues.Count; j++)
                            {
                                object[] ddRow = new object[4];
                                ddRow[0] = ot.GetStructurePercentDD.XValues[j];
                                ddRow[1] = ((Statistics.Triangular)ot.GetStructurePercentDD.YValues[j]).getMin;
                                ddRow[2] = ((Statistics.Triangular)ot.GetStructurePercentDD.YValues[j]).getMax;
                                ddRow[3] = ((Statistics.Triangular)ot.GetStructurePercentDD.YValues[j]).getMostlikely;

                                dtable.Rows.Add(ddRow);
                            }
                            Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }
                        else if (ot.GetStructurePercentDD.YValues[0].GetType() == typeof(Statistics.Uniform))
                        {
                            dtable.Columns.Add("X", typeof(double));
                            dtable.Columns.Add("Min", typeof(double));
                            dtable.Columns.Add("Max", typeof(double));

                            for (int j = 0; j < ot.GetStructurePercentDD.XValues.Count; j++)
                            {
                                object[] ddRow = new object[3];
                                ddRow[0] = ot.GetStructurePercentDD.XValues[j];
                                ddRow[1] = ((Statistics.Uniform)ot.GetStructurePercentDD.YValues[j]).GetMin;
                                ddRow[2] = ((Statistics.Uniform)ot.GetStructurePercentDD.YValues[j]).GetMax;

                                dtable.Rows.Add(ddRow);
                            }
                            Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }
                        else if (ot.GetStructurePercentDD.YValues[0].GetType() == typeof(Statistics.Uniform))
                        {
                            dtable.Columns.Add("X", typeof(double));
                            dtable.Columns.Add("Min", typeof(double));
                            dtable.Columns.Add("Max", typeof(double));

                            for (int j = 0; j < ot.GetStructurePercentDD.XValues.Count; j++)
                            {
                                object[] ddRow = new object[3];
                                ddRow[0] = ot.GetStructurePercentDD.XValues[j];
                                ddRow[1] = ((Statistics.Uniform)ot.GetStructurePercentDD.YValues[j]).GetMin;
                                ddRow[2] = ((Statistics.Uniform)ot.GetStructurePercentDD.YValues[j]).GetMax;

                                dtable.Rows.Add(ddRow);
                            }
                            Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }
                        else if (ot.GetStructurePercentDD.YValues[0].GetType() == typeof(Statistics.None))
                        {
                            //dtable.Columns.Add("X", typeof(double));
                            //dtable.Columns.Add("Y", typeof(double));

                            //for (int j = 0; j < ot.GetStructurePercentDD.XValues.Count; j++)
                            //{
                            //    object[] ddRow = new object[4];
                            //    ddRow[0] = ot.GetStructurePercentDD.XValues[j];
                            //    ddRow[1] = ((Statistics.None)ot.GetStructurePercentDD.YValues[j]).;

                            //    dtable.Rows.Add(ddRow);
                            //}
                            //Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }

                    }

                    //write a table for content dd curve

                    if (ot.GetContentPercentDD.YValues.Count > 0)
                    {
                        dtable = new System.Data.DataTable(Name + " - " + ot.Name + " - ContDDCurve");
                        if (Storage.Connection.Instance.TableNames().Contains(dtable.TableName))
                        {
                            //already exists... delete?
                            Storage.Connection.Instance.DeleteTable(dtable.TableName);
                        }

                        if (ot.GetContentPercentDD.YValues[0].GetType() == typeof(Statistics.Normal))
                        {
                            dtable.Columns.Add("X", typeof(double));
                            dtable.Columns.Add("Mean", typeof(double));
                            dtable.Columns.Add("StDev", typeof(double));

                            for (int j = 0; j < ot.GetContentPercentDD.XValues.Count; j++)
                            {
                                object[] ddRow = new object[3];
                                ddRow[0] = ot.GetContentPercentDD.XValues[j];
                                ddRow[1] = ((Statistics.Normal)ot.GetContentPercentDD.YValues[j]).GetMean;
                                ddRow[2] = ((Statistics.Normal)ot.GetContentPercentDD.YValues[j]).GetStDev;
                                dtable.Rows.Add(ddRow);
                            }
                            Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }
                        else if (ot.GetContentPercentDD.YValues[0].GetType() == typeof(Statistics.Triangular))
                        {
                            dtable.Columns.Add("X", typeof(double));
                            dtable.Columns.Add("Min", typeof(double));
                            dtable.Columns.Add("Max", typeof(double));
                            dtable.Columns.Add("MostLikely", typeof(double));


                            for (int j = 0; j < ot.GetContentPercentDD.XValues.Count; j++)
                            {
                                object[] ddRow = new object[4];
                                ddRow[0] = ot.GetContentPercentDD.XValues[j];
                                ddRow[1] = ((Statistics.Triangular)ot.GetContentPercentDD.YValues[j]).getMin;
                                ddRow[2] = ((Statistics.Triangular)ot.GetContentPercentDD.YValues[j]).getMax;
                                ddRow[3] = ((Statistics.Triangular)ot.GetContentPercentDD.YValues[j]).getMostlikely;

                                dtable.Rows.Add(ddRow);
                            }
                            Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }
                        else if (ot.GetContentPercentDD.YValues[0].GetType() == typeof(Statistics.Uniform))
                        {
                            dtable.Columns.Add("X", typeof(double));
                            dtable.Columns.Add("Min", typeof(double));
                            dtable.Columns.Add("Max", typeof(double));

                            for (int j = 0; j < ot.GetContentPercentDD.XValues.Count; j++)
                            {
                                object[] ddRow = new object[4];
                                ddRow[0] = ot.GetContentPercentDD.XValues[j];
                                ddRow[1] = ((Statistics.Uniform)ot.GetContentPercentDD.YValues[j]).GetMin;
                                ddRow[2] = ((Statistics.Uniform)ot.GetContentPercentDD.YValues[j]).GetMax;

                                dtable.Rows.Add(ddRow);
                            }
                            Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }
                        else if (ot.GetContentPercentDD.YValues[0].GetType() == typeof(Statistics.Uniform))
                        {
                            dtable.Columns.Add("X", typeof(double));
                            dtable.Columns.Add("Min", typeof(double));
                            dtable.Columns.Add("Max", typeof(double));

                            for (int j = 0; j < ot.GetContentPercentDD.XValues.Count; j++)
                            {
                                object[] ddRow = new object[4];
                                ddRow[0] = ot.GetContentPercentDD.XValues[j];
                                ddRow[1] = ((Statistics.Uniform)ot.GetContentPercentDD.YValues[j]).GetMin;
                                ddRow[2] = ((Statistics.Uniform)ot.GetContentPercentDD.YValues[j]).GetMax;

                                dtable.Rows.Add(ddRow);
                            }
                            Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }
                        else if (ot.GetContentPercentDD.YValues[0].GetType() == typeof(Statistics.None))
                        {
                            //dtable.Columns.Add("X", typeof(double));
                            //dtable.Columns.Add("Y", typeof(double));

                            //for (int j = 0; j < ot.GetContentPercentDD.XValues.Count; j++)
                            //{
                            //    object[] ddRow = new object[4];
                            //    ddRow[0] = ot.GetContentPercentDD.XValues[j];
                            //    ddRow[1] = ((Statistics.None)ot.GetContentPercentDD.YValues[j]).;

                            //    dtable.Rows.Add(ddRow);
                            //}
                            //Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }
                    }

                    //write for vehicle dd curve
                    if (ot.GetVehiclePercentDD.YValues.Count > 0)
                    {

                        dtable = new System.Data.DataTable(Name + " - " + ot.Name + " - VehDDCurve");
                        if (Storage.Connection.Instance.TableNames().Contains(dtable.TableName))
                        {
                            //already exists... delete?
                            Storage.Connection.Instance.DeleteTable(dtable.TableName);
                        }

                        if (ot.GetVehiclePercentDD.YValues[0].GetType() == typeof(Statistics.Normal))
                        {
                            dtable.Columns.Add("X", typeof(double));
                            dtable.Columns.Add("Mean", typeof(double));
                            dtable.Columns.Add("StDev", typeof(double));

                            for (int j = 0; j < ot.GetVehiclePercentDD.XValues.Count; j++)
                            {
                                object[] ddRow = new object[3];
                                ddRow[0] = ot.GetVehiclePercentDD.XValues[j];
                                ddRow[1] = ((Statistics.Normal)ot.GetVehiclePercentDD.YValues[j]).GetMean;
                                ddRow[2] = ((Statistics.Normal)ot.GetVehiclePercentDD.YValues[j]).GetStDev;
                                dtable.Rows.Add(ddRow);
                            }
                            Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }
                        else if (ot.GetVehiclePercentDD.YValues[0].GetType() == typeof(Statistics.Triangular))
                        {
                            dtable.Columns.Add("X", typeof(double));
                            dtable.Columns.Add("Min", typeof(double));
                            dtable.Columns.Add("Max", typeof(double));
                            dtable.Columns.Add("MostLikely", typeof(double));


                            for (int j = 0; j < ot.GetVehiclePercentDD.XValues.Count; j++)
                            {
                                object[] ddRow = new object[4];
                                ddRow[0] = ot.GetVehiclePercentDD.XValues[j];
                                ddRow[1] = ((Statistics.Triangular)ot.GetVehiclePercentDD.YValues[j]).getMin;
                                ddRow[2] = ((Statistics.Triangular)ot.GetVehiclePercentDD.YValues[j]).getMax;
                                ddRow[3] = ((Statistics.Triangular)ot.GetVehiclePercentDD.YValues[j]).getMostlikely;

                                dtable.Rows.Add(ddRow);
                            }
                            Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }
                        else if (ot.GetVehiclePercentDD.YValues[0].GetType() == typeof(Statistics.Uniform))
                        {
                            dtable.Columns.Add("X", typeof(double));
                            dtable.Columns.Add("Min", typeof(double));
                            dtable.Columns.Add("Max", typeof(double));

                            for (int j = 0; j < ot.GetVehiclePercentDD.XValues.Count; j++)
                            {
                                object[] ddRow = new object[4];
                                ddRow[0] = ot.GetVehiclePercentDD.XValues[j];
                                ddRow[1] = ((Statistics.Uniform)ot.GetVehiclePercentDD.YValues[j]).GetMin;
                                ddRow[2] = ((Statistics.Uniform)ot.GetVehiclePercentDD.YValues[j]).GetMax;

                                dtable.Rows.Add(ddRow);
                            }
                            Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }
                        else if (ot.GetVehiclePercentDD.YValues[0].GetType() == typeof(Statistics.Uniform))
                        {
                            dtable.Columns.Add("X", typeof(double));
                            dtable.Columns.Add("Min", typeof(double));
                            dtable.Columns.Add("Max", typeof(double));

                            for (int j = 0; j < ot.GetVehiclePercentDD.XValues.Count; j++)
                            {
                                object[] ddRow = new object[4];
                                ddRow[0] = ot.GetVehiclePercentDD.XValues[j];
                                ddRow[1] = ((Statistics.Uniform)ot.GetVehiclePercentDD.YValues[j]).GetMin;
                                ddRow[2] = ((Statistics.Uniform)ot.GetVehiclePercentDD.YValues[j]).GetMax;

                                dtable.Rows.Add(ddRow);
                            }
                            Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }
                        else if (ot.GetVehiclePercentDD.YValues[0].GetType() == typeof(Statistics.None))
                        {
                            //dtable.Columns.Add("X", typeof(double));
                            //dtable.Columns.Add("Y", typeof(double));

                            //for (int j = 0; j < ot.GetVehiclePercentDD.XValues.Count; j++)
                            //{
                            //    object[] ddRow = new object[4];
                            //    ddRow[0] = ot.GetVehiclePercentDD.XValues[j];
                            //    ddRow[1] = ((Statistics.None)ot.GetVehiclePercentDD.YValues[j]).;

                            //    dtable.Rows.Add(ddRow);
                            //}
                            //Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }

                    }


                    // other dd curve

                    if (ot.GetOtherPercentDD.YValues.Count > 0)
                    {
                        dtable = new System.Data.DataTable(Name + " - " + ot.Name + " - OtherDDCurve");
                        if (Storage.Connection.Instance.TableNames().Contains(dtable.TableName))
                        {
                            //already exists... delete?
                            Storage.Connection.Instance.DeleteTable(dtable.TableName);
                        }

                        if (ot.GetOtherPercentDD.YValues[0].GetType() == typeof(Statistics.Normal))
                        {
                            dtable.Columns.Add("X", typeof(double));
                            dtable.Columns.Add("Mean", typeof(double));
                            dtable.Columns.Add("StDev", typeof(double));

                            for (int j = 0; j < ot.GetOtherPercentDD.XValues.Count; j++)
                            {
                                object[] ddRow = new object[3];
                                ddRow[0] = ot.GetOtherPercentDD.XValues[j];
                                ddRow[1] = ((Statistics.Normal)ot.GetOtherPercentDD.YValues[j]).GetMean;
                                ddRow[2] = ((Statistics.Normal)ot.GetOtherPercentDD.YValues[j]).GetStDev;
                                dtable.Rows.Add(ddRow);
                            }
                            Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }
                        else if (ot.GetOtherPercentDD.YValues[0].GetType() == typeof(Statistics.Triangular))
                        {
                            dtable.Columns.Add("X", typeof(double));
                            dtable.Columns.Add("Min", typeof(double));
                            dtable.Columns.Add("Max", typeof(double));
                            dtable.Columns.Add("MostLikely", typeof(double));


                            for (int j = 0; j < ot.GetOtherPercentDD.XValues.Count; j++)
                            {
                                object[] ddRow = new object[4];
                                ddRow[0] = ot.GetOtherPercentDD.XValues[j];
                                ddRow[1] = ((Statistics.Triangular)ot.GetOtherPercentDD.YValues[j]).getMin;
                                ddRow[2] = ((Statistics.Triangular)ot.GetOtherPercentDD.YValues[j]).getMax;
                                ddRow[3] = ((Statistics.Triangular)ot.GetOtherPercentDD.YValues[j]).getMostlikely;

                                dtable.Rows.Add(ddRow);
                            }
                            Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }
                        else if (ot.GetOtherPercentDD.YValues[0].GetType() == typeof(Statistics.Uniform))
                        {
                            dtable.Columns.Add("X", typeof(double));
                            dtable.Columns.Add("Min", typeof(double));
                            dtable.Columns.Add("Max", typeof(double));

                            for (int j = 0; j < ot.GetOtherPercentDD.XValues.Count; j++)
                            {
                                object[] ddRow = new object[4];
                                ddRow[0] = ot.GetOtherPercentDD.XValues[j];
                                ddRow[1] = ((Statistics.Uniform)ot.GetOtherPercentDD.YValues[j]).GetMin;
                                ddRow[2] = ((Statistics.Uniform)ot.GetOtherPercentDD.YValues[j]).GetMax;

                                dtable.Rows.Add(ddRow);
                            }
                            Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }
                        else if (ot.GetOtherPercentDD.YValues[0].GetType() == typeof(Statistics.Uniform))
                        {
                            dtable.Columns.Add("X", typeof(double));
                            dtable.Columns.Add("Min", typeof(double));
                            dtable.Columns.Add("Max", typeof(double));

                            for (int j = 0; j < ot.GetOtherPercentDD.XValues.Count; j++)
                            {
                                object[] ddRow = new object[4];
                                ddRow[0] = ot.GetOtherPercentDD.XValues[j];
                                ddRow[1] = ((Statistics.Uniform)ot.GetOtherPercentDD.YValues[j]).GetMin;
                                ddRow[2] = ((Statistics.Uniform)ot.GetOtherPercentDD.YValues[j]).GetMax;

                                dtable.Rows.Add(ddRow);
                            }
                            Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }
                        else if (ot.GetOtherPercentDD.YValues[0].GetType() == typeof(Statistics.None))
                        {
                            //dtable.Columns.Add("X", typeof(double));
                            //dtable.Columns.Add("Y", typeof(double));

                            //for (int j = 0; j < ot.GetOtherPercentDD.XValues.Count; j++)
                            //{
                            //    object[] ddRow = new object[4];
                            //    ddRow[0] = ot.GetOtherPercentDD.XValues[j];
                            //    ddRow[1] = ((Statistics.None)ot.GetOtherPercentDD.YValues[j]).;

                            //    dtable.Rows.Add(ddRow);
                            //}
                            //Storage.Connection.Instance.Reader.SaveDataTable(dtable);
                        }

                    }

                    rowsList.Clear();
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

                    foreach(object o in GetOccTypeInfoArray(ot))
                    {
                        rowsList.Add(o);
                    }

                    foreach (object o in GetContinuousDistributionArray(ot.FoundationHeightUncertainty))
                    {
                        rowsList.Add(o);
                    }
                    rowsList.Add(checkedTabs[0]);
                    foreach (object o in GetContinuousDistributionArray(ot.StructureValueUncertainty))
                    {
                        rowsList.Add(o);
                    }
                    rowsList.Add(ot.GetStructurePercentDD.Distribution); //"Normal"); //how do i get this?
                    rowsList.Add(checkedTabs[1]);
                    foreach (object o in GetContinuousDistributionArray(ot.ContentValueUncertainty))
                    {
                        rowsList.Add(o);
                    }
                    rowsList.Add("Normal"); //how do i get this?
                    rowsList.Add(checkedTabs[2]);
                    foreach (object o in GetContinuousDistributionArray(ot.VehicleValueUncertainty))
                    {
                        rowsList.Add(o);
                    }
                    rowsList.Add("Normal"); //how do i get this?
                    rowsList.Add(checkedTabs[3]);
                    foreach (object o in GetContinuousDistributionArray(ot.OtherValueUncertainty))
                    {
                        rowsList.Add(o);
                    }
                    rowsList.Add("Normal"); //how do i get this?

                    rowsList.Add(Name);
                    rows.Add( rowsList.ToArray());

                    i++;
                }
                //for (int j = 0; j < rows.Count(); j++)
                //{
                //    tbl.AddRow(rows[j]);
                //}
                tbl.AddRows(rows);
                tbl.ApplyEdits();


            }
        }

        private object[] GetOccTypeInfoArray(Consequences_Assist.ComputableObjects.OccupancyType ot)
        {
            return new object[] { ot.Name, ot.Description, ot.DamageCategory.Name };

        }
        private object[] GetContinuousDistributionArray(Statistics.ContinuousDistribution cd)
        {
            object[] rowItems = new object[4];

            if(cd.GetType() == typeof(Statistics.None))
            {
                return new object[] { "None", 0, 0, 0 };
            }
            else if(cd.GetType() == typeof(Statistics.Normal))
            {
                double stDev = ((Statistics.Normal)cd).GetStDev;
                return new object[] { "Normal", 0, 0, stDev };

            }
            else if(cd.GetType() == typeof(Statistics.Triangular))
            {
                double min = ((Statistics.Triangular)cd).getMin;
                double max = ((Statistics.Triangular)cd).getMax;
                return new object[] { "Triangular", min, max, 0 };

            }
            else if(cd.GetType() == typeof(Statistics.Uniform))
            {
                double min = ((Statistics.Uniform)cd).GetMin;
                double max = ((Statistics.Uniform)cd).GetMax;
                return new object[] { "Uniform", min,max, 0 };

            }

            return rowItems;
        }

        public override object[] RowData()
        {
            return new object[] { Name };
        }

        public override bool SavesToRow()
        {
            return true;
        }

        public override bool SavesToTable()
        {
            return true;
        }

        public override string GetTableConstant()
        {
            return "OccType - ";
        }
        public override string TableName
        {
            get
            {
                return GetTableConstant() + Name;
            }
        }
    }
}
