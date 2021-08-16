using ViewModel.GeoTech;
using ViewModel.Utilities;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Editors
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


        public FailureFunctionCurveEditorVM(IFdaFunction defaultCurve, ObservableCollection<LeveeFeatureElement> latStructList, string xLabel, string yLabel, string chartTitle, EditorActionManager actionManager) :base(defaultCurve, xLabel, yLabel, chartTitle, actionManager)
        {
            LateralStructureList = latStructList;
            StudyCache.LeveeAdded += StudyCache_LeveeAdded;

        }


        public FailureFunctionCurveEditorVM(ChildElement element, ObservableCollection<LeveeFeatureElement> latStructList, string xLabel, string yLabel, string chartTitle, EditorActionManager actionManager) : base(element, xLabel, yLabel, chartTitle, actionManager)
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
