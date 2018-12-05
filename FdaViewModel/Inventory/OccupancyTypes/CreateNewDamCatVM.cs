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
        #endregion
        #region Properties
        
        
        #endregion
        #region Constructors
        public CreateNewDamCatVM(List<string> bannedNames):base()
        {
            AddValidationRules(bannedNames);
        }
        public CreateNewDamCatVM(string exampleName, List<string> bannedNames) : base()
        {
            Name = exampleName;
            AddValidationRules(bannedNames);
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
       private void AddValidationRules(List<string> bannedNames)
        {
            AddRule(nameof(Name), () => { if (Name == null) { return false; } else { return !Name.Equals(""); } }, "Name cannot be blank");

            foreach (string bannedName in bannedNames)
            {
                AddRule(nameof(Name), () => {
                    if (bannedName.Equals(Name))
                    {
                        return false;
                    }
                    else { return true; }

                }, "Name already exists.");
            }
        }

       
    }
}
