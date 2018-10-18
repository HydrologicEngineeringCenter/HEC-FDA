using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.StageTransforms
{
    public class ExteriorInteriorOwnerElement : Utilities.ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        public override string GetTableConstant()
        {
            return TableName;
        }
        #endregion
        #region Constructors
        public ExteriorInteriorOwnerElement(Utilities.ParentElement owner) : base(owner)
        {
            Name = "Exterior Interior Relationships";
            IsBold = false;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);

            Utilities.NamedAction addExteriorInterior = new Utilities.NamedAction();
            addExteriorInterior.Header = "Create New Exterior Interior Relationship";
            addExteriorInterior.Action = AddNewExteriorInteriorCurve;

            //Utilities.NamedAction ImportFromAscii = new Utilities.NamedAction();
            //ImportFromAscii.Header = "Import Exterior Interior Relationship From ASCII";
            //ImportFromAscii.Action = ImportFromASCII;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(addExteriorInterior);
            //localActions.Add(ImportFromAscii);

            Actions = localActions;

            StudyCache.ExteriorInteriorAdded += AddExteriorInteriorElement;
        }
        #endregion
        #region Voids
        private void AddExteriorInteriorElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void ImportFromASCII(object arg1, EventArgs arg2)
        {
            throw new NotImplementedException();
        }

        public void AddNewExteriorInteriorCurve(object arg1, EventArgs arg2)
        {
            //ExteriorInteriorEditorVM vm = new ExteriorInteriorEditorVM((foo) => SaveNewElement(foo), (bar) => AddOwnerRules(bar));
            //Navigate(vm);
            //if (!vm.WasCancled)
            //{
            //    if (!vm.HasError)
            //    {
            //        string creationDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM

            //        ExteriorInteriorElement ele = new ExteriorInteriorElement(vm.Name,creationDate, vm.Description, vm.Curve, this);
            //        AddElement(ele);
            //        AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(ele.Name, Utilities.Transactions.TransactionEnum.CreateNew, "", nameof(ExteriorInteriorElement)));
            //    }
            //}
        }
       
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }
        #endregion
        #region Functions
        public override string TableName
        {
            get
            {
                return "Interior Exterior Curves";
            }
        }
        public override string[] TableColumnNames()
        {
            return new string[] { "Name","Last Edit Date", "Description", "Curve Distribution Type" };
        }
        public override Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string),typeof(string), typeof(string), typeof(string) };
        }
        public override ChildElement CreateElementFromEditor(Editors.BaseEditorVM editorVM)
        {
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            //return new ExteriorInteriorElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve, this);
            return null;
        }
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            Statistics.UncertainCurveDataCollection ucdc = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
            ExteriorInteriorElement ele = new ExteriorInteriorElement((string)rowData[0],(string)rowData[1], (string)rowData[2], ucdc, this);
            ele.ExteriorInteriorCurve.fromSqliteTable(ele.TableName);
            return ele;
        }
        public override void AddElementFromRowData(object[] rowData)
        {
            
            AddElement(CreateElementFromRowData(rowData),false);
        }
        #endregion
    }
}
