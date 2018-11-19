using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Fda.Plots;
using System.Collections.ObjectModel;

namespace Fda.Conditions
{
    /// <summary>
    /// Interaction logic for ConditionsPlotEditor.xaml
    /// </summary>
    public partial class ConditionsPlotEditor : UserControl
    {
        private double _SpecifiedXValue;
        private double _SpecifiedYValue;
        private IndividualLinkedPlot _SelectedPlot;

        public bool HideTrackers { get; set; }
        public bool Plot1PoppedOut { get; set; }
        public bool Plot1DoesntExist { get; set; }
        public bool Plot5PoppedOut { get; set; }
        public bool Plot5DoesntExist { get; set; }
        public bool AreaPlotsHaveBeenRemoved { get; set; }

        public ObservableCollection<ILinkedPlot> TheAddedPlots
        {
            get;set;
        }

        public ObservableCollection<IndividualLinkedPlot> ObservablePlots { get; set; }
        public ObservableCollection<IndividualLinkedPlotControl> AddedPlotControls
        {
            get; set;
        }



        public ConditionsPlotEditor()
        {
            InitializeComponent();

            Plot0Control.UpdatePlots += new EventHandler(UpdateThePlotLinkages);
            DLMControl.UpdatePlots += new EventHandler(UpdateThePlotLinkages);
            Plot1Control.UpdatePlots += new EventHandler(UpdateThePlotLinkages);
            Plot3Control.UpdatePlots += new EventHandler(UpdateThePlotLinkages);
            DLMHorizontalControl.UpdatePlots += new EventHandler(UpdateThePlotLinkages);
            Plot5Control.UpdatePlots += new EventHandler(UpdateThePlotLinkages);
            Plot7Control.UpdatePlots += new EventHandler(UpdateThePlotLinkages);
            Plot8Control.UpdatePlots += new EventHandler(UpdateThePlotLinkages);


            DLMControl.PopImporterIntoPlot1 += new EventHandler(PopPlot1ImporterOut);
            DLMControl.PopPlotIntoPlot1 += new EventHandler(PopPlot1Out);

            DLMHorizontalControl.PopImporterIntoPlot5 += new EventHandler(PopPlot5ImporterOut);
            DLMHorizontalControl.PopPlotIntoPlot5 += new EventHandler(PopPlot5Out);

            FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;

            TheAddedPlots = new ObservableCollection<Plots.ILinkedPlot>();
            AddedPlotControls = new ObservableCollection<IndividualLinkedPlotControl>();
            ObservablePlots = new ObservableCollection<IndividualLinkedPlot>();
            //doubleLineModulator.PopOutThePlot += new EventHandler(PopPlot1Out);
            //DoubleLineHorizontal.PopOutThePlot += new EventHandler(PopPlot5Out);

            //plot0.ChangeThisCurve += new EventHandler(vm.LaunchAddInflowFrequencyCurve);

            //FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;
            
        }

        
        public void UpdateThePlotLinkages(object sender, EventArgs e)
        {
            UpdateTheListOfAddedCurves();
            UpdateTheLinkages(TheAddedPlots.ToList());
            SetTheSharedAxes();
            
        }
        private void UpdateTheListOfAddedCurves()
        {

            // i think maybe i should move this list to the VM side of things?
            // instead of looking at the ".selectedCurve" i should maybe have a bool or something
            //that way when i have plot 1 and DLM i can set one to "don't use" and the other to "Use".
           


            TheAddedPlots = new ObservableCollection<Plots.ILinkedPlot>();

            if(Plot0Control.LinkedPlot != null)
            {
                TheAddedPlots.Add(Plot0Control.LinkedPlot);
                AddedPlotControls.Add(Plot0Control);
            }
            if (Plot1Control.LinkedPlot != null && Plot1PoppedOut == true)
            {
                TheAddedPlots.Add(Plot1Control.LinkedPlot);
            }
            else if (DLMControl.LinkedPlot != null && Plot1PoppedOut == false)
            {
                TheAddedPlots.Add(DLMControl.LinkedPlot);
            }

            if (Plot3Control.LinkedPlot != null)
            {
                TheAddedPlots.Add(Plot3Control.LinkedPlot);
            }

            if(Plot5Control.LinkedPlot != null && Plot5PoppedOut == true)
            {
                TheAddedPlots.Add(Plot5Control.LinkedPlot);
            }
            else if (DLMHorizontalControl.LinkedPlot != null && Plot5PoppedOut == false)
            {
                TheAddedPlots.Add(DLMHorizontalControl.LinkedPlot);
            }

            if (Plot7Control.LinkedPlot != null)
            {
                TheAddedPlots.Add(Plot7Control.LinkedPlot);
            }
            if (Plot8Control.LinkedPlot != null)
            {
                TheAddedPlots.Add(Plot8Control.LinkedPlot);
            }

            //this is here to load the list of plots that displays in the combobox for the 
            //specified point tool
            ObservablePlots.Clear();
            foreach(ILinkedPlot plot in TheAddedPlots)
            {
                if(plot.GetType() == typeof(IndividualLinkedPlot))
                {
                    ObservablePlots.Add((IndividualLinkedPlot)plot);
                }
            }
        }





        #region Add Plots Click Events

        private void btn_AddPlot0_Click(object sender, RoutedEventArgs e)
        {
            ////display the add flow frequency window
            //FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;
            //vm.LaunchAddInflowFrequencyCurve(this, new EventArgs());
            //if (vm.Plot0VM.IsVisible == true)
            //{
            //    AddedPlots.Add(plot0);
            //    UpdateTheLinkages();
            //}
            ////plot0.SetAsStartNode();

            //plot0.OxyPlot1.InvalidatePlot(true);
        }



        private void btn_AddPlot1_Click(object sender, RoutedEventArgs e)
        {
            //FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;
            //vm.LaunchAddInflowOutflowCurve();
            //if (vm.IsPlot1Visible == true)
            //{
            //    if (Plot1PoppedOut == false)
            //    {
            //        //AddedPlots.Add(doubleLineModulator);
            //    }
            //    else
            //    {
            //        AddedPlots.Add(plot1);
            //    }
            //    UpdateTheLinkages();
            //    //btn_CollapsePlot1_Click(sender, e);
            //}

            //////the following two lines are required to match the axes up properly between the two
            ////plot0.OxyPlot1.Model.Axes[1].Maximum = plot0.MaxY;
            ////plot0.OxyPlot1.Model.Axes[1].Minimum = plot0.MinY;

            ////plot0.SetNextPlotLinkage(doubleLineModulator, "y", "x");
            ////doubleLineModulator.SetPreviousPlotLinkage(plot0);
            ////doubleLineModulator.SetAsEndNode();



            //plot1.OxyPlot1.InvalidatePlot(true);
        }


        private void btn_AddPlot3_Click(object sender, RoutedEventArgs e)
        {
            //FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;
            //vm.LaunchAddRatingCurve();
            //if (vm.IsPlot3Visible == true)
            //{
            //    AddedPlots.Add(plot3);
            //    UpdateTheLinkages();
            //}

           


            //plot0.OxyPlot1.InvalidatePlot(true);
            //plot3.OxyPlot1.InvalidatePlot(true);

        }


        private void btn_AddPlot5_Click(object sender, RoutedEventArgs e)
        {
            //FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;
            //vm.LaunchAddExteriorInteriorCurve();

            ////the following two lines are required to match the axes up properly between the two
            //plot3.OxyPlot1.Model.Axes[0].Maximum = plot3.MaxX;
            //plot3.OxyPlot1.Model.Axes[0].Minimum = plot3.MinX;

            //if (vm.IsPlot5Visible == true)
            //{
            //    if (Plot5PoppedOut == false)
            //    {
            //        //AddedPlots.Add(DoubleLineHorizontal);
            //    }
            //    else
            //    {
            //        AddedPlots.Add(plot5);
            //    }
            //    UpdateTheLinkages();

            //}

            //////turn off plot3 as endnode
            ////plot3.ThisIsEndNode = false;
            ////plot3.SetNextPlotLinkage(DoubleLineHorizontal, "X", "X");
            ////DoubleLineHorizontal.SetPreviousPlotLinkage(plot3);
            ////DoubleLineHorizontal.ThisIsEndNode = true;

            //plot3.OxyPlot1.InvalidatePlot(true);
            ////plot7.OxyPlot1.InvalidatePlot(true);
        }

        private void btn_AddPlot7_Click(object sender, RoutedEventArgs e)
        {
            //FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;
            //vm.LaunchAddStageDamageCurve();

            //if (vm.IsPlot7Visible == true)
            //{
            //    AddedPlots.Add(plot7);
            //    UpdateTheLinkages();
            //}
            

            //plot3.OxyPlot1.InvalidatePlot(true);
            //plot7.OxyPlot1.InvalidatePlot(true);
        }


        private void btn_AddPlot8_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;
            vm.RunPreviewCompute(this, new EventArgs());
            

        }
        #endregion

        #region Pop Plots In and Out
        private void PopPlot1ImporterOut(object sender, EventArgs e)
        {
            Plot1PoppedOut = true;
            //make sure it is showing the importer for the inflow outflow curve
            FdaViewModel.Plots.IndividualLinkedPlotControlVM vm = (FdaViewModel.Plots.IndividualLinkedPlotControlVM)Plot1Control.DataContext;
            vm.ImportButtonClicked(sender, e);//this should cause it to display the import form instead of the add button

            FourPlotGrid.ColumnDefinitions[2].Width = new GridLength(0);

            mainGrid.RowDefinitions[1].Height = new GridLength(.4, GridUnitType.Star);

        }
        private void PopPlot5ImporterOut(object sender, EventArgs e)
        {
            Plot5PoppedOut = true;
            //make sure it is showing the importer for the inflow outflow curve
            FdaViewModel.Plots.IndividualLinkedPlotControlVM vm = (FdaViewModel.Plots.IndividualLinkedPlotControlVM)Plot5Control.DataContext;
            vm.ImportButtonClicked(sender, e);//this should cause it to display the import form instead of the add button

            FourPlotGrid.RowDefinitions[2].Height = new GridLength(0);
            mainGrid.ColumnDefinitions[0].Width = new GridLength(.45, GridUnitType.Star);

            //move the toolbar over
            Grid.SetColumn(grid_TopRow, 0);

        }

        private void PopPlot1Out(object sender, EventArgs e)
        {            
            Plot1PoppedOut = true;    
            //UpdateTheLinkages();
            //collapse the column the modulator is in
            FourPlotGrid.ColumnDefinitions[2].Width = new GridLength(0);

            mainGrid.RowDefinitions[1].Height = new GridLength(.4, GridUnitType.Star);
        }

        private void PopPlot5Out(object sender, EventArgs e)
        {
            //FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;

            Plot5PoppedOut = true;
            //collapse the row the modulator is in
            FourPlotGrid.RowDefinitions[2].Height = new GridLength(0);

            //AddedPlots.Remove(DoubleLineHorizontal);
            //AddedPlots.Add(plot5);
            UpdateThePlotLinkages(sender, e);
            //UpdateTheLinkages(TheAddedPlots.ToList());

            mainGrid.ColumnDefinitions[0].Width = new GridLength(.45, GridUnitType.Star);

            //move the toolbar over
            Grid.SetColumn(grid_TopRow, 0);
            //plot5.OxyPlot1.Model.InvalidatePlot(true);

           
            
        }

        private void btn_CollapsePlot1_Click(object sender, RoutedEventArgs e)
        {

            FdaViewModel.Plots.IndividualLinkedPlotControlVM vm = (FdaViewModel.Plots.IndividualLinkedPlotControlVM)Plot1Control.DataContext;
            //vm.IndividualPlotWrapperVM.PlotVM = ((FdaViewModel.Plots.IndividualLinkedPlotControlVM)Plot1Control.DataContext).IndividualPlotWrapperVM.PlotVM;
            if (vm.CurrentVM == vm.CurveImporterVM)
            {
                btn_CollapsePlot1.ToolTip = "Cannot pop in the curve importer form.";
                return;//this makes it so that the user can't pop in the import form because there isn't enough room for it.
            }
            else if (vm.CurrentVM == vm.IndividualPlotWrapperVM)
            {
                vm.CurrentVM = (FdaViewModel.BaseViewModel)vm.ModulatorPlotWrapperVM;
            }
            else if(vm.CurrentVM == vm.ImportButtonVM)
            {
                //if plot1 is on the import button then there is no plot in plot1. we need to make sure that the modulator also has no plot now.
                DLMControl.LinkedPlot = null;
                vm.CurrentVM = (FdaViewModel.BaseViewModel)vm.ModulatorCoverButtonVM;
            }

            Plot1PoppedOut = false;
            UpdateThePlotLinkages(sender,e);

            //collapse the column the modulator is in
            FourPlotGrid.ColumnDefinitions[2].Width = new GridLength(45);


            mainGrid.RowDefinitions[1].Height = new GridLength(0);
           
        }

        private void btn_CollapsePlot5_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Plots.IndividualLinkedPlotControlVM vm = (FdaViewModel.Plots.IndividualLinkedPlotControlVM)Plot5Control.DataContext;
            //vm.IndividualPlotWrapperVM.PlotVM = ((FdaViewModel.Plots.IndividualLinkedPlotControlVM)Plot1Control.DataContext).IndividualPlotWrapperVM.PlotVM;
            if (vm.CurrentVM == vm.CurveImporterVM)
            {
                btn_CollapsePlot5.ToolTip = "Cannot pop in the curve importer form.";
                return;//this makes it so that the user can't pop in the import form because there isn't enough room for it.
            }
            else if (vm.CurrentVM == vm.IndividualPlotWrapperVM)
            {
                vm.CurrentVM = (FdaViewModel.BaseViewModel)vm.ModulatorPlotWrapperVM;
                
            }
            else if (vm.CurrentVM == vm.ImportButtonVM)
            {
                //if plot1 is on the import button then there is no plot in plot1. we need to make sure that the modulator also has no plot now.
                DLMControl.LinkedPlot = null;
                vm.CurrentVM = (FdaViewModel.BaseViewModel)vm.ModulatorCoverButtonVM;
            }

            Plot5PoppedOut = false;


            UpdateThePlotLinkages(sender, e);
            //collapse the row the modulator is in
            FourPlotGrid.RowDefinitions[2].Height = new GridLength(45);
            mainGrid.ColumnDefinitions[0].Width = new GridLength(0);
            //move the toolbar over
            Grid.SetColumn(grid_TopRow, 1);
        }

        private void btn_PopPlotsOut_Click(object sender, RoutedEventArgs e)
        {
            //FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;

            //if (Plot1PoppedOut == false || Plot5PoppedOut == false)
            //{
            //    if (vm.IsPlot1Visible == true)
            //    {
            //        PopPlot1Out(sender, e);
            //    }
            //    if (vm.IsPlot5Visible == true)
            //    {
            //        PopPlot5Out(sender, e);
            //    }
            //    //change the image to pop in and change the tooltip
            //    //img_PopPlotsOut.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/PopDown.png"));
            //    //btn_PopPlotsOut.ToolTip = "Pop Plots In";
            //}
            //else
            //{
            //    if (vm.IsPlot1Visible == true)
            //    {
            //        btn_CollapsePlot1_Click(sender, e);
            //    }
            //    if (vm.IsPlot5Visible == true)
            //    {
            //        btn_CollapsePlot5_Click(sender, e);
            //    }
            //    //change the image to pop out and change the tooltip
            //    //img_PopPlotsOut.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/PopUp.png"));
            //    //btn_PopPlotsOut.ToolTip = "Pop Plots Out";

            //}
        }

        #endregion

        private void btn_HideTrackers_Click(object sender, RoutedEventArgs e)
        {
            if (HideTrackers == false)
            {
                //plot0.HideTracker();
                //plot1.HideTracker();
                //plot3.HideTracker();
                //plot5.HideTracker();
                //plot7.HideTracker();
                //plot8.HideTracker();

                FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;
                vm.Plot0ControlVM.IndividualPlotWrapperVM.TrackerVisible = false;
                vm.Plot1ControlVM.IndividualPlotWrapperVM.TrackerVisible = false;
                vm.Plot1ControlVM.ModulatorPlotWrapperVM.TrackerVisible = false;

                vm.Plot3ControlVM.IndividualPlotWrapperVM.TrackerVisible = false;
                vm.Plot5ControlVM.IndividualPlotWrapperVM.TrackerVisible = false;
                vm.Plot5ControlVM.ModulatorPlotWrapperVM.TrackerVisible = false;

                vm.Plot7ControlVM.IndividualPlotWrapperVM.TrackerVisible = false;
                vm.Plot8ControlVM.IndividualPlotWrapperVM.TrackerVisible = false;


                FourPlotGrid.ColumnDefinitions[2].Width = new GridLength(0);
                FourPlotGrid.RowDefinitions[2].Height = new GridLength(0);
                HideTrackers = true;

                //clear the lines from the double line modulator. it is showing on top of the other plots for some reason
                //doubleLineModulator.myCanvas.Children.Clear();

                //change the tracker button image and change the tooltip
                img_HideTrackers.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/ShowTrackers.png"));
                btn_HideTrackers.ToolTip = "Show Trackers";

            }
            else
            {
                //plot0.ShowTracker();
                //plot1.ShowTracker();
                //plot3.ShowTracker();
                //plot5.ShowTracker();
                //plot7.ShowTracker();
                //plot8.ShowTracker();

                FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;
                vm.Plot0ControlVM.IndividualPlotWrapperVM.TrackerVisible = true;
                vm.Plot1ControlVM.IndividualPlotWrapperVM.TrackerVisible = true;
                vm.Plot1ControlVM.ModulatorPlotWrapperVM.TrackerVisible = true;

                vm.Plot3ControlVM.IndividualPlotWrapperVM.TrackerVisible = true;
                vm.Plot5ControlVM.IndividualPlotWrapperVM.TrackerVisible = true;
                vm.Plot5ControlVM.ModulatorPlotWrapperVM.TrackerVisible = true;

                vm.Plot7ControlVM.IndividualPlotWrapperVM.TrackerVisible = true;
                vm.Plot8ControlVM.IndividualPlotWrapperVM.TrackerVisible = true;

                if (Plot1PoppedOut == false)
                {
                    FourPlotGrid.ColumnDefinitions[2].Width = new GridLength(45);
                }
                if (Plot5PoppedOut == false)
                {
                    FourPlotGrid.RowDefinitions[2].Height = new GridLength(45);
                }
                HideTrackers = false;
                //change the tracker button image and change the tooltip
                img_HideTrackers.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/HideTrackers.png"));
                btn_HideTrackers.ToolTip = "Hide Trackers";

            }
        }

        #region Update the linkages
        public static void UpdateTheLinkages(List<ILinkedPlot> linkedPlots)
        {

            ////clear and then reset all the min max values for each plot
            //foreach(Plots.ILinkedPlot plot in AddedPlots)
            //{
            //    plot.set
            //}

            if (linkedPlots.Count == 0) { return; }
            List<Plots.ILinkedPlot> sortedList = linkedPlots.OrderBy(o => o.BaseFunction.FunctionType).ToList();

            //once a compute has been done, the 0 becomes a 2 and so adding a 1 will no longer work
            if (sortedList.Count > 1)
            {
                //if you have a 1 and then a 2, switch them
                if (sortedList[0].BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InflowOutflow
                    && sortedList[1].BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency)
                {
                    Plots.ILinkedPlot tempPlot = sortedList[1];
                    sortedList.RemoveAt(1);
                    sortedList.Insert(0, tempPlot);
                }
            }

            //********  clear all the startnodes and endnodes and reset min/max values ************
            foreach (Plots.ILinkedPlot p in sortedList)
            {
                p.IsStartNode = false;
                p.IsEndNode = false;
                if (p.GetType() == typeof(IndividualLinkedPlot))
                {
                    ((IndividualLinkedPlot)p).RemoveAreaPlots();
                    ((IndividualLinkedPlot)p).HasXAreaPlots = false;
                    ((IndividualLinkedPlot)p).HasYAreaPlots = false;
                    //For some reason the area plots get flipped around if the min max values get reset on plot8
                    //I think it is already getting set somewhere? Either way, plot8 is a bit different because there is no
                    //importer so i exclude it from resetting the min max values here.
                    if (((IndividualLinkedPlot)p).BaseFunction.FunctionType != FdaModel.Functions.FunctionTypes.DamageFrequency)
                    {
                        IndividualLinkedPlot.SetMinMaxValues((IndividualLinkedPlot)p);
                    }
                }
            }

            // *******    set up the first plot *********
            if (sortedList.Count == 0) { return; }
            sortedList[0].IsStartNode = true;
            if (sortedList.Count == 1)
            {
                sortedList[0].IsEndNode = true;
                return;
            }

            sortedList[0].SetNextPlotLinkage(sortedList[1], "", "");


            //**********   set the linkages for all but the last plot
            for (int i = 1; i < sortedList.Count - 1; i++)
            {

                sortedList[i].SetNextPlotLinkage(sortedList[i + 1], "", "");
                sortedList[i].SetPreviousPlotLinkage(sortedList[i - 1]);

            }
            //*********** set the last plot linkage
            sortedList[sortedList.Count - 1].SetPreviousPlotLinkage(sortedList[sortedList.Count - 2]);
            sortedList[sortedList.Count - 1].IsEndNode = true;

        }
        private void SetTheSharedAxes()
        { 

            //*********** set any shared axes
            if (IsThereAFlowFrequencyAndRatingCurves() == true)
            {
                //plot0.SetSharedYAxisWithPlot(plot3);
                ((Plots.IndividualLinkedPlot)Plot0Control.LinkedPlot).SetSharedYAxisWithPlot((Plots.IndividualLinkedPlot)Plot3Control.LinkedPlot);
            }
            if (IsThereARatingAndStageDamageCurve() == true)
            {
                ((Plots.IndividualLinkedPlot)Plot3Control.LinkedPlot).SetSharedXAxisWithPlot((Plots.IndividualLinkedPlot)Plot7Control.LinkedPlot);
            }
            if(IsThereAStageDamageAndDamageFreq())
            {
                ((Plots.IndividualLinkedPlot)Plot7Control.LinkedPlot).SetSharedYAxisWithPlot((Plots.IndividualLinkedPlot)Plot8Control.LinkedPlot);
            }
            if (IsThereAFlowFreqAndDamageFreq())
            {
                ((Plots.IndividualLinkedPlot)Plot8Control.LinkedPlot).SetSharedXAxisWithPlot((Plots.IndividualLinkedPlot)Plot0Control.LinkedPlot);

            }

        }
        private  bool IsThereAStageDamageAndDamageFreq()
        {
            bool StageDamageExists = false;
            bool DamageFreqExists = false;
            foreach (ILinkedPlot p in TheAddedPlots)
            {
                if (p.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InteriorStageDamage)
                {
                    StageDamageExists = true;
                }
                else if (p.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
                {
                    DamageFreqExists = true;
                }
            }
            if (StageDamageExists == true && DamageFreqExists == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private  bool IsThereAFlowFreqAndDamageFreq()
        {
            bool flowFrequencyExists = false;
            bool DamageFreqExists = false;
            foreach (Plots.ILinkedPlot p in TheAddedPlots)
            {
                if (p.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency || p.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency)
                {
                    flowFrequencyExists = true;
                }
                else if (p.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
                {
                    DamageFreqExists = true;
                }
            }
            if (flowFrequencyExists == true && DamageFreqExists == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private  bool IsThereAFlowFrequencyAndRatingCurves()
        {
            bool flowFrequencyExists = false;
            bool ratingExists = false;
            foreach (Plots.ILinkedPlot p in TheAddedPlots)
            {
                if (p.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency)
                {
                    flowFrequencyExists = true;
                }
                else if (p.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.Rating)
                {
                    ratingExists = true;
                }
            }
            if (flowFrequencyExists == true && ratingExists == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private  bool IsThereARatingAndStageDamageCurve()
        {
            bool stageDamageExists = false;
            bool ratingExists = false;
            foreach (Plots.ILinkedPlot p in TheAddedPlots)
            {
                if (p.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InteriorStageDamage)
                {
                    stageDamageExists = true;
                }
                else if (p.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.Rating)
                {
                    ratingExists = true;
                }
            }
            if (stageDamageExists == true && ratingExists == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        private void btn_AddImpactAreas_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;
            vm.LaunchNewImpactArea(sender, e);
        }

        private void RemoveAreaPlots()
        {
            FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;
            vm.Plot0ControlVM.IndividualPlotWrapperVM.AreaPlotVisible = false;
            vm.Plot3ControlVM.IndividualPlotWrapperVM.AreaPlotVisible = false;
            vm.Plot7ControlVM.IndividualPlotWrapperVM.AreaPlotVisible = false;
            vm.Plot8ControlVM.IndividualPlotWrapperVM.AreaPlotVisible = false;
        }

        private void AddAreaPlots()
        {
            FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;
            vm.Plot0ControlVM.IndividualPlotWrapperVM.AreaPlotVisible = true;
            vm.Plot3ControlVM.IndividualPlotWrapperVM.AreaPlotVisible = true;
            vm.Plot7ControlVM.IndividualPlotWrapperVM.AreaPlotVisible = true;
            vm.Plot8ControlVM.IndividualPlotWrapperVM.AreaPlotVisible = true;
        }

        private void btn_ToggleAreaPlots_Click(object sender, RoutedEventArgs e)
        {
            if (AreaPlotsHaveBeenRemoved == false)
            {

                RemoveAreaPlots();

                AreaPlotsHaveBeenRemoved = true;
                //change the image and change the tooltip
                img_HideAreaPlots.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/ShowAreaPlots.png"));
                btn_ToggleAreaPlots.ToolTip = "Show Area Plots";
            }
            else
            {
                AddAreaPlots();
                AreaPlotsHaveBeenRemoved = false;
                //change the image and change the tooltip
                img_HideAreaPlots.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/HideAreaPlots.png"));
                btn_ToggleAreaPlots.ToolTip = "Hide Area Plots";
            }
        }

        #region Plot Specific Point
        //private void btn_PlotSpecificPoint_Click(object sender, RoutedEventArgs e)
        //{
        //   // //if both the x and the y have values in them then just go with the x value
        //   // //if neither of them have anything then don't do anything
        //   // //message if value is not a double or is out of range

        //   // if (cmb_PlotNames.SelectedIndex == -1) { return; }
        //   // //_SelectedPlotControl = (Plots.IndividualLinkedPlotControl)cmb_PlotNames.SelectedItem; 
        //   // _SelectedPlot = (IndividualLinkedPlot)cmb_PlotNames.SelectedItem;
        //   //// double xValue = 0;
        //   // //double yValue = 0;




        //   // {
        //   //     if (_SelectedPlot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency ||
        //   //         _SelectedPlot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency ||
        //   //         _SelectedPlot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
        //   //     {
        //   //         GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(true);
        //   //     }
        //   //     else
        //   //     {
        //   //         GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(false);

        //   //     }
        //   //     ScreenPoint position = _SelectedPlot.OxyPlot1.Model.Axes[0].Transform(_SpecifiedXValue, _SpecifiedYValue, _SelectedPlot.OxyPlot1.Model.Axes[1]);

        //   //     //if the trackers are already frozen, unfreeze them
        //   //     if (_SelectedPlot.FreezeNextTracker == true)
        //   //     {
        //   //         _SelectedPlot.Model_MouseDown(new object(), new OxyMouseDownEventArgs());
        //   //     }
        //   //     _SelectedPlot.DisplayTheTrackers(position);
        //   //     //now that the trackers are displayed, freeze them
        //   //     if (_SelectedPlot.FreezeNextTracker == false)
        //   //     {
        //   //         _SelectedPlot.Model_MouseDown(new object(), new OxyMouseDownEventArgs());
        //   //     }





        //   // }
            





        //}


        private void GetAndValidateXAndYValuesForFreezingSpecificPlotPoint( bool isAxisReversed, bool plotBasedOnXValue)
        {
            //try
            //{
            //if (_SelectedPlotControl)
            //if(_SelectedPlotControl.Content.GetType() != typeof(ConditionsIndividualPlotWrapper)) { return; }

            // ConditionsIndividualPlotWrapper theSelectedPlotWrapper = ((ConditionsIndividualPlotWrapper)_SelectedPlotControl.Content);
            //IndividualLinkedPlot theSelectedPlot = theSelectedPlotWrapper.LinkedPlot;

            //if (txt_XValue.Text == null || txt_XValue.Text == "")
            if (plotBasedOnXValue == false)
            {
                //if (txt_YValue.Text == null || txt_YValue.Text == "")
                {
                    //do nothing, there are no values
                }
               // else
                {
                    //x has nothing, and y has some number
                    if(Double.TryParse(txt_YValue.Text, out _SpecifiedYValue) == false) { return; }
                    //_SpecifiedYValue = Convert.ToDouble(txt_YValue.Text);
                    _SpecifiedXValue = _SelectedPlot.GetPairedValue(_SpecifiedYValue, false, _SelectedPlot.OxyPlot1.Model, isAxisReversed);
                    if (_SpecifiedYValue > _SelectedPlot.OxyPlot1.Model.Axes[1].Maximum || _SpecifiedYValue < _SelectedPlot.OxyPlot1.Model.Axes[1].Minimum)
                    {
                        //FdaViewModel.Utilities.CustomMessageBoxVM vm = new FdaViewModel.Utilities.CustomMessageBoxVM(FdaViewModel.Utilities.CustomMessageBoxVM.ButtonsEnum.OK, "Y Value is out of range");
                        MessageBox.Show("Y Value is out of range", "Out of Range");
                        txt_YValue.Text = "";
                        return;
                    }
                    txt_XValue.Text = Math.Round(_SpecifiedXValue, 3).ToString();
                }
            }
            else
            {
                //x has a number and we don't care what y is
                
                if(Double.TryParse(txt_XValue.Text,out _SpecifiedXValue) == false)
                {
                    //MessageBox.Show("X Value is not a valid number", "Not A Number");
                    txt_XValue.Text = "";
                    return;
                }
                //_SpecifiedXValue = Convert.ToDouble(txt_XValue.Text);

                //I need a way to know which plot this is so that i can put the right arguments into this method
                _SpecifiedYValue = ((IndividualLinkedPlot)_SelectedPlot).GetPairedValue(_SpecifiedXValue, true, _SelectedPlot.OxyPlot1.Model, isAxisReversed);
                if (_SpecifiedXValue > _SelectedPlot.OxyPlot1.Model.Axes[0].Maximum || _SpecifiedXValue < _SelectedPlot.OxyPlot1.Model.Axes[0].Minimum)
                {
                    //FdaViewModel.Utilities.CustomMessageBoxVM vm = new FdaViewModel.Utilities.CustomMessageBoxVM(FdaViewModel.Utilities.CustomMessageBoxVM.ButtonsEnum.OK, "X Value is out of range");
                    MessageBox.Show("X Value is out of range", "Out of Range");
                    txt_XValue.Text = "";
                    return;
                }
                txt_YValue.Text = Math.Round(_SpecifiedYValue, 3).ToString();
            }

        }


        //private void txt_XValue_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    TextBox box = (TextBox)sender;
        //    double doubleValue = 0;
        //    if (double.TryParse(box.Text, out doubleValue) == false && box.Text != "-" && box.Text != ".")
        //    {
        //        if (box.Text.Length == 1) { box.Text = ""; }
        //    }


        //}

        
        #endregion

        private void ConditionsPlotEditorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ////anything that is popped out is getting popped back in when tabs change or when popping back in from being a popped out window
            //FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;
            //if (vm.Plot1ControlVM.CurrentVM.GetType() == typeof(FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM))
            //{
            //    PopPlot1Out(this, new EventArgs());
            //}
            //else if (vm.Plot1ControlVM.CurrentVM.GetType() == typeof(FdaViewModel.Conditions.AddInflowOutflowToConditionVM))
            //{
            //    PopPlot1ImporterOut(this, new EventArgs());
            //}
            //else if (vm.Plot1ControlVM.CurrentVM.GetType() == typeof(FdaViewModel.Plots.IndividualLinkedPlotCoverButtonVM))
            //{
            //    FourPlotGrid.ColumnDefinitions[2].Width = new GridLength(0);

            //    mainGrid.RowDefinitions[1].Height = new GridLength(.4, GridUnitType.Star);
            //}
        }

        private void txt_XValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if(cmb_PlotNames.SelectedItem == null) { return; }
            FdaViewModel.Plots.IndividualLinkedPlotControlVM control = (FdaViewModel.Plots.IndividualLinkedPlotControlVM)cmb_PlotNames.SelectedItem;
            //i need to find the corresponding individualLinkedPlot
            foreach(ILinkedPlot plot in TheAddedPlots)
            {
                if(plot.BaseFunction.FunctionType ==  control.IndividualPlotWrapperVM.PlotVM.BaseFunction.FunctionType)
                {
                    _SelectedPlot = (IndividualLinkedPlot)plot;
                    break;
                }
            }

            if(_SelectedPlot == null) { return; }
            // if (cmb_PlotNames.SelectedIndex == -1) { return; }
            //_SelectedPlot = (IndividualLinkedPlot)cmb_PlotNames.SelectedItem;
            _SelectedPlot.TrackerIsOutsideTheCurveRange = false;
            FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM vm = (FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM)_SelectedPlot.DataContext;
            vm.PlotIsInsideRange(this, new EventArgs());

            if (_SelectedPlot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency ||
                    _SelectedPlot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency ||
                    _SelectedPlot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
                {
                    GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(true, true);
                }
                else
                {
                    GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(false, true);

                }
                ScreenPoint position = _SelectedPlot.OxyPlot1.Model.Axes[0].Transform(_SpecifiedXValue, _SpecifiedYValue, _SelectedPlot.OxyPlot1.Model.Axes[1]);

                //if the trackers are already frozen, unfreeze them
                if (_SelectedPlot.FreezeNextTracker == true)
                {
                    _SelectedPlot.Model_MouseDown(new object(), new OxyMouseDownEventArgs());
                }
                _SelectedPlot.DisplayTheTrackers(position);
                //now that the trackers are displayed, freeze them
                if (_SelectedPlot.FreezeNextTracker == false)
                {
                    _SelectedPlot.Model_MouseDown(new object(), new OxyMouseDownEventArgs());
                }





            
        }

        private void txt_YValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (cmb_PlotNames.SelectedItem == null) { return; }

            // if (cmb_PlotNames.SelectedIndex == -1) { return; }
            //_SelectedPlot = (IndividualLinkedPlot)cmb_PlotNames.SelectedItem;
            FdaViewModel.Plots.IndividualLinkedPlotControlVM control = (FdaViewModel.Plots.IndividualLinkedPlotControlVM)cmb_PlotNames.SelectedItem;
            //i need to find the corresponding individualLinkedPlot
            foreach (ILinkedPlot plot in TheAddedPlots)
            {
                if (plot.BaseFunction.FunctionType == control.IndividualPlotWrapperVM.PlotVM.BaseFunction.FunctionType)
                {
                    _SelectedPlot = (IndividualLinkedPlot)plot;
                    break;
                }
            }

            if (_SelectedPlot == null) { return; }
            _SelectedPlot.TrackerIsOutsideTheCurveRange = false;
            FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM vm = (FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM)_SelectedPlot.DataContext;
            vm.PlotIsInsideRange(this, new EventArgs());

            if (_SelectedPlot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency ||
                    _SelectedPlot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency ||
                    _SelectedPlot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
                {
                    GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(true, false);
                }
                else
                {
                    GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(false, false);

                }
                ScreenPoint position = _SelectedPlot.OxyPlot1.Model.Axes[0].Transform(_SpecifiedXValue, _SpecifiedYValue, _SelectedPlot.OxyPlot1.Model.Axes[1]);

                //if the trackers are already frozen, unfreeze them
                if (_SelectedPlot.FreezeNextTracker == true)
                {
                    _SelectedPlot.Model_MouseDown(new object(), new OxyMouseDownEventArgs());
                }
                _SelectedPlot.DisplayTheTrackers(position);
                //now that the trackers are displayed, freeze them
                if (_SelectedPlot.FreezeNextTracker == false)
                {
                    _SelectedPlot.Model_MouseDown(new object(), new OxyMouseDownEventArgs());
                }





            
        }

       
    }
}
