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
        public IParameterOrdinate GroundElevation { get; }
        public IParameterOrdinate AssetHeight { get; }
        public IParameterOrdinate AssetElevation { get; }

        #endregion
    }
}
