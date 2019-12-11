using Functions;
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
        
        
        /// <summary>
        /// This enum is to aid in determining what the column widths should be.
        /// The enum basically describes the type of all the tables combined.
        /// </summary>
        
        public CoordinatesFunctionEditor()
        {
            InitializeComponent();
            Tables = new List<CoordinatesFunctionTable>();
        }
        //public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(List<CoordinatesFunctionRowItem>), typeof(CoordinatesFunctionEditor), new FrameworkPropertyMetadata(new List<CoordinatesFunctionRowItem>(), new PropertyChangedCallback(RowsChangedCallBack)));
        public static readonly DependencyProperty FunctionProperty = DependencyProperty.Register("Function", typeof(ICoordinatesFunction), typeof(CoordinatesFunctionEditor), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(FunctionChangedCallBack)));

        //private List<CoordinatesFunctionRowItem> _Rows;
        //public List<CoordinatesFunctionRowItem> Rows
        //{
        //    get { return _Rows; }
        //    set
        //    {
        //        _Rows = value;
        //        foreach (CoordinatesFunctionRowItem row in _Rows)
        //        {
        //            row.ChangedDistributionType += UpdateView;
        //            row.ChangedInterpolationType += UpdateView;
        //        }
        //    }
        //}

        public List<CoordinatesFunctionTable> Tables
        {
            get;
            set;
        }

        public ICoordinatesFunction Function
        {
            get;
            set;
        }
      

        //private static void RowsChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    CoordinatesFunctionEditor owner = d as CoordinatesFunctionEditor;
        //    owner.Rows = e.NewValue as List<CoordinatesFunctionRowItem>;
        //    //call some sort of update method that re sorts and organizes the tables and replots.
        //    owner.UpdateView(owner, new EventArgs());
        //}

        private static void FunctionChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CoordinatesFunctionEditor owner = d as CoordinatesFunctionEditor;
            owner.CreateTables( e.NewValue as ICoordinatesFunction);
            //call some sort of update method that re sorts and organizes the tables and replots.
            owner.UpdateView(owner, new EventArgs());
        }

        private void CreateTables(ICoordinatesFunction function)
        {
            List<CoordinatesFunctionRowItem> rows = CreateRows(function);
            CreateTables(rows);
        }

        #region Determine column widths
        
      

        #endregion
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
                string distType = rowItems[0].SelectedDistributionType;
                string interpType = rowItems[0].SelectedInterpolationType;
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
        public List<CoordinatesFunctionRowItem> CreateRows(ICoordinatesFunction function)
        {
            List<CoordinatesFunctionRowItem> rows = new List<CoordinatesFunctionRowItem>();
            foreach (ICoordinate coord in function.Coordinates)
            {
                rows.Add(CreateRowItemFromCoordinate(coord));

            }
            return rows;
        }
        private CoordinatesFunctionRowItem CreateRowItemFromCoordinate(ICoordinate coord)
        {
            //we know that x is always constant
            double x = coord.X.Value();
            //todo if we add a dist type for "None" then i can get rid of the "else" here and 
            //just include it in the switch.
            if (coord.Y.GetType().Equals(typeof(Distribution)))
            {
                IDistributedValue dist = ((Distribution)coord.Y).GetDistribution;
                DistributionType type = dist.Type;
                switch (type)
                {
                    case DistributionType.Normal:
                        {

                            CoordinatesFunctionRowItem row = new RowItemBuilder(x)
                                .WithNormalDist(dist.Skewness, dist.StandardDeviation)
                                .Build();
                            row.SelectedInterpolationType = "Linear";
                            //row.ChangedDistributionType += RowChangedDistributionType;
                            return row;
                        }
                    default:
                        {
                            throw new ArgumentException("The coordinate has a distributed y value that is not an allowed distribution type.");
                        }
                }

            }
            else
            {
                //then y is a constant
                double y = coord.Y.Value();
                CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItem(x, y);
                //row.ChangedDistributionType += RowChangedDistributionType;
                return row;
            }
        }

        
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
            


        }
       
        private bool AreAllTablesNone()
        {
            bool retval = true;
            foreach (CoordinatesFunctionTable table in Tables)
            {
                if(!table.TableType.Equals("None"))
                {
                    retval = false;
                    break;
                }
            }
            return retval;
        }
        private void TableRowsModified(TableRowsModifiedEventArgs e)
        {

        }
        private void CreateTable(ObservableCollection<CoordinatesFunctionRowItem> rows, ColumnWidths.TableTypes type)
        {
            //i need to add the "X" and the "Interpolator" headers on the top table only
            //the lst_tables' first item is the table topper. So add to the second item
            CoordinatesFunctionTable newTable = new CoordinatesFunctionTable(rows, type);
            newTable.RowsModified += TableRowsModified;
            newTable.UpdateView += UpdateView;
            if(lst_tables.Items.Count == 1)
            {
                newTable.DisplayXAndInterpolatorHeaders();
            }

            lst_tables.Items.Add(newTable);
            Tables.Add(newTable);
            //switch (distType)
            //{
            //    case "None":
            //        {

            //            lst_tables.Items.Add(new CoordinateFunctionTable(rows));  //new NoneTable(rows));
            //            return;
            //        }
            //    case "Normal":
            //        {

            //            lst_tables.Items.Add(new CoordinateFunctionTable(rows)); //new NormalTable(rows));
            //            return;
            //        }
            //    case "Triangular":
            //        {
            //            lst_tables.Items.Add(new TriangularTable(rows));
            //                return;
            //        }

            //}

        }

        //private List<CoordinatesFunctionRowItem> GetRowsAboveRow(CoordinatesFunctionRowItem row)
        //{
        //    List<CoordinatesFunctionRowItem> rows = new List<CoordinatesFunctionRowItem>();
        //    int i;
        //    for (i = 0; i < Rows.Count; i++)
        //    {
        //        if (Rows[i] == row)
        //        {
        //            break;
        //        }
        //        else
        //        {
        //            rows.Add(Rows[i]);
        //        }
        //    }
        //    return rows;
        //}
        //private List<CoordinatesFunctionRowItem> GetRowsBelowRow(CoordinatesFunctionRowItem row)
        //{
        //    int i;
        //    for (i = 0; i < Rows.Count; i++)
        //    {
        //        if (Rows[i] == row)
        //        {
        //            break;
        //        }
        //    }
        //    List<CoordinatesFunctionRowItem> rows = new List<CoordinatesFunctionRowItem>();
        //    i++;
        //    for (int j = i; j < Rows.Count; j++)
        //    {
        //        rows.Add(Rows[j]);
        //    }
        //    return rows;
        //}

        private class RowItemBuilder
        {
            private double _X;
            private double _Y;
            private DistributionType _distType;
            private double _skew;
            private double _standDev;

            internal RowItemBuilder(double x)
            {
                _X = x;
                _distType = DistributionType.NotSupported;
            }

            internal RowItemBuilder WithNormalDist(double skew, double standardDeviation)
            {
                _distType = DistributionType.Normal;
                _skew = skew;
                _standDev = standardDeviation;
                return this;
            }

            internal RowItemBuilder WithNoneDist(double y)
            {
                _Y = y;
                //todo john, i think we need to add a "None" option to the dist enums.
                return this;
            }

            internal CoordinatesFunctionRowItem Build()
            {
                return new CoordinatesFunctionRowItem(_X, _Y);
            }

        }
    }
}
