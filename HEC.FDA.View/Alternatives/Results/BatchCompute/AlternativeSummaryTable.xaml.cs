﻿using System;
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

namespace HEC.FDA.View.Alternatives.Results.BatchCompute
{
    /// <summary>
    /// Interaction logic for AlternativeSummaryTable.xaml
    /// </summary>
    public partial class AlternativeSummaryTable : UserControl
    {
        public AlternativeSummaryTable()
        {
            InitializeComponent();
        }

        private void FdaDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(double) || e.PropertyType == typeof(double?))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "C2";
            }
        }
    }
}
