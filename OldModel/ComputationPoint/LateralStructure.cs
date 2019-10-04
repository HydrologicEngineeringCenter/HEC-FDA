using System.Linq;
using System.Collections.Generic;
using FdaModel.Utilities.Attributes;
using FdaModel.Utilities.Messager;
using FdaModel.Functions;
using FdaModel.Functions.OrdinatesFunctions;

namespace FdaModel.ComputationPoint
{
    [Author("John Kucharski", "10/11/2016")]
    public class LateralStructure
    {
        #region Notes
        /* This class provides a damage truncation point which is necessary for levee projects and those with perched rivers. Levee fragility curves are stored as seperate (increasing ordinate function) objects. */
        #endregion

        #region Fields
        private bool _IsComputable;
        //private bool _IsLevee;                  //true = levee, false = bank
        private double _Elevation;
        #endregion
    
        #region Properties
        public bool IsComputable
        {
            get
            {
                return _IsComputable;
            }
            set
            {
                _IsComputable = value;
            }
        }
        //public bool IsLevee
        //{
        //    get
        //    {
        //        return _IsLevee;
        //    }
        //    set
        //    {
        //        _IsLevee = value;
        //    }
        //}
        public double Elevation
        {
            get
            {
                return _Elevation;
            }
            set
            {
                _Elevation = value;
            }
        }
        #endregion

        #region Constructor
        [Tested(false)]
        /// <summary> Constructor for structure with no fragility. </summary>
        /// <param name="isLevee"> True if the lateral structure is a levee, false otherwise. </param>
        /// <param name="elevation"> Top of lateral structure water surface elevation. </param>
        public LateralStructure( double elevation)
        {
            //IsLevee = isLevee;
            Elevation = elevation;
            IsComputable = true;
            PerformanceThreshold pt = new PerformanceThreshold(this);
        }
        #endregion

        #region Voids
        public bool Validate()
        {
            //if (FailureFunction.GetType() == typeof(IncreasingOrdinatesFunction))
            //{
            //    IncreasingOrdinatesFunction failureFunction = (IncreasingOrdinatesFunction)FailureFunction;
            //    for (int i = 0; i < failureFunction.Xs.Length; i++)
            //    {
            //        if (failureFunction.Xs[i] > Elevation)
            //        {
            //            double dx = (Elevation - failureFunction.Xs[i - 1]) / (failureFunction.Xs[i] - failureFunction.Xs[i - 1]);
            //            double probability = failureFunction.Ys[i - 1] + dx * (failureFunction.Ys[i] - failureFunction.Ys[i - 1]);
            //            if (probability < 1)
            //            {
            //                Logger.Instance.ReportMessage(new ErrorMessage("The probability of failure at the top of the levee is estimated to be " + probability + ", " +
            //                                                               "by convention the probability of failure above the top of the levee must equal 1." +
            //                                                               "Consequently, the lateral structure cannot be use in the compute."
            //                                                               , ErrorMessageEnum.Model & ErrorMessageEnum.Major));
            //                return false;
            //            }
            //            else
            //            {
            //                return true;
            //            }
            //        }
            //        else if (i == failureFunction.Xs.Length - 1)
            //        {
            //            if (failureFunction.Ys[i] < 1)
            //            {
            //                Logger.Instance.ReportMessage(new ErrorMessage("The lateral structure fragility function does not contain a maximum (e.g. 100%) failure point, " +
            //                                                               "by convention the probability of failure above the top of levee must equal 1. " +
            //                                                               "A maximum failure point (e.g. [" + Elevation + " , 1]) must be added to the fragility function, before it can be used in the compute."
            //                                                               , ErrorMessageEnum.Model & ErrorMessageEnum.Major));
            //                return false;
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    Logger.Instance.ReportMessage(new ErrorMessage("The lateral structure fragility function ordiantes are not monotonically increasing. " +
            //                                                   "Consequently, the lateral structure cannot be used in the compute."
            //                                                   , ErrorMessageEnum.Model & ErrorMessageEnum.Major));
            //    return false;
            //}
            return true;
        }

        [Tested(true,true, @"ComputationPoint\LateralStructure\CreateInteriorExteriorFunction.xlsx","5/18/17","Cody McCoy")]
        public OrdinatesFunction CreateInteriorExteriorFunction(BaseFunction exteriorStageFunction, bool hasFailureFunction)
        {

            List<double> exteriorStages = (exteriorStageFunction.GetOrdinatesFunction( )).Function.YValues.Cast<double>().ToList();
            List<double> interiorStages = new List<double>();
            foreach( float stage in exteriorStages)
            {
                interiorStages.Add(stage);
            }

          


            if (hasFailureFunction == false)
            {
                for(int i = 0; i < exteriorStages.Count - 1; i++)
                {
                    if(exteriorStages[i] < Elevation)
                    {
                        interiorStages[i] = 0;
                        if(exteriorStages[i + 1] >= Elevation)
                        {
                            //exteriorStages.Insert(i + 1, (float) Elevation);
                            //interiorStages[i + 1] = exteriorStages[i + 1];
                            if(exteriorStages[i+1] == Elevation)
                            {
                                exteriorStages.Insert(i + 2, (float)Elevation + .0001f);
                                interiorStages.Insert(i + 2, (float)Elevation + .0001f);
                            }
                            else
                            {
                                exteriorStages.Insert(i + 1, (float)Elevation + .0001f);
                                interiorStages.Insert(i + 1, (float)Elevation + .0001f);
                                exteriorStages.Insert(i + 1, (float)Elevation);
                                interiorStages.Insert(i + 1, 0f);

                            }
                        }
                    }
                    else if(exteriorStages[i] == Elevation)
                    {
                        interiorStages[i] = 0;
                    }
                }
            }
            else
            {
                interiorStages = exteriorStages;
            }
            return new OrdinatesFunction(exteriorStages.ToArray( ), interiorStages.ToArray( ), FunctionTypes.ExteriorInteriorStage);
        }
        #endregion
    }
}
