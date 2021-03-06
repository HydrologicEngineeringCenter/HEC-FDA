using Importer;
using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Utilities;
using static Importer.AsciiImport;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class ImportStageDamageFromFDA1VM : ImportFromFDA1VM
    {
        public ImportStageDamageFromFDA1VM() : base()
        {

        }

        public override ImportOptions GetImportOptions()
        {
            return ImportOptions.ImportStageDamages;
        }
        public override void SaveElements()
        {
            StageDamagePersistenceManager manager = Saving.PersistenceFactory.GetStageDamageManager();
            foreach (AggregatedStageDamageElement elem in ElementsToImport)
            {
                manager.SaveNew(elem);
            }
        }

        public override void CreateElements(bool checkForNameConflict = true)
        {
            AggregateDamageFunctionList aggDamageList = GlobalVariables.mp_fdaStudy.GetAggDamgFuncList();
            List<ImpactAreaElement> impAreaElems = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            string additionalMessages = "";
            ElementsToImport.AddRange(ImportFromFDA1Helper.ImportStageDamages(aggDamageList, impAreaElems, ref additionalMessages));

            ImportLog += additionalMessages;

            if (checkForNameConflict)
            {
                List<ChildElement> existingElems = StudyCache.GetChildElementsOfType(typeof(AggregatedStageDamageElement));
                FdaValidationResult vr = CheckForDuplicateNames(ElementsToImport, existingElems);
                if (!vr.IsValid)
                {
                    ImportLog += vr.ErrorMessage;
                }
            }
        }

    }
}
