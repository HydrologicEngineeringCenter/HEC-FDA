using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    class OccupancyTypeGroupEditable : NameValidatingVM, IOccupancyTypeGroupEditable
    {
        private bool _IsModified;
        public List<IOccupancyTypeEditable> Occtypes { get; set; }
        public List<IOccupancyTypeEditable> ModifiedOcctypes
        {
            get
            {
                List<IOccupancyTypeEditable> retval = new List<IOccupancyTypeEditable>();
                foreach (IOccupancyTypeEditable ot in Occtypes)
                {
                    if (ot.IsModified)
                    {
                        retval.Add(ot);
                    }
                }
                return retval;
            }
        }       

        public int ID { get; }

        public bool IsModified
        {
            get { return _IsModified; }
            set { _IsModified = value; NotifyPropertyChanged(); }
        }
        public OccupancyTypeGroupEditable(int id, string name, List<IOccupancyTypeEditable> occtypes)
        {
            ID = id;
            Name = name;
            Occtypes = occtypes;
        }

        private List<string> GetOcctypeNames()
        {
            List<string> names = new List<string>();
            foreach(IOccupancyTypeEditable ot in Occtypes)
            {
                names.Add(ot.Name);
            }
            return names;
        }
        public SaveAllReportGroupVM  SaveAll()
        {
            List<TableErrorsReport> occtypesWithWarningsAndNoFatalErrors = new List<TableErrorsReport>();
            List<TableErrorsReport> occtypesWithFatalErrors = new List<TableErrorsReport>();

            SaveAllReportGroupVM saveAllGroup = null;

            foreach (IOccupancyTypeEditable otEditable in ModifiedOcctypes)
            {
                Utilities.FdaValidationResult warningsResult = otEditable.HasWarnings();
                Utilities.FdaValidationResult fatalErrorsResult = otEditable.HasFatalErrors(GetOcctypeNames());
                
                if(warningsResult.IsValid && fatalErrorsResult.IsValid)
                {
                    otEditable.SaveOcctype();
                }
                else if(!warningsResult.IsValid && fatalErrorsResult.IsValid)
                {
                    //it only has warnings
                    occtypesWithWarningsAndNoFatalErrors.Add(new TableErrorsReport(otEditable, warningsResult.ErrorMessage));
                }
                else if(!fatalErrorsResult.IsValid)
                {
                    occtypesWithFatalErrors.Add(new TableErrorsReport(otEditable, fatalErrorsResult.ErrorMessage));
                }
   
                saveAllGroup = new SaveAllReportGroupVM(Name, occtypesWithWarningsAndNoFatalErrors, occtypesWithFatalErrors);
            }

            return saveAllGroup;
        }

    }
}
