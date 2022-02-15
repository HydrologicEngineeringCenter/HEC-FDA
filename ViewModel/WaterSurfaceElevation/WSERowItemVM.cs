using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.WaterSurfaceElevation
{
    public class WSERowItemVM:BaseViewModel
    {
        public bool IsSelected { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumberOfElevations { get; set; }
        public bool IsUsingWSE { get; set; }
        public WaterSurfaceElevationElement Element { get; set; }
        public WSERowItemVM(WaterSurfaceElevationElement element)
        {
            IsSelected = true;
            Name = element.Name;
            Description = element.Description;
            NumberOfElevations = element.RelativePathAndProbability.Count;
            IsUsingWSE = !element.IsDepthGrids;
            Element = element;

        }

    }
}
