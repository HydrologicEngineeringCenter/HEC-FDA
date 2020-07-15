using FdaViewModel.Editors;
using FdaViewModel.Saving.PersistenceManagers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory
{
    public class ImportStructuresFromFDA1VM : BaseEditorVM
    {
        private DataTable _Inventory;
        public DataTable Inventory
        {
            get { return _Inventory; }
            set { _Inventory = value; NotifyPropertyChanged(); }
        }

        public ImportStructuresFromFDA1VM(EditorActionManager actionManager) : base(actionManager)
        {
            
        }

        public void SetInventory(DataTable dt, string name)
        {
            Name = name;
            Inventory = dt;
        }

        public override void Save()
        {

            StructureInventoryPersistenceManager manager = Saving.PersistenceFactory.GetStructureInventoryManager();
            manager.SaveNew(Inventory, Name);
            manager.SaveNewInventoryToParentTable(Name, Description);
        }
    }
}
