using HEC.FDA.Model.alternatives;
using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Study;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Utility.Progress;
using Visual.Observables;

namespace HEC.FDA.ViewModel.Compute
{
    /// <summary>
    /// This guy should be the compute window for computing a alternative. 
    /// </summary>
    public class ComputeAlternativeVM : BaseViewModel
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

        public ComputeAlternativeVM(BatchJob batchJob) : base()
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

        /// <summary>
        /// Returns a 'cold' task. Does not actually start the task. Not Awaitable. 
        /// </summary>
        public static Task<AlternativeResults> RunAnnualizationCompute(AlternativeElement altElem, ProgressReporter reporter = null)
        {
            reporter ??= ProgressReporter.None();

            // Group the base and future scenarios for readability.
            var baseScenario = altElem.BaseScenario;
            var futureScenario = altElem.FutureScenario;

            // Retrieve the results from each scenario's element.
            var firstResults = baseScenario.GetElement().Results;
            var secondResults = futureScenario.GetElement().Results;

            // Retrieve the study properties.
            var studyProperties = StudyCache.GetStudyPropertiesElement();

            // Start the computation on a separate task.
            return Task.Run(() => Alternative.AnnualizationCompute(
                studyProperties.DiscountRate,
                studyProperties.PeriodOfAnalysis,
                altElem.ID,
                firstResults,
                secondResults,
                baseScenario.Year,
                futureScenario.Year,
                reporter
            ));
        }
    }
}
