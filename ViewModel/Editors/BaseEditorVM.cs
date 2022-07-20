using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Watershed;
using HEC.FDA.ViewModel.WaterSurfaceElevation;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Editors
{
    public abstract class BaseEditorVM : NameValidatingVM, IDetectChanges
    {
        private string _SavingText;
        private string _Description = "";
        private bool _IsCreatingNewElement;
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }
        //This lets me not add the sibling rules to itself when saving.
        public bool InTheProcessOfSaving = false;

     
        public bool IsCreatingNewElement
        {
            get { return _IsCreatingNewElement;}
            set { _IsCreatingNewElement = value; NotifyPropertyChanged(); }
        }
        public bool HasSaved { get; set; } = false;
        /// <summary>
        /// This is important for when saving and not exiting right away. I need access to the element id.
        /// </summary>
        public ChildElement OriginalElement { get; set; }
        public EditorActionManager ActionManager { get; set; }
        public string SavingText
        {
            get { return _SavingText; }
            set { _SavingText = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Call this one for importers.
        /// </summary>
        /// <param name="actionManager"></param>
        public BaseEditorVM(EditorActionManager actionManager)
        {
            IsCreatingNewElement = true;
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
        public BaseEditorVM(ChildElement elem, EditorActionManager actionManager)
        {
            Name = elem.Name;
            Description = elem.Description;
            IsCreatingNewElement = false;
            OriginalElement = elem.CloneElement(elem);

            ActionManager = actionManager;

            if (actionManager != null)
            {
                SetActionManagerValues();
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

        public virtual FdaValidationResult IsValid()
        {
            return new FdaValidationResult();
        }

        /// <summary>
        /// This will get called when the OK or save button is clicked on the editor
        /// </summary>
        public abstract void Save();

        /// <summary>
        /// This will get called when the OK or save button is clicked on the editor
        /// </summary>
        public void Save(ChildElement elementToSave)
        {
            InTheProcessOfSaving = true;
            string lastEditDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = lastEditDate;
            IElementManager elementManager = PersistenceFactory.GetElementManager(elementToSave);

            if (IsCreatingNewElement)
            {
                elementManager.SaveNew(elementToSave);
                IsCreatingNewElement = false;
            }
            else
            {
                elementManager.SaveExisting(elementToSave);
            }

            SavingText = CreateLastSavedText(elementToSave);
            HasChanges = false;
            HasSaved = true;
            OriginalElement = elementToSave;
        }

        /// <summary>
        /// I wanted this here so that the text could live in one place.
        /// That way if we want to change it, it should change all the places that use it.
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        private string CreateLastSavedText(ChildElement elem)
        {
            return "Last Saved: " + elem.LastEditDate;
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
            if (childElementType == typeof(SpecificIAS))
            {
                StudyCache.IASElementAdded += SiblingWasAdded;
                StudyCache.IASElementUpdated += SiblingNameChanged;
            }

        }
        private void SiblingNameChanged(object sender, ElementUpdatedEventArgs args)
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

        private void SiblingWasAdded(object sender, ElementAddedEventArgs args)
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

        public int GetElementID(SavingBase persistenceManager)
        {
            int id;
            if (IsCreatingNewElement)
            {
                id = persistenceManager.GetNextAvailableId();
            }
            else
            {
                id = OriginalElement.ID;
            }
            return id;
        }

    }
}
