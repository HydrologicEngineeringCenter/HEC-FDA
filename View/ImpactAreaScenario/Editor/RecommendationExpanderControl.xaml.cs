using System.Windows;
using System.Windows.Controls;

namespace View.ImpactAreaScenario.Editor
{
    /// <summary>
    /// This is the control that goes at the bottom of the IASEditor and gives the user information about
    /// non-overlapping ranges.
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
