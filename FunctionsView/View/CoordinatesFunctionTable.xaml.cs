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
using System.Linq;
using Functions;
using Functions.CoordinatesFunctions;

namespace FunctionsView.View
{
    /// <summary>
    /// Interaction logic for CoordinatesFunctionTable.xaml
    /// </summary>
    public partial class CoordinatesFunctionTable : UserControl
    {
        //The table rows modified has nothing to do with cells changing their value.
        //it is about rows getting added or deleted
        //public delegate void TableRowsModified(TableRowsModifiedEventArgs e);
        //public event TableRowsModified RowsModified;
        public event EventHandler UpdateView;
        public event EventHandler LastRowEnterPressed;
        public event EventHandler LastRowAndCellTabPressed;

        //public string TableType { get; set; }
        //public ObservableCollection<CoordinatesFunctionRowItem> Rows
        public CoordinatesFunctionTableVM TableVM
        {
            get;
            set;
        }

        public int SelectedIndex
        {
            get;
            set;
        }

//todo Do i need this constructor?
        public CoordinatesFunctionTable()
        {
            InitializeComponent();
            SelectedIndex = -1;
            col_x.Width = ColumnWidths.COL_X_WIDTH;
            col_dist.Width = ColumnWidths.COL_DIST_WIDTH;
            //col_interp.Width = ColumnWidths.COL_INTERP_WIDTH;
            //TableType = "None";
            dg_table.RowsAdded += Dg_table_RowsAdded;
            dg_table.DataPasted += Dg_table_DataPasted;
            dg_table.RowsDeleted += Dg_table_RowsDeleted;
            dg_table.PreviewLastRowEnter += Dg_table_PreviewLastRowEnter;
            dg_table.PreviewLastRowTab += Dg_table_PreviewLastRowTab;
        }
        /// <summary>
        /// The table type is not the type for this individual table. Is the tableType of the widest table
        /// in the editors list. It gets passed in here to make this table have that same spacing as its neighbors.
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="largestEditorTableType"></param>
        public CoordinatesFunctionTable(CoordinatesFunctionTableVM vm, ColumnWidths.TableTypes largestEditorTableType) : this()
        {
            this.DataContext = this;
            TableVM = vm;
            dg_table.Width = ColumnWidths.TotalColumnWidths(largestEditorTableType) + 8; //had to add 8 to make the lines match up

            if (TableVM.Rows.Count > 0)
            {
                DistributionType distType = TableVM.Rows[0].SelectedDistributionType;
                switch (distType)
                {
                    case DistributionType.Constant:
                        {
                            //TableType = "None";
                            AddNoneColumn(largestEditorTableType);
                            break;
                        }
                    case DistributionType.Normal:
                        {
                            //TableType = "Normal";
                            AddNormalColumns(largestEditorTableType);
                            break;
                        }
                    case DistributionType.Triangular:
                        {
                            //TableType = "Triangular";
                            AddTriangularColumns(largestEditorTableType);
                            break;
                        }
                    case DistributionType.Uniform:
                        {
                            //TableType = "Uniform";
                            AddUniformColumns(largestEditorTableType);
                            break;
                        }
                    case DistributionType.TruncatedNormal:
                        {
                            //TableType = "Triangular";
                            AddTruncatedNormalColumns(largestEditorTableType);
                            break;
                        }
                    case DistributionType.Beta4Parameters:
                        {
                            //TableType = "Triangular";
                            AddBetaColumns(largestEditorTableType);
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

            //string distType = Rows[0].SelectedDistributionType;
            //string interpType = Rows[0].SelectedInterpolationType;
            //CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItem(0, 0, distType, interpType);
            //row.ChangedDistributionType += TableStructureChanged;
            //row.ChangedInterpolationType += TableStructureChanged;
            //Rows.Add( row);
        }

        private void SetFocusToLastRowFirstCell()
        {
            DataGridCellInfo dataGridCellInfo = new DataGridCellInfo(dg_table.Items[TableVM.Rows.Count-1], dg_table.Columns[0]);
            dg_table.SelectedCells.Clear();
            dg_table.SelectedCells.Add(dataGridCellInfo);
        }

        private void Dg_table_RowsDeleted(List<int> rowindices)
        {
            if(TableVM.Rows.Count == 0)
            {

                //this will remove the table from the editor's list
                //UpdateView(this, new EventArgs());
                TableVM.RowDeleted();
            }
        }

        //todo: can i delete this?
        private void Dg_table_DataPasted()
        {
            //every row that had values pasted into it has now lost its event connections
            //i need to reconnect them
            //todo, I really don't like this but i don't know what rows have the events and which don't. Is there a better way?
            //foreach(CoordinatesFunctionRowItem row in Rows)
            //{
            //    row.StructureChanged -= TableStructureChanged;
            //    row.DataChanged -= DataChanged;

            //    //row.ChangedInterpolationType -= TableStructureChanged;

            //    row.StructureChanged += TableStructureChanged;
            //    row.DataChanged += DataChanged;

            //    //row.ChangedInterpolationType += TableStructureChanged;
            //}
        }

        private void Dg_table_RowsAdded(int startrow, int numrows)
        {
            TableVM.AddRows(startrow, numrows);
        }

       

        #region events

        //private void TableStructureChanged(object sender, EventArgs e)
        //{
        //    UpdateView?.Invoke(sender, e);
        //}

        #endregion
        public void DisplayXAndInterpolatorHeaders()
        {
            ObservableCollection<DataGridColumn> columns = dg_table.Columns;
            
            columns[0].Header = "X";
            columns[columns.Count - 1].Header = "Interpolator";
        }

        //private void AddUniformColumns(ColumnWidths.TableTypes largestEditorTableType)
        //{
        //    int[] colWidths = ColumnWidths.DynamicColumnWidths(largestEditorTableType);

        //    List<DataGridColumn> columns = new List<DataGridColumn>();

        //    DataGridTextColumn colMin = new DataGridTextColumn();
        //    colMin.Header = "Min";
        //    colMin.Width = colWidths[1]; //130;
        //    colMin.CanUserReorder = false;
        //    colMin.CanUserResize = false;
        //    colMin.CanUserSort = false;
        //    colMin.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
        //    colMin.Binding = new Binding("Min");
        //    columns.Add(colMin);

        //    DataGridTextColumn colMax = new DataGridTextColumn();
        //    colMax.Header = "Max";
        //    colMax.Width = colWidths[2]; //130;
        //    colMax.CanUserReorder = false;
        //    colMax.CanUserResize = false;
        //    colMax.CanUserSort = false;
        //    colMax.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
        //    colMax.Binding = new Binding("Max");
        //    columns.Add(colMax);

        //    AddColumns(columns);
        //}
        private void AddBetaColumns(ColumnWidths.TableTypes largestEditorTableType)
        {
            int[] colWidths = ColumnWidths.GetComputedTrancatedNormalWidths(largestEditorTableType);

            List<DataGridColumn> columns = new List<DataGridColumn>();

            DataGridTextColumn colAlpha = new DataGridTextColumn();
            colAlpha.Header = "Alpha";
            colAlpha.Width = colWidths[0];
            colAlpha.CanUserReorder = false;
            colAlpha.CanUserResize = false;
            colAlpha.CanUserSort = false;
            colAlpha.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colAlpha.Binding = new Binding("Alpha");
            columns.Add(colAlpha);

            DataGridTextColumn colBeta = new DataGridTextColumn();
            colBeta.Header = "Beta";
            colBeta.Width = colWidths[1];
            colBeta.CanUserReorder = false;
            colBeta.CanUserResize = false;
            colBeta.CanUserSort = false;
            colBeta.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colBeta.Binding = new Binding("Beta");
            columns.Add(colBeta);

            DataGridTextColumn colMin = new DataGridTextColumn();
            colMin.Header = "Min";
            colMin.Width = colWidths[2];
            colMin.CanUserReorder = false;
            colMin.CanUserResize = false;
            colMin.CanUserSort = false;
            colMin.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMin.Binding = new Binding("Min");
            columns.Add(colMin);

            DataGridTextColumn colMax = new DataGridTextColumn();
            colMax.Header = "Max";
            colMax.Width = colWidths[3];
            colMax.CanUserReorder = false;
            colMax.CanUserResize = false;
            colMax.CanUserSort = false;
            colMax.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMax.Binding = new Binding("Max");
            columns.Add(colMax);

            AddColumns(columns);
        }
        private void AddTruncatedNormalColumns(ColumnWidths.TableTypes largestEditorTableType)
        {
            int[] colWidths = ColumnWidths.GetComputedTrancatedNormalWidths(largestEditorTableType);

            List<DataGridColumn> columns = new List<DataGridColumn>();

            DataGridTextColumn colMean = new DataGridTextColumn();
            colMean.Header = "Mean";
            colMean.Width = colWidths[0];
            colMean.CanUserReorder = false;
            colMean.CanUserResize = false;
            colMean.CanUserSort = false;
            colMean.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMean.Binding = new Binding("Mean");
            columns.Add(colMean);

            DataGridTextColumn colStDev = new DataGridTextColumn();
            colStDev.Header = "St Dev";
            colStDev.Width = colWidths[1];
            colStDev.CanUserReorder = false;
            colStDev.CanUserResize = false;
            colStDev.CanUserSort = false;
            colStDev.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colStDev.Binding = new Binding("StandardDeviation");
            columns.Add(colStDev);

            DataGridTextColumn colMin = new DataGridTextColumn();
            colMin.Header = "Min";
            colMin.Width = colWidths[2];
            colMin.CanUserReorder = false;
            colMin.CanUserResize = false;
            colMin.CanUserSort = false;
            colMin.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMin.Binding = new Binding("Min");
            columns.Add(colMin);

            DataGridTextColumn colMax = new DataGridTextColumn();
            colMax.Header = "Max";
            colMax.Width = colWidths[3]; 
            colMax.CanUserReorder = false;
            colMax.CanUserResize = false;
            colMax.CanUserSort = false;
            colMax.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMax.Binding = new Binding("Max");
            columns.Add(colMax);

            AddColumns(columns);
        }

        private void AddTriangularColumns(ColumnWidths.TableTypes largestEditorTableType)
        {
            int[] colWidths = ColumnWidths.GetComputedTriangularWidths(largestEditorTableType);

            List<DataGridColumn> columns = new List<DataGridColumn>();

            DataGridTextColumn colMostLikely = new DataGridTextColumn();
            colMostLikely.Header = "Most Likely";
            colMostLikely.Width = colWidths[0]; //130;
            colMostLikely.CanUserReorder = false;
            colMostLikely.CanUserResize = false;
            colMostLikely.CanUserSort = false;
            colMostLikely.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMostLikely.Binding = new Binding("MostLikely");
            columns.Add(colMostLikely);

            DataGridTextColumn colMin = new DataGridTextColumn();
            colMin.Header = "Min";
            colMin.Width = colWidths[1]; //130;
            colMin.CanUserReorder = false;
            colMin.CanUserResize = false;
            colMin.CanUserSort = false;
            colMin.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMin.Binding = new Binding("Min");
            columns.Add(colMin);

            DataGridTextColumn colMax = new DataGridTextColumn();
            colMax.Header = "Max";
            colMax.Width = colWidths[2]; //130;
            colMax.CanUserReorder = false;
            colMax.CanUserResize = false;
            colMax.CanUserSort = false;
            colMax.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMax.Binding = new Binding("Max");
            columns.Add(colMax);

            AddColumns(columns);
        }

        private void AddUniformColumns(ColumnWidths.TableTypes largestEditorTableType)
        {
            int[] colWidths = ColumnWidths.GetComputedUniformWidths(largestEditorTableType);

            List<DataGridColumn> columns = new List<DataGridColumn>();

            DataGridTextColumn colMin = new DataGridTextColumn();
            colMin.Header = "Min";
            colMin.Width = colWidths[0]; //130;
            colMin.CanUserReorder = false;
            colMin.CanUserResize = false;
            colMin.CanUserSort = false;
            colMin.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMin.Binding = new Binding("Min");
            columns.Add(colMin);

            DataGridTextColumn colMax = new DataGridTextColumn();
            colMax.Header = "Max";
            colMax.Width = colWidths[1]; //130;
            colMax.CanUserReorder = false;
            colMax.CanUserResize = false;
            colMax.CanUserSort = false;
            colMax.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMax.Binding = new Binding("Max");
            columns.Add(colMax);

            AddColumns(columns);
        }
        private void AddNormalColumns(ColumnWidths.TableTypes largestEditorTableType)
        {
            int[] colWidths = ColumnWidths.GetComputedNormalWidths(largestEditorTableType);

            List<DataGridColumn> columns = new List<DataGridColumn>();
            DataGridTextColumn colMean = new DataGridTextColumn();
            colMean.Header = "Mean";
            colMean.Width = colWidths[0]; //130;
            colMean.CanUserReorder = false;
            colMean.CanUserResize = false;
            colMean.CanUserSort = false;
            colMean.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMean.Binding = new Binding("Mean");
            columns.Add(colMean);

            DataGridTextColumn colStDev = new DataGridTextColumn();
            colStDev.Header = "St Dev";
            colStDev.Width = colWidths[1]; //130;
            colStDev.CanUserReorder = false;
            colStDev.CanUserResize = false;
            colStDev.CanUserSort = false;
            colStDev.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colStDev.Binding = new Binding("StandardDeviation");
            columns.Add(colStDev);

            AddColumns(columns);
        }


        private void AddNoneColumn(ColumnWidths.TableTypes largestEditorTableType)
        {
            int colWidth = ColumnWidths.TotalDynamicColumnWidths(largestEditorTableType);

            DataGridTextColumn col = new DataGridTextColumn();
            col.Header = "Value (Constant)";
            col.Width = colWidth; //260;
            col.CanUserReorder = false;
            col.CanUserResize = false;
            col.CanUserSort = false;
            col.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            col.Binding = new Binding("Y");
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

       

      
    }
}
