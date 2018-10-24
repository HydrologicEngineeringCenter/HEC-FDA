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
    public class LeveeFeatureEditorVM : Editors.BaseEditorVM
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 1:20:23 PM
        #endregion
        #region Fields
      
        private double _Elevation = 0;
        #endregion
        #region Properties
       
        public double Elevation
        {
            get { return _Elevation; }
            set { _Elevation = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public LeveeFeatureEditorVM() :base(null)
        {

        }
        public LeveeFeatureEditorVM(LeveeFeatureElement element):base(element,null)
        {

            Name = element.Name;
            Description = element.Description;
            Elevation = element.Elevation;
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
            if (Description == null) { Description = ""; }
            LeveeFeatureElement elementToSave = new LeveeFeatureElement(Name,Description,Elevation);
            Saving.PersistenceManagers.LeveePersistenceManager manager = Saving.PersistenceFactory.GetLeveeManager(StudyCache);
            if (IsImporter && HasSaved == false)
            {
                manager.SaveNew(elementToSave);
                HasSaved = true;
                OriginalElement = elementToSave;
            }
            else
            {
                manager.SaveExisting((LeveeFeatureElement)OriginalElement, elementToSave, 0);
            }
        }

        //public override void Save()
        //{
        //    //throw new NotImplementedException();
        //}
    }
}
