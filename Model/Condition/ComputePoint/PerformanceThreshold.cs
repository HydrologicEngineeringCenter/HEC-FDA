using System;


namespace Model.ComputationPoint
{
    //[Author("John Kucharski", "10/11/2016")]
    public class PerformanceThreshold
    {
        #region Notes
        /* 1. This should be constructed after levees and otehr lateral structures are defined
         *    a. So, in the "levee view" it should ask if the top of levee should be set as the performance threshold.
         *    b. Or, if on selects a levee or other lateral structure threshold in the "theshold view" they should be directed to the "levee view" before saving their threshold values. 
         */
        #endregion

        #region Fields
        private PerformanceThresholdTypes _Type;
        private double _Threshold;
        #endregion

        #region Properties
        public PerformanceThresholdTypes ThresholdType
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }
        public double ThresholdValue
        {
            get
            {
                return _Threshold;
            }
            set
            {
                _Threshold = value;
            }
        }
        #endregion

        #region Constructor
        //[Tested(false)]
        /// <summary> This constructor is for damage based and non-lateral structure hieght based performance elevations. </summary>
        /// <param name="computationPoint"></param>
        /// <param name="thresholdType"></param>
        /// <param name="threshold"></param>
        public PerformanceThreshold(PerformanceThresholdTypes thresholdType, double threshold)
        {
            //if (thresholdType == PerformanceThresholdTypes.LeveeHeight)
            //{
            //    Logger.Instance.ReportMessage(new ErrorMessage("This constructor can only be used when the selected threshold is not based on a lateral structure."
            //                                                    , ErrorMessageEnum.Model & ErrorMessageEnum.Fatal));
            //}
            //else
            {
                ThresholdType = thresholdType;
                ThresholdValue = threshold;
            }
        }

        //[Tested(false)]
        /// <summary> This constructor is for levees and lateral structures only. </summary>
        /// <param name="computai"></param>
        /// <param name="lateralStructure"></param>
        public PerformanceThreshold(LateralStructure lateralStructure)
        {
            //if (lateralStructure.IsLevee == true)
            //{
            //    ThresholdType = PerformanceThresholdTypes.LeveeHeight;   
            //}
            //else
            {
                ThresholdType = PerformanceThresholdTypes.OtherExteriorStage;
            }
            ThresholdValue = lateralStructure.Elevation;
        }
        #endregion


    }
}
