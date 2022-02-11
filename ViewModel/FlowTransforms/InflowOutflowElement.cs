using paireddata;
using System;
using System.Collections.Generic;
using ViewModel.Utilities;

namespace ViewModel.FlowTransforms
{
    //[Author(q0heccdm, 6 / 8 / 2017 10:33:22 AM)]
    public class InflowOutflowElement : ChildElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 10:33:22 AM
        #endregion
        #region Fields
        private const string _TableConstant = "Inflow Outflow - ";
        #endregion

        #region Constructors
        public InflowOutflowElement(string userProvidedName, string lastEditDate, string description, UncertainPairedData inflowOutflowCurve ):base()
        {
            LastEditDate = lastEditDate;
            Name = userProvidedName;
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/InflowOutflowCircle.png");
            
            Description = description;
            if (Description == null) Description = "";
            Curve = inflowOutflowCurve;

            NamedAction editInflowOutflowCurve = new NamedAction();
            editInflowOutflowCurve.Header = "Edit Inflow-Outflow Curve...";
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
        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetInflowOutflowManager().Remove(this);
        }
        public void EditInflowOutflowCurve(object arg1, EventArgs arg2)
        {
            AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(Name, Utilities.Transactions.TransactionEnum.EditExisting, 
                "Openning " + Name + " for editing.",nameof(InflowOutflowElement)));

            //create save helper
            Editors.SaveHelper saveHelper = new Editors.SaveHelper(Saving.PersistenceFactory.GetInflowOutflowManager()
                ,this, (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToCurveEditor(editor, element),
                (editor, element) => AssignValuesFromCurveEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveHelper(saveHelper)
                .WithSiblingRules(this);

            Editors.CurveEditorVM vm = new Editors.CurveEditorVM(this,  "Inflow", "Outflow", "Inflow - Outflow", actionManager);

            string header = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditInflowOutflow" + vm.Name);
            Navigate( tab, false, false);
        }

        #endregion
        #region Functions
        public ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            Editors.CurveEditorVM editorVM = (Editors.CurveEditorVM)vm;
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new InflowOutflowElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve);
        }
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            InflowOutflowElement elem = (InflowOutflowElement)elementToClone;
            return new InflowOutflowElement(elem.Name, elem.LastEditDate,elem.Description,elem.Curve);
        }
        #endregion

        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
            AddRule(nameof(Name), () => Name != null, "Name cannot be blank.");
        }

        public override bool Equals(object obj)
        {
            bool retval = true;
            if (obj.GetType() == typeof(InflowOutflowElement))
            {
                InflowOutflowElement elem = (InflowOutflowElement)obj;
                if (!Name.Equals(elem.Name))
                {
                    retval = false;
                }
                if (Description == null && elem.Description != null)
                {
                    retval = false;
                }
                else if (Description != null && !Description.Equals(elem.Description))
                {
                    retval = false;
                }
                if (!LastEditDate.Equals(elem.LastEditDate))
                {
                    retval = false;
                }
                if (Curve != elem.Curve)
                {
                    retval = false;
                }
            }
            else
            {
                retval = false;
            }
            return retval;
        }
    }
}
