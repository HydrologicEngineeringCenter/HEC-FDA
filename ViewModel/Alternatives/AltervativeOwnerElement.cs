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

            NamedAction addPlan = new NamedAction();
            addPlan.Header = "Add Plan";
            addPlan.Action = AddNewPlan;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addPlan);
            //localActions.Add(ImportRatingCurve);

            Actions = localActions;

            StudyCache.PlanAdded += AddPlanElement;
            StudyCache.PlanRemoved += RemovePlanElement;
            StudyCache.PlanUpdated += UpdatePlanElement;

        }

        public void AddNewPlan(object arg1, EventArgs arg2)
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
            string header = "Create Plan";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "CreateNewPlan");
            Navigate(tab, false, true);
        }

        private void UpdatePlanElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void AddPlanElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemovePlanElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }

 
    }
}
