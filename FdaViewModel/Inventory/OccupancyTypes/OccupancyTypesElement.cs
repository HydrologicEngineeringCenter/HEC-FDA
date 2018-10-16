using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    //[Author(q0heccdm, 7 / 11 / 2017 3:23:11 PM)]
    public class OccupancyTypesElement : Utilities.ChildElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 7/11/2017 3:23:11 PM
        #endregion
        #region Fields
        private const string STRUCTURE_DD_CURVE_NAME = " - StructDDCurve";
        private const string CONTENT_DD_CURVE_NAME = " - ContDDCurve";
        private const string VEHICLE_DD_CURVE_NAME = " - VehDDCurve";
        private const string OTHER_DD_CURVE_NAME = " - OtherDDCurve";

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

        #region SaveTables
        //private void SaveNormalTable(System.Data.DataTable dtable, ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.ContinuousDistribution> YValues)
        //{
        //    dtable.Columns.Add("X", typeof(double));
        //    dtable.Columns.Add("Mean", typeof(double));
        //    dtable.Columns.Add("StDev", typeof(double));

        //    for (int j = 0; j < XValues.Count; j++)
        //    {
        //        object[] ddRow = new object[3];
        //        ddRow[0] = XValues[j];
        //        ddRow[1] = ((Statistics.Normal)YValues[j]).GetMean;
        //        ddRow[2] = ((Statistics.Normal)YValues[j]).GetStDev;
        //        dtable.Rows.Add(ddRow);
        //    }
        //    Storage.Connection.Instance.Reader.SaveDataTable(dtable);
        //}

        private string CreateNormalDistributionXML(ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.ContinuousDistribution> YValues)
        {
            XElement xElement = new XElement("NormalDistribution");
            for (int j = 0; j < XValues.Count; j++)
            {
                XElement ordinateElement = new XElement("Ordinate");

                ordinateElement.SetAttributeValue("x", XValues[j]);
                ordinateElement.SetAttributeValue("mean", ((Statistics.Normal)YValues[j]).GetMean);
                ordinateElement.SetAttributeValue("stDev", ((Statistics.Normal)YValues[j]).GetStDev);

                xElement.Add(ordinateElement);
            }
            return xElement.ToString();
        }

        //private void SaveTriangularTable(System.Data.DataTable dtable, ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.ContinuousDistribution> YValues)
        //{
        //    dtable.Columns.Add("X", typeof(double));
        //    dtable.Columns.Add("Min", typeof(double));
        //    dtable.Columns.Add("Max", typeof(double));
        //    dtable.Columns.Add("MostLikely", typeof(double));


        //    for (int j = 0; j < XValues.Count; j++)
        //    {
        //        object[] ddRow = new object[4];
        //        ddRow[0] = XValues[j];
        //        ddRow[1] = ((Statistics.Triangular)YValues[j]).getMin;
        //        ddRow[2] = ((Statistics.Triangular)YValues[j]).getMax;
        //        ddRow[3] = ((Statistics.Triangular)YValues[j]).getMostlikely;

        //        dtable.Rows.Add(ddRow);
        //    }
        //    Storage.Connection.Instance.Reader.SaveDataTable(dtable);

        //}
        private string CreateTriangularDistributionXML(ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.ContinuousDistribution> YValues)
        {
            XElement xElement = new XElement("TriangularDistribution");
            for (int j = 0; j < XValues.Count; j++)
            {
                XElement ordinateElement = new XElement("Ordinate");

                ordinateElement.SetAttributeValue("x", XValues[j]);
                ordinateElement.SetAttributeValue("min", ((Statistics.Triangular)YValues[j]).getMin);
                ordinateElement.SetAttributeValue("max", ((Statistics.Triangular)YValues[j]).getMax);
                ordinateElement.SetAttributeValue("mostLikely", ((Statistics.Triangular)YValues[j]).getMostlikely);

                xElement.Add(ordinateElement);
            }
            return xElement.ToString();
        }


        //private void SaveUniformTable(System.Data.DataTable dtable, ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.ContinuousDistribution> YValues)
        //{
        //    dtable.Columns.Add("X", typeof(double));
        //    dtable.Columns.Add("Min", typeof(double));
        //    dtable.Columns.Add("Max", typeof(double));

        //    for (int j = 0; j < XValues.Count; j++)
        //    {
        //        object[] ddRow = new object[3];
        //        ddRow[0] = XValues[j];
        //        ddRow[1] = ((Statistics.Uniform)YValues[j]).GetMin;
        //        ddRow[2] = ((Statistics.Uniform)YValues[j]).GetMax;

        //        dtable.Rows.Add(ddRow);
        //    }
        //    Storage.Connection.Instance.Reader.SaveDataTable(dtable);
        //}

        private string CreateUniformDistributionXML(ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.ContinuousDistribution> YValues)
        {
            XElement xElement = new XElement("UniformDistribution");
            for (int j = 0; j < XValues.Count; j++)
            {
                XElement ordinateElement = new XElement("Ordinate");

                ordinateElement.SetAttributeValue("x", XValues[j]);
                ordinateElement.SetAttributeValue("min", ((Statistics.Uniform)YValues[j]).GetMin);
                ordinateElement.SetAttributeValue("max", ((Statistics.Uniform)YValues[j]).GetMax);

                xElement.Add(ordinateElement);
            }
            return xElement.ToString();
        }

        //private void SaveNoneTable(System.Data.DataTable dtable, ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.ContinuousDistribution> YValues)
        //{
        //    dtable.Columns.Add("X", typeof(double));
        //    dtable.Columns.Add("Y", typeof(double));

        //    for (int j = 0; j < XValues.Count; j++)
        //    {
        //        object[] ddRow = new object[4];
        //        ddRow[0] = XValues[j];
        //        //ddRow[1] = ((Statistics.None)YValues[j]);
        //        ddRow[1] = YValues[j];

        //        dtable.Rows.Add(ddRow);
        //    }
        //    Storage.Connection.Instance.Reader.SaveDataTable(dtable);
        //}

        private string CreateNoneDistributionXML(ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.ContinuousDistribution> YValues)
        {
            
            XElement xElement = new XElement("NoneDistribution");
            for (int j = 0; j < XValues.Count; j++)
            {
                XElement ordinateElement = new XElement("Ordinate");

                ordinateElement.SetAttributeValue("x", XValues[j]);
                ordinateElement.SetAttributeValue("y", ((Statistics.None)YValues[j]).GetCentralTendency);

                xElement.Add(ordinateElement);
            }
            return xElement.ToString();
        }

        //private void SaveDepthDamageTable(string tableName, ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.ContinuousDistribution> YValues)
        //{
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();

        //    //string strucTableName = Name + " - " + ot.Name + tableNameAppender;
        //    System.Data.DataTable dtable = new System.Data.DataTable(tableName);
        //    if (Storage.Connection.Instance.TableNames().Contains(tableName))
        //    {
        //        //already exists... delete?
        //        Storage.Connection.Instance.DeleteTable(dtable.TableName);
        //    }

        //    if (YValues[0].GetType() == typeof(Statistics.Normal))
        //    {
        //        SaveNormalTable(dtable, XValues, YValues);
        //    }
        //    else if (YValues[0].GetType() == typeof(Statistics.Triangular))
        //    {
        //        SaveTriangularTable(dtable, XValues, YValues);
        //    }
        //    else if (YValues[0].GetType() == typeof(Statistics.Uniform))
        //    {
        //        SaveUniformTable(dtable, XValues, YValues);
        //    }
        //    else if (YValues[0].GetType() == typeof(Statistics.None))
        //    {
        //        SaveNoneTable(dtable, XValues, YValues);
        //    }

        //    sw.Stop();
        //    Console.WriteLine("**  time per depth damage table: " + sw.Elapsed.ToString());
        //    sw.Reset();

        //}

        #endregion


        public override void Save()
        {
            Stopwatch swEachOccType = new Stopwatch();
            Stopwatch swTotal = new Stopwatch();
            Stopwatch swApply = new Stopwatch();
            swTotal.Start();


            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                if (!Storage.Connection.Instance.IsOpen)
                {
                    Storage.Connection.Instance.Open();
                }
                if (Storage.Connection.Instance.TableNames().Contains(TableName))
                {
                    //already exists... delete?
                    Storage.Connection.Instance.DeleteTable(TableName);
                }

                string[] colNames = new string[] { "Name", "Description", "DamageCategory","VarInFoundHtType","FdHtMin","FdHtMax","FdHtStDev",
                    "IsStructChecked","VarInStructValueType","StructMin","StructMax","StructStDev","StructDistType", "IsContChecked",
                    "VarInContValueType", "ContMin", "ContMax", "ContStDev", "ContDistType", "IsVehChecked", "VarInVehValueType",
                    "VehMin", "VehMax", "VehStDev", "VehDistType", "IsOtherChecked", "VarInOtherValueType", "OtherMin", "OtherMax",
                    "OtherStDev", "OtherDistType","GroupName", "StructureCurve","ContentCurve","VehicleCurve","OtherCurve" };
                Type[] colTypes = new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(double),
                    typeof(double), typeof(double), typeof(bool), typeof(string), typeof(double), typeof(double), typeof(double),
                    typeof(string), typeof(bool), typeof(string), typeof(double), typeof(double), typeof(double), typeof(string),
                    typeof(bool), typeof(string), typeof(double), typeof(double), typeof(double), typeof(string), typeof(bool),
                    typeof(string), typeof(double), typeof(double), typeof(double), typeof(string),typeof(string),typeof(string),
                    typeof(string),typeof(string),typeof(string)};

                Storage.Connection.Instance.CreateTable(TableName, colNames, colTypes);
                DataBase_Reader.DataTableView tbl = Storage.Connection.Instance.GetTable(TableName);

                List<object[]> rows = new List<object[]>();

                foreach (Consequences_Assist.ComputableObjects.OccupancyType ot in ListOfOccupancyTypes)
                {
                    swEachOccType.Start();

                    ////write table for structures
                    //if (ot.GetStructurePercentDD.YValues.Count > 0)
                    //{
                    //    string strucTableName = Name + " - " + ot.Name + STRUCTURE_DD_CURVE_NAME;
                       // SaveDepthDamageTable(strucTableName, ot.GetStructurePercentDD.XValues, ot.GetStructurePercentDD.YValues);   
                    //    //SaveStructureDepthDamage(ot);
                    //}

                    ////write a table for content dd curve
                    //if (ot.GetContentPercentDD.YValues.Count > 0)
                    //{
                    //    string contTableName = Name + " - " + ot.Name + CONTENT_DD_CURVE_NAME;
                    //    SaveDepthDamageTable(contTableName, ot.GetContentPercentDD.XValues, ot.GetContentPercentDD.YValues);
                    //    //SaveContentDepthDamage(ot);
                    //}

                    ////write for vehicle dd curve
                    //if (ot.GetVehiclePercentDD.YValues.Count > 0)
                    //{
                    //    string vehicleTableName = Name + " - " + ot.Name + VEHICLE_DD_CURVE_NAME;
                    //    SaveDepthDamageTable(vehicleTableName, ot.GetVehiclePercentDD.XValues, ot.GetVehiclePercentDD.YValues);
                    //    //SaveVehicleDepthDamage(ot);
                    //}

                    //// other dd curve
                    //if (ot.GetOtherPercentDD.YValues.Count > 0)
                    //{
                    //    string otherTableName = Name + " - " + ot.Name + OTHER_DD_CURVE_NAME;
                    //    SaveDepthDamageTable(otherTableName, ot.GetOtherPercentDD.XValues, ot.GetOtherPercentDD.YValues);
                    //    //SaveOtherDepthDamage(ot);
                    //}                   


                    swEachOccType.Stop();
                    Console.WriteLine("****  time per OT: " + swEachOccType.Elapsed.ToString());
                    swEachOccType.Reset();

                    rows.Add(GetOccTypeRowForParentTable(ot).ToArray());
                }
                tbl.AddRows(rows);
                tbl.ApplyEdits();
                Storage.Connection.Instance.Close();


                swTotal.Stop();
                Console.WriteLine("***************  total time to save: " + swTotal.Elapsed.ToString());
            }
        }
        /// <summary>
        /// This method is used to create the row for the parent occtype table. 
        /// This table has a lot of columns
        /// </summary>
        /// <param name="ot"></param>
        /// <returns></returns>
        private List<object> GetOccTypeRowForParentTable(Consequences_Assist.ComputableObjects.OccupancyType ot)
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
            rowsList.Add(Name);


            //structure curve xml
            rowsList.Add(CreateXMLCurveString(ot.GetStructurePercentDD.Distribution, ot.GetStructurePercentDD.XValues, ot.GetStructurePercentDD.YValues));

            //content curve xml
            rowsList.Add(CreateXMLCurveString(ot.GetContentPercentDD.Distribution, ot.GetContentPercentDD.XValues, ot.GetContentPercentDD.YValues));
            //vehicle curve xml
            rowsList.Add(CreateXMLCurveString(ot.GetVehiclePercentDD.Distribution, ot.GetVehiclePercentDD.XValues, ot.GetVehiclePercentDD.YValues));
            //other curve xml
            rowsList.Add(CreateXMLCurveString(ot.GetOtherPercentDD.Distribution, ot.GetOtherPercentDD.XValues, ot.GetOtherPercentDD.YValues));


            return rowsList;
        }

        private string CreateXMLCurveString(Statistics.UncertainCurveDataCollection.DistributionsEnum distType, ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.ContinuousDistribution> YValues)
        {
            switch(distType)
            {
                case Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal:
                    return CreateNormalDistributionXML(XValues, YValues);
                case Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular:
                    return CreateTriangularDistributionXML(XValues, YValues);
                case Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform:
                    return CreateUniformDistributionXML(XValues, YValues);
                case Statistics.UncertainCurveDataCollection.DistributionsEnum.None:
                    return CreateNoneDistributionXML(XValues, YValues);

            }
            return "";
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
