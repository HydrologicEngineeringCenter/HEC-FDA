using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using utilities;

namespace fda_model_test.integrationtests
{
    [Trait("Category", "Integration")]
    public class dbftests
    {
        [Fact]
        public void ReadFile()
        {
            string filepath = @"C:\Users\Q0HECWPL\Downloads\ProbData.dbf";
            dbfreader dbr = new dbfreader(filepath);
            double val = (double)dbr.GetColumn(18)[0];
            Assert.Equal(2.944761, val);
        }
    }
}
