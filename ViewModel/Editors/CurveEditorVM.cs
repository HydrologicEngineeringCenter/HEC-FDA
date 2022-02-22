using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Editors
{
    public abstract class CurveEditorVM : BaseEditorVM
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
            TableWithPlot = new TableWithPlotVM(elem.ComputeComponentVM);
        }
        #endregion

        public int GetElementID(SavingBase persistenceManager)
        {
            int id = -1;
            if (IsCreatingNewElement)
            {
                id = persistenceManager.GetNextAvailableId();
            }
            else
            {
                id = OriginalElement.ID;
            }
            return id;
        }

    }
}
