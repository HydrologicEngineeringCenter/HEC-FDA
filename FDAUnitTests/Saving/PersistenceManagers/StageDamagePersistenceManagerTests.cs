using System;
using System.Collections.Generic;
using System.IO;
using FdaViewModel.AggregatedStageDamage;
using FdaViewModel.Saving;
using FdaViewModel.Saving.PersistenceManagers;
using FdaViewModel.Study;
using FdaViewModel.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Statistics;

namespace FDAUnitTests.Saving.PersistenceManagers
{
    [TestClass]
    public class StageDamagePersistenceManagerTests : PersistenceManagersBaseTest
    {
        private const String DBNAME = "AggregatedStageDamage";

        [TestMethod]
        public void SaveNewElemTest()
        {
            AggregatedStageDamageElement elem = createAggregatedStageDamageElement("testElem5");
            StageDamagePersistenceManager manager = PersistenceFactory.GetStageDamageManager();

            SaveNewTest(DBNAME, elem, manager);
        }

        [TestMethod]
        public void SaveExistingElemTest()
        {
            AggregatedStageDamageElement originalElem = createAggregatedStageDamageElement("testElem4");
            StageDamagePersistenceManager manager = PersistenceFactory.GetStageDamageManager();

            SaveExistingTest(DBNAME, originalElem, manager);
        }

        [TestMethod]
        public void UndoElemTest()
        {
            AggregatedStageDamageElement originalElem = createAggregatedStageDamageElement("testElem3");
            StageDamagePersistenceManager manager = PersistenceFactory.GetStageDamageManager();

            UndoTest(DBNAME, originalElem, manager);
        }

        [TestMethod]
        public void RedoElemTest()
        {
            AggregatedStageDamageElement originalElem = createAggregatedStageDamageElement("testElem2");
            StageDamagePersistenceManager manager = PersistenceFactory.GetStageDamageManager();

            RedoTest(DBNAME, originalElem, manager);
        }

        [TestMethod]
        public void RemoveElementTest()
        {
            AggregatedStageDamageElement elem = createAggregatedStageDamageElement("testElem1");
            StageDamagePersistenceManager manager = PersistenceFactory.GetStageDamageManager();

            RemoveTest(DBNAME, elem, manager);
        }



        private AggregatedStageDamageElement createAggregatedStageDamageElement(String name)
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
            return new AggregatedStageDamageElement(name, lastEditDate, "desc", curve, CreationMethodEnum.UserDefined);
        }


    }
}
