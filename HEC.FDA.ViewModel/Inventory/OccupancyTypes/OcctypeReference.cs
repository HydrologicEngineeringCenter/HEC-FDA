using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    /// <summary>
    /// Hold the group id and the occtype id. This way an object that needs to carry around a list of occtypes doesn't need to hang on to the whole
    /// large object. If the actual occtype is required there is a method to get it.
    /// </summary>
    public class OcctypeReference:BaseViewModel
    {
        public int GroupID { get; }
        public int ID { get; }


        public OcctypeReference(int groupID, int id)
        {
            GroupID = groupID;
            ID = id;
        }

        public OccupancyType GetOccupancyType()
        {
            OccupancyType foundOT = null;
            List<OccupancyTypesElement> occTypeElems = StudyCache.GetChildElementsOfType<OccupancyTypesElement>();
            foreach(OccupancyTypesElement elem in occTypeElems)
            {
                if(elem.ID == GroupID)
                {
                    foreach(OccupancyType ot in elem.ListOfOccupancyTypes)
                    {
                        if(ot.ID == ID)
                        {
                            foundOT = ot;
                            break;
                        }
                    }
                    break;
                }
            }

            return foundOT;
        }

    }
}
