using ViewModel.Events;

namespace View.Windows
{
    public class Window: System.Windows.Window
    {
        //private bool _ClosedByRedX = true;
        private System.Timers.Timer _t;
        public Window(ViewModel.Implementations.BaseViewModel bvm)
        {
            DataContext = bvm;
            AddHandlers(bvm);
        }
        public Window()
        {
            DataContext = null;
            //AddHandlers(bvm);
        }
        protected virtual void AddHandlers(ViewModel.Implementations.BaseViewModel bvm)
        {
            if(bvm is ViewModel.Interfaces.INavigate)
            {
                ((ViewModel.Interfaces.INavigate)bvm).NavigationEvent += Vm_RequestNavigation;
            }
            if (bvm is ViewModel.Interfaces.IUpdatePlot)
            {
                ((ViewModel.Interfaces.IUpdatePlot)bvm).UpdatePlotEvent += Vm_UpdatePlot;
            }
            if(bvm is Base.Interfaces.IReportMessage)
            {
                Base.Implementations.MessageHub.Register(bvm as Base.Interfaces.IReportMessage);
            }
            if (bvm is Base.Interfaces.IRecieveMessages)
            {
                Base.Implementations.MessageHub.Subscribe(bvm as Base.Interfaces.IRecieveMessages);
            }
            if (bvm is ViewModel.Interfaces.IClose)
            {
                ((ViewModel.Interfaces.IClose)bvm).CloseEvent += CloseProgram;
            }
        }
        protected virtual void RemoveHandlers(ViewModel.Implementations.BaseViewModel bvm)
        {
            if (bvm is ViewModel.Interfaces.INavigate)
            {
                ((ViewModel.Interfaces.INavigate)bvm).NavigationEvent -= Vm_RequestNavigation;
            }
            if (bvm is ViewModel.Interfaces.IUpdatePlot)
            {
                ((ViewModel.Interfaces.IUpdatePlot)bvm).UpdatePlotEvent -= Vm_UpdatePlot;
            }
            //if (bvm is ViewModel.Interfaces.IClose)
            //{
            //    ((ViewModel.Interfaces.IClose)bvm).CloseEvent += wb_Close;
            //}
        }
        private void Vm_UpdatePlot(object sender, UpdatePlotEventArgs e)
        {
            ViewModel.Interfaces.IUpdatePlot up = sender as ViewModel.Interfaces.IUpdatePlot;
            if (_t == null)
            {
                up.InvalidatePlotModel(e.UpdateData);
                _t = new System.Timers.Timer(e.MinMillisecondsBetweenUpdates);
                _t.Elapsed += _t_Elapsed;
                _t.Start();
            }
            else
            {
                if (!_t.Enabled)
                {
                    up.InvalidatePlotModel(e.UpdateData);
                    _t.Start();
                }
            }
        }
        private void _t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _t.Stop();
        }
        protected void Vm_RequestNavigation(object sender, ViewModel.Events.NavigationEventArgs e)
        {
            ViewModel.Implementations.BaseViewModel vm = e.ViewModel as ViewModel.Implementations.BaseViewModel;
            if (vm != null)
            {
                if ((e.NavigationProperties & ViewModel.Enumerations.NavigationOptionsEnum.AsNewWindow) > 0)
                {
                    BasicWindow vw = new BasicWindow(vm);
                    if (e.WindowTitle != "") vw.Title = e.WindowTitle;
                    vw.Owner = this;
                    if ((e.NavigationProperties & ViewModel.Enumerations.NavigationOptionsEnum.AsDialog) > 0)
                    {
                        vw.ShowDialog();
                    }
                    else
                    {
                        vw.Show();
                    }
                }
                else
                {
                    ViewModel.Implementations.BaseViewModel bvm = DataContext as ViewModel.Implementations.BaseViewModel;
                    RemoveHandlers(bvm);
                    if (e.WindowTitle != "") this.Title = e.WindowTitle;
                    this.DataContext = vm;
                    AddHandlers(vm);
                }
            }
            else
            {
                //throw an error.
            }
        }
        protected void CloseProgram(object sender, ViewModel.Events.CloseEventArgs e)
        {
            if (!e.ForceClose)//check if the user wants to, and set the cancel parameter in e.
            {
                Close();
            }
            else
            {
                Close();
            }

        }
        protected void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //if (_ClosedByRedX)
            //{
            //    ViewModel.WindowBase wb = DataContext as ViewModelBase.WindowBase;
            //    ViewModelBase.BaseViewModel bvm = wb.CurrentView;
            //    if (bvm == null)
            //    {
            //        //Close();
            //    }
            //    if (bvm.HasChanges)
            //    {
            //        ViewModel.MessageBox m = new ViewModel.MessageBox("Changes exist, are you sure you wish to close?", ViewModelBase.Enumerations.MessageBoxOptionsEnum.YesNo);
            //        Vm_RequestNavigation(this, new ViewModelBase.Events.RequestNavigationEventArgs(m, ViewModelBase.Enumerations.NavigationEnum.NewScalableDialog, "Do you wish to close?"));
            //        if ((m.Result & ViewModelBase.Enumerations.MessageBoxOptionsEnum.Yes) > 0)
            //        {
            //            //Close();
            //        }
            //        else
            //        {
            //            e.Cancel = true;
            //        }
            //    }
            //    else
            //    {
            //        // Close();
            //    }
            //}

        }
        protected void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Height = this.DesiredSize.Height;
            this.MinHeight = this.DesiredSize.Height;
            this.MinWidth = this.DesiredSize.Width;
            this.Width = this.DesiredSize.Width;
            //need to figure out how to set max widths and heights.
        }
    }
}
