using System;
using System.Collections.Generic;
using System.Linq;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
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
        /// <summary>
        /// This bool is to let the editor know which one of the elements to have selected when it opens. There should
        /// only ever be one element that is turned to "true".
        /// </summary>
        public bool IsSelected { get; set; }
        //todo: maybe this should be an occtype id and not a string?
        //public Dictionary<string,bool[]> OccTypesSelectedTabsDictionary { get; set; }
            //public string OccTypesGroupName { get; set; }
        public List<IOccupancyType> ListOfOccupancyTypes { get; set; }
        public int ID { get; set; }
        

        #endregion
        #region Constructors
        public OccupancyTypesElement():base()
        {

        }
        public OccupancyTypesElement( string occTypesGroupName, int groupID, List<IOccupancyType> listOfOccTypes):base()
        {
            Name = occTypesGroupName;
            ID = groupID;
            //OccTypesSelectedTabsDictionary = occtypesSelectedTabs;
            //OccTypesGroupName = occTypesGroupName;
            ListOfOccupancyTypes = listOfOccTypes;
        }
        #endregion
        #region Voids
        #endregion
        #region Functions

      
        public List<String> getUniqueDamageCategories()
        {
            HashSet<String> dams = new HashSet<String>();
            foreach (IOccupancyType ot in ListOfOccupancyTypes)
            {
                dams.Add(Name + " -> " + ot.DamageCategory);
            }
            return dams.ToList<String>();
        }

        #endregion
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        #region SaveTables


        //public static string CreateNormalDistributionXML(ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.ContinuousDistribution> YValues)
        //{
        //    XElement xElement = new XElement("NormalDistribution");
        //    for (int j = 0; j < XValues.Count; j++)
        //    {
        //        XElement ordinateElement = new XElement("Ordinate");

        //        ordinateElement.SetAttributeValue("x", XValues[j]);
        //        ordinateElement.SetAttributeValue("mean", ((Statistics.Normal)YValues[j]).GetMean);
        //        ordinateElement.SetAttributeValue("stDev", ((Statistics.Normal)YValues[j]).GetStDev);

        //        xElement.Add(ordinateElement);
        //    }
        //    return xElement.ToString();
        //}


        //private string CreateTriangularDistributionXML(ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.ContinuousDistribution> YValues)
        //{
        //    XElement xElement = new XElement("TriangularDistribution");
        //    for (int j = 0; j < XValues.Count; j++)
        //    {
        //        XElement ordinateElement = new XElement("Ordinate");

        //        ordinateElement.SetAttributeValue("x", XValues[j]);
        //        ordinateElement.SetAttributeValue("min", ((Statistics.Triangular)YValues[j]).getMin);
        //        ordinateElement.SetAttributeValue("max", ((Statistics.Triangular)YValues[j]).getMax);
        //        ordinateElement.SetAttributeValue("mostLikely", ((Statistics.Triangular)YValues[j]).getMostlikely);

        //        xElement.Add(ordinateElement);
        //    }
        //    return xElement.ToString();
        //}




        //private string CreateUniformDistributionXML(ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.ContinuousDistribution> YValues)
        //{
        //    XElement xElement = new XElement("UniformDistribution");
        //    for (int j = 0; j < XValues.Count; j++)
        //    {
        //        XElement ordinateElement = new XElement("Ordinate");

        //        ordinateElement.SetAttributeValue("x", XValues[j]);
        //        ordinateElement.SetAttributeValue("min", ((Statistics.Uniform)YValues[j]).GetMin);
        //        ordinateElement.SetAttributeValue("max", ((Statistics.Uniform)YValues[j]).GetMax);

        //        xElement.Add(ordinateElement);
        //    }
        //    return xElement.ToString();
        //}


        //private string CreateNoneDistributionXML(ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.ContinuousDistribution> YValues)
        //{

        //    XElement xElement = new XElement("NoneDistribution");
        //    for (int j = 0; j < XValues.Count; j++)
        //    {
        //        XElement ordinateElement = new XElement("Ordinate");

        //        ordinateElement.SetAttributeValue("x", XValues[j]);
        //        ordinateElement.SetAttributeValue("y", ((Statistics.None)YValues[j]).GetCentralTendency);

        //        xElement.Add(ordinateElement);
        //    }
        //    return xElement.ToString();
        //}



        #endregion


        public void Save()
        {

            //if (!Storage.Connection.Instance.IsConnectionNull)
            //{
            //    if (!Storage.Connection.Instance.IsOpen)
            //    {
            //        Storage.Connection.Instance.Open();
            //    }
            //    if (Storage.Connection.Instance.TableNames().Contains(TableName))
            //    {
            //        //already exists... delete?
            //        Storage.Connection.Instance.DeleteTable(TableName);
            //    }

            //    string[] colNames = new string[] { "Name", "Description", "DamageCategory","VarInFoundHtType","FdHtMin","FdHtMax","FdHtStDev",
            //        "IsStructChecked","VarInStructValueType","StructMin","StructMax","StructStDev","StructDistType", "IsContChecked",
            //        "VarInContValueType", "ContMin", "ContMax", "ContStDev", "ContDistType", "IsVehChecked", "VarInVehValueType",
            //        "VehMin", "VehMax", "VehStDev", "VehDistType", "IsOtherChecked", "VarInOtherValueType", "OtherMin", "OtherMax",
            //        "OtherStDev", "OtherDistType","GroupName", "StructureCurve","ContentCurve","VehicleCurve","OtherCurve" };
            //    Type[] colTypes = new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(double),
            //        typeof(double), typeof(double), typeof(bool), typeof(string), typeof(double), typeof(double), typeof(double),
            //        typeof(string), typeof(bool), typeof(string), typeof(double), typeof(double), typeof(double), typeof(string),
            //        typeof(bool), typeof(string), typeof(double), typeof(double), typeof(double), typeof(string), typeof(bool),
            //        typeof(string), typeof(double), typeof(double), typeof(double), typeof(string),typeof(string),typeof(string),
            //        typeof(string),typeof(string),typeof(string)};

            //    Storage.Connection.Instance.CreateTable(TableName, colNames, colTypes);
            //    DataBase_Reader.DataTableView tbl = Storage.Connection.Instance.GetTable(TableName);

            //    List<object[]> rows = new List<object[]>();

            //    foreach (Consequences_Assist.ComputableObjects.OccupancyType ot in ListOfOccupancyTypes)
            //    {

            //        rows.Add(GetOccTypeRowForParentTable(ot).ToArray());
            //    }
            //    tbl.AddRows(rows);
            //    tbl.ApplyEdits();
            //    Storage.Connection.Instance.Close();

            //}
            //Saving.PersistenceFactory.GetOccTypeManager().SaveNew(ListOfOccupancyTypes, OccTypesSelectedTabsDictionary, Name);
        }
        ///// <summary>
        ///// This method is used to create the row for the parent occtype table. 
        ///// This table has a lot of columns
        ///// </summary>
        ///// <param name="ot"></param>
        ///// <returns></returns>
        //private List<object> GetOccTypeRowForParentTable(IOccupancyType ot)
        //{
        //    List<object> rowsList = new List<object>();
        //    bool[] checkedTabs = new bool[4];
        //    if (OccTypesSelectedTabsDictionary.ContainsKey(ot.Name))
        //    {
        //        checkedTabs = OccTypesSelectedTabsDictionary[ot.Name];
        //    }
        //    else
        //    {
        //        //can't find the key in the dictionary
        //        throw new Exception();
        //    }


        //    //name, description, damacat name
        //    foreach (object o in GetOccTypeInfoArray(ot))
        //    {
        //        rowsList.Add(o);
        //    }

        //    //found ht variation type, min, max, st dev
        //    foreach (object o in GetContinuousDistributionArray(ot.FoundationHeightUncertaintyFunction))
        //    {
        //        rowsList.Add(o);
        //    }

        //    //is struct checked
        //    rowsList.Add(checkedTabs[0]);

        //    //structure variation in value type, min, max, st dev
        //    foreach (object o in GetContinuousDistributionArray(ot.StructureValueUncertainty))
        //    {
        //        rowsList.Add(o);
        //    }

        //    //structure dist type
        //    rowsList.Add(ot.StructureDepthDamageFunction.Distribution); 

        //    //is content checked
        //    rowsList.Add(checkedTabs[1]);

        //    //content variation in value type, min, max, st dev
        //    foreach (object o in GetContinuousDistributionArray(ot.ContentValueUncertainty))
        //    {
        //        rowsList.Add(o);
        //    }

        //    //cont dist type
        //    rowsList.Add(ot.ContentDepthDamageFunction.Distribution);

        //    //is vehicle checked
        //    rowsList.Add(checkedTabs[2]);

        //    //vehicle variation in value type, min, max, st dev
        //    foreach (object o in GetContinuousDistributionArray(ot.VehicleValueUncertainty))
        //    {
        //        rowsList.Add(o);
        //    }

        //    //vehicle dist type
        //    rowsList.Add(ot.VehicleDepthDamageFunction.Distribution);

        //    //is other checked
        //    rowsList.Add(checkedTabs[3]);

        //    //Other variation in value type, min, max, st dev
        //    foreach (object o in GetContinuousDistributionArray(ot.OtherValueUncertainty))
        //    {
        //        rowsList.Add(o);
        //    }

        //    //other dist type
        //    rowsList.Add(ot.OtherDepthDamageFunction.Distribution);

        //    //damcats and occtypes group name
        //    rowsList.Add(Name);


        //    //structure curve xml
        //    rowsList.Add(ExtentionMethods.CreateXMLCurveString(ot.GetStructurePercentDD.Distribution, ot.GetStructurePercentDD.XValues, ot.GetStructurePercentDD.YValues));

        //    //content curve xml
        //    rowsList.Add(ExtentionMethods.CreateXMLCurveString(ot.GetContentPercentDD.Distribution, ot.GetContentPercentDD.XValues, ot.GetContentPercentDD.YValues));
        //    //vehicle curve xml
        //    rowsList.Add(ExtentionMethods.CreateXMLCurveString(ot.GetVehiclePercentDD.Distribution, ot.GetVehiclePercentDD.XValues, ot.GetVehiclePercentDD.YValues));
        //    //other curve xml
        //    rowsList.Add(ExtentionMethods.CreateXMLCurveString(ot.GetOtherPercentDD.Distribution, ot.GetOtherPercentDD.XValues, ot.GetOtherPercentDD.YValues));


        //    return rowsList;
        //}

        //private string CreateXMLCurveString(Statistics.UncertainCurveDataCollection.DistributionsEnum distType, ReadOnlyCollection<double> XValues, ReadOnlyCollection<Statistics.ContinuousDistribution> YValues)
        //{
        //    switch(distType)
        //    {
        //        case Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal:
        //            return ExtentionMethods.CreateNormalDistributionXML(XValues, YValues);
        //        case Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular:
        //            return ExtentionMethods.CreateTriangularDistributionXML(XValues, YValues);
        //        case Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform:
        //            return ExtentionMethods.CreateUniformDistributionXML(XValues, YValues);
        //        case Statistics.UncertainCurveDataCollection.DistributionsEnum.None:
        //            return ExtentionMethods.CreateNoneDistributionXML(XValues, YValues);

        //    }
        //    return "";
        //}

        

        private object[] GetOccTypeInfoArray(IOccupancyType ot)
        {
            return new object[] { ot.Name, ot.Description, ot.DamageCategory.Name };

        }
        //private object[] GetContinuousDistributionArray(ICoordinatesFunction cd)
        //{
        //    object[] rowItems = new object[4];
        //    switch (cd.DistributionType)
        //    {
        //        case IOrdinateEnum.Constant:
        //            {
        //                return new object[] { "None", 0, 0, 0 };
        //            }
        //        case IOrdinateEnum.Normal:
        //            {
        //                double stDev = cd. ((Statistics.Normal)cd).GetStDev;
        //                return new object[] { "Normal", 0, 0, stDev };

        //            }
        //        case IOrdinateEnum.Triangular:
        //            //else if (cd.GetType() == typeof(Statistics.Triangular))
        //            {
        //                double min = ((Statistics.Triangular)cd).getMin;
        //                double max = ((Statistics.Triangular)cd).getMax;
        //                return new object[] { "Triangular", min, max, 0 };

        //            }
        //        case IOrdinateEnum.Uniform:
        //            //else if (cd.GetType() == typeof(Statistics.Uniform))
        //            {
        //                double min = ((Statistics.Uniform)cd).GetMin;
        //                double max = ((Statistics.Uniform)cd).GetMax;
        //                return new object[] { "Uniform", min, max, 0 };

        //            }
        //    }

        //    return rowItems;
        //}

        //public override object[] RowData()
        //{
        //    return new object[] { Name };
        //}

        //public override bool SavesToRow()
        //{
        //    return true;
        //}

        //public override bool SavesToTable()
        //{
        //    return true;
        //}

        //public override string GetTableConstant()
        //{
        //    return "OccType - ";
        //}
        //public override string TableName
        //{
        //    get
        //    {
        //        return GetTableConstant() + Name;
        //    }
        //}


        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            //i don't think i need this method but it is required to have it here.
            throw new NotImplementedException();
        //    OccupancyTypesElement elem = (OccupancyTypesElement)elementToClone;

        //    List<IOccupancyType> occTypes = new List<IOccupancyType>();
        //    foreach (IOccupancyType ot in elem.ListOfOccupancyTypes)
        //    {
        //        occTypes.Add(ot);
        //    }

        //    Dictionary<string, bool[]> dictionaryCopy = new Dictionary<string, bool[]>(elem.OccTypesSelectedTabsDictionary);

        //    return new OccupancyTypesElement(elem.Name, occTypes, dictionaryCopy);
        }

    }
}
