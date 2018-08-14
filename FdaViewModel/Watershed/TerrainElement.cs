using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Watershed
{
    public class TerrainElement : Utilities.OwnedElement
    {

        #region Notes
        #endregion
        #region Fields
        private const string _TableConstant = "Terrain - ";
        private string _FileName;
        private int _featureHashCode;
        #endregion
        #region Properties
        public override string GetTableConstant()
        {
            return _TableConstant;
        }
        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; NotifyPropertyChanged(nameof(FileName)); }
        }
        #endregion
        #region Constructors
        public TerrainElement(string name, string fileName, TerrainOwnerElement owner) : base(owner)
        {
            //vrt and auxilary files?  hdf5?
            Name = name; //System.IO.Path.GetFileNameWithoutExtension(filepath);
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/Terrain.png");

            _FileName = fileName;

            //actions? properties? add to map window?
            Utilities.NamedAction remove = new Utilities.NamedAction();
            remove.Header = "Remove Terrain";
            remove.Action = Remove;

            Utilities.NamedAction renameElement = new Utilities.NamedAction();
            renameElement.Header = "Rename";
            renameElement.Action = Rename;



            Utilities.NamedAction mapWindow = new Utilities.NamedAction();
            mapWindow.Header = "Add to Map Window";
            mapWindow.Action = AddTerrainToMapWindow;

            List<Utilities.NamedAction> localactions = new List<Utilities.NamedAction>();
            localactions.Add(remove);
            localactions.Add(renameElement);
            localactions.Add(mapWindow);

            Actions = localactions;
        }

        private void RemoveTerrainFromMapWindow(object arg1, EventArgs arg2)
        {
            RemoveFromMapWindow(this, new Utilities.RemoveMapFeatureEventArgs(_featureHashCode));
            foreach (Utilities.NamedAction a in Actions)
            {
                if (a.Header.Equals("Remove from Map Window"))
                {
                    a.Header = "Add to Map Window";
                    a.Action = AddTerrainToMapWindow;
                }
            }
        }
        public void removedcallback(OpenGLMapping.FeatureNodeHeader node, bool includeSelected)
        {
            foreach (Utilities.NamedAction a in Actions)
            {
                if (a.Header.Equals("Remove from Map Window"))
                {
                    a.Header = "Add to Map Window";
                    a.Action = AddTerrainToMapWindow;
                }
            }
        }
        private void AddTerrainToMapWindow(object arg1, EventArgs arg2)
        {

            //OpenGLMapping.MapRasters rfn = new OpenGLMapping.MapRasters() 
            LifeSimGIS.RasterFeatures r = new LifeSimGIS.RasterFeatures(GetTerrainPath());

            OpenGLMapping.ColorRamp c = new OpenGLMapping.ColorRamp(OpenGLMapping.ColorRamp.RampType.Terrain, r.GridReader.Max, r.GridReader.Min, r.GridReader.Mean, r.GridReader.StdDev);
            Utilities.AddGriddedDataEventArgs args = new Utilities.AddGriddedDataEventArgs(r, c);
            args.FeatureName = Name;
            AddToMapWindow(this, args);
            _featureHashCode = args.MapFeatureHash;
            foreach (Utilities.NamedAction a in Actions)
            {
                if (a.Header.Equals("Add to Map Window"))
                {
                    a.Header = "Remove from Map Window";
                    a.Action = RemoveTerrainFromMapWindow;
                }
            }
        }

        public string GetTerrainPath()
        {
            return Storage.Connection.Instance.TerrainDirectory + "\\" + FileName;

        }

        //public override void Remove(object arg1, EventArgs arg2)
        //{
        //    if (_Owner.GetType().BaseType == typeof(Utilities.OwnerElement))
        //    {
        //        Utilities.OwnerElement o = (Utilities.OwnerElement)_Owner;
        //        o.Elements.Remove(this);
        //        //delete the terrain file.
        //        //System.IO.File.Delete(FilePath);
        //    }
        //    else
        //    {
        //        //not good...
        //    }
        //}

        public override string TableName
        {
            get
            {
                throw new NotSupportedException("There is no terrain table. look for a Terrains table"); // these are not the droids you are looking for...
            }
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
            //throw new NotImplementedException();
        }

        public override object[] RowData()
        {
            return new object[] { Name, FileName };
        }
        public override bool SavesToRow()
        {
            return true;
        }
        public override bool SavesToTable()
        {
            return false;
        }
    }
}
