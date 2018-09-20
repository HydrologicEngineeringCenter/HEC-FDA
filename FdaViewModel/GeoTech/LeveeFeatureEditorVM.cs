using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.GeoTech
{
    //[Author(q0heccdm, 6 / 8 / 2017 1:20:23 PM)]
    public class LeveeFeatureEditorVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 1:20:23 PM
        #endregion
        #region Fields
        private string _Name = "";
        private string _Description = "";
        private double _Elevation = 0;
        #endregion
        #region Properties
        //public bool IsInEditMode { get; set; }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }
        public double Elevation
        {
            get { return _Elevation; }
            set { _Elevation = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public LeveeFeatureEditorVM():base()
        {

        }
        public LeveeFeatureEditorVM(string name, string description, double elevation,bool isInEditMode = false):base()
        {
            Name = name;
            Description = description;
            Elevation = elevation;
            //IsInEditMode = isInEditMode;
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }
    }
}
