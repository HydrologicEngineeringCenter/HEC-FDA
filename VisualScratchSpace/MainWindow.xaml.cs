using System.Windows;

namespace VisualScratchSpace;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainWindowVM _vm;
    public MainWindow()
    {
        InitializeComponent();
        _vm = new MainWindowVM();
        DataContext = _vm;
    }
}