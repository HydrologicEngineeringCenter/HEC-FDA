using HEC.FDA.ViewModel.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace HEC.FDA.ViewModel
{
    /// <summary>
    /// Base class for all "elements" in FDA. Elements are broken into two categories- parents and children. This
    /// class contains all the shared functionality among all elements.
    /// </summary>
    public abstract class BaseFdaElement : BaseViewModel
    {
        #region Fields
        private List<NamedAction> _Actions;
        private CustomHeaderVM _CustomTreeViewHeader;
        private bool _IsExpanded = true;
        private int _FontSize = 14;
        private bool _IsBold = true;
        private string _Name;
        private ObservableCollection<BaseFdaElement> _Elements = new();
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

        // These were added just to get rid of BindingExpression path errors. These are required by TreeViewItem, but apparently we don't actually use them?
        public object ToolTip { get; set; } 
        public ObservableCollection<BaseFdaElement> Elements
        {
            get { return _Elements; }
            set { _Elements = value; NotifyPropertyChanged(nameof(Elements)); }
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

        public BaseFdaElement()
        {
        }

        #endregion
        #region Voids

        public void UpdateTreeViewHeader(string newName, string newTooltip = null)
        {
            if (_CustomTreeViewHeader != null)
            {
                string image = _CustomTreeViewHeader.ImageSource;
                string decoration = _CustomTreeViewHeader.Decoration;
                string tooltip = _CustomTreeViewHeader.Tooltip;
                if(newTooltip != null)
                {
                    tooltip += newTooltip;
                }

                CustomTreeViewHeader = new CustomHeaderVM(newName)
                {
                    ImageSource = image,
                    Decoration = decoration,
                    Tooltip = tooltip
                };
            }
        }

        #endregion
    }
}
