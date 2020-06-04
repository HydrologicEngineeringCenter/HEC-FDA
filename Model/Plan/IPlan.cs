using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    
    /// <summary>
    /// An interface for the aggregation of conditions.
    /// </summary>
    public interface IPlan
    {
        /// <summary>
        /// The list of conditions describing the plan.
        /// </summary>
        List<ICondition> Alternative { get; }
        /// <summary>
        /// The list of conditions describing the counter-factual comparison plan
        /// </summary>
        List<ICondition> Counterfactual { get; }

        //todo: Add Cost
        //todo: Add Compute CBA
        //todo: Add compute Benefits
        //todo: Add discount rate
        //todo: Allow different years with interpolation
        //todo: Interpolator
    }
    //public interface ICondition
    //{
    //    int Year { get; }
    //    List<IImpactArea> ImpactAreas { get; }
    //    //after compute function....
    //    double ConditionEAD();
    //}
    //public interface IImpactArea
    //{
    //    IComputeArea ComputeArea { get; } 
    //    IComputePoint ComputePoint { get; }
    //}
    public interface IComputeArea 
    {        
        //todo: wsps
        //todo: inventory
    }
    public interface IComputePoint 
    {
        //indexpoint
    }




}
