using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Inputs.Functions;

namespace Model.Inputs.Conditions
{
    internal class ConditionPerformanceCompute: IComputableCondition
    {
        #region Fields
        bool _CanCompute;
        #endregion

        #region Properties
        public ICondition ConditionBase { get; }
        public bool CanCompute
        {
            get
            {
                return IsValidCompute(); 
            }
            set
            {
                _CanCompute = value;
            }
        }
        
        #endregion

        #region Constructor
        public ConditionPerformanceCompute(ICondition condition, IList<IComputePoint> thresholds)
        {
            ConditionBase = condition;
            thresholds.OrderBy(i => i.ComputePointFunction);
            Thresholds = thresholds.Select(i => new Tuple<IComputePoint, double>(i, double.NaN)).ToList();
        }
        #endregion

        #region Methods
        public bool IsValidCompute()
        {
            //if (Thresholds.Count == 0 ||
            //    Thresholds[Thresholds.Count - 1].Item1.ThresholdFunction )
            throw new NotImplementedException();
        }
        public void TestCompute()
        {
            //ConditionBase.TestCompute();
            throw new NotImplementedException();
        }
        public void Compute(int seed)
        {
            int j = 0, J = Thresholds.Count - 1;
            Random numberGenerator = new Random(seed);
            for (int i = 0; i < ConditionBase.TransformFunctions.Count; i++)
            {
                ConditionBase.FrequencyFunctions.Add(ConditionBase.FrequencyFunctions[i].Compose(ConditionBase.TransformFunctions[i], numberGenerator.NextDouble(), numberGenerator.NextDouble()));
                if (ConditionBase.FrequencyFunctions[ConditionBase.FrequencyFunctions.Count - 1].Type == Thresholds[j].Item1.ThresholdFunction)
                {
                    ComputeMetric();
                    if (j == J) break;
                    else j++;
                }
            }
        }
        #endregion

        private void TrimTransformFunctionList()
        {
            for (int i = 0; i < ConditionBase.TransformFunctions.Count; i++) //(IFunctionTransform function in ConditionBase.TransformFunctions)
            {
                if (ConditionBase.TransformFunctions[i].Type > Thresholds[Thresholds.Count - 1].Item1.ThresholdFunction)
                    ConditionBase.TransformFunctions.RemoveAt(i);
            }
        }

    }
}
