using Importer;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Utilities;
using static Importer.AsciiImport;
using HEC.FDA.ViewModel.Saving;

namespace HEC.FDA.ViewModel.StageTransforms
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
        public override void Save()
        {
            IElementManager manager = PersistenceFactory.GetElementManager<ExteriorInteriorElement>();
            foreach (ExteriorInteriorElement elem in ElementsToImport)
            {
                manager.SaveNew(elem);
            }
            HasChanges = false;
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
