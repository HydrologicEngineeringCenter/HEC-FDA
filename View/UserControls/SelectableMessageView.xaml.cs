using System.Windows;
using System.Windows.Controls;

namespace View.UserControls
{
    /// <summary>
    /// Interaction logic for SelectableMessageView.xaml
    /// </summary>
    public partial class SelectableMessageView : UserControl
    {
        private ViewModel.Implementations.SelectableMessageViewModel _vm = new ViewModel.Implementations.SelectableMessageViewModel();
        public SelectableMessageView()
        {
            InitializeComponent();
            //System.Diagnostics.Debugger.Break();
            DataContext = _vm;
            Base.Implementations.MessageHub.Subscribe(_vm);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mv.tb.Inlines.Clear();
        }
    }
}
