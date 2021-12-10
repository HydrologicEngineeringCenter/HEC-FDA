using ViewModel.ImpactArea;
using ViewModel.Inventory.DamageCategory;
using Functions;
using FunctionsView.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.AggregatedStageDamage
{
    public class ManualStageDamageRowItem : BaseViewModel
    {

        private ObservableCollection<ImpactAreaRowItem> _ImpactAreas;
        private String _SelectedDamCat;
        private ObservableCollection<String> _DamageCategories;
        private ImpactAreaRowItem _SelectedImpArea;

        public int ID { get; set; }
        public ObservableCollection<ImpactAreaRowItem> ImpactAreas
        {
            get { return _ImpactAreas; }
            set { _ImpactAreas = value; NotifyPropertyChanged(); }
        }

        public ImpactAreaRowItem SelectedImpArea
        {
            get { return _SelectedImpArea; }
            set { _SelectedImpArea = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<String> DamageCategories 
        {
            get { return _DamageCategories; }
            set { _DamageCategories = value; NotifyPropertyChanged(); }
        }
        public CoordinatesFunctionEditorVM EditorVM { get; set; }

        public String SelectedDamCat
        {
            get { return _SelectedDamCat; }
            set { _SelectedDamCat = value; NotifyPropertyChanged(); }
        }

        public ManualStageDamageRowItem(int id, ObservableCollection<ImpactAreaRowItem> impAreas, ObservableCollection<String> damCats, ICoordinatesFunction function)
        {
            ID = id;
            ImpactAreas = impAreas;
            SelectedImpArea = ImpactAreas[0];
            DamageCategories = damCats;
            if (damCats.Count > 0)
            {
                SelectedDamCat = damCats[0];
            }
            EditorVM = new CoordinatesFunctionEditorVM(function, "Stage", "Damage", "Stage-Damage");
        }

        public ManualStageDamageRowItem(int id, ObservableCollection<ImpactAreaRowItem> impAreas, ObservableCollection<String> damCats, StageDamageCurve curve)
        {
            ID = id;
            ImpactAreas = impAreas;
            SelectedImpArea = curve.ImpArea;
            DamageCategories = damCats;
            SelectedDamCat = curve.DamCat;
            EditorVM = new CoordinatesFunctionEditorVM(curve.Function, "Stage", "Damage", "Stage-Damage");

        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="rowItem"></param>
        public ManualStageDamageRowItem(int id, ManualStageDamageRowItem rowItem)
        {
            ID = id;
            ImpactAreas = rowItem.ImpactAreas;
            SelectedImpArea = rowItem.SelectedImpArea;
            DamageCategories = rowItem.DamageCategories;
            SelectedDamCat = rowItem.SelectedDamCat;
            EditorVM = new CoordinatesFunctionEditorVM(rowItem.EditorVM);
        }

    }
}
