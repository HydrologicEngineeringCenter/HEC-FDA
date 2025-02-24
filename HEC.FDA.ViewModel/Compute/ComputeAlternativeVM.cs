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

        public static Task RunAnnualizationCompute(AlternativeElement altElem, Action<AlternativeResults> callback, ProgressReporter reporter = null)
        {
            if (reporter == null)
            {
                reporter = ProgressReporter.None();
            }
            IASElement firstElem = altElem.BaseScenario.GetElement();
            IASElement secondElem = altElem.FutureScenario.GetElement();

            ScenarioResults firstResults = firstElem.Results;
            ScenarioResults secondResults = secondElem.Results;
            StudyPropertiesElement studyProperties = StudyCache.GetStudyPropertiesElement();

            double discountRate = studyProperties.DiscountRate;
            int periodOfAnalysis = studyProperties.PeriodOfAnalysis;

            int baseYear = altElem.BaseScenario.Year;
            int futureYear = altElem.FutureScenario.Year;
            return Task.Run(() =>
            {
                AlternativeResults results = Alternative.AnnualizationCompute(discountRate, periodOfAnalysis, altElem.ID,
                    firstResults, secondResults, baseYear, futureYear, reporter);
                callback?.Invoke(results);
            });
        }
    }
}
