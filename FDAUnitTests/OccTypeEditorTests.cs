using System;
using System.Collections.Generic;
using FdaViewModel.Inventory.OccupancyTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace FDAUnitTests
{
    [TestClass]
    public class OccTypeEditorTests
    {


        //i need to initialize some sort of connection with a location on the computer for it to write to.

        [TestInitialize]
        public void Initialize()
        {
            if(FdaViewModel.Storage.Connection.Instance.IsConnectionNull)
            {
                
                FdaViewModel.Storage.Connection.Instance.ProjectFile = @"C:\Users\cody\Documents\HEC - FDA\Testing\testing selected occtype.sqlite";
            }
            if (!FdaViewModel.Storage.Connection.Instance.IsOpen)
            {
                FdaViewModel.Storage.Connection.Instance.Open();
            }
        }

        [TestMethod]
        public void TestSaving()
        {
            List<Consequences_Assist.ComputableObjects.OccupancyType> listOfOccTypes = new List<Consequences_Assist.ComputableObjects.OccupancyType>();
            Dictionary<string, bool[]> _OcctypeTabsSelectedDictionary = new Dictionary<string, bool[]>();

            OccupancyTypesElement element = new OccupancyTypesElement("occTypeName", listOfOccTypes, _OcctypeTabsSelectedDictionary);

            //save the element
            FdaViewModel.Saving.PersistenceFactory.GetOccTypeManager().SaveNew(element);
            //read the element in
            List<FdaViewModel.Utilities.ChildElement> elems = FdaViewModel.Saving.PersistenceFactory.GetOccTypeManager().Load();
            //compare the element you saved to the one you read in
            Assert.IsTrue(elems.Count > 0);


        }
    }


    //methods to test

    //CreateElementFromRowData


    //Load

    //SaveExisting(List<ChildElement> elements)


    //public void SaveNew(ChildElement element)

    //public void SaveNewElements(List<ChildElement> elements)
}
