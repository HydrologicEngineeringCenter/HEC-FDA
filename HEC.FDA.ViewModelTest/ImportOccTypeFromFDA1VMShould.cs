﻿using HEC.FDA.ModelTest.Resources;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.Utilities;
using Importer;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Importer.AsciiImport;

namespace HEC.FDA.ViewModelTest
{
    [Trait("RunsOn", "Remote")]
    [Collection("Serial")]
    public class ImportOccTypeFromFDA1VMShould
    {

        public AsciiImport _Importer;

        [Fact]
        public void ImportOccTypesFromASCII()
        {
            string file = StringResourcePaths.PathToOccupancyTypes;
            var thing = Import(file);
            Assert.NotNull(thing);
        }

        public OccupancyTypesElement Import(string file)
        {
            AsciiImport importer = new AsciiImport(new AsyncLogger());
            importer.ImportAsciiData(file, ImportOptions.ImportOcctypes);
            OccupancyTypeList occupancyTypeList = GlobalVariables.mp_fdaStudy.GetOccupancyTypeList();
            string groupName = System.IO.Path.GetFileNameWithoutExtension(file);
            string messages = "";
            return (OccupancyTypesElement)ImportFromFDA1Helper.CreateOcctypes(occupancyTypeList, groupName, ref messages, 0);
        }

    }
}
