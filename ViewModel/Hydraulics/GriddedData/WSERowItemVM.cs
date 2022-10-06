namespace HEC.FDA.ViewModel.Hydraulics.GriddedData
{
    public class WSERowItemVM:BaseViewModel
    {
        public bool IsSelected { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumberOfElevations { get; set; }
        public bool IsUsingWSE { get; set; }
        public HydraulicElement Element { get; set; }
        public WSERowItemVM(HydraulicElement element)
        {
            IsSelected = true;
            Name = element.Name;
            Description = element.Description;
            NumberOfElevations = element.Profiles.Count;
            IsUsingWSE = !element.IsDepthGrids;
            Element = element;

        }

    }
}
