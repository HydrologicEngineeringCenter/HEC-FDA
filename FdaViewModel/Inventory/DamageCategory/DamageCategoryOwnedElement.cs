using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory.DamageCategory
{
    class DamageCategoryOwnedElement : Utilities.ChildElement
    {
        #region Notes
        #endregion
        #region Fields
        private List<DamageCategoryRowItem> _DamageCategories;
        #endregion
        #region Properties
       
        public List<DamageCategoryRowItem> DamageCategories
        {
            get { return _DamageCategories; }
            set { _DamageCategories = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public DamageCategoryOwnedElement(BaseFdaElement owner) : base(owner)
        {
            Name = "Damage Categories";
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);


            _DamageCategories = new List<DamageCategoryRowItem>();

            Utilities.NamedAction editDamageCategory = new Utilities.NamedAction();
            editDamageCategory.Header = "Edit Damage Categories";
            editDamageCategory.Action = EditDamageCategories;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(editDamageCategory);

            Actions = localActions;
        }
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            return null;
        }
        private void EditDamageCategories(object arg1, EventArgs arg2)
        {
            //throw new NotImplementedException();
            DamageCategoriesVM vm = null;
            if (_DamageCategories.Count > 0)
            {
                System.Collections.ObjectModel.ObservableCollection<object> objs = new System.Collections.ObjectModel.ObservableCollection<object>();
                foreach (DamageCategoryRowItem r in _DamageCategories)
                {
                    objs.Add(r);
                }
                vm = new DamageCategoriesVM(objs);
            }
            else
            {
                vm = new DamageCategoriesVM();
            }
            Navigate(vm,true,true,"Damage Category Editor");
            if (!vm.WasCanceled)
            {
                if (vm.HasChanges)
                {
                    System.Collections.ObjectModel.ObservableCollection<object> output = vm.DamageCategories;
                    _DamageCategories.Clear();
                    foreach (object r in output)
                    {
                        DamageCategoryRowItem ri = r as DamageCategoryRowItem;
                        if (ri != null)
                        {
                            _DamageCategories.Add(ri);
                        }
                    }
                }
            }

        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
        //public override string TableName
        //{
        //    get
        //    {
        //        return "DamageCategories";
        //    }
        //}

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        //public override object[] RowData()
        //{
        //    return new object[] { Name };
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
