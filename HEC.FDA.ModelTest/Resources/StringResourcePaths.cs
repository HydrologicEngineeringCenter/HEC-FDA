using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ModelTest.Resources;
public class StringResourcePaths
{
    public const string pathToNSIShapefile = @"..\..\..\..\HEC.FDA.ModelTest\Resources\MuncieNSI\Muncie-SI_CRS2965.shp";

    public const string pathToIAShapefile = @"..\..\..\..\HEC.FDA.ModelTest\Resources\MuncieImpactAreas\ImpactAreas.shp";
    public const string pathToIAShapefileNODBF = @"..\..\..\HEC.FDA.ModelTest\Resources\MuncieImpactAreas\MissingDBF\ImpactAreas.shp";
    public const string pathToIAProjectionFile = @"..\..\..\..\HEC.FDA.ModelTest\Resources\MuncieImpactAreas\ImpactAreas.prj";

    public const string ParentDirectoryToUnsteadyResult = @"..\..\..\..\HEC.FDA.ModelTest\Resources\MuncieResult";
    public const string UnsteadyHDFFileName = @"Muncie.p04.hdf";

    public const string ParentDirectoryToGrid = @"..\..\..\..\HEC.FDA.ModelTest\Resources\MuncieGrid";
    public const string GridFileName = @"WSE (Max).Terrain.muncie_clip.tif";

    public const string ParentDirectoryToSteadyResult = @"..\..\..\..\HEC.FDA.ModelTest\Resources\MuncieSteadyResult";
    public const string SteadyHDFFileName = @"Muncie.p10.hdf";

    public const string IANameColumnHeader = "Name";
    public const string SteadyHydraulicProfileName = "500";

    public const string TerrainPath = @"..\..\..\..\HEC.FDA.ModelTest\Resources\MuncieTerrain\Terrain30ft.Clone.hdf";

    public const string PathToIndexPoints = @"..\..\..\..\HEC.FDA.ModelTest\Resources\MuncieIndexPoints\MuncieIndexPts.shp";

    public const string PathToAltProjection = @"..\..\..\..\HEC.FDA.ModelTest\Resources\Projections\26844.prj";

    public const string PathToOccupancyTypes = @"..\..\..\HEC.FDA.ModelTest\Resources\MuncieOccupancyTypes.txt";
}
