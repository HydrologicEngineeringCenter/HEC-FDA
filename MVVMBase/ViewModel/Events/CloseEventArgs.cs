using System;
namespace ViewModel.Events
{
    public delegate void CloseEventHandler(object sender, CloseEventArgs e);
    public class CloseEventArgs:EventArgs
    {
        public bool ForceClose { get;}
        public CloseEventArgs(bool forceClose = false)
        {
            ForceClose = forceClose;
        }
    }
}
