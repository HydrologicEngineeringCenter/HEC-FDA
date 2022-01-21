using HEC.CS.Threading;
using Importer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Threading;
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
            return ImportOptions.ImportFailureFunction;
        }

        public override void SaveElements()
        {
            LeveePersistenceManager manager = Saving.PersistenceFactory.GetLeveeManager();
            foreach (LeveeFeatureElement elem in ElementsToImport)
            {
                manager.SaveNew(elem);
            }
        }

        //public override void Validate(Action<FdaValidationResult> validationCallback)
        //{
        //    ElementsToImport.Clear();
        //    Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        //    AsyncLogger logger = new AsyncLogger();
        //    AsciiImport import = new AsciiImport(logger);//pass in the logger.
        //    //put on background
        //    Task task = Task.Run(() => import.ImportAsciiData(Path, AsciiImport.ImportOptions.ImportFailureFunction));

        //    Timer timer = new Timer(500, 100, true);
        //    timer.Tick += () => ImportLog += logger.PopLastMessages();

        //    task.ContinueWith(t =>
        //    {
        //        timer.Stop();
        //        ImportLog += logger.PopLastMessages();

        //        LeveeList leveeList = GlobalVariables.mp_fdaStudy.GetLeveeList();
        //        string message = "";
        //        ElementsToImport.AddRange(ImportFromFDA1Helper.CreateLeveeElements(leveeList, ref message));

        //        FdaValidationResult result = new FdaValidationResult();
        //        List<ChildElement> existingElems = StudyCache.GetChildElementsOfType(typeof(LeveeFeatureElement));
        //        FdaValidationResult vr = CheckForDuplicateNames(ElementsToImport, existingElems);
        //        if (!vr.IsValid)
        //        {
        //            ImportLog += vr.ErrorMessage;
        //        }

        //        dispatcher.BeginInvoke(validationCallback, result);
        //    });
        //}

        public override List<ChildElement> CreateElements(bool checkForNameConflict = true)
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
            return ElementsToImport;
        }

    }
}
