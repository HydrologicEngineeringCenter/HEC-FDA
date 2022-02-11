using paireddata;
using System;
using System.Collections.Generic;
using ViewModel.Utilities;

namespace ViewModel.GeoTech
{
    //[Author(q0heccdm, 6 / 8 / 2017 1:11:19 PM)]
    public class LeveeFeatureElement : ChildElement
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
        public LeveeFeatureElement(string userProvidedName, string creationDate, string description, double elevation, bool isDefault, UncertainPairedData failureFunction) : base()
        {
            Name = userProvidedName;
            LastEditDate = creationDate;
            Curve = failureFunction;

            IsDefaultCurveUsed = isDefault;
            if (isDefault)
            {
                CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/LeveeFeature.png");
            }
            else
            {
                CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/FailureFunction.png");
            }

            Description = description;
            if (Description == null) Description = "";
            Elevation = elevation;

            NamedAction editLeveeFeature = new NamedAction();
            editLeveeFeature.Header = "Edit Levee Feature...";
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
        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetLeveeManager().Remove(this);
        }
        public void EditLeveeFeature(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);

            LeveeFeatureEditorVM vm = new LeveeFeatureEditorVM(this, actionManager);
            string header = "Edit " + Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditLevee" + Name);
            Navigate(tab, false,false);
        }

        
        #endregion
        #region Functions
        public ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            LeveeFeatureEditorVM editorVM = (LeveeFeatureEditorVM)vm;
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM

            return new LeveeFeatureElement(editorVM.Name, editDate, editorVM.Description,editorVM.Elevation, editorVM.IsUsingDefault, editorVM.Curve);
        }

        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            LeveeFeatureElement elem = (LeveeFeatureElement)elementToClone;
            return new LeveeFeatureElement(elem.Name,elem.LastEditDate, elem.Description, elem.Elevation, elem.IsDefaultCurveUsed, elem.Curve);
        }
        #endregion
    }


}
