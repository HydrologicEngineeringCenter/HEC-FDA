using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using FdaViewModel.Utilities;
using FdaViewModel.Editors;
using System.Collections.ObjectModel;
using Statistics;

namespace FdaViewModel.GeoTech
{
    //[Author(q0heccdm, 6 / 8 / 2017 2:04:38 PM)]
    public class FailureFunctionElement : Utilities.ChildElement
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
        public FailureFunctionElement(string userProvidedName, string lastEditDate, string description, 
            Statistics.UncertainCurveDataCollection failureFunctionCurve, LeveeFeatureElement selectedLatStructure) : base()
        {
            LastEditDate = lastEditDate;
            Name = userProvidedName;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/FailureFunction.png");

            Description = description;
            if (Description == null) Description = "";
            Curve = failureFunctionCurve;
            if (selectedLatStructure == null)
            {
                SelectedLateralStructure = new LeveeFeatureElement("", "", 0);
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
            removeFailureFunctionCurve.Action = RemoveElement;

            Utilities.NamedAction renameElement = new Utilities.NamedAction(this);
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

        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetFailureFunctionManager().Remove(this);
        }
        public void EditFailureFunctionCurve(object arg1, EventArgs arg2)
        {
            List<LeveeFeatureElement> leveeList = StudyCache.GetChildElementsOfType<LeveeFeatureElement>();
            ObservableCollection<LeveeFeatureElement> leveeCollection = new ObservableCollection<LeveeFeatureElement>();
            foreach (LeveeFeatureElement elem in leveeList)
            {
                leveeCollection.Add(elem);
            }

            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetFailureFunctionManager()
                ,this, (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToEditor(editor, element),
                (editor, element) => AssignValuesFromEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                //.WithOwnerValidationRules((editorVM, oldName) => AddOwnerRules(editorVM, oldName))
                .WithSaveUndoRedo(saveHelper)
                 .WithSiblingRules(this);
               //.WithParentGuid(this.GUID)
               //.WithCanOpenMultipleTimes(false);

            Editors.CurveEditorVM vm = new Editors.FailureFunctionCurveEditorVM(this, leveeCollection, actionManager);
            //StudyCache.AddSiblingRules(vm, this);
            string header = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditFailureFunction"+vm.Name);
            Navigate(tab, false, false);
            ////get the current list of levees
            //List<LeveeFeatureElement> leveeList = GetElementsOfType<LeveeFeatureElement>();

            //Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper((helper, elem) => ((Utilities.ParentElement)_Owner).SaveExistingElement(helper, elem), ChangeTableName());

            //Editors.EditorActionManager actionManager = new Editors.EditorActionManager()

            //    .WithOwnerValidationRules((editorVM, oldName) => ((Utilities.ParentElement)_Owner).AddOwnerRules(editorVM, oldName))
            //    .WithSaveUndoRedo(saveHelper, (editorVM) => ((Utilities.ParentElement)_Owner).CreateElementFromEditor(editorVM),
            //    (editorVM, element) => ((Utilities.ParentElement)_Owner).AssignValuesFromElementToEditor(editorVM, element),
            //     (editorVM, elem) => ((Utilities.ParentElement)_Owner).AssignValuesFromEditorToElement(editorVM, elem));



            //Editors.FailureFunctionCurveEditorVM vm = new Editors.FailureFunctionCurveEditorVM(this, leveeList, actionManager);


            ////FailureFunctionEditorVM vm = new FailureFunctionEditorVM(this, (foo) => ((Utilities.OwnerElement)_Owner).SaveExistingElement(foo), (bar) => ((Utilities.OwnerElement)_Owner).AddOwnerRules(bar), leveeList);
            //Navigate(vm, true, true);
            //if (!vm.WasCanceled)
            //{
            //    if (!vm.HasError)
            //    {
            //        vm.SaveWhileEditing();

            //    }
            //}
        }
        #endregion
        #region Functions
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            FailureFunctionElement elem = (FailureFunctionElement)elementToClone;
            return new FailureFunctionElement(elem.Name, elem.LastEditDate, elem.Description, elem.Curve, elem.SelectedLateralStructure);
        }
        public ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            Editors.FailureFunctionCurveEditorVM editorVM = (Editors.FailureFunctionCurveEditorVM)vm;

            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new FailureFunctionElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve, editorVM.SelectedLateralStructure);
            //return null;
        }

        public void AssignValuesFromEditorToElement(BaseEditorVM editorVM, ChildElement elem)
        {
            FailureFunctionCurveEditorVM vm = (FailureFunctionCurveEditorVM)editorVM;
            FailureFunctionElement element = (FailureFunctionElement)elem;

            element.Name = vm.Name;
            element.Description = vm.Description;
            element.Curve = vm.Curve;
            element.SelectedLateralStructure = vm.SelectedLateralStructure;
            element.UpdateTreeViewHeader(vm.Name);
        }

        public void AssignValuesFromElementToEditor(BaseEditorVM editorVM, ChildElement elem)
        {
            FailureFunctionCurveEditorVM vm = (FailureFunctionCurveEditorVM)editorVM;
            FailureFunctionElement element = (FailureFunctionElement)elem;

            vm.Name = element.Name;
            vm.Description = element.Description;
            vm.Curve = element.Curve;
            vm.SelectedLateralStructure = element.SelectedLateralStructure;
        }
        #endregion


        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            bool retval = true;
            if (obj.GetType() == typeof(FailureFunctionElement))
            {
                FailureFunctionElement elem = (FailureFunctionElement)obj;
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
