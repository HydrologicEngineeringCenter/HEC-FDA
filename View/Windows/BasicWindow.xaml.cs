using System.Windows;
namespace HEC.MVVMFramework.View.Windows
{
    /// <summary>
    /// Interaction logic for BasicWindow.xaml
    /// </summary>
    public partial class BasicWindow : Window
    {
        public BasicWindow(ViewModel.Implementations.BaseViewModel bvm):base(bvm)
        {
            DataContext = bvm;
            InitializeComponent();
            SizeToContent = SizeToContent.WidthAndHeight;
        }
    }
}
