using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory
{
    //[Author(q0heccdm, 12 / 1 / 2016 2:21:18 PM)]
    public class InventoryElement : Utilities.ChildElement
    {

        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/1/2016 2:21:18 PM
        #endregion
        #region Fields
        private int _featureHashCode;
        private const string _TableConstant = "Structure Inventory - ";

        private StructureInventoryBaseElement _StructureInventory;

        //private string _Name;
        #endregion
        #region Properties
        public string Description { get; set; }
        public DefineSIAttributesVM DefineSIAttributes { get; set; }
        public AttributeLinkingListVM AttributeLinkingList { get; set; }
        public override string GetTableConstant()
        {
            return _TableConstant;
        }
        public StructureInventoryBaseElement StructureInventory
        {
            get { return _StructureInventory; }
            set { _StructureInventory = value; NotifyPropertyChanged(); }
        }
      
        #endregion
        #region Constructors
        public InventoryElement(StructureInventoryBaseElement structureInventory, DefineSIAttributesVM defSIAttributes, AttributeLinkingListVM attLinkVM, BaseFdaElement owner = null) : base(owner)
        {
            //Name = structureInventory.Name;
            //CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/StructureInventory.png");

            //StructureInventory = structureInventory;
            //DefineSIAttributes = defSIAttributes;
            //AttributeLinkingList = attLinkVM;

            //Utilities.NamedAction edit = new Utilities.NamedAction();
            //edit.Header = "Edit Structure inventory";
            //edit.Action = Edit;

            //Utilities.NamedAction mapWindow = new Utilities.NamedAction();
            //mapWindow.Header = "Add to Map Window";
            //mapWindow.Action = AddToMapWindow;

            //Utilities.NamedAction removeInventory = new Utilities.NamedAction();
            //removeInventory.Header = "Remove";
            //removeInventory.Action = Remove;

            //Utilities.NamedAction renameElement = new Utilities.NamedAction();
            //renameElement.Header = "Rename";
            //renameElement.Action = Rename;

            //List<Utilities.NamedAction> localactions = new List<Utilities.NamedAction>();
            ////localactions.Add(addToMapWindow);
            //localactions.Add(edit);
            //localactions.Add(mapWindow);

            //localactions.Add(removeInventory);
            //localactions.Add(renameElement);


            //Actions = localactions;

        }

        public InventoryElement(StructureInventoryBaseElement structInventoryBaseElement, BaseFdaElement owner = null) : base(owner)
        {
            
            Name = structInventoryBaseElement.Name;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/StructureInventory.png");

            Description = structInventoryBaseElement.Description;
            if(Description == null) { Description = ""; }

            StructureInventory = structInventoryBaseElement;
            

            //Utilities.NamedAction edit = new Utilities.NamedAction();
            //edit.Header = "Edit Structure inventory";
            //edit.Action = Edit;

            Utilities.NamedAction mapWindow = new Utilities.NamedAction();
            mapWindow.Header = "Add to Map Window";
            mapWindow.Action = InventoryToMapWindow;

            Utilities.NamedAction removeInventory = new Utilities.NamedAction();
            removeInventory.Header = "Remove";
            removeInventory.Action = Remove;

            Utilities.NamedAction renameElement = new Utilities.NamedAction();
            renameElement.Header = "Rename";
            renameElement.Action = Rename;

            List<Utilities.NamedAction> localactions = new List<Utilities.NamedAction>();
            //localactions.Add(addToMapWindow);
            //localactions.Add(edit);
            localactions.Add(mapWindow);

            localactions.Add(removeInventory);
            localactions.Add(renameElement);


            Actions = localactions;

            TableContainsGeoData = true;

        }
        #endregion
        #region Voids

        private void InventoryToMapWindow(object arg1, EventArgs arg2)
        {
            //DataBase_Reader.SqLiteReader sqr = new DataBase_Reader.SqLiteReader(Storage.Connection.Instance.ProjectFile);
            LifeSimGIS.GeoPackageReader gpr = new LifeSimGIS.GeoPackageReader(Storage.Connection.Instance.Reader);
            LifeSimGIS.PointFeatures pointFeatures = (LifeSimGIS.PointFeatures)gpr.ConvertToGisFeatures(_TableConstant + this.Name);
            LifeSimGIS.VectorFeatures features = pointFeatures;
            //read from table.
            DataBase_Reader.DataTableView dtv = Storage.Connection.Instance.Reader.GetTableManager(_TableConstant + this.Name);
            int[] geometryColumns = { 0, 1 };
            dtv.DeleteColumns(geometryColumns);

            OpenGLMapping.OpenGLDrawInfo ogldi = new OpenGLMapping.OpenGLDrawInfo(15,OpenGLMapping.OpenGLDrawInfo.GlyphType.House1,true, new OpenTK.Graphics.Color4((byte)0, 0, 0, 255), true, new OpenTK.Graphics.Color4((byte)0, 0, 255, 200),true);

            Utilities.AddShapefileEventArgs args = new Utilities.AddShapefileEventArgs(Name, features, dtv, ogldi);

            AddToMapWindow(this,args );

            _featureHashCode = args.MapFeatureHash;

            foreach (Utilities.NamedAction a in Actions)
            {
                if (a.Header.Equals("Add to Map Window"))
                {
                    a.Header = "Remove from Map Window";
                    a.Action = RemoveElementFromMapWindow;
                }
            }
        }

      
        public override void RemoveElementFromMapWindow(object arg1, EventArgs arg2)
        {
            RemoveFromMapWindow(this,new Utilities.RemoveMapFeatureEventArgs(_featureHashCode));
            //foreach (Utilities.NamedAction a in Actions)
            //{
            //    if (a.Header.Equals("Remove from Map Window"))
            //    {
            //        a.Header = "Add to Map Window";
            //        a.Action = InventoryToMapWindow;
            //    }
            //}
        }
        //public void Edit(object arg1, EventArgs arg2)
        //{
            ////List<string> myStrings = new List<string>();

            //ImportStructuresFromShapefileVM vm = new ImportStructuresFromShapefileVM(_StructureInventory.Name, _StructureInventory.Description,_StructureInventory.Path, DefineSIAttributes, AttributeLinkingList);

            //Navigate(vm, true, true);
            ////if (!vm.WasCancled)
            //{
            //  //  if (!vm.HasError)
            //    {
            //        StructureInventoryBaseElement SIBase = new StructureInventoryBaseElement(vm.Name , vm.Description);
            //        this.StructureInventory = SIBase;
            //        this.DefineSIAttributes = vm.DefineSIAttributes;
            //        CustomTreeViewHeader = new Utilities.CustomHeaderVM(vm.Name, "pack://application:,,,/Fda;component/Resources/StructureInventory.png");


            //    }
            //}

        //}
        #endregion
        #region Functions
        #endregion
        public override string TableName
        {
            get
            {
                return _TableConstant + Name;
            }
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            //throw new NotImplementedException();
            //RenameDetailedInventoryTable();
        }
        //protected override void RenameTable(string oldName, string newName, string tableConstant)
        //{
        //    string PreviousTableName = "Structure Inventory-" + oldName;
        //    string newTableName = "Structure Inventory-" + newName;


        //    if (Storage.Connection.Instance.TableNames().Contains(PreviousTableName))
        //    {
        //        LifeSimGIS.GeoPackageWriter myGeoPackWriter = new LifeSimGIS.GeoPackageWriter(StructureInventoryLibrary.SharedData.StudyDatabase);
        //        myGeoPackWriter.RenameFeatures(PreviousTableName, newTableName);
        //    }
        //}

        //public override void Update(string newName)
        //{
        //    string PreviousTableName = TableName;
        //    string newTableName = GetTableConstant() + newName;


        //    if (Storage.Connection.Instance.TableNames().Contains(PreviousTableName))
        //    {
        //        LifeSimGIS.GeoPackageWriter myGeoPackWriter = new LifeSimGIS.GeoPackageWriter(StructureInventoryLibrary.SharedData.StudyDatabase);
        //        myGeoPackWriter.RenameFeatures(PreviousTableName, newTableName);
        //    }
        //}

        public void removedcallback(OpenGLMapping.FeatureNodeHeader node, bool includeSelected)
        {
            foreach (Utilities.NamedAction a in Actions)
            {
                if (a.Header.Equals("Remove from Map Window"))
                {
                    a.Header = "Add to Map Window";
                    a.IsEnabled = true;
                    a.Action = InventoryToMapWindow;
                }
            }
        }
       
        public override object[] RowData()
        {
            return new object[] { Name,Description };
        }

        public override bool SavesToRow()
        {
            return true;
        }
        public override bool SavesToTable()
        {
            return true;
        }
    }
}
