using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Watershed
{
    public class TerrainElement : ChildElement
    {

        #region Notes
        #endregion
        #region Fields
        private const string _TableConstant = "Terrain - ";
        private string _FileName;
        private int _featureHashCode;
        private const string TERRAIN_ICON = "pack://application:,,,/View;component/Resources/Terrain.png";
        #endregion
        #region Properties
        //public override string GetTableConstant()
        //{
        //    return _TableConstant;
        //}
        public OpenGLMapping.RasterFeatureNode NodeToAddBackToMapWindow
        {
            get;set;
        }

        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; NotifyPropertyChanged(nameof(FileName)); }
        }
        #endregion
        #region Constructors
        public TerrainElement(string name, string fileName, bool isTemporaryNode = false) : base()
        {
            //vrt and auxilary files?  hdf5?
                Name = name;
                _FileName = fileName;

            if (isTemporaryNode)
            {
                CustomTreeViewHeader = new CustomHeaderVM(Name, TERRAIN_ICON, " -Saving",true);
            }
            else
            {
                CustomTreeViewHeader = new CustomHeaderVM(Name, TERRAIN_ICON);

                NamedAction remove = new NamedAction();
                remove.Header = StringConstants.REMOVE_MENU;
                remove.Action = RemoveElement;

                NamedAction renameElement = new NamedAction(this);
                renameElement.Header = StringConstants.RENAME_MENU;
                renameElement.Action = Rename;

                NamedAction mapWindow = new NamedAction();
                mapWindow.Header = StringConstants.ADD_TO_MAP_WINDOW_MENU;
                mapWindow.Action = AddTerrainToMapWindow;

                List<NamedAction> localactions = new List<NamedAction>();
                localactions.Add(remove);
                localactions.Add(renameElement);
                localactions.Add(mapWindow);

                Actions = localactions;
            }
        }

        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            return new TerrainElement(elementToClone.Name, ((TerrainElement)elementToClone).FileName);
        }
        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetTerrainManager().Remove(this);
        }

        public override void RemoveElementFromMapWindow(object arg1, EventArgs arg2)
        {
            RemoveFromMapWindow(this, new RemoveMapFeatureEventArgs(_featureHashCode));
            foreach (NamedAction a in Actions)
            {
                if (a.Header.Equals(StringConstants.REMOVE_FROM_MAP_WINDOW_MENU))
                {
                    a.Header = StringConstants.ADD_TO_MAP_WINDOW_MENU;
                    a.Action = AddTerrainToMapWindow;
                }
            }
        }
        public void removedcallback(OpenGLMapping.FeatureNodeHeader node, bool includeSelected)
        {
            foreach (NamedAction a in Actions)
            {
                if (a.Header.Equals(StringConstants.REMOVE_FROM_MAP_WINDOW_MENU))
                {
                    a.Header = StringConstants.ADD_TO_MAP_WINDOW_MENU;
                    a.Action = AddTerrainToMapWindow;
                }
            }
        }
        private void AddTerrainToMapWindow(object arg1, EventArgs arg2)
        {

            //OpenGLMapping.MapRasters rfn = new OpenGLMapping.MapRasters() 
            string filePath = Storage.Connection.Instance.GetTerrainFile(Name);
            if(filePath == null) { return; }
            LifeSimGIS.RasterFeatures r = new LifeSimGIS.RasterFeatures(filePath);

            OpenGLMapping.ColorRamp c = new OpenGLMapping.ColorRamp(OpenGLMapping.ColorRamp.RampType.Terrain, r.GridReader.Max, r.GridReader.Min, r.GridReader.Mean, r.GridReader.StdDev);
            AddGriddedDataEventArgs args = new AddGriddedDataEventArgs(r, c);
            args.FeatureName = Name;
            AddToMapWindow(this, args);
            _featureHashCode = args.MapFeatureHash;
            foreach (NamedAction a in Actions)
            {
                if (a.Header.Equals(StringConstants.ADD_TO_MAP_WINDOW_MENU))
                {
                    a.Header = StringConstants.REMOVE_FROM_MAP_WINDOW_MENU;
                    a.Action = RemoveElementFromMapWindow;
                }
            }
        }

        public string GetTerrainPath()
        {
            return Storage.Connection.Instance.TerrainDirectory + "\\" + Name + ".tif";
            //return  FileName;

        }

        //public override void Remove(object arg1, EventArgs arg2)
        //{
        //    if (_Owner.GetType().BaseType == typeof(OwnerElement))
        //    {
        //        OwnerElement o = (OwnerElement)_Owner;
        //        o.Elements.Remove(this);
        //        //delete the terrain file.
        //        //System.IO.File.Delete(FilePath);
        //    }
        //    else
        //    {
        //        //not good...
        //    }
        //}

        //public override string TableName
        //{
        //    get
        //    {
        //        throw new NotSupportedException("There is no terrain table. look for a Terrains table"); // these are not the droids you are looking for...
        //    }
        //}
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
       

        //public override void Save()
        //{
        //    //throw new NotImplementedException();
        //}

        //public override object[] RowData()
        //{
        //    return new object[] { Name, FileName };
        //}
        //public override bool SavesToRow()
        //{
        //    return true;
        //}
        //public override bool SavesToTable()
        //{
        //    return false;
        //}
    }
}
