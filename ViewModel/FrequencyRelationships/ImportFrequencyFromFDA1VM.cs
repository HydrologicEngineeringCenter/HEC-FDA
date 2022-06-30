using Importer;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Utilities;
using static Importer.AsciiImport;

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

        public override void SaveElements()
        {
            Saving.PersistenceManagers.FlowFrequencyPersistenceManager manager = Saving.PersistenceFactory.GetFlowFrequencyManager();
            foreach (FrequencyElement elem in ElementsToImport)
            {
                manager.SaveNew(elem);
            }
        }

        public override void CreateElements(bool checkForNameConflict = true)
        {
            ProbabilityFunctionList probFuncs = GlobalVariables.mp_fdaStudy.GetProbabilityFuncList();
            ElementsToImport.AddRange(ImportFromFDA1Helper.CreateFlowFrequencyElements(probFuncs));

            if (checkForNameConflict)
            {
                List<ChildElement> existingElems = StudyCache.GetChildElementsOfType(typeof(FrequencyElement));
                FdaValidationResult vr = CheckForDuplicateNames(ElementsToImport, existingElems);
                if (!vr.IsValid)
                {
                    ImportLog += vr.ErrorMessage;
                }
            }
        }

    }
}
