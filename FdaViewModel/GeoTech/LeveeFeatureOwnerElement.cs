using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.GeoTech
{
    class LeveeFeatureOwnerElement : Utilities.ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
      
        #endregion
        #region Constructors
        public LeveeFeatureOwnerElement(Utilities.ParentElement owner) : base(owner)
        {
            Name = "Levee Features";
            IsBold = false;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);


            Utilities.NamedAction add = new Utilities.NamedAction();
            add.Header = "Create New Levee Feature";
            add.Action = AddNewLeveeFeature;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(add);

            Actions = localActions;

            StudyCache.LeveeAdded += AddLeveeElement;
            StudyCache.LeveeRemoved += RemoveLeveeElement;
            StudyCache.LeveeUpdated += UpdateLeveeElement;
            GUID = Guid.NewGuid();

        }
        #endregion
        #region Voids
        private void UpdateLeveeElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.OldElement, e.NewElement);
        }
     
        private void RemoveLeveeElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        private void AddLeveeElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }
        public void AddNewLeveeFeature(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this)
               .WithParentGuid(this.GUID)
               .WithCanOpenMultipleTimes(true);

            LeveeFeatureEditorVM vm = new LeveeFeatureEditorVM(actionManager);
            //StudyCache.AddSiblingRules(vm, this);
            //vm.AddSiblingRules(this);

            Navigate(vm, false, false, "Create Levee");
          
        }
       
        #endregion
        #region Functions
       
        #endregion
    }
}
