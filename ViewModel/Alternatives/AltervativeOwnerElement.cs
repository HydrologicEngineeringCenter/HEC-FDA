using ViewModel.FlowTransforms;
using ViewModel.Utilities;
using Functions;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Alternatives
{
    public class AltervativeOwnerElement: ParentElement
    {
        public AltervativeOwnerElement():base()
        {
            Name = "Alternatives";
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addAlternative = new NamedAction();
            addAlternative.Header = "Add Alternative...";
            addAlternative.Action = AddNewAlternative;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addAlternative);
            //localActions.Add(ImportRatingCurve);

            Actions = localActions;

            StudyCache.AlternativeAdded += AddAlternativeElement;
            StudyCache.AlternativeRemoved += RemoveAlternativeElement;
            StudyCache.AlternativeUpdated += UpdateAlternativeElement;

        }

        public void AddNewAlternative(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);

            List<InflowOutflowElement> conditions = new List<InflowOutflowElement>();
            List<double> xs = new List<double>() { 1, 2 };
            List<double> ys = new List<double>() { 1, 2 };
            ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(xs, ys);
            IFdaFunction fdaFunction = IFdaFunctionFactory.Factory( IParameterEnum.InflowOutflow, (IFunction)func);
            for (int i = 0; i < 100; i++)
            {
                InflowOutflowElement elem = new InflowOutflowElement("Condition " + i, "", "", fdaFunction);
                conditions.Add(elem);
            }

            CreateNewAlternativeVM vm = new CreateNewAlternativeVM(conditions, actionManager);
            string header = "Create Alternative";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "CreateNewAlternative");
            Navigate(tab, false, true);
        }

        private void UpdateAlternativeElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void AddAlternativeElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveAlternativeElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }

 
    }
}
