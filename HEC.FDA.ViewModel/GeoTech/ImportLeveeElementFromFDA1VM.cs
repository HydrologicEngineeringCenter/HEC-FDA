using Importer;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Utilities;
using static Importer.AsciiImport;
using HEC.FDA.ViewModel.Saving;

namespace HEC.FDA.ViewModel.GeoTech
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

        public override void Save()
        {
            IElementManager manager = PersistenceFactory.GetElementManager<LateralStructureElement>();
            foreach (LateralStructureElement elem in ElementsToImport)
            {
                manager.SaveNew(elem);
            }
            HasChanges = false;
        }

        public override void CreateElements(bool checkForNameConflict = true)
        {
            LeveeList leveeList = GlobalVariables.mp_fdaStudy.GetLeveeList();
            string message = "";
            ElementsToImport.AddRange(ImportFromFDA1Helper.CreateLeveeElements(leveeList, ref message));

            if (checkForNameConflict)
            {
                List<ChildElement> existingElems = StudyCache.GetChildElementsOfType(typeof(LateralStructureElement));
                FdaValidationResult vr = CheckForDuplicateNames(ElementsToImport, existingElems);
                if (!vr.IsValid)
                {
                    ImportLog += vr.ErrorMessage;
                }
            }
        }

    }
}
