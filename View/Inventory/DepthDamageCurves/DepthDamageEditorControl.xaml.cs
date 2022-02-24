using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.Inventory.DepthDamageCurves
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
            HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorControlVM vm = (HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorControlVM)this.DataContext;

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


            HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorControlVM vm = (HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorControlVM)this.DataContext;
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
