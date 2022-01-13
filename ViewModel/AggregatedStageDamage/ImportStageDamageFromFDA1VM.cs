using Importer;
using System;
using ViewModel.Editors;
using ViewModel.Utilities;

namespace ViewModel.AggregatedStageDamage
{
    public class ImportStageDamageFromFDA1VM : ImportFromFDA1VM
    {
        private ImportStageDamageFromFDA1Helper _Helper = new ImportStageDamageFromFDA1Helper();
        public ImportStageDamageFromFDA1VM(EditorActionManager manager) : base(manager)
        {

        }
        public override void SaveElements()
        {
            foreach (AggregatedStageDamageElement elem in _Helper.Elements)
            {
                Saving.PersistenceFactory.GetStageDamageManager().SaveNewElement(elem);
            }
        }

        public override FdaValidationResult Validate()
        {
            //todo: how should we do this? Maybe create a helper class? All the processing is done in
            //the persistence manager. I could put a validate down there, but that seems like the wrong place
            //for it. I think i also want to display any messages as it is being read in right? Does that
            //mean putting it on a background thread? What would that look like: a pop up when the user clicks 
            //the ok button?

            FdaValidationResult vr = new FdaValidationResult();

            _Helper.ImportStageDamages(Path);

            return vr;
        }
    }
}
