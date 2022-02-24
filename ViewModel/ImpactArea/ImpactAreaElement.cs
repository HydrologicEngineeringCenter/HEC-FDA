using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.ImpactArea
{
    public class ImpactAreaElement : ChildElement
    {
        #region Notes
        #endregion
        #region Fields
        private ObservableCollection<ImpactAreaRowItem> _ImpactAreaRows;
        private int _featureNodeHash;
        #endregion
        #region Properties
      
        public string SelectedPath { get; set; }
        
        public ObservableCollection<ImpactAreaRowItem> ImpactAreaRows
        {
            get { return _ImpactAreaRows; }
            set { _ImpactAreaRows = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public ImpactAreaElement(string userdefinedname, string description, ObservableCollection<ImpactAreaRowItem> collectionOfRows, int id) : this(userdefinedname,description,collectionOfRows, "", id)
        {
        }
        public ImpactAreaElement(string userdefinedname,string description, ObservableCollection<ImpactAreaRowItem> collectionOfRows, string selectedPath, int id ) : base(id)
        {
            Name = userdefinedname;
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/ImpactAreas.png");
            Description = description;
            ImpactAreaRows = collectionOfRows;

            SelectedPath = selectedPath;

            NamedAction edit = new NamedAction();
            edit.Header = "Edit Impact Area Set...";
            edit.Action = Edit;

            NamedAction addToMapWindow = new NamedAction();
            addToMapWindow.Header = StringConstants.ADD_TO_MAP_WINDOW_MENU;
            addToMapWindow.Action = ImpactAreasToMapWindow;

            NamedAction removeImpactArea = new NamedAction();
            removeImpactArea.Header = StringConstants.REMOVE_MENU;
            removeImpactArea.Action = RemoveElement;

            NamedAction renameElement = new NamedAction(this);
            renameElement.Header = StringConstants.RENAME_MENU;
            renameElement.Action = Rename;

            List<NamedAction> localactions = new List<NamedAction>();
            localactions.Add(addToMapWindow);
            localactions.Add(edit);
            localactions.Add(removeImpactArea);
            localactions.Add(renameElement);

            Actions = localactions;

            TableContainsGeoData = true;
        }

        public void RemoveElement(object sender, EventArgs e)
        {
            List<IASElementSet> iasElems = StudyCache.GetChildElementsOfType<IASElementSet>();
            if (iasElems.Count > 0)
            {
                StringBuilder sb = new StringBuilder(Environment.NewLine).Append(Environment.NewLine);
                foreach(IASElementSet set in iasElems)
                {
                    sb.Append("\t").Append("* ").Append(set.Name).Append(Environment.NewLine);
                }

                var result = MessageBox.Show("Deleting the impact area will also delete all existing impact area scenarios: " +
                    sb.ToString() + Environment.NewLine + "Do you want to continue with the delete?", "Do You Want to Continue", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Saving.PersistenceFactory.GetImpactAreaManager().Remove(this);
                    //delete the IAS's.
                    foreach(IASElementSet set in iasElems)
                    {
                        Saving.PersistenceFactory.GetIASManager().Remove(set);
                    }
                }
            }
            else
            {
                Saving.PersistenceFactory.GetImpactAreaManager().Remove(this);
            }
        }

        private void ImpactAreasToMapWindow(object arg1, EventArgs arg2)
        {
            DatabaseManager.SQLiteManager sqr = new DatabaseManager.SQLiteManager(Storage.Connection.Instance.ProjectFile);
            LifeSimGIS.GeoPackageReader gpr = new LifeSimGIS.GeoPackageReader(sqr);
            string tableBaseName = ImpactAreaPersistenceManager.IMPACT_AREA_TABLE_PREFIX;
            LifeSimGIS.PolygonFeatures polyFeatures = (LifeSimGIS.PolygonFeatures)gpr.ConvertToGisFeatures(tableBaseName + Name);
            LifeSimGIS.VectorFeatures features = polyFeatures;
            //read from table.
            DatabaseManager.DataTableView dtv = sqr.GetTableManager(tableBaseName + Name);

            OpenGLMapping.OpenGLDrawInfo ogldi = new OpenGLMapping.OpenGLDrawInfo(true, new OpenTK.Graphics.Color4((byte)255, 0, 0, 255), 1, true, new OpenTK.Graphics.Color4((byte)0, 255, 0, 200));
            AddShapefileEventArgs args = new AddShapefileEventArgs(Name, features, dtv, ogldi);
            AddToMapWindow(this, args);
            _featureNodeHash = args.MapFeatureHash;
            foreach (NamedAction a in Actions)
            {
                if (a.Header.Equals(StringConstants.ADD_TO_MAP_WINDOW_MENU))
                {
                    a.Header = StringConstants.REMOVE_FROM_MAP_WINDOW_MENU;
                    a.Action = RemoveElementFromMapWindow;
                }
            }
        }

        public override void RemoveElementFromMapWindow(object arg1, EventArgs arg2)
        {
            RemoveFromMapWindow(this, new RemoveMapFeatureEventArgs(_featureNodeHash));
        }

        public void removedcallback(OpenGLMapping.FeatureNodeHeader node, bool includeSelected)
        {
            foreach (NamedAction a in Actions)
            {
                if (a.Header.Equals("Remove Impact Areas from Map Window"))
                {
                    a.Header = "Add Impact Areas To Map Window";
                    a.IsEnabled = true;
                    a.Action = ImpactAreasToMapWindow;
                }
            }
        }
        #endregion
        #region Voids
        private void Edit(object arg1, EventArgs arg2)
        {
            //create an observable collection of all the available paths
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);

            ImpactAreaImporterVM vm = new ImpactAreaImporterVM(this, ImpactAreaRows, actionManager);
            string header = "Edit Impact Area Set";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditImpactArea");
            Navigate(tab, false,false);
        }

        #endregion
        #region Functions 
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            ImpactAreaElement elem = (ImpactAreaElement)elementToClone;
            return new ImpactAreaElement(elem.Name, elem.Description,elem.ImpactAreaRows,elem.SelectedPath, elem.ID);
        }
        #endregion
    }
}
