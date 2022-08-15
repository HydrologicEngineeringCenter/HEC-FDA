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
        private NormalControlVM _NormalControlVM;
        private TriangularControlVM _TriangularControlVM;
        private UniformControlVM _UniformControlVM;
        private LogNormalControlVM _LogNormalControlVM;

        private IDistributionEnum _SelectedType;
        #endregion

        #region properties
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

        public ValueUncertaintyVM(XElement uncertElem)
        {
            //read the base xml data
            ContinuousDistribution cd = new Normal();
        }

        #endregion

        public abstract XElement ToXML();

        public ContinuousDistribution CreateOrdinate()
        {
            //the currentVM can equal null. That is the deterministic case
            if (CurrentVM == null)
            {
                return new Deterministic(0);
            }
            else
            {
                return CurrentVM.CreateOrdinate();
            }
        }

        public FdaValidationResult IsValueUncertaintyValid()
        {
            //the currentVM can equal null. That is the deterministic case
            if (CurrentVM == null)
            {
                //if it is deterministic then it is valid.
                return new FdaValidationResult();
            }
            else
            {
                return CurrentVM.IsValid();
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
                        CurrentVM = null;
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
