using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.WaterSurfaceElevation
{
    //[Author(q0heccdm, 9 / 6 / 2017 9:47:42 AM)]
    public class WaterSurfaceElevationElement : Utilities.OwnedElement
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
        public override string GetTableConstant()
        {
            return _TableConstant;
        }
        public bool IsDepthGrids { get; set; }
        
        //public override string Name
        //{
        //    get { return _Name; }
        //    set { _Name = value; }
        //}
        public string Description
        {
            get { return _Description; }
            set { _Description = value;  }
        }
        public List<PathAndProbability> RelativePathAndProbability
        {
            get { return _RelativePathAndProbability; }
            set { _RelativePathAndProbability = value;  }
        }

        public override string TableName
        {
            get
            {
                return GetTableConstant() + Name;
            }
        }

        #endregion
        #region Constructors
        
        public WaterSurfaceElevationElement(string name, string description, List<PathAndProbability> relativePathAndProbabilities,bool isDepthGrids, BaseFdaElement owner) : base(owner)
        {
            Name = name;
            Description = description;
            if(Description == null)
            {
                Description = "";
            }
            RelativePathAndProbability = relativePathAndProbabilities;
            IsDepthGrids = isDepthGrids;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/WaterSurfaceElevation.png");


            Utilities.NamedAction remove = new Utilities.NamedAction();
            remove.Header = "Remove";
            remove.Action = Remove;

            Utilities.NamedAction renameElement = new Utilities.NamedAction();
            renameElement.Header = "Rename";
            renameElement.Action = Rename;



            Utilities.NamedAction mapWindow = new Utilities.NamedAction();
            mapWindow.Header = "Add to Map Window";
            mapWindow.Action = AddWSEToMapWindow;

            List<Utilities.NamedAction> localactions = new List<Utilities.NamedAction>();
            localactions.Add(remove);
            localactions.Add(renameElement);
            localactions.Add(mapWindow);

            Actions = localactions;
            TableContainsGeoData = true;

        }


        #endregion
        #region Voids
        
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
                    if (a.Header.Equals("Remove from Map Window"))
                    {
                        a.Header = "Add to Map Window";
                        a.Action = AddWSEToMapWindow;
                    }
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
                    a.Action = AddWSEToMapWindow;
                }
            }
        }

        private void AddWSEToMapWindow(object arg1, EventArgs arg2)
        {
            _featureNodeHashs = new List<int>();
            foreach (PathAndProbability file in RelativePathAndProbability)
            {

                LifeSimGIS.RasterFeatures r = new LifeSimGIS.RasterFeatures(Storage.Connection.Instance.HydraulicsDirectory + "\\"+file.Path);
                OpenGLMapping.ColorRamp c = new OpenGLMapping.ColorRamp(OpenGLMapping.ColorRamp.RampType.LightBlueDarkBlue, r.GridReader.Max, r.GridReader.Min, r.GridReader.Mean, r.GridReader.StdDev);
                Utilities.AddGriddedDataEventArgs args = new Utilities.AddGriddedDataEventArgs(r, c);
                args.FeatureName = Name;
                AddToMapWindow(this, args);

                _featureNodeHashs.Add(args.MapFeatureHash);
            }

            foreach (Utilities.NamedAction a in Actions)
            {
                if (a.Header.Equals("Add to Map Window"))
                {
                    a.Header = "Remove from Map Window";
                    a.Action = RemoveElementFromMapWindow;
                }
            }
        }

       
        #endregion
        #region Functions
        #endregion
        public override object[] RowData()
        {
            return new object[] { Name, Description,IsDepthGrids };
        }

        public override bool SavesToRow()
        {
            return true;
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            //gets called if savestotable is true
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                if (Storage.Connection.Instance.TableNames().Contains(TableName))
                {
                    //already exists... delete?
                    Storage.Connection.Instance.DeleteTable(TableName);
                }

                string[] colNames = new string[] { "Name", "Probability", "LastEdited" };
                Type[] colTypes = new Type[] { typeof(string), typeof(string), typeof(string) };

                Storage.Connection.Instance.CreateTable(TableName, colNames, colTypes);
                DataBase_Reader.DataTableView tbl = Storage.Connection.Instance.GetTable(TableName);

                object[][] rows = new object[RelativePathAndProbability.Count][];
                int i = 0;
                foreach (PathAndProbability p in RelativePathAndProbability)
                {
                    rows[i] = new object[] { p.Path, p.Probability, DateTime.Now.ToString() };
                    i++;
                }
                for (int j = 0; j < rows.Count(); j++)
                {
                    tbl.AddRow(rows[j]);
                }
                tbl.ApplyEdits();


            }
        }
        public override bool SavesToTable()
        {
            return true;
        }
    }
}
