﻿using HEC.FDA.Model.alternatives;
using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.Compute;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results;
using HEC.FDA.ViewModel.Results;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HEC.FDA.ViewModel.Alternatives.Results.BatchCompute
{
    /// <summary>
    /// This class is used to select multiple alternatives to run a batch compute that is multi-threaded.
    /// </summary>
    public class AlternativeSelectorVM : ChildSelectorVM
    {

        public AlternativeSelectorVM() : base()
        {
            ComputeButtonLabel = "View";
            ValidateAlternatives();
            ListenToChildElementUpdateEvents();
        }

        private void ValidateAlternatives()
        {
            foreach (ComputeChildRowItem row in Rows)
            {
                ValidateAlternatives(row);
            }
        }

        private void ValidateAlternatives(ComputeChildRowItem row)
        {
            AlternativeElement elem = (AlternativeElement)row.ChildElement;
            FdaValidationResult canComputeVR = elem.RunPreComputeValidation();
            if (!canComputeVR.IsValid)
            {
                row.MarkInError(canComputeVR.ErrorMessage);
            }
            else
            {
                row.ClearErrorStatus();
            }
        }

        public override void ListenToChildElementUpdateEvents()
        {
            StudyCache.AlternativeAdded += AlternativeAdded;
            StudyCache.AlternativeRemoved += AlternativeRemoved;
            StudyCache.AlternativeUpdated += AlternativeUpdated;
        }

        private void AlternativeAdded(object sender, ElementAddedEventArgs e)
        {
            ComputeChildRowItem newRow = new ComputeChildRowItem((AlternativeElement)e.Element);
            Rows.Add(newRow);
            ValidateAlternatives(newRow);
        }

        private void AlternativeRemoved(object sender, ElementAddedEventArgs e)
        {
            Rows.Remove(Rows.Where(row => row.ChildElement.ID == e.Element.ID).Single());
        }

        private void AlternativeUpdated(object sender, ElementUpdatedEventArgs e)
        {
            AlternativeElement newElement = (AlternativeElement)e.NewElement;
            int idToUpdate = newElement.ID;

            //find the row with this id and update the row's values;
            ComputeChildRowItem foundRow = Rows.Where(row => row.ChildElement.ID == idToUpdate).SingleOrDefault();
            if (foundRow != null)
            {
                foundRow.Update(newElement);
                ValidateAlternatives(foundRow);
            }
        }


        public override void LoadChildElements()
        {
            List<AlternativeElement> elems = StudyCache.GetChildElementsOfType<AlternativeElement>();

            foreach (AlternativeElement elem in elems)
            {
                Rows.Add(new ComputeChildRowItem(elem));
            }
        }

        public override void ComputeClicked()
        {
            List<ComputeChildRowItem> computeChildRowItems = GetSelectedRows();

            if (computeChildRowItems.Count > 0)
            {
                _CancellationToken = new CancellationTokenSource();
                Compute(computeChildRowItems);
            }
        }

        public override async void Compute(List<ComputeChildRowItem> altRows)
        {
            ReportMessage(this, new MessageEventArgs(new Message("Beginning Batch Compute")));

            List<Task<AlternativeResults>> computeTasks = [];
            List<AlternativeElement> alternativeElements = [];
            StudyPropertiesElement props = StudyCache.GetStudyPropertiesElement();

            foreach (ComputeChildRowItem row in altRows)
            {
                // Retrieve the alternative element from the row.
                var altElement = (AlternativeElement)row.ChildElement;
                alternativeElements.Add(altElement);

                // Check if the element is valid for computing.
                var validation = altElement.RunPreComputeValidation();
                if (validation.IsValid)
                {
                    // RunAnnualizationCompute returns a started task.
                    computeTasks.Add(AlternativeComputer.RunAnnualizationCompute(altElement, props));
                }
                else
                {
                    row.MarkInError(validation.ErrorMessage);
                }
            }
            // Wait for all valid compute tasks to finish.
            AlternativeResults[] aggregatedResults = await Task.WhenAll(computeTasks);

            // Assign the computed results to the matching alternative element.
            foreach (var result in aggregatedResults)
            {
                ComputeCompleted(result);
            }

            if (HasFatalError)
            {
                MessageBox.Show("One or more of your selected alternatives failed to compute");
                HasFatalError = false;
                return;
            }

            // Report that all scenarios have been computed.
            ReportMessage(this, new MessageEventArgs(new Message("All Scenarios Computed")));

            var altSummaryVM = new AlternativeSummaryVM(alternativeElements);
            const string header = "Alternative Summary Results";
            var dynamicTabVM = new DynamicTabVM(header, altSummaryVM, header);
            Navigate(dynamicTabVM, false, true);
        }


        private void ComputeCompleted(AlternativeResults results)
        {
            if (results == null)
            {
                HasFatalError = true;

                return; // if failed, don't try to save a result. 
            }
            // Assign the computed results to the matching alternative element.
            foreach (var row in Rows)
            {
                if (((AlternativeElement)row.ChildElement).ID == results.AlternativeID)
                {
                    ((AlternativeElement)row.ChildElement).Results = results;
                }
            }

        }

    }
}
