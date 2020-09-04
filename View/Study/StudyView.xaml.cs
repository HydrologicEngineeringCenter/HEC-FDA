using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace View.Study
{
    /// <summary>
    /// Interaction logic for StudyView.xaml
    /// </summary>
    public partial class StudyView : UserControl
    {




        public StudyView()
        {

            InitializeComponent();



            //MapWindow.MapWindow.TreeView = MapTreeView;
            //MapTreeView.MapWindow = MapWindow.MapWindow;

            //MapToolBar.MapTree = MapTreeView;
            //SelectableLayers.MapTree = MapTreeView;

            //FeatureEditorToolbar.MapTree = MapTreeView;

            //////////////////////////////////////////

            //MapToolBar.MapWindow = MapWindow.MapWindow;

            //SelectableLayers.MapWindow = MapWindow.MapWindow;
            //FeatureEditorToolbar.MapWindow = MapWindow.MapWindow;
            //StatusBorder.Child = new OpenGLMapping.MapStatusBar(MapWindow.MapWindow);

            //RadioButton ArrowTool = (RadioButton)MapToolBar.Items[0];
            //ArrowTool.IsChecked = true;
            //MapToolBar.RadioChecked += MapToolBar_RadioChecked;

            //FeatureEditorToolbar.RadioChecked += FeatureEditorToolbar_RadioChecked;

            //MapWindow.MapWindow.MouseLeave += MapWindow_Mouse;
            //MapWindow.MapWindow.MouseEnter += MapWindow_Mouse;
        }

        //private void MapToolBar_RadioChecked(RadioButton buttonChecked)
        //{
        //    foreach (object item in FeatureEditorToolbar.Items)
        //    {
        //        RadioButton r = item as RadioButton;
        //        if (r != null)
        //        {
        //            r.IsChecked = false;
        //        }
        //    }
        //}

        //private void MapWindow_Mouse(object sender, EventArgs e)
        //{
        //    FdaViewModel.Study.FdaStudyVM vm = (FdaViewModel.Study.FdaStudyVM)this.DataContext;
        //    vm.MapTreeView = MapTreeView;
        //    vm.MapWindow = MapWindow.MapWindow;

        //    MapWindow.MapWindow.TreeView = vm.MapTreeView;
        //    MapTreeView.MapWindow = vm.MapWindow;//MapWindow.MapWindow;

        //    MapToolBar.MapTree = vm.MapTreeView;
        //    SelectableLayers.MapTree = vm.MapTreeView;

        //    FeatureEditorToolbar.MapTree = vm.MapTreeView;



        //    object fe = FocusManager.GetFocusedElement(this);
        //    if (fe == null) return;
        //    if (fe.GetType() == typeof(OpenGLMapping.WinFormsHostControl)) { MapWindow.MapWindow.Focus(); }




        //}
        //private void FeatureEditorToolbar_RadioChecked(RadioButton buttonChecked)
        //{
        //    if (buttonChecked == null)
        //    {
        //        MapToolBar.SelectChecked();
        //    }
        //    else
        //    {
        //        foreach (object item in MapToolBar.Items)
        //        {
        //            RadioButton r = item as RadioButton;
        //            if (r != null)
        //            {
        //                r.IsChecked = false;
        //            }
        //        }
        //    }
        //}

        //private void btn_CloseTab_Click(object sender, RoutedEventArgs e)
        //{
        //    DependencyObject currentControl = (DependencyObject)sender;
        //    Type targetType = typeof(TabItem);

        //    while (currentControl != null)
        //    {
        //        if(currentControl.GetType() == targetType)
        //        {
        //            DynamicTabControl.SelectedItem = currentControl;
        //        }
        //        else
        //        {
        //            currentControl = LogicalTreeHelper.GetParent(currentControl);
        //        }
        //    }
        //    FdaViewModel.Study.FdaStudyVM vm = (FdaViewModel.Study.FdaStudyVM)this.DataContext;
        //    if (vm.Tabs[DynamicTabControl.SelectedIndex].CanDelete == true)
        //    {
        //        //vm.Tabs.RemoveAt(DynamicTabControl.SelectedIndex);
        //    }

        //}

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Study.FdaStudyVM vm = (FdaViewModel.Study.FdaStudyVM)this.DataContext;
            //vm.MWMTVConn. MapTreeView = MapTreeView;
            vm.AddCreateNewStudyTab();
            vm.AddMapsTab(MapTreeView);

            //MapWindowControl.MapToolBar.MapWindow = MapWindowControl.MapWindow.MapWindow;

            //MapWindowControl.SelectableLayers.MapWindow = MapWindowControl.MapWindow.MapWindow;
            //MapWindowControl.FeatureEditorToolbar.MapWindow = MapWindowControl.MapWindow.MapWindow;
            //MapWindowControl.StatusBorder.Child = new OpenGLMapping.MapStatusBar(MapWindowControl.MapWindow.MapWindow);

            //RadioButton ArrowTool = (RadioButton)MapWindowControl.MapToolBar.Items[0];
            //ArrowTool.IsChecked = true;
            //MapWindowControl.MapToolBar.RadioChecked += MapWindowControl.MapToolBar_RadioChecked;

            //MapWindowControl.FeatureEditorToolbar.RadioChecked += MapWindowControl.FeatureEditorToolbar_RadioChecked;

            //MapWindowControl.MapWindow.MapWindow.MouseLeave += MapWindowControl.MapWindow_Mouse;
            //MapWindowControl.MapWindow.MapWindow.MouseEnter += MapWindowControl.MapWindow_Mouse;

            ////FdaViewModel.Utilities.MapWindowControlVM vm = (FdaViewModel.Utilities.MapWindowControlVM)this.DataContext;
            //// vm.MWMTVConn.MapWindow = MapWindowControl.MapWindow;

            //// MapWindowControl.MapWindow.TreeView = vm.MWMTVConn.MapTreeView;
            //MapWindowControl.MapWindow.MapWindow.TreeView = MapTreeView;
            //MapTreeView.MapWindow = MapWindowControl.MapWindow.MapWindow;
            ////vm.MWMTVConn.MapTreeView.MapWindow = MapWindowControl.MapWindow;

            //MapWindowControl.MapToolBar.MapTree = MapTreeView;
            //MapWindowControl.SelectableLayers.MapTree = MapTreeView;

            //MapWindowControl.FeatureEditorToolbar.MapTree = MapTreeView;


        }

        private void lbl_Study_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            FdaViewModel.Study.FdaStudyVM vm = (FdaViewModel.Study.FdaStudyVM)this.DataContext;

            //lbl_Study.ContextMenu = vm.StudyElement.Actions;
            lbl_Study.ContextMenu.IsOpen = true;
        }


        public bool WasXClicked { get; set; } = false;
        public bool WasPopOutClicked { get; set; } = false;
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WasXClicked = true;
        }



        private void txt_PopOut_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WasPopOutClicked = true;
        }

        private void btn_MapView_Click(object sender, RoutedEventArgs e)
        {
            //MapWindowControl .Visibility = Visibility.Visible;
            //MapWindowControl.MapWindow.MapWindow.PlotFeatures();
            //DynamicTabControl.Visibility = Visibility.Hidden;
        }

        private void btn_TabsView_Click(object sender, RoutedEventArgs e)
        {
            //MapWindowControl.Visibility = Visibility.Hidden;
            //DynamicTabControl.Visibility = Visibility.Visible;
        }

        private void DynamicTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FdaViewModel.Study.FdaStudyVM vm = (FdaViewModel.Study.FdaStudyVM)this.DataContext;
        

            System.Timers.Timer updateMapWindowTimer;
            updateMapWindowTimer = new System.Timers.Timer(100);
            //updateMapWindowTimer.Elapsed += OnTimedEvent;
            updateMapWindowTimer.AutoReset = false;
            updateMapWindowTimer.Enabled = true;
        }

        //private void OnTimedEvent(Object source, ElapsedEventArgs e)
        //{
        //    Dispatcher.Invoke(new Action(() =>
        //    {
        //        FdaViewModel.Study.FdaStudyVM vm = (FdaViewModel.Study.FdaStudyVM)this.DataContext;
        //        //todo: testing only
        //        vm.UpdateMapTabTest();
        //    }));
        //}
    }
}
