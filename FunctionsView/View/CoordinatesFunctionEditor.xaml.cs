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
                newTable.UpdateView += UpdateView;
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
                senderTable.TableVM.AddRow();
                //now select the correct column in that table
                int lastRowIndex = senderTable.dg_table.Items.Count - 1;
                senderTable.SelectCellByIndex(lastRowIndex, column);
            }
            else
            {
                senderTable.dg_table.SelectedCells.Clear();
                //its not the last table, put focus on the next table
                CoordinatesFunctionTable nextTable = lst_tables.Items[tableIndex+1] as CoordinatesFunctionTable;
                //get the currently selected column. If the selected column in the current table is greater
                //than the columns in the next table then just select the largest editable column
                int columnToSelect = column;
                int lastEditableColumn = nextTable.dg_table.Columns.Count - 3;

                if(column> lastEditableColumn)
                {
                    columnToSelect = lastEditableColumn;
                }
                nextTable.SelectCellByIndex(0, columnToSelect);
            }
        }
    }
}
