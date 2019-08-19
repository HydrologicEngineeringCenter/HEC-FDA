using System;
using System.Collections.ObjectModel;
using FdaViewModel.ImpactArea;
using FdaViewModel.Saving.PersistenceManagers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FDAUnitTests.Saving.PersistenceManagers
{
    [TestClass]
    public class ImpactAreaPersistenceManagerTests : PersistenceManagersBaseTest
    {
        private const String DBNAME = "ImpactArea";

        [TestMethod]
        public void SaveNewElemTest()
        {
            ImpactAreaElement elem = createImpactAreaElement("testElem5");
            ImpactAreaPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetImpactAreaManager();
            SaveNewTest(DBNAME, elem, manager);

        }

        [TestMethod]
        public void SaveExistingElemTest()
        {
            ImpactAreaElement originalElem = createImpactAreaElement("testElem4");
            ImpactAreaPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetImpactAreaManager();
            SaveExistingTest(DBNAME, originalElem, manager);

        }


        [TestMethod]
        public void RemoveElementTest()
        {
            ImpactAreaElement elem = createImpactAreaElement("testElem1");
            ImpactAreaPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetImpactAreaManager();
            RemoveTest(DBNAME, elem, manager);

        }

        private ImpactAreaElement createImpactAreaElement(String name)
        {
            ObservableCollection<ImpactAreaRowItem> rows = new ObservableCollection<ImpactAreaRowItem>();
            for (int i = 0; i < 10; i++)
            {
                ImpactAreaRowItem ri = new ImpactAreaRowItem("a"+i, i, new ObservableCollection<object>());
                rows.Add(ri);
            }

            ImpactAreaElement elem = new ImpactAreaElement(name, "desc", rows);
            elem.SelectedPath = @"C:\Users\cody\Documents\HEC\HEC-FDA\Sample Data\UnCorrupted Data\LCC\DamageReaches\Econ_Impact_Areas.shp";
            return elem;
        }
    }
}
