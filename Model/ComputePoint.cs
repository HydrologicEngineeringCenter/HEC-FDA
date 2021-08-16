using System;
using System.Collections.Generic;
using Model.Inputs.Functions.ImpactAreaFunctions;

namespace Model.Inputs.Conditions
{
    //public class ComputePoint : IComputePoint
    //{
    //    #region Properties
    //    public double TargetValue { get; set; }
    //    public MetricEnum Unit { get; }
    //    public ImpactAreaFunctionEnum FunctionType { get; }
    //    public bool IsValid { get; } = false;
    //    #endregion

    //    #region Constructors
    //    public ComputePoint(MetricEnum type, double value)
    //    {
    //        TargetValue = value;
    //        Unit = type;
    //        FunctionType = AssignThresholdFunction();
    //        IsValid = Validate();
    //    }
    //    #endregion

    //    #region Methods
    //    private ImpactAreaFunctionEnum AssignThresholdFunction()
    //    {
    //        switch (Unit)
    //        {
    //            case MetricEnum.ExteriorStage:
    //                return ImpactAreaFunctionEnum.ExteriorStageFrequency;
    //            case MetricEnum.InteriorStage:
    //                return ImpactAreaFunctionEnum.InteriorStageFrequency;
    //            case MetricEnum.Damages:
    //                return ImpactAreaFunctionEnum.DamageFrequency;
    //            case MetricEnum.ExpectedAnnualDamage:
    //                return ImpactAreaFunctionEnum.DamageFrequency;
    //            default:
    //                return ImpactAreaFunctionEnum.NotSet;
    //        }
    //    }
    //    #endregion

    //    #region IValidateMethods
    //    public bool Validate()
    //    {
    //        if (Unit == MetricEnum.NotSet) { ReportValidationErrors(); return false; }
    //        else return true;
    //    }
    //    public IEnumerable<string> ReportValidationErrors()
    //    {
    //        IList<string> messages = new List<string>();
    //        if (Unit == MetricEnum.NotSet) messages.Add("The type of compute point (e.g. Exterior Stage, Interior Stage, Damage, Expected Annual Damages) must be set for the compute point to be used.");
    //        return messages;
    //    }
    //    #endregion
    //}
}
