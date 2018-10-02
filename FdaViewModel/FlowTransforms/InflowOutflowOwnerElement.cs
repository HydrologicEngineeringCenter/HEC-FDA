using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.FlowTransforms
{
    public class InflowOutflowOwnerElement : Utilities.OwnerElement
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
        public InflowOutflowOwnerElement(BaseFdaElement owner) : base(owner)
        {
            Name = "Inflow Outflow Relationships";
            IsBold = false;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);

            Utilities.NamedAction addInflowOutflow = new Utilities.NamedAction();
            addInflowOutflow.Header = "Create New Inflow Outflow Relationship";
            addInflowOutflow.Action = AddInflowOutflow;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(addInflowOutflow);

            Actions = localActions;
        }


        #endregion
        #region Voids
        private void ImportFromASCII(object arg1, EventArgs arg2)
        {
            throw new NotImplementedException();
        }

        public void AddInflowOutflow(object arg1, EventArgs arg2)
        {

            InflowOutflowEditorVM vm = new InflowOutflowEditorVM((foo)=> SaveNewElement(foo), (bar)=>AddOwnerRules(bar));
            
            Navigate(vm);
            if (!vm.WasCancled)
            {
                if (!vm.HasFatalError)
                {
                    
                    vm.SaveWhileEditing();
                    
                }
            }

        }
        public override void AddBaseElements()
        {
            //throw new NotImplementedException();
        }
        public override void AddValidationRules()
        {
            //AddRule(nameof(Name), () => Name != "test", "Name cannot be test.");

            //throw new NotImplementedException();
        }
        #endregion
        #region Functions
        public override string TableName
        {
            get
            {
                return "Inflow Outflow Relationships";
            }
        }

        public override string[] TableColumnNames()
        {
            return new string[] { "Name","Last Edit Date", "Description" , "Curve Distribution Type"};
        }
        public override Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string),typeof(string), typeof(string), typeof(string) };
        }
        public override OwnedElement CreateElementFromEditor(ISaveUndoRedo editorVM)
        {
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            return new InflowOutflowElement(editorVM.Name, editDate, ((InflowOutflowEditorVM)editorVM).Description, ((InflowOutflowEditorVM)editorVM).Curve, this);
        }
        public override OwnedElement CreateElementFromRowData(object[] rowData)
        {
            Statistics.UncertainCurveDataCollection ucdc = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
            InflowOutflowElement inout = new InflowOutflowElement((string)rowData[0], (string)rowData[1], (string)rowData[2], ucdc, this);
            inout.InflowOutflowCurve.fromSqliteTable(inout.TableName);
            return inout;
        }
        public override void AddElement(object[] rowData)
        {
            
            AddElement(CreateElementFromRowData(rowData),false);
        }
        #endregion
    }
}
