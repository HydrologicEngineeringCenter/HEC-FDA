using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HEC.FDA.ModelTest.Globals;

[CollectionDefinition("Serial")]
public class GeospatialIntializer: ICollectionFixture<GlobalGDALSetup>
{
}
