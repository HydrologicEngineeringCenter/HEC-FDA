using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RasMapperLib.Mapping;
using Xunit;

namespace HEC.FDA.ViewModelTest.Integration
{
    public class RASMapperLibShould
    {
        [Fact]
        public void ReturnListOfPlanNamesInHDF()
        {
            String pathToResultFile = "C:\\Users\\q0hecbbb\\Projects\\FDA\\HydraulicExamplesForCody\\SteadyFlowResultFile\\Muncie.p09.hdf";
            var result = new RasMapperLib.RASResults(pathToResultFile);
            var profileNames = result.ProfileNames;
            Assert.True(profileNames.Length > 1);
        }
    }
}
