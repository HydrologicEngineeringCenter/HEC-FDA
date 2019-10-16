using System;
using System.Collections.Generic;
using System.Text;

using Utilities.Validation;

namespace Statistics.Validation
{
    class SummaryStatisticsValidator: IValidator<SummaryStatistics>
    {
        public SummaryStatisticsValidator()
        {
        }

        public IEnumerable<string> ReportErrors(SummaryStatistics obj)
        {

        }
    }
}
