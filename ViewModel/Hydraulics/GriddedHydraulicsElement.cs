using System;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Hydraulics
{
    class GriddedHydraulicsElement : Utilities.ChildElement
    {
        #region Notes
        #endregion
        #region Fields
        private const string _TableConstant = "Gridded Water Surface Profile - ";

        private string _Name;
        private string _Path;
        private string _Description;
        private List<string> _Years;
        private Watershed.TerrainElement _Terrain;
        private List<string> _AvailableGrids;
        #endregion
        #region Properties
      
        public List<string> AvailableGrids
        {
            get { return _AvailableGrids; }
            set { _AvailableGrids = value; NotifyPropertyChanged(); }
        }
        public Watershed.TerrainElement Terrains
        {
            get { return _Terrain; }
            set { _Terrain = value; NotifyPropertyChanged(); }
        }
        public List<string> Years
        {
            get { return _Years; }
            set { _Years = value; NotifyPropertyChanged(); }
        }
        public string Path
        {
            get { return _Path; }
            set { _Path = value; NotifyPropertyChanged(); }
        }

        public string HydraulicsName
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
        public GriddedHydraulicsElement( GridImporterVM vm) : base()
        {
            Name = vm.Name;
            HydraulicsName = vm.Name;
            Path = vm.Path;
            Description = vm.Description;
            Years = vm.Years;
            Terrains = vm.Terrains[0];//needs to be the selected terrain...

            AvailableGrids = vm.AvailableGrids;

            Utilities.NamedAction properties = new Utilities.NamedAction();
            properties.Header = "Properties";
            properties.Action = ViewProperties;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(properties);
            Actions = localActions;

        }
        #endregion
        #region Voids
        private void ViewProperties(object arg1, EventArgs arg2)
        {
            //PropertiesVM propVM = new PropertiesVM(HydraulicsName, Description, Path, _Terrain);
            //Navigate(propVM, true, true);
        }
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }



        //public override object[] RowData()
        //{
        //    return new object[] { Name };
        //}



        #endregion
        #region Functions
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            return null;
        }
        #endregion

    }
}
