using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class AggregatedStageDamageElement : ChildElement
    {
        private const string SELECTED_STRUCTURES = "SelectedStructures";
        private const string SELECTED_INDEX_POINTS = "SelectedIndexPoints";
        private const string IS_MANUAL = "IsManual";
        private const string STAGE_DAMAGE_CURVES = "StageDamageCurves";
        private const string STAGE_DAMAGE_CURVE = "StageDamageCurve";
        private const string IMPACT_AREA_ROWS = "ImpactAreaRows";

        #region Properties
        public int SelectedWSE { get; }
        public int SelectedStructures { get; }
        public int SelectedIndexPoints { get; }
        public List<StageDamageCurve> Curves { get; } = new List<StageDamageCurve>();
        public bool IsManual { get; }
        public List<ImpactAreaFrequencyFunctionRowItem> ImpactAreaFrequencyRows { get; } = new List<ImpactAreaFrequencyFunctionRowItem>();

        #endregion
        #region Constructors

        public AggregatedStageDamageElement(String name, string lastEditDate, string description,int selectedWSE, int selectedStructs, 
            int indexPointsID, List<StageDamageCurve> curves, List<ImpactAreaFrequencyFunctionRowItem> impactAreaRows, bool isManual, int id) 
            : base(name, lastEditDate, description, id)
        {
            ImpactAreaFrequencyRows = impactAreaRows;            

            Curves = curves;
            IsManual = isManual;
            SelectedWSE = selectedWSE;
            SelectedStructures = selectedStructs;
            SelectedIndexPoints = indexPointsID;

            AddActions();
        }

        public AggregatedStageDamageElement(XElement elementXML, int id):base(elementXML, id)
        {
            SelectedStructures = Convert.ToInt32( elementXML.Attribute(SELECTED_STRUCTURES).Value);
            SelectedIndexPoints = Convert.ToInt32(elementXML.Attribute(SELECTED_INDEX_POINTS).Value);
            SelectedWSE = Convert.ToInt16(elementXML.Attribute(selected))
            IsManual = Convert.ToBoolean(elementXML.Attribute(IS_MANUAL).Value);

            XElement stageDamageCurves = elementXML.Element(STAGE_DAMAGE_CURVES);
            IEnumerable<XElement> curves = stageDamageCurves.Elements(STAGE_DAMAGE_CURVE);
            foreach (XElement curve in curves)
            {
                Curves.Add(new StageDamageCurve(curve));
            }

            IEnumerable<XElement> impAreaRows = elementXML.Elements(IMPACT_AREA_ROWS);
            foreach (XElement impAreaRow in impAreaRows)
            {
                ImpactAreaFrequencyRows.Add(new ImpactAreaFrequencyFunctionRowItem(impAreaRow));
            }

            AddActions();
        }
        #endregion
        #region Voids
        private void AddActions()
        {
            AddDefaultActions(EditDamageCurve, StringConstants.EDIT_STAGE_DAMAGE_MENU);

            NamedAction exportDetails = new NamedAction(this);
            exportDetails.Header = StringConstants.EXPORT_STAGE_DAMAGE_MENU;
            exportDetails.Action = ExportDetails;
            Actions.Add(exportDetails);
        }

        public void EditDamageCurve(object arg1, EventArgs arg2)
        {    
            //create action manager
            EditorActionManager actionManager = new EditorActionManager()
                 .WithSiblingRules(this);

            AggregatedStageDamageEditorVM vm = new AggregatedStageDamageEditorVM(this, actionManager);

            string title = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(title, vm, "EditStageDamageElement" + Name);
            Navigate(tab, false, true);
        }

        public void ExportDetails(object sender, EventArgs e)
        {
            if (IsManual)
            {
                MessageBox.Show("Details cannot be exported for manually entered stage damages.", "Unable to Export Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                //todo: Richard will write the logic that loads the table with the desired details.
                string path = Storage.Connection.Instance.ProjectDirectory + "\\" + Name + "_Structure_Detail.csv";
                using (var sw = new StreamWriter(path))
                {
                    WriteHeaderMetaData(sw);
                    sw.WriteLine();
                    WriteStructureTable(sw);
                }// the streamwriter WILL be closed and flushed here, even if an exception is thrown.
            }
        }
        private void WriteStructureTable(StreamWriter sw)
        {
            List<ChildElement> structureInventories = StudyCache.GetChildElementsOfType(typeof(InventoryElement));
            List<ChildElement> wseElems = StudyCache.GetChildElementsOfType(typeof(HydraulicElement));

            //I thought i would create a datatable object so that i could define columns and then create rows. I think this will make it 
            //easier to update and maintain than just trying to write it out as a big string or something like that.
            
            //i thought we could grab the structure data from the inventory element form the study cache, but i don't see that option. We might
            //have to use the persistence manager for structures and grab the table directly.

            //all this code might get pretty long. We might want to move this to another class. ie: WriteDetailsFile(AggregatedStageDamageElement elem)

            string structName = "struct name";
            string structValue = "struct value";
            DataTable dt = new DataTable();
            dt.Columns.Add(structName);
            dt.Columns.Add(structValue);

            //InventoryElement inv = (InventoryElement)structureInventories[0];
            //inv.StructureInventory.GetStructureData
            for(int i = 0;i<20;i++)
            {
                DataRow dataRow = dt.NewRow();
                dataRow[structName] = "testname " + i;
                dataRow[structValue] = "testValue" + i * 100;
                dt.Rows.Add(dataRow);
            }

            WriteDataTableToCSV(dt, sw);
        }

        private void WriteDataTableToCSV(DataTable dt, StreamWriter sw)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DataColumn col in dt.Columns)
            {
                sb.Append(col.ColumnName + ',');
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append(Environment.NewLine);

            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sb.Append(row[i].ToString() + ",");
                }

                sb.Append(Environment.NewLine);
            }
            sw.WriteLine(sb.ToString());
        }

        private void WriteHeaderMetaData(StreamWriter sw)
        {
            Study.StudyPropertiesElement propElem = StudyCache.GetStudyPropertiesElement();
            sw.Write("Study: ");
            sw.WriteLine(Path.GetFileName(propElem.StudyPath));
            
            sw.Write("Description: ");
            sw.WriteLine(propElem.Description);

            sw.Write("Created: ");
            sw.WriteLine(propElem.CreatedDate);
        }

        public override XElement ToXML()
        {
            XElement stageDamageElem = new XElement(StringConstants.ELEMENT_XML_TAG);
            stageDamageElem.Add(CreateHeaderElement());

            stageDamageElem.SetAttributeValue(SELECTED_STRUCTURES, SelectedStructures);
            stageDamageElem.SetAttributeValue(SELECTED_INDEX_POINTS, SelectedIndexPoints);
            //todo: save the selected wse
            stageDamageElem.SetAttributeValue(IS_MANUAL, IsManual);

            XElement curveElements = new XElement(STAGE_DAMAGE_CURVES);
            foreach (StageDamageCurve curve in Curves)
            {
                curveElements.Add(curve.WriteToXML());
            }
            stageDamageElem.Add(curveElements);

            XElement impactAreaRowsElem = new XElement(IMPACT_AREA_ROWS);
            foreach (ImpactAreaFrequencyFunctionRowItem row in ImpactAreaFrequencyRows)
            {
                impactAreaRowsElem.Add(row.WriteToXML());
            }
            stageDamageElem.Add(impactAreaRowsElem);

            return stageDamageElem;
        }

        public bool Equals(AggregatedStageDamageElement elem)
        {
            bool isEqual = true;

            if (!AreHeaderDataEqual(elem))
            {
                isEqual = false;
            }
            if (SelectedStructures != elem.SelectedStructures)
            {
                isEqual = false;
            }
            if (SelectedIndexPoints != elem.SelectedIndexPoints)
            {
                isEqual = false;
            }
            if (IsManual != elem.IsManual)
            {
                isEqual = false;
            }

            for(int i = 0;i< Curves.Count; i++)
            {
                if (Curves[i].Equals(elem.Curves[i]))
                {
                    isEqual = false;
                    break;
                }
            }

            for(int i = 0;i< ImpactAreaFrequencyRows.Count;i++)
            {
                //todo: I don't want to write this equals method right now
                //if(!ImpactAreaFrequencyRows[i].Equals(elem.ImpactAreaFrequencyRows[i]))
                //{
                //    isEqual = false;
                //    break;
                //}
            }

            return isEqual;
        }
        #endregion

    }
}
