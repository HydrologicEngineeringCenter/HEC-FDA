using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.Output
{
    //[Author("q0heccdm", "10 / 21 / 2016 11:30:29 AM")]
    public class CreateEconImportFileVM:BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/21/2016 11:30:29 AM
        #endregion
        #region Fields
        private string _Name;
        private string _Description;
        private List<string> _DamageReaches;
        private List<string> _Inventories;
        private List<string> _AvailablePlans;
        #endregion
        #region Properties
        public List<string> Inventories
        {
            get { return _Inventories; }
            set { _Inventories = value; NotifyPropertyChanged(); }
        }
        public List<string> AvailablePlans
        {
            get { return _AvailablePlans; }
            set { _AvailablePlans = value; NotifyPropertyChanged(); }
        }
        public List<string> DamageReaches
        {
            get { return _DamageReaches; }
            set { _DamageReaches = value; NotifyPropertyChanged(); }
        }
        public string Name
        { get { return _Name; }
          set { _Name = value; NotifyPropertyChanged(); }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }

        #endregion
        #region Constructors
        public CreateEconImportFileVM():base()
        {

        }
        public CreateEconImportFileVM(List<string> plans, List<string> structInventory, List<string> impactArea)
        {
            AvailablePlans = plans;
            Inventories = structInventory;
            DamageReaches = impactArea;
            Description = "cody";
        }


        #endregion
        #region Voids
        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => !(Name == ""), "The Name cannot be blank.");
            AddRule(nameof(Name), () => !(Name == null), "The Name cannot be blank.");
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Functions
        #endregion
    }
}
