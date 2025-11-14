using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Visual.Observables;

namespace HEC.FDA.ViewModel.Compute;
public class StageDamageComputeLogVM: BaseViewModel
{
    private BatchJob _job;

    public BatchJob Job
    {
        get { return _job; }
        set
        {
            _job = value;
            NotifyPropertyChanged();
        }
    }

    public StageDamageComputeLogVM(BatchJob batchJob) : base()
    {
        _job = batchJob;
        _job.ProgressChanged += JobProgressChanged;
        _job.PropertyChanged += JobPropertyChanged;
    }

    private void JobProgressChanged(BatchJob sender, double progress)
    {
        NotifyPropertyChanged(nameof(Job));
    }

    private void JobPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        NotifyPropertyChanged(nameof(Job));
    }

}
