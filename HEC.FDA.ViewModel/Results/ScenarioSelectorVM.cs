using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.scenarios;
using HEC.FDA.ViewModel.Compute;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HEC.FDA.ViewModel.Results
{
    public class ScenarioSelectorVM : ChildSelectorVM
    {
        
        public ScenarioSelectorVM():base()
        {

            ValidateScenarios();
        }

        public override void ListenToChildElementUpdateEvents()
        {
            StudyCache.IASElementAdded += IASAdded;
            StudyCache.IASElementRemoved += IASRemoved;
            StudyCache.IASElementUpdated += IASUpdated;
        }

        private void IASAdded(object sender, ElementAddedEventArgs e)
        {
            ComputeChildRowItem newRow = new ComputeChildRowItem((IASElement)e.Element);
            Rows.Add(newRow);
            ValidateScenario(newRow);
        }

        private void IASRemoved(object sender, ElementAddedEventArgs e)
        {
            Rows.Remove(Rows.Where(row => row.ChildElement.ID == e.Element.ID).Single());
        }

        private void IASUpdated(object sender, ElementUpdatedEventArgs e)
        {
            IASElement newElement = (IASElement)e.NewElement;
            int idToUpdate = newElement.ID;

            //find the row with this id and update the row's values;
            ComputeChildRowItem foundRow = Rows.Where(row => row.ChildElement.ID == idToUpdate).SingleOrDefault();
            if (foundRow != null)
            {
                foundRow.Update(newElement);
                ValidateScenario(foundRow);
            }
        }

        public override void LoadChildElements()
        {
            List<IASElement> elems = StudyCache.GetChildElementsOfType<IASElement>();

            foreach (IASElement elem in elems)
            {
                Rows.Add(new ComputeChildRowItem(elem));
            }
        }    

        public override async void Compute(List<ComputeChildRowItem> scenarioRows)
        {
            MessageEventArgs beginComputeMessageArgs = new MessageEventArgs(new Message("Beginning Batch Compute"));
            ReportMessage(this, beginComputeMessageArgs);
            List<Task> taskList = new List<Task>();
            List<IASElement> elementList = new List<IASElement>();
            try
            {
                foreach (ComputeChildRowItem row in scenarioRows)
                {
                    IASElement elem = (IASElement)row.ChildElement;
                    elementList.Add(elem);
                    FdaValidationResult canComputeVR = elem.CanCompute();
                    if (canComputeVR.IsValid)
                    {
                        List<ImpactAreaScenarioSimulation> sims = ComputeScenarioVM.CreateSimulations(elem.SpecificIASElements);
                        RegisterProgressAndMessages(sims);
                        Scenario scenario = new Scenario(elem.AnalysisYear, sims);
                        taskList.Add(ComputeScenarioVM.ComputeScenario(scenario, ComputeCompleted, _CancellationToken.Token));
                    }
                    else
                    {
                        row.MarkInError(canComputeVR.ErrorMessage);
                    }
                }
                await Task.WhenAll(taskList.ToArray());
            }
            catch (TaskCanceledException ex)
            {
                MessageBox.Show("Compute Canceled.", "Compute Canceled", MessageBoxButton.OK, MessageBoxImage.Information);
                ComputeButtonLabel = COMPUTE;
                return;
            }
            ComputeButtonLabel = COMPUTE;
            MessageEventArgs finishedComputeMessageArgs = new MessageEventArgs(new Message("All Scenarios Computed"));
            ReportMessage(this, finishedComputeMessageArgs);
            UpdateIASElementTooltips(elementList);
            var result = MessageBox.Show("Do you want to view summary results?", "Compute Finished", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                ScenarioDamageSummaryVM vm = new ScenarioDamageSummaryVM(elementList);
                //todo: add to string constants
                DynamicTabVM tab = new DynamicTabVM(StringConstants.VIEW_SUMMARY_RESULTS_HEADER, vm, StringConstants.VIEW_SUMMARY_RESULTS_HEADER);
                Navigate(tab, false, false);
            }
        }

        private void UpdateIASElementTooltips(List<IASElement> elems)
        {
            foreach(IASElement elem in elems)
            {
                IASTooltipHelper.UpdateTooltip(elem);
            }
        }

        private void ValidateScenarios()
        {
            foreach (ComputeChildRowItem row in Rows)
            {
                ValidateScenario(row);
            }
        }

        private void ValidateScenario(ComputeChildRowItem row)
        {
            IASElement elem = (IASElement)row.ChildElement;
            FdaValidationResult canComputeVR = elem.CanCompute();
            if (!canComputeVR.IsValid)
            {
                row.MarkInError(canComputeVR.ErrorMessage);
            }
            else
            {
                row.ClearErrorStatus();
            }
        }

        private void ComputeCompleted(ScenarioResults results)
        {
            //todo: do something here? Save? update progress bar?
            int test = 0;
        }
       
    }
}
