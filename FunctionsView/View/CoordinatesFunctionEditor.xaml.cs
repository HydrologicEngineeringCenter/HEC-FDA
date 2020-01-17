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
        public static readonly DependencyProperty EditorVMProperty = DependencyProperty.Register("EditorVM", typeof(CoordinatesFunctionEditorVM), typeof(CoordinatesFunctionEditor), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(EditorVMChangedCallBack)));
        public bool HasLoaded { get; set; }

        public CoordinatesFunctionEditor()
        {
            InitializeComponent();
        }

        public CoordinatesFunctionEditorVM EditorVM
        {
            get { return (CoordinatesFunctionEditorVM)this.GetValue(EditorVMProperty); }
            set { this.SetValue(EditorVMProperty, value); }
        }
       
        private static void EditorVMChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CoordinatesFunctionEditor owner = d as CoordinatesFunctionEditor;
            CoordinatesFunctionEditorVM editorVM = e.NewValue as CoordinatesFunctionEditorVM;
            editorVM.UpdateView += owner.UpdateView;
            owner.EditorVM = editorVM;
            owner.UpdateView(owner, new EventArgs());
        }

        private List<CoordinatesFunctionRowItem> GetAllRowsFromAllTables()
        {
            List<CoordinatesFunctionRowItem> allRows = new List<CoordinatesFunctionRowItem>();
            foreach (CoordinatesFunctionTableVM tableVM in EditorVM.Tables)
            {
                allRows.AddRange(tableVM.Rows);
            }
            return allRows;
        }

        /// <summary>
        /// This is called anytime an visual update needs to happen, such as changing distribution types.
        /// </summary>
        /// <param name="rowItems"></param>
        private void CreateTables()
        {
            if(EditorVM.Tables.Count == 0)
            {
                return;
            }

            List<CoordinatesFunctionRowItem> rowItems = GetAllRowsFromAllTables();
            int[] colWidths = ColumnWidths.GetComputedColumnWidths(rowItems);

            lst_tables.Items.Clear();

            lst_tables.Items.Add(new TableTopControl(colWidths));
            for(int i = 0;i<EditorVM.Tables.Count;i++)
            {
                CoordinatesFunctionTable newTable = new CoordinatesFunctionTable(EditorVM.Tables[i], colWidths);
                if (i == 0)
                {
                    newTable.DisplayXAndInterpolatorHeaders();
                }
                //newTable.UpdateView += UpdateView;
                newTable.FirstRowUpArrowPressed += NewTable_FirstRowUpArrowPressed;
                newTable.LastRowDownArrowPressed += NewTable_LastRowDownArrowPressed;
                newTable.LastRowEnterPressed += NewTable_LastRowEnterPressed;
                newTable.LastRowAndCellTabPressed += NewTable_LastRowAndCellTabPressed;
                lst_tables.Items.Add(newTable);

            }
            lst_tables.Items.Add(new TableBottomControl(colWidths));

            //it would be nice to just bind the items source of the list to be the list of tables
            //but i have to put the topper and bottom UI pieces in. I could do them outside the list
            //but that seemed harder to get it to line up etc.
                      
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
            CreateTables();
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
            if(EditorVM.Tables.Last().Equals(senderTable.TableVM))
            //if (lst_tables.Items[lst_tables.Items.Count - 2].Equals(senderTable))
            {
                senderTable.TableVM.AddRow();
                //now select the correct column in that table
                int lastRowIndex = senderTable.dg_table.Items.Count - 1;
                senderTable.SelectCellByIndex(lastRowIndex, 0);
            }
            else
            {
                //last editable cell on a table has been pressed, send focus to first cell of next table
                int tableIndex = lst_tables.Items.IndexOf(senderTable);
                if (tableIndex != -1)
                {
                    CoordinatesFunctionTable nextTable = lst_tables.Items[tableIndex + 1] as CoordinatesFunctionTable;
                    nextTable.SelectCellByIndex(0, 0);
                }

            }

        }
        private void NewTable_FirstRowUpArrowPressed(object sender, EventArgs e)
        {
            CoordinatesFunctionTable senderTable = (CoordinatesFunctionTable)sender;
            int tableIndex = lst_tables.Items.IndexOf(senderTable);
            if (tableIndex == -1) return;
            int column = senderTable.dg_table.CurrentColumn.DisplayIndex;
            //only go up to another table if we aren't already in the top table
            //this is "1" and not "0" because the first thing in the list is the table topper.
            if (tableIndex > 1)
            {
                CoordinatesFunctionTable previousTable = lst_tables.Items[tableIndex - 1] as CoordinatesFunctionTable;
                int columnToSelect = DetermineWhichColumnToSelect(senderTable.dg_table.Columns.Count, previousTable.dg_table.Columns.Count, column);
                previousTable.SelectCellByIndex(previousTable.TableVM.Rows.Count-1, columnToSelect);
            }
        }

        private int DetermineWhichColumnToSelect(int senderTableNumCols, int destinationTableNumCols, int currentCol)
        {
            int columnToSelect = currentCol;
            //if we are in the last column, then go to the last column in the next table
            if (currentCol == senderTableNumCols-1)
            {
                columnToSelect = destinationTableNumCols-1;
            }
            //if we are in the second to last column, then go to the second to last column in the next table.
            else if (currentCol == senderTableNumCols - 2)
            {
                columnToSelect = destinationTableNumCols - 2;
            }
            //now we know that the sender is not in the last two rows
            //if we are not in a combobox but going straight down would put the next table
            //in a combobox, then go down to the highest column that is not a combobox
            else if(currentCol>=destinationTableNumCols-2)
            {
                columnToSelect = destinationTableNumCols - 3;
            }
            return columnToSelect;
        }

        private void NewTable_LastRowDownArrowPressed(object sender, EventArgs e)
        {
            CoordinatesFunctionTable senderTable = (CoordinatesFunctionTable)sender;
            int tableIndex = lst_tables.Items.IndexOf(senderTable);
            if (tableIndex == -1) return;
            int column = senderTable.dg_table.CurrentColumn.DisplayIndex;
            //only go down to another table if we aren't already in the last table
            if (tableIndex != lst_tables.Items.Count - 2)
            {
                //there is a table below us. Go on down.
                senderTable.dg_table.SelectedCells.Clear();
                CoordinatesFunctionTable nextTable = lst_tables.Items[tableIndex + 1] as CoordinatesFunctionTable;
                int columnToSelect = DetermineWhichColumnToSelect(senderTable.dg_table.Columns.Count, nextTable.dg_table.Columns.Count, column);
                nextTable.SelectCellByIndex(0, columnToSelect);
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
            int tableIndex = lst_tables.Items.IndexOf(senderTable);
            if (tableIndex == -1) return;
            int column = senderTable.dg_table.CurrentColumn.DisplayIndex;

            //this is "-2" because this list is not just tables. There is the topper and the bottom line.
            if (tableIndex == lst_tables.Items.Count-2)
            {
                //we are in the last row so we need to add a row and then select a cell in that row.
                senderTable.TableVM.AddRow();
                //now select the correct column in that table
                int lastRowIndex = senderTable.dg_table.Items.Count - 1;
                senderTable.SelectCellByIndex(lastRowIndex, column);
            }
            else
            {
                senderTable.dg_table.SelectedCells.Clear();
                CoordinatesFunctionTable nextTable = lst_tables.Items[tableIndex + 1] as CoordinatesFunctionTable;

                int columnToSelect = DetermineWhichColumnToSelect(senderTable.dg_table.Columns.Count, nextTable.dg_table.Columns.Count, column);
                nextTable.SelectCellByIndex(0, columnToSelect);

                //int columnToSelect = column;
                //int lastEditableColumn = nextTable.dg_table.Columns.Count - 3;
                ////get the currently selected column. If the selected column in the current table is greater
                ////than the columns in the next table then just select the largest editable column
                //if (column > lastEditableColumn)
                //{
                //    columnToSelect = lastEditableColumn;
                //}
                //nextTable.SelectCellByIndex(0, columnToSelect);
            }
        }
    }
}
