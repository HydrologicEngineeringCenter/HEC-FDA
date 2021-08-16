using System;
using System.Collections.Generic;
using System.Text;

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
            //muy feo. need factories more sophisticated bin count perhaps ets..
            Statistics.IHistogram h = Statistics.IHistogramFactory.Factory(Statistics.IDataFactory.Factory(composites), 100);
            return new Elevation(new DistributedValue(h), ElevationEnum.Asset);
        }
    }
}
