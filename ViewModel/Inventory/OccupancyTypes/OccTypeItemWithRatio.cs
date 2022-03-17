using HEC.FDA.ViewModel.TableWithPlot;
using Statistics;
using System;
using ViewModel.Inventory.OccupancyTypes;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class OccTypeItemWithRatio : OccTypeItem
    {
        private bool _IsByValue;
        private bool _IsNotByValue;

        private ValueUncertaintyVM _CurrentContentValueVM;
        private ValueUncertaintyVM _ContentByRatioVM;

        public ValueUncertaintyVM CurrentValueVM
        {
            get { return _CurrentContentValueVM; }
            set { _CurrentContentValueVM = value; SomethingChanged(this, EventArgs.Empty); NotifyPropertyChanged(); }
        }

        public ValueUncertaintyVM ContentByRatioVM
        {
            get { return _ContentByRatioVM; }
        }

        public bool IsByValue
        {
            get { return _IsByValue; }
            set 
            { 
                _IsByValue = value; 
                IsNotByValue = !value; 
                UpdateContentValueUncertaintyVM(); 
                SomethingChanged(this, EventArgs.Empty); 
                NotifyPropertyChanged(); 
            }
        }
        public bool IsNotByValue
        {
            get { return _IsNotByValue; }
            set { _IsNotByValue = value; NotifyPropertyChanged(); }
        }

        public OccTypeItemWithRatio(OccTypeItemWithRatio item):base(item)
        {
            _ContentByRatioVM = new OtherValueUncertaintyVM(item.ContentByRatioVM.Distribution);
            _ContentByRatioVM.WasModified += SomethingChanged;
            IsByValue = item.IsByValue;
        }

        public OccTypeItemWithRatio(bool isChecked, ComputeComponentVM curve, ContinuousDistribution valueUncertainty,
            ContinuousDistribution ratioUncertainty, bool isByValue) : base(isChecked, curve, valueUncertainty)
        {
            _ContentByRatioVM = new OtherValueUncertaintyVM(ratioUncertainty);
            _ContentByRatioVM.WasModified += SomethingChanged;
            IsByValue = isByValue;
        }

        private void UpdateContentValueUncertaintyVM()
        {
            if (IsByValue)
            {
                CurrentValueVM = ValueUncertainty;
            }
            else
            {
                CurrentValueVM = _ContentByRatioVM;
            }
        }

    }
}
