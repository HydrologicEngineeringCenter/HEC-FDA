using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using FdaViewModel.Utilities;

namespace FdaViewModel.StageTransforms
{
    //[Author(q0heccdm, 6 / 8 / 2017 11:31:34 AM)]
    public class ExteriorInteriorElement : Utilities.ChildElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 11:31:34 AM
        #endregion
        #region Fields
        private const string _TableConstant = "Exterior Interior - ";

        #endregion
        #region Properties
        //public override string GetTableConstant()
        //{
        //    return _TableConstant;
        //}

        
        #endregion
        #region Constructors
        public ExteriorInteriorElement(string userProvidedName,string lastEditDate, string desc, Statistics.UncertainCurveDataCollection exteriorInteriorCurve):base()
        {
            LastEditDate = lastEditDate;
            Name = userProvidedName;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/ExteriorInteriorStage.png");

            Description = desc;
            if (Description == null) Description = "";
            Curve = exteriorInteriorCurve;

            Utilities.NamedAction editExteriorInteriorCurve = new Utilities.NamedAction();
            editExteriorInteriorCurve.Header = "Edit Exterior Interior Curve";
            editExteriorInteriorCurve.Action = EditExteriorInteriorCurve;

            Utilities.NamedAction removeExteriorInteriorCurve = new Utilities.NamedAction();
            removeExteriorInteriorCurve.Header = "Remove";
            removeExteriorInteriorCurve.Action = RemoveElement;

            Utilities.NamedAction renameElement = new Utilities.NamedAction(this);
            renameElement.Header = "Rename";
            renameElement.Action = Rename;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(editExteriorInteriorCurve);
            localActions.Add(removeExteriorInteriorCurve);
            localActions.Add(renameElement);

            Actions = localActions;
        }
        #endregion
        #region Voids
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            ExteriorInteriorElement elem = (ExteriorInteriorElement)elementToClone;
            return new ExteriorInteriorElement(elem.Name, elem.LastEditDate, elem.Description, elem.Curve);
        }
        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetExteriorInteriorManager().Remove(this);
        }
        public void EditExteriorInteriorCurve(object arg1, EventArgs arg2)
        {
          
            //create save helper
            Editors.SaveUndoRedoHelper saveHelper = new Editors.SaveUndoRedoHelper(Saving.PersistenceFactory.GetExteriorInteriorManager()
                ,this, (editorVM) => CreateElementFromEditor(editorVM), (editor, element) => AssignValuesFromElementToCurveEditor(editor, element),
                (editor, element) => AssignValuesFromCurveEditorToElement(editor, element));
            //create action manager
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                //.WithOwnerValidationRules((editorVM, oldName) => AddOwnerRules(editorVM, oldName))
                .WithSaveUndoRedo(saveHelper);

            Editors.CurveEditorVM vm = new Editors.CurveEditorVM(this, actionManager);
            //StudyCache.AddSiblingRules(vm, this);

            Navigate(vm, false, true, "Create Exterior Interior");
            
        }
        #endregion
        #region Functions
        public ChildElement CreateElementFromEditor(Editors.BaseEditorVM vm)
        {
            Editors.CurveEditorVM editorVM = (Editors.CurveEditorVM)vm;

            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new ExteriorInteriorElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve);
            //return null;
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
            //throw new NotImplementedException();
        }

        //public override void Save()
        //{
        //    _Curve.toSqliteTable(TableName);
        //}

        //public override object[] RowData()
        //{
        //    return new object[] { Name, LastEditDate, Description, ExteriorInteriorCurve.Distribution };
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
