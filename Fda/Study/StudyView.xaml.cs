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

namespace Fda.Study
{
    /// <summary>
    /// Interaction logic for StudyView.xaml
    /// </summary>
    public partial class StudyView : UserControl
    {
        public StudyView()
        {
            InitializeComponent();
            MapWindow.MapWindow.TreeView = MapTreeView;
            MapTreeView.MapWindow = MapWindow.MapWindow;
            MapToolBar.MapWindow = MapWindow.MapWindow;
            MapToolBar.MapTree = MapTreeView;
            SelectableLayers.MapTree = MapTreeView;
            SelectableLayers.MapWindow = MapWindow.MapWindow;
            FeatureEditorToolbar.MapWindow = MapWindow.MapWindow;
            FeatureEditorToolbar.MapTree = MapTreeView;
            StatusBorder.Child = new OpenGLMapping.MapStatusBar(MapWindow.MapWindow);

            RadioButton ArrowTool = (RadioButton)MapToolBar.Items[0];
            ArrowTool.IsChecked = true;
            MapToolBar.RadioChecked += MapToolBar_RadioChecked;

            //event handlers

            //MapTreeView.LayerAdded += MapTreeView_LayerAdded; //adds menu item to the remove Layer menu item items.
            //MapTreeView.LayerRemoved += MapTreeView_LayerRemoved; //Removes menu item for layer from the Remove Layer menu item items.
            //MapTreeView.LayerMoved += MapTreeView_LayerMoved;//reorders main menu item for RemoveLayer

            FeatureEditorToolbar.RadioChecked += FeatureEditorToolbar_RadioChecked;

            MapWindow.MapWindow.MouseLeave += MapWindow_Mouse;
            MapWindow.MapWindow.MouseEnter += MapWindow_Mouse;
        }

        private void MapToolBar_RadioChecked(RadioButton buttonChecked)
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

        private void MapWindow_Mouse(object sender, EventArgs e)
        {
            object fe = FocusManager.GetFocusedElement(this);
            if (fe == null) return;
            if (fe.GetType() == typeof(OpenGLMapping.WinFormsHostControl)) { MapWindow.MapWindow.Focus(); }
        }
        private void FeatureEditorToolbar_RadioChecked(RadioButton buttonChecked)
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

    }
}
