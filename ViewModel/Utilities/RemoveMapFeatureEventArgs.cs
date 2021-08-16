using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Utilities
{
    public class RemoveMapFeatureEventArgs: EventArgs
    {
        private readonly int _featureHashCode;
        public int FeatureHashCode { get { return _featureHashCode; } }
        public RemoveMapFeatureEventArgs(int featureHashCode)
        {
            _featureHashCode = featureHashCode;
        }
    }
}
