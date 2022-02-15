using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.Inventory
{
    //[Author("q0heccdm", "10 / 20 / 2016 3:51:13 PM")]
    public class ImportFromHAZUSVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/20/2016 3:51:13 PM
        #endregion
        #region Fields
        private string _Name;
        private string _StudyAreaPath;
        private ObservableCollection<string> _StudyAreaPathList;
        private string _BndryGbsPath;
        private string _FlVehPath;
        private string _MSHPath;

        #endregion
        #region Properties
        public ObservableCollection<string> StudyAreaPathList
        {
            get { return _StudyAreaPathList; }
            set { _StudyAreaPathList = value; NotifyPropertyChanged(); }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }
        public string MSHPath
        {
            get { return _MSHPath; }
            set { _MSHPath = value; NotifyPropertyChanged(); }
        }
        public string FlVehPath
        {
            get { return _FlVehPath; }
            set { _FlVehPath = value; NotifyPropertyChanged(); }
        }
        public string BndryGbsPath
        {
            get { return _BndryGbsPath; }
            set { _BndryGbsPath = value; NotifyPropertyChanged(); }
        }
        public string StudyAreaPath
        {
            get { return _StudyAreaPath; }
            set { _StudyAreaPath = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public ImportFromHAZUSVM():base()
        {
            Name = "Example";
            StudyAreaPathList = new ObservableCollection<string> { "test", "cody" };
        }
        #endregion
        #region Voids
        #endregion
        #region Functions

        #endregion
        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => !(Name == ""), "The Name cannot be blank.");
            AddRule(nameof(Name), () => !(Name == null), "The Name cannot be blank.");

        }

      
    }
}
