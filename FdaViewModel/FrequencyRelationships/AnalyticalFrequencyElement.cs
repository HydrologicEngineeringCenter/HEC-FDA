using FdaViewModel.Editors;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.FrequencyRelationships
{
    public class AnalyticalFrequencyElement : Utilities.ChildElement
    {
        #region Notes
        #endregion
        #region Fields
        private const string _TableConstant = "Analytical Frequency - ";

        private string _Description = "";
        private Statistics.LogPearsonIII _Distribution;
        #endregion
        #region Properties
      
        public Statistics.LogPearsonIII Distribution
        {
            get { return _Distribution; }
            set { _Distribution = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public AnalyticalFrequencyElement(string name, string lastEditDate, string desc, Statistics.LogPearsonIII dist, Utilities.ParentElement owner = null) : base(owner)
        {
            LastEditDate = lastEditDate;
            Name = name;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/FrequencyCurve.png");

            Description = desc;
            if (Description == null) Description = "";
            Distribution = dist;
            Utilities.NamedAction editflowfreq = new Utilities.NamedAction();
            editflowfreq.Header = "Edit Analyitical Flow Frequency Relationship";
            editflowfreq.Action = EditFlowFreq;

            Utilities.NamedAction removeflowfreq = new Utilities.NamedAction();
            removeflowfreq.Header = "Remove";
            removeflowfreq.Action = RemoveElement;

            Utilities.NamedAction renameElement = new Utilities.NamedAction();
            renameElement.Header = "Rename";
            renameElement.Action = Rename;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(editflowfreq);
            localActions.Add(removeflowfreq);
            localActions.Add(renameElement);

            Actions = localActions;
        }


        #endregion
        #region Voids
        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetFlowFrequencyManager(StudyCache).Remove(this);
        }
        public void EditFlowFreq(object arg1, EventArgs arg2)
        {

            //create save helper
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetFlowFrequencyManager(StudyCache)
                ,this, (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToEditor(editor, element),
                (editor, element) => AssignValuesFromEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveUndoRedo(saveHelper);

            AnalyticalFrequencyEditorVM vm = new AnalyticalFrequencyEditorVM(this, actionManager);// Name, Distribution, Description, _Owner);
            Navigate(vm, false, false, "Edit Frequency");
            //if (!vm.WasCanceled)
            //{
            //    if (!vm.HasError)
            //    {
            //        vm.SaveWhileEditing();
            //    }
            //}
        }

        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            AnalyticalFrequencyElement elem = (AnalyticalFrequencyElement)elementToClone;
            return new AnalyticalFrequencyElement(elem.Name, elem.LastEditDate, elem.Description,elem.Distribution);
        }

        public void AssignValuesFromEditorToElement(BaseEditorVM editorVM, ChildElement elem)
        {
            AnalyticalFrequencyEditorVM vm = (AnalyticalFrequencyEditorVM)editorVM;
            AnalyticalFrequencyElement element = (AnalyticalFrequencyElement)elem;
            element.Name = vm.Name;
            element.Description = vm.Description;
            element.Distribution = vm.Distribution;
            element.UpdateTreeViewHeader(vm.Name);
        }

        public void AssignValuesFromElementToEditor(BaseEditorVM editorVM, ChildElement elem)
        {
            AnalyticalFrequencyEditorVM vm = (AnalyticalFrequencyEditorVM)editorVM;
            AnalyticalFrequencyElement element = (AnalyticalFrequencyElement)elem;

            vm.Name = element.Name;
            vm.Description = element.Description;
            vm.Distribution = element.Distribution;

        }

        public ChildElement CreateElementFromEditor(Editors.BaseEditorVM editorVM)
        {
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new AnalyticalFrequencyElement(editorVM.Name, editDate, editorVM.Description, ((AnalyticalFrequencyEditorVM)editorVM).Distribution);
            //return null;
        }
        //public override void Save()
        //{
        //    throw new NotImplementedException();
        //}

        //public override object[] RowData()
        //{
        //    return new object[] { Name, LastEditDate, Description, Distribution.GetMean, Distribution.GetStDev, Distribution.GetG, Distribution.GetSampleSize };
        //}

        //public override bool SavesToRow()
        //{
        //    return true;
        //}
        //public override bool SavesToTable()
        //{
        //    return false;
        //}
        //#endregion
        //#region Functions
        //public override string TableName
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        #endregion

    }
}
