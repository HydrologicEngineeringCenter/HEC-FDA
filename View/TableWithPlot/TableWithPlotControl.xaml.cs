using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.TableWithPlot
{
    public partial class TableWithPlotControl : UserControl
    {
        public static readonly DependencyProperty ExpandedHeightProperty = DependencyProperty.Register("IsTableEnabled", typeof(bool), typeof(TableWithPlotControl), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(IsTableEnabledCallback)));

        public TableWithPlotControl()
        {
            InitializeComponent();
        }

        private void SetTableEnabled(bool isEnabled)
        {
            compComp.IsEnabled = isEnabled;
        }

        private static void IsTableEnabledCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TableWithPlotControl owner = d as TableWithPlotControl;
            bool isEnabled = (bool)e.NewValue;
            owner.SetTableEnabled(isEnabled);
        }

    }
}
