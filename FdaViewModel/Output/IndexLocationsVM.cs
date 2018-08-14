using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.Output
{
    //[Author("q0heccdm", "10 / 21 / 2016 3:26:13 PM")]
    public class IndexLocationsVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/21/2016 3:26:13 PM
        #endregion
        #region Fields
        private List<string> _ReachName;
        #endregion
        #region Properties
        public List<string> ReachName
        {
            get { return _ReachName; }
            set { _ReachName = value; }
        }
        #endregion
        #region Constructors
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
            throw new NotImplementedException();
        }
    }
}
