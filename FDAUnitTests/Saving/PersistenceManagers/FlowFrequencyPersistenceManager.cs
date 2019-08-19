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
            FlowFrequencyPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetFlowFrequencyManager();
            SaveNewTest(DBNAME, elem, manager);

        }

        [TestMethod]
        public void SaveExistingElemTest()
        {
            AnalyticalFrequencyElement originalElem = createExteriorInteriorElement("testElem4");
            FlowFrequencyPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetFlowFrequencyManager();
            SaveExistingTest(DBNAME, originalElem, manager);

        }

        [TestMethod]
        public void UndoElemTest()
        {
            AnalyticalFrequencyElement originalElem = createExteriorInteriorElement("testElem3");
            FlowFrequencyPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetFlowFrequencyManager();
            UndoTest(DBNAME, originalElem, manager);

        }

        [TestMethod]
        public void RedoElemTest()
        {
            AnalyticalFrequencyElement originalElem = createExteriorInteriorElement("testElem2");
            FlowFrequencyPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetFlowFrequencyManager();
            RedoTest(DBNAME, originalElem, manager);

        }

        [TestMethod]
        public void RemoveElementTest()
        {
            AnalyticalFrequencyElement elem = createExteriorInteriorElement("testElem1");
            FlowFrequencyPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetFlowFrequencyManager();
            RemoveTest(DBNAME, elem, manager);

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
