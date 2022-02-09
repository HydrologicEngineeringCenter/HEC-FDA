using HEC.Plotting.SciChart2D.ViewModel;
using paireddata;
using System;

namespace ViewModel.Utilities
{
    public class CoordinatesFunctionEditorVM
    {
        public event EventHandler TableChanged;
        public SciChart2DChartViewModel CoordinatesChartViewModel { get; set; }

        public UncertainPairedData Function { get; set; }         

        public CoordinatesFunctionEditorVM(UncertainPairedData upd, string xlabel, string ylabel, string name)
        {

        }

        public CoordinatesFunctionEditorVM(CoordinatesFunctionEditorVM vm)
        {

        }
        public CoordinatesFunctionEditorVM( )
        {

        }

        public UncertainPairedData CreateFunctionFromTables()
        {
            return DefaultPairedData.CreateDefaultDeterminateUncertainPairedData("", "", "");
        }

    }
}
