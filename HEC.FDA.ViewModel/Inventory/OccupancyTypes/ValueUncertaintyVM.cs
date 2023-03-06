using HEC.FDA.ViewModel.Utilities;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public abstract class ValueUncertaintyVM : BaseViewModel
    {
        public event EventHandler WasModified;

        #region fields
        private IValueUncertainty _CurrentVM;
        private DeterministicControlVM _DeterministicControlVM;
        private NormalControlVM _NormalControlVM;
        private TriangularControlVM _TriangularControlVM;
        private UniformControlVM _UniformControlVM;
        private LogNormalControlVM _LogNormalControlVM;

        private IDistributionEnum _SelectedType;
        #endregion

        #region properties

        public DeterministicControlVM DeterministicControlVM
        {
            get { return _DeterministicControlVM; }
            set { _DeterministicControlVM = value; }
        }
        public TriangularControlVM TriangularControlVM
        {
            get { return _TriangularControlVM; }
            set { _TriangularControlVM = value; }
        }
        public UniformControlVM UniformControlVM
        {
            get { return _UniformControlVM; }
            set { _UniformControlVM = value; }
        }
        public NormalControlVM NormalControlVM
        {
            get { return _NormalControlVM; }
            set { _NormalControlVM = value; }
        }
        public LogNormalControlVM LogNormalControlVM
        {
            get { return _LogNormalControlVM; }
            set { _LogNormalControlVM = value; }
        }
        public IDistributionEnum SelectedType
        {
            get { return _SelectedType; }
            set { _SelectedType = value; SelectedDistributionTypeChanged(value); NotifyPropertyChanged(); }
        }

        public IValueUncertainty CurrentVM
        {
            get { return _CurrentVM; }
            set { _CurrentVM = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<IDistributionEnum> UncertaintyTypes
        {
            get;
            set;
        }

        public ContinuousDistribution Distribution
        {
            get
            {
                return CreateOrdinate();
            }
        }
        
        #endregion

        #region constructors
        public ValueUncertaintyVM(ContinuousDistribution valueUncertaintyOrdinate)
        {
            //create the options for the combobox
            UncertaintyTypes = new ObservableCollection<IDistributionEnum>()
            {
                IDistributionEnum.Deterministic,
                IDistributionEnum.Normal,
                IDistributionEnum.Triangular,
                IDistributionEnum.Uniform
            };
            LoadControlVMs(valueUncertaintyOrdinate);
            //set the current vm to be of the selected type
            SelectedDistributionTypeChanged(valueUncertaintyOrdinate.Type);
        }

        #endregion

        public abstract XElement ToXML();
  

        public ContinuousDistribution CreateOrdinate()
        {
            //the currentVM can equal null. That is the non ratio deterministic case
            if (CurrentVM == null)
            {
                return new Deterministic(0);
            }
            else
            {
                return CurrentVM.CreateOrdinate();
            }
        }

        public abstract void LoadControlVMs(IDistribution ordinate);

        /// <summary>
        /// The selected type of ordinate has changed. This method will convert the current ordinate
        /// into the new type using whatever values it can for the new one.
        /// </summary>
        /// <param name="newValue"></param>
        public void SelectionChanged(Object newValue)
        {
            if (newValue is IDistributionEnum)
            {
                IDistributionEnum newType = (IDistributionEnum)newValue;
                SelectedDistributionTypeChanged(newType);
                WasModified(this, new EventArgs());
            }
        }

        /// <summary>
        /// Used to raise an event that will put the "*" on the occtype and prompt the save message when closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ControlWasModified(object sender, EventArgs e)
        {
            WasModified?.Invoke(this, new EventArgs());
        }

        public void SelectedDistributionTypeChanged(IDistributionEnum distType)
        {
            _SelectedType = distType;
            NotifyPropertyChanged(nameof(SelectedType));
            switch (distType)
            {
                case IDistributionEnum.Deterministic:
                    {
                        CurrentVM = _DeterministicControlVM;
                        break;
                    }
                case IDistributionEnum.Normal:
                    {

                        CurrentVM = _NormalControlVM;
                        break;
                    }
                case IDistributionEnum.Triangular:
                    {
                        CurrentVM = _TriangularControlVM;
                        break;
                    }
                case IDistributionEnum.Uniform:
                    {
                        CurrentVM = _UniformControlVM;
                        break;
                    }
                default:
                    CurrentVM = null;
                    break;
            }
            WasModified?.Invoke(this, new EventArgs());
        }
    
    }
}
