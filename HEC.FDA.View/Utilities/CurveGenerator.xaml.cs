using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace HEC.FDA.View.Utilities
{
    /// <summary>
    /// Interaction logic for CurveGenerator.xaml
    /// </summary>
    public partial class CurveGenerator : UserControl
    {
        //public static readonly DependencyProperty FunctionProperty = DependencyProperty.Register("Function", typeof(HEC.FDA.ViewModel.CurveGeneratorVM), typeof(CurveGenerator), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(FunctionChangedCallBack)));

        //private List<RowItem> _RowItems = new List<RowItem>();
        //private Model.IFdaFunction _Function;
        //public Model.IFdaFunction Function 
        //{
        //    get { return _Function; }
        //    set
        //    {
        //        _Function = value;
        //    }
        //}
        //public List<RowItem> RowItems
        //{
        //    get
        //    {
        //        RowItem row1 = new RowItem(1, 2);
        //        return new List<RowItem>() { row1 };
        //    }
        //}
        /// <summary>
        /// I wish that i could have this list on the view model side but there is some wierd stuff
        /// with comboboxes in data grids that doesn't allow that.
        /// </summary>
        private List<string> DistributionTypes()
        {
            
                List<string> types = new List<string>();
                types.Add("Normal");
                types.Add("Triangular");
                types.Add("None");

                return types;
            
        }

        private List<String> InterpolationTypes()
        {
            return new List<string>() { "Linear", "Piecewise" };
        }


        public CurveGenerator()
        {
            InitializeComponent();
            //DistTypeColumn.ItemsSource = DistributionTypes();
            //InterpTypeColumn.ItemsSource = InterpolationTypes();
        }

      

        //private static void FunctionChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    CurveGenerator owner = d as CurveGenerator;
        //    owner.Function = e.NewValue as Model.IFdaFunction;
        //}

        //public List<string> DistributionTypes
        //{
        //    get { return new List<string>() { "None", "Triangular", "Normal" }; }
        //}


        
    }
}
