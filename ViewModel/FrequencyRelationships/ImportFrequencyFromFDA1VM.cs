using Importer;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Utilities;
using static Importer.AsciiImport;
using HEC.FDA.ViewModel.Saving;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class ImportFrequencyFromFDA1VM : ImportFromFDA1VM
    {
        public ImportFrequencyFromFDA1VM() : base()
        {

        }
        public override ImportOptions GetImportOptions()
        {
            return ImportOptions.ImportFrequency;
        }

        public override void Save()
        {
            IElementManager manager = Saving.PersistenceFactory.GetElementManager<AnalyticalFrequencyElement>();
            foreach (AnalyticalFrequencyElement elem in ElementsToImport)
            {
                manager.SaveNew(elem);
            }
            HasChanges = false;
        }

        public override void CreateElements(bool checkForNameConflict = true)
        {
            ProbabilityFunctionList probFuncs = GlobalVariables.mp_fdaStudy.GetProbabilityFuncList();
            ElementsToImport.AddRange(ImportFromFDA1Helper.CreateFlowFrequencyElements(probFuncs));

            if (checkForNameConflict)
            {
                List<ChildElement> existingElems = StudyCache.GetChildElementsOfType(typeof(AnalyticalFrequencyElement));
                FdaValidationResult vr = CheckForDuplicateNames(ElementsToImport, existingElems);
                if (!vr.IsValid)
                {
                    ImportLog += vr.ErrorMessage;
                }
            }
        }

    }
}
