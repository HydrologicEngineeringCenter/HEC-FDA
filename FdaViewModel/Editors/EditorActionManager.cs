using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Editors
{
    public class EditorActionManager
    {

        public bool HasSaveHelper { get; set; }
        public SaveUndoRedoHelper SaveUndoRedoHelper { get; set; }

        public Func<BaseEditorVM, OwnedElement> CreateElementFromEditorAction { get; set; }
        public Action<BaseEditorVM,OwnedElement> AssignValuesFromElementToEditorAction { get; set; }
        public Action<BaseEditorVM, OwnedElement> AssignValuesFromEditorToElementAction { get; set; }

        public bool HasOwnerValidationRules { get; set; }
        public Action<BaseViewModel, string> OwnerValidationRules { get; set; }


        /// <summary>
        /// It is conveivable to me that the createElementFromEditor and the assign values from editor to element could be the same thing. The reason for these two 
        /// is that the first takes the values from the editor and creates a NEW element and then uses that to save. The original element that was opened from the tree
        /// does not actually have the values from the editor. The assign call will give it the correct values in memory.
        /// </summary>
        /// <param name="createElementFromEditorAction"></param>
        /// <param name="assignValuesFromElementToEditorAction"></param>
        /// <param name="assignValuesFromEditorToElementAction"></param>
        public EditorActionManager()
        {
            
        }

        public EditorActionManager WithOwnerValidationRules(Action<BaseViewModel, string> ownerValidationRules)
        {
            HasOwnerValidationRules = true;
            OwnerValidationRules = ownerValidationRules;
            return this;
        }

        public EditorActionManager WithSaveUndoRedo(SaveUndoRedoHelper helper, Func<BaseEditorVM, OwnedElement> createElementFromEditorAction, Action<BaseEditorVM, OwnedElement> assignValuesFromElementToEditorAction, Action<BaseEditorVM, OwnedElement> assignValuesFromEditorToElementAction)
        {
            HasSaveHelper = true;
            AssignValuesFromElementToEditorAction = assignValuesFromElementToEditorAction;
            AssignValuesFromEditorToElementAction = assignValuesFromEditorToElementAction;
            CreateElementFromEditorAction = createElementFromEditorAction;
            SaveUndoRedoHelper = helper;
            return this;
        }


    }
}
