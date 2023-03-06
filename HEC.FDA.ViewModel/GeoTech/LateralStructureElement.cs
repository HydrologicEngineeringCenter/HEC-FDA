using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using Statistics;
using Statistics.Distributions;
using System;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.GeoTech
{
    //[Author(q0heccdm, 6 / 8 / 2017 1:11:19 PM)]
    public class LateralStructureElement : CurveChildElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 1:11:19 PM
        #endregion
        #region Fields
        private const string IS_DEFAULT = "IsUsingDefaultCurve";
        private const string ELEVATION = "Elevation";
        private double _Elevation;

        #endregion
        #region Properties
       public bool IsDefaultCurveUsed
        {
            get; set;
        }

        public double Elevation
        {
            get { return _Elevation; }
            set { _Elevation = value; NotifyPropertyChanged(); }
        }

        #endregion
        #region Constructors
        public LateralStructureElement(string name, string lastEditDate, string description, double elevation, bool isDefault, CurveComponentVM failureFunction, int id) 
            : base(name, lastEditDate, description,failureFunction,  id)
        {
            IsDefaultCurveUsed = isDefault;
            Elevation = elevation;

            if (!isDefault)
            {
                CustomTreeViewHeader = new CustomHeaderVM(Name)
                {
                    ImageSource = ImageSources.FAILURE_IMAGE,
                    Tooltip = StringConstants.CreateLastEditTooltip(LastEditDate)
                };
            }

            AddDefaultActions(EditLeveeFeature, StringConstants.EDIT_LATERAL_STRUCTURES_MENU);
        }

        public LateralStructureElement(XElement element, int id):base(element,  id)
        {
            IsDefaultCurveUsed = Convert.ToBoolean( element.Attribute(IS_DEFAULT).Value);
            Elevation = Convert.ToDouble( element.Attribute(ELEVATION).Value);
            AddDefaultActions(EditLeveeFeature, StringConstants.EDIT_LATERAL_STRUCTURES_MENU);
        }

        #endregion
        #region Voids
        public void EditLeveeFeature(object arg1, EventArgs arg2)
        {
            EditorActionManager actionManager = new EditorActionManager()
                .WithSiblingRules(this);

            LeveeFeatureEditorVM vm = new LeveeFeatureEditorVM(this, actionManager);
            string header = "Edit " + Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditLevee" + Name);
            Navigate(tab, false,false);
        }        
        #endregion
        public UncertainPairedData CreateDefaultCurve()
        {
            double elev = Elevation;
            double[] xs = new double[] { elev, elev + double.Epsilon };
            IDistribution[] ys = new IDistribution[] { new Deterministic(0), new Deterministic(1) };
            CurveMetaData curveMetaData = new CurveMetaData(StringConstants.STAGE, StringConstants.FREQUENCY, StringConstants.FAILURE_FREQUENCY);
            return new UncertainPairedData(xs, ys, curveMetaData);
        }

        public override XElement ToXML()
        {
            XElement childElem = base.ToXML();
            childElem.SetAttributeValue(IS_DEFAULT, IsDefaultCurveUsed);
            childElem.SetAttributeValue(ELEVATION, Elevation);
            return childElem;
        }

        public bool Equals(LateralStructureElement elem)
        {
            bool isEqual = true;

            if (!AreHeaderDataEqual(elem))
            {
                isEqual = false;
            }

            if (IsDefaultCurveUsed != elem.IsDefaultCurveUsed)
            {
                isEqual = false;
            }
            if (Elevation != elem.Elevation)
            {
                isEqual = false;
            }
            
            return isEqual;
        }

    }
}
