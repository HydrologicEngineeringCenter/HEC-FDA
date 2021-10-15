using System;
using System.Collections.Generic;
using System.Text;
using Statistics.Histograms;

namespace ImpactArea.Elevations
{
    internal class AssetElevation
    {
        public IElevation Ground { get; }
        public IElevation Height { get; }
        public IElevation Composite { get; }

        internal AssetElevation(IElevation[] elements)
        {
            //assignment...
        }

        internal IElevation ComputeComposite(double[] rnos, int n = 30)
        {
            //add is distribute boolean to avoid oversampling.
            int j = 0;
            double[] composites = new double[rnos.Length / 2];
            for (int i = 0; i < rnos.Length; i += 2)
            {
                composites[j] = Ground.Height.Value(rnos[i]) + Height.Height.Value(rnos[i + 1]);
                j++;
            }
            //the below histogram may not compile correctly this is a guess
            Histogram h = new Histogram(Statistics.IDataFactory.Factory(composites), 1);
            return new Elevation(new DistributedValue(h), ElevationEnum.Asset);
        }
    }
}
