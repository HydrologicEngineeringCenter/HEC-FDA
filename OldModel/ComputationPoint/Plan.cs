using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaModel.ComputationPoint.Outputs;
using FdaModel.Utilities.Messager;

namespace FdaModel.ComputationPoint
{
    public class Plan
    {
        //Validate taht it is the same location across differnt points in time.

        #region Fields
        private string _PlanName;
        private string _Description;
        private List<Result> _ComputedConditions;
        #endregion

        #region Properties
        public string PlanName
        {
            get
            {
                return _PlanName;
            }
            set
            {
                _PlanName = value;
            }
        }
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }
        public List<Result> ComputedConditions
        {
            get
            {
                return _ComputedConditions;
            }
            set
            {
                _ComputedConditions = value;
            }
        }
        #endregion

        #region Constructor
        public Plan(string plan, string description, List<Result> computedConditions) 
        {
            PlanName = plan;
            Description = description;
            ComputedConditions = computedConditions;
        }
        #endregion

        public static Plan PlanFactory(string plan, string description, List<Condition> conditions, int maxRealizationPerResult, int randomSeed = 0, bool isLegacyCompute = false, bool isPerformanceOnlyCompute = false)
        {
            Condition previousCondition = conditions[0];
            List<Result> computedConditions = new List<Result>();
            for(int i = 0; i < conditions.Count; i++)
            {
                if(conditions[i].Location == previousCondition.Location &&
                   conditions[i].Threshold == previousCondition.Threshold)
                {
                    computedConditions.Add(new Result(conditions[i], maxRealizationPerResult, randomSeed, isLegacyCompute, isPerformanceOnlyCompute));
                }
                else
                {
                    ModelErrors fatalError = new ModelErrors(new ErrorMessage("A plan consists of one more conditions with matching locations and thresholds. One or more of the location and/or threshold conditions do not match. Therefore a plan cannot be computed.", ErrorMessageEnum.Fatal));
                    fatalError.ReportMessages( );
                    return null;
                }
            }
            return new Plan(plan, description, computedConditions);
        }
    }
}
