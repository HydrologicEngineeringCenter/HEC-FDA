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

        public ImpactAreaRowItem ImpArea { get; }
        public string DamCat { get; }
        public ComputeComponentVM ComputeComponent { get; }
        public string AssetCategory { get; }

        public StageDamageCurve(ImpactAreaRowItem impArea, String damCat, ComputeComponentVM function, string assetCategory)
        {
            ImpArea = impArea;
            DamCat = damCat;
            ComputeComponent = function;
            AssetCategory = assetCategory;
        }

        public StageDamageCurve(XElement curveElement)
        {
            int selectedImpArea = int.Parse( curveElement.Attribute(SELECTED_IMPACT_AREA_TAG).Value);
            string selectedDamCat = curveElement.Attribute(SELECTED_DAM_CAT_TAG).Value;
            AssetCategory = curveElement.Attribute(ASSET_CATEGORY).Value;
            XElement functionElem = curveElement.Element("ComputeComponentVM");
            //UncertainPairedData uncertainPairedData = UncertainPairedData.ReadFromXML(functionElem);
            ComputeComponentVM computeComponentVM = new ComputeComponentVM(functionElem);


            //todo i need to create the row item from the id? i just need to grab it from the database i guess
            ImpArea = new ImpactAreaRowItem(selectedImpArea, "teststageDamageCurve");
            DamCat = selectedDamCat;
            ComputeComponent = computeComponentVM;
        } 

        public XElement WriteToXML(StageDamageCurve curve)
        {
            XElement stageDamageCurveElement = new XElement(STAGE_DAMAGE_CURVE_TAG);
            stageDamageCurveElement.SetAttributeValue(SELECTED_IMPACT_AREA_TAG, curve.ImpArea.ID);
            stageDamageCurveElement.SetAttributeValue(SELECTED_DAM_CAT_TAG, curve.DamCat);
            stageDamageCurveElement.SetAttributeValue(ASSET_CATEGORY, AssetCategory);
            stageDamageCurveElement.Add(curve.ComputeComponent.ToXML());

            return stageDamageCurveElement;
        }
    }
}
