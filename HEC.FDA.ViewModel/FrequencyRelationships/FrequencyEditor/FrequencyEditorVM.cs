using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.MVVMFramework.ViewModel.Implementations;

namespace HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor
{
	public class FrequencyEditorVM:ValidatingBaseViewModel
    {
        #region Fields
        private BaseViewModel _analyticalVM;
        private TableWithPlotVM _graphicalVM;
        private bool _isGraphical = false; //new windows open with analytical vm open
        #endregion
        #region Properties
        public BaseViewModel AnalyticalVM
		{
			get { return _analyticalVM; }
			set { _analyticalVM = value; }
		}
		public TableWithPlotVM GraphicalVM
		{
			get { return _graphicalVM; }
			set { _graphicalVM = value; }
		}
        public bool IsGraphical
        {
            get { return _isGraphical; }
            set
            {
                _isGraphical = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsAnalytic));
            }
        }
        public bool IsAnalytic
        {
            get { return !IsGraphical; }
        }
        #endregion
        #region Constructors
        public FrequencyEditorVM()
        {
            AnalyticalVM = new AnalyticalVM();
            GraphicalVM = new TableWithPlotVM(new GraphicalVM("name","x","y"));
        }

        #endregion


    }
}
