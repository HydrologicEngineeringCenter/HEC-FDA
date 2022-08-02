using HEC.FDA.ViewModel.Editors;
using System;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.ImpactArea
{
    public class ImpactAreaRowItem : NameValidatingVM
    {
        public static String ROW_ITEM_TAG = "ImpactAreaRow";
        private static String ID_TAG = "ID";
        private static String DISPLAY_NAME_TAG = "DisplayName";

        #region Properties
        public int ID { get; set; }

        #endregion
        #region Constructors
 
        public ImpactAreaRowItem(int id, string dispName) 
        {
            ID = id;
            Name = dispName;
        }

        public ImpactAreaRowItem(XElement xmlElem)
        {
            Name = xmlElem.Attribute(DISPLAY_NAME_TAG).Value;
            ID = int.Parse(xmlElem.Attribute(ID_TAG).Value);
        }
        #endregion

        public XElement ToXML()
        {
            XElement rowElem = new XElement(ROW_ITEM_TAG);
            rowElem.SetAttributeValue(DISPLAY_NAME_TAG, Name);
            rowElem.SetAttributeValue(ID_TAG, ID);
            return rowElem;
        }
    }
}
