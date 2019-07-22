using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using FdaViewModel.Utilities;

namespace FdaViewModel.ImpactArea
{
    public class ImpactAreaOwnerElement : Utilities.ParentElement
    {
        #region Notes
        #endregion
        #region Fields

       
        #endregion
        #region Properties
        
        #endregion
        #region Constructors
        public ImpactAreaOwnerElement( ) : base()
        {
            Name = "Impact Areas";
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);
            Utilities.NamedAction add = new Utilities.NamedAction();
            add.Header = "Import Impact Areas";
            add.Action = AddNew;


            List<Utilities.NamedAction> localactions = new List<Utilities.NamedAction>();
            localactions.Add(add);

            Actions = localactions;

            StudyCache.ImpactAreaAdded += AddImpactAreaElement;
            StudyCache.ImpactAreaRemoved += RemoveImpactAreaElement;
            StudyCache.ImpactAreaUpdated += UpdateImpactAreaElement;
            GUID = Guid.NewGuid();

        }
        #endregion
        #region Voids
        private void UpdateImpactAreaElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
        private void AddImpactAreaElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        private void RemoveImpactAreaElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        #endregion
        #region Functions
        #endregion

        public void AddNew(object arg1, EventArgs arg2)
        {
            List<string> paths = new List<string>();
            ShapefilePathsOfType(ref paths,Utilities.VectorFeatureType.Polygon);
            ObservableCollection<string> observpaths = new ObservableCollection<string>();
            foreach(string s in paths)
            {
                observpaths.Add(s);
            }

            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this)
                .WithParentGuid(this.GUID)
                .WithCanOpenMultipleTimes(true);

            ImpactAreaImporterVM vm = new ImpactAreaImporterVM(observpaths, actionManager);
            //ExtendEventsToImporter(vm);
            
            //vm.AddSiblingRules(this);
            //vm.ParentGUID = this.GUID;
            //vm.CanOpenMultipleTimes = true;

            Navigate( vm,false,false,"Import Impact Areas");
            //if (!vm.HasError & !vm.WasCanceled)
            //{
                
            //    if(vm.Description == null) { vm.Description = ""; }
            //   // LifeSimGIS.ShapefileReader shp = new LifeSimGIS.ShapefileReader(vm.SelectedPath);
            //    //LifeSimGIS.PolygonFeatures polyFeatures = (LifeSimGIS.PolygonFeatures)shp.ToFeatures();

            //    ImpactAreaElement element = new ImpactAreaElement(vm.Name, vm.Description, vm.ListOfRows, vm.SelectedPath, this);
            //    AddElement(element,true);
            //    //element.WriteImpactAreaTableToSqlite(polyFeatures);
            //    AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(vm.Name, Utilities.Transactions.TransactionEnum.CreateNew, "Impact Areas were created using the file " + vm.SelectedPath, nameof(ImpactAreaElement)));

            //}
        }

        //public override void AddBaseElements()
        //{
        //    throw new NotImplementedException();
        //}

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        //public override string[] TableColumnNames()
        //{
        //    return new string[] {"Impact Area Set Name","Description"};
        //}

        //public override Type[] TableColumnTypes()
        //{
        //    return new Type[] { typeof(string), typeof(string) };
        //}


        //public override ChildElement CreateElementFromRowData(object[] rowData)
        //{
        //    ObservableCollection<ImpactAreaRowItem> dummyCollection = new ObservableCollection<ImpactAreaRowItem>();

        //    //i need to read from the sqlite table and get the polygon features and pass it to the new element
        //    //DataBase_Reader.SqLiteReader sqr = new DataBase_Reader.SqLiteReader(Storage.Connection.Instance.ProjectFile);
        //    //LifeSimGIS.GeoPackageReader gpr = new LifeSimGIS.GeoPackageReader(sqr);
        //    //LifeSimGIS.PolygonFeatures polyFeatures = (LifeSimGIS.PolygonFeatures)gpr.ConvertToGisFeatures("Impact Areas - " + rowData[0]);

        //    ImpactAreaElement iae = new ImpactAreaElement((string)rowData[0], (string)rowData[1], dummyCollection, this);

        //    int lastRow = Storage.Connection.Instance.GetTable(iae.TableName).NumberOfRows - 1;
        //    ObservableCollection<object> tempCollection = new ObservableCollection<object>();
        //    foreach (object[] row in Storage.Connection.Instance.GetTable(iae.TableName).GetRows(0, lastRow))
        //    {
        //        //each row here should be a name and an index point
        //        ImpactAreaRowItem ri = new ImpactAreaRowItem(row[2].ToString(), Convert.ToDouble(row[3]), tempCollection);
        //        tempCollection.Add(ri);
        //    }
        //    ObservableCollection<ImpactAreaRowItem> items = new ObservableCollection<ImpactAreaRowItem>();
        //    foreach (object row in tempCollection)
        //    {
        //        items.Add((ImpactAreaRowItem)row);
        //    }
        //    iae.ImpactAreaRows = items;
        //    return iae;
        //}
        //public override void AddElementFromRowData(object[] rowData)
        //{
            
        //    AddElement(CreateElementFromRowData(rowData),false);
        //}
    }
}
