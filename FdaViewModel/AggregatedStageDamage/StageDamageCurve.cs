using FdaViewModel.ImpactArea;
using Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utilities.Serialization;

namespace FdaViewModel.AggregatedStageDamage
{
    public class StageDamageCurve
    {
        public static String STAGE_DAMAGE_CURVE_TAG = "StageDamageCurve";
        private const String SELECTED_IMPACT_AREA_TAG = "SelectedImpactArea";
        private const String SELECTED_DAM_CAT_TAG = "SelectedDamCat";

        public StageDamageCurve(ImpactAreaRowItem impArea, String damCat, ICoordinatesFunction function)
        {
            ImpArea = impArea;
            DamCat = damCat;
            Function = function;
        }

        public StageDamageCurve(XElement curveElement)
        {
            int selectedImpArea = int.Parse( curveElement.Attribute(SELECTED_IMPACT_AREA_TAG).Value);
            string selectedDamCat = curveElement.Attribute(SELECTED_DAM_CAT_TAG).Value;
            XElement functionElem = curveElement.Element(SerializationConstants.FUNCTIONS);
            ICoordinatesFunction coordFunction = ICoordinatesFunctionsFactory.Factory(functionElem.ToString());

            //todo i need to create the row item from the id? i just need to grab it from the database i guess
            ImpArea = new ImpactAreaRowItem(selectedImpArea, "teststageDamageCurve", -1, new System.Collections.ObjectModel.ObservableCollection<object>());
            DamCat = selectedDamCat;
            Function = coordFunction;

        }

        public ImpactAreaRowItem ImpArea { get; }
        public string DamCat { get; }
        public ICoordinatesFunction Function { get; }


        public XElement WriteToXML(StageDamageCurve curve)
        {
            XElement stageDamageCurveElement = new XElement(STAGE_DAMAGE_CURVE_TAG);
            stageDamageCurveElement.SetAttributeValue(SELECTED_IMPACT_AREA_TAG, curve.ImpArea.ID);
            stageDamageCurveElement.SetAttributeValue(SELECTED_DAM_CAT_TAG, curve.DamCat);
            stageDamageCurveElement.Add(curve.Function.WriteToXML());

            return stageDamageCurveElement;
        }
    }
}
