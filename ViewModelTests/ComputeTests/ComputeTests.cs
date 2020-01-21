using FdaViewModel.Conditions;
using FdaViewModel.ImpactArea;
using Model;
using Model.Inputs.Conditions;
using Model.Outputs;
using System;
using System.Collections.ObjectModel;
using Xunit;

namespace ViewModelTests.ComputeTests
{
    public class ComputeTests
    {
        private ImpactAreaElement CreateImpactArea()
        {
            ObservableCollection<ImpactAreaRowItem> rows = new ObservableCollection<ImpactAreaRowItem>();
            string name = "imp name";
            string desc = "imp desc";

            for(int i = 0;i<10;i++)
            {
                double indPoint = i;
                ImpactAreaRowItem row = new ImpactAreaRowItem(name, indPoint, new ObservableCollection<object>());
                rows.Add(row);

            }
            return new ImpactAreaElement(name, desc, rows);
        }

        private ImpactAreaRowItem CreateImpactAreaRowItem(double value)
        {
            return new ImpactAreaRowItem("name", value, new ObservableCollection<object>());
        }

        [Fact]
        public void ComputeTest()
        {
            string name = "name";
            string desc = "desc";
            int analysisYear = 1999;
            double thresholdValue = 5;

            ConditionBuilder builder = new ConditionBuilder(name, desc, analysisYear, CreateImpactArea(), CreateImpactAreaRowItem(2),
                Model.MetricEnum.InteriorStage, thresholdValue);

            ConditionsElement conditionElem = builder.build();

            ICondition condition = conditionElem.CreateCondition();
            int seed = 1;
            IRealization realization = condition.Compute(seed);
        }
    }
}
