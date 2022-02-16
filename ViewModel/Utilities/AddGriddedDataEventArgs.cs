namespace HEC.FDA.ViewModel.Utilities
{
    public class AddGriddedDataEventArgs: AddMapFeatureEventArgs
    {
        public LifeSimGIS.RasterFeatures Features { get; set; }
        public OpenGLMapping.ColorRamp Ramp { get; set; }
        public AddGriddedDataEventArgs(LifeSimGIS.RasterFeatures features, OpenGLMapping.ColorRamp ramp): base()
        {
            Features = features;
            Ramp = ramp;
        }
    }
}
