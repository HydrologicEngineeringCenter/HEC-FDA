using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
{
    //[Author(q0heccdm, 6 / 30 / 2017 11:51:38 AM)]
    public class CustomHeaderVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/30/2017 11:51:38 AM
        #endregion
        #region Fields
        private string _Name;
        private string _Decoration;
        private string _ImageSource;
        #endregion
        #region Properties
        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }
        public string Decoration
        {
            get { return _Decoration; }
            set { _Decoration = value;  NotifyPropertyChanged(); }
        }

        public string ImageSource
        {
            get { return _ImageSource; }
            set { _ImageSource = value;NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
           public CustomHeaderVM(string name, string imageSource="", string decoration = "")
        {
            

            Name = name;
            Decoration = decoration;
            ImageSource = imageSource; 
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
            throw new NotImplementedException();
        }
    }
}
