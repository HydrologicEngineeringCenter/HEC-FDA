using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.ViewModel.Implementations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NamedAction = HEC.MVVMFramework.ViewModel.Implementations.NamedAction;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class RetrieveGraphicalStageFrequencyVM : BaseViewModel
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
            List<HydraulicElement> hydraulicElements = StudyCache.GetChildElementsOfType<HydraulicElement>();
            foreach(HydraulicElement hydraulic in hydraulicElements)
            {
                AvailableHydraulics.Add(hydraulic);
            }
            SelectedHydraulics = AvailableHydraulics[0];

            AvailableIndexPointSets = new ObservableCollection<IndexPointsElement>();
            List<IndexPointsElement> indexptsElements = StudyCache.GetChildElementsOfType<IndexPointsElement>();
            foreach(IndexPointsElement indexpt in indexptsElements)
            {
                AvailableIndexPointSets.Add(indexpt);
            }
            SelectedIndexPointSet = AvailableIndexPointSets[0];
        }

        private void GenerateFrequencyCurvesAction(object arg1, EventArgs arg2)
        {

            string pointShapefile = Storage.Connection.Instance.IndexPointsDirectory + "\\" + SelectedIndexPointSet.Name + "\\" + SelectedIndexPointSet.Name + ".shp";
            string hydraulicParentDirectory = Storage.Connection.Instance.HydraulicsDirectory+"\\"+SelectedHydraulics.Name;
            List<UncertainPairedData> freqCurves = SelectedHydraulics.DataSet.GetGraphicalStageFrequency(pointShapefile,hydraulicParentDirectory );
            for(int i =0; i < freqCurves.Count; i++)
            {
                AddFrequencyRelationship(freqCurves[i], SelectedIndexPointSet.IndexPoints[i]+ "|" + SelectedHydraulics.Name);
            }
        }
        private void AddFrequencyRelationship(UncertainPairedData upd, string name)
        {
            string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
            int id = PersistenceFactory.GetElementManager<AnalyticalFrequencyElement>().GetNextAvailableId();
            //Create graphical VM
            GraphicalVM graphicalVM = new GraphicalVM(StringConstants.GRAPHICAL_FREQUENCY, StringConstants.EXCEEDANCE_PROBABILITY, StringConstants.DISCHARGE);
            graphicalVM.Options[0].UpdateFromUncertainPairedData(upd);

            AnalyticalFrequencyElement element = new AnalyticalFrequencyElement(name, editDate,"Retrieved from Hydraulics",DefaultData.PeriodOfRecord, false, true, DefaultData.LP3Mean,DefaultData.LP3StDev, DefaultData.LP3Skew,new List<double>(), graphicalVM,new CurveComponentVM(), id);
            IElementManager elementManager = PersistenceFactory.GetElementManager(element);

            List<AnalyticalFrequencyElement> existingElements = StudyCache.GetChildElementsOfType<AnalyticalFrequencyElement>(); 
            bool newElementMatchesExisting = false;
            foreach(AnalyticalFrequencyElement ele in existingElements)
            {
                if (ele.Name.Equals(name))
                {
                    element.ID = ele.ID;
                    elementManager.SaveExisting(element);
                    newElementMatchesExisting = true;
                    break;
                }
            }
            if (!newElementMatchesExisting)
            {
                elementManager.SaveNew(element);
            }
        }


        #endregion
    }
}
