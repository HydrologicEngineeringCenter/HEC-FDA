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
    /// Interaction logic for ArrowPolygon.xaml
    /// </summary>
    public partial class ArrowPolygon : UserControl
    {

        public static readonly DependencyProperty RotateProperty = DependencyProperty.Register("Rotate", typeof(double), typeof(ArrowPolygon), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(RotateCallBack)));

        public double Rotate
        {
            get { return (double)GetValue(RotateProperty); }
            set { SetValue(RotateProperty, value); }
        }


        private static void RotateCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ArrowPolygon owner = d as ArrowPolygon;
            double rotateValue = (double)e.NewValue;
            RotateTransform rotTransf = new RotateTransform(rotateValue, 10, 20);
            
            owner.RenderTransform = rotTransf;
        }

        public ArrowPolygon()
        {
            InitializeComponent();
  
        }
    }
}
