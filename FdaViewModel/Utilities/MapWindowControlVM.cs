using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
{
    public class MapWindowControlVM : BaseViewModel
    {

        public event EventHandler SetMapWindowProperty;

        private MapWindowMapTreeViewConnector _MWMTVConn;


        public MapWindowMapTreeViewConnector MWMTVConn
        {
            get;set;
        }

        //private OpenGLMapping.MapTreeView _MapTreeView;
        //private OpenGLMapping.OpenGLMapWindow _MapWindow;
        //public OpenGLMapping.MapTreeView MapTreeView
        //{
        //    get { return _MapTreeView; }
        //    set { _MapTreeView = value; NotifyPropertyChanged(); }
        //}
        //public OpenGLMapping.OpenGLMapWindow MapWindow
        //{
        //    get { return _MapWindow; }
        //    set { _MapWindow = value; SetMapWindowProperty?.Invoke(this, new EventArgs());
        //        if (_MapWindow != null)
        //        {
        //            _MapWindow.TreeView = _MapTreeView;
        //            _MapWindow.PlotFeatures();
        //            //_MapWindow.;

        //        }
        //    }
        //}

        public MapWindowControlVM(MapWindowMapTreeViewConnector conn)
        {
            MWMTVConn = conn;
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

      
    }
}
