using Functions;
using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using FunctionsView.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace FunctionsView.View
{
    /// <summary>
    /// Interaction logic for CoordinatesFunctionEditor.xaml
    /// </summary>
    public partial class CoordinatesFunctionEditor : UserControl
    {
        public bool HasLoaded { get; set; }
        public CoordinatesFunctionEditor()
        {
            InitializeComponent();
            Tables = new List<CoordinatesFunctionTable>();
        }
        public static readonly DependencyProperty FunctionProperty = DependencyProperty.Register("Function", typeof(ICoordinatesFunction), typeof(CoordinatesFunctionEditor), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(FunctionChangedCallBack)));   

        public List<CoordinatesFunctionTable> Tables
        {
            get;
            set;
        }

        public ICoordinatesFunction Function
        {
            get { return (ICoordinatesFunction)this.GetValue(FunctionProperty); }
            set { this.SetValue(FunctionProperty, value); }
        }

        private static void FunctionChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CoordinatesFunctionEditor owner = d as CoordinatesFunctionEditor;
            //basically i am just storing a pointer to this function that is in the VM
            //when the user clicks OK or Save then i update this Function with whatever is
            //in this editors tables

            //i only want to do this one time when the editor is first opened.
            if (owner.HasLoaded != true)
            {
                owner.HasLoaded = true;
                owner.Function = e.NewValue as ICoordinatesFunction;
                owner.CreateTables(e.NewValue as ICoordinatesFunction);
                //call some sort of update method that re sorts and organizes the tables and replots.
                owner.UpdateView(owner, new EventArgs());
            }
            
        }

        /// <summary>
        /// This is used when the editor is opened to load the tables from a coordinates function.
        /// </summary>
        /// <param name="function"></param>
        private void CreateTables(ICoordinatesFunction function)
        {
            List<CoordinatesFunctionRowItem> rows = CreateRows(function);
            CreateTables(rows);
        }

        public void Save()
        {
            Function = CreateFunctionFromTables();
        }

        private ICoordinatesFunction CreateFunctionFromTables()
        {
            List<double> testXs = new List<double>() { 9, 99 };
            List<double> testYs = new List<double>() { 10, 100 };
            return ICoordinatesFunctionsFactory.Factory(testXs, testYs);
        }

        /// <summary>
        /// This is called anytime an visual update needs to happen, such as changing distribution types.
        /// </summary>
        /// <param name="rowItems"></param>
        private void CreateTables(List<CoordinatesFunctionRowItem> rowItems)
        {
            //we are about to create the tables, so we need to find out what kinds of data we are dealing with
            ColumnWidths.TableTypes tableType = ColumnWidths.GetTableTypeForRows(rowItems);
            //it would be nice to just bind the items source of the list to be the list of tables
            //but i have to put the topper and bottom UI pieces in. I could do them outside the list
            //but that seemed harder to get it to line up etc.
            lst_tables.Items.Clear();
            Tables.Clear();

            if (rowItems.Count > 0)
            {
                lst_tables.Items.Add(new TableTopControl(tableType));

                ObservableCollection<CoordinatesFunctionRowItem> rows = new ObservableCollection<CoordinatesFunctionRowItem>();
                DistributionType distType = rowItems[0].SelectedDistributionType;
                InterpolationEnum interpType = rowItems[0].SelectedInterpolationType;
                rows.Add(rowItems[0]);
                for (int i = 1; i < rowItems.Count; i++)
                //while (i < _Rows.Count)
                {
                    CoordinatesFunctionRowItem row = rowItems[i];
                    if (row.SelectedDistributionType.Equals(distType) && row.SelectedInterpolationType.Equals(interpType))
                    {
                        rows.Add(rowItems[i]);
                    }
                    else
                    {
                        //the dist type changed
                        CreateTable(rows, tableType);
                        //set the new dist type and add it to the list
                        distType = row.SelectedDistributionType;
                        interpType = row.SelectedInterpolationType;
                        rows = new ObservableCollection<CoordinatesFunctionRowItem>();
                        rows.Add(row);
                    }

                }
                //need to create the final table
                CreateTable(rows, tableType);

                lst_tables.Items.Add(new TableBottomControl(tableType));
            }
        }
        /// <summary>
        /// Turns a coordinates function into a list of row items.
        /// Converts each coordinate of the function into its own row.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public List<CoordinatesFunctionRowItem> CreateRows(ICoordinatesFunction function)
        {
            InterpolationEnum interpolator = function.Interpolator;
            List<CoordinatesFunctionRowItem> rows = new List<CoordinatesFunctionRowItem>();
            foreach (ICoordinate coord in function.Coordinates)
            {
                rows.Add(CreateRowItemFromCoordinate(coord, interpolator));

            }
            return rows;
        }
        private CoordinatesFunctionRowItem CreateRowItemFromCoordinate(ICoordinate coord, InterpolationEnum interpolator)
        {
            double x = coord.X.Value();
            DistributionType type = coord.Y.DistributionType;
            switch (type)
            {
                case DistributionType.Constant:
                    {
                        double y = coord.Y.Value();
                        CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItemBuilder(x)
                                .WithConstantDist(y, interpolator)
                                .Build();
                        return row;
                    }
                case DistributionType.Normal:
                    {
                        IDistributedValue dist = ((Distribution)coord.Y).GetDistribution;
                        CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItemBuilder(x)
                            .WithNormalDist(dist.Mean, dist.StandardDeviation, interpolator)
                            .Build();
                        return row;
                    }
                default:
                    {
                        throw new ArgumentException("The coordinate has a distributed y value that is not supported.");
                    }
            }
        }

        /// <summary>
        /// This gets called anytime a visual update needs to happen to the editor.
        /// It will grab the rows from all the tables, then delete the tables and create tables
        /// from scratch using the rows.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateView(object sender, EventArgs e)
        {
            List<CoordinatesFunctionRowItem> allRows = new List<CoordinatesFunctionRowItem>();
           foreach(CoordinatesFunctionTable table in Tables)
            {
                allRows.AddRange(table.Rows);
            }
           //before we create the tables we need to determine what types of rows we are dealing with and set the 
           //column widths accordingly.

            CreateTables(allRows);
            Save();
        }
       
        //private bool AreAllTablesNone()
        //{
        //    bool retval = true;
        //    foreach (CoordinatesFunctionTable table in Tables)
        //    {
        //        if(!table.TableType.Equals("None"))
        //        {
        //            retval = false;
        //            break;
        //        }
        //    }
        //    return retval;
        //}
        //private void TableRowsModified(TableRowsModifiedEventArgs e)
        //{

        //}
        /// <summary>
        /// Creates a new coordinates function table using the rows passed in.
        /// All rows should have the same distribution type and interpolation type.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="type"></param>
        private void CreateTable(ObservableCollection<CoordinatesFunctionRowItem> rows, ColumnWidths.TableTypes type)
        {
            CoordinatesFunctionTable newTable = new CoordinatesFunctionTable(rows, type);
            newTable.UpdateView += UpdateView;
            newTable.LastRowEnterPressed += NewTable_LastRowEnterPressed;
            newTable.LastRowAndCellTabPressed += NewTable_LastRowAndCellTabPressed;
            if(lst_tables.Items.Count == 1)
            {
                newTable.DisplayXAndInterpolatorHeaders();
            }

            lst_tables.Items.Add(newTable);
            Tables.Add(newTable);
        }

        /// <summary>
        /// Tab was pressed in the last row and editable cell of a table. If it is the last table
        /// in the editor, then add a row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewTable_LastRowAndCellTabPressed(object sender, EventArgs e)
        {
            CoordinatesFunctionTable senderTable = (CoordinatesFunctionTable)sender;
            if (Tables.Last().Equals(senderTable))
            {
                senderTable.AddRow();
            }
        }

        /// <summary>
        /// Enter was pressed in the last row of a table. If it is the last table
        /// in the editor, then add a row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewTable_LastRowEnterPressed(object sender, EventArgs e)
        {
            CoordinatesFunctionTable senderTable = (CoordinatesFunctionTable)sender;
            if(Tables.Last().Equals(senderTable))
            {
                senderTable.AddRow();
            }
        }

        //private class RowItemBuilder
        //{
        //    private double _X;
        //    private double _Y;
        //    private DistributionType _distType;
        //    private InterpolationEnum _interpType;
            
        //    private double _standDev;
        //    private double _min;
        //    private double _max;
        //    private double _mostLikely;
        //    private double _mean;

        //    internal RowItemBuilder(double x)
        //    {
        //        _X = x;
        //        _distType = DistributionType.NotSupported;
        //    }
        //    internal RowItemBuilder WithTriangularDist(double mostLikely, double min, double max, InterpolationEnum interpolator)
        //    {
        //        _mostLikely = mostLikely;
        //        _min = min;
        //        _max = max;
        //        _distType = DistributionType.Triangular;
        //        _interpType = interpolator;
        //        return this;
        //    }
        //    internal RowItemBuilder WithNormalDist(double mean, double standardDeviation, InterpolationEnum interpolator)
        //    {
        //        _distType = DistributionType.Normal;
        //        _mean = mean;
        //        _standDev = standardDeviation;
        //        _interpType = interpolator;
        //        return this;
        //    }

        //    internal RowItemBuilder WithConstantDist(double y, InterpolationEnum interpolator)
        //    {
        //        _Y = y;
        //        _distType = DistributionType.Constant;
        //        _interpType = interpolator;
        //        return this;
        //    }

        //    internal CoordinatesFunctionRowItem Build()
        //    {
        //        return new CoordinatesFunctionRowItem(_X, _Y,_standDev, _mean, _min, _max, _mostLikely, _distType, _interpType);
        //    }

        //}
    }
}
