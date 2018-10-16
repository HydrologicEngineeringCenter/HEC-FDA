using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.AggregatedStageDamage
{
    public class AggregatedStageDamageOwnerElement : Utilities.ParentElement
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
        public AggregatedStageDamageOwnerElement(Utilities.ParentElement owner) : base(owner)
        {
            Name = "Aggregated Stage Damage Relationships";
            IsBold = false;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);

            Utilities.NamedAction addDamageCurve = new Utilities.NamedAction();
            addDamageCurve.Header = "Create New Aggregated Stage Damage Relationship";
            addDamageCurve.Action = AddNewDamageCurve;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(addDamageCurve);

            Actions = localActions;
        }
        #endregion
        #region Voids
        public void AddNewDamageCurve(object arg1, EventArgs arg2)
        {
            List<Inventory.DamageCategory.DamageCategoryOwnedElement> damcateleements = GetElementsOfType<Inventory.DamageCategory.DamageCategoryOwnedElement>();
            
            //AggregatedStageDamageEditorVM vm = new AggregatedStageDamageEditorVM((foo) => SaveNewElement(foo), (bar) => AddOwnerRules(bar));
            //Navigate(vm, true, true);
            //if (!vm.WasCancled)
            //{
            //    if (!vm.HasError)
            //    {
            //        string creationDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM

            //        AggregatedStageDamageElement ele = new AggregatedStageDamageElement( vm.Name, creationDate, vm.Description, vm.Curve, CreationMethodEnum.UserDefined,this);
            //        AddElement(ele);
            //    }
            //}                   
        }
        public override void AddBaseElements()
        {
            
        }
        public override void AddValidationRules()
        {
        }
        #endregion
        #region Functions
        public override string TableName
        {
            get
            {
                return "Aggregated Stage Damage Relationships";
            }
        }
        
        public override string[] TableColumnNames()
        {
            return new string[] { "Name","Last Edit Date","Description", "Curve Uncertainty Type", "Creation Method" };
        }
        public override Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string), typeof(string), typeof(string), typeof(string),typeof(string) };
        }

        public override ChildElement CreateElementFromEditor(Editors.BaseEditorVM editorVM)
        {
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            //return new AggregatedStageDamageElement(editorVM.Name, editDate, editorVM.Description, editorVM.Curve, CreationMethodEnum.UserDefined, this);
            return null;
        }
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            Statistics.UncertainCurveDataCollection emptyCurve = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[3]));
            AggregatedStageDamageElement asd = new AggregatedStageDamageElement((string)rowData[0],(string)rowData[1], (string)rowData[2], emptyCurve, (CreationMethodEnum)Enum.Parse(typeof(CreationMethodEnum), (string)rowData[4]),this);
            asd.Curve.fromSqliteTable(asd.TableName);
            return asd;
        }

        public override void AddElement(object[] rowData)
        { 
            AddElement(CreateElementFromRowData(rowData),false);
        }
        #endregion
    }
}
