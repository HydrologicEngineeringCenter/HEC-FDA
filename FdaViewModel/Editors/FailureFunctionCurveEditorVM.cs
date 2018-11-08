using FdaViewModel.GeoTech;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Editors
{
    public class FailureFunctionCurveEditorVM: CurveEditorVM
    {
        private ObservableCollection<LeveeFeatureElement> _LateralStructureList;
        private LeveeFeatureElement _SelectedLateralStructure;
        public LeveeFeatureElement SelectedLateralStructure
        {
            get { return _SelectedLateralStructure; }
            set { _SelectedLateralStructure = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<LeveeFeatureElement> LateralStructureList
        {
            get { return _LateralStructureList; }
            set { _LateralStructureList = value; NotifyPropertyChanged(); }
        }        //public EditorActionManager ActionManager { get; set; }


        public FailureFunctionCurveEditorVM(Statistics.UncertainCurveDataCollection defaultCurve, ObservableCollection<LeveeFeatureElement> latStructList, EditorActionManager actionManager) :base(defaultCurve, actionManager)
        {
            LateralStructureList = latStructList;
            StudyCache.LeveeAdded += StudyCache_LeveeAdded;

        }


        public FailureFunctionCurveEditorVM(ChildElement element, ObservableCollection<LeveeFeatureElement> latStructList, EditorActionManager actionManager) : base(element, actionManager)
        {
            LateralStructureList = latStructList;
            StudyCache.LeveeAdded += StudyCache_LeveeAdded;

        }

        private void StudyCache_LeveeAdded(object sender, Saving.ElementAddedEventArgs args)
        {
            LateralStructureList.Add((LeveeFeatureElement)args.Element);
        }
       

    }
}
