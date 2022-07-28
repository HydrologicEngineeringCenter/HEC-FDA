using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.TableWithPlot;
using System;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class StageDamageCurve
    {
        public static String STAGE_DAMAGE_CURVE_TAG = "StageDamageCurve";
        private const String SELECTED_IMPACT_AREA_TAG = "SelectedImpactArea";
        private const String SELECTED_DAM_CAT_TAG = "SelectedDamCat";
        private const String ASSET_CATEGORY = "AssetCategory";
        private const String CONSTRUCTION_TYPE = "AssetCategory";

        public ImpactAreaRowItem ImpArea { get; }
        public string DamCat { get; }
        public ComputeComponentVM ComputeComponent { get; }
        public string AssetCategory { get; }
        public StageDamageConstructionType ConstructionType { get; }

        public StageDamageCurve(ImpactAreaRowItem impArea, String damCat, ComputeComponentVM function, 
            string assetCategory, StageDamageConstructionType constructionType)
        {
            ImpArea = impArea;
            DamCat = damCat;
            ComputeComponent = function;
            AssetCategory = assetCategory;
            ConstructionType = constructionType;
        }

        public StageDamageCurve(XElement curveElement)
        {
            int selectedImpArea = int.Parse( curveElement.Attribute(SELECTED_IMPACT_AREA_TAG).Value);
            string selectedDamCat = curveElement.Attribute(SELECTED_DAM_CAT_TAG).Value;
            AssetCategory = curveElement.Attribute(ASSET_CATEGORY).Value;
            XElement functionElem = curveElement.Element("ComputeComponentVM");
            ComputeComponentVM computeComponentVM = new ComputeComponentVM(functionElem);
            //I don't think the impact area row name matters here
            ImpArea = new ImpactAreaRowItem(selectedImpArea, "impact area row");
            DamCat = selectedDamCat;
            ComputeComponent = computeComponentVM;

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
    }
}
