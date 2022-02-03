using HEC.Plotting.SciChart2D.ViewModel;
using paireddata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Utilities
{
    public class CoordinatesFunctionEditorVM
    {
        public event EventHandler TableChanged;
        public SciChart2DChartViewModel CoordinatesChartViewModel { get; set; }

        public UncertainPairedData Function { get; set; }
        //public List<String> Messages { get; set; }
         

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
