using FdaViewModel.Editors;
using FdaViewModel.Utilities;
using Model;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.StageTransforms
{
    public class RatingCurveElement : ChildElement
    {
        
        #region Notes
        #endregion
        #region Fields
        private FdaLogging.FdaLogger _Logger = new FdaLogging.FdaLogger("RatingCurveElement");
        private const string TABLE_NAME_CONSTANT = "Rating Curve - ";

        #endregion
        #region Properties
        #endregion
        #region Constructors

        public RatingCurveElement(string userprovidedname, string creationDate, string desc, UncertainCurveDataCollection ratingCurve) : base()
        {
           // _Logger.LogInfo("Creating new rating curve element: " + Name, GetType(), Name);
            //FdaLogging.RetrieveFromDB.GetMessageRowsForType(GetType(), Name);
            LastEditDate = creationDate;
            Name = userprovidedname;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/RatingCurve.png");

            Curve = ratingCurve;
            Description = desc;
            if (Description == null) Description = "";
            Utilities.NamedAction editRatingCurve = new Utilities.NamedAction();
            editRatingCurve.Header = "Edit Rating Curve";
            editRatingCurve.Action = EditRatingCurve;

            Utilities.NamedAction removeRatingCurve = new Utilities.NamedAction();
            removeRatingCurve.Header = "Remove";
            removeRatingCurve.Action = RemoveElement;

            Utilities.NamedAction renameElement = new Utilities.NamedAction(this);
            renameElement.Header = "Rename";
            renameElement.Action = Rename;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
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
            //Utilities.CustomMessageBoxVM vm = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK_Cancel, "Are you sure you want to delete this element?\r\n" + Name);
            //Navigate(vm);
            //if (vm.ClickedButton == CustomMessageBoxVM.ButtonsEnum.OK)
            //{
                Saving.PersistenceFactory.GetRatingManager().Remove(this);
            
        }

        public void EditRatingCurve(object arg1, EventArgs arg2)
        {
            //foreach (Utilities.NamedAction a in Actions)
            //{
            //    if (a.Header.Equals("Edit Rating Curve"))
            //    {
            //        a.IsEnabled = false;
            //    }
            //}

            //if(IsOpenInTabOrWindow == true)
            //{
            //    ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage(Name + " is already open for editing",FdaModel.Utilities.Messager.ErrorMessageEnum.Fatal));
            //    return;
            //}

            //IsOpenInTabOrWindow = true;

            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetRatingManager(),this, (editorVM) => CreateElementFromEditor(editorVM),
                (editorVM, element) => AssignValuesFromElementToCurveEditor(editorVM, element),
                 (editorVM, elem) => AssignValuesFromCurveEditorToElement(editorVM, elem));

            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveUndoRedo(saveHelper)
                .WithSiblingRules(this);
            //.WithParentGuid(this.GUID)
            //.WithCanOpenMultipleTimes(false);

            //int ratingId = Storage.Connection.Instance.GetElementId()
            //_Logger.LogInfo("Opening " + Name + " for edit.", this.GetType(), Name);
            Editors.CurveEditorVM vm = new Editors.CurveEditorVM(this, actionManager);
            string header = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditRatingCurve" + vm.Name);
            Navigate(tab,false, false);   
        }

        //public  void AssignValuesFromEditorToElement(BaseEditorVM editorVM, ChildElement element)
        //{
        //    CurveEditorVM vm = (CurveEditorVM)editorVM;
        //    element.Name = vm.Name;
        //    element.Description = vm.Description;
        //    element.Curve = vm.Curve;
        //    element.UpdateTreeViewHeader(vm.Name);
        //}

        //public  void AssignValuesFromElementToEditor(BaseEditorVM editorVM, ChildElement element)
        //{
        //    CurveEditorVM vm = (CurveEditorVM)editorVM;

        //    vm.Name = element.Name;
        //    vm.Description = element.Description;
        //    vm.Curve = element.Curve;
        //}

        public  ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            Editors.CurveEditorVM editorVM = (Editors.CurveEditorVM)vm;
            //Editors.CurveEditorVM vm = (Editors.CurveEditorVM)editorVM;
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM

            return new RatingCurveElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve);
        }

        private bool DidStateChange()
        {
            return true;
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
                    return false;
                }
                else if (Description != null && !Description.Equals(elem.Description))
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

        private bool areCurvesEqual(UncertainCurveDataCollection curve2)
        {
            bool retval = true;
            if (Curve.GetType() != curve2.GetType())
            {
                return false;
            }
            if (Curve.Distribution != curve2.Distribution)
            {
                return false;
            }
            if (Curve.XValues.Count != curve2.XValues.Count)
            {
                return false;
            }
            if (Curve.YValues.Count != curve2.YValues.Count)
            {
                return false;
            }
            double epsilon = .0001;
            for (int i = 0; i < Curve.XValues.Count; i++)
            {
                if (Math.Abs(Curve.get_X(i)) - Math.Abs(curve2.get_X(i)) > epsilon)
                {
                    return false;
                }
                ContinuousDistribution y = Curve.get_Y(i);
                ContinuousDistribution y2 = curve2.get_Y(i);
                if (Math.Abs(y.GetCentralTendency) - Math.Abs(y2.GetCentralTendency) > epsilon)
                {
                    return false;
                }
                if (Math.Abs(y.GetSampleSize) - Math.Abs(y2.GetSampleSize) > epsilon)
                {
                    return false;
                }
            }

            return retval;
        }

    }
}
