using System;
using FdaViewModel.Saving.PersistenceManagers;
using FdaViewModel.StageTransforms;
using FdaViewModel.Study;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Statistics;

namespace FDAUnitTests.Saving.PersistenceManagers
{
    [TestClass]
    public class ExteriorInteriorPersistenceManagerTest: PersistenceManagersBaseTest
    {
        private const String DBNAME = "ExteriorInterior";

        [TestMethod]
        public void SaveNewElemTest()
        {
            ExteriorInteriorElement elem = createExteriorInteriorElement("testElem5");
            ExteriorInteriorPersistenceManager manager = createPersistenceManager();
            SaveNewTest(DBNAME, elem, manager);

        }

        [TestMethod]
        public void SaveExistingElemTest()
        {
            ExteriorInteriorElement originalElem = createExteriorInteriorElement("testElem4");
            ExteriorInteriorPersistenceManager manager = createPersistenceManager();
            SaveExistingTest(DBNAME, originalElem, manager);

        }

        [TestMethod]
        public void UndoElemTest()
        {
            ExteriorInteriorElement originalElem = createExteriorInteriorElement("testElem3");
            ExteriorInteriorPersistenceManager manager = createPersistenceManager();
            UndoTest(DBNAME, originalElem, manager);

        }

        [TestMethod]
        public void RedoElemTest()
        {
            ExteriorInteriorElement originalElem = createExteriorInteriorElement("testElem2");
            ExteriorInteriorPersistenceManager manager = createPersistenceManager();
            RedoTest(DBNAME, originalElem, manager);

        }

        [TestMethod]
        public void RemoveElementTest()
        {
            ExteriorInteriorElement elem = createExteriorInteriorElement("testElem1");
            ExteriorInteriorPersistenceManager manager = createPersistenceManager();
            RemoveTest(DBNAME, elem, manager);

        }


        private ExteriorInteriorPersistenceManager createPersistenceManager()
        {
            FDACache cache = FDACache.Create();
            return new ExteriorInteriorPersistenceManager(cache);
        }

        private ExteriorInteriorElement createExteriorInteriorElement(String name)
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
            return new ExteriorInteriorElement(name, lastEditDate, "desc", curve);
        }

    }
}
