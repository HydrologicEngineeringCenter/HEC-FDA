using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class SaveAllReportGroupVM:NameValidatingVM
    {
        public bool HasFatalErrors { get { return OcctypesWithFatalErrors.Any(); } }

        public List<TableErrorsReport> OcctypesWithFatalErrors { get; } = new List<TableErrorsReport>();

        public bool HasWarnings { get { return OcctypesWithWarnings.Any(); } }   

        public List<TableErrorsReport> OcctypesWithWarnings { get; } = new List<TableErrorsReport>();

        public SaveAllReportGroupVM(string name, List<TableErrorsReport> occtypesWithWarnings, List<TableErrorsReport> occtypesWithFatalErrors)
        {
            Name = name;
            OcctypesWithWarnings = occtypesWithWarnings;
            OcctypesWithFatalErrors = occtypesWithFatalErrors;
        }

        public void SaveOcctypes()
        {
            foreach(TableErrorsReport report in OcctypesWithWarnings)
            {
                report.OccType.SaveOcctype();
            }
        }

    }
}
