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
        public LeveeFeatureEditorVM(Action<BaseViewModel> ownerValidationRules) :base()
        {
            ownerValidationRules(this);

        }
        public LeveeFeatureEditorVM(string name, string description, double elevation, Action<BaseViewModel> ownerValidationRules):base()
        {
            ownerValidationRules(this);

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
            AddRule(nameof(Name), () => Name != null, "Name cannot be null.");
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }
    }
}
