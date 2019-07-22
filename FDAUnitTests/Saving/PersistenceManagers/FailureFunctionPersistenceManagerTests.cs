using System;
using FdaViewModel.GeoTech;
using FdaViewModel.Saving.PersistenceManagers;
using FdaViewModel.Study;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Statistics;

namespace FDAUnitTests.Saving.PersistenceManagers
{
    [TestClass]
    public class FailureFunctionPersistenceManagerTests : PersistenceManagersBaseTest
    {
        private const String DBNAME = "FailureFunction";

        [TestMethod]
        public void SaveNewElemTest()
        {
            
            FailureFunctionElement elem = createFailureFunctionElement("testElem5");
            FailureFunctionPersistenceManager manager = createPersistenceManager();

            SaveNewTest(DBNAME, elem, manager);
        }

        [TestMethod]
        public void SaveExistingElemTest()
        {
            FailureFunctionElement originalElem = createFailureFunctionElement("testElem4");
            FailureFunctionPersistenceManager manager = createPersistenceManager();

            SaveExistingTest(DBNAME, originalElem, manager);
        }

        [TestMethod]
        public void UndoElemTest()
        {
            FailureFunctionElement originalElem = createFailureFunctionElement("testElem3");
            FailureFunctionPersistenceManager manager = createPersistenceManager();

            UndoTest(DBNAME, originalElem, manager);
           
        }

        [TestMethod]
        public void RedoElemTest()
        {
            FailureFunctionElement originalElem = createFailureFunctionElement("testElem2");
            FailureFunctionPersistenceManager manager = createPersistenceManager();

            RedoTest(DBNAME, originalElem, manager);
        }

        [TestMethod]
        public void RemoveElementTest()
        {
            FailureFunctionElement elem = createFailureFunctionElement("testElem1");
            FailureFunctionPersistenceManager manager = createPersistenceManager();

            RemoveTest(DBNAME, elem, manager);
        }







        private FailureFunctionPersistenceManager createPersistenceManager()
        {
            FDACache cache = FDACache.Create();
            return new FailureFunctionPersistenceManager(cache);
        }

        private FailureFunctionElement createFailureFunctionElement(String name)
        {
            String lastEditDate = "lastEditDate";
            double[] xValues = new double[16] { 95, 97, 99, 101, 103, 105, 107, 110, 112, 115, 117, 120, 122, 125, 127, 130 };
            Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[16]
            {
                new Statistics.None(0), new Statistics.None(0), new Statistics.None(0), new Statistics.None(1), new Statistics.None(3),
                new Statistics.None(10), new Statistics.None(50), new Statistics.None(1000), new Statistics.None(2000), new Statistics.None(4000),
                new Statistics.None(8000), new Statistics.None(10000), new Statistics.None(11000), new Statistics.None(11500),
                new Statistics.None(11750), new Statistics.None(11875)
            };


            UncertainCurveDataCollection curve = new Statistics.UncertainCurveIncreasing(xValues, yValues, true, true,
                Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
            return new FailureFunctionElement(name, lastEditDate, "desc", curve, null);
        }
    }
}
