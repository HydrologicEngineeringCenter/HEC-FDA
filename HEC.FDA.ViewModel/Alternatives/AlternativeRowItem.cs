using HEC.FDA.ViewModel.ImpactAreaScenario;

namespace HEC.FDA.ViewModel.Alternatives
{
    public class AlternativeRowItem : BaseViewModel
    {
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
        public AlternativeRowItem(IASElement elem)
        {

            ID = elem.ID;
            Year = elem.AnalysisYear;
           
            //Name needs to be after the Year property is set to get the proper display name.
            Name = elem.Name ;
            IsSelected = false;
        }

    }
}
