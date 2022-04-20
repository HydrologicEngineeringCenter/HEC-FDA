using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.ViewModel.Validation;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel
{
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
        private bool _IsExpanded = true;
        private int _FontSize = 14;
        private bool _IsBold = true;
        private string _Name;
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

        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(nameof(Name)); }
        }
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set { _IsExpanded = value; NotifyPropertyChanged(nameof(IsExpanded)); }
        }
        public int FontSize
        {
            get { return _FontSize; }
            set { _FontSize = value; NotifyPropertyChanged(nameof(FontSize)); }
        }
        public bool IsBold
        {
            get { return _IsBold; }
            set { _IsBold = value; NotifyPropertyChanged(nameof(IsBold)); }
        }
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

        #endregion
    }
}
