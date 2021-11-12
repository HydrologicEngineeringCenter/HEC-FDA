using ViewModel.ImpactAreaScenario;

namespace ViewModel.Alternatives
{
    public class AlternativeRowItem : BaseViewModel
    {
        private bool _HasComputed;
        private bool _IsSelected;
        private string _Name;
        private int _Year;
        private string _Tooltip;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; NotifyPropertyChanged(); }
        }
        public string Name
        {
            get { return _Name + " (" + Year + ")"; }
            set { _Name = value; NotifyPropertyChanged(); }
        }
        public bool HasComputed
        {
            get { return _HasComputed; }
            set { _HasComputed = value; UpdateTooltip(); NotifyPropertyChanged(); }
        }
        public int Year
        {
            get { return _Year; }
            set { _Year = value; NotifyPropertyChanged(); }
        }
        public int ID { get; set; }
        public string Tooltip
        {
            get { return _Tooltip; }
            set { _Tooltip = value; NotifyPropertyChanged(); }
        }
        public AlternativeRowItem(IASElementSet elem)
        {
            ID = elem.GetElementID();
            Year = elem.AnalysisYear;
            HasComputed = elem.HasComputed;
            //Name needs to be after the Year property is set to get the proper display name.
            Name = elem.Name ;
            IsSelected = false;
            UpdateTooltip();

        }

        private void UpdateTooltip()
        {
            if (HasComputed)
            {
                Tooltip = Name;
            }
            else
            {
                Tooltip = "A compute is required to include in an alternative.";
            }
        }
    }
}
