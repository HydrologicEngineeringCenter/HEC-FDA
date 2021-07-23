using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.AggregatedStageDamage;
using ViewModel.Conditions;
using ViewModel.FlowTransforms;
using ViewModel.FrequencyRelationships;
using ViewModel.GeoTech;
using ViewModel.ImpactArea;
using ViewModel.Inventory;
using ViewModel.StageTransforms;
using ViewModel.Tabs;
using ViewModel.Utilities;
using ViewModel.Watershed;
using ViewModel.WaterSurfaceElevation;
using Statistics;

namespace ViewModel.Editors
{
    public abstract class BaseEditorVM : BaseViewModel
    {
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }
        //This lets me not add the sibling rules to itself when saving.
        public bool InTheProcessOfSaving = false;
        public bool IsImporter { get; set; }
        public bool HasSaved { get; set; } = false;
        public ChildElement OriginalElement { get; set; }
        public EditorActionManager ActionManager { get; set; }

        public ChildElement CurrentElement { 
            get; 
            set; }

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
        /// Call this one when editing an existing element. 
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="actionManager"></param>
        public BaseEditorVM(Utilities.ChildElement elem, EditorActionManager actionManager)
        {
            IsImporter = false;
            OriginalElement = elem.CloneElement(elem);
            CurrentElement = elem;

            ActionManager = actionManager;

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
            if (ActionManager.HasSiblingRules)
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
        }

        /// <summary>
        /// This will get called when the OK button is clicked on the editor
        /// </summary>
        public abstract void Save();
        /// <summary>
        /// This gets used when the "OK" or the "Save" button gets pressed on a form.
        /// It runs validation to see if the form is in state that can be closed or saved.
        /// </summary>
        /// <returns></returns>
        public virtual bool RunSpecialValidation() { return true; }

        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "00Name cannot be blank.");
            AddRule(nameof(Name), () => Name != null, "00Name cannot be blank.");
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
            List<string> existingElements = new List<string>();

            //get all elements of this type. This will include itself
            List<ChildElement> siblings = StudyCache.GetChildElementsOfType(element.GetType());

            string originalName = element.Name;
            //exclude itself
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

            AddSiblingUpdatedEvents(element);         
        }
        private void AddSiblingUpdatedEvents(ChildElement element)
        {
            var childElementType = element.GetType();

            if (childElementType == typeof(TerrainElement))
            {
                StudyCache.TerrainAdded += SiblingWasAdded;
                StudyCache.TerrainUpdated += SiblingNameChanged;
            }
            if (childElementType == typeof(ImpactAreaElement))
            {
                StudyCache.ImpactAreaAdded += SiblingWasAdded;
                StudyCache.ImpactAreaUpdated += SiblingNameChanged;
            }
            if (childElementType == typeof(WaterSurfaceElevationElement))
            {
                StudyCache.WaterSurfaceElevationAdded += SiblingWasAdded;
                StudyCache.WaterSurfaceElevationUpdated += SiblingNameChanged;
            }
            if (childElementType == typeof(AnalyticalFrequencyElement))
            {
                StudyCache.FlowFrequencyAdded += SiblingWasAdded;
                StudyCache.FlowFrequencyUpdated += SiblingNameChanged;
            }
            if (childElementType == typeof(InflowOutflowElement))
            {
                StudyCache.InflowOutflowAdded += SiblingWasAdded;
                StudyCache.InflowOutflowUpdated += SiblingNameChanged;
            }
            if (childElementType == typeof(RatingCurveElement))
            {
                StudyCache.RatingAdded += SiblingWasAdded;
                StudyCache.RatingUpdated += SiblingNameChanged;
            }
            if (childElementType == typeof(ExteriorInteriorElement))
            {
                StudyCache.ExteriorInteriorAdded += SiblingWasAdded;
                StudyCache.ExteriorInteriorUpdated += SiblingNameChanged;
            }
            if (childElementType == typeof(LeveeFeatureElement))
            {
                StudyCache.LeveeAdded += SiblingWasAdded;
                StudyCache.LeveeUpdated += SiblingNameChanged;
            }
            if (childElementType == typeof(FailureFunctionElement))
            {
                StudyCache.FailureFunctionAdded += SiblingWasAdded;
                StudyCache.FailureFunctionUpdated += SiblingNameChanged;
            }
            if (childElementType == typeof(InventoryElement))
            {
                StudyCache.StructureInventoryAdded += SiblingWasAdded;
                StudyCache.StructureInventoryUpdated += SiblingNameChanged;
            }
            if (childElementType == typeof(AggregatedStageDamageElement))
            {
                StudyCache.StageDamageAdded += SiblingWasAdded;
                StudyCache.StageDamageUpdated += SiblingNameChanged;
            }
            if (childElementType == typeof(ConditionsElement))
            {
                StudyCache.ConditionsElementAdded += SiblingWasAdded;
                StudyCache.ConditionsElementUpdated += SiblingNameChanged;
            }

        }
        private void SiblingNameChanged(object sender, Saving.ElementUpdatedEventArgs args)
        {
            string newName = args.NewElement.Name;
            //this gets called even if it is changing its own name
            //don't add rule if that is the case
            if(InTheProcessOfSaving)
            {
                InTheProcessOfSaving = false;
                return;
            }
            AddRule(nameof(Name), () =>
            {
                return Name != newName;
            }, "This name is already used. Names must be unique.");
        }

        private void SiblingWasAdded(object sender, Saving.ElementAddedEventArgs args)
        {
            string newName = args.Element.Name;
            //this gets called even if it is changing its own name
            //don't add rule if that is the case
            if (InTheProcessOfSaving)
            {
                InTheProcessOfSaving = false;
                return;
            }
            AddRule(nameof(Name), () =>
            {
                return Name != newName;
            }, "This name is already used. Names must be unique.");
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

    }
}
