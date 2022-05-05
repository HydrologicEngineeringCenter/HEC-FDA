using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class IASPersistenceManager : SavingBase
    {
        private const int XML_COLUMN = 2;

        private const string TABLE_NAME = "ImpactAreaScenarios";
        private static readonly string[] ColumnNames = { "Name","XML"};

        private static readonly Type[] TableColTypes = { typeof(string), typeof(string)};


        /// <summary>
        /// Column names for the main conditions table.
        /// </summary>
        public override string[] TableColumnNames { get { return ColumnNames; } }
        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return TableColTypes; }
        }
        /// <summary>
        /// The table name for the main conditions table.
        /// </summary>
        public override string TableName { get { return TABLE_NAME; } }

        #region constructor
        /// <summary>
        /// The persistence manager for the conditions object. This handles all the interaction between FDA and the database.
        /// </summary>
        /// <param name="studyCache"></param>
        public IASPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #endregion

        #region utilities
        /// <summary>
        /// Gets the row from the element that will go into the main table.
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            object[] retval = null;
            IASElementSet element = elem as IASElementSet;
            if (element != null)
            {
                retval = new object[] { element.Name, element.WriteToXML() };
            }
            return retval;
        }

        /// <summary>
        /// Converts the row in the main table into an actual condition element.
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            string xml = (string)rowData[XML_COLUMN];
            int id = Convert.ToInt32(rowData[ID_COL]);
            return new IASElementSet(xml, id);
        }

        #endregion

        /// <summary>
        /// Saves a new conditions element to the database
        /// </summary>
        /// <param name="element"></param>
        public void SaveNew(ChildElement element)
        {
            if (element is IASElementSet iasElem)
            {
                string editDate = DateTime.Now.ToString("G");
                element.LastEditDate = editDate;
                SaveNewElementToParentTable(GetRowDataFromElement(iasElem), TableName, TableColumnNames, TableColumnTypes);
                StudyCacheForSaving.AddElement(iasElem);
            }
        }

        /// <summary>
        /// Deletes an element from the parent table.
        /// </summary>
        /// <param name="element"></param>
        public void Remove(ChildElement element)
        {
            if (element is IASElementSet iasElem)
            {
                //remove from the cache first while you can still get the element's id.
                StudyCacheForSaving.RemoveElement(iasElem);
                RemoveFromParentTable(element, TableName);
            }
        }

        /// <summary>
        /// Updates and existing row in the database.
        /// </summary>
        /// <param name="element"></param>
        public void SaveExisting( ChildElement element)
        {
            base.SaveExisting( element);
        }

        /// <summary>
        /// Reads the tables and creates all the conditions.
        /// </summary>
        public override void Load()
        {
            List<ChildElement> iasElems = CreateElementsFromRows(TableName, rowData => CreateElementFromRowData(rowData));
            foreach (IASElementSet elem in iasElems)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

        private string WasAnalyticalFrequencyElementModified(IASElementSet iasElems,ChildElement elem, int elemID )
        {
            List<SpecificIAS> iasList = iasElems.SpecificIASElements.Where(ias => ias.FlowFreqID == elemID).ToList();
            return iasList.Count > 0 ? CreateTooltipMessage("Analytical Frequency", elem.Name) : null;
        }

        private string WasInflowOutflowModified(IASElementSet iasElems, ChildElement elem, int elemID)
        {
            List<SpecificIAS> iasList = iasElems.SpecificIASElements.Where(ias => ias.InflowOutflowID == elemID).ToList();
            return iasList.Count > 0 ? CreateTooltipMessage("Inflow-Outflow", elem.Name) : null;
        }

        private string WasRatingModified(IASElementSet iasElems, ChildElement elem, int elemID)
        {
            List<SpecificIAS> iasList = iasElems.SpecificIASElements.Where(ias => ias.RatingID == elemID).ToList();
            return iasList.Count > 0 ? CreateTooltipMessage("Rating", elem.Name) : null;
        }

        private string WasExteriorInteriorModified(IASElementSet iasElems, ChildElement elem, int elemID)
        {
            List<SpecificIAS> iasList = iasElems.SpecificIASElements.Where(ias => ias.ExtIntStageID == elemID).ToList();
            return iasList.Count > 0 ? CreateTooltipMessage("Exterior-Interior Stage", elem.Name) : null;
        }

        private string WasStageDamageModified(IASElementSet iasElems, ChildElement elem, int elemID)
        {
            List<SpecificIAS> iasList = iasElems.SpecificIASElements.Where(ias => ias.StageDamageID == elemID).ToList();
            return iasList.Count > 0 ? CreateTooltipMessage("Aggregates Stage-Damage", elem.Name) : null;
        }

        private string WasLeveeElementModified(IASElementSet iasElems, ChildElement elem, int elemID)
        {
            List<SpecificIAS> iasList = iasElems.SpecificIASElements.Where(ias => ias.LeveeFailureID == elemID).ToList();
            return iasList.Count > 0 ? CreateTooltipMessage("Levee-Failure", elem.Name) : null;
        }

        private string CreateTooltipMessage(string curveType, string curveName)
        {
            return curveType + " " + curveName + " was modified since last save.";
        }

        /// <summary>
        /// The elemID is required here because the modified element, if it was removed, no longer has a valid id.
        /// </summary>
        /// <param name="iasSet"></param>
        /// <param name="elemModified"></param>
        /// <param name="elemID"></param>
        private void CheckIASSetForBaseDataModified(IASElementSet iasSet, ChildElement elemModified, int elemID)
        {
            string msg = null;

            if (elemModified is AnalyticalFrequencyElement)
            {
                msg = WasAnalyticalFrequencyElementModified(iasSet,elemModified, elemID);
            }
            else if (elemModified is InflowOutflowElement)
            {
                msg = WasInflowOutflowModified(iasSet, elemModified, elemID);
            }
            else if (elemModified is RatingCurveElement)
            {
                msg = WasRatingModified(iasSet, elemModified, elemID);
            }
            else if (elemModified is LeveeFeatureElement)
            {
                msg = WasLeveeElementModified(iasSet, elemModified, elemID);
            }
            else if (elemModified is ExteriorInteriorElement)
            {
                msg = WasExteriorInteriorModified(iasSet, elemModified, elemID);
            }
            else if (elemModified is AggregatedStageDamageElement)
            {
                msg = WasStageDamageModified(iasSet, elemModified, elemID);
            }
            if (msg != null)
            {
                iasSet.UpdateTreeViewHeader(iasSet.Name + "*", Environment.NewLine + msg);
            }
        }

        /// <summary>
        /// This will update the condition element in the database. This will trigger
        /// an update to the study cache and the study tree as well.
        /// </summary>
        /// <param name="elem">The child element that has been removed</param>
        /// <param name="originalID">The original id </param>
        public void UpdateIASTooltipsChildElementModified(ChildElement elem, int originalID)
        {
            List<IASElementSet> conditionsElements = StudyCache.GetChildElementsOfType<IASElementSet>();

            foreach(IASElementSet set in conditionsElements)
            {
                CheckIASSetForBaseDataModified(set, elem, originalID);
            }

        }

    }
}
