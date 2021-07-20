using FdaViewModel.Editors;
using FdaViewModel.Utilities;
using Model;
using Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.AggregatedStageDamage
{
    public class AggregatedStageDamageElement : Utilities.ChildElement
    {
        #region Notes
        #endregion
        #region Fields
        private const string _TableConstant = "Aggregated Stage Damage Function - ";
        private readonly CreationMethodEnum _Method;
        #endregion
        #region Properties
        public bool CanEdit { get; }
        public int SelectedWSE { get; set; }
        public int SelectedStructures { get; set; }
        public CreationMethodEnum Method
        {
            get { return _Method; }
        }

        public List<StageDamageCurve> Curves { get; }
        public bool IsManual { get; }

        #endregion
        #region Constructors

        public AggregatedStageDamageElement(String name, string lastEditDate, string description,int selectedWSE, int selectedStructs, List<StageDamageCurve> curves, bool isManual) : base()
        {
            LastEditDate = lastEditDate;
            CustomTreeViewHeader = new CustomHeaderVM(name, "pack://application:,,,/View;component/Resources/StageDamage.png");

            Description = description;
            if (Description == null)
            {
                Description = "";
            }

            Name = name;
            Curves = curves;
            IsManual = isManual;

            NamedAction editDamageCurve = new NamedAction();
            editDamageCurve.Header = "Edit Aggregated Stage Damage Relationship";
            editDamageCurve.Action = EditDamageCurve;

            NamedAction removeDamageCurve = new NamedAction();
            removeDamageCurve.Header = "Remove";
            removeDamageCurve.Action = RemoveElement;

            NamedAction renameDamageCurve = new NamedAction(this);
            renameDamageCurve.Header = "Rename";
            renameDamageCurve.Action = Rename;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(editDamageCurve);
            localActions.Add(removeDamageCurve);
            localActions.Add(renameDamageCurve);

            Actions = localActions;
        }

       

        /// <summary>
        /// Stage damage element
        /// </summary>
        /// <param name="name">Name of element</param>
        /// <param name="lastEditDate">Last edit date</param>
        /// <param name="description">Element description</param>
        /// <param name="curve">The curve that represents the stage vs damage for the element</param>
        /// <param name="method">Creation method</param>
        public AggregatedStageDamageElement( string name , string lastEditDate, string description, IFdaFunction curve, CreationMethodEnum method) : base()
        {
            LastEditDate = lastEditDate;
            Name = name;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/StageDamage.png");

            Description = description;
            if(Description == null)
            {
                Description = "";
            }
            Curve = curve;
            _Method = method;
            //add named actions like edit.
            Utilities.NamedAction editDamageCurve = new Utilities.NamedAction();
            editDamageCurve.Header = "Edit Aggregated Stage Damage Relationship";
            editDamageCurve.Action = EditDamageCurve;

            Utilities.NamedAction removeDamageCurve = new Utilities.NamedAction();
            removeDamageCurve.Header = "Remove";
            removeDamageCurve.Action = RemoveElement;

            Utilities.NamedAction renameDamageCurve = new Utilities.NamedAction(this);
            renameDamageCurve.Header = "Rename";
            renameDamageCurve.Action = Rename;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(editDamageCurve);
            localActions.Add(removeDamageCurve);
            localActions.Add(renameDamageCurve);


            Actions = localActions;
        }
        #endregion
        #region Voids
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            AggregatedStageDamageElement elem = (AggregatedStageDamageElement)elementToClone;
            return new AggregatedStageDamageElement(elem.Name, elem.LastEditDate, elem.Description, elem.Curve, elem.Method);
        }
        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetStageDamageManager().Remove(this);
        }
        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
            AddRule(nameof(Name), () => Name != null, "Name cannot be blank.");
        }
        public void EditDamageCurve(object arg1, EventArgs arg2)
        {
            //create save helper
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetStageDamageManager(), this, 
                (editorVM) => CreateElementFromEditor(editorVM), 
                (editor, element) => AssignValuesFromElementToCurveEditor(editor, element),
                (editor, element) => AssignValuesFromCurveEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSaveUndoRedo(saveHelper)
                 .WithSiblingRules(this);

            AggregatedStageDamageEditorVM vm = new AggregatedStageDamageEditorVM(this, actionManager);
            vm.AddSiblingRules( this);

            string title = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(title, vm, "EditStageDamageElement" + Name);
            Navigate(tab, false, true);
        }

        public void AssignValuesFromElementToCurveEditor(BaseEditorVM editorVM, ChildElement element)
        {
            AggregatedStageDamageEditorVM vm = (AggregatedStageDamageEditorVM)editorVM;
            AggregatedStageDamageElement elem = (AggregatedStageDamageElement)element;

        }
        
        public ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            Editors.CurveEditorVM editorVM = (Editors.CurveEditorVM)vm;

            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new AggregatedStageDamageElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve, CreationMethodEnum.UserDefined);
        }
        #endregion


        public override bool Equals(object obj)
        {
            bool retval = true;
            if (obj.GetType() == typeof(AggregatedStageDamageElement))
            {
                AggregatedStageDamageElement elem = (AggregatedStageDamageElement)obj;
                if (!Name.Equals(elem.Name))
                {
                    retval = false;
                }
                if (Description == null && elem.Description != null)
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
                if (Method != elem.Method)
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
