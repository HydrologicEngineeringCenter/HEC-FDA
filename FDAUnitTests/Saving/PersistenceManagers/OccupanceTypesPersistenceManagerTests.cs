using System;
using System.Collections.Generic;
using Consequences_Assist.ComputableObjects;
using FdaViewModel.Inventory.OccupancyTypes;
using FdaViewModel.Saving.PersistenceManagers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FDAUnitTests.Saving.PersistenceManagers
{
    [TestClass]
    public class OccupanceTypesPersistenceManagerTests : PersistenceManagersBaseTest
    {
        private const String DBNAME = "OccType";

        /// <summary>
        /// Save elem1 to db. Assert elem1 is in db.
        /// </summary>
        [TestMethod]
        public void SaveNewElemTest()
        {
            OccupancyTypesElement elem = createOccTypeElement("testElem5");
            OccTypePersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetOccTypeManager();
            SaveNewTest(DBNAME, elem, manager);

        }

        /// <summary>
        /// Saves elem1 to db. Makes a clone of the elem1 and change the name.
        /// Assert elem2 is in db. Assert elem1 is not in db.
        /// </summary>
        [TestMethod]
        public void SaveExistingElemTest()
        {
            OccupancyTypesElement originalElem = createOccTypeElement("testElem4");
            OccTypePersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetOccTypeManager();
            SaveExistingTest(DBNAME, originalElem, manager);

        }

        
        /// <summary>
        /// Save elem1 to db. Assert elem1 in in db. Remove elem1. Assert elem1 is not in db.
        /// </summary>
        [TestMethod]
        public void RemoveElementTest()
        {
            OccupancyTypesElement elem = createOccTypeElement("testElem1");
            OccTypePersistenceManager manager = FdaViewModel.Saving.PersistenceFactory.GetOccTypeManager();
            RemoveTest(DBNAME, elem, manager);

        }

        private OccupancyTypesElement createOccTypeElement(String name)
        {
            string occTypesGroupName = "OccTypeGroup" + name;
            List<OccupancyType> listOfOccTypes = new List<OccupancyType>();
            OccupancyType ot = new OccupancyType("occTypeName", "DamCatName");
            listOfOccTypes.Add(ot);
            Dictionary<string, bool[]> dummyDictionary = new Dictionary<string, bool[]>();
            dummyDictionary.Add(ot.Name, new bool[] { true, true, true, false });
            return new OccupancyTypesElement(occTypesGroupName, listOfOccTypes, dummyDictionary);
        }
    }
}
