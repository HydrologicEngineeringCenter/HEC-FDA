using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace FdaViewModel.Utilities
{
    public class MapWindowMapTreeViewConnector:BaseViewModel
    {

        private static MapWindowMapTreeViewConnector instance;


        private OpenGLMapping.MapTreeView _MapTreeView;
        private OpenGLMapping.OpenGLMapWindow _MapWindow;

        public OpenGLMapping.MapTreeView MapTreeView
        {
            get { return _MapTreeView; }
            set
            {
                _MapTreeView = value;

                NotifyPropertyChanged();
            }
        }
        public OpenGLMapping.OpenGLMapWindow MapWindow
        {
            get { return _MapWindow; }
            set { _MapWindow = value; NotifyPropertyChanged(); }
        }

        private MapWindowMapTreeViewConnector()
        {

        }


        public static MapWindowMapTreeViewConnector Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MapWindowMapTreeViewConnector();
                }
                return instance;
            }
        }


        public void UpdateMapWindow()
        {
            if (_MapWindow != null)
            {

                //_MapTreeView.MapWindow = _MapWindow;
                //_MapWindow.TreeView = _MapTreeView;

                _MapWindow.PlotFeatures();
                //_MapWindow.PlotFeatures();
                //_MapWindow.CaptureScreenToGpu();
                //_MapWindow. MapWindow_Mouse(this, new )
                
                //_MapWindow.ZoomFromCenter(1);
                // _MapWindow.OpenGLMapWindow_Resize(null, null);
                // _MapWindow.SetupViewPort(extent); definitely not this one
                //_MapTreeView.UpdateMapWindow();

            }
        }
     

    }
}
