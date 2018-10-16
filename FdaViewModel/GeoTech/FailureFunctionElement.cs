using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.GeoTech
{
    //[Author(q0heccdm, 6 / 8 / 2017 2:04:38 PM)]
    public class FailureFunctionElement : Utilities.OwnedElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 2:04:38 PM
        #endregion
        #region Fields
        private const string _TableConstant = "Failure Function - ";
        //private string _Description;
       // private Statistics.UncertainCurveDataCollection _Curve;
        //private List<LeveeFeatureElement> _LateralStructureList;
        private LeveeFeatureElement _SelectedLateralStructure;


        #endregion
        #region Properties
        public override string GetTableConstant()
        {
            return _TableConstant;
        }
        public LeveeFeatureElement SelectedLateralStructure
        {
            get { return _SelectedLateralStructure; }
            set { _SelectedLateralStructure = value; NotifyPropertyChanged(); }
        }
        //public List<LeveeFeatureElement> LateralStructureList
        //{
        //    get { return _LateralStructureList; }
        //    set { _LateralStructureList = value; NotifyPropertyChanged(); }
        //}
        //public string Description { get { return _Description; } set { _Description = value; NotifyPropertyChanged(); } }

        //public Statistics.UncertainCurveDataCollection FailureFunctionCurve
        //{
        //    get { return _Curve; }
        //    set { _Curve = value; NotifyPropertyChanged(); }
        //}
        #endregion
        #region Constructors
        public FailureFunctionElement(string userProvidedName, string lastEditDate, string description, Statistics.UncertainCurveDataCollection failureFunctionCurve, LeveeFeatureElement selectedLatStructure, BaseFdaElement owner) : base(owner)
        {
            LastEditDate = lastEditDate;
            Name = userProvidedName;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/FailureFunction.png");

            Description = description;
            if (Description == null) Description = "";
            Curve = failureFunctionCurve;
            if (selectedLatStructure == null)
            {
                SelectedLateralStructure = new LeveeFeatureElement("", "", 0, this);
            }
            else
            {
                SelectedLateralStructure = selectedLatStructure;
            }

            Utilities.NamedAction editFailureFunctionCurve = new Utilities.NamedAction();
            editFailureFunctionCurve.Header = "Edit Failure Function Curve";
            editFailureFunctionCurve.Action = EditFailureFunctionCurve;

            Utilities.NamedAction removeFailureFunctionCurve = new Utilities.NamedAction();
            removeFailureFunctionCurve.Header = "Remove";
            removeFailureFunctionCurve.Action = Remove;

            Utilities.NamedAction renameElement = new Utilities.NamedAction();
            renameElement.Header = "Rename";
            renameElement.Action = Rename;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(editFailureFunctionCurve);
            localActions.Add(removeFailureFunctionCurve);
            localActions.Add(renameElement);


            Actions = localActions;

        }
        #endregion
        #region Voids
        public void EditFailureFunctionCurve(object arg1, EventArgs arg2)
        {
            //get the current list of levees
            List<LeveeFeatureElement> leveeList = GetElementsOfType<LeveeFeatureElement>();

            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper((helper, elem) => ((Utilities.OwnerElement)_Owner).SaveExistingElement(helper, elem), ChangeTableName());

            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()

                .WithOwnerValidationRules((editorVM, oldName) => ((Utilities.OwnerElement)_Owner).AddOwnerRules(editorVM, oldName))
                .WithSaveUndoRedo(saveHelper, (editorVM) => ((Utilities.OwnerElement)_Owner).CreateElementFromEditor(editorVM),
                (editorVM, element) => ((Utilities.OwnerElement)_Owner).AssignValuesFromElementToEditor(editorVM, element),
                 (editorVM, elem) => ((Utilities.OwnerElement)_Owner).AssignValuesFromEditorToElement(editorVM, elem));



            Editors.FailureFunctionCurveEditorVM vm = new Editors.FailureFunctionCurveEditorVM(this, leveeList, actionManager);


            //FailureFunctionEditorVM vm = new FailureFunctionEditorVM(this, (foo) => ((Utilities.OwnerElement)_Owner).SaveExistingElement(foo), (bar) => ((Utilities.OwnerElement)_Owner).AddOwnerRules(bar), leveeList);
            Navigate(vm, true, true);
            if (!vm.WasCanceled)
            {
                if (!vm.HasError)
                {
                    vm.SaveWhileEditing();

                }
            }
        }
        #endregion
        #region Functions
        #endregion
        public override string TableName
        {
            get
            {
                return GetTableConstant() + LastEditDate;
            }
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            Curve.toSqliteTable(TableName);
        }

        public override object[] RowData()
        {
            return new object[] { Name, LastEditDate, Description, _SelectedLateralStructure.Name, Curve.Distribution };
        }

        public override bool SavesToRow()
        {
            return true;
        }
        public override bool SavesToTable()
        {
            return true;
        }
    }
}
