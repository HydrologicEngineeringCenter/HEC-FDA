using Importer;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Utilities;
using static Importer.AsciiImport;

namespace HEC.FDA.ViewModel.StageTransforms
{
    public class ImportRatingsFromFDA1VM : ImportFromFDA1VM
    {
        public ImportRatingsFromFDA1VM():base()
        {

        }

        public override ImportOptions GetImportOptions()
        {
            return ImportOptions.ImportRatings;
        }
        public override void SaveElements()
        {          
            Saving.PersistenceManagers.StageDischargePersistenceManager manager = Saving.PersistenceFactory.GetRatingManager();
            foreach(StageDischargeElement elem in ElementsToImport)
            {
                manager.SaveNew(elem);
            }
        }

        public override void CreateElements(bool checkForNameConflict = true)
        {
            RatingFunctionList ratings = GlobalVariables.mp_fdaStudy.GetRatingFunctionList();
            ElementsToImport.AddRange(ImportFromFDA1Helper.CreateRatingElements(ratings));

            if (checkForNameConflict)
            {
                List<ChildElement> existingElems = StudyCache.GetChildElementsOfType(typeof(StageDischargeElement));
                FdaValidationResult vr = CheckForDuplicateNames(ElementsToImport, existingElems);
                if (!vr.IsValid)
                {
                    ImportLog += vr.ErrorMessage;
                }
            }
        }

    }
}
