using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace View.ImpactAreaScenario.Editor
{
    /// <summary>
    /// Interaction logic for RecommendationExpanderControl.xaml
    /// </summary>
    public partial class RecommendationExpanderControl : UserControl
    {      
        private static readonly int LIST_ROW = 0;

        
        public RecommendationExpanderControl()
        {
            InitializeComponent();          
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            if (sender == e.OriginalSource)
            {
                MainGrid.RowDefinitions[LIST_ROW].Height = GridLength.Auto;
                MessagesExpander.Margin = new Thickness(5, 5, 5, 5);

            }
        }

        private void MessagesExpander_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender == e.OriginalSource)
            {
                MainGrid.RowDefinitions[LIST_ROW].Height = new GridLength(1, GridUnitType.Star);
                MessagesExpander.Margin = new Thickness(5, 5, 5, 5);
            }

        }

      
    }
}
