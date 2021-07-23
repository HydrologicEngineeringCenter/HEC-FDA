using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Inventory
{
    //[Author("q0heccdm", "10 / 21 / 2016 9:09:52 AM")]
    public class ImportFromNSIVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/21/2016 9:09:52 AM
        #endregion
        #region Fields
        private string _Name;
        private string _StudyAreaPath;
        private string _UserDefinedPath;
        private bool _IsRadFromWebChecked = true;
        #endregion
        #region Properties
        public bool IsRadFromWebChecked
        {
            get { return _IsRadFromWebChecked; }
            set { _IsRadFromWebChecked = value; NotifyPropertyChanged(); }
        }
        public string UserDefinedPath
        {
            get { return _UserDefinedPath; }
            set { _UserDefinedPath = value; NotifyPropertyChanged(); }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }
        public string StudyAreaPath
        {
            get { return _StudyAreaPath; }
            set { _StudyAreaPath = value; NotifyPropertyChanged(); }
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
            AddRule(nameof(Name), () => !(Name == ""), "The Name cannot be blank.");
            AddRule(nameof(Name), () => !(Name == null), "The Name cannot be blank.");
            AddRule(nameof(StudyAreaPath), () => !(StudyAreaPath == null || StudyAreaPath == ""), "You must select a study area shapefile.");
            AddRule(nameof(IsRadFromWebChecked), () => !(IsRadFromWebChecked == false && (UserDefinedPath == null || UserDefinedPath == "")), "You must select a user defined shapefile.");

        }

      
    }
}
