using Functions;
using FunctionsView.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace FunctionsViewUITester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ICoordinatesFunction _Function;
        public ICoordinatesFunction Function
        {
            get { return _Function; }
            set
            {
                _Function = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Function"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //private List<CurveGeneratorRowItem> _Rows = new List<CurveGeneratorRowItem>();
        //public List<CurveGeneratorRowItem> RowItems
        //{
        //    get
        //    {
        //        return _Rows;
        //    }
        //}

        private void CreateFunction()
        {
            List<double> xs = new List<double>() { 0, 1, 2, 3, 4 };
            List<double> ys = new List<double>() { 10, 11, 12, 13, 14 };
            //List<IDistribution> ys = new List<IDistribution>()
            //    {
            //        IDistributionFactory.FactoryNormal(1,0),
            //        IDistributionFactory.FactoryNormal(1,0),
            //        IDistributionFactory.FactoryNormal(1,0),
            //        IDistributionFactory.FactoryNormal(1,0),
            //        IDistributionFactory.FactoryNormal(1,0),

            //    };
            Function = ICoordinatesFunctionsFactory.Factory(xs, ys);

            //Curve = new CoordinatesFunctionEditorVM(func, "SomeType");
        }
        public MainWindow()
        {
            CreateFunction();
            this.DataContext = this;

            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
        }
    }
}
