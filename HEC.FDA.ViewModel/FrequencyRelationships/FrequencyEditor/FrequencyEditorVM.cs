using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;

namespace HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor
{
	public class FrequencyEditorVM:BaseEditorVM
    {
        #region Fields
        private BaseViewModel _analyticalVM;
        private CurveComponentVM _graphicalVM;
        private bool _isGraphical = false; //new windows open with analytical vm open
        #endregion
        #region Properties
        public BaseViewModel AnalyticalVM
		{
			get { return _analyticalVM; }
			set { _analyticalVM = value; }
		}
		public CurveComponentVM GraphicalVM
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
        public FrequencyEditorVM(EditorActionManager actionManager) : base(actionManager)
		{

		}
		public FrequencyEditorVM(AnalyticalFrequencyElement elem, EditorActionManager actionManager) : base(elem, actionManager)
		{

		}

        public override void Save()
        {
            throw new System.NotImplementedException();
        }
        #endregion


    }
}
