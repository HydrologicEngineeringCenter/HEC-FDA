using System;

namespace HEC.FDA.ViewModel.Study
{
    //[Author("q0heccdm", "10 / 11 / 2016 4:08:19 PM")]

    public class AnalysisYearsVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/11/2016 4:08:19 PM
        #endregion
        #region Fields
        private int _BaseYear;
        private int _MostLikelyFuture;
        #endregion
        #region Properties
        public int BaseYear
        {
            get { return _BaseYear; }
            set { _BaseYear = value; NotifyPropertyChanged(); }
        }
        public int MostLikelyFuture
        {
            get { return _MostLikelyFuture; }
            set { _MostLikelyFuture = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public AnalysisYearsVM():base()
        {
            _BaseYear = 2015;
            _MostLikelyFuture = 2016;

        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {

            AddRule(nameof(BaseYear), () => BaseYear <= DateTime.Now.Year, "The Base Year must not be in the future.");
            AddRule(nameof(BaseYear), () => BaseYear >= 1900, "The Base Year must not be less than 1900.");
            AddRule(nameof(MostLikelyFuture), () => MostLikelyFuture >= BaseYear, "The Most Likely Future must happen after the Base Year.");
        }

      
    }
}
