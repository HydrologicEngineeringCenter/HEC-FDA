using Importer;
using System.Collections.Generic;
using ViewModel.Saving.PersistenceManagers;
using ViewModel.Utilities;
using static Importer.AsciiImport;

namespace ViewModel.GeoTech
{
    public class ImportLeveeElementFromFDA1VM : ImportFromFDA1VM
    {
        public ImportLeveeElementFromFDA1VM() : base()
        {

        }

        public override ImportOptions GetImportOptions()
        {
            return ImportOptions.ImportLevees;
        }

        public override void SaveElements()
        {
            LeveePersistenceManager manager = Saving.PersistenceFactory.GetLeveeManager();
            foreach (LeveeFeatureElement elem in ElementsToImport)
            {
                manager.SaveNew(elem);
            }
        }

        public override void CreateElements(bool checkForNameConflict = true)
        {
            LeveeList leveeList = GlobalVariables.mp_fdaStudy.GetLeveeList();
            string message = "";
            ElementsToImport.AddRange(ImportFromFDA1Helper.CreateLeveeElements(leveeList, ref message));

            if (checkForNameConflict)
            {
                List<ChildElement> existingElems = StudyCache.GetChildElementsOfType(typeof(LeveeFeatureElement));
                FdaValidationResult vr = CheckForDuplicateNames(ElementsToImport, existingElems);
                if (!vr.IsValid)
                {
                    ImportLog += vr.ErrorMessage;
                }
            }
        }

    }
}
