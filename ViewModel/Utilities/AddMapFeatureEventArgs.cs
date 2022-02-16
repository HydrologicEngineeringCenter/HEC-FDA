using System;

namespace HEC.FDA.ViewModel.Utilities
{
    public class AddMapFeatureEventArgs: EventArgs
    {
        //figure out how to detect remove..
        public int MapFeatureHash { get; set; }
        public string FeatureName { get; set; }
        public AddMapFeatureEventArgs()
        { 
        }

    }
}
