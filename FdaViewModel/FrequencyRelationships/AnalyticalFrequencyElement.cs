using FdaViewModel.Editors;
using FdaViewModel.Utilities;
using Model;
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

        private IFdaFunction _Distribution;
        #endregion
        #region Properties
      
        public IFdaFunction Distribution
        {
            get { return _Distribution; }
            set { _Distribution = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public AnalyticalFrequencyElement(string name, string lastEditDate, string desc, IFdaFunction dist) : base()
        {
            LastEditDate = lastEditDate;
            Name = name;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/FrequencyCurve.png");

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
            Saving.PersistenceFactory.GetFlowFrequencyManager().Remove(this);
        }
        public void EditFlowFreq(object arg1, EventArgs arg2)
        {

            //create save helper
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetFlowFrequencyManager()
                ,this, (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToEditor(editor, element),
                (editor, element) => AssignValuesFromEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveUndoRedo(saveHelper)
                .WithSiblingRules(this);
               //.WithParentGuid(this.GUID)
               //.WithCanOpenMultipleTimes(false);

            AnalyticalFrequencyEditorVM vm = new AnalyticalFrequencyEditorVM(this,"Frequency", "Flow","Analytical Frequency", actionManager);// Name, Distribution, Description, _Owner);
            string header = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditAnalyticalFrequency" + vm.Name);
            Navigate(tab, false, false);
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
            element.Distribution = vm.Curve;
            element.UpdateTreeViewHeader(vm.Name);
        }

        public void AssignValuesFromElementToEditor(BaseEditorVM editorVM, ChildElement elem)
        {
            AnalyticalFrequencyEditorVM vm = (AnalyticalFrequencyEditorVM)editorVM;
            AnalyticalFrequencyElement element = (AnalyticalFrequencyElement)elem;

            vm.Name = element.Name;
            vm.Description = element.Description;
            vm.Curve = element.Distribution;

        }

        public ChildElement CreateElementFromEditor(Editors.BaseEditorVM editorVM)
        {
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new AnalyticalFrequencyElement(editorVM.Name, editDate, editorVM.Description, ((AnalyticalFrequencyEditorVM)editorVM).Curve);
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

        public override bool Equals(object obj)
        {
            bool retval = true;
            if (obj.GetType() == typeof(AnalyticalFrequencyElement))
            {
                AnalyticalFrequencyElement elem = (AnalyticalFrequencyElement)obj;
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
                //todo: Refactor: Commented Out
                //if (!Double.Equals(Distribution.GetMean, elem.Distribution.GetMean))
                //{
                //    retval = false;
                //}
                //if (!Double.Equals(Distribution.GetStDev, elem.Distribution.GetStDev))
                //{
                //    retval = false;
                //}
                //if (!Double.Equals(Distribution.GetG, elem.Distribution.GetG))
                //{
                //    retval = false;
                //}
            }
            else
            {
                retval = false;
            }
            return retval;
        }

        #endregion

    }
}
