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
            if (actionManager != null )
            {
                SetActionManagerValues();
            }

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
            //StudyCache.AddSiblingRules(this, elem);
            //AddSiblingRules(elem);


            if (actionManager != null)
            {
                SetActionManagerValues();
                if (actionManager.SaveUndoRedoHelper != null)
                {
                    actionManager.SaveUndoRedoHelper.AssignValuesFromElementToEditorAction(this, elem);
                }
            }
        }

        private void SetActionManagerValues()
        {
            if(ActionManager.HasSiblingRules)
            {
                if(ActionManager.SiblingElement.GetType().BaseType == typeof(ChildElement))
                {
                    AddSiblingRules((ChildElement)ActionManager.SiblingElement);
                }
                else if(ActionManager.SiblingElement.GetType().BaseType == typeof(ParentElement))
                {
                    AddSiblingRules((ParentElement)ActionManager.SiblingElement);
                }
            }
            if(ActionManager.HasCanOpenMultipleTimes)
            {
                this.CanOpenMultipleTimes = ActionManager.CanOpenMultipleTimes;
            }
            if(ActionManager.HasParentGuid)
            {
                this.ParentGUID = ActionManager.ParentGuid;
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

        /// <summary>
        /// This is used to add rules that the name cannot be the same as a sibling. 
        /// This method is to be called for editors. It will exclude the original name
        /// from the list of banned words.
        /// </summary>
        /// <param name="editorVM"></param>
        /// <param name="element"></param>
        public void AddSiblingRules( ChildElement element)
        {
            //child elements need to exclude thier own name from the list of banned words
            //bool isChild = false;
            //if (element.GetType().IsSubclassOf(typeof(ChildElement)))
            //{
            //    isChild = true;
            //}

            List<string> existingElements = new List<string>();
            List<ChildElement> siblings = StudyCache.GetSiblingsOfChild(element);

            string originalName = element.Name;

            foreach (ChildElement elem in siblings)
            {
                if ( elem.Name.Equals(originalName))
                {
                    continue;
                }
                else
                {
                    existingElements.Add(elem.Name);
                }
            }

            foreach (string existingName in existingElements)
            {
                AddRule(nameof(Name), () =>
                {
                    return Name != existingName;
                }, "This name is already used. Names must be unique.");
            }

        }

        /// <summary>
        /// This is used to add rules that the name cannot be the same as a sibling. 
        /// This method is to be called for importers. 
        /// </summary>
        /// <param name="editorVM"></param>
        /// <param name="element"></param>
        public void AddSiblingRules( ParentElement element)
        {
            List<string> existingElements = new List<string>();
            foreach (ChildElement elem in StudyCache.GetChildrenOfParent(element))
            {
                existingElements.Add(elem.Name);
            }

            foreach (string existingName in existingElements)
            {
                AddRule(nameof(Name), () =>
                {
                    return Name != existingName;
                }, "This name is already used. Names must be unique.");
            }

        }

        //public abstract void AssignValuesFromElementToEditor(OwnedElement element);

    }
}
