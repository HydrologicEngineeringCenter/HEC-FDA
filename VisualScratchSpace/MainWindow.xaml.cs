using System.Windows;

namespace VisualScratchSpace;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    bool running = false;
    public MainWindow()
    {
        InitializeComponent();

    }

    private void btnToggle_Click(object sender, RoutedEventArgs e)
    {
        running = !running;
        btnToggle.Content = running ? "Stop" : "Run";
        tbStatus.Text = running ? "Running" : "Stopped";
    }
}