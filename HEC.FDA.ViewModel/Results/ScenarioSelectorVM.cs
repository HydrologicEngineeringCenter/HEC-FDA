using HEC.FDA.Model.metrics;
using HEC.FDA.Model.scenarios;
using HEC.FDA.ViewModel.Compute;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using System;
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

        private void LoadComputeManager(List<ComputeChildRowItem> scenarioRows)
        {
            List<IASElement> elems = new List<IASElement>();
            foreach (ComputeChildRowItem row in scenarioRows)
            {
                IASElement elem = (IASElement)row.ChildElement;
                FdaValidationResult canComputeVR = elem.CanCompute();
                if (canComputeVR.IsValid)
                {
                    elems.Add(row.ChildElement as IASElement);
                }
                else
                {
                    row.MarkInError(canComputeVR.ErrorMessage);
                }
            }
            ScenarioProgressManager.Update(elems);
        }

        public override async void Compute(List<ComputeChildRowItem> scenarioRows)
        {
            LoadComputeManager(scenarioRows);
            List<Task> taskList = new List<Task>();
            try
            {
                foreach (KeyValuePair<IASElement, Scenario> keyValue in ScenarioProgressManager.Scenarios)
                {
                    taskList.Add(ComputeScenarioVM.ComputeScenario(keyValue.Key, keyValue.Value, ComputeCompleted, _CancellationToken.Token));
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

            var result = MessageBox.Show("Do you want to view summary results?", "Compute Finished", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ScenarioDamageSummaryVM vm = new ScenarioDamageSummaryVM(ScenarioProgressManager.Scenarios.Keys.ToList());
                DynamicTabVM tab = new DynamicTabVM(StringConstants.VIEW_SUMMARY_RESULTS_HEADER, vm, StringConstants.VIEW_SUMMARY_RESULTS_HEADER);
                Navigate(tab, false, false);
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

        private void ComputeCompleted(IASElement elem, ScenarioResults results)
        {
            elem.Results = results;
            //have to get back on the main thread to save.
            Application.Current.Dispatcher.Invoke(
            (Action)(() =>
            {
                PersistenceFactory.GetIASManager().SaveExisting(elem);
                IASTooltipHelper.UpdateTooltip(elem);
            }));
        }
       
    }
}
