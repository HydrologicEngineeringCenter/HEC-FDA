

namespace View.Windows
{
    /// <summary>
    /// Interaction logic for BasicWindow.xaml
    /// </summary>
    public partial class BasicWindow : Window
    {
        public BasicWindow(ViewModel.BaseViewModel bvm):base(bvm)
        {
            DataContext = bvm;
            InitializeComponent();
        }
    }
}
