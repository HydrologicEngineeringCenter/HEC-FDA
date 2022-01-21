using Importer;
using System.Collections.Generic;
using ViewModel.Saving.PersistenceManagers;
using ViewModel.Utilities;
using static Importer.AsciiImport;

namespace ViewModel.StageTransforms
{
    public class ImportExteriorInteriorFromFDA1VM : ImportFromFDA1VM
    {
        public ImportExteriorInteriorFromFDA1VM() : base()
        {

        }

        public override ImportOptions GetImportOptions()
        {
            return ImportOptions.ImportExteriorInterior;
        }
        public override void SaveElements()
        {
            ExteriorInteriorPersistenceManager manager = Saving.PersistenceFactory.GetExteriorInteriorManager();
            foreach (RatingCurveElement elem in ElementsToImport)
            {
                manager.SaveNew(elem);
            }
        }

        public override void CreateElements(bool checkForNameConflict = true)
        {
            LeveeList leveeList = GlobalVariables.mp_fdaStudy.GetLeveeList();
            ElementsToImport.AddRange(ImportFromFDA1Helper.CreateExteriorInteriors(leveeList));

            if (checkForNameConflict)
            {
                List<ChildElement> existingElems = StudyCache.GetChildElementsOfType(typeof(ExteriorInteriorElement));
                FdaValidationResult vr = CheckForDuplicateNames(ElementsToImport, existingElems);
                if (!vr.IsValid)
                {
                    ImportLog += vr.ErrorMessage;
                }
            }
        }

    }
}
