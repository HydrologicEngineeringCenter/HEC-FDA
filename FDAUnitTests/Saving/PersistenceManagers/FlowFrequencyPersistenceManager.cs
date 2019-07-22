using System;
using FdaViewModel.FrequencyRelationships;
using FdaViewModel.Saving.PersistenceManagers;
using FdaViewModel.Study;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Statistics;

namespace FDAUnitTests.Saving.PersistenceManagers
{
    [TestClass]
    public class FlowFrequencyPersistenceManagerTests : PersistenceManagersBaseTest
    {
        private const String DBNAME = "FlowFrequency";

        [TestMethod]
        public void SaveNewElemTest()
        {
            AnalyticalFrequencyElement elem = createExteriorInteriorElement("testElem5");
            FlowFrequencyPersistenceManager manager = createPersistenceManager();
            SaveNewTest(DBNAME, elem, manager);

        }

        [TestMethod]
        public void SaveExistingElemTest()
        {
            AnalyticalFrequencyElement originalElem = createExteriorInteriorElement("testElem4");
            FlowFrequencyPersistenceManager manager = createPersistenceManager();
            SaveExistingTest(DBNAME, originalElem, manager);

        }

        [TestMethod]
        public void UndoElemTest()
        {
            AnalyticalFrequencyElement originalElem = createExteriorInteriorElement("testElem3");
            FlowFrequencyPersistenceManager manager = createPersistenceManager();
            UndoTest(DBNAME, originalElem, manager);

        }

        [TestMethod]
        public void RedoElemTest()
        {
            AnalyticalFrequencyElement originalElem = createExteriorInteriorElement("testElem2");
            FlowFrequencyPersistenceManager manager = createPersistenceManager();
            RedoTest(DBNAME, originalElem, manager);

        }

        [TestMethod]
        public void RemoveElementTest()
        {
            AnalyticalFrequencyElement elem = createExteriorInteriorElement("testElem1");
            FlowFrequencyPersistenceManager manager = createPersistenceManager();
            RemoveTest(DBNAME, elem, manager);

        }


        private FlowFrequencyPersistenceManager createPersistenceManager()
        {
            FDACache cache = FDACache.Create();
            return new FlowFrequencyPersistenceManager(cache);
        }

        private AnalyticalFrequencyElement createExteriorInteriorElement(String name)
        {
            String lastEditDate = "lastEditDate";
            double mean = 1;
            double stdev = 1;
            double skew = 1;
            int n = 100;
            return new AnalyticalFrequencyElement(name, lastEditDate, "desc", new Statistics.LogPearsonIII(mean, stdev, skew, n));
        }
    }
}
