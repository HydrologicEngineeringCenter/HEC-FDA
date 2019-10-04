using System;
using System.Collections.Generic;
using FdaModel.Functions;
using FdaModel.Functions.OrdinatesFunctions;
using FdaModel.Functions.FrequencyFunctions;
using FdaModel.Utilities.Attributes;
using FdaModel.Utilities.Messager;

namespace FdaModel.ComputationPoint.Outputs
{
    [Author("John Kucharski", "10/13/2016")]
    public class Realization
    {
        #region Notes
        /* 1. Need to work on Random Number Generator 
           2. Need to validate list of functions before compute is generated.
        */
        #endregion


        #region Fields        
        private Condition _Condition;
        private List<BaseFunction> _Functions;
        private bool _IsPerformanceOnlyCompute;
        private bool _IsCurveBasedCompute;
        private double _AnnualExceedanceProbability = double.NaN;
        private double _ExpectedAnnualDamages = double.NaN;
        private ModelErrors _Messages;
        #endregion


        #region Properties
        public Condition Condition
        {
            get
            {
                return _Condition;
            }
            private set
            {
                _Condition = value;
            }
        }

        public List<BaseFunction> Functions
        {
            get
            {
                return _Functions;
            }
            private set
            {
                _Functions = value;
            }
        }

        public bool IsPerformanceOnlyCompute
        {
            get
            {
                return _IsPerformanceOnlyCompute;
            }
            private set
            {
                _IsPerformanceOnlyCompute = value;
            }
        }

        public bool IsLegacyCompute
        {
            get
            {
                return _IsCurveBasedCompute;
            }
            private set
            {
                _IsCurveBasedCompute = value;
            }
        }

        public double AnnualExceedanceProbability
        {
            get
            {
                return _AnnualExceedanceProbability;
            }
            private set
            {
                _AnnualExceedanceProbability = value;
            }
        }

        public double ExpectedAnnualDamage
        {
            get
            {
                return _ExpectedAnnualDamages;
            }
            private set
            {
                _ExpectedAnnualDamages = value;
            }
        }

        public ModelErrors Messages
        {
            get
            {
                return _Messages;
            }
            private set
            {
                _Messages = value;
            }
        }
        #endregion


        #region Constructor
        public Realization(Condition condition, bool isLegacyCompute = false, bool isPerformanceOnlyCompute = false)
        {
            Condition = condition;
            IsLegacyCompute = isLegacyCompute;
            IsPerformanceOnlyCompute = isPerformanceOnlyCompute;
            Functions = new List<BaseFunction>();
            Messages = new ModelErrors();
        }
        public Realization(Condition condition, Random randomNumberGenerator, bool isLegacyCompute = false, bool isPerformanceOnlyCompute = false)
        {
            Condition = condition;
            IsLegacyCompute = isLegacyCompute;
            IsPerformanceOnlyCompute = isPerformanceOnlyCompute;
            Functions = new List<BaseFunction>();
            Messages = new ModelErrors();
            Compute(randomNumberGenerator);
        }
        #endregion


        #region Voids
        public void Compute(Random randomNumberGenerator)
        {
            if (IsLegacyCompute == true)
            {
                ComputeLegacyRealizationList(randomNumberGenerator);
                ComputeLegacyBasedStatistics();
            }
            else
            {
                ComputeRealization(randomNumberGenerator);
            }
        }

        [Tested(true,true, @"M:\Kucharski\Public\Fda\2.0\Testing\Realization\ComputeLegacyRealizationList.xlsx", "3/28/2017","Cody McCoy")]
        private void ComputeLegacyRealizationList(Random randomNumberGenerator)
        {
            List<ErrorMessage> errors = new List<ErrorMessage>();

            if (Condition.Messages.IsValid == true)
            {
                for (int i = 0; i < Condition.Functions.Count; i++)
                {
                    Functions.Add(Condition.Functions[i].SampleFunction(randomNumberGenerator));
                    if (i > 0)
                    {
                        if (i < Condition.Functions.Count - 1)
                        {
                            Functions.Add(Functions[Functions.Count - 2].Compose((OrdinatesFunction)Functions[Functions.Count - 1], ref errors));
                            if (IsPerformanceOnlyCompute == true && Condition.PerformanceComputePoint == Functions[Functions.Count - 1].FunctionType)
                            {
                                if (Condition.Functions[Condition.Functions.Count-1].FunctionType == FunctionTypes.LeveeFailure)
                                { Functions.Add(Condition.Functions[Condition.Functions.Count - 1]); }
                                break;
                            }
                        }
                        else
                        {
                            if (Condition.Functions[i].FunctionType != FunctionTypes.LeveeFailure)
                            {
                                Functions.Add(Functions[Functions.Count - 2].Compose((OrdinatesFunction)Functions[Functions.Count - 1], ref errors));
                            }
                        }
                    }
                }
            }
            else
            {
                Condition.Validate();
                if (Condition.Messages.IsValid == true)
                {
                    ComputeLegacyRealizationList(randomNumberGenerator);
                }
                else
                {
                    errors.Add(new ErrorMessage("The condition is associated with one or more serious errors wich must be resolved before a compute can be intitiated.", ErrorMessageEnum.Fatal));
                }
            }
            Messages.AddMessages(errors);              
        }


        [Tested(true,true, @"Realization\ComputeLegacyBasedStatistics.xlsx","5/18/17","Cody McCoy")]
        private void ComputeLegacyBasedStatistics()
        {
            //The calculations could be done better if the exterior stage is a frequency function.
            if (Condition.Messages.IsValid == true)
            {
                List<ErrorMessage> errors = new List<ErrorMessage>();
                int i = 0;
                if (Condition.Functions[Condition.Functions.Count - 1].FunctionType == FunctionTypes.LeveeFailure)
                {
                    while (Functions[i].FunctionType < FunctionTypes.ExteriorStageFrequency)
                    {
                        i++;
                    }

                    while (i < Functions.Count - 1)
                    {
                        if (Functions[i].FunctionType == Condition.PerformanceComputePoint)
                        {
                            AnnualExceedanceProbability = Functions[i].GetOrdinatesFunction().WeightedAEP(Functions[Functions.Count - 1].GetOrdinatesFunction());
                            if (IsPerformanceOnlyCompute == true)
                            {
                                return;
                            }
                            else
                            {
                                if (Functions[i].FunctionType == FunctionTypes.DamageFrequency)
                                {
                                    ExpectedAnnualDamage = Functions[i].GetOrdinatesFunction().WeightedTrapizoidalRiemannSum(Functions[Functions.Count - 1].GetOrdinatesFunction());
                                    return;
                                }
                                else
                                {
                                    i++;
                                }
                            }
                        }
                        else if (Functions[i].FunctionType == FunctionTypes.DamageFrequency)
                        {
                            ExpectedAnnualDamage = Functions[i].GetOrdinatesFunction().WeightedTrapizoidalRiemannSum(Functions[Functions.Count - 1].GetOrdinatesFunction());
                            return;
                        }
                        else
                        {
                            switch (Functions[i].FunctionType)
                            {
                                case FunctionTypes.ExteriorInteriorStage:
                                    Functions.Add(TransformFailureFunction(Functions[i].GetOrdinatesFunction()));
                                    break;
                                case FunctionTypes.InteriorStageDamage:
                                    Functions.Add(TransformFailureFunction(Functions[i].GetOrdinatesFunction()));
                                    break;
                                default:
                                    break;
                            }
                            i++;
                        }
                    }


                }
                else
                {
                    while (Functions[i].FunctionType < Condition.PerformanceComputePoint)
                    {
                        i++;
                    }

                    if (Functions[i].FunctionType != Condition.PerformanceComputePoint)
                    {
                        throw new Exception("the performance compute point set in the conditon was not encountered in the realization's list of functions.");
                    }
                    else
                    {
                        AnnualExceedanceProbability = Functions[i].GetOrdinatesFunction().GetXfromY(Condition.Threshold.ThresholdValue, ref errors);
                    }

                    if (IsPerformanceOnlyCompute == false)
                    {
                        ExpectedAnnualDamage = Functions[Functions.Count - 1].GetOrdinatesFunction().TrapizoidalRiemannSum();
                        return;
                    }
                }

                Messages.AddMessages(errors);
            }
        }

        // the following is cody's version. it was created on 3/23/2017 John's original version is commented out directly below this
        [Tested(true,true, @"Realization\Compute Realization.xlsx","5/18/17","Cody McCoy")]
        private void ComputeRealization(Random randomNumberGenerator)
        {
            OrdinatesFunction leveeFailureFunction = null;

            List<ErrorMessage> errors = new List<ErrorMessage>();
            if (Condition.Messages.IsValid == true)
            {
                for (int i = 0; i < Condition.Functions.Count; i++)
                {
                    Functions.Add(Condition.Functions[i].SampleFunction(randomNumberGenerator));

                    
                    {
                        if (Functions[Functions.Count - 1].FunctionType == FunctionTypes.ExteriorInteriorStage &&
                            Condition.Functions[Condition.Functions.Count - 1].FunctionType == FunctionTypes.LeveeFailure)
                        {
                            leveeFailureFunction = ComputeLeveeFailurePoint(randomNumberGenerator, ref errors);
                        }
                        if (Functions.Count > 1)
                        {
                            OrdinatesFunction composedFunction = Functions[Functions.Count - 2].Compose((OrdinatesFunction)Functions[Functions.Count - 1], ref errors);
                            if (composedFunction == null)
                            {
                                errors.Add(new ErrorMessage("Composition of functions returned a null value. Exiting the compute.", ErrorMessageEnum.Fatal));
                                Messages.AddMessages(errors);
                                return;
                            }

                            Functions.Add(composedFunction);
                        }

                        if (Functions[Functions.Count - 1].FunctionType == Condition.PerformanceComputePoint)
                        {
                            AnnualExceedanceProbability = Functions[Functions.Count - 1].GetXfromY(Condition.Threshold.ThresholdValue, ref errors);

                            if (IsPerformanceOnlyCompute == true)
                            {
                                Messages.AddMessages(errors);
                                if (leveeFailureFunction != null)
                                {
                                    Functions.Add(leveeFailureFunction);
                                }
                                return;
                            }

                        }

                        if (Functions[Functions.Count - 1].FunctionType == FunctionTypes.DamageFrequency)
                        {
                            ExpectedAnnualDamage = Functions[Functions.Count - 1].GetOrdinatesFunction().TrapizoidalRiemannSum();
                            Messages.AddMessages(errors);
                            if (leveeFailureFunction != null)
                            {
                                Functions.Add(leveeFailureFunction);
                            }
                            return;
                        }

                    }
                    //errors.Add(new ErrorMessage("One or more compute statistics were unexpectedly not computed during the compute sequence.", ErrorMessageEnum.Fatal));
                    //Messages.AddMessages(errors);  
                }
            }
            else
            {
                Condition.Validate();
                if (Condition.Messages.IsValid == true)
                {
                    ComputeRealization(randomNumberGenerator);
                }
                else
                {
                    Messages.AddMessage(new ErrorMessage("One or more serious errors are associated with the condtion linked to this compute. These errors must be resolved before a compute can be performed.", ErrorMessageEnum.Fatal));
                }
            }

            if (leveeFailureFunction != null)
            {
                Functions.Add(leveeFailureFunction);
            }
        }

        //[Tested(false)]
        //private void ComputeRealization(Random randomNumberGenerator)
        //{
        //    OrdinatesFunction leveeFailureFunction = null;
     
        //    List<ErrorMessage> errors = new List<ErrorMessage>();
        //    if (Condition.Messages.IsValid == true)
        //    {
        //        for (int i = 0; i < Condition.Functions.Count; i++)
        //        {
        //            Functions.Add(Condition.Functions[i].SampleFunction(randomNumberGenerator));

        //            if (i == 0)
        //            {
        //                if (Functions[Functions.Count - 1].FunctionType == Condition.PerformanceComputePoint)
        //                {
        //                    AnnualExceedanceProbability = Functions[Functions.Count - 1].GetXfromY(Condition.Threshold.ThresholdValue, ref errors);
                            
        //                    if (IsPerformanceOnlyCompute == true)
        //                    {
        //                        Messages.AddMessages(errors);
        //                        return;
        //                    }
        //                    else
        //                    {
        //                        if (Functions[Functions.Count - 1].FunctionType == FunctionTypes.DamageFrequency)
        //                        {
        //                            ExpectedAnnualDamage = Functions[Functions.Count - 1].GetOrdinatesFunction().TrapizoidalRiemannSum();
        //                            Messages.AddMessages(errors);
        //                            return;
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (Functions[Functions.Count - 1].FunctionType == FunctionTypes.ExteriorInteriorStage &&
        //                    Condition.Functions[Condition.Functions.Count - 1].FunctionType == FunctionTypes.LeveeFailure)
        //                {
        //                    leveeFailureFunction = ComputeLeveeFailurePoint(randomNumberGenerator, ref errors);
        //                }

        //                OrdinatesFunction composedFunction = Functions[Functions.Count - 2].Compose((OrdinatesFunction)Functions[Functions.Count - 1], ref errors);
        //                if(composedFunction==null)
        //                {
        //                    errors.Add(new ErrorMessage("Composition of functions returned a null value. Exiting the compute.", ErrorMessageEnum.Fatal));
        //                    Messages.AddMessages(errors);
        //                    return;
        //                }
                        
        //                Functions.Add(composedFunction);

        //                if (Functions[Functions.Count - 1].FunctionType == Condition.PerformanceComputePoint)
        //                {
        //                    AnnualExceedanceProbability = Functions[Functions.Count - 1].GetXfromY(Condition.Threshold.ThresholdValue, ref errors);

        //                    if (IsPerformanceOnlyCompute == true)
        //                    {
        //                        Messages.AddMessages(errors);
        //                        if (leveeFailureFunction != null)
        //                        {
        //                            Functions.Add(leveeFailureFunction);
        //                        }
        //                        return;
        //                    }
                            
        //                }

        //                if (Functions[Functions.Count - 1].FunctionType == FunctionTypes.DamageFrequency)
        //                {
        //                    ExpectedAnnualDamage = Functions[Functions.Count - 1].GetOrdinatesFunction().TrapizoidalRiemannSum();
        //                    Messages.AddMessages(errors);
        //                    if (leveeFailureFunction != null)
        //                    {
        //                        Functions.Add(leveeFailureFunction);
        //                    }
        //                    return;
        //                }

        //            }
        //            //errors.Add(new ErrorMessage("One or more compute statistics were unexpectedly not computed during the compute sequence.", ErrorMessageEnum.Fatal));
        //            //Messages.AddMessages(errors);  
        //        }
        //    }
        //    else
        //    {
        //        Condition.Validate();
        //        if (Condition.Messages.IsValid == true)
        //        {
        //            ComputeRealization(randomNumberGenerator);
        //        }
        //        else
        //        {
        //            Messages.AddMessage(new ErrorMessage("One or more serious errors are associated with the condtion linked to this compute. These errors must be resolved before a compute can be performed.", ErrorMessageEnum.Fatal));
        //        }
        //    }

        //    if(leveeFailureFunction != null)
        //    {
        //        Functions.Add(leveeFailureFunction);
        //    }
        //}

            [Tested(true,true, @"Realization\ComputeLeveeFailurePoint.xlsx","5/18/17","Cody McCoy")]
        private OrdinatesFunction ComputeLeveeFailurePoint(Random randomNumberGenerator, ref List<ErrorMessage> errors)
        {
            
            OrdinatesFunction failureFunction = Condition.Functions[Condition.Functions.Count - 1].SampleFunction(randomNumberGenerator).GetOrdinatesFunction();
            double failurePoint = randomNumberGenerator.NextDouble();
            double failureStage = failureFunction.GetXfromY(failurePoint, ref errors);

            if (failureStage > Condition.LateralStructure.Elevation)
            {
                return failureFunction;
            }
            else
            {
                bool addedFailurePoint = false;
                double preFailureStage = failureStage - 0.00001;
                OrdinatesFunction exteriorInteriorFunction = Functions[Functions.Count - 1].GetOrdinatesFunction();
                List<double> newXs = new List<double>( exteriorInteriorFunction.Function.XValues);
                List<double> newYs = new List<double>(exteriorInteriorFunction.Function.YValues);

                int i = 0, j = 0;
                //1. advance exterior-interior function to portion of overlap with levee failure function.
                while (newXs[i] < failureFunction.Function.get_X(0))
                {
                    //The exterior-interior function stage is lower than the 0 probability failure stage on the exerior-interior function
                    i++;
                }

                //2. OR advance levee failure function to portion of overlap with exterior-interior function.
                while (newXs[0] > failureFunction.Function.get_X(j))
                {
                    //The levee failure function stage is lower than the lowest stage on the exterior-interior function
                    j++;
                }

                if (j != 0)
                {
                    j--;
                }

                //3. overlapping parts of the curve.
                while (newXs[i] >= failureFunction.Function.get_X(j))
                {
                    if (j + 1 == failureFunction.Function.Count)
                    {
                        //a. if last point is above breach point replace it with the exterior stage otherwise do nothing (leave existing interior stage).
                        if (failureStage < newXs[i])
                        {
                            newYs[i] = newXs[i];
                        }
                        i++;
                    }
                    else
                    {
                        double probability, dx;
                        if (newXs[i] < failureFunction.Function.get_X(j + 1))
                        {
                            //b. exterior-interior ordinate is between 2 levee failure ordinates
                            dx = (newXs[i] - failureFunction.Function.get_X(j)) / (failureFunction.Function.get_X(j + 1) - failureFunction.Function.get_X(j));
                            if (double.IsNaN(dx))
                            {
                                dx = 0;
                            }
                            probability = failureFunction.Function.get_Y(j) + dx * (failureFunction.Function.get_Y(j + 1) - failureFunction.Function.get_Y(j));
                            if (failurePoint <= probability)
                            {
                                // i. levee fails
                                if (addedFailurePoint == false)
                                {

                                    //add pre-failure point to interior-exterior function
                                    if (i == 0)
                                    {
                                        newXs.Insert(i, (float)preFailureStage);
                                        newYs.Insert(i, newYs[0]);
                                        errors.Add(new ErrorMessage("In this realization failure is assumed to occur at " + failureStage + " this stage is less than or equal to " + newXs[0] + ", the lowest exterior stage on the interior-exterior stage function. Any exterior stage below the failure stage will be associated with an interior stage of " + newYs[0] + " the minimum interior stage on the exterior interior stage function.", ErrorMessageEnum.Minor));
                                    }
                                    else
                                    {
                                        double dy;
                                        if (preFailureStage != newXs[i - 1])
                                        {
                                            dy = (preFailureStage - newXs[i - 1]) / (newXs[i] - newXs[i - 1]);
                                            if (double.IsNaN(dy))
                                            {
                                                dy = 0;
                                            }
                                            newXs.Insert(i, (float)preFailureStage);
                                            newYs.Insert(i, (float)(newYs[i - 1] + dy * (newYs[i] - newYs[i - 1])));
                                            i++;
                                        }
                                    }

                                    //add failure stage point to interior-exterior stage function
                                    if (failureStage == newXs[i])
                                    {
                                        newYs[i] = newXs[i];
                                    }
                                    else
                                    {
                                        newXs.Insert(i, (float)failureStage);
                                        newYs.Insert(i, (float)failureStage);
                                        // Write a test for i = 0
                                    }
                                    addedFailurePoint = true;

                                }
                                else
                                {
                                    newYs[i] = newXs[i];
                                }
                            }
                            // ii. levee holds make no change
                            i++;
                        }
                        else
                        {
                            //c. exterior stage is beyond range of 2 levee failure stage ordinates being tracked.
                            j++;
                        }
                    }

                    if (i == newXs.Count)
                    {
                        // d. reached end of interior exterior function.
                        break;
                    }
                }
                OrdinatesFunction editedExteriorInteriorStageFunction = new OrdinatesFunction(newXs.ToArray(), newYs.ToArray(), FunctionTypes.ExteriorInteriorStage);
                Functions[Functions.Count - 1] = editedExteriorInteriorStageFunction;
                return failureFunction;
            }
        }
        #endregion


        #region Functions

        [Tested(true,true, @"Realization\TransformFailureFunction.xlsx","5/18/17","Cody McCoy")]
        private OrdinatesFunction TransformFailureFunction(OrdinatesFunction transformFunction)
        {

            

            OrdinatesFunction failureFunction = Functions[Functions.Count - 1].GetOrdinatesFunction();

            //find out which function has the smallest number of xs. This will limit the number of points in the new int stage freq function.
            int numPoints = failureFunction.Function.Count;
            if (transformFunction.Function.Count < numPoints) { numPoints = transformFunction.Function.Count; }

            List<ErrorMessage> errors = new List<ErrorMessage>();
            double[] newYs = new double[numPoints];
            double[] newXs = new double[numPoints];

            

            double dx;
            int i = 0, j = 0, k = 0;
            while (k < numPoints && i<numPoints && j<numPoints)
            {
                if (failureFunction.Function.get_X(i) >= transformFunction.Function.get_X(j))
                {
                    if (failureFunction.Function.get_X(i) == transformFunction.Function.get_X(j))
                    {
                        newXs[k] = transformFunction.Function.get_Y(j);
                        newYs[k] = failureFunction.Function.get_Y(i);
                        k++;
                        i++;
                    }
                    else if (failureFunction.Function.get_X(i) < transformFunction.Function.get_X(j + 1))
                    {
                        dx = (failureFunction.Function.get_X(i) - transformFunction.Function.get_X(j)) / (transformFunction.Function.get_X(j + 1) - transformFunction.Function.get_X(j));
                        newXs[k] = (float)(transformFunction.Function.get_Y(j) + dx * (transformFunction.Function.get_Y(j + 1) - transformFunction.Function.get_Y(j)));
                        newYs[k] = failureFunction.Function.get_Y(i);
                        k++;
                        i++;
                        
                    }
                    else
                    {
                        if (j + 1 == transformFunction.Function.Count)
                        {
                            throw new Exception("Cannot truncate failure function range.");
                        }
                        else
                        {
                            j++;
                        }
                    }
                }
                else { i++; }
                //i++;
                //else
                //{
                //    i++;
                //    //if (j - 1 < 0)
                //    //{
                //    //    throw new Exception("Cannot truncate failure function range.");
                //    //}
                //}
            }
            //if the transform function was a 7, then flip the xs and the ys around
            //if(transformFunction.FunctionType == FunctionTypes.InteriorStageDamage)
            //{
            //    float[] tempXs = new float[numPoints];
            //    tempXs = newXs;
            //    newXs = newYs;
            //    newYs = tempXs;

            //}

            return new OrdinatesFunction(newXs, newYs, FunctionTypes.LeveeFailure);
        }

        #endregion
    }
}
