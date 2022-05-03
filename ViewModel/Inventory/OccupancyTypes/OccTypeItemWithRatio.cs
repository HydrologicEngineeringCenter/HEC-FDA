using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using Statistics;
using System;
using ViewModel.Inventory.OccupancyTypes;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    /// <summary>
    /// This class extends OccTypeItem. The "Content" and "Other" have an extra boolean option
    /// where the curve can be by value or by a ratio of the structure curve.
    /// </summary>
    public class OccTypeItemWithRatio : OccTypeAsset
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
                UpdateCurrentValueUncertaintyVM(); 
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

        public OccTypeItemWithRatio(OcctypeAssetType itemType, bool isChecked, ComputeComponentVM curve, ContinuousDistribution valueUncertainty,
            ContinuousDistribution ratioUncertainty, bool isByValue) : base(itemType, isChecked, curve, valueUncertainty)
        {
            _ContentByRatioVM = new OtherValueUncertaintyVM(ratioUncertainty);
            _ContentByRatioVM.WasModified += SomethingChanged;
            IsByValue = isByValue;
        }

        private void UpdateCurrentValueUncertaintyVM()
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

        public override FdaValidationResult IsItemValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            vr.AddErrorMessage(base.IsItemValid().ErrorMessage);
            FdaValidationResult valueUncertVR = ContentByRatioVM.IsValueUncertaintyValid();
            if (!valueUncertVR.IsValid)
            {
                string errorMessage = ItemType + " value uncertainty:\n" + valueUncertVR.ErrorMessage;
                vr.AddErrorMessage(errorMessage + Environment.NewLine);
            }
            return vr;
        }

    }
}
