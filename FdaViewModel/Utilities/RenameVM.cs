using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
{
    //[Author(q0heccdm, 11 / 22 / 2016 9:05:37 AM)]
    public class RenameVM:BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 11/22/2016 9:05:37 AM
        #endregion
        #region Fields
        private string _Name;
        #endregion
        #region Properties

        #endregion
        #region Constructors
        public RenameVM():base()
        { 
            
        }
        public RenameVM(string name, ChildElement element)
        {
            Name = name;
            StudyCache.AddSiblingRules(this, element);

        }

        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }

     
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
    }
}
