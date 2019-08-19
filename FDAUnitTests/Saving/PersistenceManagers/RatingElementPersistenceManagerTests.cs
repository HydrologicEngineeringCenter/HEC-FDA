using System;
using FdaViewModel.Saving.PersistenceManagers;
using FdaViewModel.StageTransforms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Statistics;

namespace FDAUnitTests.Saving.PersistenceManagers
{
    [TestClass]
    public class RatingElementPersistenceManagerTests : PersistenceManagersBaseTest
    {
        private const String DBNAME = "Rating";

        [TestMethod]
        public void SaveNewElemTest()
        {
            RatingCurveElement elem = createInflowOutflowElement("testElem5");
            RatingElementPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetRatingManager();
            SaveNewTest(DBNAME, elem, manager);

        }

        [TestMethod]
        public void SaveExistingElemTest()
        {
            RatingCurveElement originalElem = createInflowOutflowElement("testElem4");
            RatingElementPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetRatingManager();
            SaveExistingTest(DBNAME, originalElem, manager);

        }

        [TestMethod]
        public void UndoElemTest()
        {
            RatingCurveElement originalElem = createInflowOutflowElement("testElem3");
            RatingElementPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetRatingManager();
            UndoTest(DBNAME, originalElem, manager);

        }

        [TestMethod]
        public void RedoElemTest()
        {
            RatingCurveElement originalElem = createInflowOutflowElement("testElem2");
            RatingElementPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetRatingManager();
            RedoTest(DBNAME, originalElem, manager);

        }

        [TestMethod]
        public void RemoveElementTest()
        {
            RatingCurveElement elem = createInflowOutflowElement("testElem1");
            RatingElementPersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetRatingManager();
            RemoveTest(DBNAME, elem, manager);

        }

        private RatingCurveElement createInflowOutflowElement(String name)
        {
            String lastEditDate = "lastEditDate";
            double[] xValues = new double[] { 1000, 10000, 15000, 17600, 19500, 28000, 30000, 50000, 74000, 105250, 128500, 158600 };
            ContinuousDistribution[] yValues = new ContinuousDistribution[] 
            {
                new None(95), new None(96), new None(97), new None(99), new None(104), new None(109), new None(110),
                new None(114), new None(116), new None(119), new None(120), new None(121)
            };

            UncertainCurveDataCollection curve = new UncertainCurveIncreasing(xValues, yValues, true, true,
                UncertainCurveDataCollection.DistributionsEnum.None);
            return new RatingCurveElement(name, lastEditDate, "desc", curve);
        }
    }
}
