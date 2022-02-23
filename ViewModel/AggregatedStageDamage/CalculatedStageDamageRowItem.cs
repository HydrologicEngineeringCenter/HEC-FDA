using paireddata;
using System;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.TableWithPlot;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
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
        //public CoordinatesFunctionEditorVM EditorVM { get; set; }
        public ComputeComponentVM ComputeComponent { get; set; }

        public CalculatedStageDamageRowItem(int id, ImpactAreaRowItem impArea, String damCat, ComputeComponentVM function)
        {
            ID = id;
            ImpactArea = impArea;
            DamageCategory = damCat;
            //EditorVM = new CoordinatesFunctionEditorVM(function, "Stage", "Damage", "Stage-Damage");
            ComputeComponent = function;
        }




    }
}
