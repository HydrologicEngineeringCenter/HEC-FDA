using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Watershed;
using HEC.FDA.ViewModel.WaterSurfaceElevation;
using OpenGLMapping;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HEC.FDA.View
{
    /// <summary>
    /// Interaction logic for ViewWindow.xaml
    /// </summary>
    public partial class ViewWindow : Window
    {
        private bool _CanDragIntoTab;
        private bool _MousePressed;
        private static ViewWindow _MainWindow;

        public ViewWindow()
        {
            //for some reason this only gets called once, but the loaded gets called every time. wierd.
            InitializeComponent();
            //set this window as the "main window" so that the tabs can be dragged back into it
            //Every window is using this class so we only want to set this the first time
            if (_MainWindow == null)
            {
                _MainWindow = this;
            }


   

            WindowVM vm = (WindowVM)this.DataContext;
            Title = vm.Title;

            FdaStudyVM test = (FdaStudyVM)vm.CurrentView;

            test.RequestShapefilePaths += RequestShapefilePaths;
            test.RequestShapefilePathsOfType += RequestShapefilePathsOfType;
            test.RequestAddToMapWindow += RequestAddToMapWindow;
            test.RequestRemoveFromMapWindow += RequestRemoveFromMapWindow;
            vm.LaunchNewWindow += WindowSpawner;
            Closing += vm.OnClosing;

            
        }


        private void RequestAddToMapWindow(object sender, AddMapFeatureEventArgs args)
        {
            Study.StudyView sv = GetTheVisualChild<Study.StudyView>(masterControl);
            if (sv == null) { return; }


            MapTreeView mtv = sv.MapTreeView;
            if (args.GetType().Name == nameof(AddGriddedDataEventArgs))
            {
                AddGriddedDataEventArgs gargs = args as AddGriddedDataEventArgs;
                MapRaster mapRaster = new MapRaster(gargs.Features, gargs.Ramp, args.FeatureName, mtv.MapWindow);
                RasterFeatureNode rfn = new RasterFeatureNode(mapRaster, args.FeatureName);
                mtv.AddGisData(rfn, 0, true);
                if (sender.GetType().Name == nameof(TerrainElement))
                {
                    args.MapFeatureHash = rfn.GetHashCode();
                    rfn.RemoveLayerCalled += ((TerrainElement)sender).removedcallback;
                }
                if (sender.GetType().Name == nameof(WaterSurfaceElevationElement))
                {
                    args.MapFeatureHash = rfn.GetHashCode();
                    rfn.RemoveLayerCalled += ((WaterSurfaceElevationElement)sender).removedcallback;
                }

            }
            else if (args.GetType().Name == nameof(AddShapefileEventArgs))
            {
                AddShapefileEventArgs sargs = args as AddShapefileEventArgs;

                if (sargs.Features.GetType() == typeof(LifeSimGIS.PolygonFeatures))
                {
                    LifeSimGIS.PolygonFeatures polyFeatures = (LifeSimGIS.PolygonFeatures)sargs.Features;
                    OpenGLDrawSingle drawInfo = new OpenGLMapping.OpenGLDrawSingle(sargs.DrawInfo);
                    MapPolygons mapPolys = new MapPolygons(polyFeatures, sargs.Attributes, sargs.FeatureName, drawInfo, mtv.MapWindow);
                    VectorFeatureNode vfn = new VectorFeatureNode(mapPolys, sargs.FeatureName);

                    mtv.AddGisData(vfn, 0, true);
                    args.MapFeatureHash = vfn.GetHashCode();
                    vfn.RemoveLayerCalled += ((ImpactAreaElement)sender).removedcallback;
                }
                else if (sargs.Features.GetType() == typeof(LifeSimGIS.PointFeatures))
                {
                    VectorFeatureNode vfn = new VectorFeatureNode(new MapPoints((LifeSimGIS.PointFeatures)sargs.Features, sargs.Attributes, sargs.FeatureName, new OpenGLMapping.OpenGLDrawSingle(sargs.DrawInfo), mtv.MapWindow), sargs.FeatureName);
                    mtv.AddGisData(vfn, 0, true);
                    args.MapFeatureHash = vfn.GetHashCode();
                    vfn.RemoveLayerCalled += ((InventoryElement)sender).removedcallback;
                }
                else if (sargs.Features.GetType() == typeof(LifeSimGIS.LineFeatures))
                {

                }
            }
            else if (args.GetType().Name == nameof(OpenStructureAttributeTableEventArgs))
            {
                OpenStructureAttributeTable(sender, sv, args);
            }

        }

        private VectorFeatureNode GetMapTabFeature(int hashCode, MapTreeView mtv)
        {
            List<VectorFeatureNode> nodes = mtv.GetVectorFeatureNodes();
            foreach (VectorFeatureNode node in nodes)
            {
                if (node.GetHashCode() == hashCode)
                {
                    return node;
                }
            }
            return null;
        }

        private void OpenStructureAttributeTable(object sender, HEC.FDA.View.Study.StudyView sv, AddMapFeatureEventArgs args)
        {
            MapTreeView mtv = sv.MapTreeView;
            VectorFeatureNode nodeInMapTab = GetMapTabFeature(args.MapFeatureHash, mtv);
            if(nodeInMapTab != null)
            {
                nodeInMapTab.Features.OpenAttributes();
                return;
            }
            else
            {
                OpenStructureAttributeTableEventArgs sargs = args as OpenStructureAttributeTableEventArgs;

                if (sargs.Features.GetType() == typeof(LifeSimGIS.PointFeatures))
                {
                    MapPoints mapPoints = new MapPoints((LifeSimGIS.PointFeatures)sargs.Features, sargs.Attributes, sargs.FeatureName, new OpenGLMapping.OpenGLDrawSingle(sargs.DrawInfo), mtv.MapWindow);
                    mapPoints.OpenAttributes();
                }
            }



            //OpenStructureAttributeTableEventArgs sargs = args as OpenStructureAttributeTableEventArgs;

            ////if (sender.GetType().Name == nameof(HEC.FDA.ViewModel.ImpactArea.ImpactAreaElement))
            //if (sargs.Features.GetType() == typeof(LifeSimGIS.PolygonFeatures))
            //{
            //    LifeSimGIS.PolygonFeatures polyFeatures = (LifeSimGIS.PolygonFeatures)sargs.Features;
            //    OpenGLDrawSingle drawInfo = new OpenGLMapping.OpenGLDrawSingle(sargs.DrawInfo);
            //    MapPolygons mapPolys = new MapPolygons(polyFeatures, sargs.Attributes, sargs.FeatureName, drawInfo, mtv.MapWindow);
            //    VectorFeatureNode vfn = new VectorFeatureNode(mapPolys, sargs.FeatureName);

            //    mtv.AddGisData(vfn, 0, true);
            //    args.MapFeatureHash = vfn.GetHashCode();
            //    vfn.RemoveLayerCalled += ((HEC.FDA.ViewModel.ImpactArea.ImpactAreaElement)sender).removedcallback;
            //}
            ////else if (sender.GetType().Name == nameof(HEC.FDA.ViewModel.Inventory.InventoryElement))
            //else if (sargs.Features.GetType() == typeof(LifeSimGIS.PointFeatures))
            //{
            //    MapPoints mapPoints = new MapPoints((LifeSimGIS.PointFeatures)sargs.Features, sargs.Attributes, sargs.FeatureName, new OpenGLMapping.OpenGLDrawSingle(sargs.DrawInfo), mtv.MapWindow);
            //    mapPoints.OpenAttributes();
            //}
            //else if (sargs.Features.GetType() == typeof(LifeSimGIS.LineFeatures))
            //{

            //}
        }


        private void removedcallback(FeatureNodeHeader node, bool includeSelected)
        {
            throw new NotImplementedException();
        }

        private void RequestRemoveFromMapWindow(object sender, RemoveMapFeatureEventArgs args)
        {
            HEC.FDA.View.Study.StudyView sv = GetTheVisualChild<HEC.FDA.View.Study.StudyView>(masterControl);
            if (sv == null) { return; }
            OpenGLMapping.MapTreeView mtv = sv.MapTreeView;
            foreach (OpenGLMapping.FeatureNodeHeader f in mtv.GetAllFeatureNodes())
            {
                if (f.GetHashCode() == args.FeatureHashCode)
                {
                    f.RemoveLayerEventRaiser(true);
                    return;
                }
            }
        }
        private void RequestShapefilePaths(ref System.Collections.Generic.List<string> files)
        {
            HEC.FDA.View.Study.StudyView sv = GetTheVisualChild<HEC.FDA.View.Study.StudyView>(masterControl);
            if (sv == null) { return; }
            OpenGLMapping.MapTreeView mtv = sv.MapTreeView;
            List<OpenGLMapping.VectorFeatureNode> v = mtv.GetVectorFeatureNodes();
            foreach (OpenGLMapping.VectorFeatureNode fn in v)
            {
                files.Add(fn.GetBaseFeature.SourceFile);
            }
        }
        private void RequestShapefilePathsOfType(ref System.Collections.Generic.List<string> files, VectorFeatureType featureType)
        {
            HEC.FDA.View.Study.StudyView sv = GetTheVisualChild<HEC.FDA.View.Study.StudyView>(masterControl);
            if (sv == null) { return; }
            OpenGLMapping.MapTreeView mtv = sv.MapTreeView;
            List<OpenGLMapping.VectorFeatureNode> v = mtv.GetVectorFeatureNodes();
            foreach (OpenGLMapping.VectorFeatureNode fn in v)
            {
                switch (featureType)
                {
                    case VectorFeatureType.Point:
                        if (fn.GetBaseFeature.GetType() == typeof(OpenGLMapping.MapPoints))
                        {
                            files.Add(fn.GetBaseFeature.SourceFile);
                        }
                        break;
                    case VectorFeatureType.Line:
                        if (fn.GetBaseFeature.GetType() == typeof(OpenGLMapping.MapLines))
                        {
                            files.Add(fn.GetBaseFeature.SourceFile);
                        }
                        break;
                    case VectorFeatureType.Polygon:
                        if (fn.GetBaseFeature.GetType() == typeof(OpenGLMapping.MapPolygons))
                        {
                            files.Add(fn.GetBaseFeature.SourceFile);
                        }
                        break;
                    default:
                        break;
                }


            }
        }
        public ViewWindow(WindowVM newvm)
        {
            InitializeComponent();
            DataContext = newvm;
            Title = newvm.Title;
            newvm.LaunchNewWindow += WindowSpawner;
            Closing += newvm.OnClosing;

        }
        private void btn_PopWindowInToTabs_Click(object sender, RoutedEventArgs e)
        {
            WindowVM vm = (WindowVM)this.DataContext;
            IDynamicTab tab = vm.Tab;
            tab.PopWindowIntoTab();

            Close();
        }


        private void WindowSpawner(WindowVM newvm, bool asDialogue)
        {
            newvm.WasCanceled = true;

            newvm.Scalable = false;
            ViewWindow newwindow = new ViewWindow(newvm);
            newwindow.Owner = this;

            //hide the top row with the pop in button if this vm doesn't support that
            if (newvm.Tab.CanPopOut == false)
            {
                // newwindow.MainGrid.RowDefinitions[0].Height = new GridLength(0);
            }

            if (asDialogue)
            {
                
                newwindow.ShowDialog();
            }
            else
            {

                if (newvm.Tab.IsDragging)
                {
                    //have the new window show up where the mouse is
                    newwindow.WindowStartupLocation = WindowStartupLocation.Manual;
                    newwindow.Left = PointToScreen(Mouse.GetPosition(null)).X - 80;
                    newwindow.Top = PointToScreen(Mouse.GetPosition(null)).Y - 10;
                }
                newwindow.Show();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (aTimer != null)
            {
                aTimer.Enabled = false;
                aTimer.Close();
            }
            WindowVM vm = (WindowVM)this.DataContext;
            if (vm.CurrentView.GetType() == typeof(FdaStudyVM))
            {
                FdaStudyVM studyVM = (FdaStudyVM)vm.CurrentView;
                studyVM.Dispose();
            }
            else
            {
                //this remove window call will call the IsOkToClose() method on the base vm.
                vm.Tab.RemoveWindow();
                
                
            }
            vm.Dispose();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //BaseViewModel vm = (BaseViewModel) this.DataContext;
            //Width = vm.Width;
            //Height = vm.Height;
            //if(masterControl.Content is UserControl)
            //{
            //    UserControl uc = (UserControl)masterControl.Content;
            //    this.Width = uc.Width;
            //    this.Height = uc.Height;
            //}
            //if(masterControl.Content.GetType() == typeof(Utilities.IPopOut))
            //{
            //    this.Width = ((Utilities.IPopOut)masterControl.Content).PopOutWidth;
            //}

            // if (this.DataContext.GetType() != typeof(HEC.FDA.ViewModel.Study.FdaStudyVM))
            // {
            //this.SizeToContent = SizeToContent.WidthAndHeight;
            //then we want the window to size to the content better.
            //masterControl.Content.
            //this.Height = this.DesiredSize.Height;
            //this.MinHeight = this.DesiredSize.Height;
            //this.MinWidth = this.DesiredSize.Width;
            //this.Width = this.DesiredSize.Width;
            //need to figure out how to set max widths and heights.
            //}


        }
        public T GetTheVisualChild<T>(Visual Parent) where T : Visual
        {
            T child = null;
            int NumVisuals = VisualTreeHelper.GetChildrenCount(Parent);
            for (int i = 0; i < NumVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(Parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetTheVisualChild<T>(v);
                }
                else
                {
                    return child;
                }
            }
            return child;
        }
        private void MaximizeWindow(object sender, RoutedEventArgs e)
        {
            //if (this.WindowState == WindowState.Maximized)
            //{
            //    this.WindowState = System.Windows.WindowState.Normal;
            //    MaximizeButton.Content = new Image() { Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("pack://application:,,,/View;component/Resources/Maximize.png")) };
            //    MaximizeButton.ToolTip = "Maximize";
            //}
            //else
            //{
            //    this.WindowState = System.Windows.WindowState.Maximized;
            //    MaximizeButton.ToolTip = "Restore";
            //    MaximizeButton.Content = new Image() { Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("pack://application:,,,/View;component/Resources/Restore.png")) };
            //}

        }
        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
        //private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        //{
        //    Top += e.VerticalChange;
        //    Left += e.HorizontalChange;
        //}
        //private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    if (e.ClickCount >= 2) { Close(); }
        //}

        //private void Image_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    ContextMenu c = new ContextMenu();
        //    MenuItem min = new MenuItem();
        //    min.Header = "Minimize";
        //    min.Icon = "  __";
        //    min.Click += MinimizeWindow;

        //    MenuItem max = new MenuItem();
        //    if (this.WindowState == WindowState.Maximized)
        //    {
        //        max.Header = "Restore";
        //        max.Icon = new Image() { Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("pack://application:,,,/View;component/Resources/Restore.png")) };
        //    }
        //    else
        //    {
        //        max.Header = "Maximize";
        //        max.Icon = new Image() { Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("pack://application:,,,/View;component/Resources/Maximize.png")) };
        //    }

        //    max.Click += MaximizeWindow;

        //    MenuItem close = new MenuItem();
        //    close.Header = "Close";
        //    close.Icon = "  X";
        //    close.Click += CloseWindow;
        //    c.Items.Add(min);
        //    c.Items.Add(max);
        //    c.Items.Add(close);
        //    this.ContextMenu = c;

        //}

        private void Window_StateChanged(object sender, EventArgs e)
        {
            //ViewModel.Study.FdaStudyVM vm = (HEC.FDA.ViewModel.Study.FdaStudyVM)this.DataContext;
            ////vm.MWMTVConn. MapTreeView = MapTreeView;
            //if (vm.MWMTVConn != null)
            //{
            //    vm.MWMTVConn.UpdateMapWindow();
            //}
        }

        private void Window_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowVM vm = (WindowVM)this.DataContext;
            if (vm != null && vm.Tab != null && vm.Tab.IsDragging)
            {
                vm.Tab.IsDragging = false;
            }
        }



        //public const int WM_NCLBUTTONDOWN = 0xA1;
        //public const int HT_CAPTION = 0x2;

        //[DllImportAttribute("user32.dll")]
        //public static extern int SendMessage(IntPtr hWnd, int Msg,
        //        int wParam, int lParam);

        //[DllImportAttribute("user32.dll")]
        //public static extern bool ReleaseCapture();



        #region TitleBarStuff
        public void move_window(object sender, MouseButtonEventArgs e)
        {
            //just do a normal drag move if it is the main window
            if (this == _MainWindow)
            {
                DragMove();
            }
            else
            {
                DoDraggingWithTimer();
            }
        }

        private void EXIT(object sender, MouseButtonEventArgs e)
        {
            Close();
            //Not sure why this was necessary but without it FDA wasull &&n't actually closing down. (Cody 1/28/2020)
            if (this == _MainWindow)
            {
                Environment.Exit(0);
            }
        }

        private void MINIMIZE(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MAX_RESTORE(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Normal) this.WindowState = WindowState.Maximized;
            else this.WindowState = WindowState.Normal;
        }

        private void Activate_Title_Icons(object sender, MouseEventArgs e)
        {
            Close_btn.Fill = (ImageBrush)MainGrid.Resources["Close_act"];
            Min_btn.Fill = (ImageBrush)MainGrid.Resources["Min_act"];
            Max_btn.Fill = (ImageBrush)MainGrid.Resources["Max_act"];
        }

        private void Deactivate_Title_Icons(object sender, MouseEventArgs e)
        {
            Close_btn.Fill = (ImageBrush)MainGrid.Resources["Close_inact"];
            Min_btn.Fill = (ImageBrush)MainGrid.Resources["Min_inact"];
            Max_btn.Fill = (ImageBrush)MainGrid.Resources["Max_inact"];
        }

        private void Close_pressing(object sender, MouseButtonEventArgs e)
        {
            Close_btn.Fill = (ImageBrush)MainGrid.Resources["Close_pr"];
        }

        private void Min_pressing(object sender, MouseButtonEventArgs e)
        {
            Min_btn.Fill = (ImageBrush)MainGrid.Resources["Min_pr"];
        }

        private void Max_pressing(object sender, MouseButtonEventArgs e)
        {
            Max_btn.Fill = (ImageBrush)MainGrid.Resources["Max_pr"];
        }

        #endregion

   

        #region Popping windows into tabs

        private System.Timers.Timer aTimer;
        private Window WinTemp;  // It is temporary window  which include  TabControl with only one TabItem
        private bool _IsOverTabControl = false;

        private void DoDraggingWithTimer()
        {
            if (aTimer != null)
            {
                WinTemp = this;
                aTimer.Enabled = true;
                DragMove();
                TitleBarRectangle_MouseLeftButtonUp(null, null);
            }
            else
            {
                //then this is probably some little pop up dialog that we want the user to be able to drag
                DragMove();
            }
        }

        private void initTimer()
        {
            aTimer = new System.Timers.Timer(100);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = false;
        }

        // Find Parent Window - if WinTemp over specific region of parent window it will be parent ... apply opacity ...
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                //_MainWindow.Title = WinTemp.RestoreBounds.ToString();
                if (WinTemp != null)
                {
                    var tempY = WinTemp.RestoreBounds.Top;
                    var tempX = WinTemp.RestoreBounds.Left;

                    //int countList = CommonStaticLibrary.WinList.Count;
                    //for (int intW = 0; intW < countList; intW++)
                    {
                        //Window win = (Window)CommonStaticLibrary.WinList[intW];  // need to cast to Window - because MainWindow is MainWindow class.
                        Window win = _MainWindow;
                        var winCap = SystemParameters.CaptionHeight;
                        // Header Height for Current Window

                        Label lb;
                        TabControl tUc;

                        // Grid grd = win.Content as Grid;
                        //tUc = GetTabControlFromMainWindow();
                        //var ti = tUc.Items[0] as TabItem;
                        //lb = ti.Header as Label;

                        var tabHeaderH = 50; //lb.ActualHeight;

                        var winY = win.RestoreBounds.Top;
                        var winX = win.RestoreBounds.Left;
                        var winW = win.RestoreBounds.Width;

                        //CommonStaticLibrary.IntActiveWindow = -1;
                        DoWindowNotOverTabControlStyle();
                        //tUc.Opacity = 1.0;
                        _IsOverTabControl = false;
                        if ((tempY > winY + winCap) && (tempY < winY + winCap + tabHeaderH))
                        {
                            if ((tempX < winX + winW) && (tempX > winX))
                            {
                                DoWindowOverTabControlStyle();
                                //tUc.Opacity = 0.5;
                                // CommonStaticLibrary.IntActiveWindow = intW;
                                _IsOverTabControl = true;
                                return;
                            }
                        }
                    }
                }

            }));
        }

        private void DoWindowOverTabControlStyle()
        {
            TabControl mainTabControl = GetTabControlFromMainWindow();
            mainTabControl.Opacity = .5;
            mainTabControl.Background = Brushes.LightBlue;

            this.Opacity = .5;
            this.Background = Brushes.LightBlue;

            //Rectangle rec = new Rectangle()
            //{
            //    Width = (int)this.ActualWidth,
            //    Height = (int)this.ActualHeight,
            //    Fill = System.Drawing.Brushes.Green,
            //    Stroke = System.Drawing.Brushes.Red,
            //    StrokeThickness = 2,
            //};



            //Rectangle blueRectangle = new Rectangle();
            //blueRectangle.Height = (int)this.ActualHeight;
            //blueRectangle.Width = (int)this.ActualWidth;
            //Brush brush = new SolidColorBrush(Colors.LightGreen);
            //brush.Opacity = .75;

            //blueRectangle.Fill = brush;
            ////myCanvas.Height = ActualHeight;
            ////myCanvas.Width = ActualWidth;
            //myCanvas.Children.Add(blueRectangle);

        }

        private void DoWindowNotOverTabControlStyle()
        {
            TabControl mainTabControl = GetTabControlFromMainWindow();
            mainTabControl.Opacity = 1;
            mainTabControl.Background = SystemColors.WindowBrush;

            this.Opacity = 1;
            this.Background = SystemColors.WindowBrush;
        }

        private TabControl GetTabControlFromMainWindow()
        {
            HEC.FDA.View.Study.StudyView sv = GetTheVisualChild<HEC.FDA.View.Study.StudyView>(_MainWindow.masterControl);
            if (sv == null)
            {
                return null;
            }
            else
            {
                return sv.DynamicTabControl;
            }

        }

        private void TitleBarRectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_IsOverTabControl)
            {
                btn_PopWindowInToTabs_Click(null, null);
            }
            else
            {

            }
            if(aTimer == null)
            {
                return;
            }
            aTimer.Enabled = false;
            TabControl mainTabControl = GetTabControlFromMainWindow();
            mainTabControl.Opacity = 1;
            mainTabControl.Background = SystemColors.WindowBrush;
            this.Opacity = 1;
        }

        #endregion

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            WindowVM vm = (WindowVM)this.DataContext;
            if (vm != null && vm.Tab != null && vm.Tab.IsDragging)
            {
                Opacity = 1;
                try
                {
                    initTimer();
                    DoDraggingWithTimer();
                }
                catch (Exception ex)
                {
                    //do nothing
                }
                //Opacity = 1;
                //initiate the timer that checks if we are over the main window tab control
            }
        }

    }
}
