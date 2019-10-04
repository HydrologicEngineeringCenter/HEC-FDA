using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
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
