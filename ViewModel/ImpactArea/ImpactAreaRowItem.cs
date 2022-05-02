using HEC.FDA.ViewModel.Editors;

namespace HEC.FDA.ViewModel.ImpactArea
{
    public class ImpactAreaRowItem : NameValidatingVM
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/27/2016 9:27:23 AM
        #endregion

        #region Properties
        public int ID { get; set; }

        #endregion
        #region Constructors
 
        public ImpactAreaRowItem(int id, string dispName) 
        {
            ID = id;
            Name = dispName;
        }
        #endregion
    }
}
