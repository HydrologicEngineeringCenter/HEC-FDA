using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OpenGLMapping;
using System.IO;
using System.Xml;
using FdaViewModel.Utilities;

namespace Fda
{
    /// <summary>
    /// Interaction logic for ViewWindow.xaml
    /// </summary>
    public partial class ViewWindow : Window
    {
        public ViewWindow()
        {
            InitializeComponent();
            FdaViewModel.Utilities.WindowVM vm = (FdaViewModel.Utilities.WindowVM)this.DataContext;
            Title = vm.Title;
            FdaViewModel.Study.FdaStudyVM test = (FdaViewModel.Study.FdaStudyVM)vm.CurrentView;

            test.RequestShapefilePaths += RequestShapefilePaths;
            test.RequestShapefilePathsOfType += RequestShapefilePathsOfType;
            test.RequestAddToMapWindow += RequestAddToMapWindow;
            test.RequestRemoveFromMapWindow += RequestRemoveFromMapWindow;
            vm.LaunchNewWindow += WindowSpawner;
            Closing += vm.OnClosing;

            //hide the top row with the pop in button if this vm doesn't support that
            MainGrid.RowDefinitions[0].Height = new GridLength(0);            
        }

       
        private void RequestAddToMapWindow(object sender, FdaViewModel.Utilities.AddMapFeatureEventArgs args)
        {
            Study.StudyView sv = GetTheVisualChild<Study.StudyView>(masterControl);
            if (sv == null) { return; }

            //FdaViewModel.Study.FdaStudyVM studyVM = (FdaViewModel.Study.FdaStudyVM)sv.DataContext;

            OpenGLMapping.MapTreeView mtv = sv.MapTreeView;
            if (args.GetType().Name == nameof(FdaViewModel.Utilities.AddGriddedDataEventArgs))
            {
                FdaViewModel.Utilities.AddGriddedDataEventArgs gargs = args as FdaViewModel.Utilities.AddGriddedDataEventArgs;
                OpenGLMapping.RasterFeatureNode rfn = new RasterFeatureNode(new MapRaster(gargs.Features, gargs.Ramp, args.FeatureName, mtv.MapWindow), args.FeatureName);
                mtv.AddGisData(rfn, 0, true);
                if (sender.GetType().Name == nameof(FdaViewModel.Watershed.TerrainElement))
                {
                    args.MapFeatureHash = rfn.GetHashCode();
                    rfn.RemoveLayerCalled += ((FdaViewModel.Watershed.TerrainElement)sender).removedcallback;
                }
                if (sender.GetType().Name == nameof(FdaViewModel.WaterSurfaceElevation.WaterSurfaceElevationElement))
                {
                    args.MapFeatureHash = rfn.GetHashCode();
                    rfn.RemoveLayerCalled += ((FdaViewModel.WaterSurfaceElevation.WaterSurfaceElevationElement)sender).removedcallback;
                }

            }
            else if (args.GetType().Name == nameof(FdaViewModel.Utilities.AddShapefileEventArgs))
            {
                FdaViewModel.Utilities.AddShapefileEventArgs sargs = args as FdaViewModel.Utilities.AddShapefileEventArgs;

                //if (sender.GetType().Name == nameof(FdaViewModel.ImpactArea.ImpactAreaElement))
                if (sargs.Features.GetType() == typeof(LifeSimGIS.PolygonFeatures))
                {
                    LifeSimGIS.PolygonFeatures polyFeatures = (LifeSimGIS.PolygonFeatures)sargs.Features;
                    OpenGLDrawSingle drawInfo = new OpenGLMapping.OpenGLDrawSingle(sargs.DrawInfo);
                    MapPolygons mapPolys = new MapPolygons(polyFeatures, sargs.Attributes, sargs.FeatureName, drawInfo, mtv.MapWindow);
                   VectorFeatureNode vfn = new VectorFeatureNode(mapPolys, sargs.FeatureName);

                    mtv.AddGisData(vfn, 0, true);
                    args.MapFeatureHash = vfn.GetHashCode();
                    vfn.RemoveLayerCalled += ((FdaViewModel.ImpactArea.ImpactAreaElement)sender).removedcallback;
                }
                //else if (sender.GetType().Name == nameof(FdaViewModel.Inventory.InventoryElement))
                else if (sargs.Features.GetType() == typeof(LifeSimGIS.PointFeatures))
                {
                    VectorFeatureNode vfn = new VectorFeatureNode(new MapPoints((LifeSimGIS.PointFeatures)sargs.Features, sargs.Attributes, sargs.FeatureName, new OpenGLMapping.OpenGLDrawSingle(sargs.DrawInfo), mtv.MapWindow), sargs.FeatureName);
                    mtv.AddGisData(vfn, 0, true);
                    args.MapFeatureHash = vfn.GetHashCode();
                    vfn.RemoveLayerCalled += ((FdaViewModel.Inventory.InventoryElement)sender).removedcallback;
                }
                else if (sargs.Features.GetType() == typeof(LifeSimGIS.LineFeatures))
                {

                }



            }

            
           

        }

        private void removedcallback(FeatureNodeHeader node, bool includeSelected)
        {
            throw new NotImplementedException();
        }

        private void RequestRemoveFromMapWindow(object sender, FdaViewModel.Utilities.RemoveMapFeatureEventArgs args)
        {
            Study.StudyView sv = GetTheVisualChild<Study.StudyView>(masterControl);
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
            Study.StudyView sv = GetTheVisualChild<Study.StudyView>(masterControl);
            if (sv == null) { return; }
            OpenGLMapping.MapTreeView mtv = sv.MapTreeView;
            List<OpenGLMapping.VectorFeatureNode> v = mtv.GetVectorFeatureNodes();
            foreach (OpenGLMapping.VectorFeatureNode fn in v)
            {
                files.Add(fn.GetBaseFeature.SourceFile);
            }
        }
        private void RequestShapefilePathsOfType(ref System.Collections.Generic.List<string> files, FdaViewModel.Utilities.VectorFeatureType featureType)
        {
            Study.StudyView sv = GetTheVisualChild<Study.StudyView>(masterControl);
            if (sv == null) { return; }
            OpenGLMapping.MapTreeView mtv = sv.MapTreeView;
            List<OpenGLMapping.VectorFeatureNode> v = mtv.GetVectorFeatureNodes();
            foreach (OpenGLMapping.VectorFeatureNode fn in v)
            {
                switch (featureType)
                {
                    case FdaViewModel.Utilities.VectorFeatureType.Point:
                        if (fn.GetBaseFeature.GetType() == typeof(OpenGLMapping.MapPoints))
                        {
                            files.Add(fn.GetBaseFeature.SourceFile);
                        }
                        break;
                    case FdaViewModel.Utilities.VectorFeatureType.Line:
                        if (fn.GetBaseFeature.GetType() == typeof(OpenGLMapping.MapLines))
                        {
                            files.Add(fn.GetBaseFeature.SourceFile);
                        }
                        break;
                    case FdaViewModel.Utilities.VectorFeatureType.Polygon:
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
        public ViewWindow(FdaViewModel.Utilities.WindowVM newvm)
        {
            InitializeComponent();
            DataContext = newvm;
            Title = newvm.Title;
            newvm.LaunchNewWindow += WindowSpawner;
            Closing += newvm.OnClosing;
        }
        private void btn_PopWindowInToTabs_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Utilities.WindowVM vm = (FdaViewModel.Utilities.WindowVM)this.DataContext;
            FdaViewModel.BaseViewModel vmToPopIn = vm.CurrentView;
            if (vmToPopIn.AddThisToTabs != null)
            {
                vmToPopIn.AddThisToTabs(new DynamicTabVM(vm.Title, vmToPopIn, true), true);
            }
            Close();
        }


        private void WindowSpawner(FdaViewModel.Utilities.WindowVM newvm, bool asDialogue)
        {
            newvm.WasCanceled = true;
            
            newvm.Scalable = false;
            ViewWindow newwindow = new ViewWindow(newvm);
            newwindow.Owner = this;

            //hide the top row with the pop in button if this vm doesn't support that
            if(newvm.CurrentView.CanPopIn == false)
            {
                newwindow.MainGrid.RowDefinitions[0].Height = new GridLength(0);
            }

            if (asDialogue)
            {
                newwindow.ShowDialog();

            }
            else
            {
                newwindow.Show();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           

            FdaViewModel.Utilities.WindowVM vm = (FdaViewModel.Utilities.WindowVM)this.DataContext;
            if(vm.CurrentView.GetType() == typeof(FdaViewModel.Study.FdaStudyVM))
            {
                FdaViewModel.Study.FdaStudyVM studyVM = (FdaViewModel.Study.FdaStudyVM)vm.CurrentView;
                studyVM.Dispose();
            }
            if (vm.CurrentView.RemoveFromTabsDictionary != null)
            {

                vm.CurrentView.RemoveFromTabsDictionary(vm.CurrentView);
            }
            vm.Dispose();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.Height = this.DesiredSize.Height;
            //this.MinHeight = this.DesiredSize.Height;
            //this.MinWidth = this.DesiredSize.Width;
            //this.Width = this.DesiredSize.Width;
            //need to figure out how to set max widths and heights.
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
            //    MaximizeButton.Content = new Image() { Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("pack://application:,,,/Fda;component/Resources/Maximize.png")) };
            //    MaximizeButton.ToolTip = "Maximize";
            //}
            //else
            //{
            //    this.WindowState = System.Windows.WindowState.Maximized;
            //    MaximizeButton.ToolTip = "Restore";
            //    MaximizeButton.Content = new Image() { Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("pack://application:,,,/Fda;component/Resources/Restore.png")) };
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
        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Top += e.VerticalChange;
            Left += e.HorizontalChange;
        }
        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2) { Close(); }
        }

        private void Image_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ContextMenu c = new ContextMenu();
            MenuItem min = new MenuItem();
            min.Header = "Minimize";
            min.Icon = "  __";
            min.Click += MinimizeWindow;

            MenuItem max = new MenuItem();
            if (this.WindowState == WindowState.Maximized)
            {
                max.Header = "Restore";
                max.Icon = new Image() { Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("pack://application:,,,/Fda;component/Resources/Restore.png")) };
            }
            else
            {
                max.Header = "Maximize";
                max.Icon = new Image() { Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("pack://application:,,,/Fda;component/Resources/Maximize.png")) };
            }
                
            max.Click += MaximizeWindow;

            MenuItem close = new MenuItem();
            close.Header = "Close";
            close.Icon = "  X";
            close.Click += CloseWindow;
            c.Items.Add(min);
            c.Items.Add(max);
            c.Items.Add(close);
            this.ContextMenu = c;

        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            //FdaViewModel.Study.FdaStudyVM vm = (FdaViewModel.Study.FdaStudyVM)this.DataContext;
            ////vm.MWMTVConn. MapTreeView = MapTreeView;
            //if (vm.MWMTVConn != null)
            //{
            //    vm.MWMTVConn.UpdateMapWindow();
            //}
        }
    }
}
