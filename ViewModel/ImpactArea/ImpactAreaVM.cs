namespace HEC.FDA.ViewModel.ImpactArea
{
    //[Author("q0heccdm", "10 / 13 / 2016 10:41:09 AM")]
    public class ImpactAreaVM:BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/13/2016 10:41:09 AM
        #endregion
        #region Fields
        private string _Name;
        private double _IndexPoint;
        private int _BankValue;
        public enum Bank { left=0, right=1, both=2 }; 
        
        #endregion
        #region Properties
        public int BankValue
        {
            get { return _BankValue; }
            set { _BankValue = value;  NotifyPropertyChanged(); }
        }
        //public int LeftBank
        //{
        //    get { return _BankValue; }
        //    set { _BankValue = value; }
        //}
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public double IndexPoint
        {
            get { return _IndexPoint; }
            set { _IndexPoint = value; }
        }
        #endregion
        #region Constructors
        public ImpactAreaVM(string name, double indexPoint, int bank)
        {
            _Name = name;
            _IndexPoint = indexPoint;
            _BankValue = bank;
        }

        public override void AddValidationRules()
        {
            //AddRule(nameof(IndexPoint), () => IndexPoint >= 0, "Index Point must be greater than or equal to zero.");
            //AddRule(nameof(Name), () => Name == "", "Name cannot be empty.");
            //AddRule(nameof(Name), () => Name == null, "Name cannot be empty.");

        }

    
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
    }
}
