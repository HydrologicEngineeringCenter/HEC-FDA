﻿using HEC.FDA.ViewModel.Results;
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

namespace HEC.FDA.View.Results
{
    /// <summary>
    /// Interaction logic for ComputeChildSelector.xaml
    /// </summary>
    public partial class ComputeChildSelector : UserControl
    {
        public ComputeChildSelector()
        {
            InitializeComponent();
        }

        private void Compute_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ComputeChildSelectorVM vm)
            {
                vm.ComputeClicked();
            }
        }
    }
}