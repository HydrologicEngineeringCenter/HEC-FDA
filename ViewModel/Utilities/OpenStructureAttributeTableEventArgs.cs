using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Utilities
{
    public class OpenStructureAttributeTableEventArgs: AddMapFeatureEventArgs
    {
        public LifeSimGIS.VectorFeatures Features { get; set; }
        public OpenGLMapping.OpenGLDrawInfo DrawInfo { get; set; }

        public DatabaseManager.DataTableView Attributes { get; set; }
        public OpenStructureAttributeTableEventArgs(string featureName, LifeSimGIS.VectorFeatures features, DatabaseManager.DataTableView dtv, OpenGLMapping.OpenGLDrawInfo drawInfo) : base()
        {
            Features = features;
            DrawInfo = drawInfo;
            FeatureName = featureName;
            Attributes = dtv;
        }
    }
}
