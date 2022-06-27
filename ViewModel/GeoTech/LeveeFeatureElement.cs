using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using paireddata;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.GeoTech
{
    //[Author(q0heccdm, 6 / 8 / 2017 1:11:19 PM)]
    public class LeveeFeatureElement : CurveChildElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 1:11:19 PM
        #endregion
        #region Fields
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
        public LeveeFeatureElement(string userProvidedName, string creationDate, string description, double elevation, bool isDefault, ComputeComponentVM failureFunction, int id) : base(id)
        {
            Name = userProvidedName;
            LastEditDate = creationDate;

            IsDefaultCurveUsed = isDefault;

            if (isDefault)
            {
                CustomTreeViewHeader = new CustomHeaderVM(Name)
                {
                    ImageSource = ImageSources.LEVEE_FEATURE_IMAGE,
                    Tooltip = StringConstants.CreateLastEditTooltip(LastEditDate)
                };    
            }
            else
            {
                CustomTreeViewHeader = new CustomHeaderVM(Name)
                {
                    ImageSource = ImageSources.FAILURE_IMAGE,
                    Tooltip = StringConstants.CreateLastEditTooltip(LastEditDate)
                };
            }

            ComputeComponentVM = failureFunction;
            Description = description;
            if (Description == null) Description = "";
            Elevation = elevation;

            NamedAction editLeveeFeature = new NamedAction();
            editLeveeFeature.Header = StringConstants.EDIT_LATERAL_STRUCTURES_MENU;
            editLeveeFeature.Action = EditLeveeFeature;

            NamedAction removeLeveeFeature = new NamedAction();
            removeLeveeFeature.Header = StringConstants.REMOVE_MENU;
            removeLeveeFeature.Action = RemoveElement;

            NamedAction renameElement = new NamedAction(this);
            renameElement.Header = StringConstants.RENAME_MENU;
            renameElement.Action = Rename;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(editLeveeFeature);
            localActions.Add(removeLeveeFeature);
            localActions.Add(renameElement);

            Actions = localActions;
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
        #region Functions

        public UncertainPairedData CreateDefaultCurve()
        {
            double elev = Elevation;
            double[] xs = new double[] { elev, elev + double.Epsilon };
            IDistribution[] ys = new IDistribution[] { new Deterministic(0), new Deterministic(1) };
            CurveMetaData curveMetaData = new CurveMetaData(StringConstants.STAGE, StringConstants.FREQUENCY, StringConstants.FAILURE_FREQUENCY);
            return new UncertainPairedData(xs, ys, curveMetaData);
        }

        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            ChildElement clonedElem = null;
            if (elementToClone is LeveeFeatureElement elem)
            {
                clonedElem = new LeveeFeatureElement(elem.Name,elem.LastEditDate, elem.Description, elem.Elevation, elem.IsDefaultCurveUsed, elem.ComputeComponentVM, elem.ID);
            }
            return clonedElem;
        }
        #endregion
    }
}
