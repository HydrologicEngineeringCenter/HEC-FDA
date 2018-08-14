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

namespace Fda.Inventory.DamageCategory
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
            FdaViewModel.Inventory.DamageCategory.DamageCategoriesVM vm = (FdaViewModel.Inventory.DamageCategory.DamageCategoriesVM)DataContext;
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
            FdaViewModel.Inventory.DamageCategory.DamageCategoriesVM vm = (FdaViewModel.Inventory.DamageCategory.DamageCategoriesVM)DataContext;
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
