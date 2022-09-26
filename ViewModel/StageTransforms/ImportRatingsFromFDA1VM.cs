using Importer;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Utilities;
using static Importer.AsciiImport;
using HEC.FDA.ViewModel.Saving;

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
        public override void Save()
        {          
            IElementManager manager = PersistenceFactory.GetElementManager<StageDischargeElement>();
            foreach(StageDischargeElement elem in ElementsToImport)
            {               
                manager.SaveNew(elem);
            }
            HasChanges = false;
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
