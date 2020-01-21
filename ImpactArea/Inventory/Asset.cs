using System;
using System.Collections.Generic;
using System.Text;

namespace ImpactArea
{
    internal class Asset
    {
        public IElevation Ground { get; }
        public IElevation Height { get; }
        public IElevation Elevation { get; }

        public Asset(IElevation ground, IElevation height)
        {
            //if (ground.Type != ElevationEnum.Ground) "error";
            //if (height.Type != ElevationEnum.Height) "error";
            //if ("unreasonable values") "error";
            //Ground = ground;
            //Height = height;
            //Elevation = ground + height;
        }
        public Asset(IElevation asset)
        {
            //if (asset.Type != ElevationEnum.Asset) "error";
            //Ground = new Elevation(new Constant(0), ElevationEnum.NotSet);
            //Height = new Elevation(new Constant(0), ElevationEnum.NotSet);
            //Elevation = asset;
        }
    }
}
