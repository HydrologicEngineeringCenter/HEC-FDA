using ViewModel.Conditions;
using ViewModel.Editors;
using ViewModel.FlowTransforms;
using Functions;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Alternatives
{
    public class CreateNewAlternativeVM : BaseEditorVM
    {
        public List<ConditionWrapper> Conditions { get; set; }

        public CreateNewAlternativeVM(List<InflowOutflowElement> conditions, EditorActionManager actionManager) : base(actionManager)
        {
            //It is actually kind of difficult to get the list of selected items. The way i am going to do this is to create a simple
            //object that holds a condition and a boolean, isSelected.
            //so add to the constructor the list of conditions, then use them to create the wrapper class and then add those to the
            //Conditions property.
            List<ConditionWrapper> condWrappers = new List<ConditionWrapper>();
            foreach(InflowOutflowElement cond in conditions)
            {
                ConditionWrapper condWrapper = new ConditionWrapper(cond);
                condWrappers.Add(condWrapper);
            }
            Conditions = condWrappers;
        }
        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != null, "Name cannot be empty.");
            AddRule(nameof(Name), () => Name != "", "Name cannot be empty.");
        }
        public override void Save()
        {
            //Create a plan with the selected conditions
            List<InflowOutflowElement> conditions = new List<InflowOutflowElement>();
            foreach(ConditionWrapper cond in Conditions)
            {
                if(cond.IsSelected)
                {
                    conditions.Add(cond.Condition);
                }
            }
        }

        public class ConditionWrapper
        {
            public bool IsSelected { get; set; }
            public string Name { get; set; }
            public InflowOutflowElement Condition { get; set; }
            public ConditionWrapper(InflowOutflowElement condition)
            {
                Condition = condition;
                Name = Condition.Name;
                IsSelected = false;
            }
        }


    }
}
