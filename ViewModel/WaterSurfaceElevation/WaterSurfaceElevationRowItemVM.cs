namespace HEC.FDA.ViewModel.WaterSurfaceElevation
{
    //[Author(q0heccdm, 9 / 1 / 2017 8:32:06 AM)]
    public class WaterSurfaceElevationRowItemVM:BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 9/1/2017 8:32:06 AM
        #endregion
        #region Fields
        private double _Probability;
        private double _ReturnYear;
        private bool _IsChecked;
        private bool _IsVRT;
        #endregion
        #region Properties
        public bool IsChecked
        {
            get { return _IsChecked; }
            set { _IsChecked = value; NotifyPropertyChanged(); }
        }
        public bool IsVRT
        {
            get { return _IsVRT; }
            set { _IsVRT = value; NotifyPropertyChanged(); }
        }
        public bool IsFLT { get; set; }
        public bool IsTIF { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public double Probability
        {
            get { return _Probability; }
            set { _Probability = value; _ReturnYear = 1 / _Probability; NotifyPropertyChanged();NotifyPropertyChanged("ReturnYear"); }
        }
        public double ReturnYear
        {
            get { return _ReturnYear; }
            set { _ReturnYear = value; _Probability = 1 / value; NotifyPropertyChanged();NotifyPropertyChanged("Probability"); }
        }
        #endregion
        #region Constructors
        public WaterSurfaceElevationRowItemVM(bool isChecked,string name, string path, double probability)
        {
            Path = path;
            ReturnYear = 1 / probability;
            IsChecked = isChecked;
            Name = name;
            Probability = probability;
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

     
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
    }
}
