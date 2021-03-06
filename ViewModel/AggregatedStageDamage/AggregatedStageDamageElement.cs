using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Hydraulics;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class AggregatedStageDamageElement : ChildElement
    {
        #region Properties
        public bool CanEdit { get; }
        public int SelectedWSE { get; set; }
        public int SelectedStructures { get; set; }
        public List<StageDamageCurve> Curves { get; }
        public bool IsManual { get; }
        public List<ImpactAreaFrequencyFunctionRowItem> ImpactAreaFrequencyRows { get; }

        #endregion
        #region Constructors

        public AggregatedStageDamageElement(String name, string lastEditDate, string description,int selectedWSE, int selectedStructs, 
            List<StageDamageCurve> curves, List<ImpactAreaFrequencyFunctionRowItem> impactAreaRows, bool isManual, int id) : base(id)
        {
            ImpactAreaFrequencyRows = impactAreaRows;
            LastEditDate = lastEditDate;
            CustomTreeViewHeader = new CustomHeaderVM(name)
            {
                ImageSource = ImageSources.AGGREGATED_STAGE_DAMAGE_IMAGE,
                Tooltip = StringConstants.CreateLastEditTooltip(lastEditDate)
            };

            Description = description;
            if (Description == null)
            {
                Description = "";
            }

            Name = name;
            Curves = curves;
            IsManual = isManual;
            SelectedWSE = selectedWSE;
            SelectedStructures = selectedStructs;

            NamedAction editDamageCurve = new NamedAction();
            editDamageCurve.Header = StringConstants.EDIT_STAGE_DAMAGE_MENU;
            editDamageCurve.Action = EditDamageCurve;

            NamedAction removeDamageCurve = new NamedAction();
            removeDamageCurve.Header = StringConstants.REMOVE_MENU;
            removeDamageCurve.Action = RemoveElement;

            NamedAction renameDamageCurve = new NamedAction(this);
            renameDamageCurve.Header = StringConstants.RENAME_MENU;
            renameDamageCurve.Action = Rename;

            NamedAction exportDetails = new NamedAction(this);
            exportDetails.Header = StringConstants.EXPORT_STAGE_DAMAGE_MENU;
            exportDetails.Action = ExportDetails;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(editDamageCurve);
            localActions.Add(removeDamageCurve);
            localActions.Add(renameDamageCurve);
            localActions.Add(exportDetails);

            Actions = localActions;
        }

        #endregion
        #region Voids
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            AggregatedStageDamageElement elem = (AggregatedStageDamageElement)elementToClone;
            return new AggregatedStageDamageElement(elem.Name, elem.LastEditDate, elem.Description, elem.SelectedWSE, elem.SelectedStructures, elem.Curves,elem.ImpactAreaFrequencyRows, elem.IsManual, elem.ID);
        }

        public void EditDamageCurve(object arg1, EventArgs arg2)
        {    
            //create action manager
            EditorActionManager actionManager = new EditorActionManager()
                 .WithSiblingRules(this);

            AggregatedStageDamageEditorVM vm = new AggregatedStageDamageEditorVM(this, actionManager);

            string title = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(title, vm, "EditStageDamageElement" + Name,true, false);
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

        #endregion

    }
}
