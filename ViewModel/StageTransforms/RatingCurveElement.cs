using Model;
using paireddata;
using System;
using System.Collections.Generic;
using ViewModel.Utilities;

namespace ViewModel.StageTransforms
{
    public class RatingCurveElement : ChildElement
    {
        
        #region Notes
        #endregion
        #region Fields

        #endregion
        #region Properties
        #endregion
        #region Constructors

        public RatingCurveElement(string userprovidedname, string creationDate, string desc, UncertainPairedData ratingCurve) : base()
        {
            LastEditDate = creationDate;
            Name = userprovidedname;
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/RatingCurve.png");

            Curve = ratingCurve;
            Description = desc;
            if (Description == null) Description = "";
            NamedAction editRatingCurve = new NamedAction();
            editRatingCurve.Header = "Edit Rating Curve...";
            editRatingCurve.Action = EditRatingCurve;

            NamedAction removeRatingCurve = new NamedAction();
            removeRatingCurve.Header = StringConstants.REMOVE_MENU;
            removeRatingCurve.Action = RemoveElement;

            NamedAction renameElement = new NamedAction(this);
            renameElement.Header = StringConstants.RENAME_MENU;
            renameElement.Action = Rename;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(editRatingCurve);
            localActions.Add(removeRatingCurve);
            localActions.Add(renameElement);

            Actions = localActions;

        }



        #endregion
        #region Voids
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            return new RatingCurveElement(elementToClone.Name, elementToClone.LastEditDate, elementToClone.Description, elementToClone.Curve);
        }
        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetRatingManager().Remove(this);
        }

        public void EditRatingCurve(object arg1, EventArgs arg2)
        {
           
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetRatingManager(),this, (editorVM) => CreateElementFromEditor(editorVM),
                (editorVM, element) => AssignValuesFromElementToCurveEditor(editorVM, element),
                 (editorVM, elem) => AssignValuesFromCurveEditorToElement(editorVM, elem));

            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveUndoRedo(saveHelper)
                .WithSiblingRules(this);
           
            Editors.CurveEditorVM vm = new Editors.CurveEditorVM(this,  "Outflow", "Exterior Stage", "Outflow - Exterior Stage", actionManager);
            string header = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditRatingCurve" + vm.Name);
            Navigate(tab,false, false);   
        }
     

        public  ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            Editors.CurveEditorVM editorVM = (Editors.CurveEditorVM)vm;
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM

            return new RatingCurveElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve);
        }

        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
        }


        #endregion


        public override bool Equals(object obj)
        {
            bool retval = true;
            if (obj.GetType() == typeof(RatingCurveElement))
            {
                RatingCurveElement elem = (RatingCurveElement)obj;
                if (!Name.Equals(elem.Name))
                {
                    retval = false;
                }
                if(Description == null && elem.Description != null)
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

            }
            else
            {
                retval = false;
            }
            return retval;
        }

    }
}
