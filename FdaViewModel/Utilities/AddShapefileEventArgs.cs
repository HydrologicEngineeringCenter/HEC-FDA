

namespace FdaViewModel.Utilities
{
    public class AddShapefileEventArgs : AddMapFeatureEventArgs
    {
        public LifeSimGIS.VectorFeatures Features { get; set; }
        public OpenGLMapping.OpenGLDrawInfo DrawInfo { get; set; }
        
        public DataBase_Reader.DataTableView Attributes { get; set; }
        public AddShapefileEventArgs(string featureName, LifeSimGIS.VectorFeatures features, DataBase_Reader.DataTableView dtv, OpenGLMapping.OpenGLDrawInfo drawInfo) : base()
        {
            Features = features;
            DrawInfo = drawInfo;
            FeatureName = featureName;
            Attributes = dtv;
        }
    }
}
