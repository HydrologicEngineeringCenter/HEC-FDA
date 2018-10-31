using FdaViewModel.Editors;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.StageTransforms
{
    public class RatingCurveElement : Utilities.ChildElement
    {
        #region Notes
        #endregion
        #region Fields

        private const string TABLE_NAME_CONSTANT = "Rating Curve - ";

        #endregion
        #region Properties
     
      
        #endregion
        #region Constructors

        public RatingCurveElement(string userprovidedname, string creationDate, string desc, Statistics.UncertainCurveDataCollection ratingCurve) : base()
        {
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

            if(IsOpenInTabOrWindow == true)
            {
                ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage(Name + " is already open for editing",FdaModel.Utilities.Messager.ErrorMessageEnum.Fatal));
                return;
            }

            IsOpenInTabOrWindow = true;

            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetRatingManager(),this, (editorVM) => CreateElementFromEditor(editorVM),
                (editorVM, element) => AssignValuesFromElementToCurveEditor(editorVM, element),
                 (editorVM, elem) => AssignValuesFromCurveEditorToElement(editorVM, elem));

            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveUndoRedo(saveHelper);



            Editors.CurveEditorVM vm = new Editors.CurveEditorVM(this, actionManager);


            Navigate(vm,false, false, "Edit " + vm.Name);
         
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

        //public override string ChangeTableName()
        //{
        //    return GetTableConstant() + Name + "-ChangeTable";
        //}
        //public override void Save()
        //{
        //    if (!Storage.Connection.Instance.IsOpen)
        //    {
        //        Storage.Connection.Instance.Open();
        //    }
        //    Curve.toSqliteTable(TableName); 
        //}

        //public override object[] RowData()
        //{
        //    return new object[] { Name, LastEditDate, Description, Curve.Distribution, Curve.GetType() };
        //}


        //public override bool SavesToRow()
        //{
        //    return true;
        //}
        //public override bool SavesToTable()
        //{
        //    return true;
        //}
        //public override string GetTableConstant()
        //{
        //    return TABLE_NAME_CONSTANT;
        //}

       
        //#endregion
        //#region Functions
        //public override string TableName
        //{
        //    get
        //    {
        //        return TABLE_NAME_CONSTANT + LastEditDate;
        //    }
        //}
        #endregion



    }
}
