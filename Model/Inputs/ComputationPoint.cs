using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaModel.Utilities.Attributes;

namespace FdaModel.Inputs
{
    public class ComputationPoint
    {
        #region Notes
        /* 1. I need to add impact areas to this, but this could be done as a standalone list, through compostion or something else - I haven't how decided yet. */
        #endregion

        #region Fields
        private string _Name;
        private string _Condition;
        private string _Stream;
        private int _AnalysisYear;
        #endregion

        #region Properties
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        public string Condition
        {
            get
            {
                return _Condition;
            }
            set
            {
                _Condition = value;
            }
        }

        public string Stream
        {
            get
            {
                return _Stream;
            }
            set
            {
                _Stream = value;
            }
        }
        public int AnalysisYear
        {
            get
            {
                return _AnalysisYear;
            }
            set
            {
                _AnalysisYear = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary> Represents a time and space at which a compute occurs. It is equivalent to an index point. </summary>
        /// <param name="name"></param>
        /// <param name="stream"></param>
        /// <param name="condition"></param>
        /// <param name="analysisYear"></param>
        [Tested(false)]
        public ComputationPoint(string name, string stream, string condition, int analysisYear)
        {
            Name = name;
            Stream = stream;
            Condition = condition;
            AnalysisYear = analysisYear;
        }
        #endregion
    }
}
