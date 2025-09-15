using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using HEC.FDA.View.TableWithPlot.CustomEventArgs;
using HEC.FDA.ViewModel.TableWithPlot.Rows;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;

namespace HEC.FDA.View.TableWithPlot
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
        public bool UseStarSizing { get;set;}
        public string CustomNumberFormat { get; set; }
        /// <summary>
        /// key = propertyName value = display name
        /// </summary>
        public Dictionary<string, string> ColumnNameMappings { get; set; } = [];

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
            // Handle Ctrl+C in preview to prevent default WPF copy behavior
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && e.Key == Key.C)
            {
                CopyClipBoard(this, e);
                e.Handled = true; // Prevent the default copy behavior
                return;
            }
            
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

            while ((dep != null) && !(dep is DataGridCell) && !(dep is DataGridColumnHeader))
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
            Int32 RowIndex;
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
            Int32 RowIndex;
            foreach (DataGridCellInfo cellInfo in SelectedCells)
            {
                RowIndex = Items.IndexOf(cellInfo.Item);
                if (RowIndex == 0) { return true; }
            }
            return false;
        }
        private void DeleteSelectedRows(object sender, RoutedEventArgs e)
        {
            List<Int32> UniqueRows = new List<Int32>();
            Int32 RowIndex;
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
            Int32 InsertAtRow = Items.Count;
            RowsAdded?.Invoke(this, new RowsAddedEventArgs(InsertAtRow, UniqueRows.Count));
        }

        private void InsertRowItemsBelow(object sender, RoutedEventArgs e)
        {
            PreviewAddRows?.Invoke(this, EventArgs.Empty);
            List<int> UniqueRows = GetSelectedRows();
            Int32 InsertAtRow = UniqueRows.Min();
            RowsAdded?.Invoke(this, new RowsAddedEventArgs(InsertAtRow + 1, UniqueRows.Count));
        }

        public Int32 GetSelectedIndex()
        {
            List<Int32> selectedRows = GetSelectedRows();
            if (selectedRows.Count > 0)
                return selectedRows[0];
            else
                return -1;
        }

        private void InsertRowItems(object sender, RoutedEventArgs e)
        {
            PreviewAddRows?.Invoke(this, EventArgs.Empty);
            List<int> UniqueRows = GetSelectedRows();
            Int32 InsertAtRow = UniqueRows.Min();
            RowsAdded?.Invoke(this, new RowsAddedEventArgs(InsertAtRow, UniqueRows.Count));
        }

        private List<int> GetSelectedRows()
        {
            List<Int32> UniqueRows = new List<Int32>();
            // Convert the selected cells into a list of rows by index
            Int32 RowIndex;
            foreach (DataGridCellInfo cellInfo in this.SelectedCells)
            {
                RowIndex = Items.IndexOf(cellInfo.Item);
                if (UniqueRows.Contains(RowIndex) == false)
                    UniqueRows.Add(RowIndex);
            }

            return UniqueRows;
        }

        /// <summary>
        /// The ApplicationCommands.Copy.Execute() stopped working properly at some point. This is a hack to get rid of all the different formats it copies and explicitly set only the one we want for pasting into excel.
        /// </summary>
        private void CopyClipBoard(object sender, RoutedEventArgs e)
        {
            ApplicationCommands.Copy.Execute(null, this);
            IDataObject clipboardData = Clipboard.GetDataObject();
            object data = clipboardData.GetData("Text");
            Clipboard.Clear();
            Clipboard.SetText(data.ToString());
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
                Int32 RowIndex, ColumnIndex;
                if (SelectedCells.Count == 1)
                {
                    // fill beyond selected cell
                    DataGridCellInfo cellinfo = SelectedCells[0];
                    RowIndex = Items.IndexOf(cellinfo.Item);
                    ColumnIndex = cellinfo.Column.DisplayIndex;
                    for (Int32 i = 0; i <= clipboardData.Count() - 1; i++)
                    {
                        if ((RowIndex + i) > Items.Count - 1)
                        {
                            if (PasteAddsRows == false)
                            {
                                Items.Refresh();
                                return;
                            }
                            else
                                RowsAdded?.Invoke(this, new RowsAddedEventArgs(Items.Count, 1));
                        }
                        for (Int32 j = 0; j <= clipboardData[i].Count() - 1; j++)
                        {
                            if ((ColumnIndex + j) > Columns.Count - 1)
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
                            catch
                            {
                                //ignore
                            }
                        }
                    }
                }
                else if (this.SelectedCells.Count > 1)
                {
                    // Test for continuous selection
                    List<Int32> Rows = new List<Int32>();
                    List<Int32> Columns = new List<Int32>();
                    foreach (DataGridCellInfo cellInfo in SelectedCells)
                    {
                        RowIndex = Items.IndexOf(cellInfo.Item);
                        ColumnIndex = cellInfo.Column.DisplayIndex;
                        Rows.Add(RowIndex);
                        Columns.Add(ColumnIndex);
                    }

                    Rows.Sort();
                    Int32 RowMax = Rows[Rows.Count - 1];
                    RowIndex = Rows[0];
                    Columns.Sort();
                    ColumnIndex = Columns[0];
                    Int32 ColumnMax = Columns[Columns.Count - 1];
                    DataGridCell CellCheck;
                    for (Int32 i = RowIndex; i <= RowMax; i++)
                    {
                        for (Int32 j = ColumnIndex; j <= ColumnMax; j++)
                        {
                            CellCheck = GetCell(i, j);
                            if (CellCheck.IsSelected == false)
                            {
                                MessageBox.Show("Invalid selection, selected cells must be continuous.", "Invalid Selection", MessageBoxButton.OK);
                                return;
                            }
                        }
                    }

                    for (Int32 i = 0; i <= clipboardData.Count() - 1; i++)
                    {
                        if ((RowIndex + i) > RowMax)
                        {
                            Items.Refresh();
                            return;
                        }
                        for (Int32 j = 0; j <= clipboardData[i].Count() - 1; j++)
                        {
                            if ((ColumnIndex + j) > ColumnMax)
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
                            catch
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
            DataGridRow row = (DataGridRow)this.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // May be virtualized, bring into view and try again.
                this.UpdateLayout();
                this.ScrollIntoView(Items[index]);
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
                    this.ScrollIntoView(row, Columns[column]);
                    presenter = GetTheVisualChild<DataGridCellsPresenter>(row);
                }
                if (presenter == null)
                    return null;
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        }
        public T GetTheVisualChild<T>(System.Windows.Media.Visual parent) where T : System.Windows.Media.Visual
        {
            T child = null;
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i <= numVisuals - 1; i++)
            {
                System.Windows.Media.Visual v = (System.Windows.Media.Visual)VisualTreeHelper.GetChild(parent, i);
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
            if (e.Column is DataGridTextColumn dataGridtextColumn)
            {
                bool cancel = true;
                if (sender is FdaDataGrid dataGrid)
                {
                    if (ColumnNameMappings != null && ColumnNameMappings.TryGetValue(e.PropertyName, out string value))
                    {
                        dataGridtextColumn.Header = value;
                        cancel = false;
                    }
                    else
                    {
                        PropertyInfo[] pilist = dataGrid.Items[0].GetType().GetProperties();
                        foreach (PropertyInfo pi in pilist)
                        {
                            if (pi.Name == e.PropertyName)
                            {
                                DisplayAsColumnAttribute DisplayAsColAttribute = pi.GetCustomAttribute<DisplayAsColumnAttribute>();
                                if (DisplayAsColAttribute != null)
                                {
                                    dataGridtextColumn.Header = DisplayAsColAttribute.DisplayName;
                                    cancel = false;
                                }
                            }

                        }
                    }     
                }
                e.Cancel = cancel;
                if (cancel) { return; }
                // Only override the width if UseStarSizing is true.
                if (UseStarSizing)
                {
                    dataGridtextColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                }
                else
                {
                    dataGridtextColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
                }
                Style ers = (Style)Resources["errorStyle"];
                Style ersG = (Style)Resources["errorStyleGrid"];
                dataGridtextColumn.EditingElementStyle = ers;
                dataGridtextColumn.ElementStyle = ersG;
                Binding binding = (Binding)dataGridtextColumn.Binding;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
                binding.ValidatesOnNotifyDataErrors = true;
                if (e.PropertyType == typeof(double) || e.PropertyType == typeof(double?))
                {
                    // Use custom format if specified, otherwise use the default
                    if (!string.IsNullOrEmpty(CustomNumberFormat))
                    {
                        binding.StringFormat = CustomNumberFormat;
                    }
                    else
                    {
                        binding.StringFormat = ViewModel.Utilities.StringConstants.DETAILED_DECIMAL_FORMAT;
                    }
                }
            }
        }
    }
}
