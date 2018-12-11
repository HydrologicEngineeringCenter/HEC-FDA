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

namespace Fda.Plots
{
    /// <summary>
    /// Interaction logic for IndividualLinkedPlotControl.xaml
    /// </summary>
    public partial class IndividualLinkedPlotControl : UserControl
    {
        public static readonly DependencyProperty UpdatePlotsFromVMProperty = DependencyProperty.Register("UpdatePlotsFromVM", typeof(bool), typeof(IndividualLinkedPlotControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(UpdatePlotsFromVMCallBack)));


        public event EventHandler UpdatePlots;
        public event EventHandler PopImporterIntoPlot1;
        public event EventHandler PopPlotIntoPlot1;

        public event EventHandler PopImporterIntoPlot5;
        public event EventHandler PopPlotIntoPlot5;

        public bool UpdatePlotsFromVM
        {
            get { return (bool)GetValue(UpdatePlotsFromVMProperty); }
            set { SetValue(UpdatePlotsFromVMProperty, value); }
        }
        public ILinkedPlot LinkedPlot
        {
            get;
            
            set;
        }
       
        public IndividualLinkedPlotControl()
        {
            InitializeComponent();
            
        }

        public void PopTheImporterIntoPlot5()
        {
            if (PopImporterIntoPlot5 != null)
            {
                PopImporterIntoPlot5(this, new EventArgs());
            }
        }

        public void PopThePlotIntoPlot5()
        {
            if (PopPlotIntoPlot5 != null)
            {
                PopPlotIntoPlot5(this, new EventArgs());
            }
        }
        public void PopTheImporterIntoPlot1()
        {
            if(PopImporterIntoPlot1 != null)
            {
                PopImporterIntoPlot1(this, new EventArgs());
            }
        }

        public void PopThePlotIntoPlot1()
        {
            if(PopPlotIntoPlot1 != null)
            {
                PopPlotIntoPlot1(this, new EventArgs());
            }
        }

       public void UpdateThePlots()
        {
            if(UpdatePlots != null)
            {
                UpdatePlots(this, new EventArgs());
            }
        }


        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            if(child == null) { return null; }
            T parent = VisualTreeHelper.GetParent(child) as T;

            if (parent != null)

                return parent;

            else

                return FindParent<T>(VisualTreeHelper.GetParent(child));
        }


        private static void UpdatePlotsFromVMCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndividualLinkedPlotControl owner = d as IndividualLinkedPlotControl;
            owner.UpdateThePlots();
            
        }


    }
}
