using System;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.FrequencyRelationships.FrequencyEditor
{
    public partial class GraphicalControl : UserControl
    {
        public GraphicalControl()
        {
            InitializeComponent();
            InstanceID = Guid.NewGuid().ToString();
        }

        public string InstanceID
        {
            get { return (string)GetValue(InstanceIDProperty); }
            set { SetValue(InstanceIDProperty, value); }
        }

        public static readonly DependencyProperty InstanceIDProperty =
            DependencyProperty.Register("InstanceID", typeof(string), typeof(GraphicalControl), new PropertyMetadata(string.Empty));
    }
}
