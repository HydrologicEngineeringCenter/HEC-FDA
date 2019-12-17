using Functions;
using Functions.CoordinatesFunctions;
using FunctionsView.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsView.ViewModel
{
    public class CoordinatesFunctionTableVM
    {
        //NOTE: Any row that gets added to a table needs to assign the RowIsLeavingTable event
        public delegate void RowIsLeavingEventHandler(RowLeavingEventArgs e);
        public event RowIsLeavingEventHandler RowIsLeavingTable;
        public event EventHandler NoMoreRows;

        public DistributionType DistributionType
        {
            get { return Rows[0].SelectedDistributionType; }
        }
        public InterpolationEnum InterpolationType
        {
            get { return Rows[0].SelectedInterpolationType; }
        }
        public ObservableCollection<CoordinatesFunctionRowItem> Rows { get; set; }
        //public ColumnWidths.TableTypes TableType { get; set; }

        public CoordinatesFunctionTableVM(ObservableCollection<CoordinatesFunctionRowItem> rows)
        {
            Rows = rows;
            //TableType = type;
            foreach (CoordinatesFunctionRowItem row in rows)
            {
                row.RowIsLeavingTable += Row_RowIsLeavingTable;
            }
        }

        public void RowDeleted()
        {
            if (Rows.Count == 0)
            {
                NoMoreRows?.Invoke(this, new EventArgs());
            }
        }

        private void Row_RowIsLeavingTable(object sender, EventArgs e)
        {
            CoordinatesFunctionRowItem row = sender as CoordinatesFunctionRowItem;
            int rowIndex = Rows.IndexOf(row);
            RowLeavingEventArgs args = new RowLeavingEventArgs(this, row);
            //this row is leaving this table
            //This table needs to remove its listener to the row
            row.RowIsLeavingTable -= Row_RowIsLeavingTable;
            //get rid of this row from this table
            Rows.Remove(row);
            RowIsLeavingTable?.Invoke(args);
            //if there are no more rows in the table then raise event so that the editor
            //can delete this table
            if (Rows.Count == 0)
            {
                NoMoreRows?.Invoke(this, new EventArgs());
            }
        }

        

        public void AddRow(CoordinatesFunctionRowItem row)
        {
            row.RowIsLeavingTable += Row_RowIsLeavingTable;
            Rows.Add(row);
        }

        public void AddRow()
        {
            if (Rows.Count > 0)
            {
                Rows.Add(CreateDefaultRow());
            }
            //todo: John, do we allow a table to have no rows? Right now i don't, when the last row is deleted i delete the table.
        }
        /// <summary>
        /// This gets called by the table in the view for the context menu options: Add, insert
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="numRows"></param>
        public void AddRows(int startRow, int numRows)
        {
            for (int i = 0; i < numRows; i++)
            {
                Rows.Insert(startRow, CreateDefaultRow());
            }
        }

        public void InsertRow(int index, CoordinatesFunctionRowItem row)
        {
            row.RowIsLeavingTable += Row_RowIsLeavingTable;
            Rows.Insert(index, row);
        }

        public void InsertRow(int index)
        {
            Rows.Insert(index, CreateDefaultRow());
        }

        //public void DeleteRow(int index)
        //{
        //    Rows.RemoveAt(index);
        //}

        /// <summary>
        /// Grabs the distribution type and interpolation type from the first row
        /// and then adds a row to the end of the table with that dist type and interp type.
        /// </summary>
        /// <returns></returns>
        private CoordinatesFunctionRowItem CreateDefaultRow()
        {
            InterpolationEnum interpType = Rows[0].SelectedInterpolationType;
            DistributionType distType = Rows[0].SelectedDistributionType;
            CoordinatesFunctionRowItem row = null;
            switch (distType)
            {
                case DistributionType.Constant:
                    {
                        row = new CoordinatesFunctionRowItemBuilder(0).WithConstantDist(0, interpType).Build();
                        break;
                    }
                case DistributionType.Normal:
                    {
                        row = new CoordinatesFunctionRowItemBuilder(0).WithNormalDist(0, 0, interpType).Build();
                        break;
                    }
                case DistributionType.Triangular:
                    {
                        row = new CoordinatesFunctionRowItemBuilder(0).WithTriangularDist(0, 0, 0, interpType).Build();
                        break;
                    }
            }
            row.RowIsLeavingTable += Row_RowIsLeavingTable;
            return row;
        }

        public class RowLeavingEventArgs : EventArgs
        {
            public CoordinatesFunctionRowItem Row { get; set; }
            public CoordinatesFunctionTableVM Table { get; set; }
            public RowExtractionType ExtractionType { get; set; }
            //public ObservableCollection<CoordinatesFunctionRowItem> RowsBeforeRow { get; set; }
            public ObservableCollection<CoordinatesFunctionRowItem> RowsAfterRow { get; set; }

            public RowLeavingEventArgs(CoordinatesFunctionTableVM table, CoordinatesFunctionRowItem row)
            {
                Table = table;
                Row = row.Clone();
                ExtractionType = GetRowExtractionType(table.Rows, row);
                if(ExtractionType == RowExtractionType.Splitting)
                {
                    //RowsBeforeRow = GetRowsBeforeRow();
                    RowsAfterRow = GetRowsAfterRow(row);
                }
            }

            //private ObservableCollection<CoordinatesFunctionRowItem> GetRowsBeforeRow()
            //{
            //    int rowIndex = Table.Rows.IndexOf(Row);
            //    ObservableCollection<CoordinatesFunctionRowItem> rowsBefore = new ObservableCollection<CoordinatesFunctionRowItem>();
            //    for(int i = 0;i<rowIndex;i++)
            //    {
            //        rowsBefore.Add(Table.Rows[i].Clone());
            //    }
            //    return rowsBefore;
            //}

            private ObservableCollection<CoordinatesFunctionRowItem> GetRowsAfterRow(CoordinatesFunctionRowItem row)
            {
                int rowIndex = Table.Rows.IndexOf(row);
                ObservableCollection<CoordinatesFunctionRowItem> rowsAfter = new ObservableCollection<CoordinatesFunctionRowItem>();
                for (int i = rowIndex+1; i < Table.Rows.Count; i++)
                {
                    CoordinatesFunctionRowItem rowItem = Table.Rows[i];
                    rowsAfter.Add(rowItem.Clone());
                }
                //i am removing the rows after the row because they will need to be put into a new table
                //only the previous rows get to stay in this table.
                for(int i = Table.Rows.Count-1;i>rowIndex;i--)
                {
                    Table.Rows.Remove(Table.Rows[i]);
                }
                return rowsAfter;
            }

            private RowLeavingEventArgs.RowExtractionType GetRowExtractionType(ObservableCollection<CoordinatesFunctionRowItem> rows, CoordinatesFunctionRowItem row)
            {
                int rowIndex = rows.IndexOf(row);
                if (rowIndex == 0 && rows.Count >1)
                {
                    return RowExtractionType.MovingBack;
                }
                //if there is only one row in the table, then it could go either forward or backward
                else if(rowIndex == 0 && rows.Count == 1)
                {
                    return RowExtractionType.OnlyRow;
                }
                else if (rowIndex == rows.Count - 1)
                {
                    return RowExtractionType.MovingForeward;
                }
                else
                {
                    return RowExtractionType.Splitting;
                }

            }

            public enum RowExtractionType
            {
                //the row was at index 0
                MovingBack, 
                //the row was the last row
                MovingForeward, 
                //there is 3 or more rows and this row is somewhere in the middle
                Splitting, 
                //this is the only row left in the table
                //this row can either go to a previous table, go to the next table, or glue the previous and next table together.
                OnlyRow
            }
        }


    }
}
