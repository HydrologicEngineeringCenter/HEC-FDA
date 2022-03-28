using HEC.FDA.ViewModel.Utilities;

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
        #endregion
        #region Properties
        public bool IsChecked
        {
            get { return _IsChecked; }
            set { _IsChecked = value; NotifyPropertyChanged(); }
        }
        public bool IsEnabled { get; }
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
        public WaterSurfaceElevationRowItemVM(bool isChecked,string name, string path, double probability, bool isEnabled)
        {
            IsEnabled = isEnabled;
            Path = path;
            ReturnYear = 1 / probability;
            IsChecked = isChecked;
            Name = name;
            Probability = probability;
        }
        #endregion

        public FdaValidationResult IsValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (Probability <= 0 || Probability >= 1)
            {
                vr.AddErrorMessage("Probability value in row '" + Name + "' has to be between 0 and 1.");
            }
            return vr;
        }
    }
}
