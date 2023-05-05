using HEC.CS.Collections;
using HEC.FDA.ViewModel.Compute;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Interfaces;
using System.Collections.Generic;
using System.Threading;

namespace HEC.FDA.ViewModel.Results
{
    /// <summary>
    /// Base class for the selection of child elements for running batch computes. (Scenarios, Alternatives, Alt comp reports)
    /// </summary>
    public abstract class ChildSelectorVM : BaseViewModel, IProgressReport
    {

        public const string CANCEL_COMPUTE = "Cancel Compute";
        public const string COMPUTE = "Compute";
        private string _ComputeButtonLabel = COMPUTE;
        public CancellationTokenSource _CancellationToken;
        private bool _AllSelected;
        public ScenarioProgressManager ScenarioProgressManager { get; } = new ScenarioProgressManager();

        public event ProgressReportedEventHandler ProgressReport;
        public event MessageReportedEventHandler MessageReport;

        public CustomObservableCollection<ComputeChildRowItem> Rows { get; } = new CustomObservableCollection<ComputeChildRowItem>();
        public CustomObservableCollection<ProgressRowItem> ProgressRows { get; } = new CustomObservableCollection<ProgressRowItem>();
        public string ComputeButtonLabel
        {
            get { return _ComputeButtonLabel; }
            set { _ComputeButtonLabel = value; NotifyPropertyChanged(); }
        }

        public bool SelectAll
        {
            get { return _AllSelected; }
            set { _AllSelected = value; SelectAllRows(); }
        }

        public ChildSelectorVM()
        {
            LoadChildElements();
        }

        public abstract void LoadChildElements();
        public abstract void ListenToChildElementUpdateEvents();
        public abstract void Compute(List<ComputeChildRowItem> rows);

        public List<ComputeChildRowItem> GetSelectedRows()
        {
            List<ComputeChildRowItem> selectedRows = new List<ComputeChildRowItem>();
            foreach (ComputeChildRowItem row in Rows)
            {
                if (row.IsSelected)
                {
                    selectedRows.Add(row);
                }
            }
            return selectedRows;
        }

        public void ComputeClicked()
        {
            if (ComputeButtonLabel.Equals(CANCEL_COMPUTE))
            {
                CancelCompute();
                ComputeButtonLabel = COMPUTE;
            }
            else
            {
                List<ComputeChildRowItem> computeChildRowItems = GetSelectedRows();

                if (computeChildRowItems.Count > 0)
                {
                    _CancellationToken = new CancellationTokenSource();
                    Compute(computeChildRowItems);
                    ComputeButtonLabel = CANCEL_COMPUTE;
                }
            }
        }

        public void SelectAllRows()
        {
            foreach (ComputeChildRowItem row in Rows)
            {
                if (!row.HasError)
                {
                    row.IsSelected = _AllSelected;
                }
            }
        }

        public void ReportProgress(object sender, ProgressReportEventArgs e)
        {
            ProgressReport?.Invoke(sender, e);
        }

        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }

        public void CancelCompute()
        {
            if (_CancellationToken != null)
            {
                _CancellationToken.Cancel();
            }
        }

    }
}
