﻿using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.Inventory.DamageCategory
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
            AddRule(nameof(Name),() => UniqueRule(nameof(Name), "Name Must be Unique."), "Name Must be Unique.");
            AddRule(nameof(Name), () => Name == null, "Name Must not be null.");
            AddRule(nameof(Name), () => Name == "", "Name Must not be blank.");
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