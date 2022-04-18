using System;
using System.Windows;

namespace HEC.FDA.View.Utilities
{
    public static class WindowDimensions
    {
        public static readonly string KEY = "Dimension";
        private static Dimension DEFAULT_DIMENSION = new Dimension(300, 300, 800, 600, 0, 0);

        /// <summary>
        /// Searches the App.xaml resources looking for the editor type. If the "Dimension" is not
        /// defined for a resource, then the default dimension defined in this class will be used.
        /// </summary>
        /// <param name="editorType"></param>
        /// <returns></returns>
        public static Dimension GetWindowDimensions(Type editorType)
        {
            Dimension dimension = DEFAULT_DIMENSION;
            DataTemplateKey key = GetDataTemplateKey(editorType);

            if (key != null)
            {
                DataTemplate dataTemplate = Application.Current.Resources[key] as DataTemplate;
                if (dataTemplate.Resources.Contains(KEY))
                {
                    dimension = (Dimension)dataTemplate.Resources[KEY];
                }
            }
            return dimension;
        }

        /// <summary>
        /// Recursively searches the type to see if it is in the App.xaml resources.
        /// This is recursive so that we can search for any base classes that a vm might
        /// be extending. An example of this is RatingCurveEditorVM. It implements CurveEditorVM
        /// which is the resource defined in the App.xaml.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static DataTemplateKey GetDataTemplateKey(Type type)
        {
            if (type == null)
            {
                return null;
            }
            DataTemplateKey output = new DataTemplateKey(type);

            if (Application.Current.Resources.Contains(output))
            {
                return output;
            }
            return GetDataTemplateKey(type.BaseType);
        }

    }
}
