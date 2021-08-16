using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Inventory.DamageCategory
{
    public class DamageCategoriesVM : BaseViewModel
    {
        #region Notes
        #endregion
        #region Fields
        private System.Collections.ObjectModel.ObservableCollection<object> _DamageCategoryItems;
        private string _Description;
        #endregion
        #region Properties
        public System.Collections.ObjectModel.ObservableCollection<object> DamageCategories
        {
            get { return _DamageCategoryItems; }
            set { _DamageCategoryItems = value; NotifyPropertyChanged(); }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value;  NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public DamageCategoriesVM()
        {
            DamageCategories = new System.Collections.ObjectModel.ObservableCollection<object>();
        }
        public DamageCategoriesVM(System.Collections.ObjectModel.ObservableCollection<object> dcItems)
        {
            DamageCategories = dcItems;//shouldnt i copy?
            //addhandler for rename
        }
        #endregion
        #region Voids
        //add
        public void AddDamageCategory(int insertAt = 0)
        {
            DamageCategoryRowItem dci =new DamageCategoryRowItem("New Damage Category", 1.0d, _DamageCategoryItems);
            //add handler for rename.
            dci.PropertyChanged += RowItemChanged;
            System.Reflection.PropertyInfo[] pi = dci.GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo info in pi)
            {
                if (dci.RuleMap.ContainsKey(info.Name))
                {
                    dci.RuleMap[info.Name].PropertyChanged += RulePropertyChanged;
                    
                }
            }
            if (_DamageCategoryItems.Count == 0)
            { _DamageCategoryItems.Add(dci); }
            else
            { _DamageCategoryItems.Insert(insertAt, dci); }
            
            DamageCategories = _DamageCategoryItems;
        }

        private void RulePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("HasError"))
            {
                NotifyPropertyChanged(nameof(DamageCategories));
            }
        }

        private void RowItemChanged(object sender, System.ComponentModel.PropertyChangedEventArgs propertyargs){
            if (propertyargs.PropertyName.Equals(nameof(DamageCategoryRowItem.Name)))
            {
                System.Diagnostics.Debug.Print("Damage Category Renamed!!!");
            }
        }
        //remove
        public void RemoveDamageCategory(object item)
        {
            _DamageCategoryItems.Remove(item);
            DamageCategories = _DamageCategoryItems;
        }
        //rename
        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            AddRule(nameof(DamageCategories), () => areDamageCategoriesInvalid(), "Damage Categories have validation issues.");
            //throw new NotImplementedException();
        }
        private bool areDamageCategoriesInvalid()
        {
            foreach(DamageCategoryRowItem item in _DamageCategoryItems)
            {
                System.Reflection.PropertyInfo[] pi = item.GetType().GetProperties();
                foreach(System.Reflection.PropertyInfo info in pi)
                {
                    if (item.RuleMap.ContainsKey(info.Name))
                    {
                        if (item.RuleMap[info.Name].HasError)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        
    }
}
