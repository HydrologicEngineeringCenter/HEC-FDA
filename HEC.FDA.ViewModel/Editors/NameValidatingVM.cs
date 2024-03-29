﻿namespace HEC.FDA.ViewModel.Editors
{
    public class NameValidatingVM : BaseViewModel
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value.Trim(); NotifyPropertyChanged(); }
        }

        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => !string.IsNullOrWhiteSpace(Name), "Name cannot be blank or whitespace.");
        }

    }
}
