using paireddata;
using System;
using ViewModel.ImpactArea;

namespace ViewModel.AggregatedStageDamage
{
    public class CalculatedStageDamageRowItem : BaseViewModel
    {
        private ImpactAreaRowItem _ImpactArea;
        private String _DamCat;


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
        public CoordinatesFunctionEditorVM EditorVM { get; set; }


        public CalculatedStageDamageRowItem(int id, ImpactAreaRowItem impArea, String damCat, UncertainPairedData function)
        {
            ID = id;
            ImpactArea = impArea;
            DamageCategory = damCat;
            EditorVM = new CoordinatesFunctionEditorVM(function, "Stage", "Damage", "Stage-Damage", true);
        }




    }
}
