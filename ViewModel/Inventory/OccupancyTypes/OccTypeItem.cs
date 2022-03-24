using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using Statistics;
using System;
using System.Collections;
using ViewModel.Inventory.OccupancyTypes;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    /// <summary>
    /// The "Item" is either structure, content, or other.
    /// This class holds the value uncertainty, the "is selected", and the curve
    /// </summary>
    public class OccTypeItem:BaseViewModel
    {
        public event EventHandler DataModified;

        private bool _IsChecked;
        private ComputeComponentVM _Curve;
        private TableWithPlotVM _ItemTableWithPlot;
        private ValueUncertaintyVM _ItemValueUncertainty;
 
        /// <summary>
        /// This enum is only being used so that i can use it as a string name in any error messages.
        /// </summary>
        public enum OcctypeItemType
        {
            structure,
            content,
            vehicle,
            other
        }
        public OcctypeItemType ItemType
        {
            get;
        }

        public bool IsChecked
        {
            get { return _IsChecked; }
            set { _IsChecked = value; SomethingChanged(this, EventArgs.Empty); NotifyPropertyChanged(); }
        }
        public ComputeComponentVM Curve
        {
            get { return _Curve; }
            set { _Curve = value; NotifyPropertyChanged(); }
        }

        public TableWithPlotVM TableWithPlot
        {
            get { return _ItemTableWithPlot; }
            set { _ItemTableWithPlot = value; NotifyPropertyChanged(); }
        }

        public ValueUncertaintyVM ValueUncertainty
        {
            get { return _ItemValueUncertainty; }
            set
            {
                _ItemValueUncertainty = value;
                _ItemValueUncertainty.WasModified += SomethingChanged;
            }
        }

        public OccTypeItem(OccTypeItem item)
        {
            ItemType = item.ItemType;
            IsChecked = item.IsChecked;
            Curve = item.Curve;
            TableWithPlot = new TableWithPlotVM(Curve);
            TableWithPlot.WasModified += SomethingChanged;
            ValueUncertainty = new MonetaryValueUncertaintyVM(item.ValueUncertainty.Distribution);
        }

        public OccTypeItem(OcctypeItemType itemType, bool isChecked, ComputeComponentVM curve, ContinuousDistribution valueUncertainty)
        {
            ItemType = itemType;
            IsChecked = isChecked;
            Curve = curve;
            TableWithPlot = new TableWithPlotVM(Curve);
            TableWithPlot.WasModified += SomethingChanged;
            ValueUncertainty = new MonetaryValueUncertaintyVM(valueUncertainty);
        }
        public void SomethingChanged(object sender, EventArgs e)
        {
            DataModified?.Invoke(this, EventArgs.Empty);
        }

        public virtual FdaValidationResult IsItemValid()
        {
            FdaValidationResult vr = new FdaValidationResult();

            FdaValidationResult valueUncertVR = ValueUncertainty.IsValueUncertaintyValid();
            //I want to indicate what "item type" this is.
            if(!valueUncertVR.IsValid)
            {
                string errorMessage = ItemType + " value uncertainty:\n" + valueUncertVR.ErrorMessage;
                vr.AddErrorMessage(errorMessage + Environment.NewLine);
            }

            //todo: this isn't working. I don't know how to get the errors from the table.
            paireddata.UncertainPairedData curve = TableWithPlot.ComputeComponentVM.SelectedItemToPairedData();
            curve.Validate();
            if(curve.HasErrors)
            {
                IEnumerable errors = curve.GetErrors();
                foreach(var error in errors)
                {
                    vr.AddErrorMessage(error.ToString());
                }
            }

            return vr;
        }
    }
}
