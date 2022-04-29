using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.StageTransforms
{
    public class RatingCurveElement : CurveChildElement
    {
        
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors

        public RatingCurveElement(string userprovidedname, string creationDate, string desc, ComputeComponentVM ratingCurve, int id) : base(id)
        {
            LastEditDate = creationDate;
            Name = userprovidedname;
            CustomTreeViewHeader = new CustomHeaderVM(Name)
            {
                ImageSource = "pack://application:,,,/View;component/Resources/RatingCurve.png",
                Tooltip = StringConstants.CreateChildNodeTooltip(LastEditDate)
            };


            ComputeComponentVM = ratingCurve;
            Description = desc;
            if (Description == null) Description = "";
            NamedAction editRatingCurve = new NamedAction();
            editRatingCurve.Header = StringConstants.EDIT_STAGE_DISCHARGE_MENU;
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
            ChildElement clonedElem = null;
            if(elementToClone is RatingCurveElement elem)
            {
                clonedElem = new RatingCurveElement(elementToClone.Name, elementToClone.LastEditDate, elementToClone.Description, elem.ComputeComponentVM, elem.ID);
            }
            return clonedElem;
        }

        public void EditRatingCurve(object arg1, EventArgs arg2)
        {       
            EditorActionManager actionManager = new EditorActionManager()
                .WithSiblingRules(this);

            RatingCurveEditorVM vm = new RatingCurveEditorVM(this, actionManager);
            string header = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditRatingCurve" + vm.Name);
            Navigate(tab,false, false);   
        }

       

        #endregion

    }
}
