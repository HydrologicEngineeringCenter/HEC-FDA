using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Utilities;
using Model;

namespace ViewModel.GeoTech
{
    //[Author(q0heccdm, 6 / 8 / 2017 1:11:19 PM)]
    public class LeveeFeatureElement : Utilities.ChildElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 1:11:19 PM
        #endregion
        #region Fields
        private const string _TableConstant = "Levee Feature - ";

        private string _Description;
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
        public LeveeFeatureElement(string userProvidedName, string creationDate, string description, double elevation, bool isDefault, IFdaFunction failureFunction) : base()
        {
            Name = userProvidedName;
            LastEditDate = creationDate;
            Curve = failureFunction;

            IsDefaultCurveUsed = isDefault;
            if (isDefault)
            {
                CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/LeveeFeature.png");
            }
            else
            {
                CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/FailureFunction.png");
            }

            Description = description;
            if (Description == null) Description = "";
            Elevation = elevation;

            Utilities.NamedAction editLeveeFeature = new Utilities.NamedAction();
            editLeveeFeature.Header = "Edit Levee Feature...";
            editLeveeFeature.Action = EditLeveeFeature;

            Utilities.NamedAction removeLeveeFeature = new Utilities.NamedAction();
            removeLeveeFeature.Header = StringConstants.REMOVE_MENU;
            removeLeveeFeature.Action = RemoveElement;

            Utilities.NamedAction renameElement = new Utilities.NamedAction(this);
            renameElement.Header = StringConstants.RENAME_MENU;
            renameElement.Action = Rename;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
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

            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetLeveeManager(), this, (editorVM) => CreateElementFromEditor(editorVM),
                (editorVM, element) => AssignValuesFromElementToCurveEditor(editorVM, element),
                 (editorVM, elem) => AssignValuesFromCurveEditorToElement(editorVM, elem));

            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveUndoRedo(saveHelper)
                .WithSiblingRules(this);

            LeveeFeatureEditorVM vm = new LeveeFeatureEditorVM(this, actionManager);
            string header = "Edit " + Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditLevee" + Name);
            Navigate(tab, false,false);
            //if (!vm.WasCanceled)
            //{
            //    if (!vm.HasError)
            //    {
            //        string oldName = Name;
            //        Name = vm.Name;
            //        Description = vm.Description;
            //        Elevation = vm.Elevation;

            //        ((LeveeFeatureOwnerElement)_Owner).UpdateTableRowIfModified((Utilities.ParentElement)_Owner, oldName, this);
            //        UpdateAndSaveFailureFunctionsWithNewLevee(oldName);

            //        AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(vm.Name, Utilities.Transactions.TransactionEnum.EditExisting, "Previous Name: " + oldName + " Description: " + Description + " Elevation: " + Elevation));
            //    }
            //}
        }

        private void UpdateAndSaveFailureFunctionsWithNewLevee(string oldName)
        {
            ////get the list of failure functions and update their selected levees
            //List<FailureFunctionElement> ffList = GetElementsOfType<FailureFunctionElement>();

            //foreach (FailureFunctionElement ffe in ffList)
            //{
            //    if (ffe.SelectedLateralStructure.Name == oldName)
            //    {
            //        ffe.SelectedLateralStructure = this;
            //    }
            //}
            ////save the changes
            //List<FailureFunctionOwnerElement> ffOwners = GetElementsOfType<FailureFunctionOwnerElement>();
            //if (ffOwners.FirstOrDefault() != null)
            //{
            //    ffOwners.FirstOrDefault().Save();
            //}
        }

        //public override string TableName
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}



        //public override void Save()
        //{
        //    throw new NotImplementedException();
        //}

        //public override object[] RowData()
        //{
        //    return new object[] { Name , Description, Elevation};
        //}

        //public override bool SavesToRow()
        //{
        //    return true;
        //}
        //public override bool SavesToTable()
        //{
        //    return false;
        //}

        #endregion
        #region Functions
        public ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            LeveeFeatureEditorVM editorVM = (LeveeFeatureEditorVM)vm;
            //Editors.CurveEditorVM vm = (Editors.CurveEditorVM)editorVM;
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
