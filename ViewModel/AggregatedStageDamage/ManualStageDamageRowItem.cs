using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.TableWithPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
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
        public ComputeComponentVM ComputeComponent { get; set; }

        public String SelectedDamCat
        {
            get { return _SelectedDamCat; }
            set { _SelectedDamCat = value; NotifyPropertyChanged(); }
        }

        public List<string> AssetCategories { get; } = new List<string>() { "Structure", "Content", "Vehicle", "Other", "Total" };
        
        public string SelectedAssetCategory { get; set; }

        public ManualStageDamageRowItem(int id, ObservableCollection<ImpactAreaRowItem> impAreas, ObservableCollection<String> damCats, ComputeComponentVM function, string assetCategory)
        {
            ID = id;
            ImpactAreas = impAreas;
            SelectedImpArea = ImpactAreas[0];
            DamageCategories = damCats;
            if (damCats.Count > 0)
            {
                SelectedDamCat = damCats[0];
            }
            ComputeComponent = function;
            SelectedAssetCategory = assetCategory;
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
            ComputeComponent = rowItem.ComputeComponent.Clone();
            SelectedAssetCategory = rowItem.SelectedAssetCategory;
        }
        
    }
}
