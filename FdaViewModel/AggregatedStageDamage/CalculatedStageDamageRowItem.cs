using FdaViewModel.ImpactArea;
using Functions;
using FunctionsView.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.AggregatedStageDamage
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


        public CalculatedStageDamageRowItem(int id, ImpactAreaRowItem impArea, String damCat, ICoordinatesFunction function)
        {
            ID = id;
            ImpactArea = impArea;
            DamageCategory = damCat;
            EditorVM = new CoordinatesFunctionEditorVM(function, "Stage", "Damage", "Stage-Damage", true);
        }




    }
}
