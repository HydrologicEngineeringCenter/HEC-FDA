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
        private string _Name;
        private string _Decoration;
        private string _ImageSource;
        private bool _GifVisible = false;
        #endregion
        #region Properties
        public string Name { get; set; }
        /// <summary>
        /// A wait gif can be shown at the end of the element name that indicates that it is doing an operation ie: saving, loading
        /// </summary>
        public bool GifVisible
        {
            get { return _GifVisible; }
            set { _GifVisible = value; NotifyPropertyChanged(); }
        }
        //public string Name
        //{
        //    get { return _Name; }
        //    set { _Name = value; NotifyPropertyChanged(); }
        //}
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
        /// <param name="gifVisible">True: displays a "wait" gif at the end of the name that indicates that it is busy doing some process.</param>
        public CustomHeaderVM(string name, string imageSource = "", string decoration = "", bool gifVisible = false)
        {
            Name = name;
            Decoration = decoration;
            ImageSource = imageSource;
            GifVisible = gifVisible;
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion

      
    }
}
