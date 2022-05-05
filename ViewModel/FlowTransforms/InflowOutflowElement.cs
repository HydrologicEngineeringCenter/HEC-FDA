using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.FlowTransforms
{
    //[Author(q0heccdm, 6 / 8 / 2017 10:33:22 AM)]
    public class InflowOutflowElement : CurveChildElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 10:33:22 AM
        #endregion

        #region Constructors
        public InflowOutflowElement(string userProvidedName, string lastEditDate, string description, ComputeComponentVM inflowOutflowCurve, int id):base(id)
        {
            LastEditDate = lastEditDate;
            Name = userProvidedName;
            CustomTreeViewHeader = new CustomHeaderVM(Name)
            {
                ImageSource = ImageSources.INFLOW_OUTFLOW_IMAGE,
                Tooltip = StringConstants.CreateChildNodeTooltip(lastEditDate)
            };

            Description = description;
            if (Description == null) Description = "";
            ComputeComponentVM = inflowOutflowCurve;

            NamedAction editInflowOutflowCurve = new NamedAction();
            editInflowOutflowCurve.Header = StringConstants.EDIT_REG_UNREG_MENU;
            editInflowOutflowCurve.Action = EditInflowOutflowCurve;

            NamedAction removeInflowOutflowCurve = new NamedAction();
            removeInflowOutflowCurve.Header = StringConstants.REMOVE_MENU;
            removeInflowOutflowCurve.Action = RemoveElement;

            NamedAction renameInflowOutflowCurve = new NamedAction(this);
            renameInflowOutflowCurve.Header = StringConstants.RENAME_MENU;
            renameInflowOutflowCurve.Action = Rename;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(editInflowOutflowCurve);
            localActions.Add(removeInflowOutflowCurve);
            localActions.Add(renameInflowOutflowCurve);

            Actions = localActions;
        }
        #endregion
        #region Voids
        public void EditInflowOutflowCurve(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);

            Editors.InflowOutflowEditorVM vm = new Editors.InflowOutflowEditorVM(this, actionManager);

            string header = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditInflowOutflow" + vm.Name);
            Navigate( tab, false, false);
        }

        #endregion
        #region Functions
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            ChildElement clonedElem = null;
            if (elementToClone is InflowOutflowElement elem)
            {
                clonedElem = new InflowOutflowElement(elem.Name, elem.LastEditDate,elem.Description,elem.ComputeComponentVM, elem.ID);
            }
            return clonedElem;
        }
        #endregion

        
    }
}
