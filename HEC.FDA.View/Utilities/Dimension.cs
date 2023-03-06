namespace HEC.FDA.View.Utilities
{
    public class Dimension
    {
        public int MinWidth { get; set; }
        public int MinHeight { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }

        public Dimension()
        {

        }
        public Dimension(int minWidth, int minHeight, int width, int height, int maxWidth, int maxHeight)
        {
            MinWidth = minWidth;
            MinHeight = minHeight;
            Width = width;
            Height = height;
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
        }

    }
}
