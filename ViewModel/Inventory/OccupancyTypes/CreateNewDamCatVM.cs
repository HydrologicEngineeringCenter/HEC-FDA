using HEC.FDA.ViewModel.Editors;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    //[Author(q0heccdm, 7 / 21 / 2017 1:26:32 PM)]
    public class CreateNewDamCatVM : BaseEditorVM 
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 7/21/2017 1:26:32 PM
        #endregion
        #region Constructors
        public CreateNewDamCatVM(List<string> bannedNames):base(null)
        {
            AddValidationRules(bannedNames);
        }
        public CreateNewDamCatVM(string exampleName, List<string> bannedNames) : base(null)
        {
            Name = exampleName;
            AddValidationRules(bannedNames);
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
