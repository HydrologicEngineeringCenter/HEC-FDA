﻿using HEC.FDA.ViewModel.Inventory.DamageCategory;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.Inventory.DamageCategory
{
    /// <summary>
    /// Interaction logic for DamageCategoryEditor.xaml
    /// </summary>
    public partial class DamageCategoryEditor : UserControl
    {
        public DamageCategoryEditor()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            DamageCategoriesVM vm = (DamageCategoriesVM)DataContext;
            if(Grid.SelectedIndex == -1)
            {
                vm.AddDamageCategory(Grid.Items.Count);
            }
            else
            {
                vm.AddDamageCategory(Grid.SelectedIndex);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            DamageCategoriesVM vm = (DamageCategoriesVM)DataContext;
            if (Grid.SelectedIndex == -1)
            {
                vm.RemoveDamageCategory(Grid.Items[Grid.Items.Count-1]);
            }
            else
            {
                vm.RemoveDamageCategory(Grid.SelectedItem);
            }
        }
    }
}