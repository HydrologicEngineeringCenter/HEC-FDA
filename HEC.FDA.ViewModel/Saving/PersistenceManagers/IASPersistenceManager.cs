using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class IASPersistenceManager : SavingBase<IASElement>
    {

        #region constructor
        /// <summary>
        /// The persistence manager for the conditions object. This handles all the interaction between FDA and the database.
        /// </summary>
        /// <param name="studyCache"></param>
        public IASPersistenceManager(FDACache studyCache, string tableName):base(studyCache, tableName)
        {
        }

        #endregion



        /// <summary>
        /// Updates and existing row in the database.
        /// </summary>
        /// <param name="element"></param>
        public override void SaveExisting( ChildElement element)
        {
            IASElement elem = element as IASElement;
            string currentDate = DateTime.Now.ToString("G");
            string tooltip = StringConstants.CreateLastEditTooltip(currentDate);
            if(elem.UpdateComputeDate && elem.Results != null)
            {
                elem.Results.ComputeDate = currentDate;
            }
            element.UpdateTreeViewHeader(element.Name);
            element.CustomTreeViewHeader.Tooltip = tooltip;
            base.SaveExisting( element);
        }

        public override void SaveNew(ChildElement element)
        {
            base.SaveNew( element);
            if(element is IASElement)
            {
                IASTooltipHelper.UpdateTooltip(element as IASElement);
            }
        }

        private string WasAnalyticalFrequencyElementModified(IASElement iasElems,ChildElement elem, int elemID )
        {
            List<SpecificIAS> iasList = iasElems.SpecificIASElements.Where(ias => ias.FlowFreqID == elemID).ToList();
            return iasList.Count > 0 ? CreateTooltipMessage("Analytical Frequency", elem.Name) : null;
        }

        private string WasInflowOutflowModified(IASElement iasElems, ChildElement elem, int elemID)
        {
            List<SpecificIAS> iasList = iasElems.SpecificIASElements.Where(ias => ias.InflowOutflowID == elemID).ToList();
            return iasList.Count > 0 ? CreateTooltipMessage("Inflow-Outflow", elem.Name) : null;
        }

        private string WasRatingModified(IASElement iasElems, ChildElement elem, int elemID)
        {
            List<SpecificIAS> iasList = iasElems.SpecificIASElements.Where(ias => ias.RatingID == elemID).ToList();
            return iasList.Count > 0 ? CreateTooltipMessage("Rating", elem.Name) : null;
        }

        private string WasExteriorInteriorModified(IASElement iasElems, ChildElement elem, int elemID)
        {
            List<SpecificIAS> iasList = iasElems.SpecificIASElements.Where(ias => ias.ExtIntStageID == elemID).ToList();
            return iasList.Count > 0 ? CreateTooltipMessage("Exterior-Interior Stage", elem.Name) : null;
        }

        private string WasStageDamageModified(IASElement iasElems, ChildElement elem, int elemID)
        {
            List<SpecificIAS> iasList = iasElems.SpecificIASElements.Where(ias => ias.FailureStageDamageID == elemID).ToList();
            return iasList.Count > 0 ? CreateTooltipMessage("Aggregates Stage-Damage", elem.Name) : null;
        }

        private string WasLeveeElementModified(IASElement iasElems, ChildElement elem, int elemID)
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
        private void CheckIASSetForBaseDataModified(IASElement iasSet, ChildElement elemModified, int elemID)
        {
            string msg = null;

            if (elemModified is FrequencyElement)
            {
                msg = WasAnalyticalFrequencyElementModified(iasSet,elemModified, elemID);
            }
            else if (elemModified is InflowOutflowElement)
            {
                msg = WasInflowOutflowModified(iasSet, elemModified, elemID);
            }
            else if (elemModified is StageDischargeElement)
            {
                msg = WasRatingModified(iasSet, elemModified, elemID);
            }
            else if (elemModified is LateralStructureElement)
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
        /// This will update the IAS element in the database. This will trigger
        /// an update to the study cache and the study tree as well.
        /// </summary>
        /// <param name="elem">The child element that has been removed or updated</param>
        /// <param name="originalID">The original id </param>
        public void UpdateIASTooltipsChildElementModified(ChildElement elem, int originalID)
        {
            List<IASElement> conditionsElements = StudyCache.GetChildElementsOfType<IASElement>();

            foreach(IASElement set in conditionsElements)
            {
                CheckIASSetForBaseDataModified(set, elem, originalID);
            }

        }

    }
}
