using HEC.FDA.ViewModel.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    //[Author(q0heccdm, 7 / 21 / 2017 1:26:32 PM)]
    public class CreateNewDamCatVM : BaseEditorVM 
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
        public CreateNewDamCatVM(List<string> bannedNames):base(null)
        {
            AddValidationRules(bannedNames);
            SetDimensions(360, 120, 200, 70);
        }
        public CreateNewDamCatVM(string exampleName, List<string> bannedNames) : base(null)
        {
            Name = exampleName;
            AddValidationRules(bannedNames);
            SetDimensions(360, 120, 200, 70);
        }



        #endregion
        #region Voids
        public override void Save()
        {
            
        }
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
