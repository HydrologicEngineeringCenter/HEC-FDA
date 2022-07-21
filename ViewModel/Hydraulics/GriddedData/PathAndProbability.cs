namespace HEC.FDA.ViewModel.Hydraulics.GriddedData
{
    //[Author(q0heccdm, 9 / 11 / 2017 8:48:18 AM)]
    public class PathAndProbability
    {
        #region Properties
        public string Path { get; set; }
        public double Probability { get; set;}
        #endregion
        #region Constructors
        public PathAndProbability(string path, double probability)
        {
            Path = path;
            Probability = probability;
        }
        #endregion
    }
}
