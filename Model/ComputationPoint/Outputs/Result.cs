using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaModel.Functions;
using FdaModel.Functions.OrdinatesFunctions;
using FdaModel.Utilities.Messager;


namespace FdaModel.ComputationPoint.Outputs
{
    public class Result
    {
        #region Fields
        private Random _RandomNumberGenerator;
        private List<Realization> _Realizations;
        private Statistics.Histogram _AepResult;
        private Statistics.Histogram _EadResult;
        private int _NumberOfRealizations;
        #endregion

        #region Properties
        public Random RandomNumberGenerator
        {
            get
            {
                return _RandomNumberGenerator;
            }
            set
            {
                _RandomNumberGenerator = value;
            }
        }
        
        public List<Realization> Realizations
        {
            get
            {
                return _Realizations;
            }
            private set
            {
                _Realizations = value;
            }
        }

        public Statistics.Histogram AEP
        {
            get
            {
                return _AepResult;
            }
            set
            {
                _AepResult = value;
            }
        }
        public Statistics.Histogram EAD
        {
            get
            {
                return _EadResult;
            }
            set
            {
                _EadResult = value;
            }
        }
        #endregion



        #region Constructor
        public Result(Condition condition, int nRealizations, int seed = 0, bool isLegacyCompute = false, bool isPerformanceOnlyCompute = false)
        {
            _NumberOfRealizations = nRealizations;
            RandomNumberGenerator = new Random(seed);
            Realizations = new List<Realization>();

            double maxAEP = 0;
            double minAEP = 1;
            for (int k = 0; k < 100; k++)
            {
                Realization realization = new Realization(condition, RandomNumberGenerator, isLegacyCompute, isPerformanceOnlyCompute);
                if (realization.AnnualExceedanceProbability > maxAEP)
                {
                    maxAEP = realization.AnnualExceedanceProbability;
                }
                if (realization.AnnualExceedanceProbability < minAEP)
                {
                    minAEP = realization.AnnualExceedanceProbability;
                }

            }


            AEP = new Statistics.Histogram(40, minAEP,maxAEP, false);
            if (isPerformanceOnlyCompute == true)
            {
                Parallel.For(0, nRealizations + 1, (i, ParallelLoopState) =>
                {
                    Realization realization = new Realization(condition, RandomNumberGenerator, isLegacyCompute, isPerformanceOnlyCompute);
                    if (double.IsNaN(realization.AnnualExceedanceProbability) == false)
                    {
                        Realizations.Add(realization);
                        AEP.AddObservation(realization.AnnualExceedanceProbability);
                        if (AEP.IsConverged == true)
                        {
                            //_NumberOfRealizations = i; // is this right? cody is trying to keep track of the number of realizations so that i can divide by them to turn my histogram plot y-axis into a frequency instead of total count
                            ParallelLoopState.Stop();
                        }
                    }  
                });
            }
            else
            {
                double maxDamage = 0;
                double minDamage = double.MaxValue;
                for(int k = 0; k<100; k++)
                {
                    Realization realization = new Realization(condition, RandomNumberGenerator, isLegacyCompute, isPerformanceOnlyCompute);
                    if(realization.ExpectedAnnualDamage > maxDamage)
                    {
                        maxDamage = realization.ExpectedAnnualDamage;
                    }
                    if(realization.ExpectedAnnualDamage<minDamage)
                    {
                        minDamage = realization.ExpectedAnnualDamage;
                    }

                }
                //for (int i = condition.Functions.Count - 1; i > 0; i--)
                //{
                //    if (condition.Functions[i].FunctionType == FunctionTypes.DamageFrequency || condition.Functions[i].FunctionType == FunctionTypes.InteriorStageDamage)
                //    {
                //        OrdinatesFunction damageFunction = condition.Functions[i].GetOrdinatesFunction();
                //        maxDamage = damageFunction.Ys[damageFunction.Ys.Length - 1];
                //    }
                //}

                if (maxDamage == 0)
                {
                    
                   // throw new Exception("A damage function was not found for an EAD compute.");
                }
                else
                {
                    EAD = new Statistics.Histogram(40, minDamage, maxDamage, false);
                    Parallel.For(0, nRealizations + 1, (i, ParallelLoopState) =>
                    {
                        Realization realization = new Realization(condition, RandomNumberGenerator, isLegacyCompute, isPerformanceOnlyCompute);
                        if (double.IsNaN(realization.AnnualExceedanceProbability) == false && double.IsNaN(realization.ExpectedAnnualDamage) == false)
                        {
                            Realizations.Add(realization);
                            AEP.AddObservation(realization.AnnualExceedanceProbability);
                            EAD.AddObservation(realization.ExpectedAnnualDamage);

                            if (AEP.IsConverged == true && EAD.IsConverged == true)
                            {
                                ParallelLoopState.Stop();
                            }
                        } 
                    });
                }
            }
        }
        #endregion
    }
}
