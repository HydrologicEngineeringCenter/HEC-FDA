using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Editors
{
    public abstract class CurveEditorVM : BaseLoggingEditorVM
    {
        private TableWithPlotVM _TableWithPlot;

        #region properties
        public TableWithPlotVM TableWithPlot
        {
            get { return _TableWithPlot; }
            set { _TableWithPlot = value; NotifyPropertyChanged(); }
        }    
        #endregion

        #region constructors
        public CurveEditorVM(ComputeComponentVM defaultCurve, EditorActionManager actionManager) :base(actionManager)
        {
            TableWithPlot = new TableWithPlotVM(defaultCurve);
        }

        public CurveEditorVM(CurveChildElement elem, EditorActionManager actionManager) :base(elem, actionManager)
        {
            ComputeComponentVM comp = new ComputeComponentVM( elem.ComputeComponentVM.ToXML());
            TableWithPlot = new TableWithPlotVM(comp);
        }
        #endregion
    }
}
