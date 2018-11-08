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

namespace Fda.Utilities
{
    /// <summary>
    /// Interaction logic for MapWindowControl.xaml
    /// </summary>
    public partial class MapWindowControl : UserControl
    {
        //public static readonly DependencyProperty MapTreeViewProperty = DependencyProperty.Register("MapTreeView", typeof(OpenGLMapping.MapTreeView), typeof(MapWindowControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(MapTreeViewChangedCallBack)));

        //public OpenGLMapping.MapTreeView MapTreeView
        //{
        //    get { return (OpenGLMapping.MapTreeView)GetValue(MapTreeViewProperty); }
        //    set { SetValue(MapTreeViewProperty, value); }
        //}

        public MapWindowControl()
        {
            InitializeComponent();

            //MapWindow.MapWindow.TreeView = MapTreeView;
            //MapTreeView.MapWindow = MapWindow.MapWindow;
            //MapToolBar.MapWindow = MapWindow.MapWindow;
            //MapToolBar.MapTree = MapTreeView;
            //SelectableLayers.MapTree = MapTreeView;
            //SelectableLayers.MapWindow = MapWindow.MapWindow;
            //FeatureEditorToolbar.MapWindow = MapWindow.MapWindow;
            //FeatureEditorToolbar.MapTree = MapTreeView;
            //StatusBorder.Child = new OpenGLMapping.MapStatusBar(MapWindow.MapWindow);

            //RadioButton ArrowTool = (RadioButton)MapToolBar.Items[0];
            //ArrowTool.IsChecked = true;
            //MapToolBar.RadioChecked += MapToolBar_RadioChecked;

            //FeatureEditorToolbar.RadioChecked += FeatureEditorToolbar_RadioChecked;

            //MapWindow.MapWindow.MouseLeave += MapWindow_Mouse;
            //MapWindow.MapWindow.MouseEnter += MapWindow_Mouse;

          

           


        }

        public void FeatureEditorToolbar_RadioChecked(RadioButton buttonChecked)
        {
            if (buttonChecked == null)
            {
                MapToolBar.SelectChecked();
            }
            else
            {
                foreach (object item in MapToolBar.Items)
                {
                    RadioButton r = item as RadioButton;
                    if (r != null)
                    {
                        r.IsChecked = false;
                    }
                }
            }
        }

        public void MapToolBar_RadioChecked(RadioButton buttonChecked)
        {
            foreach (object item in FeatureEditorToolbar.Items)
            {
                RadioButton r = item as RadioButton;
                if (r != null)
                {
                    r.IsChecked = false;
                }
            }
        }

        public void MapWindow_Mouse(object sender, EventArgs e)
        {
            object fe = FocusManager.GetFocusedElement(this);
            if (fe == null) return;
            if (fe.GetType() == typeof(OpenGLMapping.WinFormsHostControl)) { MapWindow.MapWindow.Focus(); }
        }


        //private static void MapTreeViewChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    MapWindowControl owner = d as MapWindowControl;
        //    if (e != null && e.NewValue != null)
        //    {
        //        if (e.NewValue.GetType() == typeof(OpenGLMapping.MapTreeView))
        //        {
        //            OpenGLMapping.MapTreeView tv = e.NewValue as OpenGLMapping.MapTreeView;

        //            owner.MapWindow.MapWindow.TreeView = tv;
        //            tv.MapWindow = owner.MapWindow.MapWindow;

        //            owner.MapToolBar.MapTree = tv;
        //            owner.SelectableLayers.MapTree = tv;

        //            owner.FeatureEditorToolbar.MapTree = tv;

        //        }
        //    }  
        //}
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
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

            //FdaViewModel.Utilities.MapWindowControlVM vm = (FdaViewModel.Utilities.MapWindowControlVM)this.DataContext;
            //vm.MWMTVConn.MapWindow = MapWindow.MapWindow;

            //MapWindow.MapWindow.TreeView = vm.MWMTVConn.MapTreeView;
            //vm.MWMTVConn.MapTreeView.MapWindow = MapWindow.MapWindow;

            //MapToolBar.MapTree = vm.MWMTVConn.MapTreeView;
            //SelectableLayers.MapTree = vm.MWMTVConn.MapTreeView;

            //FeatureEditorToolbar.MapTree = vm.MWMTVConn.MapTreeView;


            //vm.MWMTVConn.MapTreeView.UpdateMapWindow();

        }
    }
}
