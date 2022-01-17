using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alternatives;
using Statistics.Histograms;

namespace alternativeComparisonReport
{
    class AlternativeComparisonReport
{
        private List<Alternative> _withProjectAlternatives;
        private Alternative _withoutProjectAlternative;

        public List<Alternative> WithProjectAlternatives
        {
            get
            {
                return _withProjectAlternatives;
            } set
            {
                _withProjectAlternatives = value;
            }
        }
        public Alternative WithoutProjectAlternative
        {
            get
            {
                return _withoutProjectAlternative;
            }
            set
            {
                _withoutProjectAlternative = value;
            }
        }

        public AlternativeComparisonReport(Alternative withoutProject, List<Alternative> withProjecs)
        {
            _withoutProjectAlternative = withoutProject;
            _withProjectAlternatives = withProjecs;
        }

        // we need:
        // //1. total ead reduced distribution
        // 2. total aaeq reduced distribution 
        //3, damage reduced distribution by impact area for a given with project
        //4. damage reduced distribution by damage category for a given with project 

}
}
