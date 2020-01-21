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

namespace View.Inventory.DepthDamageCurves
{
    /// <summary>
    /// Interaction logic for DepthDamageEditorControl.xaml
    /// </summary>
    public partial class DepthDamageEditorControl : UserControl
    {
        //public event EventHandler ListViewNeedsUpdating;
        public DepthDamageEditorControl()
        {
            InitializeComponent();
        }

        private void DamageTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FdaViewModel.Inventory.DepthDamage.DepthDamageCurveEditorControlVM vm = (FdaViewModel.Inventory.DepthDamage.DepthDamageCurveEditorControlVM)this.DataContext;

            vm.UpdateTheRow();
            //if (this.ListViewNeedsUpdating != null)
            //{
            //    this.ListViewNeedsUpdating(this, new EventArgs());
            //}
        }

        //private void DepthDamageNameBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (this.ListViewNeedsUpdating != null)
        //    {
        //        this.ListViewNeedsUpdating(this, new EventArgs());
        //    }
        //}

        private void DepthDamageNameBox_LostFocus(object sender, RoutedEventArgs e)
        {


            FdaViewModel.Inventory.DepthDamage.DepthDamageCurveEditorControlVM vm = (FdaViewModel.Inventory.DepthDamage.DepthDamageCurveEditorControlVM)this.DataContext;
            // i need to check for name conflicts. We enforce unique names for depth damage curves
            vm.CheckWithParentForNameConflict(DepthDamageNameBox.Text);
            vm.Name = DepthDamageNameBox.Text;
            vm.UpdateTheRow();
            //if (this.ListViewNeedsUpdating != null)
            //{
            //    this.ListViewNeedsUpdating(this, new EventArgs());
            //}
        }



        //private void DepthDamageNameBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    if (this.ListViewNeedsUpdating != null)
        //    {
        //        this.ListViewNeedsUpdating(this, new EventArgs());
        //    }
        //}
    }
}
