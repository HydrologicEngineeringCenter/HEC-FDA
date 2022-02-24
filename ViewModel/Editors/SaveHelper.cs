//using System;
//using HEC.FDA.ViewModel.Saving;
//using HEC.FDA.ViewModel.Utilities;

//namespace HEC.FDA.ViewModel.Editors
//{
//    public class SaveHelper  : BaseViewModel 
//    {
//        public event EventHandler RemoveFromTabsDictionary;

//        private bool _IsImporter = false;
//        private bool _IsFirstSave = true;

//        public Func<BaseEditorVM, ChildElement> CreateElementFromEditorAction { get; set; }
//        public Action<BaseEditorVM, ChildElement> AssignValuesFromElementToEditorAction { get; set; }
//        public Action<BaseEditorVM, ChildElement> AssignValuesFromEditorToElementAction { get; set; }

//        public IElementManager SavingManager { get; set; }

//        #region Constructors

//        /// <summary>
//        /// This one is for opening an existing element to edit
//        /// </summary>
//        /// <param name="savingAction"></param>
//        /// <param name="changeTableName"></param>
//        public SaveHelper(IElementManager savingManager, Func<BaseEditorVM, ChildElement> createElementFromEditorAction, Action<BaseEditorVM, ChildElement> assignValuesFromElementToEditorAction, Action<BaseEditorVM, ChildElement> assignValuesFromEditorToElementAction)
//        {
//            _IsImporter = true;
//            SavingManager = savingManager;
//            AssignValuesFromElementToEditorAction = assignValuesFromElementToEditorAction;
//            AssignValuesFromEditorToElementAction = assignValuesFromEditorToElementAction;
//            CreateElementFromEditorAction = createElementFromEditorAction;
//        }
//        public SaveHelper(IElementManager savingManager, ChildElement element, Func<BaseEditorVM, ChildElement> createElementFromEditorAction, Action<BaseEditorVM, ChildElement> assignValuesFromElementToEditorAction, Action<BaseEditorVM, ChildElement> assignValuesFromEditorToElementAction)
//        {
//            _IsImporter = false;
//            SavingManager = savingManager;
//            AssignValuesFromElementToEditorAction = assignValuesFromElementToEditorAction;
//            AssignValuesFromEditorToElementAction = assignValuesFromEditorToElementAction;
//            CreateElementFromEditorAction = createElementFromEditorAction;
//        }

//        #endregion

//        public void Save(string oldName, ChildElement oldElement, ChildElement elementToSave)
//        {
//            if (_IsImporter && _IsFirstSave)
//            {
//                SavingManager.SaveNew(elementToSave);
//                _IsFirstSave = false;         
//            }
//            else
//            {
//                SavingManager.SaveExisting( elementToSave);
//            }
//        }

//    }
//}
