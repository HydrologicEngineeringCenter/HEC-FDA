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

        public List<OccupancyType> ListOfOccupancyTypes { get; set; } = new List<OccupancyType>();       

        #endregion
        #region Constructors

        public OccupancyTypesElement( string name, string lastEditDate, string description,  List<OccupancyType> listOfOccTypes, int id):base(name, lastEditDate, description, id)
        {
            ListOfOccupancyTypes = listOfOccTypes;
            AddDefaultActions(EditOccupancyTypes, StringConstants.EDIT_OCCTYPE_MENU);
        }

        public OccupancyTypesElement(XElement occtypeElem, int id) : base(occtypeElem, id)
        {
            ReadHeaderXElement(occtypeElem.Element(HEADER_XML_TAG));
            IEnumerable<XElement> occtypes = occtypeElem.Elements("OccType");
            foreach (XElement ot in occtypes)
            {
                ListOfOccupancyTypes.Add(new OccupancyType(ot));
            }
            AddDefaultActions(EditOccupancyTypes, StringConstants.EDIT_OCCTYPE_MENU);

        }
        public override XElement ToXML()
        {
            XElement occTypeGroup = new XElement("OccTypeGroup");
            occTypeGroup.Add(CreateHeaderElement());
            foreach (OccupancyType ot in ListOfOccupancyTypes)
            {
                occTypeGroup.Add(ot.ToXML());
            }

            return occTypeGroup;
        }

        #endregion

        private void EditOccupancyTypes(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);

            OccupancyTypesEditorVM _OccTypeEditor = new OccupancyTypesEditorVM(this, actionManager);
            _OccTypeEditor.RequestNavigation += Navigate;
            string header = StringConstants.EDIT_OCCTYPE_HEADER + ": " + Name;
            DynamicTabVM tab = new DynamicTabVM(header, _OccTypeEditor, header);
            Navigate(tab, false, false);
        }

        public List<String> getUniqueDamageCategories()
        {
            HashSet<String> dams = new HashSet<String>();
            foreach (OccupancyType ot in ListOfOccupancyTypes)
            {
                dams.Add(ot.DamageCategory);
            }
            return dams.ToList<String>();
        }

    }
}
