using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.Plots
{
    //[Author(q0heccdm, 12 / 19 / 2017 1:26:57 PM)]
    public class IndividualLinkedPlotControlVM:BaseViewModel,iConditionsControl
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/19/2017 1:26:57 PM
        #endregion
        #region Fields
        public event EventHandler SelectedCurveUpdated;
        public event EventHandler PlotIsShowing;
        public event EventHandler PlotIsNotShowing;
        public event EventHandler PopPlotOut;
        public event EventHandler PreviewCompute;

        private BaseViewModel _CurrentVM;
        #endregion
        #region Properties
        public IndividualLinkedPlotVM PreviousPlot { get; set; }
        public BaseViewModel PreviousVM
        {
            get;
            set;
        }
        public BaseViewModel CurrentVM
        {
            get { return _CurrentVM; }
            set { PreviousVM = _CurrentVM; _CurrentVM = value; NotifyPropertyChanged(); }
        }

        public IIndividualLinkedPlotWrapper IndividualPlotWrapperVM { get; set; }
        //public IndividualLinkedPlotVM MyIndividualLinkedPlotVM { get; set; }
        public ICoverButton ImportButtonVM { get; set; }
        public iConditionsImporter CurveImporterVM { get; set; }

        public ICoverButton ModulatorCoverButtonVM { get; set; }
        public IIndividualLinkedPlotWrapper ModulatorPlotWrapperVM { get; set; }
        #endregion
        #region Constructors
        public IndividualLinkedPlotControlVM():base()
        {
        }
        public IndividualLinkedPlotControlVM(IIndividualLinkedPlotWrapper indLinkedPlotWrapperVM, ICoverButton coverButton, iConditionsImporter importerVM, ICoverButton modulatorCoverButton = null, IIndividualLinkedPlotWrapper modulatorPlotWrapper = null)//Conditions.AddFlowFrequencyToConditionVM addFlowFreqVM)
        {
            IndividualPlotWrapperVM = indLinkedPlotWrapperVM;
            IndividualPlotWrapperVM.ShowImportButton += new EventHandler(ShowTheImportButton);
            IndividualPlotWrapperVM.ShowTheImporter += new EventHandler(ImportButtonClicked);
            //IndividualPlotWrapperVM.TrackerIsOutsideRange += new EventHandler(TrackerIsOutsideCurveRange);
            //IndividualPlotWrapperVM.CurveUpdated += SelectedCurveUpdated;

            ImportButtonVM = coverButton;
            ImportButtonVM.Clicked += new EventHandler(ImportButtonClicked);

            if (importerVM != null)
            {
                CurveImporterVM = importerVM;
                CurveImporterVM.OKClickedEvent += new EventHandler(AddCurveToPlot);
                CurveImporterVM.CancelClickedEvent += new EventHandler(ImporterWasCanceled);
                CurveImporterVM.PopImporterOut += new EventHandler(PopTheImporterOut);
            }

            ModulatorCoverButtonVM = modulatorCoverButton;
            //if (ModulatorCoverButtonVM != null)
            //{
            //    ModulatorCoverButtonVM.Clicked += new EventHandler(ModulatorCoverButtonClicked);
            //}

            ModulatorPlotWrapperVM = modulatorPlotWrapper;
            //show import button
            //show the importer

            if(ModulatorCoverButtonVM != null)
            {
                CurrentVM = (BaseViewModel)ModulatorCoverButtonVM;
            }
            else
            {
                CurrentVM = (BaseViewModel)ImportButtonVM;

            }
          

        }
        #endregion
        #region Voids

        //private void ModulatorCoverButtonClicked(object sender, EventArgs e)
        //{
        //    //pop plot 1 out
        //    //set the current vm to be the importer? this might already be getting fired?
        //}

        private void PopTheImporterOut(object sender, EventArgs e)
        {
            CurveImporterVM.IsPoppedOut = true;

            ShowPreviousVM(sender, e);

            Navigate((BaseViewModel)CurveImporterVM);
            if (((BaseViewModel)CurveImporterVM).WasCanceled == true)
            {
                CurveImporterVM.IsPoppedOut = false;
            }
        }

        private void ImporterWasCanceled(object sender, EventArgs e)
        {
            //if there is a previous curve then set it
            if (PreviousPlot != null)
            {
                IndividualPlotWrapperVM.PlotVM = PreviousPlot;
            }
            //update the linkages

            if (CurveImporterVM.IsPoppedOut == true)
            {

                CurveImporterVM.IsPoppedOut = false;
            }
            else
            {
                ShowPreviousVM(sender, e);
            }

            

        }

        /// <summary>
        /// This gets called when the user is on the import curve form but clicks the cancel button. I will take them back to the VM they were previously on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ShowPreviousVM(object sender, EventArgs e)
        {
            CurrentVM = PreviousVM;
        }

        //public void TrackerIsOutsideCurveRange(object sender, EventArgs e)
        //{
        //    int test = 0;
        //    //CurrentVM = (BaseViewModel)ImportButtonVM;
        //    //PlotIsNotShowingAnymore(sender, e);
        //}
        /// <summary>
        /// this gets called when the user clicks the delete button on the plot wrapper
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ShowTheImportButton(object sender, EventArgs e)
        {
            CurrentVM = (BaseViewModel)ImportButtonVM;
            PlotIsNotShowingAnymore(sender, e);
        }
        /// <summary>
        /// This gets called when the user clicks the import curve button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ImportButtonClicked(object sender,EventArgs e)
        {
            //special logic for the plot8 preview. I want to skip the importer and run the preview compute logic
            if(CurveImporterVM == null)
            {
                PreviewTheCompute(this, new EventArgs());
                return;
            }

            //this gets a little crazy but if the user clicks the modulator cover button the importer will pop up.
            //if they then click cancel i want the view to show the importer button, not the modulator importer button.
           bool previousViewShouldBeTheCoverButton = false;
           if(CurrentVM == ModulatorCoverButtonVM)
            {
                previousViewShouldBeTheCoverButton = true;
            }

            
            //update the linkages, in the curve changed call back, if it gets set to null, then take it away from the selected curve prop
            //this feels hacky but the curve in the oxyplot is going to get set to null automatically. I am not totally sure why. This will
            //cause a crash if the user moves the mouse over one of the other plots becuase the curve will be null on this plot.
            //i am therefore preemptively setting the curve to null and updating the linkages.
            //8/30/18 this doesn't seem to be the case anymore so i commented it out
            //if (IndividualPlotWrapperVM.PlotVM != null)
            //{
            //   // IndividualPlotWrapperVM.PlotVM.Curve = null;
            //}
            
            CurrentVM = (BaseViewModel)CurveImporterVM;

            if (previousViewShouldBeTheCoverButton == true)
            {
                PreviousVM = (BaseViewModel)ImportButtonVM;
            }
            
        }

       

        /// <summary>
        /// This gets called when the user clicks the OK button on the importer form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddCurveToPlot(object sender,EventArgs e)
        {

            CurveImporterVM.IsPoppedOut = false;

            IndividualPlotWrapperVM.SubTitle = CurveImporterVM.SelectedElement.Name;
            //switch to the plot vm
            IndividualPlotWrapperVM.PlotVM = new IndividualLinkedPlotVM(CurveImporterVM.BaseFunction, CurveImporterVM.SelectedCurve, CurveImporterVM.SelectedElementName);

            CurrentVM = (BaseViewModel)IndividualPlotWrapperVM;

            //store the previouse curve
            PreviousPlot = new IndividualLinkedPlotVM(CurveImporterVM.BaseFunction, CurveImporterVM.SelectedCurve, CurveImporterVM.SelectedElementName); 

            if (ModulatorPlotWrapperVM != null)
            {
                ModulatorPlotWrapperVM.PlotVM = new IndividualLinkedPlotVM(CurveImporterVM.BaseFunction, CurveImporterVM.SelectedCurve, CurveImporterVM.SelectedElementName);
                //CurrentVM = (BaseViewModel)ModulatorPlotWrapperVM;
            }

            //tell the conditions plot editor vm that this plot is showing
            if (PlotIsShowing != null)
            {
                PlotIsShowing(sender, e);
            }

            //raise the updateplots event - is this still necesarry i update the plots in the ind linked plots loaded event
            if (SelectedCurveUpdated != null)
            {
                SelectedCurveUpdated(sender, e);
            }

        }

        public void PreviewTheCompute(object sender, EventArgs e)
        {
            if (PreviewCompute != null)
            {
                PreviewCompute(sender, e);
            }
        }

        public void PlotIsNotShowingAnymore(object sender, EventArgs e)
        {
            if(PlotIsNotShowing != null)
            {
                PlotIsNotShowing(sender, e);
            }
            if (SelectedCurveUpdated != null)
            {
                SelectedCurveUpdated(sender, e);
            }
        }

        public void PopThePlotOut(object sender, EventArgs e)
        {
            if(PopPlotOut != null)
            {
                PopPlotOut(sender, e);
            }
        }

        public void SetCurrentViewToCoverButton()
        {
            CurrentVM = (BaseViewModel)ImportButtonVM;
        }

        public override void AddValidationRules()
        {
           // throw new NotImplementedException();
        }

       
        #endregion
        #region Functions
        #endregion
    }
}
