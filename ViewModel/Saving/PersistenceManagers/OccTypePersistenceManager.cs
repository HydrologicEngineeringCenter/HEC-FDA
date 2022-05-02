using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using Statistics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using static HEC.FDA.ViewModel.Inventory.OccupancyTypes.OccTypeItem;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class OccTypePersistenceManager : SavingBase
    {
        private const string VALUE_UNCERT_RATIO = "ValueUncertaintyRatio";
        private const string IS_BY_VALUE = "IsByValue";
        private const string OCCTYPES_TABLE_NAME = "occupancy_types";

        //These are the columns for the parent table
        private const int PARENT_GROUP_ID_COL = 0;
        private const int PARENT_GROUP_NAME_COL = 1;

        //These are the columns for the child table
        private const int GROUP_ID_COL = 0;
        private const int OCCTYPE_ID_COL = 1;

        private const int NAME_COL = 2;
        private const int DESC_COL = 3;
        private const int DAM_CAT_COL = 4;
        private const int FOUND_HT_UNCERTAINTY_COL = 5;

        private const int STRUCT_ITEM_COL = 6;
        private const int CONT_ITEM_COL = 7;
        private const int VEH_ITEM_COL = 8;
        private const int OTHER_ITEM_COL = 9;

        private const string ParentTableName = "occupancy_type_groups";

        private const string PARENT_NAME_FIELD = "Name";
        private const string ITEM_DATA = "OcctypeItem";
        private const string COMP_COMP = "ComputeComponentVM";
        private const string VALUE_UNCERT = "ValueUncertainty";

        private const string IS_ITEM_CHECKED = "IsItemChecked";

        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return new Type[] { typeof(string), typeof(bool) }; }
        }

        public override string TableName => ParentTableName;

        public override string[] TableColumnNames => new string[] { PARENT_NAME_FIELD, "IsSelected" };

        public OccTypePersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        public string[] OcctypeColumns
        {
            get
            {
                return new string[] {"GroupID","OcctypeID", "Name",
                    "Description", "DamageCategory","FoundHtUncertainty",
                    "Structures", "Content", "Vehicle", "Other", "OtherParams" };
            }
        }

        public Type[] OcctypeTypes
        {
            get
            {
                return new Type[] {
                    typeof(int), typeof(int), typeof(string),
                    typeof(string), typeof(string),  typeof(string),
                    typeof(string), typeof(string),  typeof(string), 
                    typeof(string), typeof(string)};
            }
        }

        //This method is not used, but it needs to be hear for the abstract
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            return null;
        }

        private List<DataRow> GetParentTableRows()
        {
            List<DataRow> retval = new List<DataRow>();
            if (!Connection.Instance.IsConnectionNull)
            {
                if (!Connection.Instance.IsOpen)
                {
                    Connection.Instance.Open();
                }
                if (Connection.Instance.TableNames().Contains(ParentTableName))
                {

                    DataTable table = Connection.Instance.GetDataTable(ParentTableName);
                    foreach (DataRow row in table.Rows)
                    {
                        retval.Add(row);
                    }
                }
            }
            return retval;
        }
        
        public override void Load()
        {
            foreach (DataRow row in GetParentTableRows())
            {
                //each of these is a group
                int groupId = Convert.ToInt32(row[PARENT_GROUP_ID_COL]);
                string groupName = (string)row[PARENT_GROUP_NAME_COL];

                //now read the child table and grab all the occtypes with this group id
                List<IOccupancyType> occtypes = LoadOcctypesFromOccTypeTable(groupId);
                OccupancyTypesElement elem = new OccupancyTypesElement(groupName, occtypes, groupId);
                StudyCacheForSaving.AddElement(elem);
            }
        }

        public void DeleteOcctypeGroup(int groupID)
        {
            //delete the row from the parent table
            DeleteRowWithKey(TableName, groupID, "ID");

            //delete all the occtypes associated with this group from the occtypes table
            //this one call will delete all the rows with that group id
            DeleteRowWithKey(OCCTYPES_TABLE_NAME, groupID, "GroupID");

            //remove from the study cache
            RemoveElementFromCache(groupID);
        }

        /// <summary>
        /// Removes the specified group from the study cache. This is done when saving the 
        /// occupancy types editor. 
        /// </summary>
        /// <param name="group"></param>
        private void RemoveElementFromCache(int groupID)
        {
            List<OccupancyTypesElement> elems = StudyCacheForSaving.OccTypeElements;
            int indexToRemove = -1;
            for (int i = 0; i < elems.Count(); i++)
            {
                if (elems[i].ID == groupID)
                {
                    indexToRemove = i;
                    break;
                }
            }
            if (indexToRemove != -1)
            {
                StudyCacheForSaving.RemoveElement(elems[indexToRemove]);
            }
        }

        public void DeleteOcctype(IOccupancyTypeEditable occtypeToDelete)
        {
            //only update the db if this occtype is actually in there.
            //if the occtype has never been saved then there is nothing to remove.
            if (occtypeToDelete.HasBeenSaved)
            {
                int[] keys = new int[] { occtypeToDelete.GroupID, occtypeToDelete.ID };
                string[] keyColNames = new string[] { "GroupID", "OcctypeID" };

                DeleteRowWithCompoundKey(OCCTYPES_TABLE_NAME, keys, keyColNames);
                DeleteOccTypeFromGroupInCache(occtypeToDelete);
            }
        }

        private void DeleteOccTypeFromGroupInCache(IOccupancyTypeEditable ot)
        {
            OccupancyTypesElement group = GetElementFromGroupID(ot.GroupID);
            if (group == null)
            {
                return;
            }
            else
            {
                foreach (IOccupancyType occtype in group.ListOfOccupancyTypes)
                {
                    if (occtype.ID == ot.ID)
                    {
                        group.ListOfOccupancyTypes.Remove(occtype);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the name of the group in the group table.
        /// </summary>
        /// <param name="groups"></param>
        public void SaveModifiedGroups(List<IOccupancyTypeGroupEditable> groups)
        {
            //these get saved to the parent table.
            foreach (IOccupancyTypeGroupEditable group in groups)
            {
                string[] columnsToUpdate = new string[] { PARENT_NAME_FIELD };
                object[] newValues = new object[] { group.Name };
                UpdateTableRow(ParentTableName, group.ID, "ID", columnsToUpdate, newValues);
            }
            UpdateOccTypeGroupsInStudyCache(groups);
        }

        private OccupancyTypesElement GetElementFromGroupID(int groupId)
        {
            List<OccupancyTypesElement> occtypeElems = StudyCacheForSaving.OccTypeElements;
            for (int i = 0; i < occtypeElems.Count; i++)
            {
                if (occtypeElems[i].ID == groupId)
                {
                    return occtypeElems[i];
                }
            }
            return null;
        }

        private void AddNewOccTypeToCache(IOccupancyType ot)
        {
            //Because the study cache and the occtype owner element are hanging on to
            //the same object. All i have to do is replace the occtype in the list of 
            //occtypes and it will show up in all places. There is no need to remove
            //a group and add a new one.
            OccupancyTypesElement group = GetElementFromGroupID(ot.GroupID);
            if (group == null)
            {
                return;
            }
            else
            {
                group.ListOfOccupancyTypes.Add(ot);
            }
        }
        
        /// <summary>
        /// The only way to modify an occtype group is to change its name. This method finds the element
        /// in the cache and updates the name.
        /// </summary>
        /// <param name="groupsToUpdateInCache"></param>
        public void UpdateOccTypeGroupsInStudyCache(List<IOccupancyTypeGroupEditable> groupsToUpdateInCache)
        {
            List<OccupancyTypesElement> occupancyTypesElements = StudyCacheForSaving.GetChildElementsOfType<OccupancyTypesElement>();
            foreach (IOccupancyTypeGroupEditable group in groupsToUpdateInCache)
            {

                foreach(OccupancyTypesElement elem in occupancyTypesElements)
                {
                    if(elem.ID == group.ID)
                    {
                        elem.Name = group.Name;
                    }
                }
            }
        }

        private void UpdateOccTypeInCache(IOccupancyType ot)
        {
            //Because the study cache and the occtype owner element are hanging on to
            //the same object. All i have to do is replace the occtype in the list of 
            //occtypes and it will show up in all places. There is no need to remove
            //a group and add a new one.
            OccupancyTypesElement group = GetElementFromGroupID(ot.GroupID);
            if (group != null)
            {
                //now replace the occtype with the new one
                for (int i = 0; i < group.ListOfOccupancyTypes.Count; i++)
                {
                    if (group.ListOfOccupancyTypes[i].ID == ot.ID)
                    {
                        group.ListOfOccupancyTypes[i] = ot;
                        break;
                    }
                }
            }
        }
        public void SaveModifiedOcctype(IOccupancyType ot)
        {
            object[] keys = new object[] { ot.GroupID, ot.ID };
            string[] keyColNames = new string[] { "GroupID", "OcctypeID" };

            //update the whole row
            string[] columnsToUpdate = OcctypeColumns;
            object[] newValues = GetOccTypeRowForOccTypesTable(ot.GroupID, ot.ID, ot).ToArray();

            UpdateTableRowWithCompoundKey(OCCTYPES_TABLE_NAME, keys, keyColNames, columnsToUpdate, newValues);
            UpdateOccTypeInCache(ot);
        }

        public void SaveNewOccType(IOccupancyType ot)
        {
            DatabaseManager.DataTableView tbl = Connection.Instance.GetTable(OCCTYPES_TABLE_NAME);
            if (tbl == null)
            {
                Connection.Instance.CreateTable(OCCTYPES_TABLE_NAME, OcctypeColumns, OcctypeTypes);
            }

            object[] newValues = GetOccTypeRowForOccTypesTable(ot.GroupID, ot.ID, ot).ToArray();
            Connection.Instance.AddRowToTableWithPrimaryKey(newValues, OCCTYPES_TABLE_NAME, OcctypeColumns);

            AddNewOccTypeToCache(ot);
        }

        /// <summary>
        /// Looks at all the current occtypes in this group and returns the max ID plus 1.
        /// </summary>
        /// <param name="groupId"></param>
        public int GetIdForNewOccType(int groupId)
        {
            List<IOccupancyType> occtypes = LoadOcctypesFromOccTypeTable(groupId);
            List<int> occtypeIds = new List<int>();
            foreach (IOccupancyType ot in occtypes)
            {
                occtypeIds.Add(ot.ID);
            }
            if (occtypeIds.Count > 0)
            {
                return occtypeIds.Max() + 1;
            }
            else
            {
                return 1;
            }
        }
        public void SaveNew(ChildElement element)
        { 
            SavingAction(element);
        }

        public void SaveNewElements(List<ChildElement> elements)
        {
            foreach (ChildElement elem in elements)
            {
                SavingAction(elem);
            }
        }

        private void SavingAction(ChildElement element)
        {
            //this will save to the parent table
            //and add the element to the study cache
            base.SaveNew(element);

            //save to the child table
            SaveNewToOcctypesTable(element);
        }

        public int GetGroupId(string groupName)
        {
            return GetElementId(ParentTableName, groupName);
        }

        public void SaveNewToOcctypesTable(ChildElement element)
        {
            //we should have already saved the element to the parent table so that we can grab the id from that table
            int elemId = GetElementId(TableName, element.Name);
            DatabaseManager.DataTableView tbl = Connection.Instance.GetTable(OCCTYPES_TABLE_NAME);
            if (tbl == null)
            {
                Connection.Instance.CreateTable(OCCTYPES_TABLE_NAME, OcctypeColumns, OcctypeTypes);
                tbl = Connection.Instance.GetTable(OCCTYPES_TABLE_NAME);
            }

            List<IOccupancyType> ListOfOccupancyTypes = ((OccupancyTypesElement)element).ListOfOccupancyTypes;
            string groupName = element.Name;

            List<object[]> rows = new List<object[]>();

            int i = 1;
            foreach (IOccupancyType ot in ListOfOccupancyTypes)
            {
                rows.Add(GetOccTypeRowForOccTypesTable(elemId, i, ot).ToArray());
                i++;
            }
            tbl.AddRows(rows);
            tbl.ApplyEdits();
        }

        private List<IOccupancyType> LoadOcctypesFromOccTypeTable(int groupId)
        {
            List<IOccupancyType> occtypes = new List<IOccupancyType>();
            if (Connection.Instance.TableNames().Contains(OCCTYPES_TABLE_NAME))
            {
                DataTable table = Connection.Instance.GetDataTable(OCCTYPES_TABLE_NAME);

                foreach (DataRow row in table.Rows)
                {
                    if (Convert.ToInt32(row[GROUP_ID_COL]) == groupId)
                    {
                        occtypes.Add(CreateOcctypeFromRow(row.ItemArray));
                    }
                }
            }
            return occtypes;
        }

        private IOccupancyType CreateOcctypeFromRow(object[] rowData)
        {
            int groupId = Convert.ToInt32(rowData[GROUP_ID_COL]);
            int occtypId = Convert.ToInt32(rowData[OCCTYPE_ID_COL]);

            string name = (string)rowData[NAME_COL];
            string desc = (string)rowData[DESC_COL];
            string damCatName = (string)rowData[DAM_CAT_COL];

            string foundHtUncertaintyXML = (string)rowData[FOUND_HT_UNCERTAINTY_COL];

            string structureItemXML = (string)rowData[STRUCT_ITEM_COL];
            string contentItemXML = (string)rowData[CONT_ITEM_COL];
            string vehicleItemXML = (string)rowData[VEH_ITEM_COL];
            string otherItemXML = (string)rowData[OTHER_ITEM_COL];

            OccTypeItem structItem = ReadItemFromXML(OcctypeItemType.structure, structureItemXML);
            OccTypeItemWithRatio contentItem = ReadItemWithRatioFromXML(OcctypeItemType.content, contentItemXML);
            OccTypeItem vehicleItem = ReadItemFromXML(OcctypeItemType.vehicle, vehicleItemXML);
            OccTypeItemWithRatio otherItem = ReadItemWithRatioFromXML(OcctypeItemType.other, otherItemXML);

            ContinuousDistribution foundHtUncert = (ContinuousDistribution)ContinuousDistribution.FromXML(XElement.Parse(foundHtUncertaintyXML));

            IOccupancyType occtype = new OccupancyType(name, desc, groupId, damCatName, structItem, contentItem,
                vehicleItem, otherItem, foundHtUncert, occtypId);

            return occtype;
        }

        /// <summary>
        /// This method is used to create the row for the parent occtype table. 
        /// This table has a lot of columns
        /// </summary>
        /// <param name="ot"></param>
        /// <returns></returns>
        private List<object> GetOccTypeRowForOccTypesTable(int elemId, int occtypeId, IOccupancyType ot)
        {
            object[] rowsList = new object[OcctypeColumns.Length];

            rowsList[GROUP_ID_COL] = elemId;
            rowsList[OCCTYPE_ID_COL] = occtypeId;
            rowsList[NAME_COL] = ot.Name;
            rowsList[DESC_COL] = ot.Description;
            rowsList[DAM_CAT_COL] = ot.DamageCategory;

            if (ot.FoundationHeightUncertainty == null)
            {
                rowsList[FOUND_HT_UNCERTAINTY_COL] = "";
            }
            else
            {
                rowsList[FOUND_HT_UNCERTAINTY_COL] = ot.FoundationHeightUncertainty.ToXML().ToString();
            }

            rowsList[STRUCT_ITEM_COL] = WriteOccTypeItemToXML(ot.StructureItem);
            rowsList[CONT_ITEM_COL] = WriteOccTypeItemWithRatioToXML(ot.ContentItem);
            rowsList[VEH_ITEM_COL] = WriteOccTypeItemToXML(ot.VehicleItem);
            rowsList[OTHER_ITEM_COL] = WriteOccTypeItemWithRatioToXML(ot.OtherItem);

            return rowsList.ToList();
        }

        private OccTypeItem ReadItemFromXML(OcctypeItemType itemType, string xmlString)
        {
            XDocument doc = XDocument.Parse(xmlString);
            XElement itemElem = doc.Element(ITEM_DATA);
            bool isChecked = Convert.ToBoolean( itemElem.Attribute(IS_ITEM_CHECKED).Value);

            XElement curveElem = itemElem.Element(COMP_COMP); 
            ComputeComponentVM comp = new ComputeComponentVM(curveElem);

            XElement valueUncertParent = itemElem.Element(VALUE_UNCERT);
            XElement valueUncert = valueUncertParent.Elements().First();
            ContinuousDistribution valueUncertainty = (ContinuousDistribution)ContinuousDistribution.FromXML(valueUncert);

            return new OccTypeItem(itemType, isChecked, comp, valueUncertainty);
        }

        private OccTypeItemWithRatio ReadItemWithRatioFromXML(OcctypeItemType itemType, string xmlString)
        {
            XDocument doc = XDocument.Parse(xmlString);
            XElement itemElem = doc.Element(ITEM_DATA);
            bool isChecked = Convert.ToBoolean(itemElem.Attribute(IS_ITEM_CHECKED).Value);
            bool isByVal = Convert.ToBoolean(itemElem.Attribute(IS_BY_VALUE).Value);

            XElement curveElem = itemElem.Element(COMP_COMP);
            ComputeComponentVM comp = new ComputeComponentVM(curveElem);

            XElement valueUncertParent = itemElem.Element(VALUE_UNCERT);
            XElement valueUncert = valueUncertParent.Elements().First();
            ContinuousDistribution valueUncertainty = (ContinuousDistribution)ContinuousDistribution.FromXML(valueUncert);

            XElement valueUncertRatioParent = itemElem.Element(VALUE_UNCERT_RATIO);
            XElement valueUncertRatio = valueUncertRatioParent.Elements().First();
            ContinuousDistribution valueUncertaintyRatio = (ContinuousDistribution)ContinuousDistribution.FromXML(valueUncertRatio);

            return new OccTypeItemWithRatio(itemType, isChecked, comp, valueUncertainty, valueUncertaintyRatio, isByVal);
        }

        private string WriteOccTypeItemToXML(OccTypeItem item)
        {
            XElement itemElem = new XElement(ITEM_DATA);
            itemElem.SetAttributeValue(IS_ITEM_CHECKED, item.IsChecked);

            XElement curveElem = item.Curve.ToXML();
            itemElem.Add(curveElem);

            itemElem.Add(WriteContinuousDistToXML(item.ValueUncertainty.Distribution));

            return itemElem.ToString();
        }

        private string WriteOccTypeItemWithRatioToXML(OccTypeItemWithRatio item)
        {
            XElement itemElem = new XElement(ITEM_DATA);
            itemElem.SetAttributeValue(IS_ITEM_CHECKED, item.IsChecked);
            itemElem.SetAttributeValue(IS_BY_VALUE, item.IsByValue);

            XElement curveElem = item.Curve.ToXML();
            itemElem.Add(curveElem);

            itemElem.Add(WriteContinuousDistToXML(item.ValueUncertainty.Distribution));
            itemElem.Add(WriteContinuousDistRatioToXML(item.ContentByRatioVM.Distribution));
            return itemElem.ToString();
        }

        private XElement WriteContinuousDistToXML(ContinuousDistribution cd)
        {
            XElement valueUncertParentElem = new XElement(VALUE_UNCERT);
            XElement valueUncertElem = cd.ToXML();
            valueUncertParentElem.Add(valueUncertElem);
            return valueUncertParentElem;
        }
        private XElement WriteContinuousDistRatioToXML(ContinuousDistribution cd)
        {
            XElement valueUncertParentElem = new XElement(VALUE_UNCERT_RATIO);
            XElement valueUncertElem = cd.ToXML();
            valueUncertParentElem.Add(valueUncertElem);
            return valueUncertParentElem;
        }

        public override object[] GetRowDataFromElement(ChildElement elem)
        {
            OccupancyTypesElement occElem = (OccupancyTypesElement)elem;
            return new object[]
            {
                    occElem.Name,
                    occElem.IsSelected
            };
        }
    }
}
