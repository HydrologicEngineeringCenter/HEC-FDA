using FunctionsView.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Functions;
using System.Windows.Controls.Primitives;

namespace FunctionsView.View
{
    /// <summary>
    /// Interaction logic for CoordinatesFunctionTable.xaml
    /// </summary>
    public partial class CoordinatesFunctionTable : UserControl
    {
        public event EventHandler LastRowEnterPressed;
        public event EventHandler LastRowAndCellTabPressed;
        public event EventHandler LastRowDownArrowPressed;
        public event EventHandler FirstRowUpArrowPressed;
        //column header constants
        private readonly string X = "X";
        private readonly string INTERPOLATOR = "Interpolator";
        private readonly string ALPHA = "Alpha";
        private readonly string BETA = "Beta";
        private readonly string MIN = "Min";
        private readonly string MAX = "Max";
        private readonly string MEAN = "Mean";
        private readonly string Y = "Value (Constant)";
        private readonly string ST_DEV = "St Dev";
        private readonly string MOST_LIKELY = "Most Likely";

        public CoordinatesFunctionTableVM TableVM
        {
            get;
            set;
        }

        public CoordinatesFunctionTable()
        {
            InitializeComponent();
            col_x.Width = ColumnWidths.COL_X_WIDTH;
            col_dist.Width = ColumnWidths.COL_DIST_WIDTH;
            dg_table.RowsAdded += Dg_table_RowsAdded;
            dg_table.RowsDeleted += Dg_table_RowsDeleted;
            dg_table.PreviewLastRowEnter += Dg_table_PreviewLastRowEnter;
            dg_table.PreviewLastRowTab += Dg_table_PreviewLastRowTab;
            dg_table.ArrowDownInLastRow += Dg_table_ArrowDownInLastRow;
            dg_table.ArrowUpInFirstRow += Dg_table_ArrowUpInFirstRow;
        }

        private void Dg_table_ArrowUpInFirstRow()
        {
            FirstRowUpArrowPressed?.Invoke(this, new EventArgs());
        }

        private void Dg_table_ArrowDownInLastRow()
        {
            LastRowDownArrowPressed?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// The table type is not the type for this individual table. Is the tableType of the widest table
        /// in the editors list. It gets passed in here to make this table have that same spacing as its neighbors.
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="largestEditorTableType"></param>
        public CoordinatesFunctionTable(CoordinatesFunctionTableVM vm, int[] columnWidths) : this()
        {
            this.DataContext = this;
            TableVM = vm;
            dg_table.Width = ColumnWidths.TotalColumnWidths(columnWidths) + 8; //had to add 8 to make the lines match up

            if (TableVM.Rows.Count > 0)
            {
                DistributionType distType = TableVM.Rows[0].SelectedDistributionType;
                switch (distType)
                {
                    case DistributionType.Constant:
                        {
                            AddNoneColumn(columnWidths);
                            break;
                        }
                    case DistributionType.Normal:
                        {
                            AddNormalColumns(columnWidths);
                            break;
                        }
                    case DistributionType.Triangular:
                        {
                            AddTriangularColumns(columnWidths);
                            break;
                        }
                    case DistributionType.Uniform:
                        {
                            AddUniformColumns(columnWidths);
                            break;
                        }
                    case DistributionType.TruncatedNormal:
                        {
                            AddTruncatedNormalColumns(columnWidths);
                            break;
                        }
                    case DistributionType.Beta4Parameters:
                        {
                            AddBetaColumns(columnWidths);
                            break;
                        }
                }
            }
        }

        private void Dg_table_PreviewLastRowTab(int cellIndex)
        {
            //the user hit the tab key while in the last row
            //but we dont know if this table is the last table in the list
            //pass the event on up to the editor and let it decide what to do
            //we didn't want the datagrid itself to determine if the tabbed cell was 
            //the last cell in the row because that isn't actually what we want. We
            //want to add the new row if the tabbed cell is the last dynamic column. (count - 3)
            if(cellIndex == dg_table.Columns.Count -3)
            {
                LastRowAndCellTabPressed?.Invoke(this, new EventArgs());
            }
        }

        private void Dg_table_PreviewLastRowEnter()
        {
            //the user hit the enter key while in the last row of this table
            //but we dont know if this table is the last table in the list
            //pass the event on up to the editor and let it decide what to do
            LastRowEnterPressed?.Invoke(this, new EventArgs());
        }

        private void Dg_table_RowsDeleted(List<int> rowindices)
        {
            //if(TableVM.Rows.Count == 0)
            {
                TableVM.DeleteRows(rowindices);
                //TableVM.RowDeleted();
            }
        }

        private void Dg_table_RowsAdded(int startrow, int numrows)
        {
            TableVM.AddRows(startrow, numrows);
        }

        #region events

        #endregion
        /// <summary>
        /// Only the top table in the editor displays the "X" and "Interpolator" column headers.
        /// When the editor craetes the first table it will call this.
        /// </summary>
        public void DisplayXAndInterpolatorHeaders()
        {
            ObservableCollection<DataGridColumn> columns = dg_table.Columns;
            
            columns[0].Header = X;
            columns[columns.Count - 1].Header = INTERPOLATOR;
        }
      
        #region AddColumns
        private Binding CreateBinding(String name)
        {
            Binding binding = new Binding(name);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            return binding;
        }
        private void AddBetaColumns(int[] columnWidths)
        {
            int[] betaColumnWidths = ColumnWidths.GetComputedBetaWidths(columnWidths);

            List<DataGridColumn> columns = new List<DataGridColumn>();

            DataGridTextColumn colAlpha = new DataGridTextColumn();
            colAlpha.Header = ALPHA;
            colAlpha.Width = betaColumnWidths[0];
            colAlpha.CanUserReorder = false;
            colAlpha.CanUserResize = false;
            colAlpha.CanUserSort = false;
            colAlpha.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colAlpha.Binding = CreateBinding("Alpha");
            columns.Add(colAlpha);

            DataGridTextColumn colBeta = new DataGridTextColumn();
            colBeta.Header = BETA;
            colBeta.Width = betaColumnWidths[1];
            colBeta.CanUserReorder = false;
            colBeta.CanUserResize = false;
            colBeta.CanUserSort = false;
            colBeta.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colBeta.Binding = CreateBinding("Beta");
            columns.Add(colBeta);

            DataGridTextColumn colMin = new DataGridTextColumn();
            colMin.Header = MIN;
            colMin.Width = betaColumnWidths[2];
            colMin.CanUserReorder = false;
            colMin.CanUserResize = false;
            colMin.CanUserSort = false;
            colMin.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMin.Binding = CreateBinding("Min");
            columns.Add(colMin);

            DataGridTextColumn colMax = new DataGridTextColumn();
            colMax.Header = MAX;
            colMax.Width = betaColumnWidths[3];
            colMax.CanUserReorder = false;
            colMax.CanUserResize = false;
            colMax.CanUserSort = false;
            colMax.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMax.Binding = CreateBinding("Max");
            columns.Add(colMax);

            AddColumns(columns);
        }
        private void AddTruncatedNormalColumns(int[] columnWidths)
        {
            int[] truncatedNormalColumnWidths = ColumnWidths.GetComputedTrancatedNormalWidths(columnWidths);

            List<DataGridColumn> columns = new List<DataGridColumn>();

            DataGridTextColumn colMean = new DataGridTextColumn();
            colMean.Header = MEAN;
            colMean.Width = truncatedNormalColumnWidths[0];
            colMean.CanUserReorder = false;
            colMean.CanUserResize = false;
            colMean.CanUserSort = false;
            colMean.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMean.Binding = CreateBinding("Mean");
            columns.Add(colMean);

            DataGridTextColumn colStDev = new DataGridTextColumn();
            colStDev.Header = ST_DEV;
            colStDev.Width = truncatedNormalColumnWidths[1];
            colStDev.CanUserReorder = false;
            colStDev.CanUserResize = false;
            colStDev.CanUserSort = false;
            colStDev.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colStDev.Binding = CreateBinding("StandardDeviation");
            columns.Add(colStDev);

            DataGridTextColumn colMin = new DataGridTextColumn();
            colMin.Header = MIN;
            colMin.Width = truncatedNormalColumnWidths[2];
            colMin.CanUserReorder = false;
            colMin.CanUserResize = false;
            colMin.CanUserSort = false;
            colMin.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMin.Binding = CreateBinding("Min");
            columns.Add(colMin);

            DataGridTextColumn colMax = new DataGridTextColumn();
            colMax.Header = MAX;
            colMax.Width = truncatedNormalColumnWidths[3]; 
            colMax.CanUserReorder = false;
            colMax.CanUserResize = false;
            colMax.CanUserSort = false;
            colMax.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMax.Binding = CreateBinding("Max");
            columns.Add(colMax);

            AddColumns(columns);
        }

        private void AddTriangularColumns(int[] columnWidths)
        {
            int[] triangularColumnWidths = ColumnWidths.GetComputedTriangularWidths(columnWidths);

            List<DataGridColumn> columns = new List<DataGridColumn>();

            DataGridTextColumn colMostLikely = new DataGridTextColumn();
            colMostLikely.Header = MOST_LIKELY;
            colMostLikely.Width = triangularColumnWidths[0];
            colMostLikely.CanUserReorder = false;
            colMostLikely.CanUserResize = false;
            colMostLikely.CanUserSort = false;
            colMostLikely.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMostLikely.Binding = CreateBinding("MostLikely");
            columns.Add(colMostLikely);

            DataGridTextColumn colMin = new DataGridTextColumn();
            colMin.Header = MIN;
            colMin.Width = triangularColumnWidths[1];
            colMin.CanUserReorder = false;
            colMin.CanUserResize = false;
            colMin.CanUserSort = false;
            colMin.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMin.Binding = CreateBinding("Min");
            columns.Add(colMin);

            DataGridTextColumn colMax = new DataGridTextColumn();
            colMax.Header = MAX;
            colMax.Width = triangularColumnWidths[2];
            colMax.CanUserReorder = false;
            colMax.CanUserResize = false;
            colMax.CanUserSort = false;
            colMax.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMax.Binding = CreateBinding("Max");
            columns.Add(colMax);

            AddColumns(columns);
        }

        private void AddUniformColumns(int[] columnWidths)
        {
            int[] uniformColumnWidths = ColumnWidths.GetComputedUniformWidths(columnWidths);

            List<DataGridColumn> columns = new List<DataGridColumn>();

            DataGridTextColumn colMin = new DataGridTextColumn();
            colMin.Header = MIN;
            colMin.Width = uniformColumnWidths[0];
            colMin.CanUserReorder = false;
            colMin.CanUserResize = false;
            colMin.CanUserSort = false;
            colMin.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMin.Binding = CreateBinding("Min");
            columns.Add(colMin);

            DataGridTextColumn colMax = new DataGridTextColumn();
            colMax.Header = MAX;
            colMax.Width = uniformColumnWidths[1]; 
            colMax.CanUserReorder = false;
            colMax.CanUserResize = false;
            colMax.CanUserSort = false;
            colMax.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMax.Binding = CreateBinding("Max");
            columns.Add(colMax);

            AddColumns(columns);
        }

        private void AddNormalColumns(int[] columnWidths)
        {
            int[] normalColumnWidths = ColumnWidths.GetComputedNormalWidths(columnWidths);

            List<DataGridColumn> columns = new List<DataGridColumn>();
            DataGridTextColumn colMean = new DataGridTextColumn();
            colMean.Header = MEAN;
            colMean.Width = normalColumnWidths[0];
            colMean.CanUserReorder = false;
            colMean.CanUserResize = false;
            colMean.CanUserSort = false;
            colMean.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMean.Binding = CreateBinding("Mean");
            columns.Add(colMean);

            DataGridTextColumn colStDev = new DataGridTextColumn();
            colStDev.Header = ST_DEV;
            colStDev.Width = normalColumnWidths[1];
            colStDev.CanUserReorder = false;
            colStDev.CanUserResize = false;
            colStDev.CanUserSort = false;
            colStDev.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colStDev.Binding = CreateBinding("StandardDeviation");
            columns.Add(colStDev);

            AddColumns(columns);
        }


        private void AddNoneColumn(int[] columnWidths)
        {
            int colWidth = columnWidths.Sum();

            DataGridTextColumn col = new DataGridTextColumn();
            col.Header = Y;
            col.Width = colWidth; //260;
            col.CanUserReorder = false;
            col.CanUserResize = false;
            col.CanUserSort = false;
            col.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            col.Binding = CreateBinding("Y");
            AddColumns(new List<DataGridColumn>() { col });
        }

        /// <summary>
        /// Columns will get inserted at position 1, just after the "X" column
        /// </summary>
        /// <param name="columnsToInsert"></param>
        private void AddColumns(List<DataGridColumn> columnsToInsert)
        {
            for (int i = 0; i < columnsToInsert.Count; i++)
            {
                dg_table.Columns.Insert(i + 1, columnsToInsert[i]);
            }
        }
        #endregion

        #region Select Cell

        //I needed a way to select individual cells. This appears to be harder than it should be. I found the 
        //following methods on line and it seems to work great. Cells need to get the focus when jumping from the
        //last row of one table to the first row in another table. Also when adding rows by tabbing or pressing 
        //enter in the last row which will create a new row.
        public void SelectCellByIndex( int rowIndex, int columnIndex)
        {
            if (!dg_table.SelectionUnit.Equals(DataGridSelectionUnit.Cell))
                throw new ArgumentException("The SelectionUnit of the DataGrid must be set to Cell.");

            if (rowIndex < 0 || rowIndex > (dg_table.Items.Count - 1))
                throw new ArgumentException(string.Format("{0} is an invalid row index.", rowIndex));

            if (columnIndex < 0 || columnIndex > (dg_table.Columns.Count - 1))
                throw new ArgumentException(string.Format("{0} is an invalid column index.", columnIndex));

            dg_table.SelectedCells.Clear();

            object item = dg_table.Items[rowIndex];
            DataGridRow row = dg_table.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            if (row == null)
            {
                dg_table.ScrollIntoView(item);
                row = dg_table.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            }
            if (row != null)
            {
                DataGridCell cell = GetCell( row, columnIndex);
                if (cell != null)
                {
                    DataGridCellInfo dataGridCellInfo = new DataGridCellInfo(cell);
                    dg_table.SelectedCells.Add(dataGridCellInfo);
                    cell.Focus();
                }
            }
        }

        public DataGridCell GetCell(DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    /* if the row has been virtualized away, call its ApplyTemplate() method
                     * to build its visual tree in order for the DataGridCellsPresenter
                     * and the DataGridCells to be created */
                    rowContainer.ApplyTemplate();
                    presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    if (cell == null)
                    {
                        /* bring the column into view
                         * in case it has been virtualized away */
                        dg_table.ScrollIntoView(rowContainer, dg_table.Columns[column]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    }
                    return cell;
                }
            }
            return null;
        }

        public T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        #endregion

        private void dg_table_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            TableVM.CellEditEnding();
        }
    }
}
