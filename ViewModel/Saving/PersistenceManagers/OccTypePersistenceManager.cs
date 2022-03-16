using FdaLogging;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class OccTypePersistenceManager : SavingBase
    {
        private const string OCCTYPES_TABLE_NAME = "occupancy_types";
        //These are the columns for the parent table
        private const int PARENT_GROUP_ID_COL = 0;
        private const int PARENT_GROUP_NAME_COL = 1;
        private const int PARENT_IS_SELECTED_COL = 2;

        //These are the columns for the child table
        private const int GROUP_ID_COL = 0;
        private const int OCCTYPE_ID_COL = 1;

        private const int NAME_COL = 2;
        private const int DESC_COL = 3;
        private const int DAM_CAT_COL = 4;
        //private const int FOUND_HT_UNCERTAINTY_TYPE_COL = 5;
        private const int FOUND_HT_UNCERTAINTY_COL = 5;

        private const int STRUCT_ITEM_COL = 6;
        private const int CONT_ITEM_COL = 7;
        private const int VEH_ITEM_COL = 8;
        private const int OTHER_ITEM_COL = 9;

        private const int OTHER_PARAMS_COL = 10;

        


        //ELEMENT_TYPE is used to store the type in the log tables. Initially i was actually storing the type
        //of the element. But since they get stored as strings if a developer changes the name of the class
        //you would no longer get any of the old logs. So i use this constant.
        private const string ELEMENT_TYPE = "OccType";
        private static readonly FdaLogger.FdaLogger LOGGER = new FdaLogger.FdaLogger("OccTypePersistenceManager");

        private const string ParentTableName = "occupancy_type_groups";

        private const string PARENT_NAME_FIELD = "Name";
        private const string ITEM_DATA = "OcctypeItem";
        private const string COMP_COMP = "ComputeComponentVM";
        private const string VALUE_UNCERT = "ValueUncertainty";

        private const string IS_ITEM_CHECKED = "IsItemChecked";
        private const string OTHER_PARAMS = "OtherParams";

        /// <summary>
        /// The types of the columns in the parent table
        /// </summary>
        public override Type[] TableColumnTypes
        {
            get { return new Type[] { typeof(string), typeof(bool) }; }
        }
        internal override string ChangeTableConstant { get { return "OccType"; } }

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


        //todo: i don't think this method is used, but it needs to be hear for the abstract
        public override ChildElement CreateElementFromRowData(object[] rowData)
        {
            return null;
        }

        private List<DataRow> GetParentTableRows()
        {
            List<DataRow> retval = new List<DataRow>();
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                if (!Storage.Connection.Instance.IsOpen)
                {
                    Storage.Connection.Instance.Open();
                }
                if (Storage.Connection.Instance.TableNames().Contains(ParentTableName))
                {

                    System.Data.DataTable table = Storage.Connection.Instance.GetDataTable(ParentTableName);
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
            DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(OCCTYPES_TABLE_NAME);
            if (tbl == null)
            {
                Storage.Connection.Instance.CreateTable(OCCTYPES_TABLE_NAME, OcctypeColumns, OcctypeTypes);
            }

            object[] newValues = GetOccTypeRowForOccTypesTable(ot.GroupID, ot.ID, ot).ToArray();
            Storage.Connection.Instance.AddRowToTableWithPrimaryKey(newValues, OCCTYPES_TABLE_NAME, OcctypeColumns);

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
            DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(OCCTYPES_TABLE_NAME);
            if (tbl == null)
            {
                Storage.Connection.Instance.CreateTable(OCCTYPES_TABLE_NAME, OcctypeColumns, OcctypeTypes);
                tbl = Storage.Connection.Instance.GetTable(OCCTYPES_TABLE_NAME);
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
            if (Storage.Connection.Instance.TableNames().Contains(OCCTYPES_TABLE_NAME))
            {
                DataTable table = Storage.Connection.Instance.GetDataTable(OCCTYPES_TABLE_NAME);

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

            OccTypeItem structItem = ReadItemFromXML(structureItemXML);
            OccTypeItem contentItem = ReadItemFromXML(contentItemXML);
            OccTypeItem vehicleItem = ReadItemFromXML(vehicleItemXML);
            OccTypeItem otherItem = ReadItemFromXML(otherItemXML);

            ContinuousDistribution foundHtUncert = (ContinuousDistribution)ContinuousDistribution.FromXML(XElement.Parse(foundHtUncertaintyXML));
            ContinuousDistribution contToStruct = ReadContentToStructureValueUncertainty((string)rowData[OTHER_PARAMS_COL]);
            ContinuousDistribution otherToStruct = ReadOtherToStructureValueUncertainty((string)rowData[OTHER_PARAMS_COL]);
            double contToStrucValue = ReadContentToStructureValue((string)rowData[OTHER_PARAMS_COL]);
            double otherToStrucValue = ReadOtherToStructureValue((string)rowData[OTHER_PARAMS_COL]);

            IOccupancyType occtype = new OccupancyType(name, desc, groupId, damCatName, structItem, contentItem,
                vehicleItem, otherItem, foundHtUncert, contToStruct, otherToStruct,contToStrucValue, otherToStrucValue, occtypId);

            return occtype;
        }

        private const string CONT_TO_STRUCT = "ContentToStructureValue";
        private const string OTHER_TO_STRUCT = "OtherToStructureValue";
        private const string CONT_TO_STRUCT_VALUE = "ContentToStructureValue";
        private const string OTHER_TO_STRUCT_VALUE = "OtherToStructureValue";

        private string WriteOtherParamsToXML(IOccupancyType ot)
        {
            XElement otherParamsElem = new XElement(OTHER_PARAMS);

            XElement contentToStructureElem = new XElement(CONT_TO_STRUCT);
            contentToStructureElem.SetAttributeValue(CONT_TO_STRUCT_VALUE, ot.ContentToStructureValue);
            contentToStructureElem.Add(ot.ContentToStructureValueUncertainty.ToXML());

            XElement otherToStructureElem = new XElement(OTHER_TO_STRUCT);
            otherToStructureElem.SetAttributeValue(OTHER_TO_STRUCT_VALUE, ot.OtherToStructureValue);
            otherToStructureElem.Add(ot.OtherToStructureValueUncertainty.ToXML());

            otherParamsElem.Add(contentToStructureElem);
            otherParamsElem.Add(otherToStructureElem);

            return otherParamsElem.ToString();
        }

        private double ReadContentToStructureValue(string xmlString)
        {
            XDocument doc = XDocument.Parse(xmlString);
            XElement otherParamsElem = doc.Element(OTHER_PARAMS);
            XElement contToStructElem = otherParamsElem.Element(CONT_TO_STRUCT);
            return Convert.ToDouble(contToStructElem.Attribute(CONT_TO_STRUCT_VALUE).Value);
        }

        private double ReadOtherToStructureValue(string xmlString)
        {
            XDocument doc = XDocument.Parse(xmlString);
            XElement otherParamsElem = doc.Element(OTHER_PARAMS);
            XElement otherToStructElem = otherParamsElem.Element(OTHER_TO_STRUCT);
            return Convert.ToDouble(otherToStructElem.Attribute(OTHER_TO_STRUCT_VALUE).Value);
        }

        private ContinuousDistribution ReadContentToStructureValueUncertainty(string xmlString)
        {
            XDocument doc = XDocument.Parse(xmlString);
            XElement otherParamsElem = doc.Element(OTHER_PARAMS);
            XElement contToStructElem = otherParamsElem.Element(CONT_TO_STRUCT);
            XElement contElem = contToStructElem.Elements().First();
            return (ContinuousDistribution)ContinuousDistribution.FromXML(contElem);
        }
        private ContinuousDistribution ReadOtherToStructureValueUncertainty(string xmlString)
        {
            XDocument doc = XDocument.Parse(xmlString);
            XElement otherParamsElem = doc.Element(OTHER_PARAMS);
            XElement otherToStructElem = otherParamsElem.Element(OTHER_TO_STRUCT);
            XElement contElem = otherToStructElem.Elements().First();
            return (ContinuousDistribution)ContinuousDistribution.FromXML(contElem);
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
            rowsList[CONT_ITEM_COL] = WriteOccTypeItemToXML(ot.ContentItem);
            rowsList[VEH_ITEM_COL] = WriteOccTypeItemToXML(ot.VehicleItem);
            rowsList[OTHER_ITEM_COL] = WriteOccTypeItemToXML(ot.OtherItem);

            rowsList[OTHER_PARAMS_COL] = WriteOtherParamsToXML(ot);
            return rowsList.ToList();
        }

        private OccTypeItem ReadItemFromXML(string xmlString)
        {
            XDocument doc = XDocument.Parse(xmlString);
            XElement itemElem = doc.Element(ITEM_DATA);
            bool isChecked = Convert.ToBoolean( itemElem.Attribute(IS_ITEM_CHECKED).Value);

            XElement curveElem = itemElem.Element(COMP_COMP); 
            ComputeComponentVM comp = new ComputeComponentVM(curveElem);

            XElement valueUncertParent = itemElem.Element(VALUE_UNCERT);
            XElement valueUncert = valueUncertParent.Elements().First();
            ContinuousDistribution valueUncertainty = (ContinuousDistribution)ContinuousDistribution.FromXML(valueUncert);

            return new OccTypeItem(isChecked, comp, valueUncertainty);
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

        private XElement WriteContinuousDistToXML(ContinuousDistribution cd)
        {
            XElement valueUncertParentElem = new XElement(VALUE_UNCERT);
            XElement valueUncertElem = cd.ToXML();
            valueUncertParentElem.Add(valueUncertElem);
            return valueUncertParentElem;
        }

        public ObservableCollection<LogItem> GetLogMessages(ChildElement element)
        {
            return new ObservableCollection<LogItem>();
        }

        /// <summary>
        /// This will put a log into the log tables. Logs are only unique by element id and
        /// element type. ie. Rating Curve id=3.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="elementName"></param>
        public override void Log(LoggingLevel level, string message, string elementName)
        {
            // int elementId = GetElementId(TableName, elementName);
            //LOGGER.Log(level, message, ELEMENT_TYPE, elementId);
        }

        /// <summary>
        /// This will look in the parent table for the element id using the element name. 
        /// Then it will sweep through the log tables pulling out any logs with that id
        /// and element type. 
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public override ObservableCollection<LogItem> GetLogMessages(string elementName)
        {
            //int id = GetElementId(TableName, elementName);
            //return RetrieveFromDB.GetLogMessages(id, ELEMENT_TYPE);
            return new ObservableCollection<LogItem>();
        }

        /// <summary>
        /// Gets all the log messages for this element from the specified log level table.
        /// This is used by the MessageExpander to filter by log level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public override ObservableCollection<LogItem> GetLogMessagesByLevel(LoggingLevel level, string elementName)
        {
            //int id = GetElementId(TableName, elementName);
            //return RetrieveFromDB.GetLogMessagesByLevel(level, id, ELEMENT_TYPE);
            return new ObservableCollection<LogItem>();

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
