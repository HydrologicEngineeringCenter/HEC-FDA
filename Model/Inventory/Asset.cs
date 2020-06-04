using System;
using System.Collections.Generic;
using System.Text;
using Functions;

namespace Model.Inventory
{
    internal class Asset
    {
        #region Properties
        public IAssetEnum Type { get; }
        public IElevation<IOrdinate> GroundElevation { get; }
        public IElevation<IOrdinate> AssetHeight { get; }
        public IElevation<IOrdinate> AssetElevation { get; }

        #endregion
    }
}
