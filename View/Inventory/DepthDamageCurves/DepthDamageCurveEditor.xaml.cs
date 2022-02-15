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
using System.Collections.ObjectModel;


namespace HEC.FDA.View.Inventory.DepthDamageCurves
{
    /// <summary>
    /// Interaction logic for DepthDamageCurveEditor.xaml
    /// </summary>
    public partial class DepthDamageCurveEditor : UserControl
    {
        private ListCollectionView _LCV;
        public DepthDamageCurveEditor()
        {
            InitializeComponent();

            
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorVM vm = (HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorVM)this.DataContext;

            LoadTheListView(this,new EventArgs());
            //DamageEditorControl.ListViewNeedsUpdating += new EventHandler(UpdateTheListView);
            for(int i =0;i< vm.ListOfDepthDamageVMs.Count;i++)
            {
                vm.ListOfDepthDamageVMs[i].ThisRowNeedsUpdating += new EventHandler(UpdateTheListView);
            }

        }
        private void UpdateTheListView(object sender, EventArgs e)
        {
            _LCV.Refresh();
        }
        private void LoadTheListView(object sender, EventArgs e)
        {
            //load the list view
            HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorVM vm = (HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorVM)this.DataContext;
            //if (vm.SelectedDepthDamageCurve == null) { return; }

            //ViewModel.Inventory.DepthDamage.DepthDamageCurve currentlySelectedCurve = vm.SelectedDepthDamageCurve;

            ObservableCollection<ViewModel.Inventory.DepthDamage.DepthDamageCurve> collectionOfCurves = new ObservableCollection<ViewModel.Inventory.DepthDamage.DepthDamageCurve>();

            foreach (HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurve curve in vm.CurveDictionary.Values)
            {
                
                collectionOfCurves.Add(curve);
            }
            _LCV = new ListCollectionView(vm.ListOfDepthDamageVMs);    //collectionOfCurves);

            _LCV.GroupDescriptions.Add(new PropertyGroupDescription(nameof(HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurve.DamageType)));
            _LCV.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurve.DamageType), System.ComponentModel.ListSortDirection.Ascending));
            _LCV.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurve.Name), System.ComponentModel.ListSortDirection.Ascending));

            DepthDamageListView.ItemsSource = _LCV;
            
            //vm.SelectedDepthDamageCurve = currentlySelectedCurve; // for some reason, the line above (itemssource) is setting the selectedddcurve back to the first one.

        }

        //public void UpdateTheListView(object sender, EventArgs e)
        //{
        //    //load the list view
        //    HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorVM vm = (HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorVM)this.DataContext;
        //    if (vm.SelectedDepthDamageCurve == null) { return; }

        //    ObservableCollection<ViewModel.Inventory.DepthDamage.DepthDamageCurve> collectionOfCurves = new ObservableCollection<ViewModel.Inventory.DepthDamage.DepthDamageCurve>();

        //    foreach (HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurve curve in HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveData.CurveDictionary.Values)
        //    //for(int i = 0;i< HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveData.CurveDictionary.Count();i++)
        //    {
        //        collectionOfCurves.Add( curve);
        //        //collectionOfCurves.Add(ot);
        //    }
        //    ListCollectionView lcv = new ListCollectionView(collectionOfCurves);

        //    lcv.GroupDescriptions.Add(new PropertyGroupDescription(nameof(HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurve.DamageType)));
        //    lcv.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurve.DamageType), System.ComponentModel.ListSortDirection.Ascending));
        //    lcv.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurve.Name), System.ComponentModel.ListSortDirection.Ascending));

        //    DepthDamageListView.ItemsSource = lcv;
        //}


        private void CreateNewButton_Click(object sender, RoutedEventArgs e)
        {
            HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorVM vm = (HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorVM)this.DataContext;
            vm.LanuchNewDepthDamageWindow();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorVM vm = (HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorVM)this.DataContext;
            vm.ListOfDepthDamageVMs.Remove(vm.CurrentDepthDamageCurveVM);
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorVM vm = (HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorVM)this.DataContext;
            vm.LanuchCopyDepthDamageWindow();
        }

        private void DepthDamageListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            //ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorVM vm = (HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorVM)this.DataContext;

            //vm.ListOfDepthDamageVMs.Remove(vm.CurrentDepthDamageCurveVM);
            
            
            //ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorControlVM ddc = (HEC.FDA.ViewModel.Inventory.DepthDamage.DepthDamageCurveEditorControlVM)e.AddedItems[0];
            //if(ddc != null)
            //{
            //    vm.ChangeSelectedDepthDamageCurve(ddc);

            //}
        }
    }
}
