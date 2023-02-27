using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.TableWithPlot;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class StageDamageCurve : BaseViewModel
    {
        public static String STAGE_DAMAGE_CURVE_TAG = "StageDamageCurve";
        private const String SELECTED_IMPACT_AREA_TAG = "SelectedImpactArea";
        private const String SELECTED_DAM_CAT_TAG = "SelectedDamCat";
        private const String ASSET_CATEGORY = "AssetCategory";
        private const String CONSTRUCTION_TYPE = "ConstructionType";

        public ImpactAreaRowItem ImpArea { get; }
        public string DamCat { get; }
        public CurveComponentVM ComputeComponent { get; }
        public string AssetCategory { get; }
        public StageDamageConstructionType ConstructionType { get; }

        public StageDamageCurve(ImpactAreaRowItem impArea, String damCat, CurveComponentVM function,
            string assetCategory, StageDamageConstructionType constructionType)
        {
            ImpArea = impArea;
            //todo: i don't like this if statement. We might be able to remove it once we have the impact areas working correctly.
            if(impArea == null)
            {
                ImpArea = new ImpactAreaRowItem(-1, "");
            }
            DamCat = damCat;
            ComputeComponent = function;
            AssetCategory = assetCategory;
            ConstructionType = constructionType;
        }

        public StageDamageCurve(XElement curveElement)
        {
            int selectedImpArea = int.Parse(curveElement.Attribute(SELECTED_IMPACT_AREA_TAG).Value);
            string selectedDamCat = curveElement.Attribute(SELECTED_DAM_CAT_TAG).Value;
            AssetCategory = curveElement.Attribute(ASSET_CATEGORY).Value;

            CurveComponentVM curveComponentVM = CurveComponentVM.CreateCurveComponentVM(curveElement);

            //get the impact area name from the id
            string impAreaName = GetImpactAreaNameFromID(selectedImpArea);
            ImpArea = new ImpactAreaRowItem(selectedImpArea, impAreaName);
            DamCat = selectedDamCat;
            ComputeComponent = curveComponentVM;

            //this is for backwards compatability
            if (curveElement.Attribute(CONSTRUCTION_TYPE) != null)
            {
                Enum.TryParse(curveElement.Attribute(CONSTRUCTION_TYPE).Value, out StageDamageConstructionType constructionType);
                ConstructionType = constructionType;
            }
            else
            {
                ConstructionType = StageDamageConstructionType.COMPUTED;
            }
        }

        //todo: does this method exist somewhere else? Maybe this should be a static somewhere. I am guessing
        //we do this in multiple places.
        private string GetImpactAreaNameFromID(int id)
        {
            string name = "";
            if (StudyCache != null)
            {
                List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
                if (impactAreaElements.Count > 0)
                {
                    foreach (ImpactAreaRowItem row in impactAreaElements[0].ImpactAreaRows)
                    {
                        if (row.ID == id)
                        {
                            name = row.Name;
                            break;
                        }
                    }
                }
            }
            return name;
        }

        public XElement WriteToXML()
        {
            XElement stageDamageCurveElement = new XElement(STAGE_DAMAGE_CURVE_TAG);
            stageDamageCurveElement.SetAttributeValue(SELECTED_IMPACT_AREA_TAG, ImpArea.ID);
            stageDamageCurveElement.SetAttributeValue(SELECTED_DAM_CAT_TAG, DamCat);
            stageDamageCurveElement.SetAttributeValue(ASSET_CATEGORY, AssetCategory);
            stageDamageCurveElement.SetAttributeValue(CONSTRUCTION_TYPE, ConstructionType);
            stageDamageCurveElement.Add(ComputeComponent.ToXML());

            return stageDamageCurveElement;
        }

        public bool Equals(StageDamageCurve elem)
        {
            bool isEqual = true;
            if(ImpArea.ID != elem.ImpArea.ID)
            {
                isEqual = false;
            }
            if (!DamCat.Equals(elem.DamCat))
            {
                isEqual = false;
            }
            if (!AssetCategory.Equals(elem.AssetCategory))
            {
                isEqual = false;
            }
            if (ConstructionType !=elem.ConstructionType)
            {
                isEqual = false;
            }
            if (ComputeComponent.SelectedItemToPairedData().Equals(elem.ComputeComponent.SelectedItemToPairedData()))
            {
                isEqual = false;
            }

            return isEqual;
        }
    }
}
