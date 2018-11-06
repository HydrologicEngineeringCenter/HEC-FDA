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

        public bool HasSiblingRules { get; set; }
        public BaseFdaElement SiblingElement { get; set; }//this can be a parent element or a sibling
        public bool HasCanOpenMultipleTimes { get; set; }
        public bool CanOpenMultipleTimes { get; set; }
        public bool HasParentGuid { get; set; }
        public Guid ParentGuid { get; set; }

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

        public EditorActionManager WithParentGuid(Guid parentGuid)
        {
            HasParentGuid = true;
            ParentGuid = parentGuid;
            return this;
        }
        public EditorActionManager WithCanOpenMultipleTimes(bool canOpenMultipleTimes)
        {
            HasCanOpenMultipleTimes = true;
            CanOpenMultipleTimes = canOpenMultipleTimes;
            return this;
        }
        public EditorActionManager WithSiblingRules(BaseFdaElement element)
        {
            HasSiblingRules = true;
            SiblingElement = element;
            return this;
        }
        public EditorActionManager WithSaveUndoRedo(SaveUndoRedoHelper helper)
        {
            HasSaveHelper = true;         
            SaveUndoRedoHelper = helper;
            return this;
        }


    }
}
