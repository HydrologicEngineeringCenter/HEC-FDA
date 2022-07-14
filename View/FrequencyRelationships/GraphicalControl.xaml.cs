using System.Windows.Controls;

namespace HEC.FDA.View.FrequencyRelationships
{
    public partial class GraphicalControl : UserControl
    {
        public GraphicalControl()
        {
            InitializeComponent();
            
        }

        private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            int ti = 0;
        }
    }
}
