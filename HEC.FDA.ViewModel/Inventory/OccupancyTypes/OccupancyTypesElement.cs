using HEC.FDA.ViewModel.Inventory.OccupancyTypes.SQLiteSaving;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

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

        public OccupancyTypesElement(string name, string lastEditDate, string description, List<OccupancyType> listOfOccTypes, int id) : base(name, lastEditDate, description, id)
        {
            ListOfOccupancyTypes = listOfOccTypes;
            AddDefaultActions(EditOccupancyTypes, StringConstants.EDIT_OCCTYPE_MENU);
            AddExportActions();
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
            AddExportActions();
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

        public List<String> GetUniqueDamageCategories()
        {
            HashSet<String> dams = new HashSet<String>();
            foreach (OccupancyType ot in ListOfOccupancyTypes)
            {
                dams.Add(ot.DamageCategory);
            }
            return dams.ToList<String>();
        }

        private void AddExportActions()
        {
            NamedAction exportToXML = new()
            {
                Header = "Export to XML...",
                Action = ExportToXML
            };
            Actions.Insert(1, exportToXML);
            NamedAction exportToSQLite = new()
            {
                Header = "Export to SQLite...",
                Action = ExportToSQLite
            };
            Actions.Insert(2, exportToSQLite);
        }

        private void ExportToXML(object arg1, EventArgs args)
        {
            var sfd = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save XML",
                Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
                FileName = $"{Name}.xml",
                DefaultExt = ".xml",
                AddExtension = true,
                OverwritePrompt = true,
                RestoreDirectory = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (sfd.ShowDialog() == false)
                return;

            try
            {
                this.ToXML().Save(sfd.FileName);
                System.Windows.MessageBox.Show("Saved.", "XML", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to save XML:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void ExportToSQLite(object arg1, EventArgs args)
        {
            var sfd = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save SQLite",
                Filter = "SQLite files (*.sqlite)|*.sqlite|All files (*.*)|*.*",
                FileName = $"{Name}.sqlite",
                DefaultExt = ".sqlite",
                AddExtension = true,
                OverwritePrompt = true,
                RestoreDirectory = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (sfd.ShowDialog() == false)
                return;

            try
            {
                string dbPath = sfd.FileName;
                using OccupancyTypeSaver saver = new(dbPath);
                foreach (var occtype in ListOfOccupancyTypes)
                    saver.SaveToSQLite(occtype);
                System.Windows.MessageBox.Show("Saved.", "SQLite", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to save SQLite:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
    }
}
