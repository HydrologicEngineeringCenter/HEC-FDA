using paireddata;
using System;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.TableWithPlot;

namespace HEC.FDA.ViewModel.StageTransforms
{
    //[Author(q0heccdm, 6 / 8 / 2017 11:31:34 AM)]
    public class ExteriorInteriorElement : CurveChildElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 11:31:34 AM
        #endregion
        #region Fields
        #endregion
        #region Properties   
        #endregion
        #region Constructors
        public ExteriorInteriorElement(string userProvidedName,string lastEditDate, string desc, ComputeComponentVM exteriorInteriorCurve, int id):base(id)
        {
            LastEditDate = lastEditDate;
            Name = userProvidedName;
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/ExteriorInteriorStage.png");

            Description = desc;
            if (Description == null) Description = "";
            ComputeComponentVM = exteriorInteriorCurve;

            NamedAction editExteriorInteriorCurve = new NamedAction();
            editExteriorInteriorCurve.Header = "Edit Exterior Interior Curve...";
            editExteriorInteriorCurve.Action = EditExteriorInteriorCurve;

            NamedAction removeExteriorInteriorCurve = new NamedAction();
            removeExteriorInteriorCurve.Header = StringConstants.REMOVE_MENU;
            removeExteriorInteriorCurve.Action = RemoveElement;

            NamedAction renameElement = new NamedAction(this);
            renameElement.Header = StringConstants.RENAME_MENU;
            renameElement.Action = Rename;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(editExteriorInteriorCurve);
            localActions.Add(removeExteriorInteriorCurve);
            localActions.Add(renameElement);

            Actions = localActions;
        }
        #endregion
        #region Voids
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            ExteriorInteriorElement elem = (ExteriorInteriorElement)elementToClone;

            return new ExteriorInteriorElement(elem.Name, elem.LastEditDate, elem.Description, elem.ComputeComponentVM, elem.ID);
        }
        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetExteriorInteriorManager().Remove(this);
        }
        public void EditExteriorInteriorCurve(object arg1, EventArgs arg2)
        {         
            //create action manager
            EditorActionManager actionManager = new EditorActionManager()
                .WithSiblingRules(this);

            ExteriorInteriorEditorVM vm = new ExteriorInteriorEditorVM(this, actionManager);
            string header = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditExtInt"+vm.Name);
            Navigate(tab, false, true);         
        }
        #endregion

    }
}
