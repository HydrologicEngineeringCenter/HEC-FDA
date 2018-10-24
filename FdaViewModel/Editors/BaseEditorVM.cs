using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Utilities;
using Statistics;

namespace FdaViewModel.Editors
{
    public abstract class BaseEditorVM : BaseViewModel
    {

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }
        public bool IsImporter { get; set; }
        public bool HasSaved { get; set; } = false;
        public ChildElement OriginalElement { get; set; }
        public EditorActionManager ActionManager { get; set; }

        public ChildElement CurrentElement { get; set; }

        /// <summary>
        /// Call this one for importers.
        /// </summary>
        /// <param name="actionManager"></param>
        public BaseEditorVM(EditorActionManager actionManager)
        {
            IsImporter = true;
            ActionManager = actionManager;
            //if (actionManager != null && actionManager.HasOwnerValidationRules)
            //{
            //    actionManager.OwnerValidationRules.Invoke(this, null);
            //}
        }
        /// <summary>
        /// Call this one when editing an existing element
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="actionManager"></param>
        public BaseEditorVM(Utilities.ChildElement elem, EditorActionManager actionManager)
        {
            IsImporter = false;
            OriginalElement = elem.CloneElement(elem);
            CurrentElement = elem;

            ActionManager = actionManager;
            //if (actionManager.HasOwnerValidationRules)
            //{
            //    actionManager.OwnerValidationRules.Invoke(this, elem.Name);
            //}
            StudyCache.AddSiblingRules(this, elem);

            if (actionManager != null && actionManager.SaveUndoRedoHelper != null)
            {
                actionManager.SaveUndoRedoHelper.AssignValuesFromElementToEditorAction(this, elem);
            }
        }

        /// <summary>
        /// This will get called when the OK button is clicked on the editor
        /// </summary>
        public abstract void Save();
        public virtual bool RunSpecialValidation() { return true; }

        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
            AddRule(nameof(Name), () => Name != null, "Name cannot be blank.");
        }

   

        //public abstract void AssignValuesFromElementToEditor(OwnedElement element);
       
    }
}
