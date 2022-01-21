using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Utilities;

namespace ViewModel.WaterSurfaceElevation
{
    //[Author(q0heccdm, 9 / 6 / 2017 9:47:42 AM)]
    public class WaterSurfaceElevationElement : Utilities.ChildElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 9/6/2017 9:47:42 AM
        #endregion
        #region Fields
        private const string _TableConstant = "Water Surface Elevation - ";

        private string _Name;
        private string _Description;
        private List<PathAndProbability> _RelativePathAndProbability;
        private List<int> _featureNodeHashs;
        #endregion
        #region Properties
     
        public bool IsDepthGrids { get; set; }
        
        //public override string Name
        //{
        //    get { return _Name; }
        //    set { _Name = value; }
        //}
       
        public List<PathAndProbability> RelativePathAndProbability
        {
            get { return _RelativePathAndProbability; }
            set { _RelativePathAndProbability = value;  }
        }
        public bool HasAssociatedFiles { get; set; }

        #endregion
        #region Constructors
        /// <summary>
        /// This constructor is only used when importing from old fda files. Old fda does not have paths to map layer files.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="isDepthGrids"></param>
        public WaterSurfaceElevationElement(string name, string description,List<double> probabilites, bool isDepthGrids):base()
        {
            HasAssociatedFiles = false;
            List<PathAndProbability> pathAndProbs = new List<PathAndProbability>();
            foreach(double p in probabilites)
            {
                pathAndProbs.Add(new PathAndProbability("NA", p));
            }
            SetConstructorParams(name, description,pathAndProbs, isDepthGrids);
        }
        public WaterSurfaceElevationElement(string name, string description, List<PathAndProbability> relativePathAndProbabilities,bool isDepthGrids) : base()
        {
            HasAssociatedFiles = true;
            SetConstructorParams(name, description,relativePathAndProbabilities, isDepthGrids);
        }

        private void SetConstructorParams(string name, string description,List<PathAndProbability> pathAndProbs, bool isDepthGrids)
        {
            RelativePathAndProbability = pathAndProbs;
            Name = name;
            Description = description;
            if (Description == null)
            {
                Description = "";
            }
            IsDepthGrids = isDepthGrids;
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/WaterSurfaceElevation.png");

            NamedAction editElement = new NamedAction(this);
            editElement.Header = "Edit Water Surface Elevations...";
            editElement.Action = EditElement;

            NamedAction remove = new NamedAction();
            remove.Header = StringConstants.REMOVE_MENU;
            remove.Action = RemoveElement;

            NamedAction renameElement = new NamedAction(this);
            renameElement.Header = StringConstants.RENAME_MENU;
            renameElement.Action = Rename;

            NamedAction mapWindow = new NamedAction();
            mapWindow.Header = StringConstants.ADD_TO_MAP_WINDOW_MENU;
            mapWindow.Action = AddWSEToMapWindow;

            //"NA" has been placed in the "path" column of the database. That means that this WSE came
            //from old FDA and doesn't have a path associated with it and so we disable this menu item.
            bool hasMapLayers = true;
            if(pathAndProbs.Count>0)
            {
                if(pathAndProbs[0].Path.Equals("NA"))
                {
                    hasMapLayers = false;
                }
            }

            if (!hasMapLayers)
            {
                mapWindow.IsEnabled = false;
                mapWindow.ToolTip = "No map layers exist when imported from FDA 1.0";
            }

            List<NamedAction> localactions = new List<Utilities.NamedAction>();
            localactions.Add(editElement);
            localactions.Add(remove);
            localactions.Add(renameElement);
            localactions.Add(mapWindow);

            Actions = localactions;
            TableContainsGeoData = true;
        }



        #endregion
        #region Voids
        public void EditElement(object sender, EventArgs e)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
               .WithSiblingRules(this);

            WaterSurfaceElevationImporterVM vm = new WaterSurfaceElevationImporterVM(this, actionManager);

            string header = "Edit Water Surface Elevation -" + Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditWatSurfElev" + Name);
            Navigate(tab, false, false);
        }
        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetWaterSurfaceManager().Remove(this);
        }
        public override void RemoveElementFromMapWindow(object arg1, EventArgs arg2)
        {
            if (_featureNodeHashs != null)
            {
                foreach (int hash in _featureNodeHashs)
                {
                    RemoveFromMapWindow(this, new Utilities.RemoveMapFeatureEventArgs(hash));
                }
                foreach (Utilities.NamedAction a in Actions)
                {
                    if (a.Header.Equals(StringConstants.REMOVE_FROM_MAP_WINDOW_MENU))
                    {
                        a.Header = StringConstants.ADD_TO_MAP_WINDOW_MENU;
                        a.Action = AddWSEToMapWindow;
                    }
                }
            }
        }

        public void removedcallback(OpenGLMapping.FeatureNodeHeader node, bool includeSelected)
        {
            foreach (Utilities.NamedAction a in Actions)
            {
                if (a.Header.Equals(StringConstants.REMOVE_FROM_MAP_WINDOW_MENU))
                {
                    a.Header = StringConstants.ADD_TO_MAP_WINDOW_MENU;
                    a.Action = AddWSEToMapWindow;
                }
            }
        }

        private void AddWSEToMapWindow(object arg1, EventArgs arg2)
        {
            _featureNodeHashs = new List<int>();
            foreach (PathAndProbability file in RelativePathAndProbability)
            {

                LifeSimGIS.RasterFeatures r = new LifeSimGIS.RasterFeatures(Storage.Connection.Instance.HydraulicsDirectory + "\\" + file.Path);
                OpenGLMapping.ColorRamp c = new OpenGLMapping.ColorRamp(OpenGLMapping.ColorRamp.RampType.LightBlueDarkBlue, r.GridReader.Max, r.GridReader.Min, r.GridReader.Mean, r.GridReader.StdDev);
                Utilities.AddGriddedDataEventArgs args = new Utilities.AddGriddedDataEventArgs(r, c);
                args.FeatureName = Name;
                AddToMapWindow(this, args);

                _featureNodeHashs.Add(args.MapFeatureHash);
            }


            foreach (Utilities.NamedAction a in Actions)
            {
                if (a.Header.Equals(StringConstants.ADD_TO_MAP_WINDOW_MENU))
                {
                    a.Header = StringConstants.REMOVE_FROM_MAP_WINDOW_MENU;
                    a.Action = RemoveElementFromMapWindow;
                }
            }
        }


        #endregion
        #region Functions
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            WaterSurfaceElevationElement elem = (WaterSurfaceElevationElement)elementToClone;
            return new WaterSurfaceElevationElement(elem.Name, elem.Description,elem.RelativePathAndProbability,elem.IsDepthGrids);
        }
        #endregion


  

        //public override void Save()
        //{
        //    //gets called if savestotable is true
        //    if (!Storage.Connection.Instance.IsConnectionNull)
        //    {
        //        if (Storage.Connection.Instance.TableNames().Contains(TableName))
        //        {
        //            //already exists... delete?
        //            Storage.Connection.Instance.DeleteTable(TableName);
        //        }

        //        string[] colNames = new string[] { "Name", "Probability", "LastEdited" };
        //        Type[] colTypes = new Type[] { typeof(string), typeof(string), typeof(string) };

        //        Storage.Connection.Instance.CreateTable(TableName, colNames, colTypes);
        //        DataBase_Reader.DataTableView tbl = Storage.Connection.Instance.GetTable(TableName);

        //        object[][] rows = new object[RelativePathAndProbability.Count][];
        //        int i = 0;
        //        foreach (PathAndProbability p in RelativePathAndProbability)
        //        {
        //            rows[i] = new object[] { p.Path, p.Probability, DateTime.Now.ToString() };
        //            i++;
        //        }
        //        for (int j = 0; j < rows.Count(); j++)
        //        {
        //            tbl.AddRow(rows[j]);
        //        }
        //        tbl.ApplyEdits();


        //    }
        //}
       
    }
}
