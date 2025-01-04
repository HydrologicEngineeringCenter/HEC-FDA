namespace HEC.FDA.ViewModel.Utilities
{
    //[Author(q0heccdm, 6 / 30 / 2017 11:51:38 AM)]
    /// <summary>
    /// Governs the look of the element in the tree.
    /// </summary>
    public class CustomHeaderVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/30/2017 11:51:38 AM
        #endregion
        #region Fields
        private string _Decoration;
        private string _ImageSource;
        private string _Tooltip;
        #endregion
        #region Properties
        public string Tooltip
        {
            get { return _Tooltip; }
            set { _Tooltip = value; NotifyPropertyChanged(); }
        }

        public string Name { get; set; }

        /// <summary>
        /// The decoration can be any string added to the end of the element name.
        /// A '*' can be used to indicate that the element has unsaved changes.
        /// </summary>
        public string Decoration
        {
            get { return _Decoration; }
            set { _Decoration = value;  NotifyPropertyChanged(); }
        }
        /// <summary>
        /// The image that will be displayed for the element.
        /// </summary>
        public string ImageSource
        {
            get { return _ImageSource; }
            set { _ImageSource = value;NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        /// <summary>
        /// Creates the image and text for the element in the main trees
        /// </summary>
        /// <param name="name">Name of the element</param>
        /// <param name="imageSource">image source for the image that will be displayed before the element name</param>
        /// <param name="decoration">A string that will be added to the end of the element name ie: '*'</param>
        public CustomHeaderVM(string name, string imageSource = "", string decoration = "")
        {
            Name = name;
            Decoration = decoration;
            ImageSource = imageSource;
        }
        #endregion
      
    }
}
