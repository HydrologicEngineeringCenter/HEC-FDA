namespace HEC.FDA.Model.impactarea
{
    public class ImpactArea
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public ImpactArea(string name, int id)
        {
            Name = name;
            ID = id;
        }


    }
}
