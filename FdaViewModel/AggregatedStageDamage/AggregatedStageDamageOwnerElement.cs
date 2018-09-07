using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.AggregatedStageDamage
{
    public class AggregatedStageDamageOwnerElement : Utilities.OwnerElement
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
        public AggregatedStageDamageOwnerElement(BaseFdaElement owner) : base(owner)
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
            //Inventory.DamageCategory.DamageCategoryOwnedElement damcatelement = damcateleements.FirstOrDefault();
            //if (damcatelement.DamageCategories.Count > 0)
            
            AggregatedStageDamageEditorVM vm = new AggregatedStageDamageEditorVM();
            Navigate(vm, true, true);
            if (!vm.WasCancled)
            {
                if (!vm.HasError)
                {
                    AggregatedStageDamageElement ele = new AggregatedStageDamageElement(this, vm.Name, vm.Description, vm.Curve, CreationMethodEnum.UserDefined);
                    AddElement(ele);
                }
            }
            
            
            

            //throw new NotImplementedException();
        }
        public override void AddBaseElements()
        {
            
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
                return "Aggregated Stage Damage Relationships";
            }
        }
        
        public override string[] TableColumnNames()
        {
            return new string[] { "Aggregated Stage Damage Relationship","Description", "Curve Uncertainty Type", "Creation Method" };
        }
        public override Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string),typeof(string), typeof(string),typeof(string) };
        }

        public override void AddElement(object[] rowData)
        {
            //Inventory.DamageCategory.DamageCategoryOwnedElement dce = (Inventory.DamageCategory.DamageCategoryOwnedElement)GetElementOfTypeAndName(typeof(Inventory.DamageCategory.DamageCategoryOwnedElement), "Damage Categories");
            //Inventory.DamageCategory.DamageCategoryRowItem dcri = null;
            //foreach(Inventory.DamageCategory.DamageCategoryRowItem d in dce.DamageCategories)
            //{
            //    if (d.Name == (string)rowData[2])
            //    {
            //        dcri = d;
            //    }
            //}
            Statistics.UncertainCurveDataCollection emptyCurve = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[2]));
            AggregatedStageDamageElement asd = new AggregatedStageDamageElement(this, (string)rowData[0], (string)rowData[1], emptyCurve, (CreationMethodEnum)Enum.Parse(typeof(CreationMethodEnum),(string)rowData[3]));
            asd.Curve.fromSqliteTable(asd.TableName);
            AddElement(asd,false);
        }
        #endregion
    }
}
