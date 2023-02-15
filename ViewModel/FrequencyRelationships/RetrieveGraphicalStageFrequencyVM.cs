using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.MVVMFramework.ViewModel.Implementations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class RetrieveGraphicalStageFrequencyVM : ValidatingBaseViewModel
    {
        #region Fields
        private IndexPointsElement _selectedIndexPointSet;
        private HydraulicElement _selectedHydraulics;
        #endregion
        #region Properties
        public ObservableCollection<HydraulicElement> AvailableHydraulics { get; set; }
        public ObservableCollection<IndexPointsElement> AvailableIndexPointSets { get; set; }
        public HydraulicElement SelectedHydraulics
        {
            get
            {
                return _selectedHydraulics;
            }
            set
            {
                _selectedHydraulics = value;
                NotifyPropertyChanged();
            }
        }
        public IndexPointsElement SelectedIndexPointSet {
            get 
            {
                return _selectedIndexPointSet;
            }
            set 
            {
                _selectedIndexPointSet = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        #region Named Actions
        private NamedAction _generateFrequencyCurves;
        public NamedAction GenerateFrequencyCurves { get { return _generateFrequencyCurves; } set { _generateFrequencyCurves = value; NotifyPropertyChanged(); } }
        #endregion
        #region Constructors
        public RetrieveGraphicalStageFrequencyVM()
        {
            Initialize();
            GenerateFrequencyCurves = new NamedAction();
            GenerateFrequencyCurves.Name = "GenerateFrequencyCurves";
            GenerateFrequencyCurves.Action = GenerateFrequencyCurvesAction;
        }
        #endregion
        #region Methods
        private void Initialize()
        {
            AvailableHydraulics = new ObservableCollection<HydraulicElement>();
            //Add all hydraulic elements in the study here
            SelectedHydraulics = AvailableHydraulics[0];

            AvailableIndexPointSets = new ObservableCollection<IndexPointsElement>();
            //Add all indexPointsElements in the study here
            SelectedIndexPointSet = AvailableIndexPointSets[0];
        }

        private void GenerateFrequencyCurvesAction(object arg1, EventArgs arg2)
        {
            //Get the index points shapefile here somehow
            List<PairedData> freqCurves = SelectedHydraulics.DataSet.GetGraphicalStageFrequency("somepath", Storage.Connection.Instance.HydraulicsDirectory);
            foreach(PairedData freqCurve in freqCurves)
            {
                AddFrequencyRelationship(freqCurve);
            }
        }
        private void AddFrequencyRelationship(PairedData pairedData)
        {
            //Do Something Here
        }
        #endregion


    }
}
