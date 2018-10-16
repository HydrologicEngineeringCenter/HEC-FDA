using FdaViewModel.GeoTech;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Editors
{
    public class FailureFunctionCurveEditorVM: CurveEditorVM
    {
        private List<LeveeFeatureElement> _LateralStructureList;
        private LeveeFeatureElement _SelectedLateralStructure;
        public LeveeFeatureElement SelectedLateralStructure
        {
            get { return _SelectedLateralStructure; }
            set { _SelectedLateralStructure = value; NotifyPropertyChanged(); }
        }
        public List<LeveeFeatureElement> LateralStructureList
        {
            get { return _LateralStructureList; }
            set { _LateralStructureList = value; NotifyPropertyChanged(); }
        }        //public EditorActionManager ActionManager { get; set; }


        public FailureFunctionCurveEditorVM(Statistics.UncertainCurveDataCollection defaultCurve, List<LeveeFeatureElement> latStructList, EditorActionManager actionManager) :base(defaultCurve, actionManager)
        {
            LateralStructureList = latStructList;

        }
        public FailureFunctionCurveEditorVM(OwnedElement element, List<LeveeFeatureElement> latStructList, EditorActionManager actionManager) : base(element, actionManager)
        {
            LateralStructureList = latStructList;
        }

       

    }
}
