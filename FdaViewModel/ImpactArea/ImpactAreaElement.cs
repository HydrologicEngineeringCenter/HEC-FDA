using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using FdaViewModel.Utilities;

namespace FdaViewModel.ImpactArea
{
    public class ImpactAreaElement : Utilities.ChildElement
    {
        #region Notes
        #endregion
        #region Fields
        private const string _TableConstant = "Impact Areas - ";
        private ObservableCollection<ImpactAreaRowItem> _ImpactAreaRows;
        private int _featureNodeHash;
        //private string _FilePath;
        #endregion
        #region Properties
      
        public string SelectedPath { get; set; }
        public string SelectedUniqueName { get; set; }
        public List<string> UniqueNames { get; set; }
        //public LifeSimGIS.PolygonFeatures PolygonFeatures { get; set; }
        
        public ObservableCollection<ImpactAreaRowItem> ImpactAreaRows
        {
            get { return _ImpactAreaRows; }
            set { _ImpactAreaRows = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public ImpactAreaElement(string userdefinedname, string description, ObservableCollection<ImpactAreaRowItem> collectionOfRows,
            ImpactAreaOwnerElement owner = null) : this(userdefinedname,description,collectionOfRows, "", owner)
        {
        }
        public ImpactAreaElement(string userdefinedname,string description, ObservableCollection<ImpactAreaRowItem> collectionOfRows, string selectedPath, ImpactAreaOwnerElement owner ) : base(owner)
        {
            Name = userdefinedname;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/ImpactAreas.png");
            Description = description;
            //_FilePath = shapeFilePath;
            //SelectedUniqueName = uniqueName;
            ImpactAreaRows = collectionOfRows;

            //if (writeGeoDataToSqlite == true)
            //{
            //    WriteImpactAreaTableToSqlite(polyFeatures);
            //}
            //ImpactAreaRows = new ObservableCollection<ImpactAreaRowItem>();
            //foreach (object o in items)
            //{
            //    ImpactAreaRows.Add(items as ImpactAreaRowItem);
            //}

            SelectedPath = selectedPath;

            Utilities.NamedAction edit = new Utilities.NamedAction();
            edit.Header = "Edit Impact Areas";
            edit.Action = Edit;

            Utilities.NamedAction addToMapWindow = new Utilities.NamedAction();
            addToMapWindow.Header = "Add Impact Areas To Map Window";
            addToMapWindow.Action = ImpactAreasToMapWindow;

            Utilities.NamedAction removeImpactArea = new Utilities.NamedAction();
            removeImpactArea.Header = "Remove";
            removeImpactArea.Action = RemoveElement;

            Utilities.NamedAction renameElement = new Utilities.NamedAction(this);
            renameElement.Header = "Rename";
            renameElement.Action = Rename;

            List<Utilities.NamedAction> localactions = new List<Utilities.NamedAction>();
            localactions.Add(addToMapWindow);
            localactions.Add(edit);
            localactions.Add(removeImpactArea);
            localactions.Add(renameElement);


            Actions = localactions;

            TableContainsGeoData = true;
        }

        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetImpactAreaManager(StudyCache).Remove(this);
        }

        private void ImpactAreasToMapWindow(object arg1, EventArgs arg2)
        {
            //DataBase_Reader.SqLiteReader sqr = new DataBase_Reader.SqLiteReader(Storage.Connection.Instance.ProjectFile);
            //LifeSimGIS.GeoPackageReader gpr = new LifeSimGIS.GeoPackageReader(sqr);
            //LifeSimGIS.PolygonFeatures polyFeatures = (LifeSimGIS.PolygonFeatures)gpr.ConvertToGisFeatures(GetTableConstant() + this.Name);
            //LifeSimGIS.VectorFeatures features = polyFeatures;
            ////read from table.
            //DataBase_Reader.DataTableView dtv = sqr.GetTableManager(GetTableConstant() + this.Name);
            //int[] geometryColumns = { 0, 1 };
            //dtv.DeleteColumns(geometryColumns);

            //OpenGLMapping.OpenGLDrawInfo ogldi = new OpenGLMapping.OpenGLDrawInfo(true, new OpenTK.Graphics.Color4((byte)255, 0, 0, 255), 1, true, new OpenTK.Graphics.Color4((byte)0, 255, 0, 200));
            //Utilities.AddShapefileEventArgs args = new Utilities.AddShapefileEventArgs(Name, features, dtv, ogldi);
            //AddToMapWindow(this, args);
            //_featureNodeHash = args.MapFeatureHash;
            Saving.PersistenceFactory.GetImpactAreaManager(StudyCache).ImpactAreasToMapWindow(this);
            foreach (Utilities.NamedAction a in Actions)
            {
                if (a.Header.Equals("Add Impact Areas To Map Window"))
                {
                    a.Header = "Remove Impact Areas from Map Window";
                    a.Action = RemoveElementFromMapWindow;
                }
            }

        }

        

        public override void RemoveElementFromMapWindow(object arg1, EventArgs arg2)
        {
            RemoveFromMapWindow(this, new Utilities.RemoveMapFeatureEventArgs(_featureNodeHash));
            //foreach (Utilities.NamedAction a in Actions)
            //{
            //    if (a.Header.Equals("Remove Impact Areas from Map Window"))
            //    {
            //        a.Header = "Add Impact Areas To Map Window";
            //        a.Action = ImpactAreasToMapWindow;
            //    }
            //}
        }
        public void removedcallback(OpenGLMapping.FeatureNodeHeader node, bool includeSelected)
        {
            foreach (Utilities.NamedAction a in Actions)
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


            ImpactAreaImporterVM vm = new ImpactAreaImporterVM(this, ImpactAreaRows);
            Navigate(vm, false,false,"Edit Impact Area");

            //if (!vm.WasCanceled)
            //{
            //    string originalName = Name;
            //    Name = vm.Name;
                
            //    this.Description = vm.Description;
            //    ImpactAreaRows = vm.ListOfRows;

            //    ((ImpactAreaOwnerElement)_Owner).UpdateTableRowIfModified((Utilities.ParentElement)_Owner, originalName, this);
            //    UpdateExistingTable();

            //}
            //ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("Under Construction.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel | FdaModel.Utilities.Messager.ErrorMessageEnum.Report));
        }
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        //public  void Save()
        //{
        //    LifeSimGIS.ShapefileReader shp = new LifeSimGIS.ShapefileReader(SelectedPath);
        //    LifeSimGIS.PolygonFeatures polyFeatures = (LifeSimGIS.PolygonFeatures)shp.ToFeatures();
        //    WriteImpactAreaTableToSqlite(polyFeatures);
        //}

        //private void UpdateExistingTable()
        //{

        //    DataBase_Reader.SqLiteReader sqlReader = new DataBase_Reader.SqLiteReader(Storage.Connection.Instance.ProjectFile);
        //    DataBase_Reader.DataTableView dtv = sqlReader.GetTableManager(TableName);

        //    object[] nameArray = new object[ImpactAreaRows.Count];
        //    object[] indexPointArray = new object[ImpactAreaRows.Count];
        //    int i = 0;
        //    foreach (ImpactAreaRowItem row in ImpactAreaRows)
        //    {
        //        nameArray[i] = row.Name;
        //        indexPointArray[i] = row.IndexPoint;
        //        i++;
        //    }

        //    dtv.EditColumn(2,nameArray);
        //    dtv.EditColumn(3, indexPointArray);

        //    dtv.ApplyEdits();


        //}

        //private void WriteImpactAreaTableToSqlite(LifeSimGIS.PolygonFeatures polyFeatures)
        //{
        //    if (!Storage.Connection.Instance.IsConnectionNull)
        //    {
        //        if (Storage.Connection.Instance.TableNames().Contains(TableName))
        //        {
        //            //already exists... delete?
        //            Storage.Connection.Instance.DeleteTable(TableName);
        //        }



        //        LifeSimGIS.GeoPackageWriter gpw = new LifeSimGIS.GeoPackageWriter(Storage.Connection.Instance.Reader);


        //        System.Data.DataTable dt = new System.Data.DataTable(TableName);
        //        dt.Columns.Add("Name", typeof(string));
        //        dt.Columns.Add("IndexPoint", typeof(double));

        //        foreach (ImpactAreaRowItem row in ImpactAreaRows)
        //        {
        //            dt.Rows.Add(row.Name, row.IndexPoint);

        //        }

        //        DataBase_Reader.InMemoryReader imr = new DataBase_Reader.InMemoryReader(dt);

        //        gpw.AddFeatures(TableName, polyFeatures, imr.GetTableManager(imr.TableNames[0]));
        //    }

        //}

        //public override object[] RowData()
        //{
        //    return new object[] { Name,Description };
        //}


        #endregion
        #region Functions 
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            ImpactAreaElement elem = (ImpactAreaElement)elementToClone;
            return new ImpactAreaElement(elem.Name, elem.Description,elem.ImpactAreaRows,elem.SelectedPath,null);
        }
        #endregion

        public override string ToString()
        {
            return this.Name;
        }

    }
}
