using System;
using System.Collections.Generic;
using ViewModel.Editors;
using ViewModel.Utilities;

namespace ViewModel
{
    public delegate void TransactionEventHandler(object sender, Utilities.Transactions.TransactionEventArgs args);
    /// <summary>
    /// Base class for all "elements" in FDA. Elements are broken into two categories- parents and children. This
    /// class contains all the shared functionality among all elements.
    /// </summary>
    public abstract class BaseFdaElement : BaseViewModel
    {
        #region Notes
        #endregion
        #region Fields
        private List<NamedAction> _Actions;
        private CustomHeaderVM _CustomTreeViewHeader;
        private bool _TableContainsGeoData = false;
        private string _Tooltip = null;

        #endregion
        #region Events
        public event EventHandler RenameMapTreeViewElement;
        public event EventHandler AddMapTreeViewElementBackIn;

        public void AddMapTreeViewItemBackIn(object sender, EventArgs e)
        {
            AddMapTreeViewElementBackIn?.Invoke(sender, e);
        }
        public void RenameMapTreeViewItem(object sender, EventArgs e)
        {
            RenameMapTreeViewElement?.Invoke(sender, e);
        }
        #endregion
        #region Properties

        public String ToolTip
        {
            get { return _Tooltip; }
            set { _Tooltip = value; NotifyPropertyChanged(); }
        }
        public bool TableContainsGeoData
        {
            get { return _TableContainsGeoData; }
            set { _TableContainsGeoData = value; }
        }
        /// <summary>
        /// This governs the image and text that is displayed in the main trees.
        /// </summary>
        public CustomHeaderVM CustomTreeViewHeader
        {
            get { return _CustomTreeViewHeader; }
            set { _CustomTreeViewHeader = value; NotifyPropertyChanged(nameof(CustomTreeViewHeader)); }
        }

        /// <summary>
        /// These become the right click menu options for each element in the tree.
        /// </summary>
        public List<NamedAction> Actions
        {
            get { return _Actions; }
            set { _Actions = value; NotifyPropertyChanged(nameof(Actions)); }
        }

        #endregion
        #region Constructors
            /// <summary>
            /// Constructor
            /// </summary>
        public BaseFdaElement()
        {
        }

        #endregion
        #region Voids
      
        public void UpdateTreeViewHeader(string newName)
        {
            if (_CustomTreeViewHeader == null) { return; }
            string image = _CustomTreeViewHeader.ImageSource;
            string decoration = _CustomTreeViewHeader.Decoration;
            bool gifVisible = _CustomTreeViewHeader.GifVisible;
            CustomTreeViewHeader = new CustomHeaderVM(newName, image, decoration,gifVisible);
        }

        public void AssignValuesFromCurveEditorToElement(BaseEditorVM editorVM, ChildElement element)
        {
            CurveEditorVM vm = (CurveEditorVM)editorVM;
            element.Name = vm.Name;
            element.Description = vm.Description;
            element.Curve = vm.Curve;
            element.UpdateTreeViewHeader(vm.Name);
        }

        public void AssignValuesFromElementToCurveEditor(BaseEditorVM editorVM, ChildElement element)
        {
            CurveEditorVM vm = (CurveEditorVM)editorVM;

            vm.Name = element.Name;
            vm.Description = element.Description;
            vm.Curve = element.Curve;
            if (vm.EditorVM != null)
            {
                vm.EditorVM.Function = element.Curve;
            }
        }

        #endregion
        #region Functions
        #endregion

    }
}
