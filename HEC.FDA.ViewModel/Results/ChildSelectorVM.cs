using HEC.CS.Collections;
using HEC.FDA.ViewModel.Compute;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Results
{
    public abstract class ChildSelectorVM : ComputeWithProgressAndMessagesBase, IProgressReport
    {
        public const string CANCEL_COMPUTE = "Cancel Compute";
        public const string COMPUTE = "Compute";
        private string _ComputeButtonLabel = COMPUTE;
        public CancellationTokenSource _CancellationToken;
        private bool _AllSelected;

        public event ProgressReportedEventHandler ProgressReport;
        public event MessageReportedEventHandler MessageReport;

        public CustomObservableCollection<ComputeChildRowItem> Rows { get; } = new CustomObservableCollection<ComputeChildRowItem>();

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
            MessageHub.Register(this);
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
