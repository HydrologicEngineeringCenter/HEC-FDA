using System;
using FdaViewModel.FlowTransforms;
using FdaViewModel.Saving.PersistenceManagers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Statistics;

namespace FDAUnitTests.Saving.PersistenceManagers
{
    [TestClass]
    public class InflowOutflowPersistenceManagerTests : PersistenceManagersBaseTest
    {
        private const String DBNAME = "InflowOutflow";

        [TestMethod]
        public void SaveNewElemTest()
        {
            InflowOutflowElement elem = createInflowOutflowElement("testElem5");
            InflowOutflowPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetInflowOutflowManager();
            SaveNewTest(DBNAME, elem, manager);

        }

        [TestMethod]
        public void SaveExistingElemTest()
        {
            InflowOutflowElement originalElem = createInflowOutflowElement("testElem4");
            InflowOutflowPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetInflowOutflowManager();
            SaveExistingTest(DBNAME, originalElem, manager);

        }

        [TestMethod]
        public void UndoElemTest()
        {
            InflowOutflowElement originalElem = createInflowOutflowElement("testElem3");
            InflowOutflowPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetInflowOutflowManager();
            UndoTest(DBNAME, originalElem, manager);

        }

        [TestMethod]
        public void RedoElemTest()
        {
            InflowOutflowElement originalElem = createInflowOutflowElement("testElem2");
            InflowOutflowPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetInflowOutflowManager();
            RedoTest(DBNAME, originalElem, manager);

        }

        [TestMethod]
        public void RemoveElementTest()
        {
            InflowOutflowElement elem = createInflowOutflowElement("testElem1");
            InflowOutflowPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetInflowOutflowManager();
            RemoveTest(DBNAME, elem, manager);

        }

        private InflowOutflowElement createInflowOutflowElement(String name)
        {
            String lastEditDate = "lastEditDate";
            double[] xs = new double[] { 0, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000 };
            //double[] ys = new double[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            Statistics.ContinuousDistribution[] yValues = new Statistics.ContinuousDistribution[] 
            {
                new None(2000), new None(3000), new None(4000), new None(5000), new None(6000), new None(7000),
                new None(8000), new None(9000), new None(10000), new None(11000)
            };



            UncertainCurveDataCollection curve = new Statistics.UncertainCurveIncreasing(xs, yValues, true, true,
                UncertainCurveDataCollection.DistributionsEnum.None);
            return new InflowOutflowElement(name, lastEditDate, "desc", curve);
        }
    }
}
