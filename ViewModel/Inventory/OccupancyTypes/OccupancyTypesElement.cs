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
        /// <summary>
        /// This bool is to let the editor know which one of the elements to have selected when it opens. There should
        /// only ever be one element that is turned to "true".
        /// </summary>
        public bool IsSelected { get; set; }
        public List<IOccupancyType> ListOfOccupancyTypes { get; set; }        

        #endregion
        #region Constructors

        public OccupancyTypesElement( string occTypesGroupName, List<IOccupancyType> listOfOccTypes, int id):base(occTypesGroupName,"","", id)
        {
            Name = occTypesGroupName;
            ListOfOccupancyTypes = listOfOccTypes;
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
       


        public override XElement ToXML()
        {
            throw new NotImplementedException();
        }
    }
}
