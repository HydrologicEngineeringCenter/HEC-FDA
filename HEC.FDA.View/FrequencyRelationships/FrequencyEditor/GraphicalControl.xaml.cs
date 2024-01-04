using System.Windows.Controls;

namespace HEC.FDA.View.FrequencyRelationships.FrequencyEditor
{
    public partial class GraphicalControl : UserControl
    {
        public GraphicalControl()
        {
            InitializeComponent();
            //Hack to get the stage radio button to load in checked when useFlow is false on the vm. 
            if(FlowRadioButton.IsChecked == false)
            {
                StageRadioButton.IsChecked = true;
            }
        }
    }
}
