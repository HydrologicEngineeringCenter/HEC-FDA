using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.ImpactArea
{
    //[Author("q0heccdm", "10 / 13 / 2016 10:05:26 AM")]
    public class ImpactAreaEditorVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/13/2016 10:05:26 AM
        #endregion
        #region Fields
        private List<ImpactAreaRowItem> _IAList;

        #endregion
        #region Properties
       
        public List<ImpactAreaRowItem> IAList
        {
            get{ return _IAList;}
            set
            {
                _IAList = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        #region Constructors
        public ImpactAreaEditorVM(List<ImpactAreaRowItem> ialist)
        {
            IAList = ialist;
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            //AddRule(nameof(Name), () => { if (Name == null) { return false; } else { return !Name.Equals(""); } }, "Name cannot be blank");

        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }
    }
}
