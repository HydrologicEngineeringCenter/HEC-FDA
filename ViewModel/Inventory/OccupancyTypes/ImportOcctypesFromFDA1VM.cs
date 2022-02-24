using Importer;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Utilities;
using static Importer.AsciiImport;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class ImportOcctypesFromFDA1VM : ImportFromFDA1VM
    {
        public ImportOcctypesFromFDA1VM() : base()
        {

        }

        public override ImportOptions GetImportOptions()
        {
            return ImportOptions.ImportOcctypes;
        }
        public override void SaveElements()
        {
            OccTypePersistenceManager manager = Saving.PersistenceFactory.GetOccTypeManager();
            foreach (OccupancyTypesElement elem in ElementsToImport)
            {
                manager.SaveNew(elem);
            }
        }

        public override void CreateElements(bool checkForNameConflict = true)
        {
            OccupancyTypeList occupancyTypeList = GlobalVariables.mp_fdaStudy.GetOccupancyTypeList();
            string groupName = System.IO.Path.GetFileNameWithoutExtension(Path);
            string messages = "";
            ElementsToImport.Add(ImportFromFDA1Helper.CreateOcctypes(occupancyTypeList, groupName, ref messages));
            ImportLog += messages;
            if (checkForNameConflict)
            {
                List<ChildElement> existingElems = StudyCache.GetChildElementsOfType(typeof(OccupancyTypesElement));
                FdaValidationResult vr = CheckForDuplicateNames(ElementsToImport, existingElems);
                if (!vr.IsValid)
                {
                    ImportLog += vr.ErrorMessage;
                }
            }
        }

    }
}
