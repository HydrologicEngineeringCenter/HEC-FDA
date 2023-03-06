using HEC.FDA.ViewModel.FrequencyRelationships;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace HEC.FDA.View.FrequencyRelationships
{
    /// <summary>
    /// Interaction logic for AnalyticalFrequencyEditor.xaml
    /// </summary>
    public partial class AnalyticalFrequencyEditor : UserControl
    {
        public AnalyticalFrequencyEditor()
        {
            InitializeComponent();

            dg_table.RowsAdded += Dg_table_RowsAdded;
            dg_table.RowsDeleted += Dg_table_RowsDeleted;
            dg_table.PreviewLastRowEnter += Dg_table_PreviewLastRowEnter;
        }

        private void Dg_table_PreviewLastRowEnter()
        {
            //This was added to allow the table to complete it's enter/edit complete action and then perform the add logic.
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (DataContext is AnalyticalFrequencyEditorVM vm)
                {
                    vm.AddRow();
                    int lastRowIndex = dg_table.Items.Count - 1;
                    SelectCellByIndex(lastRowIndex, 0);
                }
            }));
        }

        private void Dg_table_RowsDeleted(List<int> rowindices)
        {
            if (DataContext is AnalyticalFrequencyEditorVM vm)
            {
                vm.DeleteRows(rowindices);
            }
        }

        private void Dg_table_RowsAdded(int startrow, int numrows)
        {
            if (DataContext is AnalyticalFrequencyEditorVM vm)
            {
                vm.AddRows(startrow, numrows);
            }
        }

        private void rad_analytical_Checked(object sender, RoutedEventArgs e)
        {
            if (group_analytical != null && group_graphical != null)
            {
                group_analytical.Visibility = Visibility.Visible;
                group_graphical.Visibility = Visibility.Hidden;
            }
        }

        private void rad_graphical_Checked(object sender, RoutedEventArgs e)
        {
            if (group_analytical != null && group_graphical != null)
            {
                group_analytical.Visibility = Visibility.Hidden;
                group_graphical.Visibility = Visibility.Visible;
            }
        }

        private void rad_standard_Checked(object sender, RoutedEventArgs e)
        {
            if (grid_analyticalLeft_standard != null && grid_analyticalLeft_fitToFlows != null)
            {
                grid_analyticalLeft_standard.Visibility = Visibility.Visible;
                grid_analyticalLeft_fitToFlows.Visibility = Visibility.Hidden;

                if (DataContext is AnalyticalFrequencyEditorVM vm)
                {
                    vm.UpdateChartLineData();
                }
            }
        }

        private void rad_fitToFlows_Checked(object sender, RoutedEventArgs e)
        {
            if (grid_analyticalLeft_standard != null && grid_analyticalLeft_fitToFlows != null)
            {
                grid_analyticalLeft_standard.Visibility = Visibility.Hidden;
                grid_analyticalLeft_fitToFlows.Visibility = Visibility.Visible;

                if (DataContext is AnalyticalFrequencyEditorVM vm)
                {
                    vm.UpdateChartLineData();
                }
            }
        }

        private void SelectCellByIndex(int rowIndex, int columnIndex)
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
                DataGridCell cell = GetCell(row, columnIndex);
                if (cell != null)
                {
                    DataGridCellInfo dataGridCellInfo = new DataGridCellInfo(cell);
                    dg_table.SelectedCells.Add(dataGridCellInfo);
                    cell.Focus();
                }
            }
        }

        private DataGridCell GetCell(DataGridRow rowContainer, int column)
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

        private void FitToFlows_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AnalyticalFrequencyEditorVM vm)
            {
                vm.UpdateChartLineData();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is AnalyticalFrequencyEditorVM vm)
            {
                vm.HasChanges = false;
            }
        }
    }
}
