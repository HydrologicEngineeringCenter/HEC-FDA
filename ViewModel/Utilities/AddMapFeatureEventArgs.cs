using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
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
