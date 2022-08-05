using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.ImpactArea
{
    public class ImpactAreaElement : ChildElement
    {
        #region Notes
        #endregion
        #region Fields
        private static String IMPACT_AREAS_TAG = "ImpactAreas";
        private static String NAME_TAG = "Name";
        private static String DESCRIPTION_TAG = "Description";
        private static String LAST_EDIT_DATE_TAG = "LastEditDate";
        private static String IMPACT_AREA_ROWS_TAG = "ImpactAreaRows";

        #endregion
        #region Properties

        public List<ImpactAreaRowItem> ImpactAreaRows { get; } = new List<ImpactAreaRowItem>();

        #endregion
        #region Constructors
        public ImpactAreaElement(string userdefinedname, string description, List<ImpactAreaRowItem> collectionOfRows, int id) : base(id)
        {      
            Name = userdefinedname;
            CustomTreeViewHeader = new CustomHeaderVM(Name, ImageSources.IMPACT_AREAS_IMAGE);
            Description = description;
            ImpactAreaRows = collectionOfRows;
            AddActions();
        }

        public ImpactAreaElement(XElement xmlString, int id) : base(id)
        {
            ID = id;
            XDocument doc = XDocument.Parse(xmlString);
            XElement impactAreaElem = doc.Element(IMPACT_AREAS_TAG);
            Name = impactAreaElem.Attribute(NAME_TAG).Value;
            CustomTreeViewHeader = new CustomHeaderVM(Name, ImageSources.IMPACT_AREAS_IMAGE);

            Description = impactAreaElem.Attribute(DESCRIPTION_TAG).Value;
            LastEditDate = impactAreaElem.Attribute(LAST_EDIT_DATE_TAG).Value;

            XElement rowsElem = impactAreaElem.Element(IMPACT_AREA_ROWS_TAG);
            IEnumerable<XElement> rowElems = rowsElem.Elements(ImpactAreaRowItem.ROW_ITEM_TAG);
            foreach (XElement nameElem in rowElems)
            {
                ImpactAreaRows.Add(new ImpactAreaRowItem(nameElem));
            }

            AddActions();
        }

        private void AddActions()
        {
            NamedAction edit = new NamedAction();
            edit.Header = StringConstants.EDIT_IMPACT_AREA_SET_MENU;
            edit.Action = Edit;

            NamedAction removeImpactArea = new NamedAction();
            removeImpactArea.Header = StringConstants.REMOVE_MENU;
            removeImpactArea.Action = RemoveElement;

            NamedAction renameElement = new NamedAction(this);
            renameElement.Header = StringConstants.RENAME_MENU;
            renameElement.Action = Rename;

            List<NamedAction> localactions = new List<NamedAction>();
            localactions.Add(edit);
            localactions.Add(removeImpactArea);
            localactions.Add(renameElement);

            Actions = localactions;
        }

        private string GetScenariosToDeleteMessage()
        {
            List<IASElementSet> iasElems = StudyCache.GetChildElementsOfType<IASElementSet>();
            string scenarioMessage = null;
            if (iasElems.Count > 0)
            {
                List<string> scenarios = new List<string>();
                foreach (IASElementSet set in iasElems)
                {
                    scenarios.Add(set.Name);
                }

                string scenarioCSV = string.Join(", ", scenarios);

                scenarioMessage = "Deleting the impact area will also delete all existing scenarios: " + scenarioCSV;            
            }
            return scenarioMessage;
        }

        private string GetStageDamagesToDeleteMessage()
        {
            List<AggregatedStageDamageElement> stageDamageElems = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
            string stageDamageMessage = null;
            List<string> stageDamages = new List<string>();
            if (stageDamageElems.Count > 0)
            {
                foreach (AggregatedStageDamageElement stageDamage in stageDamageElems)
                {
                    stageDamages.Add(stageDamage.Name);
                }

                string stageDamageCSV = string.Join(", ", stageDamages);
                stageDamageMessage = "Deleting the impact area will also delete all existing stage damage relationships: " + stageDamageCSV;
            }
            return stageDamageMessage;
        }

        public void RemoveElement(object sender, EventArgs e)
        {
            string scenariosToDeleteMessage = GetScenariosToDeleteMessage();

            string stageDamagesToDeleteMessage = GetStageDamagesToDeleteMessage();

            StringBuilder sb = new StringBuilder();

            if(scenariosToDeleteMessage != null)
            {
                sb.AppendLine(scenariosToDeleteMessage);
            }

            if(stageDamagesToDeleteMessage != null)
            {
                sb.AppendLine(stageDamagesToDeleteMessage);
            }

            if(scenariosToDeleteMessage == null && stageDamagesToDeleteMessage == null)
            {
                ShowDefaultDeleteMessage();
            }
            else
            {
                var result = MessageBox.Show(sb.ToString() 
                     + Environment.NewLine + "Do you want to continue with the delete?", "Do You Want to Continue", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Saving.PersistenceFactory.GetImpactAreaManager().Remove(this);
                    //remove the directory
                    if(Directory.Exists(Connection.Instance.ImpactAreaDirectory + "\\" + Name))
                    {
                        Directory.Delete(Connection.Instance.ImpactAreaDirectory + "\\" + Name, true);
                    }
                    DeleteAllScenariosAndStageDamages();
                }
            }
        }

        private void DeleteAllScenariosAndStageDamages()
        {
            //delete the scenarios
            List<IASElementSet> iasElems = StudyCache.GetChildElementsOfType<IASElementSet>();
            foreach (IASElementSet set in iasElems)
            {
                Saving.PersistenceFactory.GetIASManager().Remove(set);
            }

            //delete the stage damages
            List<AggregatedStageDamageElement> stageDamageElems = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
            foreach (AggregatedStageDamageElement elem in stageDamageElems)
            {
                Saving.PersistenceFactory.GetStageDamageManager().Remove(elem);
            }
        }

        private void ShowDefaultDeleteMessage()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete '" + Name + "'?", "Delete " + Name + "?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Saving.PersistenceFactory.GetImpactAreaManager().Remove(this);
            }
            //remove the directory
            if (Directory.Exists(Connection.Instance.ImpactAreaDirectory + "\\" + Name))
            {
                Directory.Delete(Connection.Instance.ImpactAreaDirectory + "\\" + Name, true);
            }
        }

        #endregion
        #region Voids
        private void Edit(object arg1, EventArgs arg2)
        {
            //create an observable collection of all the available paths
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);

            ImpactAreaImporterVM vm = new ImpactAreaImporterVM(this, ImpactAreaRows, actionManager);
            string header = StringConstants.EDIT_IMPACT_AREA_SET_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, vm, header + Name);
            Navigate(tab, false,false);
        }

        #endregion
        #region Functions 
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            ImpactAreaElement elem = (ImpactAreaElement)elementToClone;
            return new ImpactAreaElement(elem.Name, elem.Description,elem.ImpactAreaRows, elem.ID);
        }

        public override XElement ToXML()
        {
            XElement impactAreaElem = new XElement(IMPACT_AREAS_TAG);
            impactAreaElem.SetAttributeValue(NAME_TAG, Name);
            impactAreaElem.SetAttributeValue(DESCRIPTION_TAG, Description);
            impactAreaElem.SetAttributeValue(LAST_EDIT_DATE_TAG, LastEditDate);

            XElement impactAreaRows = new XElement(IMPACT_AREA_ROWS_TAG);
            foreach (ImpactAreaRowItem row in ImpactAreaRows)
            {
                impactAreaRows.Add(row.ToXML());
            }

            impactAreaElem.Add(impactAreaRows);

            return impactAreaElem;
        }

        public override void Rename(object sender, EventArgs e)
        {
            string originalName = Name;
            RenameVM renameViewModel = new RenameVM(this, CloneElement);
            string header = "Rename";
            DynamicTabVM tab = new DynamicTabVM(header, renameViewModel, "Rename", false, false);
            Navigate(tab);
            if (!renameViewModel.WasCanceled)
            {
                string newName = renameViewModel.Name;
                //rename the folders in the study.
                if (!originalName.Equals(newName))
                {
                    try
                    {
                        string sourceFilePath = Connection.Instance.ImpactAreaDirectory + "\\" + originalName;
                        string destinationFilePath = Connection.Instance.ImpactAreaDirectory + "\\" + newName;
                        Directory.Move(sourceFilePath, destinationFilePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Renaming the impact area directory failed.\n" + ex.Message, "Rename Failed", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        #endregion


    }
}
