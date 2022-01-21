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

namespace ViewModel.Inventory.OccupancyTypes
{
    public class ImportOcctypesFromFDA1VM : ImportFromFDA1VM
    {
        public ImportOcctypesFromFDA1VM() : base()
        {

        }

        public override ImportOptions GetImportOptions()
        {
            return ImportOptions.ImportOcctypes;
        }
        public override void SaveElements()
        {

            OccTypePersistenceManager manager = Saving.PersistenceFactory.GetOccTypeManager();
            foreach (OccupancyTypesElement elem in ElementsToImport)
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
        //    Task task = Task.Run(() => import.ImportAsciiData(Path, AsciiImport.ImportOptions.ImportOcctypes));

        //    Timer timer = new Timer(500, 100, true);
        //    timer.Tick += () => ImportLog += logger.PopLastMessages();

        //    task.ContinueWith(t =>
        //    {
        //        timer.Stop();
        //        ImportLog += logger.PopLastMessages();

        //        OccupancyTypeList occupancyTypeList = GlobalVariables.mp_fdaStudy.GetOccupancyTypeList();
        //        ElementsToImport.Add(ImportFromFDA1Helper.CreateOcctypes(occupancyTypeList, import._FileName));

        //        FdaValidationResult result = new FdaValidationResult();
        //        List<ChildElement> existingElems = StudyCache.GetChildElementsOfType(typeof(OccupancyTypesElement));
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
            OccupancyTypeList occupancyTypeList = GlobalVariables.mp_fdaStudy.GetOccupancyTypeList();
            string groupName = System.IO.Path.GetFileNameWithoutExtension(Path);
            ElementsToImport.Add(ImportFromFDA1Helper.CreateOcctypes(occupancyTypeList, groupName));

            if (checkForNameConflict)
            {
                List<ChildElement> existingElems = StudyCache.GetChildElementsOfType(typeof(OccupancyTypesElement));
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
