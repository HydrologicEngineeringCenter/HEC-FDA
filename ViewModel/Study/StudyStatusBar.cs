using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Study
{
    public static class StudyStatusBar
    {
        public static event EventHandler SaveStatusChanged;

        public static readonly string UnsavedChangesMessage = "Unsaved Changes";

        private static string _SaveStatus;
        public static string SaveStatus
        {
            get { return _SaveStatus; }
            set { _SaveStatus = value; SaveStatusChanged?.Invoke(_SaveStatus, new EventArgs()); }
        }

    }
}
