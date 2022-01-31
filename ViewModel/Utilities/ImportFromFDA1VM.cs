using HEC.CS.Threading;
using Importer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using static Importer.AsciiImport;

namespace ViewModel.Utilities
{
    public abstract class ImportFromFDA1VM : BaseViewModel
    {
        private string _ImportLog = "";
        public AsciiImport _Importer;
        public List<ChildElement> ElementsToImport { get; } = new List<ChildElement>();

        public string ImportLog
        {
            get { return _ImportLog; }
            set { _ImportLog = value; NotifyPropertyChanged(); }
        }
        public string Path { get; set; }
        public ImportFromFDA1VM() 
        {
            //i have to set the name to something to get past the default validation
            Name = "myName";
        }

        public abstract void SaveElements();

        //public abstract void Validate(Action<FdaValidationResult> validationResult);
        public abstract void CreateElements(bool checkForNameConflict = true);
        public abstract ImportOptions GetImportOptions();

        public virtual void Import()
        {
            ImportLog = "";
            ElementsToImport.Clear();

            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            AsyncLogger logger = new AsyncLogger();
            _Importer = new AsciiImport(logger);//pass in the logger.
            //put on background
            Task task = Task.Run(() => _Importer.ImportAsciiData(Path, GetImportOptions()));

            Timer timer = new Timer(500, 100, true);
            timer.Tick += () => ImportLog += logger.PopLastMessages();

            task.ContinueWith(t => 
            {
                timer.Stop();
                ImportLog += logger.PopLastMessages();

                CreateElements();

                FdaValidationResult result = new FdaValidationResult();
                dispatcher.Invoke(new Action(() =>
                {
                    SaveElements();
                }));               
            });
        }

        /// <summary>
        /// Checks if the new element names match the existing element names. If they do, then i return the appropriate 
        /// message in the FdaValidationResult.
        /// </summary>
        /// <param name="newElems"></param>
        /// <param name="existingElems"></param>
        /// <returns></returns>
        public static FdaValidationResult CheckForDuplicateNames(List<ChildElement> newElems, List<ChildElement> existingElems)
        {
            FdaValidationResult vr = new FdaValidationResult();
            foreach (ChildElement elem in newElems)
            {
                bool hasDuplicate = true;
                int i = 2;
                while (hasDuplicate)
                {
                    IEnumerable<ChildElement> matches = existingElems.Where(item => item.Name.Equals(elem.Name));
                    if (matches.Count() > 0)
                    {
                        string currentName = elem.Name;
                        string newName = elem.Name + "_" + i;
                        //we have a duplicate.
                        elem.Name = newName;
                        elem.UpdateTreeViewHeader(elem.Name);
                        i++;
                        vr.AddErrorMessage("\nAn element with name '" + currentName + "' already exists.\nChanging the importing element to name '" +
                            newName + "'.");
                    }
                    else
                    {
                        hasDuplicate = false;
                    }
                }
            }
            return vr;
        }
    }
}
