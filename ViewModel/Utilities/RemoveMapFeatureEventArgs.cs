using System;

namespace HEC.FDA.ViewModel.Utilities
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
