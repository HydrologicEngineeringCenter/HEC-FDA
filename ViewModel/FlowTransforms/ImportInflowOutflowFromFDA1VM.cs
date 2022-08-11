using Importer;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Utilities;
using static Importer.AsciiImport;
using HEC.FDA.ViewModel.Saving;

namespace HEC.FDA.ViewModel.FlowTransforms
{
    public class ImportInflowOutflowFromFDA1VM : ImportFromFDA1VM
    {
        public ImportInflowOutflowFromFDA1VM() : base()
        {

        }

        public override ImportOptions GetImportOptions()
        {
            return ImportOptions.ImportInflowOutflow;
        }

        public override void SaveElements()
        {
            IElementManager manager = Saving.PersistenceFactory.GetElementManager<InflowOutflowElement>();
            foreach (InflowOutflowElement elem in ElementsToImport)
            {
                manager.SaveNew(elem);
            }
        }

        public override void CreateElements(bool checkForNameConflict = true)
        {
            ProbabilityFunctionList probFuncs = GlobalVariables.mp_fdaStudy.GetProbabilityFuncList();
            ElementsToImport.AddRange(ImportFromFDA1Helper.CreateInflowOutflowElements(probFuncs));

            if (checkForNameConflict)
            {
                List<ChildElement> existingElems = StudyCache.GetChildElementsOfType(typeof(InflowOutflowElement));
                FdaValidationResult vr = CheckForDuplicateNames(ElementsToImport, existingElems);
                if (!vr.IsValid)
                {
                    ImportLog += vr.ErrorMessage;
                }
            }
        }

    }
}
