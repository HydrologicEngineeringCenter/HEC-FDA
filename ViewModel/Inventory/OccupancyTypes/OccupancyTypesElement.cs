using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    //[Author(q0heccdm, 7 / 11 / 2017 3:23:11 PM)]
    public class OccupancyTypesElement : ChildElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 7/11/2017 3:23:11 PM
        #endregion

        #region Properties

        public List<IOccupancyType> ListOfOccupancyTypes { get; set; } = new List<IOccupancyType>();       

        #endregion
        #region Constructors

        public OccupancyTypesElement( string occTypesGroupName, List<IOccupancyType> listOfOccTypes, int id):base(occTypesGroupName,"","", id)
        {
            Name = occTypesGroupName;
            ListOfOccupancyTypes = listOfOccTypes;
        }

        public OccupancyTypesElement(XElement occtypeElem, int id) : base(occtypeElem, id)
        {
            ReadHeaderXElement(occtypeElem.Element(HEADER_XML_TAG));
            IEnumerable<XElement> occtypes = occtypeElem.Elements("OccType");
            foreach (XElement ot in occtypes)
            {
                ListOfOccupancyTypes.Add(new OccupancyType(ot));
            }
        }
        public override XElement ToXML()
        {
            XElement occTypeGroup = new XElement("OccTypeGroup");
            occTypeGroup.Add(CreateHeaderElement());
            foreach (IOccupancyType ot in ListOfOccupancyTypes)
            {
                occTypeGroup.Add(ot.ToXML());
            }

            return occTypeGroup;
        }

        #endregion

        #region Functions
        public List<String> getUniqueDamageCategories()
        {
            HashSet<String> dams = new HashSet<String>();
            foreach (IOccupancyType ot in ListOfOccupancyTypes)
            {
                dams.Add(ot.DamageCategory);
            }
            return dams.ToList<String>();
        }

        #endregion       
       


        
    }
}
