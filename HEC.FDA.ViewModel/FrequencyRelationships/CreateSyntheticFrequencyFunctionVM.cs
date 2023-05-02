using HEC.FDA.Model.utilities;
using HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using Importer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    public class CreateSyntheticFrequencyFunctionVM : BaseViewModel
    {
        private string _ImportLog = "";
        private string _Path;
        private bool _ImportEnabled;

        private List<ChildElement> ElementsToImport { get; } = new List<ChildElement>();

        public bool ImportEnabled
        {
            get { return _ImportEnabled; }
            set { _ImportEnabled = value; NotifyPropertyChanged(); }
        }

        public string ImportLog
        {
            get { return _ImportLog; }
            set { _ImportLog = value; NotifyPropertyChanged(); }
        }
        public string Path
        {
            get { return _Path; }
            set { _Path = value; ImportEnabled = true; NotifyPropertyChanged(); }
        }

        public CreateSyntheticFrequencyFunctionVM()
        {
            AddRule(nameof(Path), () => !string.IsNullOrWhiteSpace(Path), "Import file path cannot be blank or whitespace.");
            Validate();
        }

        public void Import()
        {
            if (Path != null && System.IO.Path.GetExtension(Path).ToUpper() != ".DBF")
            {
                MessageBox.Show("Selected file must have the extension *.dbf", "Invalid Extension", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                dbfreader dbr = new dbfreader(Path);

                ImportLog = "";
                ElementsToImport.Clear();

                AsyncLogger logger = new AsyncLogger();

                IElementManager freqPersistenceManager = Saving.PersistenceFactory.GetElementManager<FrequencyElement>();
                int id = freqPersistenceManager.GetNextAvailableId();
                int numRows = dbr.NumberOfRows;
                ImportLog += "Reading " + numRows + " rows:\n";

                for (int i = 0; i < numRows; i++)
                {
                    double mean = (double)dbr.GetCell("LOG_MEAN", i);
                    double stdev = (double)dbr.GetCell("STD_DEV", i);
                    double skew = (double)dbr.GetCell("SKEW", i);
                    int por = (int)dbr.GetCell("REC_LENGTH", i);

                    string name = (string)dbr.GetCell("NM_PROBFU", i);
                    string description = (string)dbr.GetCell("DE_PROBFU", i);

                    if (IsRowValid(mean, stdev, skew))
                    {
                        ElementsToImport.Add(CreateFrequencyElement(name, description, mean, stdev, skew, por, id));
                        id++;
                    }
                    else
                    {
                        ImportLog += "Ignoring row with invalid data (-901)\n";
                    }
                }

                //check name conflicts
                List<ChildElement> existingElems = StudyCache.GetChildElementsOfType(typeof(FrequencyElement));
                FdaValidationResult duplicateNamesResult = CheckForDuplicateNames(ElementsToImport, existingElems);
                if (!duplicateNamesResult.IsValid)
                {
                    ImportLog += duplicateNamesResult.ErrorMessage + Environment.NewLine;
                }

                //save
                foreach (ChildElement elem in ElementsToImport)
                {
                    freqPersistenceManager.SaveNew(elem);
                    ImportLog += "\nSaved new frequency element: " + elem.Name + Environment.NewLine;
                }

                ImportLog += "\nImport Completed\n";
            }
        }

        private bool IsRowValid(double mean, double stDev, double skew)
        {
            bool isValid = true;
            if(mean == -901 || stDev == -901 || skew == -901)
            {
                isValid = false;
            }
            return isValid;
        }

        private FrequencyElement CreateFrequencyElement(string name, string description, double mean, double stDev, double skew, int por, int id)
        {
            string lastEditDate = DateTime.Now.ToString("G");
            FrequencyEditorVM vm = new();
            vm.IsGraphical = false;
            vm.AnalyticalVM.IsFitToFlows = false;
            vm.AnalyticalVM.ParameterEntryVM.LP3Distribution = new Statistics.Distributions.LogPearson3(mean, stDev, skew, por);

            FrequencyElement newFreqElem = new FrequencyElement(name, lastEditDate, description, id, vm);
            return newFreqElem; 
        }

        private List<double> CreateDefaultFitToFlowValues()
        {
            List<double> deafaultFitToFlows = new List<double>();
            for (int i = 1; i < 11; i++)
            {
                deafaultFitToFlows.Add(i * 1000);
            }
            return deafaultFitToFlows;
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
                        vr.AddErrorMessage("\nAn element with name '" + currentName + "' already exists.\nChanging the importing element to name '" + newName + "'.");
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
