using FunctionsView.ViewModel;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for CoordinatesFunctionTable.xaml
    /// </summary>
    public partial class CoordinatesFunctionTable : UserControl
    {
        public List<CoordinatesFunctionRowItem> Rows
        {
            get;
            set;
        }
        public CoordinatesFunctionTable()
        {
            InitializeComponent();
        }

        public CoordinatesFunctionTable(List<CoordinatesFunctionRowItem> rows)
        {
            InitializeComponent();
            this.DataContext = this;
            Rows = rows;
            if (rows.Count > 0)
            {
                string distType = rows[0].SelectedDistributionType;
                switch (distType)
                {
                    case "None":
                        {
                            AddNoneColumn();
                            break;
                        }
                    case "Normal":
                        {
                            AddNormalColumns();
                            break;
                        }
                }
            }
        }

        private void AddNormalColumns()
        {
            List<DataGridColumn> columns = new List<DataGridColumn>();
            DataGridTextColumn colMean = new DataGridTextColumn();
            colMean.Header = "Mean";
            colMean.Width = 130;
            colMean.CanUserReorder = false;
            colMean.CanUserResize = false;
            colMean.CanUserSort = false;
            colMean.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colMean.Binding = new Binding("Mean");
            columns.Add(colMean);

            DataGridTextColumn colStDev = new DataGridTextColumn();
            colStDev.Header = "St Dev";
            colStDev.Width = 130;
            colStDev.CanUserReorder = false;
            colStDev.CanUserResize = false;
            colStDev.CanUserSort = false;
            colStDev.HeaderStyle = Resources["ColumnHeaderStyle1"] as Style;
            colStDev.Binding = new Binding("Distribution.StandardDeviation");
            columns.Add(colStDev);

            AddColumns(columns);
        }


        private void AddNoneColumn()
        {
            DataGridTextColumn col = new DataGridTextColumn();
            col.Header = "TestValue (Constant)";
            col.Width = 260;
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
