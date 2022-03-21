using FdaLogging;
using HEC.FDA.ViewModel.TableWithPlot;
using Statistics;
using System;
using System.Collections.Generic;
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

        public List<LogItem> IsItemValid()
        {
            List<LogItem> constructionErrors = new List<LogItem>();
            constructionErrors.AddRange(IsValueUncertaintyConstructable(ValueUncertainty, ItemType + " value uncertainty"));
            return constructionErrors;
        }

        public List<LogItem> IsValueUncertaintyConstructable(ValueUncertaintyVM uncertaintyVM, string uncertaintyName)
        {
            List<LogItem> constructionErrors = new List<LogItem>();
            try
            {
                uncertaintyVM.CreateOrdinate();
            }
            catch (Exception e)
            {
                string logMessage = "Error constructing " + uncertaintyName + ": " + e.Message;
                constructionErrors.Add(LogItemFactory.FactoryTemp(LoggingLevel.Fatal, logMessage));
            }
            return constructionErrors;
        }
    }
}
