using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace OccupancyTypesTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = directoryName + "\\DamCatsAndOcctypes.txt";
            OccupancyTypes.OccTypeGroupReader.ReadOccupancyTypeGroup(path);
            int i = 0;
        }
    }
}
