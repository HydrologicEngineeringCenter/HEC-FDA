using System;
using System.Collections.Generic;
using System.Text;

namespace OccupancyTypes
{
    public static class OccTypeGroupReader
    {
        public static IOccupancyTypeGroup ReadOccupancyTypeGroup(string filePath)
        {
            //calling the CA occTypes and passing in the path will read the file and create
            //all the occtypes.
            Consequences_Assist.ComputableObjects.OccupancyTypes oldGroup = new Consequences_Assist.ComputableObjects.OccupancyTypes(filePath);

            //now i need to translate the CA version of the group and the list of occtypes into the FDA version
            List<IOccupancyType> occtypes = new List<IOccupancyType>();
            foreach(Consequences_Assist.ComputableObjects.OccupancyType ot in oldGroup.OccupancyTypes)
            {
                IOccupancyType newOcctype = OccupancyTypeFactory.Factory(ot);
                occtypes.Add(newOcctype);
            }

            return new OccupancyTypeGroup(occtypes);
        }

    }
}
