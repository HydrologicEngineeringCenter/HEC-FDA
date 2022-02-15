using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Hydraulics
{
    //[Author("q0heccdm", "10 / 21 / 2016 3:59:07 PM")]
    public class GridImporterVM:BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/21/2016 3:59:07 PM
        #endregion
        #region Fields
        private string _Name;
        private string _Path;
        private string _Description;
        private List<string> _Years;
        private List<Watershed.TerrainElement> _Terrains;
        private List<string> _AvailableGrids;
        #endregion
        #region Properties
        public List<string> AvailableGrids
        {
            get { return _AvailableGrids; }
            set { _AvailableGrids = value; NotifyPropertyChanged(); }
        }
        public List<Watershed.TerrainElement> Terrains
        {
            get { return _Terrains; }
            set { _Terrains = value; NotifyPropertyChanged(); }
        }
        public List<string> Years
        {
            get { return _Years; }
            set { _Years = value; NotifyPropertyChanged(); }
        }
        public string Path
        {
            get { return _Path; }
            set { _Path = value; NotifyPropertyChanged(); LoadAvailableGrids(); }
        }

     

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


        #endregion
        #region Constructors
        public GridImporterVM():base()
        {
            Name = "Example";
        }
        //these years are coming from the analysis years
        public GridImporterVM(List<Watershed.TerrainElement> terrainList, Int32 baseYear, Int32 futureYear)
        {
            Name = "Loaded Example";
            Years = new List<string>();
            Years.Add(baseYear.ToString());
            if (futureYear != 0) Years.Add(futureYear.ToString());
            Terrains = terrainList;
            Description = "This is my description";
        }

        #endregion
        #region Voids
        private void LoadAvailableGrids()
        {
            List<string> tifFiles = new List<string>();
            List<string> fltFiles = new List<string>();
            //List<string>
        }
        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => { if (Name == null) { return false; } else { return !Name.Equals(""); } }, "Name cannot be blank");


        }

       
        #endregion
        #region Functions
        #endregion
    }
}
