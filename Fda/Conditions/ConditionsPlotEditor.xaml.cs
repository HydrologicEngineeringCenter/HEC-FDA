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

namespace Fda.Conditions
{
    /// <summary>
    /// Interaction logic for ConditionsPlotEditor.xaml
    /// </summary>
    public partial class ConditionsPlotEditor : UserControl
    {
       

        public bool HideTrackers { get; set; }
        public bool Plot1PoppedOut { get; set; }
        public bool Plot1DoesntExist { get; set; }
        public bool Plot5PoppedOut { get; set; }
        public bool Plot5DoesntExist { get; set; }
        public bool AreaPlotsHaveBeenRemoved { get; set; }

        public List<Plots.ILinkedPlot> AddedPlots { get; set; }

        

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


            DLMControl.PopImporterIntoPlot1 += new EventHandler(PopPlot1ImporterOut);
            DLMControl.PopPlotIntoPlot1 += new EventHandler(PopPlot1Out);

            DLMHorizontalControl.PopImporterIntoPlot5 += new EventHandler(PopPlot5ImporterOut);
            DLMHorizontalControl.PopPlotIntoPlot5 += new EventHandler(PopPlot5Out);

            FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;

            AddedPlots = new List<Plots.ILinkedPlot>();
            //doubleLineModulator.PopOutThePlot += new EventHandler(PopPlot1Out);
            //DoubleLineHorizontal.PopOutThePlot += new EventHandler(PopPlot5Out);

            //plot0.ChangeThisCurve += new EventHandler(vm.LaunchAddInflowFrequencyCurve);

            //FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;
            
        }

        
        public void UpdateThePlotLinkages(object sender, EventArgs e)
        {
            UpdateTheListOfAddedCurves();
            UpdateTheLinkages();
        }
        private void UpdateTheListOfAddedCurves()
        {

            // i think maybe i should move this list to the VM side of things?
            // instead of looking at the ".selectedCurve" i should maybe have a bool or something
            //that way when i have plot 1 and DLM i can set one to "don't use" and the other to "Use".
           


            AddedPlots = new List<Plots.ILinkedPlot>();

            if(Plot0Control.LinkedPlot != null)
            {
                AddedPlots.Add(Plot0Control.LinkedPlot);
            }
            if (Plot1Control.LinkedPlot != null && Plot1PoppedOut == true)
            {
                AddedPlots.Add(Plot1Control.LinkedPlot);
            }
            else if (DLMControl.LinkedPlot != null && Plot1PoppedOut == false)
            {
                AddedPlots.Add(DLMControl.LinkedPlot);
            }

            if (Plot3Control.LinkedPlot != null)
            {
                AddedPlots.Add(Plot3Control.LinkedPlot);
            }

            if(Plot5Control.LinkedPlot != null && Plot5PoppedOut == true)
            {
                AddedPlots.Add(Plot5Control.LinkedPlot);
            }
            else if (DLMHorizontalControl.LinkedPlot != null && Plot5PoppedOut == false)
            {
                AddedPlots.Add(DLMHorizontalControl.LinkedPlot);
            }

            if (Plot7Control.LinkedPlot != null)
            {
                AddedPlots.Add(Plot7Control.LinkedPlot);
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
            vm.RunPreviewCompute();

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
            UpdateTheLinkages();

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
            FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;

            if (Plot1PoppedOut == false || Plot5PoppedOut == false)
            {
                if (vm.IsPlot1Visible == true)
                {
                    PopPlot1Out(sender, e);
                }
                if (vm.IsPlot5Visible == true)
                {
                    PopPlot5Out(sender, e);
                }
                //change the image to pop in and change the tooltip
                //img_PopPlotsOut.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/PopDown.png"));
                //btn_PopPlotsOut.ToolTip = "Pop Plots In";
            }
            else
            {
                if (vm.IsPlot1Visible == true)
                {
                    btn_CollapsePlot1_Click(sender, e);
                }
                if (vm.IsPlot5Visible == true)
                {
                    btn_CollapsePlot5_Click(sender, e);
                }
                //change the image to pop out and change the tooltip
                //img_PopPlotsOut.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/PopUp.png"));
                //btn_PopPlotsOut.ToolTip = "Pop Plots Out";

            }
        }

        #endregion

        private void btn_HideTrackers_Click(object sender, RoutedEventArgs e)
        {
            if (HideTrackers == false)
            {
                //plot0.HideTracker();
                //plot1.HideTracker();
                //plot3.HideTracker();
               // plot5.HideTracker();
                //plot7.HideTracker();
               // plot8.HideTracker();
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
        private void UpdateTheLinkages()
        {
            //FdaViewModel.Conditions.ConditionsPlotEditorVM vm = (FdaViewModel.Conditions.ConditionsPlotEditorVM)this.DataContext;
            

            if (AddedPlots.Count == 0) { return; }
            List<Plots.ILinkedPlot> sortedList = AddedPlots.OrderBy(o => o.BaseFunction.FunctionType).ToList();

            //********  clear all the startnodes and endnodes  ************
            foreach(Plots.ILinkedPlot p in sortedList)
            {
                p.ThisIsStartNode = false;
                p.ThisIsEndNode = false;
            }

            // *******    set up the first plot *********
            if(sortedList.Count == 0) { return; }
            sortedList[0].ThisIsStartNode = true;
            if (sortedList.Count == 1)
            {
                sortedList[0].ThisIsEndNode=true;
                return;
            }

            sortedList[0].SetNextPlotLinkage(sortedList[1], "", "");


            //**********   set the linkages for all but the last plot
            for(int i = 1;i<sortedList.Count-1;i++)
            {
               
                sortedList[i].SetNextPlotLinkage(sortedList[i + 1],"","");
                sortedList[i].SetPreviousPlotLinkage(sortedList[i-1]);

            }
            //*********** set the last plot linkage
            sortedList[sortedList.Count - 1].SetPreviousPlotLinkage(sortedList[sortedList.Count-2]);
            sortedList[sortedList.Count - 1].ThisIsEndNode = true;

           

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
            
        }
        private bool IsThereAFlowFrequencyAndRatingCurves()
        {
            bool flowFrequencyExists = false;
            bool ratingExists = false;
            foreach (Plots.ILinkedPlot p in AddedPlots)
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


        private bool IsThereARatingAndStageDamageCurve()
        {
            bool stageDamageExists = false;
            bool ratingExists = false;
            foreach (Plots.ILinkedPlot p in AddedPlots)
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

        private void btn_ToggleAreaPlots_Click(object sender, RoutedEventArgs e)
        {
            //if (AreaPlotsHaveBeenRemoved == false)
            //{
            //    plot0.RemoveAreaPlots();
            //    plot3.RemoveAreaPlots();
            //    plot7.RemoveAreaPlots();
            //    plot8.RemoveAreaPlots();
            //    AreaPlotsHaveBeenRemoved = true;
            //    //change the image and change the tooltip
            //    img_HideAreaPlots.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/ShowAreaPlots.png"));
            //    btn_ToggleAreaPlots.ToolTip = "Show Area Plots";
            //}
            //else
            //{
            //    plot0.AddAreaPlots();
            //    plot3.AddAreaPlots();
            //    plot7.AddAreaPlots();
            //    plot8.AddAreaPlots();
            //    AreaPlotsHaveBeenRemoved = false;
            //    //change the image and change the tooltip
            //    img_HideAreaPlots.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/HideAreaPlots.png"));
            //    btn_ToggleAreaPlots.ToolTip = "Hide Area Plots";
            //}
        }
    }
}
