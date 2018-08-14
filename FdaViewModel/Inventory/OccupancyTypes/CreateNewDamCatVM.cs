using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    //[Author(q0heccdm, 7 / 21 / 2017 1:26:32 PM)]
    public class CreateNewDamCatVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 7/21/2017 1:26:32 PM
        #endregion
        #region Fields
        private string _Name;
        #endregion
        #region Properties
        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }
        
        #endregion
        #region Constructors
        public CreateNewDamCatVM():base()
        {

        }
        public CreateNewDamCatVM(string name) : base()
        {
            Name = name;
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }
    }
}
