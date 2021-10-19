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
        //public static readonly DependencyProperty ExpandedHeightProperty = DependencyProperty.Register("ExpandedHeight", typeof(int), typeof(MessageExpanderControl), new FrameworkPropertyMetadata(300, new PropertyChangedCallback(ExpandedHeightCallBack)));
        //public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(String), typeof(MessageExpanderControl), new FrameworkPropertyMetadata("Errors", new PropertyChangedCallback(HeaderTextCallBack)));
        //public static readonly DependencyProperty StatusLevelProperty = DependencyProperty.Register("StatusLevel", typeof(LoggingLevel), typeof(MessageExpanderControl), new FrameworkPropertyMetadata(LoggingLevel.Debug, new PropertyChangedCallback(StatusLevelCallBack)));
        //public static readonly DependencyProperty StatusVisibleProperty = DependencyProperty.Register("StatusVisible", typeof(bool), typeof(MessageExpanderControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(StatusVisibleCallBack)));

        private static readonly int LIST_ROW = 0;

        //public int ExpandedHeight
        //{
        //    get { return (int)GetValue(ExpandedHeightProperty); }
        //    set { SetValue(ExpandedHeightProperty, value); }
        //}
        //public String HeaderText
        //{
        //    get { return (String)GetValue(HeaderTextProperty); }
        //    set { SetValue(HeaderTextProperty, value); }
        //}

        
        //public bool StatusVisible
        //{
        //    get { return (bool)GetValue(StatusVisibleProperty); }
        //    set { SetValue(StatusVisibleProperty, value); }
        //}
        public RecommendationExpanderControl()
        {
            InitializeComponent();
            
        }
        
        private static void HeaderTextCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RecommendationExpanderControl owner = d as RecommendationExpanderControl;
            String headerText = (String)e.NewValue;
            owner.MessagesExpander.Header = headerText;

        }
        //private static void StatusLevelCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    RecommendationExpanderControl owner = d as RecommendationExpanderControl;
        //    LoggingLevel level = (LoggingLevel)e.NewValue;
        //    owner.UpdateStatusText(level);

        //}
        //private static void StatusVisibleCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    RecommendationExpanderControl owner = d as RecommendationExpanderControl;
        //    bool showStatus = (bool)e.NewValue;
        //    if (showStatus)
        //    {
        //        owner.StatusPanel.Height = 25;
        //    }
        //    else
        //    {
        //        owner.StatusPanel.Height = 0;
        //    }

        //}
        //private static void ExpandedHeightCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    RecommendationExpanderControl owner = d as RecommendationExpanderControl;
        //    int maxHeight = (int)e.NewValue;
        //    owner.MainGrid.RowDefinitions[LIST_ROW].MaxHeight = maxHeight;
        //    if (maxHeight > 50)
        //    {
        //        //this is in order to actually see the bottom of the scrolling
        //        owner.MessagesExpander.MaxHeight = maxHeight - 50;
        //    }
        //}

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
                MessagesExpander.Margin = new Thickness(5, 5, 5, 40);
            }

        }

      
    }
}
