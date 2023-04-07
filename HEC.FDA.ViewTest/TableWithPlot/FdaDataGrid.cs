using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using HEC.FDA.ViewModel.TableWithPlot.Rows;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.FDA.ViewTest.TableWithPlot.CustomEventArgs;

namespace HEC.FDA.ViewTest.TableWithPlot
{
    public class FdaDataGrid : DataGrid
    {
        public event EventHandler ArrowDownInLastRow;
        public event EventHandler ArrowUpInFirstRow;
        public event EventHandler PreviewPasteData;
        public event EventHandler PostPasteData;
        public event EventHandler PreviewAddRows;
        public event EventHandler PreviewLastRowEnter;
        public event EventHandler<PreviewLastRowTabEventArgs> PreviewLastRowTab;
        public event EventHandler<RowsAddedEventArgs> RowsAdded;
        public event EventHandler<RowsDeletedEventArgs> RowsDeleted;
        public event EventHandler<PreviewDeleteRowsEventArgs> PreviewDeleteRows;

        public bool AllowAddDeleteRows { get; set; } = true;
        public bool PasteAddsRows { get; set; } = true;
        public FdaDataGrid()
        {
            //TODO:  This might be bad, I never remove these handlers, but I don't know how to do it better. 
            KeyDown += OnKeyDown;
            PreviewKeyDown += OnPreviewKeyDown;
            MouseRightButtonUp += OnMouseRightButtonUp;
            AutoGeneratingColumn += myDataGrid_AutoGeneratingColumn;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (IsLastRowSelected())
            {
                if (e.Key == Key.Enter)
                {
                    e.Handled = true;
                    PreviewLastRowEnter?.Invoke(this, EventArgs.Empty);
                }
                else if (e.Key == Key.Tab & CurrentColumn.DisplayIndex == Columns.Count - 3)
                {
                    e.Handled = true;
                    PreviewLastRowTab?.Invoke(this, new PreviewLastRowTabEventArgs(CurrentColumn.DisplayIndex));
                }
                else if (Keyboard.IsKeyDown(Key.Down))
                {
                    ArrowDownInLastRow?.Invoke(this, EventArgs.Empty);
                    e.Handled = true;
                }
            }
            if (IsFirstRowSelected())
            {
                if (Keyboard.IsKeyDown(Key.Up))
                {
                    ArrowUpInFirstRow?.Invoke(this, EventArgs.Empty);
                    e.Handled = true;
                }
            }
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) | Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.V)
                {
                    PreviewPasteData?.Invoke(this, EventArgs.Empty);
                    PasteClipboard();
                    PostPasteData?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while (dep != null && !(dep is DataGridCell) && !(dep is DataGridColumnHeader))
                dep = VisualTreeHelper.GetParent(dep);

            if (dep == null)
                return;

            if (dep is DataGridCell)
            {
                DataGridCell cell = dep as DataGridCell;
                if (SelectedCells.Count <= 1)
                {
                    UnselectAllCells();
                    cell.IsSelected = true;
                }
                if (cell.IsSelected == false)
                {
                    UnselectAllCells();
                    cell.IsSelected = true;
                }
                cell.ContextMenu = new ContextMenu();
                MenuItem CopyMenuItem = new MenuItem
                {
                    Header = "Copy"
                };
                CopyMenuItem.Click += CopyClipBoard;
                if (SelectedCells.Count <= 0)
                    CopyMenuItem.IsEnabled = false;
                cell.ContextMenu.Items.Add(CopyMenuItem);

                if (IsReadOnly == false)
                {
                    MenuItem PasteMenuItem = new MenuItem
                    {
                        Header = "Paste"
                    };
                    PasteMenuItem.Click += MenuItemPasteClipboard;
                    try
                    {
                        string[][] clipboardData = Clipboard.GetText().Split('\n').Select(row => row.Split('\t').Select(Clipboardcell => Clipboardcell.Length > 0 && Clipboardcell[Clipboardcell.Length - 1] == '\r' ? Clipboardcell.Substring(0, Clipboardcell.Length - 1) : Clipboardcell).ToArray()).Where(a => a.Any(b => b.Length > 0)).ToArray();

                        if (clipboardData.Length == 0)
                            PasteMenuItem.IsEnabled = false;
                    }
                    catch (Exception)
                    {
                        PasteMenuItem.IsEnabled = false;
                    }
                    cell.ContextMenu.Items.Add(PasteMenuItem);

                    if (AllowAddDeleteRows == true)
                    {
                        MenuItem InsertRows = new MenuItem
                        {
                            Header = "Insert Row(s) Above"
                        };
                        InsertRows.Click += InsertRowItems;
                        cell.ContextMenu.Items.Add(InsertRows);

                        MenuItem InsertRowsBelow = new MenuItem
                        {
                            Header = "Insert Row(s) Below"
                        };
                        InsertRowsBelow.Click += InsertRowItemsBelow;
                        cell.ContextMenu.Items.Add(InsertRowsBelow);

                        MenuItem DeleteRows = new MenuItem
                        {
                            Header = "Delete Row(s)"
                        };
                        DeleteRows.Click += DeleteSelectedRows;
                        cell.ContextMenu.Items.Add(DeleteRows);
                    }
                }
                cell.ContextMenu.IsOpen = true;
            }
        }
        private bool IsLastRowSelected()
        {
            int RowIndex;
            foreach (DataGridCellInfo cellInfo in SelectedCells)
            {
                RowIndex = Items.IndexOf(cellInfo.Item);
                if (RowIndex == Items.Count - 1)
                    return true;
            }
            return false;
        }
        private bool IsFirstRowSelected()
        {
            int RowIndex;
            foreach (DataGridCellInfo cellInfo in SelectedCells)
            {
                RowIndex = Items.IndexOf(cellInfo.Item);
                if (RowIndex == 0) { return true; }
            }
            return false;
        }
        private void DeleteSelectedRows(object sender, RoutedEventArgs e)
        {
            List<int> UniqueRows = new List<int>();
            int RowIndex;
            foreach (DataGridCellInfo cellInfo in SelectedCells)
            {
                RowIndex = Items.IndexOf(cellInfo.Item);
                if (UniqueRows.Contains(RowIndex) == false)
                    UniqueRows.Add(RowIndex);
            }
            PreviewDeleteRowsEventArgs pdrea = new PreviewDeleteRowsEventArgs(UniqueRows, Items.Count, false);
            PreviewDeleteRows?.Invoke(this, pdrea);
            if (!pdrea.Cancel)
            {
                UniqueRows.Sort();
                System.ComponentModel.IEditableCollectionView items = Items;
                RowsDeleted?.Invoke(this, new RowsDeletedEventArgs(UniqueRows));
            }
        }
        private void AddSelectedRows(object sender, RoutedEventArgs e)
        {
            PreviewAddRows?.Invoke(this, EventArgs.Empty);
            List<int> UniqueRows = GetSelectedRows();
            int InsertAtRow = Items.Count;
            RowsAdded?.Invoke(this, new RowsAddedEventArgs(InsertAtRow, UniqueRows.Count));
        }

        private void InsertRowItemsBelow(object sender, RoutedEventArgs e)
        {
            PreviewAddRows?.Invoke(this, EventArgs.Empty);
            List<int> UniqueRows = GetSelectedRows();
            int InsertAtRow = UniqueRows.Min();
            RowsAdded?.Invoke(this, new RowsAddedEventArgs(InsertAtRow + 1, UniqueRows.Count));
        }

        public int GetSelectedIndex()
        {
            List<int> selectedRows = GetSelectedRows();
            if (selectedRows.Count > 0)
                return selectedRows[0];
            else
                return -1;
        }

        private void InsertRowItems(object sender, RoutedEventArgs e)
        {
            PreviewAddRows?.Invoke(this, EventArgs.Empty);
            List<int> UniqueRows = GetSelectedRows();
            int InsertAtRow = UniqueRows.Min();
            RowsAdded?.Invoke(this, new RowsAddedEventArgs(InsertAtRow, UniqueRows.Count));
        }

        private List<int> GetSelectedRows()
        {
            List<int> UniqueRows = new List<int>();
            // Convert the selected cells into a list of rows by index
            int RowIndex;
            foreach (DataGridCellInfo cellInfo in SelectedCells)
            {
                RowIndex = Items.IndexOf(cellInfo.Item);
                if (UniqueRows.Contains(RowIndex) == false)
                    UniqueRows.Add(RowIndex);
            }

            return UniqueRows;
        }
        private void CopyClipBoard(object sender, RoutedEventArgs e)
        {
            ApplicationCommands.Copy.Execute(null, this);
        }
        private void MenuItemPasteClipboard(object sender, RoutedEventArgs e)
        {
            PreviewPasteData?.Invoke(this, EventArgs.Empty);
            PasteClipboard();
        }
        public void PasteClipboard()
        {
            try
            {
                string[][] clipboardData = Clipboard.GetText().Split('\n').Select(row => row.Split('\t').Select(cell => cell.Length > 0 && cell[cell.Length - 1] == '\r' ? cell.Substring(0, cell.Length - 1) : cell).ToArray()).Where(a => a.Any(b => b.Length > 0)).ToArray();
                int RowIndex, ColumnIndex;
                if (SelectedCells.Count == 1)
                {
                    // fill beyond selected cell
                    DataGridCellInfo cellinfo = SelectedCells[0];
                    RowIndex = Items.IndexOf(cellinfo.Item);
                    ColumnIndex = cellinfo.Column.DisplayIndex;
                    for (int i = 0; i <= clipboardData.Count() - 1; i++)
                    {
                        if (RowIndex + i > Items.Count - 1)
                        {
                            if (PasteAddsRows == false)
                            {
                                Items.Refresh();
                                return;
                            }
                            else
                                RowsAdded?.Invoke(this, new RowsAddedEventArgs(Items.Count, 1));
                        }
                        for (int j = 0; j <= clipboardData[i].Count() - 1; j++)
                        {
                            if (ColumnIndex + j > Columns.Count - 1)
                            {
                                continue;
                            }
                            if (Columns[ColumnIndex + j].IsReadOnly)
                            {
                                continue;
                            }
                            Binding binding = (Columns[ColumnIndex + j] as DataGridBoundColumn).Binding as Binding;
                            Type RowType = Items[RowIndex + i].GetType();
                            PropertyInfo y = RowType.GetProperty(binding.Path.Path);

                            try
                            {
                                var newVal = Convert.ChangeType(clipboardData[i][j], y.PropertyType);
                                ((SequentialRow)Items[RowIndex + i]).UpdateRow(ColumnIndex + j, (double)newVal);
                            }
                            catch (Exception ex)
                            {
                                //ignore
                            }
                        }
                    }
                }
                else if (SelectedCells.Count > 1)
                {
                    // Test for continuous selection
                    List<int> Rows = new List<int>();
                    List<int> Columns = new List<int>();
                    foreach (DataGridCellInfo cellInfo in SelectedCells)
                    {
                        RowIndex = Items.IndexOf(cellInfo.Item);
                        ColumnIndex = cellInfo.Column.DisplayIndex;
                        Rows.Add(RowIndex);
                        Columns.Add(ColumnIndex);
                    }

                    Rows.Sort();
                    int RowMax = Rows[Rows.Count - 1];
                    RowIndex = Rows[0];
                    Columns.Sort();
                    ColumnIndex = Columns[0];
                    int ColumnMax = Columns[Columns.Count - 1];
                    DataGridCell CellCheck;
                    for (int i = RowIndex; i <= RowMax; i++)
                    {
                        for (int j = ColumnIndex; j <= ColumnMax; j++)
                        {
                            CellCheck = GetCell(i, j);
                            if (CellCheck.IsSelected == false)
                            {
                                MessageBox.Show("Invalid selection, selected cells must be continuous.", "Invalid Selection", MessageBoxButton.OK);
                                return;
                            }
                        }
                    }

                    for (int i = 0; i <= clipboardData.Count() - 1; i++)
                    {
                        if (RowIndex + i > RowMax)
                        {
                            Items.Refresh();
                            return;
                        }
                        for (int j = 0; j <= clipboardData[i].Count() - 1; j++)
                        {
                            if (ColumnIndex + j > ColumnMax)
                                continue;
                            if (this.Columns[ColumnIndex + j].IsReadOnly)
                                continue;
                            Binding binding = (this.Columns[ColumnIndex + j] as DataGridBoundColumn).Binding as Binding;
                            Type RowType = Items[RowIndex + i].GetType();
                            PropertyInfo y = RowType.GetProperty(binding.Path.Path);
                            try
                            {
                                var newVal = Convert.ChangeType(clipboardData[i][j], y.PropertyType);
                                ((SequentialRow)Items[RowIndex + i]).UpdateRow(ColumnIndex + j, (double)newVal);
                            }
                            catch (Exception ex)
                            {
                                //ignore
                            }
                        }
                    }
                }

                Items.Refresh();
            }
            catch (Exception)
            {
                MessageBox.Show("Error pasting data from clipboard.", "Error in paste from clipboard", MessageBoxButton.OK);
            }
        }
        public DataGridRow GetRow(int index)
        {
            DataGridRow row = (DataGridRow)ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // May be virtualized, bring into view and try again.
                UpdateLayout();
                ScrollIntoView(Items[index]);
                row = (DataGridRow)ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }
        public DataGridCell GetCell(int rowindex, int column)
        {
            DataGridRow row = GetRow(rowindex);
            if (row != null)
            {
                DataGridCellsPresenter presenter = GetTheVisualChild<DataGridCellsPresenter>(row);


                if (presenter == null)
                {
                    ScrollIntoView(row, Columns[column]);
                    presenter = GetTheVisualChild<DataGridCellsPresenter>(row);
                }
                if (presenter == null)
                    return null;
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        }
        public T GetTheVisualChild<T>(Visual parent) where T : Visual
        {
            T child = null;
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i <= numVisuals - 1; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                    child = GetTheVisualChild<T>(v);
                if (child != null)
                    break;
            }
            return child;
        }
        protected override void OnCanExecuteBeginEdit(CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
        protected override void OnCanExecuteCommitEdit(CanExecuteRoutedEventArgs e)
        {
            base.OnCanExecuteCommitEdit(e);
        }
        private void myDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Cancel = true;
            if (e.Column.GetType() == typeof(DataGridTextColumn))
            {
                DataGridTextColumn dgtc = (DataGridTextColumn)e.Column;
                FdaDataGrid sndr = sender as FdaDataGrid;
                bool cancel = true;
                if (sndr != null)
                {
                    PropertyInfo[] pilist = sndr.Items.CurrentItem.GetType().GetProperties();
                    foreach (PropertyInfo pi in pilist)
                    {
                        if (pi.Name == e.PropertyName)
                        {
                            DisplayAsColumnAttribute dna = (DisplayAsColumnAttribute)pi.GetCustomAttribute(typeof(DisplayAsColumnAttribute));
                            if (dna != null)
                            {
                                Console.WriteLine(dna.DisplayName);
                                dgtc.Header = dna.DisplayName;
                                cancel = false;
                            }
                        }

                    }
                }
                e.Cancel = cancel;
                if (cancel) { return; }

                dgtc.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                Style ers = (Style)Resources["errorStyle"];
                Style ersG = (Style)Resources["errorStyleGrid"];
                dgtc.EditingElementStyle = ers;
                dgtc.ElementStyle = ersG;
                Binding b = (Binding)dgtc.Binding;
                b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
                b.ValidatesOnNotifyDataErrors = true;
                //TODO: This approach fixes all of the data grid columns to the same format. I think we could use a way to access the format of certain columns. 
                b.StringFormat = ViewModel.Utilities.StringConstants.DETAILED_DECIMAL_FORMAT;
            }
        }
    }
}
