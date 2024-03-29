﻿using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using System;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.Inventory.OccupancyTypes.Controls
{
    /// <summary>
    /// Interaction logic for OccTypeEditorControl.xaml
    /// </summary>
    public partial class OccTypeEditorControl : UserControl
    {
        public event EventHandler ListViewNeedsUpdating;

        public OccTypeEditorControl()
        {
            InitializeComponent();
        }

        private void DamageCategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //redraw the list view so that the occtype that changed dam cats will be in the correct group
            ListViewNeedsUpdating?.Invoke(this, new EventArgs());
        }

        private void CreateNewDamCat_Click(object sender, RoutedEventArgs e)
        {
            OccupancyTypeEditable vm = (OccupancyTypeEditable)DataContext;
            if (vm == null) { return; }
            vm.LaunchNewDamCatWindow();
            ListViewNeedsUpdating?.Invoke(this, new EventArgs());
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
            {
                Visibility = Visibility.Collapsed;
            }
          
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext == null)
            {
                Visibility = Visibility.Collapsed;
            }
            else
            {
                Visibility = Visibility.Visible;
            }
        }
    }
}
