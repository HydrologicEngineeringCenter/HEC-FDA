using paireddata;
using System;
using System.Collections.Generic;
using ViewModel.Editors;
using ViewModel.Utilities;

namespace ViewModel.StageTransforms
{
    //[Author(q0heccdm, 6 / 8 / 2017 11:31:34 AM)]
    public class ExteriorInteriorElement : ChildElement
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
        public ExteriorInteriorElement(string userProvidedName,string lastEditDate, string desc, UncertainPairedData exteriorInteriorCurve):base()
        {
            LastEditDate = lastEditDate;
            Name = userProvidedName;
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/ExteriorInteriorStage.png");

            Description = desc;
            if (Description == null) Description = "";
            Curve = exteriorInteriorCurve;

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
            return new ExteriorInteriorElement(elem.Name, elem.LastEditDate, elem.Description, elem.Curve);
        }
        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetExteriorInteriorManager().Remove(this);
        }
        public void EditExteriorInteriorCurve(object arg1, EventArgs arg2)
        {         
            //create save helper
            SaveHelper saveHelper = new SaveHelper(Saving.PersistenceFactory.GetExteriorInteriorManager()
                ,this, (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToCurveEditor(editor, element),
                (editor, element) => AssignValuesFromCurveEditorToElement(editor, element));
            //create action manager
            EditorActionManager actionManager = new EditorActionManager()
                .WithSaveHelper(saveHelper)
                .WithSiblingRules(this);

            CurveEditorVM vm = new CurveEditorVM(this, "Exterior - Interior Stage", "Exterior Stage", "Interior Stage", actionManager);
            string header = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditExtInt"+vm.Name);
            Navigate(tab, false, true);         
        }
        #endregion
        #region Functions
        public ChildElement CreateElementFromEditor(BaseEditorVM vm)
        {
            CurveEditorVM editorVM = (CurveEditorVM)vm;

            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new ExteriorInteriorElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve);
        }
        #endregion

        public override bool Equals(object obj)
        {
            if(Description == null)
            {
                Description = "";
            }

            bool retval = true;
            if(obj.GetType() == typeof(ExteriorInteriorElement))
            {
                ExteriorInteriorElement elem = (ExteriorInteriorElement)obj;
                if (!Name.Equals(elem.Name))
                {
                    retval = false;
                }
                if (!Description.Equals(elem.Description))
                {
                    retval = false;
                }
                if (!LastEditDate.Equals(elem.LastEditDate))
                {
                    retval = false;
                }
                if (!areCurvesEqual(elem.Curve))
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

        //todo: do i need this?
        private bool areCurvesEqual(UncertainPairedData curve2)
        {
            bool retval = true;
            //if (Curve.GetType() != curve2.GetType())
            //{
            //    return false;
            //}
            //if (Curve.Distribution != curve2.Distribution)
            //{
            //    return false;
            //}
            //if (Curve.XValues.Count != curve2.XValues.Count)
            //{
            //    return false;
            //}
            //if (Curve.YValues.Count != curve2.YValues.Count)
            //{
            //    return false;
            //}
            //double epsilon = .0001;
            //for (int i = 0; i < Curve.XValues.Count; i++)
            //{
            //    if (Math.Abs(Curve.get_X(i)) - Math.Abs(curve2.get_X(i)) > epsilon)
            //    {
            //        return false;
            //    }
            //    ContinuousDistribution y = Curve.get_Y(i);
            //    ContinuousDistribution y2 = curve2.get_Y(i);
            //    if (Math.Abs(y.GetCentralTendency) - Math.Abs(y2.GetCentralTendency) > epsilon)
            //    {
            //        return false;
            //    }
            //    if (Math.Abs(y.GetSampleSize) - Math.Abs(y2.GetSampleSize) > epsilon)
            //    {
            //        return false;
            //    }
            //}

            return retval;
        }
    }
}
