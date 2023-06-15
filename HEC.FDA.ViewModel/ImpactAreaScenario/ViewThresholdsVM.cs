using HEC.FDA.ViewModel.ImpactAreaScenario.Editor;
using HEC.FDA.ViewModel.Saving;
using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.ImpactAreaScenario
{
    public class ViewThresholdsVM : BaseViewModel
    {
        private readonly int _IASElementID;
        public ObservableCollection<ViewThresholdRowItem> Rows { get; set; } = new();
        public ViewThresholdsVM(IASElement elem)
        {
            _IASElementID = elem.ID;
            AddLiveUpdateEvents();
            LoadTheTable(elem);
        }
        private void LoadTheTable(IASElement elem)
        {
            Rows.Clear();
            foreach (SpecificIAS ias in elem.SpecificIASElements)
            {
                foreach (ThresholdRowItem thresh in ias.Thresholds)
                {
                    Rows.Add(new ViewThresholdRowItem(ias.GetSpecificImpactAreaName(), thresh.ThresholdType.DisplayName, thresh.ThresholdValue));
                }
            }
        }

        private void AddLiveUpdateEvents()
        {
            StudyCache.IASElementUpdated += UpdateIASElement;
        }

        private void UpdateIASElement(object sender, ElementUpdatedEventArgs e)
        {
            //If this element was updated, then reload the table to get the new changes.
            if (e.NewElement.ID == _IASElementID)
            {
                LoadTheTable((IASElement)e.NewElement);
            }
        }
    }
}
