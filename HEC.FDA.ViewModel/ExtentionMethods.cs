namespace ViewModel
{
    public static class ExtentionMethods
    {
        /// <summary>
        /// this is dumb, but for some reason anytime a window is opened in all of FDA it
        /// recreates the studyVM. This static prop in a static class fixes it
        /// </summary>
        public static bool IsStudyOpen { get; set; }

    }

}
