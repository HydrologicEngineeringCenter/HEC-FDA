using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using FdaViewModel.Utilities;

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
        public InflowOutflowElement(string userProvidedName, string lastEditDate, string description, Statistics.UncertainCurveDataCollection inflowOutflowCurve ):base()
        {
            LastEditDate = lastEditDate;
            Name = userProvidedName;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/InflowOutflowCircle.png");
            
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

            FdaModel.Utilities.Messager.ErrorMessage err = new FdaModel.Utilities.Messager.ErrorMessage("Test message when opening", FdaModel.Utilities.Messager.ErrorMessageEnum.Report, nameof(InflowOutflowElement));
            FdaModel.Utilities.Messager.Logger.Instance.ReportMessage(err);

           
            //create save helper
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetInflowOutflowManager()
                ,this, (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToCurveEditor(editor, element),
                (editor, element) => AssignValuesFromCurveEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                //.WithOwnerValidationRules((editorVM, oldName) => AddOwnerRules(editorVM, oldName))
                .WithSaveUndoRedo(saveHelper);

            Editors.CurveEditorVM vm = new Editors.CurveEditorVM(this, actionManager);
            //StudyCache.AddSiblingRules(vm, this);
            Navigate(vm, false, false, "Edit Inflow Outflow");

       
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

        //public override void Save()
        //{
        //    _Curve.toSqliteTable(TableName);
        //}
        //public override object[] RowData()
        //{
        //    return new object[] { Name, LastEditDate, Description, InflowOutflowCurve.Distribution };
        //}

        //public override bool SavesToRow()
        //{
        //    return true;
        //}

        //public override bool SavesToTable()
        //{
        //    return true;
        //}
    }
}
