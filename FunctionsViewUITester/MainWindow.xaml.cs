using Functions;
using Functions.CoordinatesFunctions;
using FunctionsView.ViewModel;
using Model;
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
    public partial class MainWindow : Window //, INotifyPropertyChanged
    {
        //private ICoordinatesFunction _Function;

        public CoordinatesFunctionEditorVM EditorVM { get; set; }
        //public ICoordinatesFunction Function
        //{
        //    get { return _Function; }
        //    set
        //    {
        //        _Function = value;
        //        if (PropertyChanged != null)
        //        {
        //            PropertyChanged(this, new PropertyChangedEventArgs("Function"));
        //        }
        //    }
        //}



            private IFdaFunction CreateFunction()
        {
            List<double> xValues = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            List<double> yValues = new List<double>() { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            Functions.ICoordinatesFunction func = Functions.ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
            IFunction function = IFunctionFactory.Factory(func.Coordinates, func.Interpolator);
            IFdaFunction defaultCurve = IFdaFunctionFactory.Factory( IParameterEnum.Rating, function);
            return defaultCurve;
        }

        //private void CreateFunction2()
        //{
        //    List<double> xs = new List<double>() { 0, 1, 2, 3, 4 };
        //    List<double> ys = new List<double>() { 10, 11, 12, 13, 14 };

        //    List<double> xs2 = new List<double>() { 10, 11, 12, 13, 14 };
        //    List<double> ys2 = new List<double>() { 100, 110, 120, 130, 140 };
           
        //    ICoordinatesFunction func1 = ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
        //    ICoordinatesFunction func2 = ICoordinatesFunctionsFactory.Factory(xs2, ys2, InterpolationEnum.Piecewise);
        //    List<ICoordinatesFunction> functions = new List<ICoordinatesFunction>() { func1, func2 };
        //    List<InterpolationEnum> interps = new List<InterpolationEnum>();
        //    interps.Add(func1.Interpolator);
        //    ICoordinatesFunction linkedFunc = ICoordinatesFunctionsFactory.Factory(functions, interps);
        //    Function = linkedFunc;
        //    //Function = func1;
        //}
        public MainWindow()
        {
            this.DataContext = this;
            IFdaFunction function = CreateFunction();
            //EditorVM = new CoordinatesFunctionEditorVM(function.Function);

            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ICoordinatesFunction functionToSave = EditorVM.CreateFunctionFromTables();

        }
    }
}
