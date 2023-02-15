using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.ViewModel.Implementations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NamedAction = HEC.MVVMFramework.ViewModel.Implementations.NamedAction;

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
            List<UncertainPairedData> freqCurves = SelectedHydraulics.DataSet.GetGraphicalStageFrequency("somepath", Storage.Connection.Instance.HydraulicsDirectory);
            foreach(UncertainPairedData freqCurve in freqCurves)
            {
                AddFrequencyRelationship(freqCurve);
            }
        }
        private void AddFrequencyRelationship(UncertainPairedData upd)
        {
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            int id = PersistenceFactory.GetElementManager<AnalyticalFrequencyElement>().GetNextAvailableId();
            GraphicalVM graphicalVM = new GraphicalVM(StringConstants.GRAPHICAL_FREQUENCY, StringConstants.EXCEEDANCE_PROBABILITY, StringConstants.DISCHARGE);
            graphicalVM.Options[0].UpdateFromUncertainPairedData(upd);
            AnalyticalFrequencyEditorVM vm = new AnalyticalFrequencyEditorVM();
            AnalyticalFrequencyElement element = AnalyticalFrequencyElement.CreateGraphicalFrequencyElement(editDate,graphicalVM,id);
            //element.save
        }
        #endregion


    }
}
