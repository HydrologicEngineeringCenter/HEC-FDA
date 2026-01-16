using CommunityToolkit.Mvvm.ComponentModel;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Hydraulics
{
    public partial class WaterSurfaceElevationRowItemVM : ObservableObject
    {
        [ObservableProperty]
        [property: DisplayAsColumn("Name")]
        private string _name;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ReturnYear))]
        [property: DisplayAsColumn("Annual Exceedence Probability")]
        private double _probability;

        /// <summary>
        /// full, absolute path to the file
        /// </summary>
        [ObservableProperty]
        private string _path;
        public bool IsEnabled { get; }

        [DisplayAsColumn("Return Interval")]
        public double ReturnYear
        {
            get => Probability > 0 ? 1 / Probability : 0;
            set
            {
                if (value > 0)
                {
                    Probability = 1 / value;
                }
            }
        }

        /// <param name="name"> The name visible to the UI</param>
        /// <param name="path"> The absolute path to the file </param>
        public WaterSurfaceElevationRowItemVM(string name, string path, double probability, bool isEnabled)
        {
            IsEnabled = isEnabled;
            _path = path;
            _name = name;
            _probability = probability;
        }

        public FdaValidationResult IsValid()
        {
            FdaValidationResult vr = new();
            if (Probability <= 0 || Probability >= 1)
            {
                vr.AddErrorMessage("Probability value in row '" + Name + "' has to be between 0 and 1.");
            }
            return vr;
        }
    }
}
