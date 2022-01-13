using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Editors;

namespace ViewModel.Utilities
{
    public abstract class ImportFromFDA1VM : BaseEditorVM
    {
        public string Path { get; set; }
        public ImportFromFDA1VM(EditorActionManager actionManager) : base(actionManager)
        {
            //i have to set the name to something to get past the default validation
            Name = "myName";
        }

        public abstract void SaveElements();

        public abstract FdaValidationResult Validate();



        public override void Save()
        {
            FdaValidationResult vr = Validate();
            if(vr.IsValid)
            {
                SaveElements();
            }

            //todo: else, do what? message the user?
        }
    }
}
