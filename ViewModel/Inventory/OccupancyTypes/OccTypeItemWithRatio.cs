using FdaLogging;
using HEC.FDA.ViewModel.TableWithPlot;
using Statistics;
using System;
using System.Collections.Generic;
using ViewModel.Inventory.OccupancyTypes;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    /// <summary>
    /// This class extends OccTypeItem. The "Content" and "Other" have an extra boolean option
    /// where the curve can be by value or by a ratio of the structure curve.
    /// </summary>
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

        public OccTypeItemWithRatio(OcctypeItemType itemType, bool isChecked, ComputeComponentVM curve, ContinuousDistribution valueUncertainty,
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

        public List<LogItem> IsItemValid()
        {
            List<LogItem> constructionErrors = new List<LogItem>();
            constructionErrors.AddRange(base.IsItemValid());
            constructionErrors.AddRange(IsValueUncertaintyConstructable(ContentByRatioVM, ItemType + " value uncertainty"));
            return constructionErrors;
        }

    }
}
