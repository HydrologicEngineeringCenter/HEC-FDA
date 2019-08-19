using System;
using FdaViewModel.Inventory;
using FdaViewModel.Saving.PersistenceManagers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FDAUnitTests.Saving.PersistenceManagers
{
    [TestClass]
    public class StructureInventoriesPersistenceManagerTests : PersistenceManagersBaseTest
    {
        private const String DBNAME = "Rating";

        [TestMethod]
        public void SaveNewElemTest()
        {
            InventoryElement elem = createInflowOutflowElement("testElem5");
            StructureInventoryPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetStructureInventoryManager();
            SaveNewTest(DBNAME, elem, manager);

        }

        [TestMethod]
        public void SaveExistingElemTest()
        {
            InventoryElement originalElem = createInflowOutflowElement("testElem4");
            StructureInventoryPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetStructureInventoryManager();
            SaveExistingTest(DBNAME, originalElem, manager);

        }

        [TestMethod]
        public void RemoveElementTest()
        {
            InventoryElement elem = createInflowOutflowElement("testElem1");
            StructureInventoryPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetStructureInventoryManager();
            RemoveTest(DBNAME, elem, manager);

        }

        private InventoryElement createInflowOutflowElement(String name)
        {
            StructureInventoryBaseElement baseElem = new StructureInventoryBaseElement("structInv", "desc");
            InventoryElement elem = new InventoryElement(baseElem);
            return elem;
        }
    }
}
