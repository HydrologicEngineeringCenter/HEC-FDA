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
            AddSinglePropertyRule(nameof(Name), new Rule(() =>{return Name != "";}, "Name cannot be blank.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Name), new Rule(() => { return Name != null; }, "Name cannot be blank.", ErrorLevel.Severe));
        }

    }
}
