using HEC.FDA.ViewModel.TableWithPlot;
using Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Inventory.OccupancyTypes;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class OccTypeItem:BaseViewModel
    {
        public event EventHandler DataModified;

        private bool _IsChecked;
        private ComputeComponentVM _Curve;
        private TableWithPlotVM _StructureTableWithPlot;
        private ValueUncertaintyVM _StructureValueUncertainty;


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
        //public ContinuousDistribution Distribution
        //{
        //    get { return _Distribution; }
        //    set { _Distribution = value; NotifyPropertyChanged(); }
        //}
        public TableWithPlotVM TableWithPlot
        {
            get { return _StructureTableWithPlot; }
            set { _StructureTableWithPlot = value; NotifyPropertyChanged(); }
        }

        public ValueUncertaintyVM ValueUncertainty
        {
            get { return _StructureValueUncertainty; }
            set
            {
                _StructureValueUncertainty = value;
                _StructureValueUncertainty.WasModified += SomethingChanged;
            }
        }

        public OccTypeItem(bool isChecked, ComputeComponentVM curve, ContinuousDistribution valueUncertainty)
        {
            IsChecked = isChecked;
            Curve = curve;
            //ValueUncertainty = valueUncertainty;
            TableWithPlot = new TableWithPlotVM(Curve);
            ValueUncertainty = new MonetaryValueUncertaintyVM(valueUncertainty);

        }
        private void SomethingChanged(object sender, EventArgs e)
        {
            DataModified?.Invoke(this, EventArgs.Empty);
        }

    }
}
