using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.TableWithPlot;
using System;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class CalculatedStageDamageRowItem : BaseViewModel
    {
        private ImpactAreaRowItem _ImpactArea;
        private String _DamCat;

        public StageDamageConstructionType ConstructionType { get; set; }

        public string AssetCategory { get; set; }
        public int ID { get; set; }
        public ImpactAreaRowItem ImpactArea
        {
            get { return _ImpactArea; }
            set { _ImpactArea = value; NotifyPropertyChanged(); }
        }
        public String DamageCategory
        {
            get { return _DamCat; }
            set { _DamCat = value; NotifyPropertyChanged(); }
        }
        public CurveComponentVM ComputeComponent { get; set; }

        public CalculatedStageDamageRowItem(int id, ImpactAreaRowItem impArea, String damCat, CurveComponentVM function, 
            string assetCategory, StageDamageConstructionType constructionType)
        {
            AssetCategory = assetCategory;
            ID = id;
            ImpactArea = impArea;
            DamageCategory = damCat;
            ComputeComponent = function;
            ConstructionType = constructionType;
        }
    }
}
