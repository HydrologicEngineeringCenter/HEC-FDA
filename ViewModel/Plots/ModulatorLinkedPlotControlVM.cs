using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Plots
{
    public class ModulatorLinkedPlotControlVM : IndividualLinkedPlotControlVM
    {
        private double _MinX;
        private double _MaxX;
        private double _MinY;
        private double _MaxY;

        private double _CurrentX;
        private double _CurrentY;

        public double MinX
        {
            get { return _MinX; }
            set { _MinX = value; NotifyPropertyChanged(); }
        }

        public double MaxX
        {
            get { return _MaxX; }
            set { _MaxX = value; NotifyPropertyChanged(); }
        }
        public double MinY
        {
            get { return _MinY; }
            set { _MinY = value; NotifyPropertyChanged(); }
        }
        public double MaxY
        {
            get { return _MaxY; }
            set { _MaxY = value; NotifyPropertyChanged(); }
        }
        public double CurrentX
        {
            get { return _CurrentX; }
            set { _CurrentX = value; NotifyPropertyChanged(); }
        }
        public double CurrentY
        {
            get { return _CurrentY; }
            set { _CurrentY = value; NotifyPropertyChanged(); }
        }


        //public ModulatorLinkedPlotControlVM(IIndividualLinkedPlotWrapper indLinkedPlotWrapperVM, ICoverButton coverButton, iConditionsImporter importerVM,
        //    bool isYAxisLog, bool isProbabilityXAxis,
        //     ICoverButton modulatorCoverButton = null, IIndividualLinkedPlotWrapper modulatorPlotWrapper = null):base(indLinkedPlotWrapperVM,
        //         coverButton, importerVM, isYAxisLog, isProbabilityXAxis)
        //{
        //    int i = 0;
        //}


    }
}
