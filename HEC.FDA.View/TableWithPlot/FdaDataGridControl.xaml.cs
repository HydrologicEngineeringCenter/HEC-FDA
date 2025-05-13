using HEC.FDA.View.TableWithPlot.CustomEventArgs;
using HEC.FDA.ViewModel.TableWithPlot.Data.Interfaces;
using HEC.FDA.ViewModel.TableWithPlot.Rows;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.TableWithPlot
{
    /// <summary>
    /// Interaction logic for FdaDataGridControl.xaml
    /// </summary>
    public partial class FdaDataGridControl : UserControl
    {
        #region UseStarSizingProperty
        // Register a dependency property so that users outside can set this property.
        public static readonly DependencyProperty UseStarSizingProperty =
            DependencyProperty.Register(
                "UseStarSizing",
                typeof(bool),
                typeof(FdaDataGridControl),
                new PropertyMetadata(false, OnUseStarSizingChanged));

        public bool UseStarSizing
        {
            get { return (bool)GetValue(UseStarSizingProperty); }
            set { SetValue(UseStarSizingProperty, value); }
        }

        private static void OnUseStarSizingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FdaDataGridControl;
            if (control != null)
            {
                control.UpdateInternalGridStarSizing((bool)e.NewValue);
            }
        }

        private void UpdateInternalGridStarSizing(bool useStar)
        {
            // Assuming your FdaDataGrid in the XAML has the name "MyDataGrid"
            if (MyDataGrid != null)
            {
                MyDataGrid.UseStarSizing = useStar;
            }
        }
        #endregion

        #region CustomNumberFormatProperty
        public static readonly DependencyProperty CustomNumberFormatProperty =
            DependencyProperty.Register(
                "CustomNumberFormat",
                typeof(string),
                typeof(FdaDataGridControl),
                new PropertyMetadata(null, OnCustomNumberFormatChanged));

        public string CustomNumberFormat
        {
            get { return (string)GetValue(CustomNumberFormatProperty); }
            set { SetValue(CustomNumberFormatProperty, value); }
        }
        private static void OnCustomNumberFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FdaDataGridControl;
            if (control != null)
            {
                control.UpdateInternalCustomNumberFormatChanged((string)e.NewValue);
            }
        }
        private void UpdateInternalCustomNumberFormatChanged(string newValue)
        {
            // Assuming your FdaDataGrid in the XAML has the name "MyDataGrid"
            if (MyDataGrid != null)
            {
                MyDataGrid.CustomNumberFormat = newValue;
            }
        }
        #endregion

        #region ColumnNamesMappingProperty
        public static readonly DependencyProperty ColumnNameMappingsProperty =
            DependencyProperty.Register(
            "ColumnNameMappings",
            typeof(Dictionary<string, string>),
            typeof(FdaDataGridControl),
            new PropertyMetadata(null, OnColumnNameMappingsChanged));

        public Dictionary<string, string> ColumnNameMappings
        {
            get { return (Dictionary<string, string>)GetValue(ColumnNameMappingsProperty); }
            set { SetValue(ColumnNameMappingsProperty, value); }
        }

        private static void OnColumnNameMappingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FdaDataGridControl;
            if (control != null && control.MyDataGrid != null)
            {
                control.MyDataGrid.ColumnNameMappings = e.NewValue as Dictionary<string, string>;

                // Refresh the grid if it's already loaded
                if (control.MyDataGrid.Items.Count > 0)
                {
                    control.MyDataGrid.Items.Refresh();
                }
            }
        }
        #endregion


        public FdaDataGridControl()
        {
            InitializeComponent();
            ColumnNameMappings = [];
        }

        public void MyDataGrid_PreviewDeleteRows(object sender, PreviewDeleteRowsEventArgs e)
        {
            if ((e.TotalRows - e.RowIndices.Count) < 2)
            {
                e.Cancel = true;
                MessageBox.Show("Cannot delete the requested rows. Two coordinates are required in the table.", "Two Coordinates Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void MyDataGrid_RowsDeleted(object sender, RowsDeletedEventArgs e)
        {
            IDataProvider vm = (IDataProvider)DataContext;
            vm.RemoveRows(e.RowIndices);
        }

        public void MyDataGrid_RowsAdded(object sender, RowsAddedEventArgs e)
        {
            IDataProvider vm = (IDataProvider)DataContext;
            for (int i = e.StartRow; i < e.StartRow + e.NumRows; i++)
            {
                vm.AddRow(i);
            }

        }

        private void MyDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            IDataProvider vm = (IDataProvider)DataContext;
            foreach (SequentialRow row in vm.Data)
            {
                row.Validate();
            }
        }
    }
}
