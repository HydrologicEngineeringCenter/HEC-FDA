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

        public string TableType { get; set; }
        public ObservableCollection<CoordinatesFunctionRowItem> Rows
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
            TableType = "None";
            dg_table.RowsAdded += Dg_table_RowsAdded;
            dg_table.DataPasted += Dg_table_DataPasted;
            dg_table.RowsDeleted += Dg_table_RowsDeleted;
            dg_table.PreviewLastRowEnter += Dg_table_PreviewLastRowEnter;
            dg_table.PreviewLastRowTab += Dg_table_PreviewLastRowTab;
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

        public void AddRow()
        {
            string distType = Rows[0].SelectedDistributionType;
            string interpType = Rows[0].SelectedInterpolationType;
            CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItem(0, 0, distType, interpType);
            row.ChangedDistributionType += TableStructureChanged;
            row.ChangedInterpolationType += TableStructureChanged;
            Rows.Add(row);
            SetFocusToLastRowFirstCell();
        }

        private void SetFocusToLastRowFirstCell()
        {
            DataGridCellInfo dataGridCellInfo = new DataGridCellInfo(dg_table.Items[Rows.Count-1], dg_table.Columns[0]);
            dg_table.SelectedCells.Clear();
            dg_table.SelectedCells.Add(dataGridCellInfo);
        }

        private void Dg_table_RowsDeleted(List<int> rowindices)
        {
            if(Rows.Count == 0)
            {
                //this will remove the table from the parents list
                UpdateView(this, new EventArgs());
            }
        }

        private void Dg_table_DataPasted()
        {
            //every row that had values pasted into it has now lost its event connections
            //i need to reconnect them
            //todo, I really don't like this but i don't know what rows have the events and which don't. Is there a better way?
            foreach(CoordinatesFunctionRowItem row in Rows)
            {
                row.ChangedDistributionType -= TableStructureChanged;
                row.ChangedInterpolationType -= TableStructureChanged;

                row.ChangedDistributionType += TableStructureChanged;
                row.ChangedInterpolationType += TableStructureChanged;
            }
        }

        private void Dg_table_RowsAdded(int startrow, int numrows)
        {
            //pre-fill the dist type and the interp type for each new row
            string distType = Rows[0].SelectedDistributionType;
            string interpType = Rows[0].SelectedInterpolationType;
            for(int i = 0;i<numrows;i++)
            {
                CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItem(0, 0, distType, interpType);
                row.ChangedDistributionType += TableStructureChanged;
                row.ChangedInterpolationType += TableStructureChanged;
                Rows.Insert(startrow, row);
            }
        }

        public CoordinatesFunctionTable(ObservableCollection<CoordinatesFunctionRowItem> rows, ColumnWidths.TableTypes tableType):this()
        {

            dg_table.Width = ColumnWidths.TotalColumnWidths(tableType) + 8;
               
            //InitializeComponent();
            foreach(CoordinatesFunctionRowItem row in rows)
            {
                row.ChangedDistributionType += TableStructureChanged;
                row.ChangedInterpolationType += TableStructureChanged;
            }
            this.DataContext = this;
            Rows = rows;
            if (rows.Count > 0)
            {
                string distType = rows[0].SelectedDistributionType;
                switch (distType)
                {
                    case "None":
                        {
                            TableType = "None";
                            AddNoneColumn(tableType);
                            break;
                        }
                    case "Normal":
                        {
                            TableType = "Normal";
                            AddNormalColumns(tableType);
                            break;
                        }
                    case "Triangular":
                        {
                            TableType = "Triangular";
                            AddTriangularColumns(tableType);
                            break;
                        }
                }
            }
        }

        #region events

        private void TableStructureChanged(object sender, EventArgs e)
        {
            UpdateView?.Invoke(sender, e);
        }

        #endregion
        public void DisplayXAndInterpolatorHeaders()
        {
            ObservableCollection<DataGridColumn> columns = dg_table.Columns;
            
            columns[0].Header = "X";
            columns[columns.Count - 1].Header = "Interpolator";
        }

        private void AddTriangularColumns(ColumnWidths.TableTypes tableType)
        {
            int[] colWidths = ColumnWidths.DynamicColumnWidths(tableType);

            List<DataGridColumn> columns = new List<DataGridColumn>();

            DataGridTextColumn colMostLikely = new DataGridTextColumn();
            colMostLikely.Header = "Most Likely";
            colMostLikely.Width = colWidths[0]; //130;
            colMostLikely.CanUserReorder = false;
            colMostLikely.CanUserResize = false;
            colMostLikely.CanUserSort = false;
            colMostLikely.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMostLikely.Binding = new Binding("Distribution.StandardDeviation");
            columns.Add(colMostLikely);

            DataGridTextColumn colMin = new DataGridTextColumn();
            colMin.Header = "Min";
            colMin.Width = colWidths[1]; //130;
            colMin.CanUserReorder = false;
            colMin.CanUserResize = false;
            colMin.CanUserSort = false;
            colMin.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMin.Binding = new Binding("Mean");
            columns.Add(colMin);

            DataGridTextColumn colMax = new DataGridTextColumn();
            colMax.Header = "Max";
            colMax.Width = colWidths[2]; //130;
            colMax.CanUserReorder = false;
            colMax.CanUserResize = false;
            colMax.CanUserSort = false;
            colMax.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMax.Binding = new Binding("Distribution.StandardDeviation");
            columns.Add(colMax);

            AddColumns(columns);
        }
        private void AddNormalColumns(ColumnWidths.TableTypes tableType)
        {
            int[] colWidths = ColumnWidths.GetComputedNormalWidths(tableType);

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
            colStDev.Binding = new Binding("Distribution.StandardDeviation");
            columns.Add(colStDev);

            AddColumns(columns);
        }


        private void AddNoneColumn(ColumnWidths.TableTypes tableType)
        {
            int colWidth = ColumnWidths.TotalDynamicColumnWidths(tableType);

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
