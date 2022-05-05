using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.ViewModel.Validation;

namespace HEC.FDA.ViewModel.Editors
{
    public class NameValidatingVM : BaseViewModel
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }

        public override void AddValidationRules()
        {
            AddSinglePropertyRule(nameof(Name), new Rule(() =>{return !string.IsNullOrWhiteSpace(Name);}, "Name cannot be blank or whitespace.", ErrorLevel.Severe));
        }

    }
}
