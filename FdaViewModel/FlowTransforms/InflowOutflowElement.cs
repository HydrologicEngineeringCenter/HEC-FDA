using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Utilities;
using Model;
using Statistics;

namespace FdaViewModel.FlowTransforms
{
    //[Author(q0heccdm, 6 / 8 / 2017 10:33:22 AM)]
    public class InflowOutflowElement : Utilities.ChildElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 10:33:22 AM
        #endregion
        #region Fields
        private const string _TableConstant = "Inflow Outflow - ";
   
        #endregion
        #region Properties
        //public override string GetTableConstant()
        //{
        //    return _TableConstant;
        //}
        //public string Description { get { return _Description; } set { _Description = value; NotifyPropertyChanged(); } }

      

        

        #endregion
        #region Constructors
        public InflowOutflowElement(string userProvidedName, string lastEditDate, string description, IFdaFunction inflowOutflowCurve ):base()
        {
            LastEditDate = lastEditDate;
            Name = userProvidedName;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/InflowOutflowCircle.png");
            
            Description = description;
            if (Description == null) Description = "";
            Curve = inflowOutflowCurve;

            Utilities.NamedAction editInflowOutflowCurve = new Utilities.NamedAction();
            editInflowOutflowCurve.Header = "Edit Inflow-Outflow Curve";
            editInflowOutflowCurve.Action = EditInflowOutflowCurve;

            Utilities.NamedAction removeInflowOutflowCurve = new Utilities.NamedAction();
            removeInflowOutflowCurve.Header = "Remove";
            removeInflowOutflowCurve.Action = RemoveElement;

            Utilities.NamedAction renameInflowOutflowCurve = new Utilities.NamedAction(this);
            renameInflowOutflowCurve.Header = "Rename";
            renameInflowOutflowCurve.Action = Rename;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(editInflowOutflowCurve);
            localActions.Add(removeInflowOutflowCurve);
            localActions.Add(renameInflowOutflowCurve);



            Actions = localActions;
        }
        #endregion
        #region Voids
        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetInflowOutflowManager().Remove(this);
        }
        public void EditInflowOutflowCurve(object arg1, EventArgs arg2)
        {
            AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(Name, Utilities.Transactions.TransactionEnum.EditExisting, 
                "Openning " + Name + " for editing.",nameof(InflowOutflowElement)));

            //FdaModel.Utilities.Messager.ErrorMessage err = new FdaModel.Utilities.Messager.ErrorMessage("Test message when opening", FdaModel.Utilities.Messager.ErrorMessageEnum.Report, nameof(InflowOutflowElement));
            //FdaModel.Utilities.Messager.Logger.Instance.ReportMessage(err);

           
            //create save helper
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetInflowOutflowManager()
                ,this, (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToCurveEditor(editor, element),
                (editor, element) => AssignValuesFromCurveEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                //.WithOwnerValidationRules((editorVM, oldName) => AddOwnerRules(editorVM, oldName))
                .WithSaveUndoRedo(saveHelper)
                .WithSiblingRules(this);
               //.WithParentGuid(this.GUID)
               //.WithCanOpenMultipleTimes(false);

            Editors.CurveEditorVM vm = new Editors.CurveEditorVM(this,  "Inflow", "Outflow", "Inflow - Outflow", actionManager);
            //vm.ParentGUID = this.GUID;
            //StudyCache.AddSiblingRules(vm, this);
            string header = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditInflowOutflow" + vm.Name);
            Navigate( tab, false, false);

       
        }

        #endregion
        #region Functions
        public ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            Editors.CurveEditorVM editorVM = (Editors.CurveEditorVM)vm;

            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new InflowOutflowElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve);
            // return null;
        }
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            InflowOutflowElement elem = (InflowOutflowElement)elementToClone;
            return new InflowOutflowElement(elem.Name, elem.LastEditDate,elem.Description,elem.Curve);
        }
        #endregion
        //public override string TableName
        //{
        //    get
        //    {
        //        return GetTableConstant() + LastEditDate;
        //    }
        //}

        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
            AddRule(nameof(Name), () => Name != null, "Name cannot be blank.");
            AddRule(nameof(Name), () => Name != "test", "Name cannot be test.", false);
        }

        public override bool Equals(object obj)
        {
            bool retval = true;
            if (obj.GetType() == typeof(InflowOutflowElement))
            {
                InflowOutflowElement elem = (InflowOutflowElement)obj;
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

        private bool areCurvesEqual(IFdaFunction curve2)
        {
            bool retval = true;
            
            //todo: Refactor: I commented out. I think the curves have an equals method
            //if (Curve.GetType() != curve2.GetType())
            //{
            //    return false;
            //}
            //if (Curve.Distribution != curve2.Distribution)
            //{
            //    return false;
            //}
            //if (Curve.XValues.Count != curve2.XValues.Count)
            //{
            //    return false;
            //}
            //if (Curve.YValues.Count != curve2.YValues.Count)
            //{
            //    return false;
            //}
            //double epsilon = .0001;
            //for (int i = 0; i < Curve.XValues.Count; i++)
            //{
            //    if (Math.Abs(Curve.get_X(i)) - Math.Abs(curve2.get_X(i)) > epsilon)
            //    {
            //        return false;
            //    }
            //    ContinuousDistribution y = Curve.get_Y(i);
            //    ContinuousDistribution y2 = curve2.get_Y(i);
            //    if (Math.Abs(y.GetCentralTendency) - Math.Abs(y2.GetCentralTendency) > epsilon)
            //    {
            //        return false;
            //    }
            //    if (Math.Abs(y.GetSampleSize) - Math.Abs(y2.GetSampleSize) > epsilon)
            //    {
            //        return false;
            //    }
            //}

            return retval;
        }
    }
}
