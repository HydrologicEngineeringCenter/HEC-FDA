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

        public EditorActionManager ActionManager { get; set; }

        public OwnedElement CurrentElement { get; set; }


        public BaseEditorVM(EditorActionManager actionManager)
        {
            //ownerValidationRules(this);
            ActionManager = actionManager;
            if (actionManager != null && actionManager.HasOwnerValidationRules)
            {
                actionManager.OwnerValidationRules.Invoke(this, null);
            }
        }

        public BaseEditorVM(Utilities.OwnedElement elem, EditorActionManager actionManager)
        {
            CurrentElement = elem;

            ActionManager = actionManager;
            if (actionManager.HasOwnerValidationRules)
            {
                actionManager.OwnerValidationRules.Invoke(this, elem.Name);
            }

            actionManager.AssignValuesFromElementToEditorAction(this, elem);
        }

        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
            AddRule(nameof(Name), () => Name != null, "Name cannot be blank.");
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }

        //public abstract void AssignValuesFromElementToEditor(OwnedElement element);
       
    }
}
