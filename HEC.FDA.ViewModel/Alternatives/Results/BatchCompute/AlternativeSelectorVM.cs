﻿using HEC.CS.Collections;
using HEC.FDA.Model.alternatives;
using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.scenarios;
using HEC.FDA.ViewModel.Compute;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Results;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HEC.FDA.ViewModel.Alternatives.Results.BatchCompute
{
    public class AlternativeSelectorVM : ChildSelectorVM
    {

 




        public AlternativeSelectorVM():base()
        {

            //todo: validate alternatives

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
            //ValidateScenario(newRow);
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
                //ValidateScenario(foundRow);
            }
        }

        //private void LoadScenarios()
        //{
        //    List<AlternativeElement> elems = StudyCache.GetChildElementsOfType<AlternativeElement>();

        //    foreach (AlternativeElement elem in elems)
        //    {
        //        Rows.Add(new ComputeChildRowItem(elem));
        //    }
        //}

        public override void LoadChildElements()
        {
            List<AlternativeElement> elems = StudyCache.GetChildElementsOfType<AlternativeElement>();

            foreach (AlternativeElement elem in elems)
            {
                Rows.Add(new ComputeChildRowItem(elem));
            }
        }

     

       


        //todo: look at the alt comp report validation and logic for how to do this. 
        //look at AltCompReportElement DoAlternativesStillExist and getCanComputeResults
        public override async void Compute(List<ComputeChildRowItem> altRows)
        {
            MessageEventArgs beginComputeMessageArgs = new MessageEventArgs(new Message("Beginning Batch Compute"));
            ReportMessage(this, beginComputeMessageArgs);

            List<Task> taskList = new List<Task>();
            List<AlternativeElement> elementList = new List<AlternativeElement>();

            try
            {
                foreach (ComputeChildRowItem row in altRows)
                {
                    AlternativeElement elem = (AlternativeElement)row.ChildElement;
                    elementList.Add(elem);
                    FdaValidationResult canComputeVR = elem.RunPreComputeValidation();
                    if (canComputeVR.IsValid)
                    {
                        Alternative alt = new Alternative();
                        ComputeAlternativeVM vm = new ComputeAlternativeVM(elem, ComputeCompleted);
                        taskList.Add(Task.Run(()=> vm.RunAnnualizationCompute(alt, elem, ComputeCompleted)));
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
            var result = MessageBox.Show("Do you want to view summary results?", "Compute Finished", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                //todo: show the summary results UI
                AlternativeSummaryVM vm = new AlternativeSummaryVM(elementList);
                //AltCompReportResultsVM vm = new AltCompReportResultsVM(CreateResults());
                string header = "Alternative Summary Results";
                DynamicTabVM tab = new DynamicTabVM(header, vm, header);
                Navigate(tab, false, true);
            }
        }



        //private void UpdateIASElementTooltips(List<IASElement> elems)
        //{
        //    foreach (IASElement elem in elems)
        //    {
        //        IASTooltipHelper.UpdateTooltip(elem);
        //    }
        //}

        

        //private void ValidateScenarios()
        //{
        //    foreach (ComputeChildRowItem row in Rows)
        //    {
        //        ValidateScenario(row);
        //    }
        //}

        //private void ValidateScenario(ComputeChildRowItem row)
        //{
        //    IASElement elem = (IASElement)row.ChildElement;
        //    FdaValidationResult canComputeVR = elem.CanCompute();
        //    if (!canComputeVR.IsValid)
        //    {
        //        row.MarkInError(canComputeVR.ErrorMessage);
        //    }
        //    else
        //    {
        //        row.ClearErrorStatus();
        //    }
        //}

        private void ComputeCompleted(AlternativeResults results)
        {
            //i have to assign the results back onto the alt.
            
            foreach(ComputeChildRowItem row in Rows)
            {
                if(row.ChildElement.ID == results.AlternativeID)
                {
                    ((AlternativeElement)row.ChildElement).Results = results;
                }
            }

            //todo: do something here? Save? update progress bar?
            int test = 0;
            //Results = results;
            //Application.Current.Dispatcher.Invoke(
            //(Action)(() =>
            //{
            //    PersistenceFactory.GetIASManager().SaveExisting(this);
            //    MessageBoxResult messageBoxResult = MessageBox.Show("Compute completed. Would you like to view the results?", Name + " Compute Complete", MessageBoxButton.YesNo, MessageBoxImage.Information);
            //    if (messageBoxResult == MessageBoxResult.Yes)
            //    {
            //        ViewResults(this, new EventArgs());
            //    }
            //}));
        }

      

    }
}