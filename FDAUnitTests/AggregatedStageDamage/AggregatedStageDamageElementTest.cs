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

namespace FDAUnitTests.AggregatedStageDamage
{
    [TestClass]
    public class AggregatedStageDamageElementTest
    {

        [TestInitialize]
        public void Initialize()
        {

        }

        [TestMethod]
        public void CtorTest()
        {
            createAggregatedStageDamageElement("testElem4");
            AggregatedStageDamageElement asde2 = new AggregatedStageDamageElement(null, null, null, null, CreationMethodEnum.UserDefined);

        }

        [TestMethod]
        public void CloneElementTest()
        {
            AggregatedStageDamageElement elem = createAggregatedStageDamageElement("testElem3");
            AggregatedStageDamageElement elem2 = (AggregatedStageDamageElement) elem.CloneElement(elem);

            Assert.IsTrue(elem.Equals(elem2));


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

