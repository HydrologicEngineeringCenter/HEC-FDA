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
        public IElevation GroundElevation { get; }
        public IElevation AssetHeight { get; }
        public IElevation AssetElevation { get; }

        #endregion
    }
}
