using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory.DamageCategory
{
    public class DamageCategoryRowItem : Consequences_Assist.DataGridRowItem
    {
        #region Notes
        #endregion
        #region Fields
        private string _Name;
        private double _IndexFactor;
        #endregion
        #region Properties
        public string Name
        {
            get { return _Name; }
            set { _Name = value;  NotifyPropertyChanged(); }
        }
        public double IndexFactor
        {
            get { return _IndexFactor; }
            set { _IndexFactor = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public DamageCategoryRowItem(ObservableCollection<object> list) : base(list)
        {
        }
        public DamageCategoryRowItem(string name, double indexfactor, ObservableCollection<object> list) : base(list)
        {
            this.Name = name;
            this.IndexFactor = indexfactor;
        }
        #endregion
        #region Voids
        public override void AddValidationRules()
        {
            //name cannot be blank
            AddRule(nameof(Name),() => UniqueRule(nameof(Name), "Name Must be Unique."), "Name Must be Unique.");
            AddRule(nameof(Name), () => Name == null, "Name Must not be null.");
            AddRule(nameof(Name), () => Name == "", "Name Must not be blank.");
            //AddRule(nameof(Name), () => Name == "", "Name Must not be blank.");
            //name must be unique
            //factor must be greater than zero and less than 1?
        }
        #endregion
        #region Functions
        public override bool IsGridDisplayable(string propertyName)
        {
            return true;
        }

        public override string PropertyDisplayName(string propertyName)
        {
            if (propertyName.Equals(nameof(IndexFactor))) return "Index Factor";
            return propertyName;
        }
        #endregion





    }
}
