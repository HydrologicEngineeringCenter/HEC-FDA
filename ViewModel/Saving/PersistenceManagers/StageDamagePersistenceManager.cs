
using Importer;
using paireddata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using ViewModel.AggregatedStageDamage;
using ViewModel.ImpactArea;
using ViewModel.Utilities;

namespace ViewModel.Saving.PersistenceManagers
{
    public class StageDamagePersistenceManager : UndoRedoBase, IPersistableWithUndoRedo
    {
        private const int NAME_COL = 1;
        private const int LAST_EDIT_DATE_COL = 2;
        private const int DESC_COL = 3;
        private const int IS_MANUAL_COL = 4;
        private const int SELECTED_WSE_COL = 5;
        private const int SELECTED_STRUCTURE_COL = 6;
        private const int CURVES_COL = 7;

        //ELEMENT_TYPE is used to store the type in the log tables. Initially i was actually storing the type
        //of the element. But since they get stored as strings if a developer changes the name of the class
        //you would no longer get any of the old logs. So i use this constant.
        private const string ELEMENT_TYPE = "Stage_Damage";
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("StageDamagePersistenceManager");
        private const String STAGE_DAMAGE_CURVES_TAG = "StageDamageCurves";


        private const string TABLE_NAME = "stage_damage_relationships";
        internal override string ChangeTableConstant { get { return "Aggregated Stage Damage Function - "; } }
        private static readonly string[] TableColNames = { NAME, LAST_EDIT_DATE, DESCRIPTION, "is_manual", "selected_wse", "selected_structures", "curves" };
    
        private static readonly Type[] TableColTypes = { typeof(string), typeof(string), typeof(string), typeof(bool), typeof(int), typeof(int), typeof(string) };
        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return TableColTypes; }
        }

        public override string TableName { get { return TABLE_NAME; } }

        /// <summary>
        /// The name of the change table that will hold the various states of elements.
        /// </summary>
        public override string ChangeTableName { get { return "stage_damage_changes"; } }

        /// <summary>
        /// Names of the columns in the change table
        /// </summary>
        public override string[] ChangeTableColumnNames
        {
            get
            {
                return new string[]{ ELEMENT_ID_COL_NAME, NAME, LAST_EDIT_DATE, DESCRIPTION, "is_manual", "selected_wse", "selected_structures", "curves", STATE_INDEX_COL_NAME };
            }
        }
        /// <summary>
        /// Types for the columns in the change table
        /// </summary>
        public override Type[] ChangeTableColumnTypes
        {
            get
            {
                return new Type[] {typeof(int), typeof(string), typeof(string), typeof(string), typeof(bool), typeof(int), typeof(int), typeof(string), typeof(int) };
            }
        }

        public override string[] TableColumnNames
        {
            get
            {
                return TableColNames;
            }
        }

        public StageDamagePersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities
        private object[] GetRowDataFromElement(AggregatedStageDamageElement element)
        {
            return new object[] { element.Name, element.LastEditDate, element.Description,
               element.IsManual, element.SelectedWSE, element.SelectedStructures, WriteCurvesToXML(element.Curves) };
        }

        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            bool isManual = Convert.ToBoolean( rowData[IS_MANUAL_COL]);
            int selectedWSE = Convert.ToInt32(rowData[SELECTED_WSE_COL]);
            int selectedStructs = Convert.ToInt32( rowData[SELECTED_STRUCTURE_COL]);
            string curvesXmlString = (string)rowData[CURVES_COL];
            List<StageDamageCurve> stageDamageCurves = LoadCurvesFromXML(curvesXmlString);

            AggregatedStageDamageElement asd = new AggregatedStageDamageElement((string)rowData[NAME_COL], (string)rowData[LAST_EDIT_DATE_COL],
            (string)rowData[DESC_COL], selectedWSE, selectedStructs,stageDamageCurves, isManual);
            return asd;
        }
        #endregion
        public void SaveAssetCurve(ChildElement element, StageDamageAssetType type, string nameOfTotalFunctionInParentTable)
        {
            SaveNew(element);
            //if (element.GetType() == typeof(AggregatedStageDamageElement))
            //{
            //    object[] data = GetRowDataForAssetTable(element, type, nameOfTotalFunctionInParentTable);
            //}
        }

        #region save from old fda1

        public void SaveFDA1Elements(AggregateDamageFunctionList stageDamages)
        {
            //StructureValueType { STRUCTURE, CONTENT, OTHER, CAR, TOTAL };

            //convert the list of all the curves into FDA 2.0 curves
            IList<AggregateDamageFunction> curves = stageDamages.GetAggDamageFunctions.Values;

            //the damage reach name needs to match an existing impact area to be included.
            //message user if it does not.

            //sort the curves by their plan and year.
            List<List< AggregateDamageFunction>> groupedCurves = curves.GroupBy(curve => new { curve.PlanName, curve.YearName })
                .Select(group => group.ToList())
                .ToList();
            
            //now create elements from the groups of curves
            foreach(List<AggregateDamageFunction> funcs in groupedCurves)
            {
                AggregatedStageDamageElement elem = CreateElement(funcs);
                SaveNew(elem);
            }
        }

        private AggregatedStageDamageElement CreateElement( List<AggregateDamageFunction> funcs)
        {
            //for the creation date, i am grabbing the creation date from one of the curves

            List<StageDamageCurve> curves = new List<StageDamageCurve>();
            foreach(AggregateDamageFunction func in funcs)
            {
                SingleDamageFunction totalDamageFunc = func.DamageFunctions[(int)StructureValueType.TOTAL];
                StageDamageCurve stageDamageCurve = CreateStageDamageCurve(totalDamageFunc, func.DamageReachName, func.CategoryName);
                if(stageDamageCurve != null)
                {
                    curves.Add(stageDamageCurve);
                }
            }

            //todo what if the count is zero
            string name = funcs[0].PlanName.Trim() + " - " + funcs[0].YearName.Trim();

            //ImpactAreaRowItem ri = new ImpactAreaRowItem(-1, "testImpactArea");
            //StageDamageCurve curve = new StageDamageCurve(ri, "testDamCat", func);
            if(curves.Count>0)
            {
                return new AggregatedStageDamageElement(name, funcs[0].CalculationDate, funcs[0].Description, -1, -1, curves, true);
            }
            else
            {
                return null;
            }
        }

        private StageDamageCurve CreateStageDamageCurve(SingleDamageFunction sdf, string damageReachName, string damCat)
        {
            StageDamageCurve curve = null;
            double[] depths = sdf.Depth;
            double[] damages = sdf.Damage;

            //these arrays might have a bunch of "Study.badNumber" (-901). I need to get rid of them by only grabbing the correct number of points.
            List<double> depthsList = new List<double>();
            List<double> damagesList = new List<double>();
            for (int i = 0; i < sdf.GetNumRows(); i++)
            {
                depthsList.Add(depths[i]);
                damagesList.Add(damages[i]);
            }
            //always use linear. This is the only option in Old Fda.
            UncertainPairedData func = Utilities.DefaultPairedData.CreateDefaultDeterminateUncertainPairedData(depthsList, damagesList, "Stage", "Damage", "Stage-Damage"); 
            //IFdaFunction stageDamage = IFdaFunctionFactory.Factory( IParameterEnum.InteriorStageDamage, (IFunction)func);

            //there should only ever be 0 or 1 impact area elements
            List<ChildElement> impactElems = StudyCache.GetChildElementsOfType(typeof(ImpactAreaElement));
            if(impactElems.Count>0)
            {
                ObservableCollection<ImpactAreaRowItem> impactAreaRows = ((ImpactAreaElement)impactElems[0]).ImpactAreaRows;
                ImpactAreaRowItem selectedRow = null;
                foreach(ImpactAreaRowItem row in impactAreaRows)
                {
                    if(row.Name.Equals(damageReachName))
                    {
                        curve = new StageDamageCurve(row, damCat, func);
                        break;
                    }
                }
            }

            return curve;
        }


        #endregion

        public void SaveNew(ChildElement element)
        {
            if (element.GetType() == typeof(AggregatedStageDamageElement))
            {
                //save to parent table
                SaveNewElement(element);
                //save to change table
                SaveToChangeTable(element);
                //log message
                Log(FdaLogging.LoggingLevel.Info, "Created new stage damage curve: " + element.Name, element.Name);
            }
        }
        public void Remove(ChildElement element)
        {
            base.Remove(element);
        }
        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex)
        {
            if (elementToSave.Description == null) 
            { 
                elementToSave.Description = ""; 
            }
            base.SaveExisting(oldElement, elementToSave, changeTableIndex);
        }

        public void Load()
        {
            List<ChildElement> stageDamages = CreateElementsFromRows(TableName, (asdf) => CreateElementFromRowData(asdf));
            foreach (AggregatedStageDamageElement elem in stageDamages)
            {
                StudyCacheForSaving.AddElement(elem);
            }
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
            int elementId = GetElementId(TableName, elementName);
            LOGGER.Log(level, message, ELEMENT_TYPE, elementId);
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
            int id = GetElementId(TableName, elementName);
            return FdaLogging.RetrieveFromDB.GetLogMessages(id, ELEMENT_TYPE);
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
            int id = GetElementId(TableName, elementName);
            return FdaLogging.RetrieveFromDB.GetLogMessagesByLevel(level, id, ELEMENT_TYPE);
        }

        public object[] GetRowDataForAssetTable(ChildElement element,StageDamageAssetType assetType, string nameOfTotalFunctionInParentTable)
        {
            if (element.Description == null)
            {
                element.Description = "";
            }

            int elemId = GetElementId(TableName, nameOfTotalFunctionInParentTable);
                    
            return new object[] {elemId, element.Name, element.LastEditDate, element.Description,
                element.Curve, ((AggregatedStageDamageElement)element).Method,
                element.Curve.WriteToXML().ToString(),
                assetType};
        }

        public override object[] GetRowDataForChangeTable(ChildElement elem)
        {
            AggregatedStageDamageElement element = (AggregatedStageDamageElement)elem;
            if (element.Description == null)
            {
                element.Description = "";
            }

            int elemId = GetElementId(TableName, element.Name);
            //the new stateId will be one higher than the max that is in the table already.
            int stateId = Storage.Connection.Instance.GetMaxStateIndex(ChangeTableName, elemId, ELEMENT_ID_COL_NAME, STATE_INDEX_COL_NAME) + 1;

            return new object[] {elemId, element.Name, element.LastEditDate, element.Description,
               element.IsManual, element.SelectedWSE, element.SelectedStructures, WriteCurvesToXML(element.Curves), stateId};
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            return GetRowDataFromElement((AggregatedStageDamageElement)elem);
        }
      
        private XElement WriteCurvesToXML(List<StageDamageCurve> curves)
        {
            XElement curvesElement = new XElement(STAGE_DAMAGE_CURVES_TAG);
            foreach(StageDamageCurve curve in curves)
            {
                curvesElement.Add(curve.WriteToXML(curve));
            }
            return curvesElement;
        }

        private List<StageDamageCurve> LoadCurvesFromXML(string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            XElement curvesElement = doc.Element(STAGE_DAMAGE_CURVES_TAG);
            IEnumerable<XElement> curveElems = curvesElement.Elements(StageDamageCurve.STAGE_DAMAGE_CURVE_TAG);
            List<StageDamageCurve> curves = new List<StageDamageCurve>();
            foreach(XElement elem in curveElems)
            {
                curves.Add(new StageDamageCurve(elem));
            }

            return curves;
        }
    }
}
