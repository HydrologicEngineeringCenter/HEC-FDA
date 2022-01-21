using HEC.CS.Threading;
using Importer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using ViewModel.Saving.PersistenceManagers;
using ViewModel.Utilities;
using static Importer.AsciiImport;
using static Importer.ProbabilityFunction;

namespace ViewModel.FlowTransforms
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
            InflowOutflowPersistenceManager manager = Saving.PersistenceFactory.GetInflowOutflowManager();
            foreach (InflowOutflowElement elem in ElementsToImport)
            {
                manager.SaveNew(elem);
            }
        }

        //public override void Validate(Action<FdaValidationResult> validationCallback)
        //{
        //    Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        //    AsyncLogger logger = new AsyncLogger();
        //    AsciiImport import = new AsciiImport(logger);//pass in the logger.
        //    //put on background
        //    Task task = Task.Run(() => import.ImportAsciiData(Path, AsciiImport.ImportOptions.ImportInflowOutflow));

        //    Timer timer = new Timer(500, 100, true);
        //    timer.Tick += () => ImportUpdates += logger.PopLastMessages();

        //    task.ContinueWith(t => {
        //        timer.Stop();
        //        ImportUpdates += logger.PopLastMessages();

        //        string messages = "";
        //        ElementsToImport.AddRange(CreateInflowOutflow(ref messages));
        //        ImportUpdates += messages;

        //        FdaValidationResult result = new FdaValidationResult();
        //        dispatcher.BeginInvoke(validationCallback, result);
        //    });
        //}

        public override List<ChildElement> CreateElements(bool checkForNameConflict = true)
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
            return ElementsToImport;
        }

    }
}
