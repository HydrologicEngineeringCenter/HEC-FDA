using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Utilities;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace HEC.FDA.View
{
    /// <summary>
    /// Interaction logic for ViewWindow.xaml
    /// </summary>
    public partial class ViewWindow : Window
    {
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
            vm.LaunchNewWindow += WindowSpawner;
            Closing += vm.OnClosing;   
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
                newwindow.Show();
            }
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
       
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindowVM vm = (WindowVM)this.DataContext;
            if (vm.Tab != null)
            {
                IDynamicTab tab = vm.Tab;
                tab.RemoveWindow();
            }
        }

        private void btn_PopOut_Click(object sender, RoutedEventArgs e)
        {
            WindowVM winVM = (WindowVM)DataContext;
            if(winVM.Tab != null)
            {
                winVM.Tab.PopWindowIntoTab();
                Close();
            }

        }

        static public void EnumVisual(Visual myVisual)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myVisual); i++)
            {
                // Retrieve child visual at specified index value.
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(myVisual, i);

                // Do processing of the child visual object.

                // Enumerate children of the child visual object.
                EnumVisual(childVisual);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.Height = this.DesiredSize.Height;
            //this.MinHeight = this.DesiredSize.Height;
            //this.MinWidth = this.DesiredSize.Width;
            //this.Width = this.DesiredSize.Width;


            //EnumVisual(this);
        }
    }
}
